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

namespace ManagedDoom.Doom.Map;

public sealed record Subsector(Sector Sector, int SegCount, int FirstSeg)
{
    private const int DataSize = 4;

    private static Subsector FromData(ReadOnlySpan<byte> data, ReadOnlySpan<Seg> segments)
    {
        var segCount = BitConverter.ToInt16(data[..2]);
        var firstSegNumber = BitConverter.ToInt16(data.Slice(2, 2));

        return new Subsector(
            segments[firstSegNumber].SideDef!.Sector!,
            segCount,
            firstSegNumber);
    }

    public static Subsector[] FromWad(Wad.Wad wad, int lump, Seg[] segments)
    {
        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % DataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / DataSize;
        var subSectors = new Subsector[count];

        for (var i = 0; i < subSectors.Length; i++)
        {
            var offset = DataSize * i;
            subSectors[i] = FromData(lumpData.Slice(offset, DataSize), segments);
        }

        return subSectors;
    }
}