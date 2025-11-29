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

using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Map;

public sealed class LineDef
{
    public LineDef(
        Vertex vertex1,
        Vertex vertex2,
        LineFlags flags,
        LineSpecial special,
        short tag,
        SideDef? frontSide,
        SideDef? backSide)
    {
        this.Vertex1 = vertex1;
        this.Vertex2 = vertex2;
        this.Flags = flags;
        this.Special = special;
        this.Tag = tag;
        this.FrontSide = frontSide;
        this.BackSide = backSide;

        Dx = vertex2.X - vertex1.X;
        Dy = vertex2.Y - vertex1.Y;

        if (Dx == Fixed.Zero)
            SlopeType = SlopeType.Vertical;
        else if (Dy == Fixed.Zero)
            SlopeType = SlopeType.Horizontal;
        else
            SlopeType = Dy / Dx > Fixed.Zero ? SlopeType.Positive : SlopeType.Negative;

        BoundingBox =
        [
            Fixed.Max(vertex1.Y, vertex2.Y), // Top
            Fixed.Min(vertex1.Y, vertex2.Y), // Bottom
            Fixed.Min(vertex1.X, vertex2.X), // Left
            Fixed.Max(vertex1.X, vertex2.X)  // Right
        ];

        FrontSector = frontSide?.Sector!;
        BackSector = backSide?.Sector;
    }

    public Vertex Vertex1 { get; }
    public Vertex Vertex2 { get; }
    public Fixed Dx { get; }
    public Fixed Dy { get; }
    public LineFlags Flags { get; set; }
    public LineSpecial Special { get; set; }
    public short Tag { get; set; }
    public SideDef? FrontSide { get; }
    public SideDef? BackSide { get; }
    public Fixed[] BoundingBox { get; }
    public SlopeType SlopeType { get; }
    public Sector FrontSector { get; }
    public Sector? BackSector { get; }
    public int ValidCount { get; set; }
    public Thinker SpecialData { get; set; } = null!;
    public Mobj SoundOrigin { get; set; } = null!;
}
