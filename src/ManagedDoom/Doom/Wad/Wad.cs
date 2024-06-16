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
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Doom.Wad;

public sealed class Wad : IDisposable
{
    private readonly record struct WadHeader(string Id, int LumpCount, int LumpInfoTableOffset);

    private readonly List<string> names;
    private readonly List<Stream> streams;

    public Wad(IEnumerable<string> fileNames) : this(fileNames.ToArray())
    {
    }

    public Wad(params string[] fileNames)
    {
        try
        {
            Console.Write("Open WAD files: ");
            var start = Stopwatch.GetTimestamp();

            names = new List<string>(fileNames.Length);
            streams = new List<Stream>(fileNames.Length);
            LumpInfos = fileNames.SelectMany(AddFile).ToArray();

            var nameSpan = CollectionsMarshal.AsSpan(names);
            GameMode = GetGameMode(nameSpan);
            MissionPack = GetMissionPack(nameSpan);
            GameVersion = GetGameVersion(nameSpan);

            Console.WriteLine($"OK ({string.Join(", ", fileNames.Select(Path.GetFileName))}) [{Stopwatch.GetElapsedTime(start)}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            Dispose();
            ExceptionDispatchInfo.Throw(e);
        }
    }

    private List<LumpInfo> AddFile(string fileName)
    {
        names.Add(Path.GetFileNameWithoutExtension(fileName));

        var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        streams.Add(stream);

        var wadHeader = ReadWadHeader(stream);

        {
            var size = LumpInfo.DataSize * wadHeader.LumpCount;
            var data = ArrayPool<byte>.Shared.Rent(size);
            stream.Seek(wadHeader.LumpInfoTableOffset, SeekOrigin.Begin);

            try
            {
                var lumps = new List<LumpInfo>(size);
                var read = stream.Read(data);

                if (read != size)
                    throw new Exception("Failed to read the WAD file.");

                var slice = data.AsSpan(0, read);

                for (var i = 0; i < wadHeader.LumpCount; i++)
                {
                    var offset = LumpInfo.DataSize * i;
                    var position = BitConverter.ToInt32(slice.Slice(offset, 4));
                    var s = BitConverter.ToInt32(slice.Slice(offset + 4, 4));
                    var name = DoomInterop.ToString(slice.Slice(offset + 8, 8));

                    if (s == -1)
                        lumps.Add(new LumpInfo(name, null));
                    else
                    {
                        var buffer = new byte[s];
                        stream.Seek(position, SeekOrigin.Begin);
                        read = stream.Read(buffer);
                        if (read != s)
                            throw new Exception("Failed to read the WAD file.");

                        lumps.Add(new LumpInfo(name, buffer));
                    }
                }

                return lumps;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SkipLocalsInit]
    private static WadHeader ReadWadHeader(FileStream stream)
    {
        const int headerSize = 12;

        Span<byte> data = stackalloc byte[headerSize];

        if (stream.Read(data) != headerSize)
            throw new Exception("Failed to read the WAD file.");

        var identification = DoomInterop.ToString(data[..4]);
        var lumpCount = BitConverter.ToInt32(data.Slice(4, 4));
        var lumpInfoTableOffset = BitConverter.ToInt32(data.Slice(8, 4));

        if (!IsValidWadId(identification))
            throw new Exception("The file is not a WAD file.");

        return new WadHeader(identification, lumpCount, lumpInfoTableOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetLumpNumber(string name)
    {
        var lumpSpan = LumpInfos.AsSpan();
        for (var i = lumpSpan.Length - 1; i >= 0; i--)
        {
            if (lumpSpan[i].Name == name)
                return i;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int, int) GetLumpNumberAndSize(string name)
    {
        var lumpSpan = LumpInfos.AsSpan();
        for (var i = lumpSpan.Length - 1; i >= 0; i--)
        {
            if (lumpSpan[i].Name == name)
                return (i, lumpSpan[i].Data?.Length ?? -1);
        }

        return (-1, -1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetLumpSize(int number)
    {
        return LumpInfos[number].Data?.Length ?? -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadLump(int number)
    {
        return LumpInfos[number].Data!;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> GetLumpData(int number)
    {
        return LumpInfos[number].Data.AsSpan();
    }

    public void ReadLump(int number, Span<byte> buffer)
    {
        var lumpInfo = LumpInfos[number];
        lumpInfo.Data.CopyTo(buffer);
    }

    public byte[] ReadLump(string name)
    {
        var lumpNumber = GetLumpNumber(name);

        if (lumpNumber == -1)
            throw new Exception($"The lump '{name}' was not found.");

        return ReadLump(lumpNumber);
    }

    public void Dispose()
    {
        Console.WriteLine("Close WAD files.");

        foreach (var stream in streams)
            stream.Dispose();

        streams.Clear();
    }

    private static bool IsValidWadId(ReadOnlySpan<char> wadId)
    {
        return wadId is "IWAD" or "PWAD";
    }

    private static GameVersion GetGameVersion(ReadOnlySpan<string> names)
    {
        foreach (var name in names)
            if (TryGetGameVersion(name, out var gameVersion))
                return gameVersion;

        return GameVersion.Version109;
    }

    [SkipLocalsInit]
    private static bool TryGetGameVersion(ReadOnlySpan<char> name, out GameVersion gameVersion)
    {
        gameVersion = default;

        Span<char> lower = stackalloc char[name.Length];
        name.ToLower(lower, CultureInfo.InvariantCulture);

        if (lower is "doom2" or "freedoom2")
            gameVersion = GameVersion.Version109;
        if (lower is "doom" or "doom1" or "freedoom1")
            gameVersion = GameVersion.Ultimate;
        if (lower is "plutonia" or "tnt")
            gameVersion = GameVersion.Final;

        return gameVersion != default;
    }

    private static GameMode GetGameMode(ReadOnlySpan<string> names)
    {
        foreach (var name in names)
            if (TryGetGameGameMode(name, out var gameMode))
                return gameMode;

        return GameMode.Indetermined;
    }

    [SkipLocalsInit]
    private static bool TryGetGameGameMode(ReadOnlySpan<char> name, out GameMode gameMode)
    {
        gameMode = default;

        Span<char> lower = stackalloc char[name.Length];
        name.ToLower(lower, CultureInfo.InvariantCulture);

        if (lower is "doom2" or "plutonia" or "tnt" or "freedoom2")
            gameMode = GameMode.Commercial;
        if (lower is "doom" or "freedoom1")
            gameMode = GameMode.Retail;
        if (lower is "doom1")
            gameMode = GameMode.Shareware;

        return gameMode != default;
    }

    private static MissionPack GetMissionPack(ReadOnlySpan<string> names)
    {
        foreach (var name in names)
            if (TryGetMissionPack(name, out var missionPack))
                return missionPack;

        return MissionPack.Doom2;
    }

    [SkipLocalsInit]
    private static bool TryGetMissionPack(ReadOnlySpan<char> name, out MissionPack missionPack)
    {
        missionPack = default;

        Span<char> lower = stackalloc char[name.Length];
        name.ToLower(lower, CultureInfo.InvariantCulture);

        if (lower is "plutonia")
            missionPack = MissionPack.Plutonia;
        if (lower is "tnt")
            missionPack = MissionPack.Tnt;

        return missionPack != default;
    }

    public LumpInfo[] LumpInfos { get; }

    public GameVersion GameVersion { get; }
    public GameMode GameMode { get; }
    public MissionPack MissionPack { get; }
}