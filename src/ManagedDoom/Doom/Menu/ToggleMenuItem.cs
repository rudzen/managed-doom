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

public sealed record ToggleMenuItem(
    string Name,
    int SkullX,
    int SkullY,
    int ItemX,
    int ItemY,
    string state1,
    string state2,
    int StateX,
    Func<int> OnReset,
    Action<int> action,
    IMenuDef Next = null)
    : IMenuItem
{
    private readonly string[] states = [state1, state2];

    private int stateNumber;

    public string State => states[stateNumber];

    public void Reset()
    {
        if (OnReset != null)
            stateNumber = OnReset();
    }

    public void Up()
    {
        stateNumber++;
        if (stateNumber == states.Length)
            stateNumber = 0;

        action.Invoke(stateNumber);
    }

    public void Down()
    {
        stateNumber--;
        if (stateNumber == -1)
            stateNumber = states.Length - 1;

        action.Invoke(stateNumber);
    }
}