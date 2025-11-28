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
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.World;
using ManagedDoom.Extensions;

namespace ManagedDoom.Doom.Math;

public static class Geometry
{
    private const uint SlopeRange = 2048;
    private const int SlopeBits = 11;
    private const int FracToSlopeShift = Fixed.FracBits - SlopeBits;

    private static uint SlopeDiv(Fixed num, Fixed den)
    {
        if ((uint)den.Data < 512)
            return SlopeRange;

        var ans = ((uint)num.Data << 3) / ((uint)den.Data >> 8);

        return ans <= SlopeRange ? ans : SlopeRange;
    }

    /// <summary>
    /// Calculate the distance between the two points.
    /// </summary>
    public static Fixed PointToDist(Fixed fromX, Fixed fromY, Fixed toX, Fixed toY)
    {
        var dx = Fixed.Abs(toX - fromX);
        var dy = Fixed.Abs(toY - fromY);

        if (dy > dx)
            (dx, dy) = (dy, dx);

        // The code below to avoid division by zero is based on Chocolate Doom's implementation.
        var frac = dx != Fixed.Zero ? dy / dx : Fixed.Zero;

        var angle = (Trig.TanToAngle((uint)frac.Data >> FracToSlopeShift) + Angle.Ang90);

        // Use as cosine.
        var dist = dx / Trig.Sin(angle);

        return dist;
    }

    /// <summary>
    /// Calculate on which side of the node the point is.
    /// </summary>
    /// <returns>
    /// 0 (front) or 1 (back).
    /// </returns>
    public static int PointOnSide(Fixed x, Fixed y, Node node)
    {
        if (node.Dx == Fixed.Zero)
            return x <= node.X ? (node.Dy > Fixed.Zero).AsInt() : (node.Dy < Fixed.Zero).AsInt();

        if (node.Dy == Fixed.Zero)
            return y <= node.Y ? (node.Dx < Fixed.Zero).AsInt() : (node.Dx > Fixed.Zero).AsInt();

        var dx = x - node.X;
        var dy = y - node.Y;

        // Try to quickly decide by looking at sign bits.
        if (((node.Dy.Data ^ node.Dx.Data ^ dx.Data ^ dy.Data) & 0x80000000) != 0)
        {
            // Left is negative.
            return (((node.Dy.Data ^ dx.Data) & 0x80000000) != 0).AsInt();
        }

        var left = new Fixed(node.Dy.Data >> Fixed.FracBits) * dx;
        var right = dy * new Fixed(node.Dx.Data >> Fixed.FracBits);

        // if right < left, front side, else back side
        return (right >= left).AsInt();
    }

    /// <summary>
    /// Calculate the angle of the line passing through the two points.
    /// </summary>
    public static Angle PointToAngle(Fixed fromX, Fixed fromY, Fixed toX, Fixed toY)
    {
        var x = toX - fromX;
        var y = toY - fromY;

        if (x == Fixed.Zero && y == Fixed.Zero)
            return Angle.Ang0;

        if (x >= Fixed.Zero)
        {
            // x >= 0
            if (y >= Fixed.Zero)
            {
                // y >= 0

                // octant 0
                if (x > y)
                    return Trig.TanToAngle(SlopeDiv(y, x));

                // octant 1
                return new Angle(Angle.Ang90.Data - 1) - Trig.TanToAngle(SlopeDiv(x, y));
            }

            // y < 0
            y = -y;

            // octant 8
            if (x > y)
                return -Trig.TanToAngle(SlopeDiv(y, x));

            // octant 7
            return Angle.Ang270 + Trig.TanToAngle(SlopeDiv(x, y));
        }

        // x < 0
        x = -x;

        // y >= 0
        if (y >= Fixed.Zero)
        {
            // octant 3
            if (x > y)
                return new Angle(Angle.Ang180.Data - 1) - Trig.TanToAngle(SlopeDiv(y, x));

            // octant 2
            return Angle.Ang90 + Trig.TanToAngle(SlopeDiv(x, y));
        }

        // y < 0
        y = -y;

        // octant 4
        if (x > y)
            return Angle.Ang180 + Trig.TanToAngle(SlopeDiv(y, x));

        // octant 5
        return new Angle(Angle.Ang270.Data - 1) - Trig.TanToAngle(SlopeDiv(x, y));
    }

    /// <summary>
    /// Get the subsector which contains the point.
    /// </summary>
    public static Subsector PointInSubsector(Fixed x, Fixed y, Map.Map map)
    {
        // Single subsector is a special case.
        if (map.Nodes.Length == 0)
            return map.Subsectors[0];

        var nodeNumber = map.Nodes.Length - 1;

        while (!Node.IsSubsector(nodeNumber))
        {
            var node = map.Nodes[nodeNumber];
            var side = PointOnSide(x, y, node);
            nodeNumber = node.Children[side];
        }

        return map.Subsectors[Node.GetSubsector(nodeNumber)];
    }

    /// <summary>
    /// Calculate on which side of the line the point is.
    /// </summary>
    /// <returns>
    /// 0 (front) or 1 (back).
    /// </returns>
    public static int PointOnSegSide(Fixed x, Fixed y, Seg line)
    {
        var lx = line.Vertex1.X;
        var ly = line.Vertex1.Y;

        var ldx = line.Vertex2.X - lx;
        var ldy = line.Vertex2.Y - ly;

        if (ldx == Fixed.Zero)
            return x <= lx ? (ldy > Fixed.Zero).AsInt() : (ldy < Fixed.Zero).AsInt();

        if (ldy == Fixed.Zero)
            return y <= ly ? (ldx < Fixed.Zero).AsInt() : (ldx > Fixed.Zero).AsInt();

        var dx = x - lx;
        var dy = y - ly;

        // Try to quickly decide by looking at sign bits.
        if (((ldy.Data ^ ldx.Data ^ dx.Data ^ dy.Data) & 0x80000000) != 0)
        {
            // Left is negative.
            return (((ldy.Data ^ dx.Data) & 0x80000000) != 0).AsInt();
        }

        var left = new Fixed(ldy.Data >> Fixed.FracBits) * dx;
        var right = dy * new Fixed(ldx.Data >> Fixed.FracBits);

        // Front side.
        if (right < left)
            return 0;

        // Back side.
        return 1;
    }

    /// <summary>
    /// Calculate on which side of the line the point is.
    /// </summary>
    /// <returns>
    /// 0 (front) or 1 (back).
    /// </returns>
    public static int PointOnLineSide(Fixed x, Fixed y, LineDef line)
    {
        if (line.Dx == Fixed.Zero)
            return x <= line.Vertex1.X ? (line.Dy > Fixed.Zero).AsInt() : (line.Dy < Fixed.Zero).AsInt();

        if (line.Dy == Fixed.Zero)
            return y <= line.Vertex1.Y ? (line.Dx < Fixed.Zero).AsInt() : (line.Dx > Fixed.Zero).AsInt();

        var dx = x - line.Vertex1.X;
        var dy = y - line.Vertex1.Y;

        var left = new Fixed(line.Dy.Data >> Fixed.FracBits) * dx;
        var right = dy * new Fixed(line.Dx.Data >> Fixed.FracBits);

        // If right < left, front side, else back side.
        return (right >= left).AsInt();
    }

    /// <summary>
    /// Calculate on which side of the line the box is.
    /// </summary>
    /// <returns>
    /// 0 (front), 1 (back), or -1 if the box crosses the line.
    /// </returns>
    public static int BoxOnLineSide(Fixed[] box, LineDef line)
    {
        int p1;
        int p2;

        switch (line.SlopeType)
        {
            case SlopeType.Horizontal:
                p1 = (box[Box.Top] > line.Vertex1.Y).AsInt();
                p2 = (box[Box.Bottom] > line.Vertex1.Y).AsInt();
                if (line.Dx < Fixed.Zero)
                {
                    p1 ^= 1;
                    p2 ^= 1;
                }

                break;

            case SlopeType.Vertical:
                p1 = (box[Box.Right] < line.Vertex1.X).AsInt();
                p2 = (box[Box.Left] < line.Vertex1.X).AsInt();
                if (line.Dy < Fixed.Zero)
                {
                    p1 ^= 1;
                    p2 ^= 1;
                }

                break;

            case SlopeType.Positive:
                p1 = PointOnLineSide(box[Box.Left], box[Box.Top], line);
                p2 = PointOnLineSide(box[Box.Right], box[Box.Bottom], line);
                break;

            case SlopeType.Negative:
                p1 = PointOnLineSide(box[Box.Right], box[Box.Top], line);
                p2 = PointOnLineSide(box[Box.Left], box[Box.Bottom], line);
                break;

            default:
                throw new Exception("Invalid SlopeType.");
        }

        return p1 == p2 ? p1 : -1;
    }

    /// <summary>
    /// Calculate on which side of the line the point is.
    /// </summary>
    /// <returns>
    /// 0 (front) or 1 (back).
    /// </returns>
    public static int PointOnDivLineSide(Fixed x, Fixed y, DivLine line)
    {
        if (line.Dx == Fixed.Zero)
            return x <= line.X ? (line.Dy > Fixed.Zero).AsInt() : (line.Dy < Fixed.Zero).AsInt();

        if (line.Dy == Fixed.Zero)
            return y <= line.Y ? (line.Dx < Fixed.Zero).AsInt() : (line.Dx > Fixed.Zero).AsInt();

        var dx = x - line.X;
        var dy = y - line.Y;

        // Try to quickly decide by looking at sign bits.
        if (((line.Dy.Data ^ line.Dx.Data ^ dx.Data ^ dy.Data) & 0x80000000) != 0)
        {
            // Left is negative.
            return (((line.Dy.Data ^ dx.Data) & 0x80000000) != 0).AsInt();
        }

        var left = new Fixed(line.Dy.Data >> 8) * new Fixed(dx.Data >> 8);
        var right = new Fixed(dy.Data >> 8) * new Fixed(line.Dx.Data >> 8);

        // Front side.
        if (right < left)
            return 0;

        // Back side.
        return 1;
    }

    /// <summary>
    /// Gives an estimation of distance (not exact).
    /// </summary>
    public static Fixed AproxDistance(Fixed dx, Fixed dy)
    {
        dx = Fixed.Abs(dx);
        dy = Fixed.Abs(dy);

        if (dx < dy)
            return dx + dy - (dx >> 1);

        return dx + dy - (dy >> 1);
    }

    /// <summary>
    /// Calculate on which side of the line the point is.
    /// </summary>
    /// <returns>
    /// 0 (front) or 1 (back), or 2 if the box crosses the line.
    /// </returns>
    public static int DivLineSide(Fixed x, Fixed y, DivLine line)
    {
        if (line.Dx == Fixed.Zero)
        {
            if (x == line.X)
                return 2;

            return x <= line.X ? (line.Dy > Fixed.Zero).AsInt() : (line.Dy < Fixed.Zero).AsInt();
        }

        if (line.Dy == Fixed.Zero)
        {
            if (x == line.Y)
                return 2;

            return y <= line.Y ? (line.Dx < Fixed.Zero).AsInt() : (line.Dx > Fixed.Zero).AsInt();
        }

        var dx = (x - line.X);
        var dy = (y - line.Y);

        var left = new Fixed((line.Dy.Data >> Fixed.FracBits) * (dx.Data >> Fixed.FracBits));
        var right = new Fixed((dy.Data >> Fixed.FracBits) * (line.Dx.Data >> Fixed.FracBits));

        // Front side.
        if (right < left)
            return 0;

        // If right == left, the point is on (or crosses) the line, otherwise the back side
        return (left == right).AsInt() + 1;
    }

    /// <summary>
    /// Calculate on which side of the line the point is.
    /// </summary>
    /// <returns>
    /// 0 (front) or 1 (back), or 2 if the box crosses the line.
    /// </returns>
    public static int DivLineSide(Fixed x, Fixed y, Node node)
    {
        if (node.Dx == Fixed.Zero)
        {
            if (x == node.X)
                return 2;

            return x <= node.X ? (node.Dy > Fixed.Zero).AsInt() : (node.Dy < Fixed.Zero).AsInt();
        }

        if (node.Dy == Fixed.Zero)
        {
            if (x == node.Y)
                return 2;

            return y <= node.Y ? (node.Dx < Fixed.Zero).AsInt() : (node.Dx > Fixed.Zero).AsInt();
        }

        var dx = x - node.X;
        var dy = y - node.Y;

        var left = new Fixed((node.Dy.Data >> Fixed.FracBits) * (dx.Data >> Fixed.FracBits));
        var right = new Fixed((dy.Data >> Fixed.FracBits) * (node.Dx.Data >> Fixed.FracBits));

        // Front side.
        if (right < left)
            return 0;

        // If right == left, the point is on (or crosses) the line, otherwise the back side
        return (left == right).AsInt() + 1;
    }
}