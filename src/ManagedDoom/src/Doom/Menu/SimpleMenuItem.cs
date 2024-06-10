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

namespace ManagedDoom.Doom.Menu;

public sealed class SimpleMenuItem : MenuItem
{
    private readonly Func<bool> selectable;

    public SimpleMenuItem(
        string name,
        int skullX, int skullY,
        int itemX, int itemY,
        Action action, MenuDef next)
        : base(skullX, skullY, next)
    {
        this.Name = name;
        this.ItemX = itemX;
        this.ItemY = itemY;
        this.Action = action;
        this.selectable = null;
    }

    public SimpleMenuItem(
        string name,
        int skullX, int skullY,
        int itemX, int itemY,
        Action action, MenuDef next, Func<bool> selectable)
        : base(skullX, skullY, next)
    {
        this.Name = name;
        this.ItemX = itemX;
        this.ItemY = itemY;
        this.Action = action;
        this.selectable = selectable;
    }

    public string Name { get; }

    public int ItemX { get; }

    public int ItemY { get; }

    public Action Action { get; }

    public bool Selectable => selectable == null || selectable();
}