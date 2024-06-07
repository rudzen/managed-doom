﻿//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//


using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using DrippyAL;
using MeltySynth;
using ManagedDoom.Audio;
using ManagedDoom.Config;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Silk;

public sealed class SilkMusic : IMusic, IDisposable
{
    private readonly ConfigValues config;
    private readonly Wad wad;

    private MusStream stream;
    private Bgm current;

    public SilkMusic(ConfigValues config, GameContent content, AudioDevice device, string sfPath)
    {
        try
        {
            Console.Write("Initialize music: ");
            var start = Stopwatch.GetTimestamp();

            this.config = config;
            this.wad = content.Wad;

            stream = new MusStream(this, config, device, sfPath);
            current = Bgm.NONE;

            Console.WriteLine($"OK [{Stopwatch.GetElapsedTime(start)}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            Dispose();
            ExceptionDispatchInfo.Throw(e);
        }
    }

    public void StartMusic(Bgm bgm, bool loop)
    {
        if (bgm == current)
            return;

        var lumpName = $"D_{DoomInfo.BgmNames[(int)bgm].ToString().ToUpper()}";
        var data = wad.ReadLump(lumpName);
        var decoder = ReadData(data, loop);
        stream.SetDecoder(decoder);

        current = bgm;
    }

    private static IDecoder ReadData(byte[] data, bool loop)
    {
        var dataSpan = data.AsSpan();
        var isMus = dataSpan[..MusDecoder.MusHeader.Length].SequenceEqual(MusDecoder.MusHeader);

        if (isMus)
            return new MusDecoder(data, loop);

        var isMidi = dataSpan[..MidiDecoder.MidiHeader.Length].SequenceEqual(MidiDecoder.MidiHeader);

        if (isMidi)
            return new MidiDecoder(data, loop);

        throw new Exception("Unknown format!");
    }

    public void Dispose()
    {
        Console.WriteLine("Shutdown music.");

        if (stream is null)
            return;

        stream.Dispose();
        stream = null;
    }

    public int MaxVolume => 15;

    public int Volume
    {
        get => config.AudioMusicVolume;
        set => config.AudioMusicVolume = value;
    }

    private sealed class MusStream : IDisposable
    {
        private const int latency = 200;
        private const int blockLength = 2048;

        private readonly SilkMusic parent;
        private readonly ConfigValues config;

        private readonly Synthesizer synthesizer;

        private AudioStream audioStream;

        private readonly float[] left;
        private readonly float[] right;

        private volatile IDecoder current;
        private volatile IDecoder reserved;

        public MusStream(SilkMusic parent, ConfigValues config, AudioDevice device, string sfPath)
        {
            this.parent = parent;
            this.config = config;

            config.AudioMusicVolume = Math.Clamp(config.AudioMusicVolume, 0, parent.MaxVolume);

            var settings = new SynthesizerSettings(MusDecoder.SampleRate)
            {
                BlockSize = MusDecoder.BlockLength,
                EnableReverbAndChorus = config.AudioMusicEffect
            };

            synthesizer = new Synthesizer(sfPath, settings);

            left = new float[blockLength];
            right = new float[blockLength];

            audioStream = new AudioStream(device, MusDecoder.SampleRate, 2, true, latency, blockLength);
        }

        public void SetDecoder(IDecoder decoder)
        {
            reserved = decoder;

            if (audioStream.State == PlaybackState.Stopped)
                audioStream.Play(OnGetData);
        }

        private void OnGetData(short[] samples)
        {
            if (reserved != current)
            {
                synthesizer.Reset();
                current = reserved;
            }

            var a = 32768 * (2.0F * config.AudioMusicVolume / parent.MaxVolume);

            current.RenderWaveform(synthesizer, left, right);

            var pos = 0;

            for (var t = 0; t < blockLength; t++)
            {
                var sampleLeft = (int)(a * left[t]);
                sampleLeft = Math.Clamp(sampleLeft, short.MinValue, short.MaxValue);

                var sampleRight = (int)(a * right[t]);
                sampleRight = Math.Clamp(sampleRight, short.MinValue, short.MaxValue);

                samples[pos++] = (short)sampleLeft;
                samples[pos++] = (short)sampleRight;
            }
        }

        public void Dispose()
        {
            if (audioStream is null)
                return;

            audioStream.Stop();
            audioStream.Dispose();
            audioStream = null;
        }
    }

    private interface IDecoder
    {
        void RenderWaveform(Synthesizer synthesizer, Span<float> left, Span<float> right);
    }

    private sealed class MusDecoder : IDecoder
    {
        public const int SampleRate = 44100;
        public const int BlockLength = SampleRate / 140;

        public static readonly byte[] MusHeader =
        [
            (byte)'M',
            (byte)'U',
            (byte)'S',
            0x1A
        ];

        private readonly byte[] data;
        private readonly bool loop;

        private int scoreLength;
        private readonly int scoreStart;
        private int channelCount;
        private int channelCount2;
        private readonly int instrumentCount;
        private readonly int[] instruments;

        private readonly MusEvent[] events;
        private int eventCount;

        private readonly int[] lastVolume;
        private int p;
        private int delay;

        private int blockWrote;

        public MusDecoder(byte[] data, bool loop)
        {
            CheckHeader(data);

            this.data = data;
            this.loop = loop;

            scoreLength = BitConverter.ToUInt16(data, 4);
            scoreStart = BitConverter.ToUInt16(data, 6);
            channelCount = BitConverter.ToUInt16(data, 8);
            channelCount2 = BitConverter.ToUInt16(data, 10);
            instrumentCount = BitConverter.ToUInt16(data, 12);
            instruments = new int[instrumentCount];
            for (var i = 0; i < instruments.Length; i++)
                instruments[i] = BitConverter.ToUInt16(data, 16 + 2 * i);

            events = new MusEvent[128];
            for (var i = 0; i < events.Length; i++)
                events[i] = new MusEvent();

            eventCount = 0;

            lastVolume = new int[16];

            Reset();

            blockWrote = BlockLength;
        }

        private static void CheckHeader(ReadOnlySpan<byte> data)
        {
            for (var p = 0; p < MusHeader.Length; p++)
                if (data[p] != MusHeader[p])
                    throw new Exception("Invalid format!");
        }

        public void RenderWaveform(Synthesizer synthesizer, Span<float> left, Span<float> right)
        {
            var wrote = 0;
            while (wrote < left.Length)
            {
                if (blockWrote == synthesizer.BlockSize)
                {
                    ProcessMidiEvents(synthesizer);
                    blockWrote = 0;
                }

                var srcRem = synthesizer.BlockSize - blockWrote;
                var dstRem = left.Length - wrote;
                var rem = Math.Min(srcRem, dstRem);

                synthesizer.Render(left.Slice(wrote, rem), right.Slice(wrote, rem));

                blockWrote += rem;
                wrote += rem;
            }
        }

        private void ProcessMidiEvents(Synthesizer synthesizer)
        {
            if (delay > 0)
                delay--;

            if (delay != 0)
                return;

            delay = ReadSingleEventGroup();
            SendEvents(synthesizer);

            if (delay != -1)
                return;

            synthesizer.NoteOffAll(false);

            if (loop)
                Reset();
        }

        private void Reset()
        {
            for (var i = 0; i < lastVolume.Length; i++)
                lastVolume[i] = 0;

            p = scoreStart;
            delay = 0;
        }

        private int ReadSingleEventGroup()
        {
            eventCount = 0;
            while (true)
            {
                var result = ReadSingleEvent();
                if (result == ReadResult.EndOfGroup)
                    break;

                if (result == ReadResult.EndOfFile)
                    return -1;
            }

            var time = 0;
            while (true)
            {
                var value = data[p++];
                time = time * 128 + (value & 127);
                if ((value & 128) == 0)
                    break;
            }

            return time;
        }

        private ReadResult ReadSingleEvent()
        {
            var channelNumber = data[p] & 0xF;

            switch (channelNumber)
            {
                case 15:
                    channelNumber = 9;
                    break;
                case >= 9:
                    channelNumber++;
                    break;
            }

            var eventType = (data[p] & 0x70) >> 4;
            var last = (data[p] >> 7) != 0;

            p++;

            var me = events[eventCount];
            eventCount++;

            switch (eventType)
            {
                case 0: // RELEASE NOTE
                    me.Type = 0;
                    me.Channel = channelNumber;

                    var releaseNote = data[p++];

                    me.Data1 = releaseNote;
                    me.Data2 = 0;

                    break;

                case 1: // PLAY NOTE
                    me.Type = 1;
                    me.Channel = channelNumber;

                    var playNote = data[p++];
                    var noteNumber = playNote & 127;
                    var noteVolume = (playNote & 128) != 0 ? data[p++] : -1;

                    me.Data1 = noteNumber;
                    if (noteVolume == -1)
                        me.Data2 = lastVolume[channelNumber];
                    else
                    {
                        me.Data2 = noteVolume;
                        lastVolume[channelNumber] = noteVolume;
                    }

                    break;

                case 2: // PITCH WHEEL
                    me.Type = 2;
                    me.Channel = channelNumber;

                    var pitchWheel = data[p++];

                    var pw2 = (pitchWheel << 7) / 2;
                    var pw1 = pw2 & 127;
                    pw2 >>= 7;
                    me.Data1 = pw1;
                    me.Data2 = pw2;

                    break;

                case 3: // SYSTEM EVENT
                    me.Type = 3;
                    me.Channel = channelNumber;

                    var systemEvent = data[p++];
                    me.Data1 = systemEvent;
                    me.Data2 = 0;

                    break;

                case 4: // CONTROL CHANGE
                    me.Type = 4;
                    me.Channel = channelNumber;

                    var controllerNumber = data[p++];
                    var controllerValue = data[p++];

                    me.Data1 = controllerNumber;
                    me.Data2 = controllerValue;

                    break;

                case 6: // END OF FILE
                    return ReadResult.EndOfFile;

                default:
                    throw new Exception("Unknown event type!");
            }

            return last
                ? ReadResult.EndOfGroup
                : ReadResult.Ongoing;
        }

        private void SendEvents(Synthesizer synthesizer)
        {
            for (var i = 0; i < eventCount; i++)
            {
                var me = events[i];
                switch (me.Type)
                {
                    case 0: // RELEASE NOTE
                        synthesizer.NoteOff(me.Channel, me.Data1);
                        break;

                    case 1: // PLAY NOTE
                        synthesizer.NoteOn(me.Channel, me.Data1, me.Data2);
                        break;

                    case 2: // PITCH WHEEL
                        synthesizer.ProcessMidiMessage(me.Channel, 0xE0, me.Data1, me.Data2);
                        break;

                    case 3: // SYSTEM EVENT
                        switch (me.Data1)
                        {
                            case 11: // ALL NOTES OFF
                                synthesizer.NoteOffAll(me.Channel, false);
                                break;

                            case 14: // RESET ALL CONTROLS
                                synthesizer.ResetAllControllers(me.Channel);
                                break;
                        }

                        break;

                    case 4: // CONTROL CHANGE
                        switch (me.Data1)
                        {
                            case 0: // PROGRAM CHANGE
                                synthesizer.ProcessMidiMessage(me.Channel, 0xC0, me.Data2, 0);
                                break;

                            case 1: // BANK SELECTION
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x00, me.Data2);
                                break;

                            case 2: // MODULATION
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x01, me.Data2);
                                break;

                            case 3: // VOLUME
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x07, me.Data2);
                                break;

                            case 4: // PAN
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x0A, me.Data2);
                                break;

                            case 5: // EXPRESSION
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x0B, me.Data2);
                                break;

                            case 6: // REVERB
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x5B, me.Data2);
                                break;

                            case 7: // CHORUS
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x5D, me.Data2);
                                break;

                            case 8: // PEDAL
                                synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x40, me.Data2);
                                break;
                        }

                        break;
                }
            }
        }

        private sealed class MusEvent
        {
            public int Type { get; set; }
            public int Channel { get; set; }
            public int Data1 { get; set; }
            public int Data2 { get; set; }
        }

        private enum ReadResult
        {
            Ongoing,
            EndOfGroup,
            EndOfFile
        }
    }

    private sealed class MidiDecoder : IDecoder
    {
        public static byte[] MidiHeader =>
        [
            (byte)'M',
            (byte)'T',
            (byte)'h',
            (byte)'d'
        ];

        private readonly MidiFile midi;
        private MidiFileSequencer sequencer;

        private readonly bool loop;

        public MidiDecoder(byte[] data, bool loop)
        {
            midi = new MidiFile(new MemoryStream(data));
            this.loop = loop;
        }

        public void RenderWaveform(Synthesizer synthesizer, Span<float> left, Span<float> right)
        {
            if (sequencer is null)
            {
                sequencer = new MidiFileSequencer(synthesizer);
                sequencer.Play(midi, loop);
            }

            sequencer.Render(left, right);
        }
    }
}