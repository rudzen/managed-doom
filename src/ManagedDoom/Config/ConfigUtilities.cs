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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagedDoom.Config;

public static class ConfigUtilities
{
    private static readonly string[] iwadNames =
    [
        "DOOM2.WAD",
        "PLUTONIA.WAD",
        "TNT.WAD",
        "DOOM.WAD",
        "DOOM1.WAD",
        "FREEDOOM2.WAD",
        "FREEDOOM1.WAD"
    ];

    public static string GetExeDirectory => Path.GetDirectoryName(Environment.ProcessPath)!;

    public static string GetConfigPath()
    {
        return Path.Combine(GetExeDirectory, "managed-doom.json");
    }

    private static string GetDefaultIwadPath()
    {
        var exeDirectory = GetExeDirectory;
        var currentDirectory = Directory.GetCurrentDirectory();
        foreach (var name in iwadNames)
        {
            var path = Path.Combine(exeDirectory, name);
            if (File.Exists(path))
                return path;

            path = Path.Combine(currentDirectory, name);
            if (File.Exists(path))
                return path;
        }

        throw new Exception("No IWAD was found!");
    }

    public static bool IsIwad(string path)
    {
        var name = Path.GetFileName(path);
        return iwadNames.Any(wadFile => string.Equals(name, wadFile, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<string> GetWadPaths(ICommandLineArgs args)
    {
        if (args.Iwad.Present)
            yield return args.Iwad.Value;
        else
            yield return GetDefaultIwadPath();

        if (!args.File.Present)
            yield break;

        foreach (var path in args.File.Value)
            yield return path;
    }
}