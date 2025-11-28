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
public enum CardType
{
    None = 0,
    BlueCard = 1 << 0,
    YellowCard = 1 << 1,
    RedCard = 1 << 2,
    BlueSkull = 1 << 3,
    YellowSkull = 1 << 4,
    RedSkull = 1 << 5,

    KeyCards = BlueCard | YellowCard | RedCard,
    SkullKeys = BlueSkull | YellowSkull | RedSkull,
    All = KeyCards | SkullKeys
}

public static class CardTypeExtensions
{
    public static CardType[] CardTypes => [CardType.BlueCard, CardType.YellowCard, CardType.RedCard, CardType.BlueSkull, CardType.YellowSkull, CardType.RedSkull];

    extension(CardType ct)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Index()
        {
            // Convert enum value to ulong
            var mask = (uint)ct;

            // Use BitOperations to get the log2 value, which gives the bit index
            return BitOperations.Log2(mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(CardType f) => (ct & f) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CardType Next(ref CardType ct)
    {
        var lsb = Lsb(ct);
        ResetLsb(ref ct);
        return lsb;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CardType Lsb(CardType ct)
    {
        var tzc = BitOperations.TrailingZeroCount((int)ct);
        return (CardType)(1 << tzc);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void ResetLsb(ref CardType ct) => ct &= ct - 1;
}