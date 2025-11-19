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
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.Map;

public sealed record Seg(
    Vertex Vertex1,
    Vertex Vertex2,
    Fixed Offset,
    Angle Angle,
    SideDef? SideDef,
    LineDef? LineDef,
    Sector? FrontSector,
    Sector? BackSector)
{
    private const int DataSize = 12;

    private static Seg FromData(
        ReadOnlySpan<byte> data,
        ReadOnlySpan<Vertex> vertices,
        ReadOnlySpan<LineDef> lines)
    {
        var vertex1Number = BitConverter.ToInt16(data[..2]);
        var vertex2Number = BitConverter.ToInt16(data.Slice(2, 2));
        var angle = BitConverter.ToInt16(data.Slice(4, 2));
        var lineNumber = BitConverter.ToInt16(data.Slice(6, 2));
        var side = BitConverter.ToInt16(data.Slice(8, 2));
        var segOffset = BitConverter.ToInt16(data.Slice(10, 2));

        var lineDef = lines[lineNumber];

        SideDef? frontSide;
        SideDef? backSide;

        if (side == 0)
        {
            frontSide = lineDef.FrontSide;
            backSide = lineDef.BackSide;
        }
        else
        {
            frontSide = lineDef.BackSide;
            backSide = lineDef.FrontSide;
        }

        return new Seg(
            Vertex1: vertices[vertex1Number],
            Vertex2: vertices[vertex2Number],
            Offset: Fixed.FromInt(segOffset),
            Angle: new Angle((uint)angle << 16),
            SideDef: frontSide,
            LineDef: lineDef,
            FrontSector: frontSide?.Sector,
            BackSector: (lineDef.Flags & LineFlags.TwoSided) != 0 ? backSide?.Sector : null);
    }

    public static Seg[] FromWad(Wad.Wad wad, int lump, Vertex[] vertices, LineDef[] lines)
    {
        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % DataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / DataSize;
        var segments = new Seg[count];

        for (var i = 0; i < segments.Length; i++)
        {
            var offset = DataSize * i;
            segments[i] = FromData(lumpData.Slice(offset, DataSize), vertices, lines);
        }

        return segments;
    }
}