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

public sealed class SkyRender
{
    public const int angleToSkyShift = 22;
    public Fixed SkyTextureAlt { get; } = Fixed.FromInt(100);
    public Fixed SkyInvScale { get; private set; } = Fixed.Zero;

    public void Reset(int windowWidth, int screenWidth, int screenHeight)
    {
        // The code below is based on PrBoom+' sky rendering implementation.
        var num = (long)Fixed.FracUnit * screenWidth * 200;
        var den = windowWidth * screenHeight;
        SkyInvScale = new Fixed((int)(num / den));
    }
}