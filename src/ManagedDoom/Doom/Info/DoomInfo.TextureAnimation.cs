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

using ManagedDoom.Doom.Graphics;

namespace ManagedDoom.Doom.Info;

public static partial class DoomInfo
{
    public static readonly AnimationDef[] TextureAnimation =
    [
        new(false, "NUKAGE3", "NUKAGE1"),
        new(false, "FWATER4", "FWATER1"),
        new(false, "SWATER4", "SWATER1"),
        new(false, "LAVA4", "LAVA1"),
        new(false, "BLOOD3", "BLOOD1"),

        // DOOM II flat animations.
        new(false, "RROCK08", "RROCK05"),
        new(false, "SLIME04", "SLIME01"),
        new(false, "SLIME08", "SLIME05"),
        new(false, "SLIME12", "SLIME09"),

        new(true, "BLODGR4", "BLODGR1"),
        new(true, "SLADRIP3", "SLADRIP1"),

        new(true, "BLODRIP4", "BLODRIP1"),
        new(true, "FIREWALL", "FIREWALA"),
        new(true, "GSTFONT3", "GSTFONT1"),
        new(true, "FIRELAVA", "FIRELAV3"),
        new(true, "FIREMAG3", "FIREMAG1"),
        new(true, "FIREBLU2", "FIREBLU1"),
        new(true, "ROCKRED3", "ROCKRED1"),

        new(true, "BFALL4", "BFALL1"),
        new(true, "SFALL4", "SFALL1"),
        new(true, "WFALL4", "WFALL1"),
        new(true, "DBRAIN4", "DBRAIN1")
    ];
}