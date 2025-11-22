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

public enum OutputType : byte
{
    Null,
    Silk
}

public sealed class Sound : IDisposable
{
    public OutputType SoundType { get; set; }
    public Mobj? Listener { get; set; }

    public int MaxVolume { get; set; }
    public int Volume { get; set; }

    // only for silk-sound
    public AudioClip?[]? buffers;
    public float[] amplitudes;
    public DoomRandom? random;
    public AudioChannel?[]? channels;
    public ChannelInfo[] infos { get; set; }
    public AudioChannel? uiChannel;
    public Sfx uiReserved;
    public float masterVolumeDecay;
    public long lastUpdate;

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

        uiChannel?.Dispose();
    }
}

public static class SoundExtensions
{
    private const int ChannelCount = 8;
    private const int MaxVolume = 15;

    private const float ClipDist = 1200;
    private const float CloseDist = 160;
    private const float Attenuator = ClipDist - CloseDist;

    private static readonly float fastDecay = (float)Math.Pow(0.5, 0.14285714285714285D);
    private static readonly float slowDecay = (float)Math.Pow(0.5, 0.02857142857142857D);

    public static Sound CreateNull()
    {
        Console.WriteLine("Initialize sound: OK [null output]");
        return new Sound
        {
            SoundType = OutputType.Null
        };
    }

    public static Sound Create(ConfigValues config, GameContent content, AudioDevice device)
    {
        Console.Write("Initialize sound: ");
        var start = Stopwatch.GetTimestamp();
        var sound = new Sound
        {
            SoundType = OutputType.Silk
        };

        try
        {
            sound.MaxVolume = MaxVolume;
            config.AudioSoundVolume = Math.Clamp(config.AudioSoundVolume, 0, MaxVolume);

            var sfxNames = DoomInfo.SfxNames.AsSpan();

            sound.buffers = new AudioClip[sfxNames.Length];
            sound.amplitudes = new float[sfxNames.Length];

            sound.random = config.AudioRandomPitch ? new DoomRandom() : null;

            for (var i = 0; i < sfxNames.Length; i++)
            {
                var name = $"DS{sfxNames[i]}";
                var lump = content.Wad.GetLumpNumber(name);

                if (lump == -1)
                    continue;

                var samples = GetSamples(content.Wad, name, out var sampleRate, out var sampleCount);

                if (samples.IsEmpty)
                    continue;

                sound.buffers[i] = new AudioClip(device, sampleRate, 1, samples);
                sound.amplitudes[i] = GetAmplitude(samples, sampleRate, sampleCount);
            }

            sound.channels = new AudioChannel[ChannelCount];
            sound.infos = new ChannelInfo[ChannelCount];
            for (var i = 0; i < sound.channels.Length; i++)
            {
                sound.channels[i] = new AudioChannel(device);
                sound.infos[i] = new ChannelInfo();
            }

            sound.uiChannel = new AudioChannel(device);
            sound.uiReserved = Sfx.NONE;

            sound.masterVolumeDecay = (float)config.AudioSoundVolume / MaxVolume;

            sound.lastUpdate = 0;

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK [{end}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            sound.uiChannel?.Dispose();
            ExceptionDispatchInfo.Throw(e);
        }

        return sound;
    }

    private static void UpdateChannelInfo(ChannelInfo info, AudioChannel channel, float masterVolumeDecay, Mobj listener)
    {
        if (info.Playing == Sfx.NONE)
            return;

        if (channel.State != PlaybackState.Stopped)
        {
            var decay = info.Type == SfxType.Diffuse ? slowDecay : fastDecay;
            info.Priority *= decay;
            SetParam(channel, info, masterVolumeDecay, listener);
        }
        else
        {
            info.Playing = Sfx.NONE;
            if (info.Reserved == Sfx.NONE)
                info.Source = null;
        }
    }

    private static void SetParam(AudioChannel audioChannel, ChannelInfo info, float masterVolumeDecay, Mobj listener)
    {
        if (info.Type == SfxType.Diffuse)
        {
            audioChannel.Position = new Vector3(0, 0, -1);
            audioChannel.Volume = 0.01F * masterVolumeDecay * info.Volume;
        }
        else
        {
            var (sourceX, sourceY) = GetSourceXy(info);

            var x = (sourceX - listener.X).ToFloat();
            var y = (sourceY - listener.Y).ToFloat();

            if (Math.Abs(x) < 16 && Math.Abs(y) < 16)
            {
                audioChannel.Position = new Vector3(0, 0, -1);
                audioChannel.Volume = 0.01F * masterVolumeDecay * info.Volume;
            }
            else
            {
                var dist = MathF.Sqrt(x * x + y * y);
                var angle = MathF.Atan2(y, x) - (float)listener.Angle.ToRadian();
                audioChannel.Position = new Vector3(-MathF.Sin(angle), 0, -MathF.Cos(angle));
                audioChannel.Volume = 0.01F * masterVolumeDecay * GetDistanceDecay(dist) * info.Volume;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetPitch(DoomRandom? random, SfxType type, Sfx sfx)
    {
        if (random is null)
            return 1.0F;

        if (sfx is Sfx.ITEMUP or Sfx.TINK or Sfx.RADIO)
            return 1.0F;

        if (type == SfxType.Voice)
            return 1.0F + 0.075F * (random.Next() - 128) / 128;

        return 1.0F + 0.025F * (random.Next() - 128) / 128;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetDistanceDecay(float dist)
    {
        return dist < CloseDist
            ? 1F
            : Math.Max((ClipDist - dist) / Attenuator, 0F);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (Fixed, Fixed) GetSourceXy(ChannelInfo info)
    {
        return info.Source is null
            ? (info.LastX, info.LastY)
            : (info.Source.X, info.Source.Y);
    }

    private static void PlayUiSound(Sound sound)
    {
        if (sound.uiReserved == Sfx.NONE)
            return;

        if (sound.uiChannel!.State == PlaybackState.Playing)
            sound.uiChannel.Stop();

        sound.uiChannel.Position = new Vector3(0, 0, -1);
        sound.uiChannel.Volume = sound.masterVolumeDecay;
        sound.uiChannel.AudioClip = sound.buffers![sound.uiReserved.AsInt()];
        sound.uiChannel.Play();
        sound.uiReserved = Sfx.NONE;
    }

    extension(Sound sound)
    {
        public void StartSound(Sfx sfx)
        {
            if (sound.SoundType == OutputType.Null || sound.buffers![sfx.AsInt()] is null)
                return;

            sound.uiReserved = sfx;
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume = 100)
        {
            if (sound.SoundType == OutputType.Null || sound.buffers![(int)sfx] is null)
                return;

            var x = (mobj.X - sound.Listener!.X).ToFloat();
            var y = (mobj.Y - sound.Listener.Y).ToFloat();
            var dist = MathF.Sqrt(x * x + y * y);

            var priority = type == SfxType.Diffuse
                ? volume
                : sound.amplitudes[(int)sfx] * GetDistanceDecay(dist) * volume;

            var infos = sound.infos.AsSpan();

            foreach (var info in infos)
            {
                if (info.Source != mobj || info.Type != type)
                    continue;

                info.Reserved = sfx;
                info.Priority = priority;
                info.Volume = volume;
                return;
            }

            foreach (var info in infos)
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

            UpdatePriority(infos, priority, mobj, sfx, type, volume);
        }

        public void Update()
        {
            if (sound.SoundType == OutputType.Null)
                return;

            var now = Stopwatch.GetTimestamp();
            // we don't need to update all the time, so be a bit liberal (fx for timedemo)
            if (Stopwatch.GetElapsedTime(sound.lastUpdate) - Stopwatch.GetElapsedTime(now) < TimeSpan.FromSeconds(0.01))
                return;

            var infoSpan = sound.infos.AsSpan();
            var channelSpan = sound.channels.AsSpan();

            for (var i = 0; i < infoSpan.Length; i++)
            {
                var info = infoSpan[i];
                var channel = channelSpan[i];

                if (channel is null)
                    return;

                UpdateChannelInfo(info, channel, sound.masterVolumeDecay, sound.Listener!);

                if (info.Reserved == Sfx.NONE)
                    continue;

                if (info.Playing != Sfx.NONE)
                    channel.Stop();

                channel.AudioClip = sound.buffers![info.Reserved.AsInt()];
                SetParam(channel, info, sound.masterVolumeDecay, sound.Listener!);
                channel.Pitch = GetPitch(sound.random, info.Type, info.Reserved);
                channel.Play();
                info.Playing = info.Reserved;
                info.Reserved = Sfx.NONE;
            }

            PlayUiSound(sound);

            sound.lastUpdate = now;
        }

        public void StopSound(Mobj mobj)
        {
            if (sound.SoundType == OutputType.Null)
                return;

            var infos = sound.infos.AsSpan();
            foreach (var info in infos)
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
            if (sound.SoundType == OutputType.Null)
                return;

            sound.random?.Clear();

            for (var i = 0; i < sound.infos.Length; i++)
            {
                sound.channels![i]!.Stop();
                sound.infos[i].Clear();
            }

            sound.Listener = null!;
        }

        public void Pause()
        {
            if (sound.SoundType == OutputType.Null)
                return;

            for (var i = 0; i < sound.infos.Length; i++)
            {
                var channel = sound.channels![i];

                if (channel is { State: PlaybackState.Playing, AudioClip: not null }
                    && channel.AudioClip.Duration - channel.PlayingOffset > TimeSpan.FromMilliseconds(200))
                    channel.Pause();
            }
        }

        public void Resume()
        {
            if (sound.SoundType == OutputType.Null)
                return;

            for (var i = 0; i < sound.infos.Length; i++)
            {
                var channel = sound.channels![i]!;

                if (channel.State == PlaybackState.Paused)
                    channel.Play();
            }
        }
    }

    private static void UpdatePriority(ReadOnlySpan<ChannelInfo> infos, float priority, Mobj mobj, Sfx sfx, SfxType type, int volume = 100)
    {
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

    private static Span<byte> GetSamples(Wad wad, string lumpName, out int sampleRate, out int sampleCount)
    {
        var data = wad.ReadLump(lumpName).AsSpan();

        if (data.Length < 8)
        {
            sampleRate = -1;
            sampleCount = -1;
            return [];
        }

        sampleRate = BitConverter.ToUInt16(data.Slice(2, 2));
        sampleCount = BitConverter.ToInt32(data.Slice(4, 4));

        var offset = 8;

        if (sampleCount >= 32 && ContainsDmxPadding(data, sampleCount))
        {
            offset += 16;
            sampleCount -= 32;
        }

        return sampleCount > 0
            ? data.Slice(offset, sampleCount)
            : [];
    }

    /// <summary>
    /// Checks if the provided audio data contains DMX padding.
    /// DMX padding is a specific pattern in Doom sound data where the first and last 16 samples are the same.
    /// This method has been optimized to use Span&lt;T&gt; for efficient data access and manipulation.
    /// https://doomwiki.org/wiki/Sound
    /// </summary>
    private static bool ContainsDmxPadding(ReadOnlySpan<byte> data, int sampleCount)
    {
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
}

public sealed class ChannelInfo
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