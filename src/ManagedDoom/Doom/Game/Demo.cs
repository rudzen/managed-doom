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
using System.IO;
using System.Linq;

namespace ManagedDoom.Doom.Game;

public sealed class Demo
{
    private readonly byte[] data;
    private readonly int playerCount;
    private int p;

    public Demo(byte[] data)
    {
        p = 0;

        if (data[p++] != 109)
            throw new Exception("Demo is from a different game version!");

        this.data = data;

        Options = GameOptions.CreateDefault();
        Options.Skill = (GameSkill)data[p++];

        Options.Episode = data[p++];
        Options.Map = data[p++];
        Options.Deathmatch = data[p++];
        Options.RespawnMonsters = data[p++] != 0;
        Options.FastMonsters = data[p++] != 0;
        Options.NoMonsters = data[p++] != 0;
        Options.ConsolePlayer = data[p++];

        Options.Players[0].InGame = data[p++] != 0;
        Options.Players[1].InGame = data[p++] != 0;
        Options.Players[2].InGame = data[p++] != 0;
        Options.Players[3].InGame = data[p++] != 0;

        Options.DemoPlayback = true;

        playerCount = Options.Players.Count(x => x.InGame);
        Options.NetGame = playerCount >= 2;
    }

    public Demo(string fileName) : this(File.ReadAllBytes(fileName))
    {
    }

    public GameOptions Options { get; }

    public bool ReadCmd(ReadOnlySpan<TicCommand> cmds)
    {
        if (p == data.Length)
            return false;

        if (data[p] == 0x80)
            return false;

        if (p + 4 * playerCount > data.Length)
            return false;

        for (var i = 0; i < Options.Players.Length; i++)
        {
            if (!Options.Players[i].InGame)
                continue;

            var cmd = cmds[i];
            cmd.ForwardMove = (sbyte)data[p++];
            cmd.SideMove = (sbyte)data[p++];
            cmd.AngleTurn = (short)(data[p++] << 8);
            cmd.Buttons = data[p++];
        }

        return true;
    }
}