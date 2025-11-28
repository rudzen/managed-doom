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

namespace ManagedDoom.Doom.Graphics.Dummy;

public sealed class DummyFlatLookup : IFlatLookup
{
    private readonly Flat[] flats;

    private readonly FrozenDictionary<string, Flat> nameToFlat;
    private readonly FrozenDictionary<string, Flat>.AlternateLookup<ReadOnlySpan<char>> nameToFlatLookup;

    private readonly FrozenDictionary<string, int> nameToNumber;
    private readonly FrozenDictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> nameToNumberLookup;

    public DummyFlatLookup(Wad.Wad wad)
    {
        var firstFlat = wad.GetLumpNumber("F_START") + 1;
        var lastFlat = wad.GetLumpNumber("F_END") - 1;
        var count = lastFlat - firstFlat + 1;

        flats = new Flat[count];

        var nameToNumberLocal = new Dictionary<string, int>(256);
        var nameToFlatLocal = new Dictionary<string, Flat>(256);

        for (var lump = firstFlat; lump <= lastFlat; lump++)
        {
            if (wad.GetLumpSize(lump) != 4096)
                continue;

            var number = lump - firstFlat;
            var name = wad.LumpInfos[lump].Name;
            var flat = name != "F_SKY1" ? DummyData.GetFlat() : DummyData.GetSkyFlat();

            flats[number] = flat;
            nameToFlatLocal[name] = flat;
            nameToNumberLocal[name] = number;
        }

        SkyFlatNumber = nameToNumberLocal["F_SKY1"];
        SkyFlat = nameToFlatLocal["F_SKY1"];

        nameToNumber = nameToNumberLocal.ToFrozenDictionary();
        nameToNumberLookup = nameToNumber.GetAlternateLookup<ReadOnlySpan<char>>();

        nameToFlat = nameToFlatLocal.ToFrozenDictionary();
        nameToFlatLookup = nameToFlat.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    public int Count => flats.Length;
    public Flat this[int num] => flats[num];
    public Flat this[string name] => nameToFlat[name];
    public Flat this[ReadOnlySpan<char> name] => nameToFlatLookup[name];
    public int SkyFlatNumber { get; }
    public Flat SkyFlat { get; }

    public int GetNumber(string name)
    {
        if (nameToNumber.TryGetValue(name, out var number))
            return number;

        return -1;
    }

    public int GetNumber(ReadOnlySpan<char> name)
    {
        if (nameToNumberLookup.TryGetValue(name, out var number))
            return number;

        return -1;
    }

    public IEnumerator<Flat> GetEnumerator()
    {
        return ((IEnumerable<Flat>)flats).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return flats.GetEnumerator();
    }
}