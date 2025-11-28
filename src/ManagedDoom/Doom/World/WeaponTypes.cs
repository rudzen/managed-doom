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
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ManagedDoom.Doom.World;

[Flags]
public enum WeaponTypes
{
    None = 0,
    Fist = 1 << 0,
    Pistol = 1 << 1,
    Shotgun = 1 << 2,
    Chaingun = 1 << 3,
    Missile = 1 << 4,
    Plasma = 1 << 5,
    Bfg = 1 << 6,
    Chainsaw = 1 << 7,
    SuperShotgun = 1 << 8,

    Shareware = Fist | Pistol | Shotgun | Chaingun | Missile,
    All = Fist | Pistol | Shotgun | Chaingun | Missile | Plasma | Bfg | Chainsaw | SuperShotgun
}

public static class WeaponTypesExtensions
{
    public static WeaponTypes[] AllWeaponTypes =>
    [
        WeaponTypes.Fist, WeaponTypes.Pistol, WeaponTypes.Shotgun, WeaponTypes.Chaingun,
        WeaponTypes.Missile, WeaponTypes.Plasma, WeaponTypes.Bfg, WeaponTypes.Chainsaw,
        WeaponTypes.SuperShotgun
    ];

    extension(WeaponTypes wt)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Index()
        {
            // Convert enum value to ulong
            var mask = (uint)wt;

            // Use BitOperations to get the log2 value, which gives the bit index
            return BitOperations.Log2(mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(WeaponTypes f)
        {
            return (wt & f) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref WeaponInfo WeaponInfo()
        {
            return ref weaponInfos[wt.Index()];
        }
    }

    private static readonly WeaponInfo[] weaponInfos =
    [
        // fist
        new(
            ammo: AmmoType.NoAmmo,
            upState: MobjState.Punchup,
            downState: MobjState.Punchdown,
            readyState: MobjState.Punch,
            attackState: MobjState.Punch1,
            flashState: MobjState.Null
        ),

        // pistol
        new(
            ammo: AmmoType.Clip,
            upState: MobjState.Pistolup,
            downState: MobjState.Pistoldown,
            readyState: MobjState.Pistol,
            attackState: MobjState.Pistol1,
            flashState: MobjState.Pistolflash
        ),

        // shotgun
        new(
            ammo: AmmoType.Shell,
            upState: MobjState.Sgunup,
            downState: MobjState.Sgundown,
            readyState: MobjState.Sgun,
            attackState: MobjState.Sgun1,
            flashState: MobjState.Sgunflash1
        ),

        // chaingun
        new(
            ammo: AmmoType.Clip,
            upState: MobjState.Chainup,
            downState: MobjState.Chaindown,
            readyState: MobjState.Chain,
            attackState: MobjState.Chain1,
            flashState: MobjState.Chainflash1
        ),

        // missile launcher
        new(
            ammo: AmmoType.Missile,
            upState: MobjState.Missileup,
            downState: MobjState.Missiledown,
            readyState: MobjState.Missile,
            attackState: MobjState.Missile1,
            flashState: MobjState.Missileflash1
        ),

        // plasma rifle
        new(
            ammo: AmmoType.Cell,
            upState: MobjState.Plasmaup,
            downState: MobjState.Plasmadown,
            readyState: MobjState.Plasma,
            attackState: MobjState.Plasma1,
            flashState: MobjState.Plasmaflash1
        ),

        // bfg 9000
        new(
            ammo: AmmoType.Cell,
            upState: MobjState.Bfgup,
            downState: MobjState.Bfgdown,
            readyState: MobjState.Bfg,
            attackState: MobjState.Bfg1,
            flashState: MobjState.Bfgflash1
        ),

        // chainsaw
        new(
            ammo: AmmoType.NoAmmo,
            upState: MobjState.Sawup,
            downState: MobjState.Sawdown,
            readyState: MobjState.Saw,
            attackState: MobjState.Saw1,
            flashState: MobjState.Null
        ),

        // super shotgun
        new(
            ammo: AmmoType.Shell,
            upState: MobjState.Dsgunup,
            downState: MobjState.Dsgundown,
            readyState: MobjState.Dsgun,
            attackState: MobjState.Dsgun1,
            flashState: MobjState.Dsgunflash1
        )
    ];
}