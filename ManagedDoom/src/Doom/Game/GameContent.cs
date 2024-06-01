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
    public sealed class GameContent : IDisposable
    {
        private GameContent()
        {
        }

        public GameContent(CommandLineArgs args)
        {
            Wad = new Wad(ConfigUtilities.GetWadPaths(args));

            DeHackEd.Initialize(args, Wad);

            Palette = new Palette(Wad);
            ColorMap = new ColorMap(Wad);
            Textures = new TextureLookup(Wad);
            Flats = new FlatLookup(Wad);
            Sprites = new SpriteLookup(Wad);
            Animation = new TextureAnimation(Textures, Flats);
        }

        public static GameContent CreateDummy(params string[] wadPaths)
        {
            var gc = new GameContent();

            gc.Wad = new Wad(wadPaths);
            gc.Palette = new Palette(gc.Wad);
            gc.ColorMap = new ColorMap(gc.Wad);
            gc.Textures = new DummyTextureLookup(gc.Wad);
            gc.Flats = new DummyFlatLookup(gc.Wad);
            gc.Sprites = new DummySpriteLookup(gc.Wad);
            gc.Animation = new TextureAnimation(gc.Textures, gc.Flats);

            return gc;
        }

        public void Dispose()
        {
            if (Wad != null)
            {
                Wad.Dispose();
                Wad = null;
            }
        }

        public Wad Wad { get; private set; }

        public Palette Palette { get; private set; }

        public ColorMap ColorMap { get; private set; }

        public ITextureLookup Textures { get; private set; }

        public IFlatLookup Flats { get; private set; }

        public ISpriteLookup Sprites { get; private set; }

        public TextureAnimation Animation { get; private set; }
    }
}
