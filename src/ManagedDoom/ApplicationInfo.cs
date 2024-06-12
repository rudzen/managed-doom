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
using System.Globalization;
using System.Reflection;

namespace ManagedDoom;

public static class ApplicationInfo
{
    public static string Logo()
    {
        const string doom = """
                            ▓█████▄  ▒█████   ▒█████   ███▄ ▄███▓      ███▄    █ ▓█████▄▄▄█████▓
                            ▒██▀ ██▌▒██▒  ██▒▒██▒  ██▒▓██▒▀█▀ ██▒      ██ ▀█   █ ▓█   ▀▓  ██▒ ▓▒
                            ░██   █▌▒██░  ██▒▒██░  ██▒▓██    ▓██░     ▓██  ▀█ ██▒▒███  ▒ ▓██░ ▒░
                            ░▓█▄   ▌▒██   ██░▒██   ██░▒██    ▒██      ▓██▒  ▐▌██▒▒▓█  ▄░ ▓██▓ ░
                            ░▒████▓ ░ ████▓▒░░ ████▓▒░▒██▒   ░██▒ ██▓ ▒██░   ▓██░░▒████▒ ▒██▒ ░
                             ▒▒▓  ▒ ░ ▒░▒░▒░ ░ ▒░▒░▒░ ░ ▒░   ░  ░ ▒▓▒ ░ ▒░   ▒ ▒ ░░ ▒░ ░ ▒ ░░
                             ░ ▒  ▒   ░ ▒ ▒░   ░ ▒ ▒░ ░  ░      ░ ░▒  ░ ░░   ░ ▒░ ░ ░  ░   ░
                             ░ ░  ░ ░ ░ ░ ▒  ░ ░ ░ ▒  ░      ░    ░      ░   ░ ░    ░    ░
                               ░        ░ ░      ░ ░         ░     ░           ░    ░  ░
                             ░                                     ░
                            """;

        return $"{doom}\nBuild at [{GetLinkerTime(Assembly.GetEntryAssembly()!):O}]\n.NET {Environment.Version}";
    }

    public static readonly string Title = $"Managed Doom v2.1a : {Environment.Version}";

    private static DateTime GetLinkerTime(Assembly assembly)
    {
        const string buildVersionMetadataPrefix = "+build";
        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion == null) return default;
        var value = attribute.InformationalVersion;
        var index = value.IndexOf(buildVersionMetadataPrefix, StringComparison.OrdinalIgnoreCase);
        if (index <= 0) return default;
        value = value[(index + buildVersionMetadataPrefix.Length)..];
        return DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss:fffZ", CultureInfo.InvariantCulture);
    }
}