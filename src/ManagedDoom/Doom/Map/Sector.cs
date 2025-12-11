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
    // For frame interpolation.
    public Fixed oldFloorHeight;
    public Fixed oldCeilingHeight;

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

    /// <summary>
    /// 0 = untraversed, 1, 2 = sndlines - 1.
    /// </summary>
    public int SoundTraversed { get; set; }

    /// <summary>
    /// Thing that made a sound (or null).
    /// </summary>
    public Mobj SoundTarget { get; set; }

    /// <summary>
    /// Mapblock bounding box for height changes.
    /// </summary>
    public int[] BlockBox { get; set; } = null!;

    /// <summary>
    /// Origin for any sounds played by the sector.
    /// </summary>
    public Mobj SoundOrigin { get; set; } = null!;

    /// <summary>
    /// If == validcount, already checked.
    /// </summary>
    public int ValidCount { get; set; }

    /// <summary>
    /// List of mobjs in sector.
    /// </summary>
    public List<Mobj> ThingList { get; }

    /// <summary>
    /// Thinker for reversable actions.
    /// </summary>
    public IThinker SpecialData { get; set; }

    public LineDef[] Lines { get; set; } = null!;

    public List<Mobj>.Enumerator GetEnumerator() => ThingList.GetEnumerator();
}

public static class SectorExtensions
{
    extension(Sector sector)
    {
        public void UpdateFrameInterpolationInfo()
        {
            sector.oldFloorHeight = sector.FloorHeight;
            sector.oldCeilingHeight = sector.CeilingHeight;
        }

        public Fixed GetInterpolatedFloorHeight(Fixed frameFrac) => sector.oldFloorHeight + frameFrac * (sector.FloorHeight - sector.oldFloorHeight);
        public Fixed GetInterpolatedCeilingHeight(Fixed frameFrac) => sector.oldCeilingHeight + frameFrac * (sector.CeilingHeight - sector.oldCeilingHeight);

        public void DisableFrameInterpolationForOneFrame()
        {
            sector.oldFloorHeight = sector.FloorHeight;
            sector.oldCeilingHeight = sector.CeilingHeight;
        }
    }
}