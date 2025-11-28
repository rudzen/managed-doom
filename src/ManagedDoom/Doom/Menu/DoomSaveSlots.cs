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
using System.Runtime.CompilerServices;
using ManagedDoom.Config;
using ManagedDoom.Doom.Common;

namespace ManagedDoom.Doom.Menu;

public static class DoomSaveSlots
{
    [SkipLocalsInit]
    public static string[] ReadSlots()
    {
        const int slotCount = 6;
        const int descriptionSize = 24;
        var directory = ConfigUtilities.GetExeDirectory;
        var slots = new string[slotCount];
        Span<byte> buffer = stackalloc byte[descriptionSize];
        for (var i = 0; i < slots.Length; i++)
        {
            var path = Path.Combine(directory, $"doomsav{i}.dsg");
            if (!File.Exists(path))
            {
                slots[i] = string.Empty;
                continue;
            }

            using var reader = File.OpenRead(path);
            var read = reader.Read(buffer);
            slots[i] = DoomInterop.ToString(buffer[..read]);
        }

        return slots;
    }
}