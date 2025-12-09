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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Doom.Graphics;

public sealed record Patch(
    string Name,
    int Width,
    int Height,
    int LeftOffset,
    int TopOffset,
    Column[][] Columns)
{
    public override string ToString() => Name;
}

public sealed class PatchCache(GameContent content)
{
    private readonly Wad.Wad wad = content.Wad;
    private readonly Dictionary<string, Patch> cache = new(32);

    public Patch this[string name]
    {
        get
        {
            ref var p2 = ref CollectionsMarshal.GetValueRefOrAddDefault(cache, name, out var exists);

            if (exists)
                return p2!;

            p2 = GraphicsFactory.CreatePatch(wad, name);

            return p2;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetWidth(string name) => this[name].Width;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHeight(string name) => this[name].Height;
}

public static class PatchExtensions
{
    extension(Patch[] patches)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckCompletion()
        {
            return patches.All(x => x != null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasRotation()
        {
            var zero = patches[0];
            return zero != patches[1] || zero != patches[2] || zero != patches[3] || zero != patches[4] || zero != patches[5] || zero != patches[6] || zero != patches[7];
        }
    }
}