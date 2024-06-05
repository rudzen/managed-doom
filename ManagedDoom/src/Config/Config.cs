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
using System.Text.Json;

namespace ManagedDoom.Config;

public sealed class Config
{
    public ConfigValues Values { get; set; }

    public Config(string path)
    {
        try
        {
            Console.Write("Restore settings: ");
            var start = Stopwatch.GetTimestamp();

            IsRestoredFromFile = File.Exists(path);

            if (!IsRestoredFromFile)
                Values = ConfigValues.CreateDefaults();
            else
            {
                var f = File.ReadAllText(path);
                Values = JsonSerializer.Deserialize(f, ConfigValuesContext.Default.ConfigValues);
            }

            Console.WriteLine($"OK [{Stopwatch.GetElapsedTime(start)}]");
        }
        catch
        {
            IsRestoredFromFile = false;
            Console.WriteLine("Failed to read configuration file");
        }
    }

    public void Save(string path)
    {
        try
        {
            var configJson = JsonSerializer.Serialize(Values, ConfigValuesContext.Default.ConfigValues);
            Console.WriteLine(configJson);
            File.WriteAllText(path, configJson);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unable to save settings. {e.Message}");
        }
    }

    public bool IsRestoredFromFile { get; }
}