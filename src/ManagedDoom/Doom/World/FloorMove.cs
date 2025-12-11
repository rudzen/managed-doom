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

using ManagedDoom.Audio;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public enum FloorMoveType
{
    // Lower floor to highest surrounding floor.
    LowerFloor,

    // Lower floor to lowest surrounding floor.
    LowerFloorToLowest,

    // Lower floor to highest surrounding floor very fast.
    TurboLower,

    // Raise floor to lowest surrounding ceiling.
    RaiseFloor,

    // Raise floor to next highest surrounding floor.
    RaiseFloorToNearest,

    // Raise floor to shortest height texture around it.
    RaiseToTexture,

    // Lower floor to lowest surrounding floor and
    // change floor texture.
    LowerAndChange,

    RaiseFloor24,
    RaiseFloor24AndChange,
    RaiseFloorCrush,

    // Raise to next highest floor, turbo-speed.
    RaiseFloorTurbo,
    DonutRaise,
    RaiseFloor512
}

public sealed class FloorMove : IThinker
{
    public FloorMoveType Type { get; set; }
    public bool Crush { get; set; }
    public Sector Sector { get; set; }
    public int Direction { get; set; }
    public SectorSpecial NewSpecial { get; set; }
    public int Texture { get; set; }
    public Fixed FloorDestHeight { get; set; }
    public Fixed Speed { get; set; }
    public ThinkerState ThinkerState { get; set; }

    public void Run(World world)
    {
        var sa = world.SectorAction;

        var result = sa.MovePlane(
            sector: Sector,
            speed: Speed,
            dest: FloorDestHeight,
            crush: Crush,
            floorOrCeiling: 0,
            direction: Direction
        );

        if (((world.LevelTime + Sector.Number) & 7) == 0)
            world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);

        if (result != SectorActionResult.PastDestination)
            return;

        Sector.SpecialData = null;

        if (Direction == 1)
        {
            if (Type == FloorMoveType.DonutRaise)
            {
                Sector.Special = NewSpecial;
                Sector.FloorFlat = Texture;
            }
        }
        else if (Direction == -1)
        {
            if (Type == FloorMoveType.LowerAndChange)
            {
                Sector.Special = NewSpecial;
                Sector.FloorFlat = Texture;
            }
        }

        Thinkers.Remove(this);
        Sector.DisableFrameInterpolationForOneFrame();

        world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
    }
}