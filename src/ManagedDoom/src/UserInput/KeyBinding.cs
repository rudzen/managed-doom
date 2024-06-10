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
using System.Linq;

namespace ManagedDoom.UserInput;

public sealed class KeyBinding
{
    private static readonly KeyBinding empty = new([], []);

    public DoomKey[] Keys { get; set; }
    public DoomMouseButton[] MouseButtons { get; set; }

    public KeyBinding(DoomKey[] keys, DoomMouseButton[] mouseButtons)
    {
        Keys = keys;
        MouseButtons = mouseButtons;
    }

    public override string ToString()
    {
        var keyValues = Keys.Select(DoomKeyEx.ToString);
        var mouseValues = MouseButtons.Select(DoomMouseButtonEx.ToString);
        var values = keyValues.Concat(mouseValues).ToArray();
        return values.Length > 0 ? string.Join(", ", values) : "none";
    }

    public static KeyBinding Parse(string value)
    {
        if (value == "none")
            return empty;

        var split = value.Split(',');

        var keys = new List<DoomKey>(split.Length);
        var mouseButtons = new List<DoomMouseButton>(split.Length);

        foreach (var s in split)
        {
            var span = s.AsSpan().Trim();
            var key = DoomKeyEx.Parse(span);
            if (key != DoomKey.Unknown)
            {
                keys.Add(key);
                continue;
            }

            var mouseButton = DoomMouseButtonEx.Parse(span);
            if (mouseButton != DoomMouseButton.Unknown)
                mouseButtons.Add(mouseButton);
        }

        return new KeyBinding(keys.ToArray(), mouseButtons.ToArray());
    }
}