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

namespace ManagedDoom
{
    public sealed class HelpScreen : MenuDef
    {
        private readonly int pageCount;

        public HelpScreen(DoomMenu menu) : base(menu)
        {
            if (menu.Options.GameMode == GameMode.Shareware)
            {
                pageCount = 2;
            }
            else
            {
                pageCount = 1;
            }
        }

        public override void Open()
        {
            Page = pageCount - 1;
        }

        public override bool DoEvent(DoomEvent e)
        {
            if (e.Type != EventType.KeyDown)
            {
                return true;
            }

            if (e.Key is DoomKey.Enter or DoomKey.Space or DoomKey.LControl or DoomKey.RControl)
            {
                Page--;
                if (Page == -1)
                {
                    Menu.Close();
                }
                Menu.StartSound(Sfx.PISTOL);
            }

            if (e.Key == DoomKey.Escape)
            {
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
            }

            return true;
        }

        public int Page { get; private set; }
    }
}
