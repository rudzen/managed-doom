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
using ManagedDoom.Doom.World;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class ColorTranslation
{
    private readonly byte[] greenToGray;
    private readonly byte[] greenToBrown;
    private readonly byte[] greenToRed;

    public ColorTranslation()
    {
        greenToGray = new byte[256];
        greenToBrown = new byte[256];
        greenToRed = new byte[256];
        for (var i = 0; i < 256; i++)
        {
            greenToGray[i] = (byte)i;
            greenToBrown[i] = (byte)i;
            greenToRed[i] = (byte)i;
        }

        for (var i = 112; i < 128; i++)
        {
            greenToGray[i] -= 16;
            greenToBrown[i] -= 48;
            greenToRed[i] -= 80;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> GetTranslation(MobjFlags flags)
    {
        return ((int)(flags & MobjFlags.Translation) >> (int)MobjFlags.TransShift) switch
        {
            1 => greenToGray.AsSpan(),
            2 => greenToBrown.AsSpan(),
            _ => greenToRed.AsSpan()
        };
    }
}