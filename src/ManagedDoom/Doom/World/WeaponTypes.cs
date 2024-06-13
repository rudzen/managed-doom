//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
// Copyright (C) 2024 Rudy Alex Kohn
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

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ManagedDoom.Doom.World;

public enum WeaponTypes : byte
{
    Fist,
    Pistol,
    Shotgun,
    Chaingun,
    Missile,
    Plasma,
    Bfg,
    Chainsaw,
    SuperShotgun,

    Count,

    // No pending weapon change.
    NoChange
}

public readonly record struct WeaponType(WeaponTypes Value) : IMinMaxValue<WeaponType>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WeaponType(int wt) : this((WeaponTypes)wt)
    {
    }

    public static WeaponType Fist => new(WeaponTypes.Fist);
    public static WeaponType Pistol => new(WeaponTypes.Pistol);
    public static WeaponType Shotgun => new(WeaponTypes.Shotgun);
    public static WeaponType Chaingun => new(WeaponTypes.Chaingun);
    public static WeaponType Missile => new(WeaponTypes.Missile);
    public static WeaponType Plasma => new(WeaponTypes.Plasma);
    public static WeaponType Bfg => new(WeaponTypes.Bfg);
    public static WeaponType Chainsaw => new(WeaponTypes.Chainsaw);
    public static WeaponType SuperShotgun => new(WeaponTypes.SuperShotgun);
    public static WeaponType NoChange => new(WeaponTypes.NoChange);
    public static WeaponType MaxValue => SuperShotgun;
    public static WeaponType MinValue => Fist;

    public const int Count = (int)WeaponTypes.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(WeaponType left, WeaponTypes right) => left.Value == right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(WeaponType left, WeaponTypes right) => left.Value != right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(WeaponType left, int right) => left.Value == (WeaponTypes)right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(WeaponType left, int right) => left.Value != (WeaponTypes)right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator +(WeaponType left, WeaponType right) => new(left.AsInt() + right.AsInt());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator +(WeaponType left, int right) => new(left.AsInt() + right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator +(WeaponType left, WeaponTypes right) => new(left.AsInt() + (int)right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator -(WeaponType left, WeaponType right) => new(left.AsInt() - right.AsInt());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator -(WeaponType left, int right) => new(left.AsInt() - right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator -(WeaponType left, WeaponTypes right) => new(left.AsInt() - (int)right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator ++(WeaponType f) => new(f.Value + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WeaponType operator --(WeaponType f) => new(f.Value - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int operator >> (WeaponType left, int right) => left.AsInt() >> right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(WeaponType left, WeaponType right) => left.AsInt() > right.AsInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(WeaponType left, WeaponType right) => left.AsInt() < right.AsInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(WeaponType left, WeaponType right) => left.AsInt() >= right.AsInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(WeaponType left, WeaponType right) => left.AsInt() <= right.AsInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator byte(WeaponType f) => (byte)f.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int AsInt() => (int)Value;
}