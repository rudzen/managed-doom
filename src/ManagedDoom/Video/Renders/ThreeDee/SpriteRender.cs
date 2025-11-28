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
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class SpriteRender
{
    public static readonly Fixed minZ = Fixed.FromInt(4);

    public SpriteRender()
    {
        VisSprites = new VisSprite[256];
        for (var i = 0; i < VisSprites.Length; i++)
            VisSprites[i] = new VisSprite();
    }

    public int VisSpriteCount { get; set; }
    public VisSprite[] VisSprites { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        VisSpriteCount = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasTooManySprites()
    {
        return VisSpriteCount == VisSprites.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<VisSprite> GetVisibleSprites()
    {
        var span = VisSprites.AsSpan(0, VisSpriteCount);
        span.Sort();
        return span;
    }
}