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

using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.Map;

public sealed class MapThing
{
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
}