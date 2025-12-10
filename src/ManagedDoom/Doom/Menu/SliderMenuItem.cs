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

namespace ManagedDoom.Doom.Menu;

public sealed record SliderMenuItem(
    string Name,
    int SkullX,
    int SkullY,
    int ItemX,
    int ItemY,
    int SliderLength,
    Func<int> OnReset,
    Action<int> action,
    MenuDef Next = null)
    : IMenuItem
{
    public int SliderX => ItemX;
    public int SliderY => ItemY + 16;
    public int SliderPosition { get; private set; }

    public void Reset()
    {
        if (OnReset is not null)
            SliderPosition = OnReset();
    }

    public void Up()
    {
        if (SliderPosition < SliderLength - 1)
            SliderPosition++;

        action?.Invoke(SliderPosition);
    }

    public void Down()
    {
        if (SliderPosition > 0)
            SliderPosition--;

        action?.Invoke(SliderPosition);
    }
}