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


using ManagedDoom.Doom.Common;

namespace ManagedDoom.Doom.Info;

public sealed record DoomStringPair(DoomString First, DoomString Second);

public static partial class DoomInfo
{
    public static readonly DoomStringPair[] SwitchNames =
    [
        new(new DoomString("SW1BRCOM"), new DoomString("SW2BRCOM")),
        new(new DoomString("SW1BRN1"), new DoomString("SW2BRN1")),
        new(new DoomString("SW1BRN2"), new DoomString("SW2BRN2")),
        new(new DoomString("SW1BRNGN"), new DoomString("SW2BRNGN")),
        new(new DoomString("SW1BROWN"), new DoomString("SW2BROWN")),
        new(new DoomString("SW1COMM"), new DoomString("SW2COMM")),
        new(new DoomString("SW1COMP"), new DoomString("SW2COMP")),
        new(new DoomString("SW1DIRT"), new DoomString("SW2DIRT")),
        new(new DoomString("SW1EXIT"), new DoomString("SW2EXIT")),
        new(new DoomString("SW1GRAY"), new DoomString("SW2GRAY")),
        new(new DoomString("SW1GRAY1"), new DoomString("SW2GRAY1")),
        new(new DoomString("SW1METAL"), new DoomString("SW2METAL")),
        new(new DoomString("SW1PIPE"), new DoomString("SW2PIPE")),
        new(new DoomString("SW1SLAD"), new DoomString("SW2SLAD")),
        new(new DoomString("SW1STARG"), new DoomString("SW2STARG")),
        new(new DoomString("SW1STON1"), new DoomString("SW2STON1")),
        new(new DoomString("SW1STON2"), new DoomString("SW2STON2")),
        new(new DoomString("SW1STONE"), new DoomString("SW2STONE")),
        new(new DoomString("SW1STRTN"), new DoomString("SW2STRTN")),
        new(new DoomString("SW1BLUE"), new DoomString("SW2BLUE")),
        new(new DoomString("SW1CMT"), new DoomString("SW2CMT")),
        new(new DoomString("SW1GARG"), new DoomString("SW2GARG")),
        new(new DoomString("SW1GSTON"), new DoomString("SW2GSTON")),
        new(new DoomString("SW1HOT"), new DoomString("SW2HOT")),
        new(new DoomString("SW1LION"), new DoomString("SW2LION")),
        new(new DoomString("SW1SATYR"), new DoomString("SW2SATYR")),
        new(new DoomString("SW1SKIN"), new DoomString("SW2SKIN")),
        new(new DoomString("SW1VINE"), new DoomString("SW2VINE")),
        new(new DoomString("SW1WOOD"), new DoomString("SW2WOOD")),
        new(new DoomString("SW1PANEL"), new DoomString("SW2PANEL")),
        new(new DoomString("SW1ROCK"), new DoomString("SW2ROCK")),
        new(new DoomString("SW1MET2"), new DoomString("SW2MET2")),
        new(new DoomString("SW1WDMET"), new DoomString("SW2WDMET")),
        new(new DoomString("SW1BRIK"), new DoomString("SW2BRIK")),
        new(new DoomString("SW1MOD1"), new DoomString("SW2MOD1")),
        new(new DoomString("SW1ZIM"), new DoomString("SW2ZIM")),
        new(new DoomString("SW1STON6"), new DoomString("SW2STON6")),
        new(new DoomString("SW1TEK"), new DoomString("SW2TEK")),
        new(new DoomString("SW1MARB"), new DoomString("SW2MARB")),
        new(new DoomString("SW1SKULL"), new DoomString("SW2SKULL"))
    ];
}