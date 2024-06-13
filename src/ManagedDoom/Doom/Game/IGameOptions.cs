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

using ManagedDoom.Audio;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Intermission;
using ManagedDoom.UserInput;
using ManagedDoom.Video;

namespace ManagedDoom.Doom.Game;

public interface IGameOptions
{
    GameVersion GameVersion { get; set; }
    GameMode GameMode { get; set; }
    MissionPack MissionPack { get; set; }
    Player[] Players { get; }
    int ConsolePlayer { get; set; }
    int Episode { get; set; }
    int Map { get; set; }
    GameSkill Skill { get; set; }
    bool DemoPlayback { get; set; }
    bool NetGame { get; set; }
    int Deathmatch { get; set; }
    bool FastMonsters { get; set; }
    bool RespawnMonsters { get; set; }
    bool NoMonsters { get; set; }
    IntermissionInfo IntermissionInfo { get; }
    DoomRandom Random { get; }
    IVideo Video { get; set; }
    ISound Sound { get; set; }
    IMusic Music { get; set; }
    IUserInput UserInput { get; }
}