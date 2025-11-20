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
using System.Collections;
using System.Collections.Generic;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Info;

namespace ManagedDoom.Doom.Graphics;

public sealed class TextureLookup : ITextureLookup
{
    private readonly List<Texture> textures = [];
    private readonly Dictionary<string, Texture> nameToTexture = [];
    private readonly Dictionary<string, int> nameToNumber = [];

    public TextureLookup(Wad.Wad wad)
    {
        var patches = LoadPatches(wad);

        for (var n = 1; n <= 2; n++)
        {
            var lumpNumber = wad.GetLumpNumber($"TEXTURE{n}");
            if (lumpNumber == -1)
                break;

            var lumpData = wad.GetLumpData(lumpNumber);

            var count = BitConverter.ToInt32(lumpData);
            for (var i = 0; i < count; i++)
            {
                var offset = BitConverter.ToInt32(lumpData[(4 + 4 * i)..]);
                var texture = Texture.FromData(lumpData, offset, patches);
                nameToNumber.TryAdd(texture.Name, textures.Count);
                textures.Add(texture);
                nameToTexture.TryAdd(texture.Name, texture);
            }
        }

        SwitchList = CreateSwitchList();
    }

    public Texture this[int num] => textures[num];
    public Texture this[string name] => nameToTexture[name];
    public int Count => textures.Count;
    public int[] SwitchList { get; }

    private int[] CreateSwitchList()
    {
        var list = new List<int>(DoomInfo.SwitchNames.Length);
        foreach (var (tex1, tex2) in DoomInfo.SwitchNames.AsSpan())
        {
            var texNum1 = GetNumber(tex1);
            var texNum2 = GetNumber(tex2);
            if (texNum1 != -1 && texNum2 != -1)
            {
                list.Add(texNum1);
                list.Add(texNum2);
            }
        }

        return [.. list];
    }

    public int GetNumber(string name)
    {
        if (name[0] == '-')
            return 0;

        if (nameToNumber.TryGetValue(name, out var number))
            return number;

        return -1;
    }

    private static Patch[] LoadPatches(Wad.Wad wad)
    {
        var patchNames = LoadPatchNames(wad);
        var patches = new Patch[patchNames.Length];
        for (var i = 0; i < patches.Length; i++)
        {
            var name = patchNames[i];

            var lumpNumber = wad.GetLumpNumber(name);

            // This check is necessary to avoid crash in DOOM1.WAD.
            if (lumpNumber == -1)
                continue;

            var lumpData = wad.ReadLump(lumpNumber);
            patches[i] = Patch.FromData(name, lumpData);
        }

        return patches;
    }

    private static string[] LoadPatchNames(Wad.Wad wad)
    {
        const string lumpName = "PNAMES";
        var lumpNumber = wad.GetLumpNumber(lumpName);
        var lumpData = wad.GetLumpData(lumpNumber);

        var count = BitConverter.ToInt32(lumpData[..4]);
        var names = new string[count];

        for (var i = 0; i < names.Length; i++)
            names[i] = DoomInterop.ToString(lumpData.Slice(4 + 8 * i, 8));

        return names;
    }

    public IEnumerator<Texture> GetEnumerator()
    {
        return textures.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return textures.GetEnumerator();
    }
}