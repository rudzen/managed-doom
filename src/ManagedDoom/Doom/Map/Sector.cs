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
using System.Collections.Generic;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Map;

public sealed class Sector
{
    private const int DataSize = 26;

    // 0 = untraversed, 1, 2 = sndlines - 1.

    // Thing that made a sound (or null).

    // Mapblock bounding box for height changes.

    // Origin for any sounds played by the sector.

    // If == validcount, already checked.

    // List of mobjs in sector.

    // Thinker for reversable actions.

    // For frame interpolation.
    private Fixed oldFloorHeight;
    private Fixed oldCeilingHeight;

    private Sector(
        int number,
        Fixed floorHeight,
        Fixed ceilingHeight,
        int floorFlat,
        int ceilingFlat,
        int lightLevel,
        SectorSpecial special,
        int tag)
    {
        this.Number = number;
        this.FloorHeight = floorHeight;
        this.CeilingHeight = ceilingHeight;
        this.FloorFlat = floorFlat;
        this.CeilingFlat = ceilingFlat;
        this.LightLevel = lightLevel;
        this.Special = special;
        this.Tag = tag;

        oldFloorHeight = floorHeight;
        oldCeilingHeight = ceilingHeight;

        ThingList = new List<Mobj>(512);
    }

    public int Number { get; }
    public Fixed FloorHeight { get; set; }
    public Fixed CeilingHeight { get; set; }
    public int FloorFlat { get; set; }
    public int CeilingFlat { get; set; }
    public int LightLevel { get; set; }
    public SectorSpecial Special { get; set; }
    public int Tag { get; set; }
    public int SoundTraversed { get; set; }
    public Mobj? SoundTarget { get; set; }
    public int[] BlockBox { get; set; } = null!;
    public Mobj SoundOrigin { get; set; } = null!;
    public int ValidCount { get; set; }
    public List<Mobj> ThingList { get; }
    public Thinker? SpecialData { get; set; }
    public LineDef[] Lines { get; set; } = null!;

    private static Sector FromData(ReadOnlySpan<byte> data, int number, IFlatLookup flats)
    {
        var floorHeight = BitConverter.ToInt16(data[..2]);
        var ceilingHeight = BitConverter.ToInt16(data.Slice(2, 2));
        var floorFlatName = DoomInterop.ToString(data.Slice(4, 8));
        var ceilingFlatName = DoomInterop.ToString(data.Slice(12, 8));
        var lightLevel = BitConverter.ToInt16(data.Slice(20, 2));
        var special = BitConverter.ToInt16(data.Slice(22, 2));
        var tag = BitConverter.ToInt16(data.Slice(24, 2));

        return new Sector(
            number,
            Fixed.FromInt(floorHeight),
            Fixed.FromInt(ceilingHeight),
            flats.GetNumber(floorFlatName),
            flats.GetNumber(ceilingFlatName),
            lightLevel,
            (SectorSpecial)special,
            tag);
    }

    public static Sector[] FromWad(Wad.Wad wad, int lump, IFlatLookup flats)
    {
        var lumpSize = wad.GetLumpSize(lump);
        if (lumpSize % DataSize != 0)
            throw new Exception();

        var lumpData = wad.GetLumpData(lump);

        var count = lumpSize / DataSize;
        var sectors = new Sector[count];

        for (var i = 0; i < sectors.Length; i++)
        {
            var offset = DataSize * i;
            sectors[i] = FromData(lumpData[offset..], i, flats);
        }

        return sectors;
    }

    public void UpdateFrameInterpolationInfo()
    {
        oldFloorHeight = FloorHeight;
        oldCeilingHeight = CeilingHeight;
    }

    public Fixed GetInterpolatedFloorHeight(Fixed frameFrac)
    {
        return oldFloorHeight + frameFrac * (FloorHeight - oldFloorHeight);
    }

    public Fixed GetInterpolatedCeilingHeight(Fixed frameFrac)
    {
        return oldCeilingHeight + frameFrac * (CeilingHeight - oldCeilingHeight);
    }

    public void DisableFrameInterpolationForOneFrame()
    {
        oldFloorHeight = FloorHeight;
        oldCeilingHeight = CeilingHeight;
    }

    public List<Mobj>.Enumerator GetEnumerator()
    {
        return ThingList.GetEnumerator();
    }
}