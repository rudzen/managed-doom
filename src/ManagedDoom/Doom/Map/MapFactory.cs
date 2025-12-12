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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Map;

/// <summary>
/// Factory functions for creating map-related objects.
/// </summary>
public static class MapFactory
{
    public static Map CreateMap(GameContent resources, World.World world)
    {
        return CreateMap(resources.Wad, resources.Textures, resources.Flats, resources.Animations, world);
    }

    private static Map CreateMap(Wad wad, ITextureLookup textures, IFlatLookup flats, TextureAnimationInfo[] animations, World.World world)
    {
        var start = Stopwatch.GetTimestamp();

        var options = world.Options;

        var name = wad.GameMode == GameMode.Commercial
            ? $"MAP{options.Map:00}"
            : $"E{options.Episode}M{options.Map}";

        Console.Write($"Load map '{name}': ");

        var map = wad.GetLumpNumber(name);

        try
        {
            if (map == -1)
                throw new Exception($"Map '{name}' was not found!");

            var vertices = CreateVertices(wad, map + 4);
            var sectors = CreateSectors(wad, map + 8, flats);
            var sides = CreateSideDefs(wad, map + 3, textures, sectors);
            var lines = CreateLineDefs(wad, map + 2, vertices, sides);
            var segs = CreateSegs(wad, map + 5, vertices, lines);
            var subSectors = CreateSubSectors(wad, map + 6, segs);
            var nodes = CreateNodes(wad, map + 7);
            var things = CreateMapThings(wad, map + 1);
            var blockMap = CreateBlockMap(wad, map + 10, lines);
            var reject = CreateReject(wad, map + 9, sectors);

            GroupMapLines(world, lines.AsSpan(), sectors.AsSpan(), blockMap);

            var skyTexture = GetMapSkyTextureByMapName(name, textures);

            var title = options.GameMode == GameMode.Commercial
                ? DoomInfo.MapTitles.GetMapTitle(options.MissionPack, options.Map - 1)
                : DoomInfo.MapTitles.GetMapTitle(options.Episode - 1, options.Map - 1);

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK [{end}]");

            return new Map(
                Textures: textures,
                Flats: flats,
                Animations: animations,
                Vertices: vertices,
                Sectors: sectors,
                Lines: lines,
                Segs: segs,
                Subsectors: subSectors,
                Nodes: nodes,
                Things: things,
                BlockMap: blockMap,
                Reject: reject,
                SkyTexture: skyTexture,
                Title: title
            );
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }

        return null!;
    }

    [SkipLocalsInit]
    private static void GroupMapLines(World.World world, ReadOnlySpan<LineDef> lines, ReadOnlySpan<Sector> sectors, BlockMap blockMap)
    {
        var sectorLines = new List<LineDef>(lines.Length);
        var boundingBox = new Fixed[4];

        foreach (var line in lines)
        {
            if (line.Special == 0) continue;
            var x = (line.Vertex1.X + line.Vertex2.X) / 2;
            var y = (line.Vertex1.Y + line.Vertex2.Y) / 2;
            line.SoundOrigin = new Mobj(world)
            {
                X = x,
                Y = y
            };
        }

        foreach (var sector in sectors)
        {
            sectorLines.Clear();
            boundingBox.Clear();

            foreach (var line in lines)
            {
                if (line.FrontSector != sector && line.BackSector != sector)
                    continue;

                sectorLines.Add(line);

                boundingBox.AddPoint(line.Vertex1);
                boundingBox.AddPoint(line.Vertex2);
            }

            sector.Lines = [.. sectorLines];

            // Set the degenmobj_t to the middle of the bounding box.
            var x = (boundingBox[Box.Right] + boundingBox[Box.Left]) / 2;
            var y = (boundingBox[Box.Top] + boundingBox[Box.Bottom]) / 2;
            sector.SoundOrigin = new Mobj(world)
            {
                X = x,
                Y = y
            };

            sector.BlockBox = new int[4];

            // Adjust bounding box to map blocks.
            var block = (boundingBox[Box.Top] - blockMap.OriginY + GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
            block = block >= blockMap.Height ? blockMap.Height - 1 : block;
            sector.BlockBox[Box.Top] = block;

            block = (boundingBox[Box.Bottom] - blockMap.OriginY - GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
            block = block < 0 ? 0 : block;
            sector.BlockBox[Box.Bottom] = block;

            block = (boundingBox[Box.Right] - blockMap.OriginX + GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
            block = block >= blockMap.Width ? blockMap.Width - 1 : block;
            sector.BlockBox[Box.Right] = block;

            block = (boundingBox[Box.Left] - blockMap.OriginX - GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
            block = block < 0 ? 0 : block;
            sector.BlockBox[Box.Left] = block;
        }
    }

    private static Texture GetMapSkyTextureByMapName(string name, ITextureLookup textures)
    {
        if (name.Length == 4)
            return textures[$"SKY{name[1]}"];

        var number = int.Parse(name[3..]);
        return number switch
        {
            <= 11 => textures["SKY1"],
            <= 21 => textures["SKY2"],
            _     => textures["SKY3"]
        };
    }

    public static Vertex[] CreateVertices(Wad wad, int lump)
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

    public static Subsector[] CreateSubSectors(Wad wad, int lump, Seg[] segments)
    {
        const int dataSize = 4;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var subSectors = new Subsector[count];

        for (var i = 0; i < subSectors.Length; i++)
        {
            var offset = dataSize * i;
            subSectors[i] = CreateSubSector(lumpData.Slice(offset, dataSize), segments);
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

    public static SideDef[] CreateSideDefs(Wad wad, int lump, ITextureLookup textures, ReadOnlySpan<Sector> sectors)
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

    public static Seg[] CreateSegs(Wad wad, int lump, ReadOnlySpan<Vertex> vertices, LineDef[] lines)
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

        SideDef frontSide;
        SideDef backSide;

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

    public static Reject CreateReject(Wad wad, int lump, Sector[] sectors)
    {
        // TODO (rudzen) : add a clever way to ready this lump with auto resize of buffer

        var data = wad.ReadLump(lump);
        var sectorCount = sectors.Length;

        // If the reject table is too small, expand it to avoid crash.
        // https://doomwiki.org/wiki/Reject#Reject_Overflow
        var expectedLength = (sectorCount * sectorCount + 7) / 8;
        if (data.Length < expectedLength)
        {
            Console.WriteLine($"Warning: Reject table is too small ({data.Length} bytes). Expanding to {expectedLength} bytes.");
            Array.Resize(ref data, expectedLength);
        }

        return new Reject(data, sectorCount);
    }

    public static Sector[] CreateSectors(Wad wad, int lump, IFlatLookup flats)
    {
        const int dataSize = 26;

        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % dataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / dataSize;
        var sectors = new Sector[count];

        for (var i = 0; i < sectors.Length; i++)
        {
            var offset = dataSize * i;
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

    public static Node[] CreateNodes(Wad wad, int lump)
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
        var x = Fixed.FromInt(BitConverter.ToInt16(data[..2]));
        var y = Fixed.FromInt(BitConverter.ToInt16(data.Slice(2, 2)));
        var dx = Fixed.FromInt(BitConverter.ToInt16(data.Slice(4, 2)));
        var dy = Fixed.FromInt(BitConverter.ToInt16(data.Slice(6, 2)));
        var frontBoundingBoxTop = Fixed.FromInt(BitConverter.ToInt16(data.Slice(8, 2)));
        var frontBoundingBoxBottom = Fixed.FromInt(BitConverter.ToInt16(data.Slice(10, 2)));
        var frontBoundingBoxLeft = Fixed.FromInt(BitConverter.ToInt16(data.Slice(12, 2)));
        var frontBoundingBoxRight = Fixed.FromInt(BitConverter.ToInt16(data.Slice(14, 2)));
        var backBoundingBoxTop = Fixed.FromInt(BitConverter.ToInt16(data.Slice(16, 2)));
        var backBoundingBoxBottom = Fixed.FromInt(BitConverter.ToInt16(data.Slice(18, 2)));
        var backBoundingBoxLeft = Fixed.FromInt(BitConverter.ToInt16(data.Slice(20, 2)));
        var backBoundingBoxRight = Fixed.FromInt(BitConverter.ToInt16(data.Slice(22, 2)));
        var frontChild = BitConverter.ToInt16(data.Slice(24, 2));
        var backChild = BitConverter.ToInt16(data.Slice(26, 2));

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

        return new Node(
            X: x,
            Y: y,
            Dx: dx,
            Dy: dy,
            BoundingBox: [frontBoundingBox, backBoundingBox],
            Children: [frontChild, backChild]
        );
    }

    public static Node CreateNode(
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

        Fixed[][] boundingBox =
        [
            frontBoundingBox,
            backBoundingBox
        ];

        int[] children =
        [
            frontChild,
            backChild
        ];

        return new Node(
            X: x,
            Y: y,
            Dx: dx,
            Dy: dy,
            BoundingBox: boundingBox,
            Children: children
        );
    }

    public static BlockMap CreateBlockMap(Wad wad, int lump, LineDef[] lines)
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

    public static MapThing[] CreateMapThings(Wad wad, int lump)
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

    public static LineDef[] CreateLineDefs(Wad wad, int lump, ReadOnlySpan<Vertex> vertices, ReadOnlySpan<SideDef> sides)
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