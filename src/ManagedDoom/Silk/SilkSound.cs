//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
// Copyright (C)      2024 Rudy Alex Kohn
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
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using DrippyAL;
using ManagedDoom.Audio;
using ManagedDoom.Config;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.Wad;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Silk;

public sealed class SilkSound : ISound
{
    private const int channelCount = 8;

    private static readonly float fastDecay = (float)Math.Pow(0.5, 1.0 / (35 / 5));
    private static readonly float slowDecay = (float)Math.Pow(0.5, 1.0 / 35);

    private const float clipDist = 1200;
    private const float closeDist = 160;
    private const float attenuator = clipDist - closeDist;

    private readonly ConfigValues config;

    private AudioClip?[]? buffers;
    private readonly float[] amplitudes;

    private readonly DoomRandom? random;

    private AudioChannel?[]? channels;
    private readonly ChannelInfo[] infos;

    private AudioChannel? uiChannel;
    private Sfx uiReserved;

    private Mobj listener;

    private float masterVolumeDecay;

    private long lastUpdate;

    [SkipLocalsInit]
    public SilkSound(ConfigValues config, IGameContent content, AudioDevice device)
    {
        try
        {
            Console.Write("Initialize sound: ");
            var start = Stopwatch.GetTimestamp();

            this.config = config;

            config.AudioSoundVolume = Math.Clamp(config.AudioSoundVolume, 0, MaxVolume);

            var sfxNames = DoomInfo.SfxNames.AsSpan();

            buffers = new AudioClip[sfxNames.Length];
            amplitudes = new float[sfxNames.Length];

            random = config.AudioRandomPitch ? new DoomRandom() : null;

            for (var i = 0; i < sfxNames.Length; i++)
            {
                var name = $"DS{sfxNames[i].ToString().ToUpper()}";
                var lump = content.Wad.GetLumpNumber(name);

                if (lump == -1)
                    continue;

                var samples = GetSamples(content.Wad, lump, name, out var sampleRate, out var sampleCount);

                if (samples.IsEmpty)
                    continue;

                buffers[i] = new AudioClip(device, sampleRate, 1, samples);
                amplitudes[i] = GetAmplitude(samples, sampleRate, sampleCount);
            }

            channels = new AudioChannel[channelCount];
            infos = new ChannelInfo[channelCount];
            for (var i = 0; i < channels.Length; i++)
            {
                channels[i] = new AudioChannel(device);
                infos[i] = new ChannelInfo();
            }

            uiChannel = new AudioChannel(device);
            uiReserved = Sfx.NONE;

            masterVolumeDecay = (float)config.AudioSoundVolume / MaxVolume;

            lastUpdate = 0;

            Console.WriteLine($"OK [{Stopwatch.GetElapsedTime(start)}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            Dispose();
            ExceptionDispatchInfo.Throw(e);
        }
    }

    private static Span<byte> GetSamples(Wad wad, int lumpNumber, string lumpName, out int sampleRate, out int sampleCount)
    {
        // TODO (rudzen) : modify to not Span<T> instead of real array
        var data = wad.ReadLump(lumpName);

        if (data.Length < 8)
        {
            sampleRate = -1;
            sampleCount = -1;
            return [];
        }

        sampleRate = BitConverter.ToUInt16(data, 2);
        sampleCount = BitConverter.ToInt32(data, 4);

        var offset = 8;

        if (ContainsDmxPadding(data))
        {
            offset += 16;
            sampleCount -= 32;
        }

        return sampleCount > 0
            ? data.AsSpan(offset, sampleCount)
            : [];
    }

    /// <summary>
    /// Checks if the provided audio data contains DMX padding.
    /// DMX padding is a specific pattern in Doom sound data where the first and last 16 samples are the same.
    /// This method has been optimized to use Span&lt;T&gt; for efficient data access and manipulation.
    /// https://doomwiki.org/wiki/Sound
    /// </summary>
    private static bool ContainsDmxPadding(ReadOnlySpan<byte> data)
    {
        var sampleCount = BitConverter.ToInt32(data.Slice(4, 4));
        if (sampleCount < 32)
            return false;

        var first16Samples = data.Slice(8, 16);
        var firstSample = first16Samples[0];
        foreach (var sample in first16Samples[1..])
        {
            if (sample != firstSample)
                return false;
        }

        var last16Samples = data.Slice(8 + sampleCount - 16, 16);
        var lastSample = last16Samples[0];
        foreach (var sample in last16Samples[1..])
        {
            if (sample != lastSample)
                return false;
        }

        return true;
    }

    private static float GetAmplitude(Span<byte> samples, int sampleRate, int sampleCount)
    {
        var max = 0;

        if (sampleCount <= 0)
            return (float)max / 128;

        var count = Math.Min(sampleRate / 5, sampleCount);
        for (var t = 0; t < count; t++)
        {
            var a = samples[t] - 128;
            if (a < 0)
                a = -a;

            if (a > max)
                max = a;
        }

        return (float)max / 128;
    }

    public void SetListener(Mobj listener)
    {
        this.listener = listener;
    }

    public void Update()
    {
        var now = Stopwatch.GetTimestamp();
        // we don't need to update all the time, so be a bit liberal (fx for timedemo)
        if (Stopwatch.GetElapsedTime(lastUpdate) - Stopwatch.GetElapsedTime(now) < TimeSpan.FromSeconds(0.01))
            return;

        var infoSpan = infos.AsSpan();
        var channelSpan = channels.AsSpan();

        ref var infoRef = ref MemoryMarshal.GetReference(infoSpan);
        ref var channelRef = ref MemoryMarshal.GetReference(channelSpan);

        for (var i = 0; i < infoSpan.Length; i++)
        {
            ref var info = ref Unsafe.Add(ref infoRef, i);
            ref var channel = ref Unsafe.Add(ref channelRef, i);

            if (info.Playing != Sfx.NONE)
            {
                if (channel.State != PlaybackState.Stopped)
                {
                    if (info.Type == SfxType.Diffuse)
                        info.Priority *= slowDecay;
                    else
                        info.Priority *= fastDecay;

                    SetParam(channel, info);
                }
                else
                {
                    info.Playing = Sfx.NONE;
                    if (info.Reserved == Sfx.NONE)
                        info.Source = null;
                }
            }

            if (info.Reserved == Sfx.NONE)
                continue;

            if (info.Playing != Sfx.NONE)
                channel.Stop();

            channel.AudioClip = buffers[(int)info.Reserved];
            SetParam(channel, info);
            channel.Pitch = GetPitch(info.Type, info.Reserved);
            channel.Play();
            info.Playing = info.Reserved;
            info.Reserved = Sfx.NONE;
        }

        if (uiReserved != Sfx.NONE)
        {
            if (uiChannel.State == PlaybackState.Playing)
                uiChannel.Stop();

            uiChannel.Position = new Vector3(0, 0, -1);
            uiChannel.Volume = masterVolumeDecay;
            uiChannel.AudioClip = buffers[(int)uiReserved];
            uiChannel.Play();
            uiReserved = Sfx.NONE;
        }

        lastUpdate = now;
    }

    public void StartSound(Sfx sfx)
    {
        if (buffers[sfx.AsInt()] is null)
            return;

        uiReserved = sfx;
    }

    public void StartSound(Mobj mobj, Sfx sfx, SfxType type)
    {
        StartSound(mobj, sfx, type, 100);
    }

    public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume)
    {
        if (buffers[(int)sfx] is null)
            return;

        var x = (mobj.X - listener.X).ToFloat();
        var y = (mobj.Y - listener.Y).ToFloat();
        var dist = MathF.Sqrt(x * x + y * y);

        var priority = type == SfxType.Diffuse
            ? volume
            : amplitudes[(int)sfx] * GetDistanceDecay(dist) * volume;

        foreach (var info in infos.AsSpan())
        {
            if (info.Source != mobj || info.Type != type)
                continue;

            info.Reserved = sfx;
            info.Priority = priority;
            info.Volume = volume;
            return;
        }

        foreach (var info in infos.AsSpan())
        {
            if (info.Reserved != Sfx.NONE || info.Playing != Sfx.NONE)
                continue;

            info.Reserved = sfx;
            info.Priority = priority;
            info.Source = mobj;
            info.Type = type;
            info.Volume = volume;
            return;
        }

        var minPriority = float.MaxValue;
        var minChannel = -1;
        for (var i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            if (!(info.Priority < minPriority))
                continue;

            minPriority = info.Priority;
            minChannel = i;
        }

        if (priority >= minPriority)
        {
            var info = infos[minChannel];
            info.Reserved = sfx;
            info.Priority = priority;
            info.Source = mobj;
            info.Type = type;
            info.Volume = volume;
        }
    }

    public void StopSound(Mobj mobj)
    {
        foreach (var info in infos.AsSpan())
        {
            if (info.Source != mobj)
                continue;

            info.LastX = info.Source.X;
            info.LastY = info.Source.Y;
            info.Source = null;
            info.Volume /= 5;
        }
    }

    public void Reset()
    {
        random?.Clear();

        for (var i = 0; i < infos.Length; i++)
        {
            channels![i]!.Stop();
            infos[i].Clear();
        }

        listener = null!;
    }

    public void Pause()
    {
        for (var i = 0; i < infos.Length; i++)
        {
            var channel = channels![i];

            if (channel is { State: PlaybackState.Playing, AudioClip: not null }
                && channel.AudioClip.Duration - channel.PlayingOffset > TimeSpan.FromMilliseconds(200))
                channel.Pause();
        }
    }

    public void Resume()
    {
        for (var i = 0; i < infos.Length; i++)
        {
            var channel = channels![i]!;

            if (channel.State == PlaybackState.Paused)
                channel.Play();
        }
    }

    private void SetParam(AudioChannel sound, ChannelInfo info)
    {
        if (info.Type == SfxType.Diffuse)
        {
            sound.Position = new Vector3(0, 0, -1);
            sound.Volume = 0.01F * masterVolumeDecay * info.Volume;
        }
        else
        {
            var (sourceX, sourceY) = GetSourceXy(info);

            var x = (sourceX - listener.X).ToFloat();
            var y = (sourceY - listener.Y).ToFloat();

            if (Math.Abs(x) < 16 && Math.Abs(y) < 16)
            {
                sound.Position = new Vector3(0, 0, -1);
                sound.Volume = 0.01F * masterVolumeDecay * info.Volume;
            }
            else
            {
                var dist = MathF.Sqrt(x * x + y * y);
                var angle = MathF.Atan2(y, x) - (float)listener.Angle.ToRadian();
                sound.Position = new Vector3(-MathF.Sin(angle), 0, -MathF.Cos(angle));
                sound.Volume = 0.01F * masterVolumeDecay * GetDistanceDecay(dist) * info.Volume;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (Fixed, Fixed) GetSourceXy(ChannelInfo info)
    {
        return info.Source is null
            ? (info.LastX, info.LastY)
            : (info.Source.X, info.Source.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetDistanceDecay(float dist)
    {
        return dist < closeDist
            ? 1F
            : Math.Max((clipDist - dist) / attenuator, 0F);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float GetPitch(SfxType type, Sfx sfx)
    {
        if (random is null)
            return 1.0F;

        if (sfx is Sfx.ITEMUP or Sfx.TINK or Sfx.RADIO)
            return 1.0F;

        if (type == SfxType.Voice)
            return 1.0F + 0.075F * (random.Next() - 128) / 128;

        return 1.0F + 0.025F * (random.Next() - 128) / 128;
    }

    public void Dispose()
    {
        Console.WriteLine("Shutdown sound.");

        var channelSpan = channels.AsSpan();

        foreach (var channel in channelSpan)
        {
            if (channel is null)
                continue;

            channel.Stop();
            channel.Dispose();
        }

        var bufferSpan = buffers.AsSpan();

        foreach (var buffer in bufferSpan)
            buffer?.Dispose();

        if (uiChannel is not null)
        {
            uiChannel.Dispose();
            uiChannel = null;
        }

        channels = null;
        buffers = null;
    }

    public int MaxVolume => 15;

    public int Volume
    {
        get => config.AudioSoundVolume;
        set
        {
            config.AudioSoundVolume = value;
            masterVolumeDecay = (float)config.AudioSoundVolume / MaxVolume;
        }
    }

    private sealed class ChannelInfo
    {
        public Sfx Reserved { get; set; }
        public Sfx Playing { get; set; }
        public float Priority { get; set; }

        public Mobj? Source { get; set; }
        public SfxType Type { get; set; }
        public int Volume { get; set; }
        public Fixed LastX { get; set; }
        public Fixed LastY { get; set; }

        public void Clear()
        {
            Reserved = Sfx.NONE;
            Playing = Sfx.NONE;
            Priority = 0;
            Source = null;
            Type = 0;
            Volume = 0;
            LastX = Fixed.Zero;
            LastY = Fixed.Zero;
        }
    }
}