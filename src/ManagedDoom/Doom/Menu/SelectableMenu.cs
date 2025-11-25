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

public sealed class SelectableMenu : MenuDef
{
    private readonly int[] titleX;
    private readonly int[] titleY;
    private readonly MenuItem[] items;

    private int index;

    private TextInput? textInput;

    public SelectableMenu(
        DoomMenu menu,
        string name, int titleX, int titleY,
        int firstChoice,
        params MenuItem[] items) : base(menu)
    {
        this.Name = [name];
        this.titleX = [titleX];
        this.titleY = [titleY];
        this.items = items;

        index = firstChoice;
        Choice = items[index];
    }

    public SelectableMenu(
        DoomMenu menu,
        string name1, int titleX1, int titleY1,
        string name2, int titleX2, int titleY2,
        int firstChoice,
        params MenuItem[] items) : base(menu)
    {
        this.Name = [name1, name2];
        this.titleX = [titleX1, titleX2];
        this.titleY = [titleY1, titleY2];
        this.items = items;

        index = firstChoice;
        Choice = items[index];
    }

    public string[] Name { get; }
    public ReadOnlySpan<int> TitleX => titleX;
    public ReadOnlySpan<int> TitleY => titleY;
    public ReadOnlySpan<MenuItem> Items => items;
    public MenuItem Choice { get; private set; }

    public override void Open()
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case ToggleMenuItem toggle:
                    toggle.Reset();
                    break;
                case SliderMenuItem slider:
                    slider.Reset();
                    break;
            }
        }
    }

    private void Up()
    {
        index--;
        if (index < 0)
            index = items.Length - 1;

        Choice = items[index];
    }

    private void Down()
    {
        index++;
        if (index >= items.Length)
            index = 0;

        Choice = items[index];
    }

    public override bool DoEvent(DoomEvent e)
    {
        if (e.Type != EventType.KeyDown)
            return true;

        if (textInput is not null && HandleTextInputEvent(e))
            return true;

        switch (e.Key)
        {
            case DoomKey.Up:
                Up();
                Menu.StartSound(Sfx.PSTOP);
                break;
            case DoomKey.Down:
                Down();
                Menu.StartSound(Sfx.PSTOP);
                break;
            case DoomKey.Left:
            {
                switch (Choice)
                {
                    case ToggleMenuItem toggle:
                        toggle.Down();
                        Menu.StartSound(Sfx.PISTOL);
                        break;
                    case SliderMenuItem slider:
                        slider.Down();
                        Menu.StartSound(Sfx.STNMOV);
                        break;
                }

                break;
            }
            case DoomKey.Right:
            {
                switch (Choice)
                {
                    case ToggleMenuItem toggle:
                        toggle.Up();
                        Menu.StartSound(Sfx.PISTOL);
                        break;
                    case SliderMenuItem slider:
                        slider.Up();
                        Menu.StartSound(Sfx.STNMOV);
                        break;
                }

                break;
            }
            case DoomKey.Enter:
            {
                switch (Choice)
                {
                    case ToggleMenuItem toggle:
                        toggle.Up();
                        Menu.StartSound(Sfx.PISTOL);
                        break;
                    case SimpleMenuItem simple:
                    {
                        if (simple.Selectable)
                        {
                            simple.Action?.Invoke();
                            if (simple.Next is not null)
                                Menu.SetCurrent(simple.Next);
                            else
                                Menu.Close();
                        }

                        Menu.StartSound(Sfx.PISTOL);
                        return true;
                    }
                }

                if (Choice.Next is not null)
                {
                    Menu.SetCurrent(Choice.Next);
                    Menu.StartSound(Sfx.PISTOL);
                }

                break;
            }
            case DoomKey.Escape:
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
                break;
        }

        return true;
    }

    private bool HandleTextInputEvent(DoomEvent e)
    {
        var result = textInput!.DoEvent(e);

        textInput = textInput.State switch
        {
            TextInputState.Canceled or TextInputState.Finished => null,
            _                                                  => textInput
        };

        return result;
    }
}