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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Map;

public sealed record Map(
    ITextureLookup Textures,
    IFlatLookup Flats,
    TextureAnimation Animation,
    Vertex[] Vertices,
    Sector[] Sectors,
    LineDef[] Lines,
    Seg[] Segs,
    Subsector[] Subsectors,
    Node[] Nodes,
    MapThing[] Things,
    BlockMap BlockMap,
    Reject Reject,
    Texture SkyTexture,
    string Title
)
{
    public int SkyFlatNumber => Flats.SkyFlatNumber;
}

public static class MapExtensions
{
    public static Map Create(GameContent resources, World.World world)
    {
        return Create(resources.Wad, resources.Textures, resources.Flats, resources.Animation, world);
    }

    private static Map Create(Wad.Wad wad, ITextureLookup textures, IFlatLookup flats, TextureAnimation animation, World.World world)
    {
        try
        {
            var start = Stopwatch.GetTimestamp();

            var options = world.Options;

            var name = wad.GameMode == GameMode.Commercial
                ? $"MAP{options.Map:00}"
                : $"E{options.Episode}M{options.Map}";

            Console.Write($"Load map '{name}': ");

            var map = wad.GetLumpNumber(name);

            if (map == -1)
                throw new Exception($"Map '{name}' was not found!");

            var vertices = Vertex.FromWad(wad, map + 4);
            var sectors = Sector.FromWad(wad, map + 8, flats);
            var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            var segs = Seg.FromWad(wad, map + 5, vertices, lines);
            var subSectors = Subsector.FromWad(wad, map + 6, segs);
            var nodes = Node.FromWad(wad, map + 7);
            var things = MapThing.FromWad(wad, map + 1);
            var blockMap = BlockMap.FromWad(wad, map + 10, lines);
            var reject = Reject.FromWad(wad, map + 9, sectors);

            GroupLines(world, lines.AsSpan(), sectors.AsSpan(), blockMap);

            var skyTexture = GetSkyTextureByMapName(name, textures);

            var title = options.GameMode == GameMode.Commercial
                ? DoomInfo.MapTitles.GetMapTitle(options.MissionPack, options.Map - 1)
                : DoomInfo.MapTitles.GetMapTitle(options.Episode - 1, options.Map - 1);

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK [{end}]");

            return new Map(
                Textures: textures,
                Flats: flats,
                Animation: animation,
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
    private static void GroupLines(World.World world, ReadOnlySpan<LineDef> lines, ReadOnlySpan<Sector> sectors, BlockMap blockMap)
    {
        var sectorLines = new List<LineDef>();
        var boundingBox = new Fixed[4];

        foreach (var line in lines)
        {
            if (line.Special == 0) continue;
            line.SoundOrigin = new Mobj(world)
            {
                X = (line.Vertex1.X + line.Vertex2.X) / 2,
                Y = (line.Vertex1.Y + line.Vertex2.Y) / 2
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
            sector.SoundOrigin = new Mobj(world)
            {
                X = (boundingBox[Box.Right] + boundingBox[Box.Left]) / 2,
                Y = (boundingBox[Box.Top] + boundingBox[Box.Bottom]) / 2
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

    private static Texture GetSkyTextureByMapName(string name, ITextureLookup textures)
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

    private static readonly Bgm[] e4BgmList =
    [
        Bgm.E3M4, // American   e4m1
        Bgm.E3M2, // Romero     e4m2
        Bgm.E3M3, // Shawn      e4m3
        Bgm.E1M5, // American   e4m4
        Bgm.E2M7, // Tim        e4m5
        Bgm.E2M4, // Romero     e4m6
        Bgm.E2M6, // J.Anderson e4m7 CHIRON.WAD
        Bgm.E2M5, // Shawn      e4m8
        Bgm.E1M9  // Tim        e4m9
    ];

    public static Bgm GetMapBgm(GameOptions options)
    {
        if (options.GameMode == GameMode.Commercial)
            return Bgm.RUNNIN + options.Map - 1;

        if (options.Episode < 4)
            return Bgm.E1M1 + (options.Episode - 1) * 9 + options.Map - 1;

        return e4BgmList[options.Map - 1];
    }
}