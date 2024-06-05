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
using System.Diagnostics;
using System.IO;
using ManagedDoom.Config;

namespace ManagedDoom
{
    public sealed class DemoPlayback
    {
        private readonly Demo demo;
        private readonly TicCmd[] ticCommands;

        private readonly Stopwatch stopwatch;
        private int frameCount;

        public DemoPlayback(CommandLineArgs args, GameContent content, GameOptions options, string demoName)
        {
            if (File.Exists(demoName))
                demo = new Demo(demoName);
            else if (File.Exists($"{demoName}.lmp"))
                demo = new Demo($"{demoName}.lmp");
            else
            {
                var lumpName = demoName.ToUpper();
                if (content.Wad.GetLumpNumber(lumpName) == -1)
                    throw new Exception($"Demo '{demoName}' was not found!");
                demo = new Demo(content.Wad.ReadLump(lumpName));
            }

            demo.Options.GameVersion = options.GameVersion;
            demo.Options.GameMode = options.GameMode;
            demo.Options.MissionPack = options.MissionPack;
            demo.Options.Video = options.Video;
            demo.Options.Sound = options.Sound;
            demo.Options.Music = options.Music;

            if (args.solonet.Present)
                demo.Options.NetGame = true;

            ticCommands = new TicCmd[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
                ticCommands[i] = new TicCmd();

            Game = new DoomGame(content, demo.Options);
            Game.DeferedInitNew();

            stopwatch = new Stopwatch();
        }

        public UpdateResult Update()
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            if (!demo.ReadCmd(ticCommands))
            {
                stopwatch.Stop();
                return UpdateResult.Completed;
            }

            frameCount++;
            return Game.Update(ticCommands);
        }

        public void DoEvent(in DoomEvent e)
        {
            Game.DoEvent(in e);
        }

        public DoomGame Game { get; }

        public double Fps => frameCount / stopwatch.Elapsed.TotalSeconds;
    }
}
