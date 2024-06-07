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


namespace ManagedDoom.Doom.World;

public sealed class WeaponInfo
{
    public WeaponInfo(
        AmmoType ammo,
        MobjState upState,
        MobjState downState,
        MobjState readyState,
        MobjState attackState,
        MobjState flashState)
    {
        this.Ammo = ammo;
        this.UpState = upState;
        this.DownState = downState;
        this.ReadyState = readyState;
        this.AttackState = attackState;
        this.FlashState = flashState;
    }

    public AmmoType Ammo { get; set; }

    public MobjState UpState { get; set; }

    public MobjState DownState { get; set; }

    public MobjState ReadyState { get; set; }

    public MobjState AttackState { get; set; }

    public MobjState FlashState { get; set; }
}