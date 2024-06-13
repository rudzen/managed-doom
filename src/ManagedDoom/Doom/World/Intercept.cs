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

public sealed class Intercept
{
    public Fixed Frac { get; set; }

    public Mobj Thing { get; private set; }

    public LineDef? Line { get; private set; }

    public void Make(Fixed frac, Mobj thing)
    {
        this.Frac = frac;
        this.Thing = thing;
        this.Line = null;
    }

    public void Make(Fixed frac, LineDef line)
    {
        this.Frac = frac;
        this.Thing = null;
        this.Line = line;
    }
}