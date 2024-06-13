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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagedDoom.Config;

public sealed record Warp(int Episode, int Map);

public sealed class CommandLineArgs : ICommandLineArgs
{
    public Arg<string> Iwad { get; }
    public Arg<string[]> File { get; }
    public Arg<string[]> Deh { get; }

    public Arg<Warp> Warp { get; }
    public Arg<int> Episode { get; }
    public Arg<int> Skill { get; }
    public Arg DeathMatch { get; }
    public Arg AltDeath { get; }
    public Arg Fast { get; }
    public Arg Respawn { get; }
    public Arg NoMonsters { get; }
    public Arg SoloNet { get; }

    public Arg<string> PlayDemo { get; }
    public Arg<string> TimeDemo { get; }

    public Arg<int> LoadGame { get; }

    public Arg NoMouse { get; }
    public Arg NoSound { get; }
    public Arg NoSfx { get; }
    public Arg NoMusic { get; }

    public Arg NoDeh { get; }

    public CommandLineArgs(string[] args)
    {
        Iwad = GetString(args, "-iwad");
        File = Check(args, "-file");
        Deh = Check(args, "-deh");

        Warp = Check_warp(args);
        Episode = GetInt(args, "-episode");
        Skill = GetInt(args, "-skill");

        DeathMatch = new Arg(args.Contains("-deathmatch"));
        AltDeath = new Arg(args.Contains("-altdeath"));
        Fast = new Arg(args.Contains("-fast"));
        Respawn = new Arg(args.Contains("-respawn"));
        NoMonsters = new Arg(args.Contains("-nomonsters"));
        SoloNet = new Arg(args.Contains("-solo-net"));

        PlayDemo = GetString(args, "-playdemo");
        TimeDemo = GetString(args, "-timedemo");

        LoadGame = GetInt(args, "-loadgame");

        NoMouse = new Arg(args.Contains("-nomouse"));
        NoSound = new Arg(args.Contains("-nosound"));
        NoSfx = new Arg(args.Contains("-nosfx"));
        NoMusic = new Arg(args.Contains("-nomusic"));

        NoDeh = new Arg(args.Contains("-nodeh"));

        // Check for drag & drop.
        if (args.Length > 0 && args.All(arg => arg.FirstOrDefault() != '-'))
        {
            var iwadPath = string.Empty;
            var pwadPaths = new List<string>();
            var dehPaths = new List<string>();

            foreach (var path in args)
            {
                var extension = Path.GetExtension(path).ToLower();

                if (extension == ".wad")
                {
                    if (ConfigUtilities.IsIwad(path))
                        iwadPath = path;
                    else
                        pwadPaths.Add(path);
                }
                else if (extension == ".deh")
                    dehPaths.Add(path);
            }

            if (!string.IsNullOrEmpty(iwadPath))
                Iwad = new Arg<string>(iwadPath);

            if (pwadPaths.Count > 0)
                File = new Arg<string[]>(pwadPaths.ToArray());

            if (dehPaths.Count > 0)
                Deh = new Arg<string[]>(dehPaths.ToArray());
        }
    }

    private static Arg<string[]> Check(string[] args, string value)
    {
        var values = GetValues(args, value);
        return values.Length >= 1 ? new Arg<string[]>(values) : new Arg<string[]>();
    }

    private static Arg<Warp> Check_warp(string[] args)
    {
        var values = GetValues(args, "-warp");
        return values.Length switch
        {
            1 when int.TryParse(values[0], out var map)                                             => new Arg<Warp>(new(1, map)),
            2 when int.TryParse(values[0], out var episode) && int.TryParse(values[1], out var map) => new Arg<Warp>(new(episode, map)),
            _                                                                                       => new Arg<Warp>()
        };
    }

    private static Arg<string> GetString(string[] args, string name)
    {
        var values = GetValues(args, name);
        return values.Length == 1 ? new Arg<string>(values[0]) : new Arg<string>();
    }

    private static Arg<int> GetInt(string[] args, string name)
    {
        var values = GetValues(args, name);
        return values.Length == 1 && int.TryParse(values[0], out var result)
            ? new Arg<int>(result)
            : new Arg<int>();
    }

    private static string[] GetValues(string[] args, string name)
    {
        return args
               .SkipWhile(arg => arg != name)
               .Skip(1)
               .TakeWhile(arg => arg[0] != '-')
               .ToArray();
    }
}