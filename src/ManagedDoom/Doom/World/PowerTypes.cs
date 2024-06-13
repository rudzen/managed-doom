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

namespace ManagedDoom.Doom.World;

public enum PowerTypes : byte
{
    Invulnerability,
    Strength,
    Invisibility,
    IronFeet,
    AllMap,
    Infrared,
    Count
}

public readonly record struct PowerType(PowerTypes Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PowerType(byte t) : this((PowerTypes)t)
    {
    }

    public static PowerType Invulnerability => new(PowerTypes.Invulnerability);
    public static PowerType Strength => new(PowerTypes.Strength);
    public static PowerType Invisibility => new(PowerTypes.Invisibility);
    public static PowerType IronFeet => new(PowerTypes.IronFeet);
    public static PowerType AllMap => new(PowerTypes.AllMap);
    public static PowerType Infrared => new(PowerTypes.Infrared);

    public const int Count = (int)PowerTypes.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PowerType(byte f) => new((PowerTypes)f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator byte(PowerType f) => (byte)f.Value;
}