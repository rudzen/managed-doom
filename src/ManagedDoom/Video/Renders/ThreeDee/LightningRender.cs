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
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class LightningRender
{
    public const int lightLevelCount = 16;
    public const int lightSegShift = 4;
    public const int scaleLightShift = 12;
    public const int zLightShift = 20;
    private const int colorMapCount = 32;

    public const int maxZLight = 128;

    private readonly byte[][][] diminishingScaleLight;
    private readonly byte[][][] diminishingZLight;
    private readonly byte[][][]? fixedLight;

    public LightningRender(int screenWidth, ColorMap colorMap)
    {
        MaxScaleLight = 48 * (screenWidth / 320);

        diminishingScaleLight = new byte[lightLevelCount][][];
        diminishingZLight = new byte[lightLevelCount][][];
        fixedLight = new byte[lightLevelCount][][];

        for (var i = 0; i < lightLevelCount; i++)
        {
            diminishingScaleLight[i] = new byte[MaxScaleLight][];
            diminishingZLight[i] = new byte[maxZLight][];
            fixedLight[i] = new byte[Math.Max(MaxScaleLight, maxZLight)][];
        }

        const int distMap = 2;

        // Calculate the light levels to use for each level / distance combination.
        for (var i = 0; i < lightLevelCount; i++)
        {
            var start = ((lightLevelCount - 1 - i) * 2) * colorMapCount / lightLevelCount;
            for (var j = 0; j < maxZLight; j++)
            {
                var scale = Fixed.FromInt(320 / 2) / new Fixed((j + 1) << zLightShift);
                scale >>= scaleLightShift;

                var level = start - scale.Data / distMap;
                level = level switch
                {
                    < 0              => 0,
                    >= colorMapCount => colorMapCount - 1,
                    _                => level
                };

                diminishingZLight[i][j] = colorMap[level];
            }
        }
    }

    public int MaxScaleLight { get; }
    public byte[][][] scaleLight { get; private set; }
    public byte[][][] zLight { get; private set; }
    public int ExtraLight { get; set; }
    public int FixedColorMap { get; set; }

    public void Reset(int windowWidth, ColorMap colorMap)
    {
        const int distMap = 2;

        // Calculate the light levels to use for each level / scale combination.
        for (var i = 0; i < lightLevelCount; i++)
        {
            var start = ((lightLevelCount - 1 - i) * 2) * colorMapCount / lightLevelCount;
            for (var j = 0; j < MaxScaleLight; j++)
            {
                var level = start - j * 320 / windowWidth / distMap;
                level = level switch
                {
                    < 0              => 0,
                    >= colorMapCount => colorMapCount - 1,
                    _                => level
                };

                diminishingScaleLight[i][j] = colorMap[level];
            }
        }
    }

    public void Clear(ColorMap colorMap)
    {
        if (FixedColorMap == 0)
        {
            scaleLight = diminishingScaleLight;
            zLight = diminishingZLight;
            fixedLight![0][0] = null!;
        }
        else if (fixedLight![0][0] != colorMap[FixedColorMap])
        {
            for (var i = 0; i < lightLevelCount; i++)
                Array.Fill(fixedLight[i], colorMap[FixedColorMap]);

            scaleLight = fixedLight;
            zLight = fixedLight;
        }
    }
}