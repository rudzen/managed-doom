//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
// Copyright (C)      2025 Rudy Alex Kohn
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
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.Map;

/// <summary>
/// Factory functions for creating map-related objects.
/// </summary>
public static class MapFactory
{
    public static Vertex[] CreateVertices(Wad.Wad wad, int lump)
    {
        const int dataSize = 4;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var vertices = new Vertex[count];

        for (var i = 0; i < vertices.Length; i++)
        {
            var offset = dataSize * i;
            vertices[i] = CreateVertex(lumpData[offset..]);
        }

        return vertices;
    }

    private static Vertex CreateVertex(ReadOnlySpan<byte> data)
    {
        var x = BitConverter.ToInt16(data);
        var y = BitConverter.ToInt16(data.Slice(2, 2));

        return new Vertex(Fixed.FromInt(x), Fixed.FromInt(y));
    }

    public static Subsector[] CreateSubSectors(Wad.Wad wad, int lump, Seg[] segments)
    {
        const int DataSize = 4;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % DataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / DataSize;
        var subSectors = new Subsector[count];

        for (var i = 0; i < subSectors.Length; i++)
        {
            var offset = DataSize * i;
            subSectors[i] = CreateSubSector(lumpData.Slice(offset, DataSize), segments);
        }

        return subSectors;
    }

    private static Subsector CreateSubSector(ReadOnlySpan<byte> data, ReadOnlySpan<Seg> segments)
    {
        var segCount = BitConverter.ToInt16(data[..2]);
        var firstSegNumber = BitConverter.ToInt16(data.Slice(2, 2));

        return new Subsector(
            segments[firstSegNumber].SideDef!.Sector!,
            segCount,
            firstSegNumber);
    }

    public static SideDef[] CreateSideDefs(Wad.Wad wad, int lump, ITextureLookup textures, ReadOnlySpan<Sector> sectors)
    {
        const int dataSize = 30;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var sides = new SideDef[count];

        for (var i = 0; i < sides.Length; i++)
        {
            var offset = dataSize * i;
            sides[i] = CreateSideDef(lumpData[offset..], textures, sectors);
        }

        return sides;
    }

    private static SideDef CreateSideDef(ReadOnlySpan<byte> data, ITextureLookup textures, ReadOnlySpan<Sector> sectors)
    {
        var textureOffset = BitConverter.ToInt16(data[..2]);
        var rowOffset = BitConverter.ToInt16(data.Slice(2, 2));
        var topTextureName = DoomInterop.ToString(data.Slice(4, 8));
        var bottomTextureName = DoomInterop.ToString(data.Slice(12, 8));
        var middleTextureName = DoomInterop.ToString(data.Slice(20, 8));
        var sectorNum = BitConverter.ToInt16(data.Slice(28, 2));

        return new SideDef(
            Fixed.FromInt(textureOffset),
            Fixed.FromInt(rowOffset),
            textures.GetNumber(topTextureName),
            textures.GetNumber(bottomTextureName),
            textures.GetNumber(middleTextureName),
            sectorNum != -1 ? sectors[sectorNum] : null);
    }

    public static Seg[] CreateSegs(Wad.Wad wad, int lump, ReadOnlySpan<Vertex> vertices, LineDef[] lines)
    {
        const int dataSize = 12;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var segments = new Seg[count];

        for (var i = 0; i < segments.Length; i++)
        {
            var offset = dataSize * i;
            segments[i] = CreateSeg(lumpData.Slice(offset, dataSize), vertices, lines);
        }

        return segments;
    }

    private static Seg CreateSeg(
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

    public static Reject CreateReject(Wad.Wad wad, int lump, Sector[] sectors)
    {
        // TODO (rudzen) : add a clever way to ready this lump with auto resize of buffer
        return new Reject(wad.ReadLump(lump), sectors.Length);
    }

    public static Sector[] CreateSectors(Wad.Wad wad, int lump, IFlatLookup flats)
    {
        const int DataSize = 26;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % DataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / DataSize;
        var sectors = new Sector[count];

        for (var i = 0; i < sectors.Length; i++)
        {
            var offset = DataSize * i;
            sectors[i] = CreateSector(lumpData[offset..], i, flats);
        }

        return sectors;
    }

    private static Sector CreateSector(ReadOnlySpan<byte> data, int number, IFlatLookup flats)
    {
        var floorHeight = BitConverter.ToInt16(data[..2]);
        var ceilingHeight = BitConverter.ToInt16(data.Slice(2, 2));
        var floorFlatName = DoomInterop.ToString(data.Slice(4, 8));
        var ceilingFlatName = DoomInterop.ToString(data.Slice(12, 8));
        var lightLevel = BitConverter.ToInt16(data.Slice(20, 2));
        var special = BitConverter.ToInt16(data.Slice(22, 2));
        var tag = BitConverter.ToInt16(data.Slice(24, 2));

        return new Sector(
            number,
            Fixed.FromInt(floorHeight),
            Fixed.FromInt(ceilingHeight),
            flats.GetNumber(floorFlatName),
            flats.GetNumber(ceilingFlatName),
            lightLevel,
            (SectorSpecial)special,
            tag);
    }

    public static Node[] CreateNodes(Wad.Wad wad, int lump)
    {
        const int dataSize = 28;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var nodes = new Node[count];

        for (var i = 0; i < nodes.Length; i++)
        {
            var offset = dataSize * i;
            nodes[i] = CreateNode(lumpData.Slice(offset, dataSize));
        }

        return nodes;
    }

    private static Node CreateNode(ReadOnlySpan<byte> data)
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


    public static BlockMap CreateBlockMap(Wad.Wad wad, int lump, LineDef[] lines)
    {
        var lumpSize = wad.GetLumpSize(lump);
        var lumpData = wad.GetLumpData(lump);

        var table = new short[lumpSize / 2];
        for (var i = 0; i < table.Length; i++)
        {
            var offset = 2 * i;
            table[i] = BitConverter.ToInt16(lumpData.Slice(offset, 2));
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

    public static MapThing[] CreateMapThings(Wad.Wad wad, int lump)
    {
        const int dataSize = 10;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var things = new MapThing[count];

        for (var i = 0; i < things.Length; i++)
        {
            var offset = dataSize * i;
            things[i] = CreateMapThing(lumpData.Slice(offset, dataSize));
        }

        return things;
    }

    private static MapThing CreateMapThing(ReadOnlySpan<byte> data)
    {
        var x = BitConverter.ToInt16(data[..2]);
        var y = BitConverter.ToInt16(data.Slice(2, 2));
        var angle = BitConverter.ToInt16(data.Slice(4, 2));
        var type = BitConverter.ToInt16(data.Slice(6, 2));
        var flags = BitConverter.ToInt16(data.Slice(8, 2));

        return new MapThing(
            Fixed.FromInt(x),
            Fixed.FromInt(y),
            new Angle(Angle.Ang45.Data * (uint)(angle / 45)),
            type,
            (ThingFlags)flags
        );
    }

    public static LineDef[] CreateLineDefs(Wad.Wad wad, int lump, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<SideDef> sides)
    {
        const int dataSize = 14;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);
        var count = lumpSize / dataSize;
        var lines = new LineDef[count];

        for (var i = 0; i < lines.Length; i++)
        {
            var offset = 14 * i;
            lines[i] = CreateLineDef(lumpData[offset..], vertices, sides);
        }

        return lines;
    }

    private static LineDef CreateLineDef(ReadOnlySpan<byte> data, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<SideDef> sides)
    {
        var vertex1Number = BitConverter.ToInt16(data[..2]);
        var vertex2Number = BitConverter.ToInt16(data.Slice(2, 2));
        var flags = BitConverter.ToInt16(data.Slice(4, 2));
        var special = BitConverter.ToInt16(data.Slice(6, 2));
        var tag = BitConverter.ToInt16(data.Slice(8, 2));
        var side0Number = BitConverter.ToInt16(data.Slice(10, 2));
        var side1Number = BitConverter.ToInt16(data.Slice(12, 2));

        return new LineDef(
            vertex1: vertices[vertex1Number],
            vertex2: vertices[vertex2Number],
            flags: (LineFlags)flags,
            special: (LineSpecial)special,
            tag: tag,
            frontSide: sides[side0Number],
            backSide: side1Number != -1 ? sides[side1Number] : null
        );
    }
}