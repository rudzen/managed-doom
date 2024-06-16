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

using System.Collections.Generic;

namespace ManagedDoom.Doom.Graphics.Dummy;

public static class DummyData
{
    private const string Name = "DUMMY";

    private static Patch? _dummyPatch;
    private static Flat? _dummyFlat;
    private static Flat? _dummySkyFlat;
    private static readonly Dictionary<int, Texture> dummyTextures = [];

    public static Patch GetPatch()
    {
        if (_dummyPatch is not null)
            return _dummyPatch;

        const int width = 64;
        const int height = 128;

        var data = new byte[height + 32];
        for (var y = 0; y < data.Length; y++)
            data[y] = y / 32 % 2 == 0 ? (byte)80 : (byte)96;

        var columns = new Column[width][];
        var c1 = new[] { new Column(0, data, 0, height) };
        var c2 = new[] { new Column(0, data, 32, height) };
        for (var x = 0; x < width; x++)
            columns[x] = x / 32 % 2 == 0 ? c1 : c2;

        _dummyPatch = new Patch(Name, width, height, 32, 128, columns);

        return _dummyPatch;
    }

    public static Texture GetTexture(int height)
    {
        if (dummyTextures.TryGetValue(height, out var texture))
            return texture;

        TexturePatch[] patch = [new TexturePatch(0, 0, GetPatch())];

        dummyTextures.Add(height, new Texture(Name, false, 64, height, patch));

        return dummyTextures[height];
    }

    public static Flat GetFlat()
    {
        if (_dummyFlat is not null)
            return _dummyFlat;

        var data = new byte[64 * 64];
        var spot = 0;
        for (var y = 0; y < 64; y++)
        {
            for (var x = 0; x < 64; x++)
            {
                data[spot] = ((x / 32) ^ (y / 32)) == 0 ? (byte)80 : (byte)96;
                spot++;
            }
        }

        _dummyFlat = new Flat(Name, data);

        return _dummyFlat;
    }

    public static Flat GetSkyFlat()
    {
        if (_dummySkyFlat is not null)
            return _dummySkyFlat;

        _dummySkyFlat = new Flat(Name, GetFlat().Data);

        return _dummySkyFlat;
    }
}