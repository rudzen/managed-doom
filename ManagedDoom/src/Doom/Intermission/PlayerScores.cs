﻿//
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



using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public class PlayerScores
    {
        // Whether the player is in game.

        // Player stats, kills, collected items etc.

        public PlayerScores()
        {
            Frags = new int[Player.MaxPlayerCount];
        }

        public bool InGame { get; set; }

        public int KillCount { get; set; }

        public int ItemCount { get; set; }

        public int SecretCount { get; set; }

        public int Time { get; set; }

        public int[] Frags { get; }
    }
}
