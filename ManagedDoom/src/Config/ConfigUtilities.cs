﻿//
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
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ManagedDoom
{
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

        public static string GetExeDirectory => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static string GetConfigPath()
        {
            return Path.Combine(GetExeDirectory, "managed-doom.cfg");
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
            var name = Path.GetFileName(path).ToUpper();
            return iwadNames.Contains(name);
        }

        public static IEnumerable<string> GetWadPaths(CommandLineArgs args)
        {
            if (args.iwad.Present)
            {
                yield return args.iwad.Value;
            }
            else
            {
                yield return GetDefaultIwadPath();
            }

            if (args.file.Present)
            {
                foreach (var path in args.file.Value)
                    yield return path;
            }
        }
    }
}