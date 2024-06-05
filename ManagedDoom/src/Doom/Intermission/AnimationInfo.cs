﻿//
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


using System.Collections.Generic;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Doom.Intermission
{
    public sealed class AnimationInfo
    {
        public AnimationInfo(AnimationType type, int period, int count, int x, int y)
        {
            this.Type = type;
            this.Period = period;
            this.Count = count;
            this.X = x;
            this.Y = y;
        }

        public AnimationInfo(AnimationType type, int period, int count, int x, int y, int data)
        {
            this.Type = type;
            this.Period = period;
            this.Count = count;
            this.X = x;
            this.Y = y;
            this.Data = data;
        }

        public AnimationType Type { get; }

        public int Period { get; }

        public int Count { get; }

        public int X { get; }

        public int Y { get; }

        public int Data { get; }

        public static readonly IReadOnlyList<IReadOnlyList<AnimationInfo>> Episodes = new AnimationInfo[][]
        {
            [
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 224, 104),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 184, 160),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 112, 136),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 72, 112),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 88, 96),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 64, 48),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 192, 40),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 136, 16),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 80, 16),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 64, 24)
            ],

            [
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 1),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 2),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 3),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 4),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 5),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 6),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 7),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 3, 192, 144, 8),
                new AnimationInfo(AnimationType.Level, GameConst.TicRate / 3, 1, 128, 136, 8)
            ],

            [
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 104, 168),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 40, 136),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 160, 96),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 104, 80),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 3, 3, 120, 32),
                new AnimationInfo(AnimationType.Always, GameConst.TicRate / 4, 3, 40, 0)
            ]
        };
    }
}
