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
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class VisSprite : IComparer<VisSprite>, IComparable<VisSprite>
{
    public int X1 { get; set; }
    public int X2 { get; set; }

    // For line side calculation.
    public Fixed GlobalX { get; set; }
    public Fixed GlobalY { get; set; }

    // Global bottom / top for silhouette clipping.
    public Fixed GlobalBottomZ { get; set; }
    public Fixed GlobalTopZ { get; set; }

    // Horizontal position of x1.
    public Fixed StartFrac { get; set; }

    public Fixed Scale { get; set; }

    // Negative if flipped.
    public Fixed InvScale { get; set; }

    public Fixed TextureAlt { get; set; }
    public Patch Patch { get; set; }

    // For color translation and shadow draw.
    public byte[] ColorMap { get; set; }

    public MobjFlags MobjFlags { get; set; }

    // to avoid reverse iteration, x - y is used instead of y - x
    // if y - x is used, the sprites should be iterated in reverse order
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(VisSprite? x, VisSprite? y)
    {
        return x!.Scale.Data - y!.Scale.Data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(VisSprite? other)
    {
        return Scale.Data - other!.Scale.Data;
    }
}
