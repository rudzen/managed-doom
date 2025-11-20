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

using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class StatusBar
{
    // Used for evil grin.
    private WeaponTypes oldWeaponsOwned;

    private readonly DoomRandom random;

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

    public StatusBar(Player consolePlayer)
    {
        oldHealth = -1;
        oldWeaponsOwned = consolePlayer.WeaponOwned;
        faceCount = 0;
        FaceIndex = 0;
        randomNumber = 0;
        priority = 0;
        lastAttackDown = -1;
        lastPainOffset = 0;

        random = new DoomRandom();
    }

    public int FaceIndex { get; private set; }

    public void Reset(Player consolePlayer)
    {
        oldHealth = -1;
        oldWeaponsOwned = consolePlayer.WeaponOwned;
        faceCount = 0;
        FaceIndex = 0;
        randomNumber = 0;
        priority = 0;
        lastAttackDown = -1;
        lastPainOffset = 0;
    }

    public void Update(Player consolePlayer)
    {
        randomNumber = random.Next();
        UpdateFace(consolePlayer);
    }

    private void UpdateFace(Player consolePlayer)
    {
        if (priority < 10)
        {
            // Dead.
            if (consolePlayer.Health == 0)
            {
                priority = 9;
                FaceIndex = Face.DeadIndex;
                faceCount = 1;
                return;
            }
        }

        if (priority < 9)
        {
            if (consolePlayer.BonusCount != 0)
            {
                // Picking up bonus.
                var doEvilGrin = false;

                if (oldWeaponsOwned != consolePlayer.WeaponOwned)
                {
                    doEvilGrin = true;
                    oldWeaponsOwned = consolePlayer.WeaponOwned;
                }

                if (doEvilGrin)
                {
                    // Evil grin if just picked up weapon.
                    priority = 8;
                    faceCount = Face.EvilGrinDuration;
                    FaceIndex = CalcPainOffset(consolePlayer.Health) + Face.EvilGrinOffset;
                }
            }
        }

        if (priority < 8)
        {
            if (consolePlayer.DamageCount != 0 &&
                consolePlayer.Attacker != null &&
                consolePlayer.Attacker != consolePlayer.Mobj)
            {
                // Being attacked.
                priority = 7;

                if (consolePlayer.Health - oldHealth > Face.MuchPain)
                {
                    faceCount = Face.OuchDuration;
                    FaceIndex = CalcPainOffset(consolePlayer.Health) + Face.OuchOffset;
                }
                else
                {
                    var attackerAngle = Geometry.PointToAngle(
                        consolePlayer.Mobj!.X, consolePlayer.Mobj.Y,
                        consolePlayer.Attacker.X, consolePlayer.Attacker.Y);

                    Angle diff;
                    bool right;
                    if (attackerAngle > consolePlayer.Mobj.Angle)
                    {
                        // Whether right or left.
                        diff = attackerAngle - consolePlayer.Mobj.Angle;
                        right = diff > Angle.Ang180;
                    }
                    else
                    {
                        // Whether left or right.
                        diff = consolePlayer.Mobj.Angle - attackerAngle;
                        right = diff <= Angle.Ang180;
                    }

                    faceCount = Face.TurnDuration;
                    FaceIndex = CalcPainOffset(consolePlayer.Health);

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
            if (consolePlayer.DamageCount != 0)
            {
                if (consolePlayer.Health - oldHealth > Face.MuchPain)
                {
                    priority = 7;
                    faceCount = Face.TurnDuration;
                    FaceIndex = CalcPainOffset(consolePlayer.Health) + Face.OuchOffset;
                }
                else
                {
                    priority = 6;
                    faceCount = Face.TurnDuration;
                    FaceIndex = CalcPainOffset(consolePlayer.Health) + Face.RampageOffset;
                }
            }
        }

        if (priority < 6)
        {
            // Rapid firing.
            if (consolePlayer.AttackDown)
            {
                if (lastAttackDown == -1)
                    lastAttackDown = Face.RampageDelay;
                else if (--lastAttackDown == 0)
                {
                    priority = 5;
                    FaceIndex = CalcPainOffset(consolePlayer.Health) + Face.RampageOffset;
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
            if ((consolePlayer.Cheats & CheatFlags.GodMode) != 0 || consolePlayer.Powers[PowerType.Invulnerability] != 0)
            {
                priority = 4;

                FaceIndex = Face.GodIndex;
                faceCount = 1;
            }
        }

        // Look left or look right if the facecount has timed out.
        if (faceCount == 0)
        {
            FaceIndex = CalcPainOffset(consolePlayer.Health) + (randomNumber % 3);
            faceCount = Face.StraightFaceDuration;
            priority = 0;
        }

        faceCount--;
    }

    private int CalcPainOffset(int currentHealth)
    {
        var health = currentHealth > 100 ? 100 : currentHealth;

        if (health != oldHealth)
        {
            lastPainOffset = Face.Stride * (((100 - health) * Face.PainFaceCount) / 101);
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