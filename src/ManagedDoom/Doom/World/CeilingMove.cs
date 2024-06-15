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

using System.Runtime.CompilerServices;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class CeilingMove : Thinker
{
    private readonly World world;

    // 1 = up, 0 = waiting, -1 = down.

    // Corresponding sector tag.

    public CeilingMove(World world)
    {
        this.world = world;
    }

    public CeilingMoveType Type { get; set; }
    public Sector Sector { get; set; }
    public Fixed BottomHeight { get; set; }
    public Fixed TopHeight { get; set; }
    public Fixed Speed { get; set; }
    public bool Crush { get; set; }
    public int Direction { get; set; }
    public int Tag { get; set; }
    public int OldDirection { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Run()
    {
        var moveDirection = (CeilingMoveDirection)Direction;

        if (moveDirection == CeilingMoveDirection.Waiting)
            return;

        if (moveDirection == CeilingMoveDirection.Up)
            Up(world.SectorAction);
        else
            Down(world.SectorAction);
    }

    private void Up(SectorAction sa)
    {
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
    }

    private void Down(SectorAction sa)
    {
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
                    UpdateDownwardSpeed();
                    Direction = 1;
                    break;

                case CeilingMoveType.LowerAndCrush:
                case CeilingMoveType.LowerToFloor:
                    sa.RemoveActiveCeiling(this);
                    Sector.DisableFrameInterpolationForOneFrame();
                    break;
            }
        }
        else if (result == SectorActionResult.Crushed && Type is CeilingMoveType.SilentCrushAndRaise or CeilingMoveType.CrushAndRaise or CeilingMoveType.LowerAndCrush)
            Speed = SectorAction.CeilingSpeed / 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateDownwardSpeed()
    {
        if (Type == CeilingMoveType.SilentCrushAndRaise)
        {
            world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
            Speed = SectorAction.CeilingSpeed;
        }
        else if (Type == CeilingMoveType.CrushAndRaise)
            Speed = SectorAction.CeilingSpeed;
    }
}