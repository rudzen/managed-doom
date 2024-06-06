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
using System.Linq;
using System.Runtime.InteropServices;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.Menu;

public sealed class SaveMenu : MenuDef
{
    private readonly string[] name;
    private readonly int[] titleX;
    private readonly int[] titleY;
    private readonly TextBoxMenuItem[] items;

    private int index;
    private TextBoxMenuItem choice;

    private TextInput textInput;

    public SaveMenu(
        DoomMenu menu,
        string name, int titleX, int titleY,
        int firstChoice,
        params TextBoxMenuItem[] items) : base(menu)
    {
        this.name = [name];
        this.titleX = [titleX];
        this.titleY = [titleY];
        this.items = items;

        index = firstChoice;
        choice = items[index];

        LastSaveSlot = -1;
    }

    public IReadOnlyList<string> Name => name;
    public IReadOnlyList<int> TitleX => titleX;
    public IReadOnlyList<int> TitleY => titleY;
    public IReadOnlyList<MenuItem> Items => items;
    public MenuItem Choice => choice;
    public int LastSaveSlot { get; private set; }

    public override void Open()
    {
        if (Menu.Doom.State != DoomState.Game ||
            Menu.Doom.Game.State != GameState.Level)
        {
            Menu.NotifySaveFailed();
            return;
        }

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

        if (textInput != null)
        {
            var result = textInput.DoEvent(in e);

            switch (textInput.State)
            {
                case TextInputState.Canceled:
                case TextInputState.Finished:
                    textInput = null;
                    break;
            }

            if (result)
                return true;
        }

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
                textInput = choice.Edit(() => DoSave(index));
                Menu.StartSound(Sfx.PISTOL);
                break;
            case DoomKey.Escape:
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
                break;
        }

        return true;
    }

    public void DoSave(int slotNumber)
    {
        var text = items[slotNumber].Text;
        var s = string.Create(text.Count, text, (span, list) =>
        {
            for (var i = 0; i < list.Count; i++)
                span[i] = list[i];
        });
        Menu.SaveSlots[slotNumber] = s;
        if (Menu.Doom.SaveGame(slotNumber, s))
        {
            Menu.Close();
            LastSaveSlot = slotNumber;
        }
        else
            Menu.NotifySaveFailed();

        Menu.StartSound(Sfx.PISTOL);
    }
}