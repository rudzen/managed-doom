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


using System;
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
        random = new DoomRandom(DateTime.Now.Millisecond);
    }

    public static WipeEffect CreateInstance(int width, int height)
    {
        return new WipeEffect(width, height);
    }

    public short[] Y { get; }

    public void Start()
    {
        Y[0] = (short)(-(random.Next() % 16));
        for (var i = 1; i < Y.Length; i++)
        {
            var r = random.Next() % 3 - 1;
            var v = (short)(Y[i - 1] + r);
            Y[i] = v switch
            {
                > 0 => 0,
                -16 => -15,
                _   => v
            };
        }
    }

    public UpdateResult Update()
    {
        var done = true;

        for (var i = 0; i < Y.Length; i++)
        {
            if (Y[i] < 0)
            {
                Y[i]++;
                done = false;
            }
            else if (Y[i] < height)
            {
                var dy = (Y[i] < 16) ? Y[i] + 1 : 8;
                if (Y[i] + dy >= height)
                    dy = height - Y[i];
                Y[i] += (short)dy;
                done = false;
            }
        }

        return updateResults[done.AsByte()];
    }
}