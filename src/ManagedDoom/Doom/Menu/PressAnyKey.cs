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
using ManagedDoom.Audio;
using ManagedDoom.Doom.Event;

namespace ManagedDoom.Doom.Menu;

public sealed class PressAnyKey : IMenuDef
{
    private readonly string[] text;
    private readonly Action action;

    public PressAnyKey(DoomMenu menu, string text, Action action)
    {
        this.Menu = menu;
        this.action = action;
        this.text = text.Split('\n');
    }

    public ReadOnlySpan<string> Text => text.AsSpan();

    public DoomMenu Menu { get; }

    public void Open()
    {
    }

    public void Update()
    {
    }

    public bool DoEvent(DoomEvent e)
    {
        if (e.Type != EventType.KeyDown)
            return true;

        action?.Invoke();

        Menu.Close();
        Menu.StartSound(Sfx.SWTCHX);

        return true;
    }
}