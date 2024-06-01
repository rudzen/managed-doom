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
    public sealed class LineDef
    {
        private static readonly int dataSize = 14;

        public LineDef(
            Vertex vertex1,
            Vertex vertex2,
            LineFlags flags,
            LineSpecial special,
            short tag,
            SideDef frontSide,
            SideDef backSide)
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
            {
                SlopeType = SlopeType.Vertical;
            }
            else if (Dy == Fixed.Zero)
            {
                SlopeType = SlopeType.Horizontal;
            }
            else
            {
                if (Dy / Dx > Fixed.Zero)
                {
                    SlopeType = SlopeType.Positive;
                }
                else
                {
                    SlopeType = SlopeType.Negative;
                }
            }

            BoundingBox = new Fixed[4];
            BoundingBox[Box.Top] = Fixed.Max(vertex1.Y, vertex2.Y);
            BoundingBox[Box.Bottom] = Fixed.Min(vertex1.Y, vertex2.Y);
            BoundingBox[Box.Left] = Fixed.Min(vertex1.X, vertex2.X);
            BoundingBox[Box.Right] = Fixed.Max(vertex1.X, vertex2.X);

            FrontSector = frontSide?.Sector;
            BackSector = backSide?.Sector;
        }

        public static LineDef FromData(byte[] data, int offset, Vertex[] vertices, SideDef[] sides)
        {
            var vertex1Number = BitConverter.ToInt16(data, offset);
            var vertex2Number = BitConverter.ToInt16(data, offset + 2);
            var flags = BitConverter.ToInt16(data, offset + 4);
            var special = BitConverter.ToInt16(data, offset + 6);
            var tag = BitConverter.ToInt16(data, offset + 8);
            var side0Number = BitConverter.ToInt16(data, offset + 10);
            var side1Number = BitConverter.ToInt16(data, offset + 12);

            return new LineDef(
                vertices[vertex1Number],
                vertices[vertex2Number],
                (LineFlags)flags,
                (LineSpecial)special,
                tag,
                sides[side0Number],
                side1Number != -1 ? sides[side1Number] : null);
        }

        public static LineDef[] FromWad(Wad wad, int lump, Vertex[] vertices, SideDef[] sides)
        {
            var length = wad.GetLumpSize(lump);
            if (length % dataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / dataSize;
            var lines = new LineDef[count]; ;

            for (var i = 0; i < count; i++)
            {
                var offset = 14 * i;
                lines[i] = FromData(data, offset, vertices, sides);
            }

            return lines;
        }

        public Vertex Vertex1 { get; }

        public Vertex Vertex2 { get; }

        public Fixed Dx { get; }

        public Fixed Dy { get; }

        public LineFlags Flags { get; set; }

        public LineSpecial Special { get; set; }

        public short Tag { get; set; }

        public SideDef FrontSide { get; }

        public SideDef BackSide { get; }

        public Fixed[] BoundingBox { get; }

        public SlopeType SlopeType { get; }

        public Sector FrontSector { get; }

        public Sector BackSector { get; }

        public int ValidCount { get; set; }

        public Thinker SpecialData { get; set; }

        public Mobj SoundOrigin { get; set; }
    }
}
