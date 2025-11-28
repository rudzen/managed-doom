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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;
using ManagedDoom.Extensions;

namespace ManagedDoom.Video;

public sealed class WipeEffect
{
    private static readonly UpdateResult[] updateResults = [UpdateResult.None, UpdateResult.Completed];

    private readonly int height;
    private readonly DoomRandom random;

    private WipeEffect(int width, int height)
    {
        Y = new short[width];
        this.height = height;
        random = new DoomRandom(Stopwatch.GetTimestamp());
    }

    public static WipeEffect Create(int width, int height)
    {
        return new WipeEffect(width, height);
    }

    public short[] Y { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Start()
    {
        var ySpan = Y.AsSpan();
        ref var yRef = ref MemoryMarshal.GetReference(ySpan);

        yRef = (short)-(random.Next() % 16);
        for (var i = 1; i < Y.Length; i++)
        {
            ref var y = ref Unsafe.Add(ref yRef, i);
            var r = random.Next() % 3 - 1;
            var v = (short)(Unsafe.Add(ref yRef, i - 1) + r);
            y = v switch
            {
                > 0 => 0,
                -16 => -15,
                _   => v
            };
        }
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UpdateResult Update()
    {
        var done = true;

        var ySpan = Y.AsSpan();
        ref var yRef = ref MemoryMarshal.GetReference(ySpan);

        for (var i = 0; i < Y.Length; i++)
        {
            ref var y = ref Unsafe.Add(ref yRef, i);
            if (y < 0)
            {
                y++;
                done = false;
            }
            else if (y < height)
            {
                var dy = y < 16 ? y + 1 : 8;
                if (y + dy >= height)
                    dy = height - y;
                y += (short)dy;
                done = false;
            }
        }

        return updateResults[done.AsByte()];
    }
}