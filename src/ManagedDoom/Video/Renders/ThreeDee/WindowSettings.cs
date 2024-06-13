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

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class WindowSettings
{
    public int WindowX { get; private set; }
    public int WindowY { get; private set; }
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    public int CenterY { get; private set; }
    public Fixed CenterXFrac { get; private set; }
    public Fixed CenterYFrac { get; private set; }
    public Fixed Projection { get; private set; }

    public void Reset(int x, int y, int width, int height)
    {
        WindowX = x;
        WindowY = y;
        WindowWidth = width;
        WindowHeight = height;
        var centerX = WindowWidth / 2;
        CenterY = WindowHeight / 2;
        CenterXFrac = Fixed.FromInt(centerX);
        CenterYFrac = Fixed.FromInt(CenterY);
        Projection = CenterXFrac;
    }
}