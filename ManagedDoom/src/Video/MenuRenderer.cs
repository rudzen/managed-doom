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
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Menu;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Video
{
    public sealed class MenuRenderer
    {
        private static readonly char[] cursor = ['_'];

        private readonly DrawScreen screen;

        private readonly PatchCache cache;

        public MenuRenderer(Wad wad, DrawScreen screen)
        {
            this.screen = screen;
            cache = new PatchCache(wad);
        }

        public void Render(DoomMenu menu)
        {
            switch (menu.Current)
            {
                case SelectableMenu selectable:
                    DrawSelectableMenu(selectable);
                    break;
                case SaveMenu save:
                    DrawSaveMenu(save);
                    break;
                case LoadMenu load:
                    DrawLoadMenu(load);
                    break;
                case YesNoConfirm yesNo:
                    DrawText(yesNo.Text);
                    break;
                case PressAnyKey pressAnyKey:
                    DrawText(pressAnyKey.Text);
                    break;
                case QuitConfirm quit:
                    DrawText(quit.Text);
                    break;
                case HelpScreen help:
                    DrawHelp(help);
                    break;
            }
        }

        private void DrawSelectableMenu(SelectableMenu selectable)
        {
            for (var i = 0; i < selectable.Name.Count; i++)
            {
                DrawMenuPatch(
                    selectable.Name[i],
                    selectable.TitleX[i],
                    selectable.TitleY[i]);
            }

            foreach (var item in selectable.Items)
                DrawMenuItem(selectable.Menu, item);

            var choice = selectable.Choice;
            var skull = selectable.Menu.Tics / 8 % 2 == 0 ? "M_SKULL1" : "M_SKULL2";
            DrawMenuPatch(skull, choice.SkullX, choice.SkullY);
        }

        private void DrawSaveMenu(SaveMenu save)
        {
            for (var i = 0; i < save.Name.Count; i++)
            {
                DrawMenuPatch(
                    save.Name[i],
                    save.TitleX[i],
                    save.TitleY[i]);
            }

            foreach (var item in save.Items)
                DrawMenuItem(save.Menu, item);

            var choice = save.Choice;
            var skull = save.Menu.Tics / 8 % 2 == 0 ? "M_SKULL1" : "M_SKULL2";
            DrawMenuPatch(skull, choice.SkullX, choice.SkullY);
        }

        private void DrawLoadMenu(LoadMenu load)
        {
            for (var i = 0; i < load.Name.Count; i++)
            {
                DrawMenuPatch(
                    load.Name[i],
                    load.TitleX[i],
                    load.TitleY[i]);
            }

            foreach (var item in load.Items)
                DrawMenuItem(load.Menu, item);

            var choice = load.Choice;
            var skull = load.Menu.Tics / 8 % 2 == 0 ? "M_SKULL1" : "M_SKULL2";
            DrawMenuPatch(skull, choice.SkullX, choice.SkullY);
        }

        private void DrawMenuItem(DoomMenu menu, MenuItem item)
        {
            switch (item)
            {
                case SimpleMenuItem simple:
                    DrawSimpleMenuItem(simple);
                    break;
                case ToggleMenuItem toggle:
                    DrawToggleMenuItem(toggle);
                    break;
                case SliderMenuItem slider:
                    DrawSliderMenuItem(slider);
                    break;
                case TextBoxMenuItem textBox:
                    DrawTextBoxMenuItem(textBox, menu.Tics);
                    break;
            }
        }

        private void DrawMenuPatch(string name, int x, int y)
        {
            var scale = screen.Width / 320;
            screen.DrawPatch(cache[name], scale * x, scale * y, scale);
        }

        private void DrawMenuText(IReadOnlyList<char> text, int x, int y)
        {
            var scale = screen.Width / 320;
            screen.DrawText(text, scale * x, scale * y, scale);
        }

        private void DrawSimpleMenuItem(SimpleMenuItem item)
        {
            DrawMenuPatch(item.Name, item.ItemX, item.ItemY);
        }

        private void DrawToggleMenuItem(ToggleMenuItem item)
        {
            DrawMenuPatch(item.Name, item.ItemX, item.ItemY);
            DrawMenuPatch(item.State, item.StateX, item.ItemY);
        }

        private void DrawSliderMenuItem(SliderMenuItem item)
        {
            DrawMenuPatch(item.Name, item.ItemX, item.ItemY);

            DrawMenuPatch("M_THERML", item.SliderX, item.SliderY);
            for (var i = 0; i < item.SliderLength; i++)
            {
                var x = item.SliderX + 8 * (1 + i);
                DrawMenuPatch("M_THERMM", x, item.SliderY);
            }

            var end = item.SliderX + 8 * (1 + item.SliderLength);
            DrawMenuPatch("M_THERMR", end, item.SliderY);

            var pos = item.SliderX + 8 * (1 + item.SliderPosition);
            DrawMenuPatch("M_THERMO", pos, item.SliderY);
        }

        private readonly char[] emptyText = "EMPTY SLOT".ToCharArray();

        private void DrawTextBoxMenuItem(TextBoxMenuItem item, int tics)
        {
            var length = 24;
            DrawMenuPatch("M_LSLEFT", item.ItemX, item.ItemY);
            for (var i = 0; i < length; i++)
            {
                var x = item.ItemX + 8 * (1 + i);
                DrawMenuPatch("M_LSCNTR", x, item.ItemY);
            }
            DrawMenuPatch("M_LSRGHT", item.ItemX + 8 * (1 + length), item.ItemY);

            if (!item.Editing)
            {
                var text = item.Text ?? emptyText;
                DrawMenuText(text, item.ItemX + 8, item.ItemY);
            }
            else
            {
                DrawMenuText(item.Text, item.ItemX + 8, item.ItemY);
                if (tics / 3 % 2 == 0)
                {
                    var textWidth = screen.MeasureText(item.Text, 1);
                    DrawMenuText(cursor, item.ItemX + 8 + textWidth, item.ItemY);
                }
            }
        }

        private void DrawText(IReadOnlyList<string> text)
        {
            var scale = screen.Width / 320;
            var height = 7 * scale * text.Count;

            for (var i = 0; i < text.Count; i++)
            {
                var x = (screen.Width - screen.MeasureText(text[i], scale)) / 2;
                var y = (screen.Height - height) / 2 + 7 * scale * (i + 1);
                screen.DrawText(text[i], x, y, scale);
            }
        }

        private void DrawHelp(HelpScreen help)
        {
            var skull = help.Menu.Tics / 8 % 2 == 0 ? "M_SKULL1" : "M_SKULL2";

            if (help.Menu.Options.GameMode == GameMode.Commercial)
            {
                DrawMenuPatch("HELP", 0, 0);
                DrawMenuPatch(skull, 298, 160);
            }
            else
            {
                if (help.Page == 0)
                {
                    DrawMenuPatch("HELP1", 0, 0);
                    DrawMenuPatch(skull, 298, 170);
                }
                else
                {
                    DrawMenuPatch("HELP2", 0, 0);
                    DrawMenuPatch(skull, 248, 180);
                }
            }
        }
    }
}
