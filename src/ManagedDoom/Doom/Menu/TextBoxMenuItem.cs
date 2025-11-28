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
using System.Text;

namespace ManagedDoom.Doom.Menu;

public sealed class TextBoxMenuItem(int skullX, int skullY, int itemX, int itemY) : MenuItem(skullX, skullY, null)
{
    private StringBuilder text = new();
    private TextInput? edit;

    public string Text => edit?.Text.ToString() ?? text.ToString();
    public int ItemX { get; } = itemX;
    public int ItemY { get; } = itemY;
    public bool Editing => edit is not null;

    public TextInput Edit(Action finished)
    {
        edit = new TextInput(
            text.Length == 0 ? string.Empty : text.ToString(),
            cs => { },
            cs =>
            {
                text = cs;
                edit = null;
                finished();
            },
            () => { edit = null; });

        return edit;
    }

    public void SetText(string? inputText)
    {
        if (string.IsNullOrEmpty(inputText))
            return;

        this.text.Clear();
        this.text.Append(inputText);
    }
}