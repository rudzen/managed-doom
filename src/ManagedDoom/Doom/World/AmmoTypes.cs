//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
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

namespace ManagedDoom.Doom.World;

public enum AmmoTypes : byte
{
    // Pistol / chaingun ammo.
    Clip,

    // Shotgun / double barreled shotgun.
    Shell,

    // Plasma rifle, BFG.
    Cell,

    // Missile launcher.
    Missile,

    Count,

    // Unlimited for chainsaw / fist.
    NoAmmo
}

public readonly record struct AmmoType(AmmoTypes Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AmmoType(int t) : this((AmmoTypes)t)
    {
    }

    public const int Count = (int)AmmoTypes.Count;

    public static AmmoType Clip => new(AmmoTypes.Clip);
    public static AmmoType Shell => new(AmmoTypes.Shell);
    public static AmmoType Cell => new(AmmoTypes.Cell);
    public static AmmoType Missile => new(AmmoTypes.Missile);
    public static AmmoType NoAmmo => new(AmmoTypes.NoAmmo);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AmmoType(byte f) => new((AmmoTypes)f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator byte(AmmoType f) => (byte)f.Value;
}