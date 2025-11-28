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
using Silk.NET.Maths;
using Silk.NET.Windowing;
using ManagedDoom.Config;

namespace ManagedDoom.Silk;

public sealed class SilkConfig
{
    private const int MinScreenWidth = 320;
    private const int MaxScreenWidth = 3200;

    private const int MinScreenHeight = 200;
    private const int MaxScreenHeight = 2000;

    private const int MaxScreenSize = 9;
    private const int MaxGammaCorrectionLevel = 11;

    public SilkConfig(DoomConfig doomConfig)
    {
        DoomConfig = doomConfig;

        if (!DoomConfig.IsRestoredFromFile)
        {
            var vm = GetDefaultVideoMode();
            DoomConfig.Values.VideoScreenWidth = vm.Resolution!.Value.X;
            DoomConfig.Values.VideoScreenHeight = vm.Resolution.Value.Y;
        }

        doomConfig.Values.VideoScreenWidth = Math.Clamp(doomConfig.Values.VideoScreenWidth, MinScreenWidth, MaxScreenWidth);
        doomConfig.Values.VideoScreenHeight = Math.Clamp(doomConfig.Values.VideoScreenHeight, MinScreenHeight, MaxScreenHeight);

        doomConfig.Values.VideoGameScreenSize = Math.Clamp(doomConfig.Values.VideoGameScreenSize, 0, MaxScreenSize);
        doomConfig.Values.VideoGammaCorrection = Math.Clamp(doomConfig.Values.VideoGammaCorrection, 0, MaxGammaCorrectionLevel);
    }

    public DoomConfig DoomConfig { get; }

    private static VideoMode GetDefaultVideoMode()
    {
        var monitor = Monitor.GetMainMonitor(null);

        const int baseWidth = 640;
        const int baseHeight = 400;

        var currentWidth = baseWidth;
        var currentHeight = baseHeight;

        while (true)
        {
            var nextWidth = currentWidth + baseWidth;
            var nextHeight = currentHeight + baseHeight;

            if (nextWidth >= 0.9 * monitor.VideoMode.Resolution!.Value.X ||
                nextHeight >= 0.9 * monitor.VideoMode.Resolution.Value.Y)
            {
                break;
            }

            currentWidth = nextWidth;
            currentHeight = nextHeight;
        }

        return new VideoMode(new Vector2D<int>(currentWidth, currentHeight));
    }
}