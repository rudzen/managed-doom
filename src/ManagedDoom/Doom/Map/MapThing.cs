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
using ManagedDoom.Doom.Math;
using ManagedDoom.Interfaces;

namespace ManagedDoom.Doom.Map;

public sealed class MapThing : IFixedCoordinates
{
    private const int dataSize = 10;

    public static readonly MapThing Empty = new(
        Fixed.Zero,
        Fixed.Zero,
        Angle.Ang0,
        0,
        0);

    public MapThing(
        Fixed x,
        Fixed y,
        Angle angle,
        int type,
        ThingFlags flags)
    {
        this.X = x;
        this.Y = y;
        this.Angle = angle;
        this.Type = type;
        this.Flags = flags;
    }

    public Fixed X { get; }
    public Fixed Y { get; }
    public Angle Angle { get; }
    public int Type { get; set; }
    public ThingFlags Flags { get; }

    public static MapThing[] FromWad(Wad.Wad wad, int lump)
    {
        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var things = new MapThing[count];

        for (var i = 0; i < count; i++)
        {
            var offset = dataSize * i;
            things[i] = FromData(lumpData.Slice(offset, dataSize));
        }

        return things;
    }

    private static MapThing FromData(ReadOnlySpan<byte> data)
    {
        var x = BitConverter.ToInt16(data[..2]);
        var y = BitConverter.ToInt16(data.Slice(2, 2));
        var angle = BitConverter.ToInt16(data.Slice(4, 2));
        var type = BitConverter.ToInt16(data.Slice(6, 2));
        var flags = BitConverter.ToInt16(data.Slice(8, 2));

        return new MapThing(
            Fixed.FromInt(x),
            Fixed.FromInt(y),
            new Angle(Angle.Ang45.Data * (uint)(angle / 45)),
            type,
            (ThingFlags)flags
        );
    }
}