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

using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class MapCollision
{
    public Fixed OpenTop { get; set; }

    public Fixed OpenBottom { get; set; }

    public Fixed OpenRange { get; set; }

    public Fixed LowFloor { get; set; }
}

public static class MapCollisionExtensions
{
    /// <summary>
    /// Sets opentop and openbottom to the window through a two-sided line.
    /// </summary>
    public static void LineOpening(this MapCollision mapCollision, LineDef line)
    {
        if (line.BackSide is null)
        {
            // If the line is single sided, nothing can pass through.
            mapCollision.OpenRange = Fixed.Zero;
            return;
        }

        var front = line.FrontSector;
        var back = line.BackSector;

        var openTop = front.CeilingHeight < back!.CeilingHeight
            ? front.CeilingHeight
            : back.CeilingHeight;

        Fixed openBottom;
        Fixed lowFloor;

        if (front.FloorHeight > back.FloorHeight)
        {
            openBottom = front.FloorHeight;
            lowFloor = back.FloorHeight;
        }
        else
        {
            openBottom = back.FloorHeight;
            lowFloor = front.FloorHeight;
        }

        mapCollision.OpenTop = openTop;
        mapCollision.OpenBottom = openBottom;
        mapCollision.LowFloor = lowFloor;
        mapCollision.OpenRange = openTop - openBottom;
    }
}