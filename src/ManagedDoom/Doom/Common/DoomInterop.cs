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

namespace ManagedDoom.Doom.Common;

public static class DoomInterop
{
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(ReadOnlySpan<byte> data)
    {
        const byte zero = 0;

        Span<char> chars = stackalloc char[data.Length];
        var pos = 0;

        foreach (var t in data)
        {
            var c = (char)t;
            if (c == zero)
                break;
            if (char.IsBetween(c, 'a', 'z'))
                chars[pos++] = (char)(c - 0x20);
            else
                chars[pos++] = c;
        }

        return new string(chars[..pos]);
    }
}