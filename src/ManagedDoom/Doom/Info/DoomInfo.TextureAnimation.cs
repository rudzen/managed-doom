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
        new AnimationDef(false, "NUKAGE3", "NUKAGE1"),
        new AnimationDef(false, "FWATER4", "FWATER1"),
        new AnimationDef(false, "SWATER4", "SWATER1"),
        new AnimationDef(false, "LAVA4", "LAVA1"),
        new AnimationDef(false, "BLOOD3", "BLOOD1"),

        // DOOM II flat animations.
        new AnimationDef(false, "RROCK08", "RROCK05"),
        new AnimationDef(false, "SLIME04", "SLIME01"),
        new AnimationDef(false, "SLIME08", "SLIME05"),
        new AnimationDef(false, "SLIME12", "SLIME09"),

        new AnimationDef(true, "BLODGR4", "BLODGR1"),
        new AnimationDef(true, "SLADRIP3", "SLADRIP1"),

        new AnimationDef(true, "BLODRIP4", "BLODRIP1"),
        new AnimationDef(true, "FIREWALL", "FIREWALA"),
        new AnimationDef(true, "GSTFONT3", "GSTFONT1"),
        new AnimationDef(true, "FIRELAVA", "FIRELAV3"),
        new AnimationDef(true, "FIREMAG3", "FIREMAG1"),
        new AnimationDef(true, "FIREBLU2", "FIREBLU1"),
        new AnimationDef(true, "ROCKRED3", "ROCKRED1"),

        new AnimationDef(true, "BFALL4", "BFALL1"),
        new AnimationDef(true, "SFALL4", "SFALL1"),
        new AnimationDef(true, "WFALL4", "WFALL1"),
        new AnimationDef(true, "DBRAIN4", "DBRAIN1")
    ];
}