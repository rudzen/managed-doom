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


using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagedDoom.Config;

public sealed record Warp(int Episode, int Map);

public sealed class CommandLineArgs
{
    public Arg<string> iwad { get; }
    public Arg<string[]> file { get; }
    public Arg<string[]> deh { get; }

    public Arg<Warp> warp { get; }
    public Arg<int> episode { get; }
    public Arg<int> skill { get; }

    public Arg deathmatch { get; }
    public Arg altdeath { get; }
    public Arg fast { get; }
    public Arg respawn { get; }
    public Arg nomonsters { get; }
    public Arg solonet { get; }

    public Arg<string> playdemo { get; }
    public Arg<string> timedemo { get; }

    public Arg<int> loadgame { get; }

    public Arg nomouse { get; }
    public Arg nosound { get; }
    public Arg nosfx { get; }
    public Arg nomusic { get; }

    public Arg nodeh { get; }

    public CommandLineArgs(string[] args)
    {
        iwad = GetString(args, "-iwad");
        file = Check(args, "-file");
        deh = Check(args, "-deh");

        warp = Check_warp(args);
        episode = GetInt(args, "-episode");
        skill = GetInt(args, "-skill");

        deathmatch = new Arg(args.Contains("-deathmatch"));
        altdeath = new Arg(args.Contains("-altdeath"));
        fast = new Arg(args.Contains("-fast"));
        respawn = new Arg(args.Contains("-respawn"));
        nomonsters = new Arg(args.Contains("-nomonsters"));
        solonet = new Arg(args.Contains("-solo-net"));

        playdemo = GetString(args, "-playdemo");
        timedemo = GetString(args, "-timedemo");

        loadgame = GetInt(args, "-loadgame");

        nomouse = new Arg(args.Contains("-nomouse"));
        nosound = new Arg(args.Contains("-nosound"));
        nosfx = new Arg(args.Contains("-nosfx"));
        nomusic = new Arg(args.Contains("-nomusic"));

        nodeh = new Arg(args.Contains("-nodeh"));

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
                iwad = new Arg<string>(iwadPath);

            if (pwadPaths.Count > 0)
                file = new Arg<string[]>(pwadPaths.ToArray());

            if (dehPaths.Count > 0)
                deh = new Arg<string[]>(dehPaths.ToArray());
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

    public readonly record struct Arg(bool Present);

    public readonly struct Arg<T>
    {
        public Arg()
        {
            this.Present = false;
            this.Value = default;
        }

        public Arg(T value)
        {
            this.Present = value is not null;
            this.Value = value;
        }

        public bool Present { get; }

        public T Value { get; }
    }
}