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

namespace ManagedDoom.Doom.Menu
{
    public class SliderMenuItem : MenuItem
    {
        private readonly Func<int> reset;
        private readonly Action<int> action;

        public SliderMenuItem(
            string name,
            int skullX, int skullY,
            int itemX, int itemY,
            int sliderLength,
            Func<int> reset,
            Action<int> action)
            : base(skullX, skullY, null)
        {
            this.Name = name;
            this.ItemX = itemX;
            this.ItemY = itemY;

            this.SliderLength = sliderLength;
            SliderPosition = 0;

            this.action = action;
            this.reset = reset;
        }

        public void Reset()
        {
            if (reset != null)
                SliderPosition = reset();
        }

        public void Up()
        {
            if (SliderPosition < SliderLength - 1)
                SliderPosition++;

            action?.Invoke(SliderPosition);
        }

        public void Down()
        {
            if (SliderPosition > 0)
                SliderPosition--;

            action?.Invoke(SliderPosition);
        }

        public string Name { get; }

        public int ItemX { get; }

        public int ItemY { get; }

        public int SliderX => ItemX;
        public int SliderY => ItemY + 16;
        public int SliderLength { get; }

        public int SliderPosition { get; private set; }
    }
}
