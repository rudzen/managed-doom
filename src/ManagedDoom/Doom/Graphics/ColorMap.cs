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
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace ManagedDoom.Doom.Graphics;

public sealed class ColorMap
{
    public const int Inverse = 32;

    private readonly byte[][] data;

    public ColorMap(Wad.Wad wad)
    {
        const string lump = "COLORMAP";
        const int blockSize = 256;

        Console.Write("Load color map: ");

        var start = Stopwatch.GetTimestamp();

        try
        {
            var (lumpNumber, lumpSize) = wad.GetLumpNumberAndSize(lump);
            var num = lumpSize / blockSize;

            var lumpData = wad.GetLumpData(lumpNumber)[..lumpSize];

            data = new byte[num][];
            for (var i = 0; i < num; i++)
                data[i] = lumpData.Slice(blockSize * i, blockSize).ToArray();

            Console.WriteLine($"OK ({num} maps) [{Stopwatch.GetElapsedTime(start)}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }
    }

    public byte[] this[int index] => data[index];

    public byte[] FullBright => data[0];
}