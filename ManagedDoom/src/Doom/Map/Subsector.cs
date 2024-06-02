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

namespace ManagedDoom
{
    public sealed class Subsector
    {
        private const int dataSize = 4;

        private Subsector(Sector sector, int segCount, int firstSeg)
        {
            this.Sector = sector;
            this.SegCount = segCount;
            this.FirstSeg = firstSeg;
        }

        private static Subsector FromData(ReadOnlySpan<byte> data, ReadOnlySpan<Seg> segments)
        {
            var segCount = BitConverter.ToInt16(data[..2]);
            var firstSegNumber = BitConverter.ToInt16(data.Slice(2, 2));

            return new Subsector(
                segments[firstSegNumber].SideDef.Sector,
                segCount,
                firstSegNumber);
        }

        public static Subsector[] FromWad(Wad wad, int lump, ReadOnlySpan<Seg> segments)
        {
            var lumpSize = wad.GetLumpSize(lump);
            if (lumpSize % dataSize != 0)
                throw new Exception();

            var lumpData = ArrayPool<byte>.Shared.Rent(lumpSize);

            try
            {
                var lumpBuffer = lumpData.AsSpan(0, lumpSize);
                wad.ReadLump(lumpSize, lumpBuffer);

                var count = lumpSize / dataSize;
                var subSectors = new Subsector[count];

                for (var i = 0; i < count; i++)
                {
                    var offset = dataSize * i;
                    subSectors[i] = FromData(lumpBuffer.Slice(offset, dataSize), segments);
                }

                return subSectors;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(lumpData);
            }
        }

        public Sector Sector { get; }

        public int SegCount { get; }

        public int FirstSeg { get; }
    }
}