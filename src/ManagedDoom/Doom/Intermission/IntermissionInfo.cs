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

using ManagedDoom.Doom.Game;

namespace ManagedDoom.Doom.Intermission;

public sealed class IntermissionInfo
{
    // Episode number (0-2).

    // If true, splash the secret level.

    // Previous and next levels, origin 0.

    // The par time.

    public IntermissionInfo()
    {
        PlayerScores = new PlayerScores[Player.MaxPlayerCount];
        for (var i = 0; i < PlayerScores.Length; i++)
            PlayerScores[i] = new PlayerScores();
    }

    public int Episode { get; set; }

    public bool DidSecret { get; set; }

    public int LastLevel { get; set; }

    public int NextLevel { get; set; }

    public int MaxKillCount
    {
        get => System.Math.Max(field, 1);
        set;
    }

    public int MaxItemCount
    {
        get => System.Math.Max(field, 1);
        set;
    }

    public int MaxSecretCount
    {
        get => System.Math.Max(field, 1);
        set;
    }

    public int TotalFrags
    {
        get => System.Math.Max(field, 1);
        set;
    }

    public int ParTime { get; set; }

    public PlayerScores[] PlayerScores { get; }
}