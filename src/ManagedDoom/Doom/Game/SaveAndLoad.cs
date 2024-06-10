//
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
using System.Buffers;
using System.Diagnostics;
using System.IO;

namespace ManagedDoom.Doom.Game;

/// <summary>
/// Vanilla-compatible save and load, full of messy binary handling code.
/// </summary>
public static partial class SaveAndLoad
{
    private const int DescriptionSize = 24;

    private const int VersionSize = 16;
    private const int SaveBufferSize = 360 * 1024;

    private enum ThinkerClass
    {
        End,
        Mobj
    }

    private enum SpecialClass
    {
        Ceiling,
        Door,
        Floor,
        Plat,
        Flash,
        Strobe,
        Glow,
        EndSpecials
    }

    public static void Save(DoomGame game, string description, string path)
    {
        Console.WriteLine($"Saving game to: {path}");
        var start = Stopwatch.GetTimestamp();
        var fileData = ArrayPool<byte>.Shared.Rent(SaveBufferSize);

        try
        {
            var fileBuffer = fileData.AsSpan(0, SaveBufferSize);

            var ptr = SaveHeader(description, fileBuffer);
            ptr = Save(game, fileBuffer, ptr);

            using (var writer = new FileStream(path, FileMode.Create, FileAccess.Write))
                writer.Write(fileBuffer[..ptr]);

            Console.WriteLine($"Saved in: {Stopwatch.GetElapsedTime(start)}, size: {ptr} bytes");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(fileData);
        }
    }

    public static void Load(DoomGame game, string path)
    {
        Console.WriteLine($"Loading game from: {path}");
        var start = Stopwatch.GetTimestamp();
        var options = game.Options;
        game.InitNew(options.Skill, options.Episode, options.Map);

        var file = new FileInfo(path);
        var length = (int)file.Length;

        var fileData = ArrayPool<byte>.Shared.Rent(length);

        try
        {
            // create a buffer to store the file data
            var fileBuffer = fileData.AsSpan(0, length);

            // load file content
            using (var reader = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var read = reader.Read(fileBuffer);
                if (read != length)
                    throw new Exception($"Failed to read the whole file: {path}");
            }

            // validate header
            var ptr = ValidateHeader(fileBuffer);

            // check current position
            if (ptr != (VersionSize + DescriptionSize))
                throw new Exception($"Invalid save file header size: {path}");

            Load(game, fileBuffer, ptr);

            Console.WriteLine($"Loaded in: {Stopwatch.GetElapsedTime(start)}, size: {length} bytes");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(fileData);
        }
    }

    private static int PadPointer(int ptr)
    {
        ptr += (4 - (ptr & 3)) & 3;
        return ptr;
    }
}