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
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using ManagedDoom.Doom.Info;

namespace ManagedDoom.Doom.Graphics.Dummy;

public sealed class DummyTextureLookup : ITextureLookup
{
    private List<Texture> textures;
    private Dictionary<string, Texture> nameToTexture;
    private Dictionary<string, int> nameToNumber;

    public DummyTextureLookup(Wad.Wad wad)
    {
        InitLookup(wad);
        InitSwitchList();
    }


    public int Count => textures.Count;
    public Texture this[int num] => textures[num];
    public Texture this[string name] => nameToTexture[name];
    public int[] SwitchList { get; private set; }

    private void InitLookup(Wad.Wad wad)
    {
        textures = [];
        nameToTexture = new Dictionary<string, Texture>();
        nameToNumber = new Dictionary<string, int>();

        for (var n = 1; n <= 2; n++)
        {
            var lumpNumber = wad.GetLumpNumber($"TEXTURE{n}");
            if (lumpNumber == -1)
                break;

            var lumpSize = wad.GetLumpSize(lumpNumber);

            var lumpData = ArrayPool<byte>.Shared.Rent(lumpSize);

            try
            {
                var lumpBuffer = lumpData.AsSpan(0, lumpSize);
                wad.ReadLump(lumpNumber, lumpBuffer);

                var count = BitConverter.ToInt32(lumpBuffer);
                for (var i = 0; i < count; i++)
                {
                    var offset = BitConverter.ToInt32(lumpBuffer.Slice(4 + 4 * i, 4));
                    var name = Texture.GetName(lumpBuffer.Slice(offset));
                    var height = Texture.GetHeight(lumpBuffer, offset);
                    var texture = DummyData.GetTexture(height);
                    nameToNumber.TryAdd(name, textures.Count);
                    textures.Add(texture);
                    nameToTexture.TryAdd(name, texture);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(lumpData);
            }
        }
    }

    private void InitSwitchList()
    {
        var list = new List<int>();
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

        SwitchList = list.ToArray();
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