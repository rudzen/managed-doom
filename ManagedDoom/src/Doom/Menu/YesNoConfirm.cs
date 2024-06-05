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
using ManagedDoom.Audio;
using ManagedDoom.Doom.Event;
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.Menu
{
    public sealed class YesNoConfirm : MenuDef
    {
        private readonly string[] text;
        private readonly Action action;

        public YesNoConfirm(DoomMenu menu, string text, Action action) : base(menu)
        {
            this.text = text.Split('\n');
            this.action = action;
        }

        public override bool DoEvent(in DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
                return true;

            if (e.Key is DoomKey.Y or DoomKey.Enter or DoomKey.Space)
            {
                action();
                Menu.Close();
                Menu.StartSound(Sfx.PISTOL);
            }

            if (e.Key is DoomKey.N or DoomKey.Escape)
            {
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
            }

            return true;
        }

        public IReadOnlyList<string> Text => text;
    }
}
