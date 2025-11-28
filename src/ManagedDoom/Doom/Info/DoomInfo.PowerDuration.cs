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

namespace ManagedDoom.Doom.Info;

public static partial class DoomInfo
{
    public static class PowerDuration
    {
        public const int Invulnerability = 30 * GameConst.TicRate;
        public const int Invisibility = 60 * GameConst.TicRate;
        public const int Infrared = 120 * GameConst.TicRate;
        public const int IronFeet = 60 * GameConst.TicRate;
    }
}