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


using System.Collections.Generic;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Event;
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.Menu;

public sealed class LoadMenu : MenuDef
{
    private readonly string[] name;
    private readonly int[] titleX;
    private readonly int[] titleY;
    private readonly TextBoxMenuItem[] items;

    private int index;
    private TextBoxMenuItem choice;

    public LoadMenu(
        DoomMenu menu,
        string name,
        int titleX,
        int titleY,
        int firstChoice,
        params TextBoxMenuItem[] items) : base(menu)
    {
        this.name = [name];
        this.titleX = [titleX];
        this.titleY = [titleY];
        this.items = items;

        index = firstChoice;
        choice = items[index];
    }

    public IReadOnlyList<string> Name => name;
    public IReadOnlyList<int> TitleX => titleX;
    public IReadOnlyList<int> TitleY => titleY;
    public IReadOnlyList<MenuItem> Items => items;
    public MenuItem Choice => choice;

    public override void Open()
    {
        for (var i = 0; i < items.Length; i++)
            items[i].SetText(Menu.SaveSlots[i]);
    }

    private void Up()
    {
        index--;
        if (index < 0)
            index = items.Length - 1;

        choice = items[index];
    }

    private void Down()
    {
        index++;
        if (index >= items.Length)
            index = 0;

        choice = items[index];
    }

    public override bool DoEvent(in DoomEvent e)
    {
        if (e.Type != EventType.KeyDown)
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
            case DoomKey.Enter:
            {
                if (DoLoad(index))
                    Menu.Close();
                Menu.StartSound(Sfx.PISTOL);
                break;
            }
            case DoomKey.Escape:
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
                break;
        }

        return true;
    }

    public bool DoLoad(int slotNumber)
    {
        var slotExists = Menu.SaveSlots[slotNumber] != null;

        if (slotExists)
            Menu.Doom.LoadGame(slotNumber);

        return slotExists;
    }
}