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

using System.Collections;
using System.Collections.Generic;

namespace ManagedDoom.Doom.Graphics.Dummy;

public sealed class DummyFlatLookup : IFlatLookup
{
    private readonly Flat[] flats;

    private readonly Dictionary<string, Flat> nameToFlat;
    private readonly Dictionary<string, int> nameToNumber;

    public DummyFlatLookup(Wad.Wad wad)
    {
        var firstFlat = wad.GetLumpNumber("F_START") + 1;
        var lastFlat = wad.GetLumpNumber("F_END") - 1;
        var count = lastFlat - firstFlat + 1;

        flats = new Flat[count];

        nameToFlat = new Dictionary<string, Flat>();
        nameToNumber = new Dictionary<string, int>();

        for (var lump = firstFlat; lump <= lastFlat; lump++)
        {
            if (wad.GetLumpSize(lump) != 4096)
            {
                continue;
            }

            var number = lump - firstFlat;
            var name = wad.LumpInfos[lump].Name;
            var flat = name != "F_SKY1" ? DummyData.GetFlat() : DummyData.GetSkyFlat();

            flats[number] = flat;
            nameToFlat[name] = flat;
            nameToNumber[name] = number;
        }

        SkyFlatNumber = nameToNumber["F_SKY1"];
        SkyFlat = nameToFlat["F_SKY1"];
    }

    public int Count => flats.Length;
    public Flat this[int num] => flats[num];
    public Flat this[string name] => nameToFlat[name];
    public int SkyFlatNumber { get; }
    public Flat SkyFlat { get; }

    public int GetNumber(string name)
    {
        if (nameToNumber.TryGetValue(name, out var number))
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