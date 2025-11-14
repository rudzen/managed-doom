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

namespace ManagedDoom.Doom.World;

public static class BoxEx
{
    extension(Fixed[] box)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fixed Top()
        {
            return box[Box.Top];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fixed Bottom()
        {
            return box[Box.Bottom];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fixed Left()
        {
            return box[Box.Left];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fixed Right()
        {
            return box[Box.Right];
        }
    }

    extension(int[] box)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Top()
        {
            return box[Box.Top];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Bottom()
        {
            return box[Box.Bottom];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Left()
        {
            return box[Box.Left];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Right()
        {
            return box[Box.Right];
        }
    }
}