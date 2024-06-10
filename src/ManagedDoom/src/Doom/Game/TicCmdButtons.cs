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


namespace ManagedDoom.Doom.Game;

public static class TicCmdButtons
{
    public const byte Attack = 1;

    // Use button, to open doors, activate switches.
    public const byte Use = 2;

    // Flag: game events, not really buttons.
    public const byte Special = 128;
    public const byte SpecialMask = 3;

    // Flag, weapon change pending.
    // If true, the next 3 bits hold weapon num.
    public const byte Change = 4;

    // The 3bit weapon mask and shift, convenience.
    public const byte WeaponMask = 8 + 16 + 32;
    public const byte WeaponShift = 3;

    // Pause the game.
    public const byte Pause = 1;
}