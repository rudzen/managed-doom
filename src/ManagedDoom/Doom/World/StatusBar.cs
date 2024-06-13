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
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class StatusBar
{
    // Used for evil grin.
    private readonly bool[] oldWeaponsOwned;

    private readonly DoomRandom random;
    private readonly World world;

    // Count until face changes.
    private int faceCount;

    private int lastAttackDown;
    private int lastPainOffset;

    // Used for appopriately pained face.
    private int oldHealth;

    private int priority;

    // Current face index.

    // A random number per tick.
    private int randomNumber;

    public StatusBar(World world)
    {
        this.world = world;

        oldHealth = -1;
        oldWeaponsOwned = new bool[DoomInfo.WeaponInfos.Length];
        Array.Copy(
            world.ConsolePlayer.WeaponOwned,
            oldWeaponsOwned,
            DoomInfo.WeaponInfos.Length);
        faceCount = 0;
        FaceIndex = 0;
        randomNumber = 0;
        priority = 0;
        lastAttackDown = -1;
        lastPainOffset = 0;

        random = new DoomRandom();
    }

    public int FaceIndex { get; private set; }

    public void Reset()
    {
        oldHealth = -1;
        Array.Copy(
            world.ConsolePlayer.WeaponOwned,
            oldWeaponsOwned,
            DoomInfo.WeaponInfos.Length);
        faceCount = 0;
        FaceIndex = 0;
        randomNumber = 0;
        priority = 0;
        lastAttackDown = -1;
        lastPainOffset = 0;
    }

    public void Update()
    {
        randomNumber = random.Next();
        UpdateFace();
    }

    private void UpdateFace()
    {
        var player = world.ConsolePlayer;

        if (priority < 10)
        {
            // Dead.
            if (player.Health == 0)
            {
                priority = 9;
                FaceIndex = Face.DeadIndex;
                faceCount = 1;
            }
        }

        if (priority < 9)
        {
            if (player.BonusCount != 0)
            {
                // Picking up bonus.
                var doEvilGrin = false;

                for (var i = 0; i < DoomInfo.WeaponInfos.Length; i++)
                {
                    if (oldWeaponsOwned[i] != player.WeaponOwned[i])
                    {
                        doEvilGrin = true;
                        oldWeaponsOwned[i] = player.WeaponOwned[i];
                    }
                }

                if (doEvilGrin)
                {
                    // Evil grin if just picked up weapon.
                    priority = 8;
                    faceCount = Face.EvilGrinDuration;
                    FaceIndex = CalcPainOffset() + Face.EvilGrinOffset;
                }
            }
        }

        if (priority < 8)
        {
            if (player.DamageCount != 0 &&
                player.Attacker != null &&
                player.Attacker != player.Mobj)
            {
                // Being attacked.
                priority = 7;

                if (player.Health - oldHealth > Face.MuchPain)
                {
                    faceCount = Face.TurnDuration;
                    FaceIndex = CalcPainOffset() + Face.OuchOffset;
                }
                else
                {
                    var attackerAngle = Geometry.PointToAngle(
                        player.Mobj.X, player.Mobj.Y,
                        player.Attacker.X, player.Attacker.Y);

                    Angle diff;
                    bool right;
                    if (attackerAngle > player.Mobj.Angle)
                    {
                        // Whether right or left.
                        diff = attackerAngle - player.Mobj.Angle;
                        right = diff > Angle.Ang180;
                    }
                    else
                    {
                        // Whether left or right.
                        diff = player.Mobj.Angle - attackerAngle;
                        right = diff <= Angle.Ang180;
                    }

                    faceCount = Face.TurnDuration;
                    FaceIndex = CalcPainOffset();

                    // Head-on.
                    if (diff < Angle.Ang45)
                        FaceIndex += Face.RampageOffset;
                    // Turn face right.
                    else if (right)
                        FaceIndex += Face.TurnOffset;
                    // Turn face left.
                    else
                        FaceIndex += Face.TurnOffset + 1;
                }
            }
        }

        if (priority < 7)
        {
            // Getting hurt because of your own damn stupidity.
            if (player.DamageCount != 0)
            {
                if (player.Health - oldHealth > Face.MuchPain)
                {
                    priority = 7;
                    faceCount = Face.TurnDuration;
                    FaceIndex = CalcPainOffset() + Face.OuchOffset;
                }
                else
                {
                    priority = 6;
                    faceCount = Face.TurnDuration;
                    FaceIndex = CalcPainOffset() + Face.RampageOffset;
                }
            }
        }

        if (priority < 6)
        {
            // Rapid firing.
            if (player.AttackDown)
            {
                if (lastAttackDown == -1)
                    lastAttackDown = Face.RampageDelay;
                else if (--lastAttackDown == 0)
                {
                    priority = 5;
                    FaceIndex = CalcPainOffset() + Face.RampageOffset;
                    faceCount = 1;
                    lastAttackDown = 1;
                }
            }
            else
                lastAttackDown = -1;
        }

        if (priority < 5)
        {
            // Invulnerability.
            if ((player.Cheats & CheatFlags.GodMode) != 0 || player.Powers[PowerType.Invulnerability] != 0)
            {
                priority = 4;

                FaceIndex = Face.GodIndex;
                faceCount = 1;
            }
        }

        // Look left or look right if the facecount has timed out.
        if (faceCount == 0)
        {
            FaceIndex = CalcPainOffset() + (randomNumber % 3);
            faceCount = Face.StraightFaceDuration;
            priority = 0;
        }

        faceCount--;
    }

    private int CalcPainOffset()
    {
        var player = world.Options.Players[world.Options.ConsolePlayer];

        var health = player.Health > 100 ? 100 : player.Health;

        if (health != oldHealth)
        {
            lastPainOffset = Face.Stride *
                             (((100 - health) * Face.PainFaceCount) / 101);
            oldHealth = health;
        }

        return lastPainOffset;
    }


    public static class Face
    {
        public const int PainFaceCount = 5;
        public const int StraightFaceCount = 3;
        private const int TurnFaceCount = 2;
        private const int SpecialFaceCount = 3;

        public const int Stride = StraightFaceCount + TurnFaceCount + SpecialFaceCount;
        private const int ExtraFaceCount = 2;
        public const int FaceCount = Stride * PainFaceCount + ExtraFaceCount;

        public const int TurnOffset = StraightFaceCount;
        public const int OuchOffset = TurnOffset + TurnFaceCount;
        public const int EvilGrinOffset = OuchOffset + 1;
        public const int RampageOffset = EvilGrinOffset + 1;
        public const int GodIndex = PainFaceCount * Stride;
        public const int DeadIndex = GodIndex + 1;

        public const int EvilGrinDuration = (2 * GameConst.TicRate);
        public const int StraightFaceDuration = (GameConst.TicRate / 2);
        public const int TurnDuration = (1 * GameConst.TicRate);
        public const int OuchDuration = (1 * GameConst.TicRate);
        public const int RampageDelay = (2 * GameConst.TicRate);

        public const int MuchPain = 20;
    }
}