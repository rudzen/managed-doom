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

namespace ManagedDoom
{
    public sealed class Subsector
    {
        private static readonly int dataSize = 4;

        public Subsector(Sector sector, int segCount, int firstSeg)
        {
            this.Sector = sector;
            this.SegCount = segCount;
            this.FirstSeg = firstSeg;
        }

        public static Subsector FromData(byte[] data, int offset, Seg[] segs)
        {
            var segCount = BitConverter.ToInt16(data, offset);
            var firstSegNumber = BitConverter.ToInt16(data, offset + 2);

            return new Subsector(
                segs[firstSegNumber].SideDef.Sector,
                segCount,
                firstSegNumber);
        }

        public static Subsector[] FromWad(Wad wad, int lump, Seg[] segs)
        {
            var length = wad.GetLumpSize(lump);
            if (length % dataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / dataSize;
            var subsectors = new Subsector[count];

            for (var i = 0; i < count; i++)
            {
                var offset = dataSize * i;
                subsectors[i] = FromData(data, offset, segs);
            }

            return subsectors;
        }

        public Sector Sector { get; }

        public int SegCount { get; }

        public int FirstSeg { get; }
    }
}
