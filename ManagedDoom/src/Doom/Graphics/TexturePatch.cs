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



using System;

namespace ManagedDoom
{
    public sealed class TexturePatch
    {
        public const int DataSize = 10;

        private readonly Patch patch;

        public TexturePatch(
            int originX,
            int originY,
            Patch patch)
        {
            this.OriginX = originX;
            this.OriginY = originY;
            this.patch = patch;
        }

        public static TexturePatch FromData(ReadOnlySpan<byte> data, Patch[] patches)
        {
            var originX = BitConverter.ToInt16(data);
            var originY = BitConverter.ToInt16(data[2..]);
            var patchNum = BitConverter.ToInt16(data[4..]);

            return new TexturePatch(
                originX,
                originY,
                patches[patchNum]);
        }

        public string Name => patch.Name;
        public int OriginX { get; }

        public int OriginY { get; }

        public int Width => patch.Width;
        public int Height => patch.Height;
        public Column[][] Columns => patch.Columns;
    }
}
