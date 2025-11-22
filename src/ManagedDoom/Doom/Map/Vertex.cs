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
using ManagedDoom.Interfaces;

namespace ManagedDoom.Doom.Map;

public readonly record struct Vertex(Fixed X, Fixed Y);

public static class VertexExtensions
{
    private const int DataSize = 4;

    public static Vertex[] CreateVertices(this Wad.Wad wad, int lump)
    {
        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % DataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / DataSize;
        var vertices = new Vertex[count];

        for (var i = 0; i < vertices.Length; i++)
        {
            var offset = DataSize * i;
            vertices[i] = FromData(lumpData[offset..]);
        }

        return vertices;
    }

    private static Vertex FromData(ReadOnlySpan<byte> data)
    {
        var x = BitConverter.ToInt16(data);
        var y = BitConverter.ToInt16(data.Slice(2, 2));

        return new Vertex(Fixed.FromInt(x), Fixed.FromInt(y));
    }
}