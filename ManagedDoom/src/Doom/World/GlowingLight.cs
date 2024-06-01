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


namespace ManagedDoom
{
    public sealed class GlowingLight : Thinker
    {
        private static readonly int glowSpeed = 8;

        private World world;

        public GlowingLight(World world)
        {
            this.world = world;
        }

        public override void Run()
        {
            switch (Direction)
            {
                case -1:
                    // Down.
                    Sector.LightLevel -= glowSpeed;
                    if (Sector.LightLevel <= MinLight)
                    {
                        Sector.LightLevel += glowSpeed;
                        Direction = 1;
                    }
                    break;

                case 1:
                    // Up.
                    Sector.LightLevel += glowSpeed;
                    if (Sector.LightLevel >= MaxLight)
                    {
                        Sector.LightLevel -= glowSpeed;
                        Direction = -1;
                    }
                    break;
            }
        }

        public Sector Sector { get; set; }

        public int MinLight { get; set; }

        public int MaxLight { get; set; }

        public int Direction { get; set; }
    }
}
