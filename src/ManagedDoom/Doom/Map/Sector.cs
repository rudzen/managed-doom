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
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Map;

public sealed class Sector
{
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

    public Sector(
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

    public void UpdateFrameInterpolationInfo()
    {
        oldFloorHeight = FloorHeight;
        oldCeilingHeight = CeilingHeight;
    }

    public Fixed GetInterpolatedFloorHeight(Fixed frameFrac) => oldFloorHeight + frameFrac * (FloorHeight - oldFloorHeight);

    public Fixed GetInterpolatedCeilingHeight(Fixed frameFrac) => oldCeilingHeight + frameFrac * (CeilingHeight - oldCeilingHeight);

    public void DisableFrameInterpolationForOneFrame()
    {
        oldFloorHeight = FloorHeight;
        oldCeilingHeight = CeilingHeight;
    }

    public List<Mobj>.Enumerator GetEnumerator() => ThingList.GetEnumerator();
}