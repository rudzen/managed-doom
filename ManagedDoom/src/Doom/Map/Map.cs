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
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace ManagedDoom
{
    public sealed class Map
    {
        private readonly World world;

        public Map(GameContent resources, World world)
            : this(resources.Wad, resources.Textures, resources.Flats, resources.Animation, world)
        {
        }

        public Map(Wad wad, ITextureLookup textures, IFlatLookup flats, TextureAnimation animation, World world)
        {
            try
            {
                var start = Stopwatch.GetTimestamp();

                this.Textures = textures;
                this.Flats = flats;
                this.Animation = animation;
                this.world = world;

                var options = world.Options;

                var name = wad.GameMode == GameMode.Commercial
                    ? "MAP" + options.Map.ToString("00")
                    : "E" + options.Episode + "M" + options.Map;

                Console.Write($"Load map '{name}': ");

                var map = wad.GetLumpNumber(name);

                if (map == -1)
                    throw new Exception("Map '" + name + "' was not found!");

                Vertices = Vertex.FromWad(wad, map + 4);
                Sectors = Sector.FromWad(wad, map + 8, flats);
                Sides = SideDef.FromWad(wad, map + 3, textures, Sectors);
                Lines = LineDef.FromWad(wad, map + 2, Vertices, Sides);
                Segs = Seg.FromWad(wad, map + 5, Vertices, Lines);
                Subsectors = Subsector.FromWad(wad, map + 6, Segs);
                Nodes = Node.FromWad(wad, map + 7, Subsectors);
                Things = MapThing.FromWad(wad, map + 1);
                BlockMap = BlockMap.FromWad(wad, map + 10, Lines);
                Reject = Reject.FromWad(wad, map + 9, Sectors);

                GroupLines();

                SkyTexture = GetSkyTextureByMapName(name);

                Title = options.GameMode == GameMode.Commercial
                    ? DoomInfo.MapTitles.GetMapTitle(options.MissionPack, options.Map - 1)
                    : DoomInfo.MapTitles.Doom[options.Episode - 1][options.Map - 1];

                Console.WriteLine("OK [" + Stopwatch.GetElapsedTime(start) + ']');
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private void GroupLines()
        {
            var sectorLines = new List<LineDef>();
            var boundingBox = new Fixed[4];

            foreach (var line in Lines)
            {
                if (line.Special == 0) continue;
                line.SoundOrigin = new Mobj(world)
                {
                    X = (line.Vertex1.X + line.Vertex2.X) / 2,
                    Y = (line.Vertex1.Y + line.Vertex2.Y) / 2
                };
            }

            foreach (var sector in Sectors)
            {
                sectorLines.Clear();
                Box.Clear(boundingBox);

                foreach (var line in Lines)
                {
                    if (line.FrontSector != sector && line.BackSector != sector) continue;
                    sectorLines.Add(line);
                    Box.AddPoint(boundingBox, line.Vertex1.X, line.Vertex1.Y);
                    Box.AddPoint(boundingBox, line.Vertex2.X, line.Vertex2.Y);
                }

                sector.Lines = sectorLines.ToArray();

                // Set the degenmobj_t to the middle of the bounding box.
                sector.SoundOrigin = new Mobj(world)
                {
                    X = (boundingBox[Box.Right] + boundingBox[Box.Left]) / 2,
                    Y = (boundingBox[Box.Top] + boundingBox[Box.Bottom]) / 2
                };

                sector.BlockBox = new int[4];

                // Adjust bounding box to map blocks.
                var block = (boundingBox[Box.Top] - BlockMap.OriginY + GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block >= BlockMap.Height ? BlockMap.Height - 1 : block;
                sector.BlockBox[Box.Top] = block;

                block = (boundingBox[Box.Bottom] - BlockMap.OriginY - GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block < 0 ? 0 : block;
                sector.BlockBox[Box.Bottom] = block;

                block = (boundingBox[Box.Right] - BlockMap.OriginX + GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block >= BlockMap.Width ? BlockMap.Width - 1 : block;
                sector.BlockBox[Box.Right] = block;

                block = (boundingBox[Box.Left] - BlockMap.OriginX - GameConst.MaxThingRadius).Data >> BlockMap.FracToBlockShift;
                block = block < 0 ? 0 : block;
                sector.BlockBox[Box.Left] = block;
            }
        }

        private Texture GetSkyTextureByMapName(string name)
        {
            if (name.Length == 4)
                return Textures[$"SKY{name[1]}"];

            var number = int.Parse(name[3..]);
            return number switch
            {
                <= 11 => Textures["SKY1"],
                <= 21 => Textures["SKY2"],
                _     => Textures["SKY3"]
            };
        }

        public ITextureLookup Textures { get; }

        public IFlatLookup Flats { get; }

        public TextureAnimation Animation { get; }

        public Vertex[] Vertices { get; }

        public Sector[] Sectors { get; }

        public SideDef[] Sides { get; }

        public LineDef[] Lines { get; }

        public Seg[] Segs { get; }

        public Subsector[] Subsectors { get; }

        public Node[] Nodes { get; }

        public MapThing[] Things { get; }

        public BlockMap BlockMap { get; }

        public Reject Reject { get; }

        public Texture SkyTexture { get; }

        public int SkyFlatNumber => Flats.SkyFlatNumber;
        public string Title { get; }


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
            Bgm bgm;
            if (options.GameMode == GameMode.Commercial)
                bgm = Bgm.RUNNIN + options.Map - 1;
            else
            {
                if (options.Episode < 4)
                    bgm = Bgm.E1M1 + (options.Episode - 1) * 9 + options.Map - 1;
                else
                    bgm = e4BgmList[options.Map - 1];
            }

            return bgm;
        }
    }
}