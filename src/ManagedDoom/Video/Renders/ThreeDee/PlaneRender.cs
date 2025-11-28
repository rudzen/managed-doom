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

using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class PlaneRender(int screenWidth, int screenHeight)
{
    public Fixed[] PlaneYSlope { get; } = new Fixed[screenHeight];
    public Fixed[] PlaneDistScale { get; } = new Fixed[screenWidth];
    public Fixed PlaneBaseXScale { get; private set; }
    public Fixed PlaneBaseYScale { get; private set; }

    public Sector? CeilingPrevSector { get; set; }
    public int CeilingPrevX { get; set; }
    public int CeilingPrevY1 { get; set; }
    public int CeilingPrevY2 { get; set; }
    public Fixed[] CeilingXFrac { get; } = new Fixed[screenHeight];
    public Fixed[] CeilingYFrac { get; } = new Fixed[screenHeight];
    public Fixed[] CeilingXStep { get; } = new Fixed[screenHeight];
    public Fixed[] CeilingYStep { get; } = new Fixed[screenHeight];
    public byte[][] CeilingLights { get; } = new byte[screenHeight][];

    public Sector? FloorPrevSector { get; set; }
    public int FloorPrevX { get; set; }
    public int FloorPrevY1 { get; set; }
    public int FloorPrevY2 { get; set; }
    public Fixed[] FloorXFrac { get; } = new Fixed[screenHeight];
    public Fixed[] FloorYFrac { get; } = new Fixed[screenHeight];
    public Fixed[] FloorXStep { get; } = new Fixed[screenHeight];
    public Fixed[] FloorYStep { get; } = new Fixed[screenHeight];
    public byte[][] FloorLights { get; } = new byte[screenHeight][];

    public void Reset(int windowWidth, int windowHeight, WallRender wallRender)
    {
        for (var i = 0; i < windowHeight; i++)
        {
            var dy = Fixed.FromInt(i - windowHeight / 2) + Fixed.One / 2;
            dy = Fixed.Abs(dy);
            PlaneYSlope[i] = Fixed.FromInt(windowWidth / 2) / dy;
        }

        for (var i = 0; i < windowWidth; i++)
        {
            var cos = Fixed.Abs(Trig.Cos(wallRender.XToAngle[i]));
            PlaneDistScale[i] = Fixed.One / cos;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear(Angle viewAngle, Fixed centerXFrac)
    {
        var angle = viewAngle - Angle.Ang90;
        PlaneBaseXScale = Trig.Cos(angle) / centerXFrac;
        PlaneBaseYScale = -(Trig.Sin(angle) / centerXFrac);

        CeilingPrevSector = null;
        CeilingPrevX = int.MaxValue;

        FloorPrevSector = null;
        FloorPrevX = int.MaxValue;
    }
}