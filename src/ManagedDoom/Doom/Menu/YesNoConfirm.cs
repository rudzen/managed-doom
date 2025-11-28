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
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.Menu;

public sealed class YesNoConfirm(DoomMenu menu, string text, Action action) : MenuDef(menu)
{
    private readonly string[] text = text.Split('\n');

    public override bool DoEvent(DoomEvent e)
    {
        if (e.Type != EventType.KeyDown)
            return true;

        switch (e.Key)
        {
            case DoomKey.Y or DoomKey.Enter or DoomKey.Space:
                action();
                Menu.Close();
                Menu.StartSound(Sfx.PISTOL);
                break;
            case DoomKey.N or DoomKey.Escape:
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
                break;
        }

        return true;
    }

    public ReadOnlySpan<string> Text => text;
}