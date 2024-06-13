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

using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Opening;

namespace ManagedDoom.Video;

public sealed class OpeningSequenceRenderer(IPatchCache patchCache, IDrawScreen screen) : IOpeningSequenceRenderer
{
    public bool Render(IOpeningSequence sequence)
    {
        var scale = screen.Width / 320;

        switch (sequence.State)
        {
            case OpeningSequenceState.Title:
                screen.DrawPatch(patchCache["TITLEPIC"], 0, 0, scale);
                return true;

            case OpeningSequenceState.Demo:
                return false;

            case OpeningSequenceState.Credit:
                screen.DrawPatch(patchCache["CREDIT"], 0, 0, scale);
                return true;
            default:
                return true;
        }
    }
}