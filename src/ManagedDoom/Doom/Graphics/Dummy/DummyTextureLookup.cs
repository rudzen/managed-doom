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
using System.Collections.Frozen;
using System.Collections.Generic;
using ManagedDoom.Doom.Info;

namespace ManagedDoom.Doom.Graphics.Dummy;

public sealed class DummyTextureLookup : ITextureLookup
{
    private readonly List<Texture> textures = [];

    private readonly FrozenDictionary<string, Texture> nameToTexture;
    private readonly FrozenDictionary<string, Texture>.AlternateLookup<ReadOnlySpan<char>> nameToTextureLookup;

    private readonly FrozenDictionary<string, int> nameToNumber;
    private readonly FrozenDictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> nameToNumberLookup;

    public DummyTextureLookup(Wad.Wad wad)
    {
        var nameToTextureLocal = new Dictionary<string, Texture>(512);
        var nameToNumberLocal = new Dictionary<string, int>(512);

        for (var n = 1; n <= 2; n++)
        {
            var lumpNumber = wad.GetLumpNumber($"TEXTURE{n}");
            if (lumpNumber == -1)
                break;

            var lumpData = wad.GetLumpData(lumpNumber);

            var count = BitConverter.ToInt32(lumpData);
            for (var i = 0; i < count; i++)
            {
                var offset = BitConverter.ToInt32(lumpData.Slice(4 + 4 * i, 4));
                var name = Texture.GetName(lumpData[offset..]);
                var height = Texture.GetHeight(lumpData, offset);
                var texture = DummyData.GetTexture(height);
                nameToNumberLocal.TryAdd(name, textures.Count);
                textures.Add(texture);
                nameToTextureLocal.TryAdd(name, texture);
            }
        }

        this.nameToTexture = nameToTextureLocal.ToFrozenDictionary();
        this.nameToTextureLookup = this.nameToTexture.GetAlternateLookup<ReadOnlySpan<char>>();

        this.nameToNumber = nameToNumberLocal.ToFrozenDictionary();
        this.nameToNumberLookup = this.nameToNumber.GetAlternateLookup<ReadOnlySpan<char>>();

        SwitchList = CreateSwitchList();
    }

    public Texture this[int num] => textures[num];
    public Texture this[string name] => nameToTexture[name];
    public Texture this[ReadOnlySpan<char> name] => nameToTextureLookup[name];

    public int Count => textures.Count;
    public int[] SwitchList { get; }

    private int[] CreateSwitchList()
    {
        var list = new List<int>(DoomInfo.SwitchNames.Length);
        foreach (var (tex1, tex2) in DoomInfo.SwitchNames)
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

    public int GetNumber(ReadOnlySpan<char> name)
    {
        if (name[0] == '-')
            return 0;

        if (nameToNumberLookup.TryGetValue(name, out var number))
            return number;

        return -1;
    }

    public int GetNumber(string name)
    {
        if (name[0] == '-')
            return 0;

        if (nameToNumber.TryGetValue(name, out var number))
            return number;

        return -1;
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