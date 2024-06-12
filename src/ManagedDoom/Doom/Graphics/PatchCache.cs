//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
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


using System.Collections.Generic;
using System.Runtime.InteropServices;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Doom.Graphics;

public sealed class PatchCache(IGameContent content) : IPatchCache
{
    private readonly Wad.Wad wad = content.Wad;
    private readonly Dictionary<string, Patch> cache = new(32);

    public Patch this[string name]
    {
        get
        {
            ref var p2 = ref CollectionsMarshal.GetValueRefOrAddDefault(cache, name, out var exists);

            if (exists)
                return p2;

            p2 = Patch.FromWad(wad, name);

            return p2;
        }
    }

    public int GetWidth(string name)
    {
        return this[name].Width;
    }

    public int GetHeight(string name)
    {
        return this[name].Height;
    }
}