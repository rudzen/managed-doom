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
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace ManagedDoom
{
    public sealed class ColorMap
    {
        public const int Inverse = 32;

        private readonly byte[][] data;

        public ColorMap(Wad wad)
        {
            try
            {
                Console.Write("Load color map: ");
                var start = Stopwatch.GetTimestamp();

                const string lump = "COLORMAP";

                var (lumpNumber, lumpSize) = wad.GetLumpNumberAndSize(lump);

                var lumpData = ArrayPool<byte>.Shared.Rent(lumpSize);

                try
                {
                    var lumpBuffer = lumpData.AsSpan(0, lumpSize);
                    wad.ReadLump(lumpNumber, lumpBuffer);

                    var num = lumpSize / 256;

                    data = new byte[num][];
                    for (var i = 0; i < num; i++)
                        data[i] = lumpBuffer.Slice(256 * i, 256).ToArray();

                    Console.WriteLine($"OK [{Stopwatch.GetElapsedTime(start)}]");
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(lumpData);
                }
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
}