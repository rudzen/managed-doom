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

using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.Game;

public static class GameConst
{
    public const int TicRate = 35;

    public static readonly Fixed MaxThingRadius = Fixed.FromInt(32);

    public const int TurboThreshold = 0x32;
}

public enum MissionPack : byte
{
    Doom2,
    Plutonia,
    Tnt
}

public enum GameVersion
{
    Version109,
    Ultimate,
    Final,
    Final2
}

public enum UpdateResult
{
    None,
    Completed,
    NeedWipe
}

public enum GameState : byte
{
    Level,
    Intermission,
    Finale
}

public enum GameSkill : byte
{
    Baby,
    Easy,
    Medium,
    Hard,
    Nightmare
}

public enum GameMode
{
    Shareware,  // DOOM 1 shareware, E1, M9
    Registered, // DOOM 1 registered, E3, M27
    Commercial, // DOOM 2 retail, E1 M34

    // DOOM 2 german edition not handled
    Retail,      // DOOM 1 retail, E4, M36
    Indetermined // Well, no IWAD found.
}

public static class GameModeExtensions
{
    public static int DemoTimer(this GameMode gameMode)
    {
        const int commercialTimer = 35 * 11;
        const int nonCommercialTimer = 170;

        return gameMode == GameMode.Commercial ? commercialTimer : nonCommercialTimer;
    }
}