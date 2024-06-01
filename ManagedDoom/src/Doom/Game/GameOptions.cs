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



using System;
using ManagedDoom.Video;
using ManagedDoom.Audio;
using ManagedDoom.UserInput;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        public GameOptions()
        {
            GameVersion = GameVersion.Version109;
            GameMode = GameMode.Commercial;
            MissionPack = MissionPack.Doom2;

            Players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                Players[i] = new Player(i);
            }
            Players[0].InGame = true;
            ConsolePlayer = 0;

            Episode = 1;
            Map = 1;
            Skill = GameSkill.Medium;

            DemoPlayback = false;
            NetGame = false;

            Deathmatch = 0;
            FastMonsters = false;
            RespawnMonsters = false;
            NoMonsters = false;

            IntermissionInfo = new IntermissionInfo();

            Random = new DoomRandom();

            Video = NullVideo.GetInstance();
            Sound = NullSound.GetInstance();
            Music = NullMusic.GetInstance();
            UserInput = NullUserInput.GetInstance();
        }

        public GameOptions(CommandLineArgs args, GameContent content) : this()
        {
            if (args.solonet.Present)
            {
                NetGame = true;
            }

            GameVersion = content.Wad.GameVersion;
            GameMode = content.Wad.GameMode;
            MissionPack = content.Wad.MissionPack;
        }

        public GameVersion GameVersion { get; set; }

        public GameMode GameMode { get; set; }

        public MissionPack MissionPack { get; set; }

        public Player[] Players { get; }

        public int ConsolePlayer { get; set; }

        public int Episode { get; set; }

        public int Map { get; set; }

        public GameSkill Skill { get; set; }

        public bool DemoPlayback { get; set; }

        public bool NetGame { get; set; }

        public int Deathmatch { get; set; }

        public bool FastMonsters { get; set; }

        public bool RespawnMonsters { get; set; }

        public bool NoMonsters { get; set; }

        public IntermissionInfo IntermissionInfo { get; }

        public DoomRandom Random { get; }

        public IVideo Video { get; set; }

        public ISound Sound { get; set; }

        public IMusic Music { get; set; }

        public IUserInput UserInput { get; set; }
    }
}
