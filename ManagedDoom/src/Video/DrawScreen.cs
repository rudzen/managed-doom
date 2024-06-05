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
using System.Collections.Generic;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Video
{
    public sealed class DrawScreen
    {
        private readonly Patch[] chars;

        public DrawScreen(Wad wad, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Data = new byte[width * height];
            
            chars = new Patch[128];
            for (var i = 0; i < chars.Length; i++)
            {
                var name = $"STCFN{i:000}";
                var lump = wad.GetLumpNumber(name);
                if (lump != -1)
                    chars[i] = Patch.FromData(name, wad.ReadLump(lump));
            }
        }

        public void DrawPatch(Patch patch, int x, int y, int scale)
        {
            var drawX = x - scale * patch.LeftOffset;
            var drawY = y - scale * patch.TopOffset;
            var drawWidth = scale * patch.Width;

            var i = 0;
            var frac = Fixed.One / scale - Fixed.Epsilon;
            var step = Fixed.One / scale;

            if (drawX < 0)
            {
                var exceed = -drawX;
                frac += exceed * step;
                i += exceed;
            }

            if (drawX + drawWidth > Width)
            {
                var exceed = drawX + drawWidth - Width;
                drawWidth -= exceed;
            }

            for (; i < drawWidth; i++)
            {
                DrawColumn(patch.Columns[frac.ToIntFloor()], drawX + i, drawY, scale);
                frac += step;
            }
        }

        public void DrawPatchFlip(Patch patch, int x, int y, int scale)
        {
            var drawX = x - scale * patch.LeftOffset;
            var drawY = y - scale * patch.TopOffset;
            var drawWidth = scale * patch.Width;

            var i = 0;
            var frac = Fixed.One / scale - Fixed.Epsilon;
            var step = Fixed.One / scale;

            if (drawX < 0)
            {
                var exceed = -drawX;
                frac += exceed * step;
                i += exceed;
            }

            if (drawX + drawWidth > Width)
            {
                var exceed = drawX + drawWidth - Width;
                drawWidth -= exceed;
            }

            for (; i < drawWidth; i++)
            {
                var col = patch.Width - frac.ToIntFloor() - 1;
                DrawColumn(patch.Columns[col], drawX + i, drawY, scale);
                frac += step;
            }
        }

        private void DrawColumn(Column[] source, int x, int y, int scale)
        {
            var step = Fixed.One / scale;

            foreach (var (topDelta, bytes, sourceIndex, length) in source)
            {
                var exTopDelta = scale * topDelta;
                var exLength = scale * length;

                var drawY = y + exTopDelta;
                var drawLength = exLength;

                var i = 0;
                var p = Height * x + drawY;
                var frac = Fixed.One / scale - Fixed.Epsilon;

                if (drawY < 0)
                {
                    var exceed = -drawY;
                    p += exceed;
                    frac += exceed * step;
                    i += exceed;
                }

                if (drawY + drawLength > Height)
                {
                    var exceed = drawY + drawLength - Height;
                    drawLength -= exceed;
                }

                for (; i < drawLength; i++)
                {
                    Data[p] = bytes[sourceIndex + frac.ToIntFloor()];
                    p++;
                    frac += step;
                }
            }
        }

        public void DrawText(IReadOnlyList<char> text, int x, int y, int scale)
        {
            var drawX = x;
            var drawY = y - 7 * scale;
            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                    continue;

                if (ch == 32)
                {
                    drawX += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if (index is >= 'a' and <= 'z')
                    index = index - 'a' + 'A';

                var patch = chars[index];
                if (patch == null)
                    continue;

                DrawPatch(patch, drawX, drawY, scale);

                drawX += scale * patch.Width;
            }
        }

        public void DrawChar(char ch, int x, int y, int scale)
        {
            var drawX = x;
            var drawY = y - 7 * scale;

            if (ch >= chars.Length)
                return;

            if (ch == 32)
                return;

            var index = (int)ch;
            if (index is >= 'a' and <= 'z')
                index = index - 'a' + 'A';

            var patch = chars[index];
            if (patch == null)
                return;

            DrawPatch(patch, drawX, drawY, scale);
        }

        public void DrawText(string text, int x, int y, int scale)
        {
            var drawX = x;
            var drawY = y - 7 * scale;
            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                    continue;

                if (ch == 32)
                {
                    drawX += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if (index is >= 'a' and <= 'z')
                    index = index - 'a' + 'A';

                var patch = chars[index];
                if (patch == null)
                    continue;

                DrawPatch(patch, drawX, drawY, scale);

                drawX += scale * patch.Width;
            }
        }

        public int MeasureChar(char ch, int scale)
        {
            if (ch >= chars.Length)
                return 0;

            if (ch == 32)
                return 4 * scale;

            var index = (int)ch;
            if (index is >= 'a' and <= 'z')
                index = index - 'a' + 'A';

            var patch = chars[index];
            if (patch == null)
                return 0;

            return scale * patch.Width;
        }

        public int MeasureText(IReadOnlyList<char> text, int scale)
        {
            var width = 0;

            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                    continue;

                if (ch == 32)
                {
                    width += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if (index is >= 'a' and <= 'z')
                    index = index - 'a' + 'A';

                var patch = chars[index];
                if (patch == null)
                    continue;

                width += scale * patch.Width;
            }

            return width;
        }

        public int MeasureText(string text, int scale)
        {
            var width = 0;

            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                    continue;

                if (ch == 32)
                {
                    width += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if (index is >= 'a' and <= 'z')
                    index = index - 'a' + 'A';

                var patch = chars[index];
                if (patch == null)
                    continue;

                width += scale * patch.Width;
            }

            return width;
        }

        public void FillRect(int x, int y, int w, int h, int color)
        {
            var x2 = x + w;
            for (var drawX = x; drawX < x2; drawX++)
            {
                var pos = Height * drawX + y;
                for (var i = 0; i < h; i++)
                {
                    Data[pos] = (byte)color;
                    pos++;
                }
            }
        }



        [Flags]
        private enum OutCode
        {
            Inside = 0,
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8
        }

        private OutCode ComputeOutCode(float x, float y)
        {
            var code = OutCode.Inside;

            if (x < 0)
                code |= OutCode.Left;
            else if (x > Width)
                code |= OutCode.Right;

            if (y < 0)
                code |= OutCode.Bottom;
            else if (y > Height)
                code |= OutCode.Top;

            return code;
        }

        public void DrawLine(float x1, float y1, float x2, float y2, int color)
        {
            var outCode1 = ComputeOutCode(x1, y1);
            var outCode2 = ComputeOutCode(x2, y2);

            var accept = false;

            while (true)
            {
                if ((outCode1 | outCode2) == 0)
                {
                    accept = true;
                    break;
                }

                if ((outCode1 & outCode2) != 0)
                    break;
                var x = 0.0F;
                var y = 0.0F;

                var outCodeOut = outCode2 > outCode1 ? outCode2 : outCode1;

                if ((outCodeOut & OutCode.Top) != 0)
                {
                    x = x1 + (x2 - x1) * (Height - y1) / (y2 - y1);
                    y = Height;
                }
                else if ((outCodeOut & OutCode.Bottom) != 0)
                {
                    x = x1 + (x2 - x1) * (0 - y1) / (y2 - y1);
                    y = 0;
                }
                else if ((outCodeOut & OutCode.Right) != 0)
                {
                    y = y1 + (y2 - y1) * (Width - x1) / (x2 - x1);
                    x = Width;
                }
                else if ((outCodeOut & OutCode.Left) != 0)
                {
                    y = y1 + (y2 - y1) * (0 - x1) / (x2 - x1);
                    x = 0;
                }

                if (outCodeOut == outCode1)
                {
                    x1 = x;
                    y1 = y;
                    outCode1 = ComputeOutCode(x1, y1);
                }
                else
                {
                    x2 = x;
                    y2 = y;
                    outCode2 = ComputeOutCode(x2, y2);
                }
            }

            if (accept)
            {
                var bx1 = Math.Clamp((int)x1, 0, Width - 1);
                var by1 = Math.Clamp((int)y1, 0, Height - 1);
                var bx2 = Math.Clamp((int)x2, 0, Width - 1);
                var by2 = Math.Clamp((int)y2, 0, Height - 1);
                Bresenham(bx1, by1, bx2, by2, color);
            }
        }

        private void Bresenham(int x1, int y1, int x2, int y2, int color)
        {
            var dx = x2 - x1;
            var ax = 2 * (dx < 0 ? -dx : dx);
            var sx = dx < 0 ? -1 : 1;

            var dy = y2 - y1;
            var ay = 2 * (dy < 0 ? -dy : dy);
            var sy = dy < 0 ? -1 : 1;

            var x = x1;
            var y = y1;

            if (ax > ay)
            {
                var d = ay - ax / 2;

                while (true)
                {
                    Data[Height * x + y] = (byte)color;

                    if (x == x2)
                        return;

                    if (d >= 0)
                    {
                        y += sy;
                        d -= ax;
                    }

                    x += sx;
                    d += ay;
                }
            }
            else
            {
                var d = ax - ay / 2;
                while (true)
                {
                    Data[Height * x + y] = (byte)color;

                    if (y == y2)
                        return;

                    if (d >= 0)
                    {
                        x += sx;
                        d -= ay;
                    }

                    y += sy;
                    d += ax;
                }
            }
        }

        public int Width { get; }

        public int Height { get; }

        public byte[] Data { get; }
    }
}
