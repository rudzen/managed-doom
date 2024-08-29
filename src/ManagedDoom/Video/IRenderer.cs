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
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video;

public interface IRenderer
{
    int Width { get; }
    int Height { get; }
    int WipeBandCount { get; }
    int WipeHeight { get; }
    int MaxWindowSize { get; }
    int WindowSize { get; set; }
    bool DisplayMessage { get; set; }
    int GammaCorrectionLevel { get; set; }
    void RenderGame(DoomGame game, Fixed frameFrac, in long fps);
    void Render(Doom.Doom doom, Span<byte> destination, Fixed frameFrac, in long fps);
    void InitializeWipe();
}