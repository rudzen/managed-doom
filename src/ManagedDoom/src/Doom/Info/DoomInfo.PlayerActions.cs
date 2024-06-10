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


using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Info;

public static partial class DoomInfo
{
    private static class PlayerActions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Light0(World.World world, Player player, PlayerSpriteDef psp)
        {
            WeaponBehavior.Light0(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WeaponReady(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.WeaponReady(player, psp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Lower(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.Lower(player, psp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.Raise(player, psp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Punch(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.Punch(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReFire(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.ReFire(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FirePistol(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.FirePistol(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Light1(World.World world, Player player, PlayerSpriteDef psp)
        {
            WeaponBehavior.Light1(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FireShotgun(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.FireShotgun(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Light2(World.World world, Player player, PlayerSpriteDef psp)
        {
            WeaponBehavior.Light2(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FireShotgun2(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.FireShotgun2(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckReload(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.CheckReload(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OpenShotgun2(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.OpenShotgun2(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadShotgun2(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.LoadShotgun2(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CloseShotgun2(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.CloseShotgun2(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FireCGun(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.FireCGun(player, psp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GunFlash(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.GunFlash(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FireMissile(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.FireMissile(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Saw(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.Saw(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FirePlasma(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.FirePlasma(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BFGsound(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.A_BFGsound(player);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FireBFG(World.World world, Player player, PlayerSpriteDef psp)
        {
            world.WeaponBehavior.FireBFG(player);
        }
    }
}