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

using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Info;

public static partial class DoomInfo
{
    public static readonly WeaponInfo[] WeaponInfos =
    [
        // fist
        new WeaponInfo(
            ammo: AmmoType.NoAmmo,
            upState: MobjState.Punchup,
            downState: MobjState.Punchdown,
            readyState: MobjState.Punch,
            attackState: MobjState.Punch1,
            flashState: MobjState.Null
        ),

        // pistol
        new WeaponInfo(
            ammo: AmmoType.Clip,
            upState: MobjState.Pistolup,
            downState: MobjState.Pistoldown,
            readyState: MobjState.Pistol,
            attackState: MobjState.Pistol1,
            flashState: MobjState.Pistolflash
        ),

        // shotgun
        new WeaponInfo(
            ammo: AmmoType.Shell,
            upState: MobjState.Sgunup,
            downState: MobjState.Sgundown,
            readyState: MobjState.Sgun,
            attackState: MobjState.Sgun1,
            flashState: MobjState.Sgunflash1
        ),

        // chaingun
        new WeaponInfo(
            ammo: AmmoType.Clip,
            upState: MobjState.Chainup,
            downState: MobjState.Chaindown,
            readyState: MobjState.Chain,
            attackState: MobjState.Chain1,
            flashState: MobjState.Chainflash1
        ),

        // missile launcher
        new WeaponInfo(
            ammo: AmmoType.Missile,
            upState: MobjState.Missileup,
            downState: MobjState.Missiledown,
            readyState: MobjState.Missile,
            attackState: MobjState.Missile1,
            flashState: MobjState.Missileflash1
        ),

        // plasma rifle
        new WeaponInfo(
            ammo: AmmoType.Cell,
            upState: MobjState.Plasmaup,
            downState: MobjState.Plasmadown,
            readyState: MobjState.Plasma,
            attackState: MobjState.Plasma1,
            flashState: MobjState.Plasmaflash1
        ),

        // bfg 9000
        new WeaponInfo(
            ammo: AmmoType.Cell,
            upState: MobjState.Bfgup,
            downState: MobjState.Bfgdown,
            readyState: MobjState.Bfg,
            attackState: MobjState.Bfg1,
            flashState: MobjState.Bfgflash1
        ),

        // chainsaw
        new WeaponInfo(
            ammo: AmmoType.NoAmmo,
            upState: MobjState.Sawup,
            downState: MobjState.Sawdown,
            readyState: MobjState.Saw,
            attackState: MobjState.Saw1,
            flashState: MobjState.Null
        ),

        // super shotgun
        new WeaponInfo(
            ammo: AmmoType.Shell,
            upState: MobjState.Dsgunup,
            downState: MobjState.Dsgundown,
            readyState: MobjState.Dsgun,
            attackState: MobjState.Dsgun1,
            flashState: MobjState.Dsgunflash1
        )
    ];
}