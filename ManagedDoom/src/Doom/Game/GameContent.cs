﻿//
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
using ManagedDoom.Config;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Graphics.Dummy;

namespace ManagedDoom.Doom.Game;

public sealed class GameContent : IDisposable
{
    private GameContent(string[] wadPaths)
    {
        Wad = new Wad.Wad(wadPaths);
        Palette = new Palette(Wad);
        ColorMap = new ColorMap(Wad);
        Textures = new DummyTextureLookup(Wad);
        Flats = new DummyFlatLookup(Wad);
        Sprites = new DummySpriteLookup(Wad);
        Animation = new TextureAnimation(Textures, Flats);
    }

    public GameContent(CommandLineArgs args)
    {
        Wad = new Wad.Wad(ConfigUtilities.GetWadPaths(args));

        DeHackEd.Initialize(args, Wad);

        Palette = new Palette(Wad);
        ColorMap = new ColorMap(Wad);
        Textures = new TextureLookup(Wad);
        Flats = new FlatLookup(Wad);
        Sprites = new SpriteLookup(Wad);
        Animation = new TextureAnimation(Textures, Flats);
    }


    public Wad.Wad Wad { get; private set; }

    public Palette Palette { get; private set; }

    public ColorMap ColorMap { get; private set; }

    public ITextureLookup Textures { get; private set; }

    public IFlatLookup Flats { get; private set; }

    public ISpriteLookup Sprites { get; private set; }

    public TextureAnimation Animation { get; private set; }

    public static GameContent CreateDummy(params string[] wadPaths)
    {
        var gc = new GameContent(wadPaths);

        return gc;
    }

    public void Dispose()
    {
        if (Wad is null) return;
        Wad.Dispose();
        Wad = null;
    }
}