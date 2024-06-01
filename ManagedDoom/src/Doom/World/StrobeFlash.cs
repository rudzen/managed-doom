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

namespace ManagedDoom
{
    public sealed class StrobeFlash : Thinker
    {
        public static readonly int StrobeBright = 5;
        public static readonly int FastDark = 15;
        public static readonly int SlowDark = 35;

        private World world;

        public StrobeFlash(World world)
        {
            this.world = world;
        }

        public override void Run()
        {
            if (--Count > 0)
            {
                return;
            }

            if (Sector.LightLevel == MinLight)
            {
                Sector.LightLevel = MaxLight;
                Count = BrightTime;
            }
            else
            {
                Sector.LightLevel = MinLight;
                Count = DarkTime;
            }
        }

        public Sector Sector { get; set; }

        public int Count { get; set; }

        public int MinLight { get; set; }

        public int MaxLight { get; set; }

        public int DarkTime { get; set; }

        public int BrightTime { get; set; }
    }
}
