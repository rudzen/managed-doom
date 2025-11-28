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

namespace ManagedDoom.Config;

public sealed record Warp(int Episode, int Map);

// TODO (rudz) : Replace all this crap with a proper command line parser library.

public sealed class CommandLineArgs
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

    public CommandLineArgs(string[] allArgs)
    {
        var args = allArgs.AsSpan();
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

        // Check for drag & drop (this implementation is deprecated).

        // if (args.Length > 0 && args.All(arg => arg.FirstOrDefault() != '-'))
        // {
        //     var iwadPath = string.Empty;
        //     var pwadPaths = new List<string>();
        //     var dehPaths = new List<string>();
        //
        //     foreach (var path in args)
        //     {
        //         var extension = Path.GetExtension(path);
        //
        //         if (string.Equals(extension, ".wad", StringComparison.OrdinalIgnoreCase))
        //         {
        //             if (ConfigUtilities.IsIwad(path))
        //                 iwadPath = path;
        //             else
        //                 pwadPaths.Add(path);
        //         }
        //         else if (string.Equals(extension, ".deh", StringComparison.OrdinalIgnoreCase))
        //             dehPaths.Add(path);
        //     }
        //
        //     if (!string.IsNullOrEmpty(iwadPath))
        //         Iwad = new Arg<string>(iwadPath);
        //
        //     if (pwadPaths.Count > 0)
        //         File = new Arg<string[]>([.. pwadPaths]);
        //
        //     if (dehPaths.Count > 0)
        //         Deh = new Arg<string[]>([.. dehPaths]);
        // }
    }

    private static Arg<string[]> Check(ReadOnlySpan<string> args, string value)
    {
        var values = GetValues(args, value);
        return values.Length >= 1
            ? ArgExtensions.Success(values.ToArray())
            : ArgExtensions.Failure<string[]>();
    }

    private static Arg<Warp> Check_warp(ReadOnlySpan<string> args)
    {
        var values = GetValues(args, "-warp");
        if (values.Length > 2)
            values = values[..2];

        return values.Length switch
        {
            1 when int.TryParse(values[0], out var map)                                             => ArgExtensions.Success(new Warp(1, map)),
            2 when int.TryParse(values[0], out var episode) && int.TryParse(values[1], out var map) => ArgExtensions.Success(new Warp(episode, map)),
            _                                                                                       => ArgExtensions.Failure<Warp>()
        };
    }

    private static Arg<string> GetString(ReadOnlySpan<string> args, string name)
    {
        var values = GetValues(args, name);
        return values.Length == 1
            ? ArgExtensions.Success(values[0])
            : ArgExtensions.Failure<string>();
    }

    private static Arg<int> GetInt(ReadOnlySpan<string> args, string name)
    {
        var values = GetValues(args, name);
        return values.Length == 1 && int.TryParse(values[0], out var result)
            ? ArgExtensions.Success(result)
            : ArgExtensions.Failure<int>();
    }

    private static ReadOnlySpan<string> GetValues(ReadOnlySpan<string> args, string name)
    {
        var startIndex = args.IndexOf(name);

        if (startIndex == -1 || startIndex == args.Length - 1)
            return [];

        var begin = startIndex + 1;
        var end = begin;

        while (end < args.Length && args[end][0] != '-')
            end++;

        // hack to only read the first iwad after -iwad
        if (name == "-iwad" && end > begin + 1)
            end = begin + 1;

        return args.Slice(begin, end - begin);
    }
}