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
using System.IO;
using System.Runtime.CompilerServices;

namespace ManagedDoom
{
    public sealed class SaveSlots
    {
        private const int slotCount = 6;
        private const int descriptionSize = 24;

        private string[] slots;

        [SkipLocalsInit]
        private void ReadSlots()
        {
            if (slots == null)
                slots = new string[slotCount];
            else
                Array.Clear(slots);

            var directory = ConfigUtilities.GetExeDirectory();
            Span<byte> buffer = stackalloc byte[descriptionSize];
            for (var i = 0; i < slots.Length; i++)
            {
                var path = Path.Combine(directory, $"doomsav{i}.dsg");
                if (File.Exists(path))
                {
                    using var reader = new FileStream(path, FileMode.Open, FileAccess.Read);
                    var read = reader.Read(buffer);
                    slots[i] = DoomInterop.ToString(buffer[..read]);
                }
            }
        }

        public string this[int number]
        {
            get
            {
                if (slots == null)
                    ReadSlots();

                return slots[number];
            }

            set => slots[number] = value;
        }

        public int Count => slots.Length;
    }
}
