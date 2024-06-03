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

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        private class MobjActions
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BFGSpray(World world, Mobj actor) => world.WeaponBehavior.BFGSpray(actor);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Explode(World world, Mobj actor) => world.MonsterBehavior.Explode(actor);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Pain(World world, Mobj actor)
            {
                world.MonsterBehavior.Pain(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void PlayerScream(World world, Mobj actor)
            {
                world.PlayerBehavior.PlayerScream(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Fall(World world, Mobj actor)
            {
                world.MonsterBehavior.Fall(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void XScream(World world, Mobj actor)
            {
                world.MonsterBehavior.XScream(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Look(World world, Mobj actor)
            {
                world.MonsterBehavior.Look(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Chase(World world, Mobj actor)
            {
                world.MonsterBehavior.Chase(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FaceTarget(World world, Mobj actor)
            {
                world.MonsterBehavior.FaceTarget(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void PosAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.PosAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Scream(World world, Mobj actor)
            {
                world.MonsterBehavior.Scream(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SPosAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.SPosAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void VileChase(World world, Mobj actor)
            {
                world.MonsterBehavior.VileChase(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void VileStart(World world, Mobj actor)
            {
                world.MonsterBehavior.VileStart(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void VileTarget(World world, Mobj actor)
            {
                world.MonsterBehavior.VileTarget(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void VileAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.VileAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void StartFire(World world, Mobj actor)
            {
                world.MonsterBehavior.StartFire(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Fire(World world, Mobj actor)
            {
                world.MonsterBehavior.Fire(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FireCrackle(World world, Mobj actor)
            {
                world.MonsterBehavior.FireCrackle(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Tracer(World world, Mobj actor)
            {
                world.MonsterBehavior.Tracer(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SkelWhoosh(World world, Mobj actor)
            {
                world.MonsterBehavior.SkelWhoosh(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SkelFist(World world, Mobj actor)
            {
                world.MonsterBehavior.SkelFist(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SkelMissile(World world, Mobj actor)
            {
                world.MonsterBehavior.SkelMissile(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FatRaise(World world, Mobj actor)
            {
                world.MonsterBehavior.FatRaise(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FatAttack1(World world, Mobj actor)
            {
                world.MonsterBehavior.FatAttack1(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FatAttack2(World world, Mobj actor)
            {
                world.MonsterBehavior.FatAttack2(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FatAttack3(World world, Mobj actor)
            {
                world.MonsterBehavior.FatAttack3(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BossDeath(World world, Mobj actor)
            {
                world.MonsterBehavior.BossDeath(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CPosAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.CPosAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CPosRefire(World world, Mobj actor)
            {
                world.MonsterBehavior.CPosRefire(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void TroopAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.TroopAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SargAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.SargAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void HeadAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.HeadAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BruisAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.BruisAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SkullAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.SkullAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Metal(World world, Mobj actor)
            {
                world.MonsterBehavior.Metal(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SpidRefire(World world, Mobj actor)
            {
                world.MonsterBehavior.SpidRefire(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BabyMetal(World world, Mobj actor)
            {
                world.MonsterBehavior.BabyMetal(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BspiAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.BspiAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Hoof(World world, Mobj actor)
            {
                world.MonsterBehavior.Hoof(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CyberAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.CyberAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void PainAttack(World world, Mobj actor)
            {
                world.MonsterBehavior.PainAttack(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void PainDie(World world, Mobj actor)
            {
                world.MonsterBehavior.PainDie(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void KeenDie(World world, Mobj actor)
            {
                world.MonsterBehavior.KeenDie(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BrainPain(World world, Mobj actor)
            {
                world.MonsterBehavior.BrainPain(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BrainScream(World world, Mobj actor)
            {
                world.MonsterBehavior.BrainScream(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BrainDie(World world, Mobj actor)
            {
                world.MonsterBehavior.BrainDie(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BrainAwake(World world, Mobj actor)
            {
                world.MonsterBehavior.BrainAwake(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BrainSpit(World world, Mobj actor)
            {
                world.MonsterBehavior.BrainSpit(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SpawnSound(World world, Mobj actor)
            {
                world.MonsterBehavior.SpawnSound(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SpawnFly(World world, Mobj actor)
            {
                world.MonsterBehavior.SpawnFly(actor);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void BrainExplode(World world, Mobj actor)
            {
                world.MonsterBehavior.BrainExplode(actor);
            }
        }
    }
}
