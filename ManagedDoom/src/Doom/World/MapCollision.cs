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
    public sealed class MapCollision
    {
        private World world;

        public MapCollision(World world)
        {
            this.world = world;
        }

        /// <summary>
        /// Sets opentop and openbottom to the window through a two sided line.
        /// </summary>
        public void LineOpening(LineDef line)
        {
            if (line.BackSide == null)
            {
                // If the line is single sided, nothing can pass through.
                OpenRange = Fixed.Zero;
                return;
            }

            var front = line.FrontSector;
            var back = line.BackSector;

            if (front.CeilingHeight < back.CeilingHeight)
            {
                OpenTop = front.CeilingHeight;
            }
            else
            {
                OpenTop = back.CeilingHeight;
            }

            if (front.FloorHeight > back.FloorHeight)
            {
                OpenBottom = front.FloorHeight;
                LowFloor = back.FloorHeight;
            }
            else
            {
                OpenBottom = back.FloorHeight;
                LowFloor = front.FloorHeight;
            }

            OpenRange = OpenTop - OpenBottom;
        }

        public Fixed OpenTop { get; private set; }

        public Fixed OpenBottom { get; private set; }

        public Fixed OpenRange { get; private set; }

        public Fixed LowFloor { get; private set; }
    }
}
