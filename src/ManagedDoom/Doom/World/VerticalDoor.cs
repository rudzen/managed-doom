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

public sealed class VerticalDoor : IThinker
{
    public VerticalDoorType Type { get; set; }
    public Sector Sector { get; set; }
    public Fixed TopHeight { get; set; }
    public Fixed Speed { get; set; }

    /// <summary>
    /// 1 = up, 0 = waiting at top, -1 = down.
    /// </summary>
    public int Direction { get; set; }

    /// <summary>
    /// Tics to wait at the top.
    /// </summary>
    public int TopWait { get; set; }

    /// <summary>
    /// When it reaches 0, start going down
    /// </summary>
    public int TopCountDown { get; set; }

    /// <summary>
    /// (keep in case a door going down is reset).
    /// </summary>
    public ThinkerState ThinkerState { get; set; }

    public void Run(World world)
    {
        var sa = world.SectorAction;

        SectorActionResult result;

        switch (Direction)
        {
            case 0:
                // Waiting.
                if (--TopCountDown == 0)
                {
                    switch (Type)
                    {
                        case VerticalDoorType.BlazeRaise:
                            // Time to go back down.
                            Direction = -1;
                            world.StartSound(Sector.SoundOrigin, Sfx.BDCLS, SfxType.Misc);
                            break;

                        case VerticalDoorType.Normal:
                            // Time to go back down.
                            Direction = -1;
                            world.StartSound(Sector.SoundOrigin, Sfx.DORCLS, SfxType.Misc);
                            break;

                        case VerticalDoorType.Close30ThenOpen:
                            Direction = 1;
                            world.StartSound(Sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
                            break;
                    }
                }

                break;

            case 2:
                // Initial wait.
                if (--TopCountDown == 0)
                {
                    switch (Type)
                    {
                        case VerticalDoorType.RaiseIn5Mins:
                            Direction = 1;
                            Type = VerticalDoorType.Normal;
                            world.StartSound(Sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
                            break;
                    }
                }

                break;

            case -1:
                // Down.
                result = sa.MovePlane(
                    Sector,
                    Speed,
                    Sector.FloorHeight,
                    false, 1, Direction);
                if (result == SectorActionResult.PastDestination)
                {
                    switch (Type)
                    {
                        case VerticalDoorType.BlazeRaise:
                        case VerticalDoorType.BlazeClose:
                            Sector.SpecialData = null;
                            // Unlink and free.
                            Thinkers.Remove(this);
                            Sector.DisableFrameInterpolationForOneFrame();
                            world.StartSound(Sector.SoundOrigin, Sfx.BDCLS, SfxType.Misc);
                            break;

                        case VerticalDoorType.Normal:
                        case VerticalDoorType.Close:
                            Sector.SpecialData = null;
                            // Unlink and free.
                            Thinkers.Remove(this);
                            Sector.DisableFrameInterpolationForOneFrame();
                            break;

                        case VerticalDoorType.Close30ThenOpen:
                            Direction = 0;
                            TopCountDown = 35 * 30;
                            break;
                    }
                }
                else if (result == SectorActionResult.Crushed)
                {
                    switch (Type)
                    {
                        case VerticalDoorType.BlazeClose:
                        case VerticalDoorType.Close: // Do not go back up!
                            break;

                        default:
                            Direction = 1;
                            world.StartSound(Sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
                            break;
                    }
                }

                break;

            case 1:
                // Up.
                result = sa.MovePlane(
                    Sector,
                    Speed,
                    TopHeight,
                    false, 1, Direction);

                if (result == SectorActionResult.PastDestination)
                {
                    switch (Type)
                    {
                        case VerticalDoorType.BlazeRaise:
                        case VerticalDoorType.Normal:
                            // Wait at top.
                            Direction = 0;
                            TopCountDown = TopWait;
                            break;

                        case VerticalDoorType.Close30ThenOpen:
                        case VerticalDoorType.BlazeOpen:
                        case VerticalDoorType.Open:
                            Sector.SpecialData = null;
                            // Unlink and free.
                            Thinkers.Remove(this);
                            Sector.DisableFrameInterpolationForOneFrame();
                            break;
                    }
                }

                break;
        }
    }
}