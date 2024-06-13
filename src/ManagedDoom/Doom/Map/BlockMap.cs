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
using System.Buffers;
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Map;

public sealed class BlockMap
{
    private const int IntBlockSize = 128;
    public static readonly Fixed BlockSize = Fixed.FromInt(IntBlockSize);
    public static readonly int BlockMask = BlockSize.Data - 1;
    public const int FracToBlockShift = Fixed.FracBits + 7;
    public const int BlockToFracShift = FracToBlockShift - Fixed.FracBits;

    private readonly short[] table;

    private readonly LineDef[] lines;

    private BlockMap(
        Fixed originX,
        Fixed originY,
        int width,
        int height,
        short[] table,
        LineDef[] lines)
    {
        this.OriginX = originX;
        this.OriginY = originY;
        this.Width = width;
        this.Height = height;
        this.table = table;
        this.lines = lines;

        ThingLists = new Mobj[width * height];
    }

    public Fixed OriginX { get; }
    public Fixed OriginY { get; }
    public int Width { get; }
    public int Height { get; }
    public Mobj[] ThingLists { get; }

    public static BlockMap FromWad(Wad.Wad wad, int lump, LineDef[] lines)
    {
        var lumpSize = wad.GetLumpSize(lump);
        var lumpData = ArrayPool<byte>.Shared.Rent(lumpSize);

        try
        {
            var lumpBuffer = lumpData.AsSpan(0, lumpSize);
            wad.ReadLump(lump, lumpBuffer);

            var table = new short[lumpSize / 2];
            for (var i = 0; i < table.Length; i++)
            {
                var offset = 2 * i;
                table[i] = BitConverter.ToInt16(lumpBuffer.Slice(offset, 2));
            }

            var originX = Fixed.FromInt(table[0]);
            var originY = Fixed.FromInt(table[1]);
            var width = table[2];
            var height = table[3];

            return new BlockMap(
                originX,
                originY,
                width,
                height,
                table,
                lines);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(lumpData);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetBlockX(Fixed x)
    {
        return (x - OriginX).Data >> FracToBlockShift;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetBlockY(Fixed y)
    {
        return (y - OriginY).Data >> FracToBlockShift;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndex(int blockX, int blockY)
    {
        if (0 <= blockX && blockX < Width && 0 <= blockY && blockY < Height)
            return Width * blockY + blockX;

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndex(Fixed x, Fixed y)
    {
        var blockX = GetBlockX(x);
        var blockY = GetBlockY(y);
        return GetIndex(blockX, blockY);
    }

    public bool IterateLines(int blockX, int blockY, Func<LineDef, bool> func, int validCount)
    {
        var index = GetIndex(blockX, blockY);

        if (index == -1)
            return true;

        for (var offset = table[4 + index]; table[offset] != -1; offset++)
        {
            var line = lines[table[offset]];

            if (line.ValidCount == validCount)
                continue;

            line.ValidCount = validCount;

            if (!func(line))
                return false;
        }

        return true;
    }

    public bool IterateThings(int blockX, int blockY, Func<Mobj, bool> func)
    {
        var index = GetIndex(blockX, blockY);

        if (index == -1)
            return true;

        for (var mobj = ThingLists[index]; mobj != null; mobj = mobj.BlockNext)
            if (!func(mobj))
                return false;

        return true;
    }
}