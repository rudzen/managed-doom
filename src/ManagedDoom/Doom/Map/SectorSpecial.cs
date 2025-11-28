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

namespace ManagedDoom.Doom.Map;

public enum SectorSpecial
{
    Normal = 0,

    // Spawns
    FlickeringLightsSpawn = 1,
    StrobeFastSpawn = 2,
    StrobeSlowSpawn = 3,
    StrobeFastDeathSlimeSpawn = 4,
    GlowingLightSpawn = 8,
    SecretSectorSpawn = 9,
    DoorCloseIn30SecondsSpawn = 10,
    SyncStrobeSlowSpawn = 12,
    SyncStrobeFastSpawn = 13,
    DoorRaiseIn5MinutesSpawn = 14,
    FireFlickerSpawn = 17,
}