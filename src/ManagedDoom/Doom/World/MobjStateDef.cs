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
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;

namespace ManagedDoom.Doom.World;

public class MobjStateDef
{
    public MobjStateDef(
        int number,
        Sprite sprite,
        int frame,
        int tics,
        Action<World, Player, PlayerSpriteDef>? playerAction,
        Action<World, Mobj>? mobjAction,
        MobjState next,
        int misc1,
        int misc2)
    {
        this.Number = number;
        this.Sprite = sprite;
        this.Frame = frame;
        this.Tics = tics;
        this.PlayerAction = playerAction;
        this.MobjAction = mobjAction;
        this.Next = next;
        this.Misc1 = misc1;
        this.Misc2 = misc2;
    }

    public int Number { get; set; }

    public Sprite Sprite { get; set; }

    public int Frame { get; set; }

    public int Tics { get; set; }

    public Action<World, Player, PlayerSpriteDef>? PlayerAction { get; set; }

    public Action<World, Mobj>? MobjAction { get; set; }

    public MobjState Next { get; set; }

    public int Misc1 { get; set; }

    public int Misc2 { get; set; }
}