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
    public sealed class Vertex
    {
        private static readonly int dataSize = 4;

        public Vertex(Fixed x, Fixed y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Vertex FromData(ReadOnlySpan<byte> data)
        {
            var x = BitConverter.ToInt16(data);
            var y = BitConverter.ToInt16(data.Slice(2, 2));

            return new Vertex(Fixed.FromInt(x), Fixed.FromInt(y));
        }

        public static Vertex FromData(byte[] data, int offset)
        {
            var x = BitConverter.ToInt16(data, offset);
            var y = BitConverter.ToInt16(data, offset + 2);

            return new Vertex(Fixed.FromInt(x), Fixed.FromInt(y));
        }

        public static Vertex[] FromWad(Wad wad, int lump)
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
                var vertices = new Vertex[count];

                for (var i = 0; i < count; i++)
                {
                    var offset = dataSize * i;
                    vertices[i] = FromData(lumpBuffer[offset..]);
                }

                return vertices;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(lumpData);
            }
        }

        public Fixed X { get; }

        public Fixed Y { get; }
    }
}