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

using System.Runtime.CompilerServices;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Info;

public static partial class DoomInfo
{
    private static class MobjActions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BFGSpray(World.World world, Mobj actor) => world.WeaponBehavior.BFGSpray(actor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Explode(World.World world, Mobj actor) => world.MonsterBehavior.Explode(actor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pain(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Pain(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayerScream(World.World world, Mobj actor)
        {
            world.PlayerBehavior.PlayerScream(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fall(World.World world, Mobj actor)
        {
            MonsterBehavior.Fall(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void XScream(World.World world, Mobj actor)
        {
            world.MonsterBehavior.XScream(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Look(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Look(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Chase(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Chase(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FaceTarget(World.World world, Mobj actor)
        {
            world.MonsterBehavior.FaceTarget(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PosAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.PosAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scream(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Scream(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SPosAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SPosAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void VileChase(World.World world, Mobj actor)
        {
            world.MonsterBehavior.VileChase(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void VileStart(World.World world, Mobj actor)
        {
            world.MonsterBehavior.VileStart(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void VileTarget(World.World world, Mobj actor)
        {
            world.MonsterBehavior.VileTarget(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void VileAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.VileAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StartFire(World.World world, Mobj actor)
        {
            world.MonsterBehavior.StartFire(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fire(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Fire(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FireCrackle(World.World world, Mobj actor)
        {
            world.MonsterBehavior.FireCrackle(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Tracer(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Tracer(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkelWhoosh(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SkelWhoosh(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkelFist(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SkelFist(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkelMissile(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SkelMissile(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FatRaise(World.World world, Mobj actor)
        {
            world.MonsterBehavior.FatRaise(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FatAttack1(World.World world, Mobj actor)
        {
            world.MonsterBehavior.FatAttack1(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FatAttack2(World.World world, Mobj actor)
        {
            world.MonsterBehavior.FatAttack2(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FatAttack3(World.World world, Mobj actor)
        {
            world.MonsterBehavior.FatAttack3(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BossDeath(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BossDeath(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CPosAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.CPosAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CPosRefire(World.World world, Mobj actor)
        {
            world.MonsterBehavior.CPosRefire(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TroopAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.TroopAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SargAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SargAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HeadAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.HeadAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BruisAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BruisAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkullAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SkullAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Metal(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Metal(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpidRefire(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SpidRefire(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BabyMetal(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BabyMetal(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BspiAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BspiAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Hoof(World.World world, Mobj actor)
        {
            world.MonsterBehavior.Hoof(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CyberAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.CyberAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PainAttack(World.World world, Mobj actor)
        {
            world.MonsterBehavior.PainAttack(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PainDie(World.World world, Mobj actor)
        {
            world.MonsterBehavior.PainDie(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void KeenDie(World.World world, Mobj actor)
        {
            world.MonsterBehavior.KeenDie(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BrainPain(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BrainPain(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BrainScream(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BrainScream(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BrainDie(World.World world, Mobj _)
        {
            world.MonsterBehavior.BrainDie();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BrainAwake(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BrainAwake(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BrainSpit(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BrainSpit(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpawnSound(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SpawnSound(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpawnFly(World.World world, Mobj actor)
        {
            world.MonsterBehavior.SpawnFly(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BrainExplode(World.World world, Mobj actor)
        {
            world.MonsterBehavior.BrainExplode(actor);
        }
    }
}