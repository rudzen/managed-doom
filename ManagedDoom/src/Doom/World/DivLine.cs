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
    public sealed class DivLine
    {
        public void MakeFrom(LineDef line)
        {
            X = line.Vertex1.X;
            Y = line.Vertex1.Y;
            Dx = line.Dx;
            Dy = line.Dy;
        }

        public Fixed X { get; set; }

        public Fixed Y { get; set; }

        public Fixed Dx { get; set; }

        public Fixed Dy { get; set; }
    }
}
