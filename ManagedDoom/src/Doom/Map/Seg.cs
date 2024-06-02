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

namespace ManagedDoom
{
    public sealed class Seg
    {
        private const int dataSize = 12;

        public Seg(
            Vertex vertex1,
            Vertex vertex2,
            Fixed offset,
            Angle angle,
            SideDef sideDef,
            LineDef lineDef,
            Sector frontSector,
            Sector backSector)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Offset = offset;
            this.Angle = angle;
            this.SideDef = sideDef;
            this.LineDef = lineDef;
            this.FrontSector = frontSector;
            this.BackSector = backSector;
        }

        private static Seg FromData(ReadOnlySpan<byte> data, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<LineDef> lines)
        {
            var vertex1Number = BitConverter.ToInt16(data[..2]);
            var vertex2Number = BitConverter.ToInt16(data.Slice(2, 2));
            var angle = BitConverter.ToInt16(data.Slice(4, 2));
            var lineNumber = BitConverter.ToInt16(data.Slice(6, 2));
            var side = BitConverter.ToInt16(data.Slice(8, 2));
            var segOffset = BitConverter.ToInt16(data.Slice(10, 2));

            var lineDef = lines[lineNumber];
            var frontSide = side == 0 ? lineDef.FrontSide : lineDef.BackSide;
            var backSide = side == 0 ? lineDef.BackSide : lineDef.FrontSide;

            return new Seg(
                vertices[vertex1Number],
                vertices[vertex2Number],
                Fixed.FromInt(segOffset),
                new Angle((uint)angle << 16),
                frontSide,
                lineDef,
                frontSide.Sector,
                (lineDef.Flags & LineFlags.TwoSided) != 0 ? backSide?.Sector : null);
        }

        public static Seg[] FromWad(Wad wad, int lump, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<LineDef> lines)
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
                var segments = new Seg[count];

                for (var i = 0; i < count; i++)
                {
                    var offset = dataSize * i;
                    segments[i] = FromData(lumpBuffer.Slice(offset, dataSize), vertices, lines);
                }

                return segments;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(lumpData);
            }
        }

        public Vertex Vertex1 { get; }

        public Vertex Vertex2 { get; }

        public Fixed Offset { get; }

        public Angle Angle { get; }

        public SideDef SideDef { get; }

        public LineDef LineDef { get; }

        public Sector FrontSector { get; }

        public Sector BackSector { get; }
    }
}