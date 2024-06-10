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


namespace ManagedDoom.Doom.Menu;

public abstract class MenuItem
{
    protected MenuItem(int skullX, int skullY, MenuDef next)
    {
        this.SkullX = skullX;
        this.SkullY = skullY;
        this.Next = next;
    }

    public int SkullX { get; }

    public int SkullY { get; }

    public MenuDef Next { get; }
}