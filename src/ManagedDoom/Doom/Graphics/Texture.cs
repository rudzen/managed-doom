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
using ManagedDoom.Doom.Common;

namespace ManagedDoom.Doom.Graphics;

public sealed class Texture
{
    public Texture(
        string name,
        bool masked,
        int width,
        int height,
        ReadOnlySpan<TexturePatch> patches)
    {
        this.Name = name;
        this.Masked = masked;
        this.Width = width;
        this.Height = height;
        Composite = GenerateComposite(name, width, height, patches);
    }

    public static Texture FromData(ReadOnlySpan<byte> data, int offset, ReadOnlySpan<Patch> patchLookup)
    {
        var root = data[offset..];
        var name = DoomInterop.ToString(root);
        var masked = BitConverter.ToInt32(root[8..]);
        var width = BitConverter.ToInt16(root[12..]);
        var height = BitConverter.ToInt16(root[14..]);
        var patchCount = BitConverter.ToInt16(root[20..]);
        var patches = new TexturePatch[patchCount];
        for (var i = 0; i < patches.Length; i++)
        {
            var patchOffset = offset + 22 + TexturePatch.DataSize * i;
            patches[i] = TexturePatch.FromData(data[patchOffset..], patchLookup);
        }

        return new Texture(
            name,
            masked != 0,
            width,
            height,
            patches);
    }

    public static string GetName(ReadOnlySpan<byte> data)
    {
        return DoomInterop.ToString(data);
    }

    public static int GetHeight(ReadOnlySpan<byte> data, int offset)
    {
        return BitConverter.ToInt16(data[(offset + 14)..]);
    }

    private static Patch GenerateComposite(string name, int width, int height, ReadOnlySpan<TexturePatch> patches)
    {
        var patchCount = new int[width];
        var columns = new Column[width][];
        var compositeColumnCount = 0;

        foreach (var patch in patches)
        {
            var left = patch.OriginX;
            var right = left + patch.Width;

            var start = System.Math.Max(left, 0);
            var end = System.Math.Min(right, width);

            for (var x = start; x < end; x++)
            {
                patchCount[x]++;
                if (patchCount[x] == 2)
                    compositeColumnCount++;

                columns[x] = patch.Columns[x - patch.OriginX];
            }
        }

        var padding = System.Math.Max(128 - height, 0);
        var data = new byte[height * compositeColumnCount + padding];
        var i = 0;
        for (var x = 0; x < width; x++)
        {
            if (patchCount[x] == 0)
                columns[x] = [];
            else if (patchCount[x] >= 2)
            {
                var column = new Column(0, data, height * i, height);

                foreach (var patch in patches)
                {
                    var px = x - patch.OriginX;
                    if (px < 0 || px >= patch.Width)
                        continue;

                    var patchColumn = patch.Columns[px];
                    DrawColumnInCache(
                        source: patchColumn,
                        destination: column.Data,
                        destinationOffset: column.Offset,
                        destinationY: patch.OriginY,
                        destinationHeight: height
                    );
                }

                columns[x] = [column];

                i++;
            }
        }

        return new Patch(name, width, height, 0, 0, columns);
    }

    private static void DrawColumnInCache(
        ReadOnlySpan<Column> source,
        Span<byte> destination,
        int destinationOffset,
        int destinationY,
        int destinationHeight)
    {
        foreach (var column in source)
        {
            var sourceIndex = column.Offset;
            var destinationIndex = destinationOffset + destinationY + column.TopDelta;
            var length = column.Length;

            var topExceedance = -(destinationY + column.TopDelta);
            if (topExceedance > 0)
            {
                sourceIndex += topExceedance;
                destinationIndex += topExceedance;
                length -= topExceedance;
            }

            var bottomExceedance = destinationY + column.TopDelta + column.Length - destinationHeight;

            if (bottomExceedance > 0)
                length -= bottomExceedance;

            if (length > 0)
                column.Data.AsSpan(sourceIndex, length).CopyTo(destination[destinationIndex..]);
        }
    }

    public override string ToString()
    {
        return Name;
    }

    public string Name { get; }

    public bool Masked { get; }

    public int Width { get; }

    public int Height { get; }
    public Patch Composite { get; }
}