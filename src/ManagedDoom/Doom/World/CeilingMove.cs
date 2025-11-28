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

public sealed class CeilingMove : Thinker
{
    private readonly World world;

    public CeilingMove(World world)
    {
        this.world = world;
    }

    /// <summary>
    /// 1 = up, 0 = waiting, -1 = down.
    /// </summary>
    public CeilingMoveType Type { get; set; }

    /// <summary>
    /// Corresponding sector tag.
    /// </summary>
    public Sector Sector { get; set; }

    public Fixed BottomHeight { get; set; }
    public Fixed TopHeight { get; set; }
    public Fixed Speed { get; set; }
    public bool Crush { get; set; }
    public int Direction { get; set; }
    public int Tag { get; set; }
    public int OldDirection { get; set; }

    public override void Run()
    {
        switch (Direction)
        {
            case 0:
                // In statis.
                break;

            case 1:
            {
                var sa = world.SectorAction;

                // Up.
                var result = sa.MovePlane(
                    sector: Sector,
                    speed: Speed,
                    dest: TopHeight,
                    crush: false,
                    floorOrCeiling: 1,
                    direction: Direction
                );

                if (((world.LevelTime + Sector.Number) & 7) == 0 && Type != CeilingMoveType.SilentCrushAndRaise)
                    world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);

                if (result == SectorActionResult.PastDestination)
                {
                    switch (Type)
                    {
                        case CeilingMoveType.RaiseToHighest:
                            sa.RemoveActiveCeiling(this);
                            Sector.DisableFrameInterpolationForOneFrame();
                            break;

                        case CeilingMoveType.SilentCrushAndRaise:
                        case CeilingMoveType.FastCrushAndRaise:
                        case CeilingMoveType.CrushAndRaise:
                            if (Type == CeilingMoveType.SilentCrushAndRaise)
                                world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);

                            Direction = -1;
                            break;
                    }
                }

                break;
            }

            case -1:
            {
                var sa = world.SectorAction;

                // Down.
                var result = sa.MovePlane(
                    sector: Sector,
                    speed: Speed,
                    dest: BottomHeight,
                    crush: Crush,
                    floorOrCeiling: 1,
                    direction: Direction
                );

                if (((world.LevelTime + Sector.Number) & 7) == 0 && Type != CeilingMoveType.SilentCrushAndRaise)
                    world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);

                if (result == SectorActionResult.PastDestination)
                {
                    switch (Type)
                    {
                        case CeilingMoveType.SilentCrushAndRaise:
                        case CeilingMoveType.CrushAndRaise:
                        case CeilingMoveType.FastCrushAndRaise:
                            switch (Type)
                            {
                                case CeilingMoveType.SilentCrushAndRaise:
                                    world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
                                    Speed = SectorAction.CeilingSpeed;
                                    break;
                                case CeilingMoveType.CrushAndRaise:
                                    Speed = SectorAction.CeilingSpeed;
                                    break;
                            }

                            Direction = 1;
                            break;

                        case CeilingMoveType.LowerAndCrush:
                        case CeilingMoveType.LowerToFloor:
                            sa.RemoveActiveCeiling(this);
                            Sector.DisableFrameInterpolationForOneFrame();
                            break;
                    }
                }
                else
                {
                    if (result == SectorActionResult.Crushed)
                    {
                        Speed = Type switch
                        {
                            CeilingMoveType.SilentCrushAndRaise or CeilingMoveType.CrushAndRaise or CeilingMoveType.LowerAndCrush => SectorAction.CeilingSpeed / 8,
                            _                                                                                                     => Speed
                        };
                    }
                }

                break;
            }
        }
    }
}