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

public sealed class Platform(World world) : Thinker
{
    public Sector Sector { get; set; } = null!;

    public Fixed Speed { get; set; }

    public Fixed Low { get; set; }

    public Fixed High { get; set; }

    public int Wait { get; set; }

    public int Count { get; set; }

    public PlatformState Status { get; set; }

    public PlatformState OldStatus { get; set; }

    public bool Crush { get; set; }

    public int Tag { get; set; }

    public PlatformType Type { get; set; }

    public override void Run()
    {
        var sa = world.SectorAction;

        SectorActionResult result;

        switch (Status)
        {
            case PlatformState.Up:
                result = sa.MovePlane(Sector, Speed, High, Crush, 0, 1);

                if (Type is PlatformType.RaiseAndChange or PlatformType.RaiseToNearestAndChange)
                {
                    if (((world.LevelTime + Sector.Number) & 7) == 0)
                        world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
                }

                if (result == SectorActionResult.Crushed && !Crush)
                {
                    Count = Wait;
                    Status = PlatformState.Down;
                    world.StartSound(Sector.SoundOrigin, Sfx.PSTART, SfxType.Misc);
                }
                else
                {
                    if (result == SectorActionResult.PastDestination)
                    {
                        Count = Wait;
                        Status = PlatformState.Waiting;
                        world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);

                        switch (Type)
                        {
                            case PlatformType.BlazeDwus:
                            case PlatformType.DownWaitUpStay:
                                sa.RemoveActivePlatform(this);
                                Sector.DisableFrameInterpolationForOneFrame();
                                break;

                            case PlatformType.RaiseAndChange:
                            case PlatformType.RaiseToNearestAndChange:
                                sa.RemoveActivePlatform(this);
                                Sector.DisableFrameInterpolationForOneFrame();
                                break;
                        }
                    }
                }

                break;

            case PlatformState.Down:
                result = sa.MovePlane(Sector, Speed, Low, false, 0, -1);

                if (result == SectorActionResult.PastDestination)
                {
                    Count = Wait;
                    Status = PlatformState.Waiting;
                    world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
                }

                break;

            case PlatformState.Waiting:
                if (--Count == 0)
                {
                    Status = Sector.FloorHeight == Low ? PlatformState.Up : PlatformState.Down;
                    world.StartSound(Sector.SoundOrigin, Sfx.PSTART, SfxType.Misc);
                }

                break;

            case PlatformState.InStasis:
                break;
        }
    }
}