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

using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.Map;

public sealed class Node
{
    public Node(
        Fixed x,
        Fixed y,
        Fixed dx,
        Fixed dy,
        Fixed frontBoundingBoxTop,
        Fixed frontBoundingBoxBottom,
        Fixed frontBoundingBoxLeft,
        Fixed frontBoundingBoxRight,
        Fixed backBoundingBoxTop,
        Fixed backBoundingBoxBottom,
        Fixed backBoundingBoxLeft,
        Fixed backBoundingBoxRight,
        int frontChild,
        int backChild)
    {
        this.X = x;
        this.Y = y;
        this.Dx = dx;
        this.Dy = dy;

        var frontBoundingBox = new[]
        {
            frontBoundingBoxTop,
            frontBoundingBoxBottom,
            frontBoundingBoxLeft,
            frontBoundingBoxRight
        };

        var backBoundingBox = new[]
        {
            backBoundingBoxTop,
            backBoundingBoxBottom,
            backBoundingBoxLeft,
            backBoundingBoxRight
        };

        BoundingBox =
        [
            frontBoundingBox,
            backBoundingBox
        ];

        Children =
        [
            frontChild,
            backChild
        ];
    }

    public Fixed X { get; }
    public Fixed Y { get; }
    public Fixed Dx { get; }
    public Fixed Dy { get; }
    public Fixed[][] BoundingBox { get; }
    public int[] Children { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSubsector(int node) => (node & unchecked((int)0xFFFF8000)) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSubsector(int node) => node ^ unchecked((int)0xFFFF8000);
}