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
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public static class Box
{
    public const int Top = 0;
    public const int Bottom = 1;
    public const int Left = 2;
    public const int Right = 3;

    extension(Fixed[] box)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            box[Top] = box[Right] = Fixed.MinValue;
            box[Bottom] = box[Left] = Fixed.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPoint(Vertex vertex)
        {
            var x = vertex.X;
            var y = vertex.Y;

            if (x < box[Left])
                box[Left] = x;
            else if (x > box[Right])
                box[Right] = x;

            if (y < box[Bottom])
                box[Bottom] = y;
            else if (y > box[Top])
                box[Top] = y;
        }
    }
}