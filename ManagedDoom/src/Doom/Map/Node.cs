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
using System.Buffers;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.Map;

public sealed class Node
{
    private const int dataSize = 28;

    public Node(
        Fixed x,
        Fixed y,
        Fixed dx,
        Fixed dy,
        Fixed frontBoundingBoxTop,
        Fixed frontBoundingBoxBottom,
        Fixed frontBoundingBoxLeft,
        Fixed frontBoundingBoxRight,
        Fixed backBoundingBoxTop,
        Fixed backBoundingBoxBottom,
        Fixed backBoundingBoxLeft,
        Fixed backBoundingBoxRight,
        int frontChild,
        int backChild)
    {
        this.X = x;
        this.Y = y;
        this.Dx = dx;
        this.Dy = dy;

        var frontBoundingBox = new[]
        {
            frontBoundingBoxTop,
            frontBoundingBoxBottom,
            frontBoundingBoxLeft,
            frontBoundingBoxRight
        };

        var backBoundingBox = new[]
        {
            backBoundingBoxTop,
            backBoundingBoxBottom,
            backBoundingBoxLeft,
            backBoundingBoxRight
        };

        BoundingBox =
        [
            frontBoundingBox,
            backBoundingBox
        ];

        Children =
        [
            frontChild,
            backChild
        ];
    }

    public Fixed X { get; }
    public Fixed Y { get; }
    public Fixed Dx { get; }
    public Fixed Dy { get; }
    public Fixed[][] BoundingBox { get; }
    public int[] Children { get; }

    private static Node FromData(ReadOnlySpan<byte> data)
    {
        var x = BitConverter.ToInt16(data[..2]);
        var y = BitConverter.ToInt16(data.Slice(2, 2));
        var dx = BitConverter.ToInt16(data.Slice(4, 2));
        var dy = BitConverter.ToInt16(data.Slice(6, 2));
        var frontBoundingBoxTop = BitConverter.ToInt16(data.Slice(8, 2));
        var frontBoundingBoxBottom = BitConverter.ToInt16(data.Slice(10, 2));
        var frontBoundingBoxLeft = BitConverter.ToInt16(data.Slice(12, 2));
        var frontBoundingBoxRight = BitConverter.ToInt16(data.Slice(14, 2));
        var backBoundingBoxTop = BitConverter.ToInt16(data.Slice(16, 2));
        var backBoundingBoxBottom = BitConverter.ToInt16(data.Slice(18, 2));
        var backBoundingBoxLeft = BitConverter.ToInt16(data.Slice(20, 2));
        var backBoundingBoxRight = BitConverter.ToInt16(data.Slice(22, 2));
        var frontChild = BitConverter.ToInt16(data.Slice(24, 2));
        var backChild = BitConverter.ToInt16(data.Slice(26, 2));

        return new Node(
            x: Fixed.FromInt(x),
            y: Fixed.FromInt(y),
            dx: Fixed.FromInt(dx),
            dy: Fixed.FromInt(dy),
            frontBoundingBoxTop: Fixed.FromInt(frontBoundingBoxTop),
            frontBoundingBoxBottom: Fixed.FromInt(frontBoundingBoxBottom),
            frontBoundingBoxLeft: Fixed.FromInt(frontBoundingBoxLeft),
            frontBoundingBoxRight: Fixed.FromInt(frontBoundingBoxRight),
            backBoundingBoxTop: Fixed.FromInt(backBoundingBoxTop),
            backBoundingBoxBottom: Fixed.FromInt(backBoundingBoxBottom),
            backBoundingBoxLeft: Fixed.FromInt(backBoundingBoxLeft),
            backBoundingBoxRight: Fixed.FromInt(backBoundingBoxRight),
            frontChild: frontChild,
            backChild: backChild);
    }

    public static Node[] FromWad(Wad.Wad wad, int lump)
    {
        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = ArrayPool<byte>.Shared.Rent(lumpSize);

        try
        {
            var lumpBuffer = lumpData.AsSpan(0, lumpSize);
            wad.ReadLump(lump, lumpBuffer);

            var count = lumpSize / dataSize;
            var nodes = new Node[count];

            for (var i = 0; i < count; i++)
            {
                var offset = dataSize * i;
                nodes[i] = FromData(lumpBuffer.Slice(offset, dataSize));
            }

            return nodes;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(lumpData);
        }
    }

    public static bool IsSubsector(int node)
    {
        return (node & unchecked((int)0xFFFF8000)) != 0;
    }

    public static int GetSubsector(int node)
    {
        return node ^ unchecked((int)0xFFFF8000);
    }
}