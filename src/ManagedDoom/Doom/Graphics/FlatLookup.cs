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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace ManagedDoom.Doom.Graphics;

public sealed class FlatLookup : IFlatLookup
{
    private Flat[] flats;

    private FrozenDictionary<string, Flat> nameToFlat;
    private FrozenDictionary<string, int> nameToNumber;

    public FlatLookup(Wad.Wad wad)
    {
        var fStartCount = CountLump(wad, "F_START");
        var fEndCount = CountLump(wad, "F_END");
        var ffStartCount = CountLump(wad, "FF_START");
        var ffEndCount = CountLump(wad, "FF_END");

        // Usual case.
        var standard =
            fStartCount == 1 &&
            fEndCount == 1 &&
            ffStartCount == 0 &&
            ffEndCount == 0;

        // A trick to add custom flats is used.
        // https://www.doomworld.com/tutorials/fx2.php
        var customFlatTrick =
            fStartCount == 1 &&
            fEndCount >= 2;

        // Need deutex to add flats.
        var deutexMerge =
            fStartCount + ffStartCount >= 2 &&
            fEndCount + ffEndCount >= 2;

        if (standard || customFlatTrick)
            InitStandard(wad);
        else if (deutexMerge)
            InitDeuTexMerge(wad);
        else
            throw new Exception("Failed to read flats.");
    }

    public int Count => flats.Length;
    public Flat this[int num] => flats[num];
    public Flat this[string name] => nameToFlat[name];
    public int SkyFlatNumber { get; private set; }
    public Flat SkyFlat { get; private set; }

    private void InitStandard(Wad.Wad wad)
    {
        try
        {
            Console.Write("Load flats: ");
            var start = Stopwatch.GetTimestamp();

            var firstFlat = wad.GetLumpNumber("F_START") + 1;
            var lastFlat = wad.GetLumpNumber("F_END") - 1;
            var count = lastFlat - firstFlat + 1;

            flats = new Flat[count];

            var nameToFlatMapping = new Dictionary<string, Flat>();
            var nameToNumberMapping = new Dictionary<string, int>();

            for (var lump = firstFlat; lump <= lastFlat; lump++)
            {
                if (wad.GetLumpSize(lump) != 4096)
                    continue;

                var number = lump - firstFlat;
                var name = wad.LumpInfos[lump].Name;
                var flat = new Flat(name, wad.ReadLump(lump));

                flats[number] = flat;
                nameToFlatMapping[name] = flat;
                nameToNumberMapping[name] = number;
            }

            SkyFlatNumber = nameToNumberMapping["F_SKY1"];
            SkyFlat = nameToFlatMapping["F_SKY1"];

            this.nameToFlat = nameToFlatMapping.ToFrozenDictionary();
            this.nameToNumber = nameToNumberMapping.ToFrozenDictionary();

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK ({nameToFlatMapping.Count} flats) [{end}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }
    }

    private void InitDeuTexMerge(Wad.Wad wad)
    {
        try
        {
            Console.Write("Load flats: ");

            var allFlats = new List<int>();
            var flatZone = false;
            for (var lump = 0; lump < wad.LumpInfos.Length; lump++)
            {
                var name = wad.LumpInfos[lump].Name;
                if (flatZone)
                {
                    if (name is "F_END" or "FF_END")
                        flatZone = false;
                    else
                        allFlats.Add(lump);
                }
                else if (name is "F_START" or "FF_START")
                    flatZone = true;
            }

            allFlats.Reverse();

            var dupCheck = new HashSet<string>();
            var distinctFlats = new List<int>();
            foreach (var lump in allFlats)
            {
                if (dupCheck.Contains(wad.LumpInfos[lump].Name))
                    continue;

                distinctFlats.Add(lump);
                dupCheck.Add(wad.LumpInfos[lump].Name);
            }

            distinctFlats.Reverse();

            flats = new Flat[distinctFlats.Count];

            var nameToFlatMapping = new Dictionary<string, Flat>();
            var nameToNumberMapping = new Dictionary<string, int>();

            for (var number = 0; number < flats.Length; number++)
            {
                var lump = distinctFlats[number];

                if (wad.GetLumpSize(lump) != 4096)
                    continue;

                var name = wad.LumpInfos[lump].Name;
                var flat = new Flat(name, wad.ReadLump(lump));

                flats[number] = flat;
                nameToFlatMapping[name] = flat;
                nameToNumberMapping[name] = number;
            }

            this.nameToFlat = nameToFlatMapping.ToFrozenDictionary();
            this.nameToNumber = nameToNumberMapping.ToFrozenDictionary();

            SkyFlatNumber = nameToNumberMapping["F_SKY1"];
            SkyFlat = nameToFlatMapping["F_SKY1"];

            Console.WriteLine($"OK ({nameToFlatMapping.Count} flats)");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetNumber(string name)
    {
        return nameToNumber.TryGetValue(name, out var number) ? number : -1;
    }

    public IEnumerator<Flat> GetEnumerator()
    {
        return ((IEnumerable<Flat>)flats).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return flats.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CountLump(Wad.Wad wad, string name)
    {
        return wad.LumpInfos.Count(lump => lump.Name == name);
    }
}