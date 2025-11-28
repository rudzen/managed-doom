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

using ManagedDoom.Config;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace ManagedDoom.Silk;

public sealed class WindowFactory
{
    private readonly IWindow window;

    public WindowFactory(DoomConfig doomConfig)
    {
        var windowOptions = WindowOptions.Default;
        windowOptions.Size = new Vector2D<int>(doomConfig.Values.VideoScreenWidth, doomConfig.Values.VideoScreenHeight);
        windowOptions.Title = ApplicationInfo.Title;
        windowOptions.VSync = doomConfig.Values.VideoVsync;
        windowOptions.WindowState = doomConfig.Values.VideoFullscreen ? WindowState.Fullscreen : WindowState.Normal;
        window = Window.Create(windowOptions);
    }

    public IWindow GetWindow() => window;
}