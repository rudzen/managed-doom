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
    public WeaponTypes oldWeaponsOwned;

    // Count until face changes.
    public int faceCount;

    public int lastAttackDown;
    public int lastPainOffset;

    // Used for appopriately pained face.
    public int oldHealth;

    public int priority;

    // A random number per tick.
    public int randomNumber;

    public int FaceIndex;
}

public static class StatusBarExtensions
{
    private static readonly DoomRandom statusBarRandom = new();

    extension(StatusBar statusBar)
    {
        public void Reset(WeaponTypes weaponTypes)
        {
            statusBar.oldHealth = -1;
            statusBar.oldWeaponsOwned = weaponTypes;
            statusBar.faceCount = 0;
            statusBar.FaceIndex = 0;
            statusBar.randomNumber = 0;
            statusBar.priority = 0;
            statusBar.lastAttackDown = -1;
            statusBar.lastPainOffset = 0;
        }

        public void Update(Player consolePlayer)
        {
            statusBar.randomNumber = statusBarRandom.Next();
            UpdateFace(statusBar, consolePlayer);
        }

        private void UpdateFace(Player consolePlayer)
        {
            if (statusBar.priority < 10)
            {
                // Dead.
                if (consolePlayer.Health == 0)
                {
                    statusBar.priority = 9;
                    statusBar.FaceIndex = Face.DeadIndex;
                    statusBar.faceCount = 1;
                    return;
                }
            }

            if (statusBar.priority < 9)
            {
                if (consolePlayer.BonusCount != 0)
                {
                    // Picking up bonus.
                    var doEvilGrin = false;

                    if (statusBar.oldWeaponsOwned != consolePlayer.WeaponOwned)
                    {
                        doEvilGrin = true;
                        statusBar.oldWeaponsOwned = consolePlayer.WeaponOwned;
                    }

                    if (doEvilGrin)
                    {
                        // Evil grin if just picked up weapon.
                        statusBar.priority = 8;
                        statusBar.faceCount = Face.EvilGrinDuration;
                        statusBar.FaceIndex = CalcPainOffset(statusBar, consolePlayer.Health) + Face.EvilGrinOffset;
                    }
                }
            }

            if (statusBar.priority < 8)
            {
                if (consolePlayer.DamageCount != 0 &&
                    consolePlayer.Attacker != null &&
                    consolePlayer.Attacker != consolePlayer.Mobj)
                {
                    // Being attacked.
                    statusBar.priority = 7;

                    if (consolePlayer.Health - statusBar.oldHealth > Face.MuchPain)
                    {
                        statusBar.faceCount = Face.OuchDuration;
                        statusBar.FaceIndex = CalcPainOffset(statusBar, consolePlayer.Health) + Face.OuchOffset;
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

                        statusBar.faceCount = Face.TurnDuration;
                        statusBar.FaceIndex = CalcPainOffset(statusBar, consolePlayer.Health);

                        // Head-on.
                        if (diff < Angle.Ang45)
                            statusBar.FaceIndex += Face.RampageOffset;
                        // Turn face right.
                        else if (right)
                            statusBar.FaceIndex += Face.TurnOffset;
                        // Turn face left.
                        else
                            statusBar.FaceIndex += Face.TurnOffset + 1;
                    }
                }
            }

            if (statusBar.priority < 7)
            {
                // Getting hurt because of your own damn stupidity.
                if (consolePlayer.DamageCount != 0)
                {
                    if (consolePlayer.Health - statusBar.oldHealth > Face.MuchPain)
                    {
                        statusBar.priority = 7;
                        statusBar.faceCount = Face.TurnDuration;
                        statusBar.FaceIndex = CalcPainOffset(statusBar, consolePlayer.Health) + Face.OuchOffset;
                    }
                    else
                    {
                        statusBar.priority = 6;
                        statusBar.faceCount = Face.TurnDuration;
                        statusBar.FaceIndex = CalcPainOffset(statusBar, consolePlayer.Health) + Face.RampageOffset;
                    }
                }
            }

            if (statusBar.priority < 6)
            {
                // Rapid firing.
                if (consolePlayer.AttackDown)
                {
                    if (statusBar.lastAttackDown == -1)
                        statusBar.lastAttackDown = Face.RampageDelay;
                    else if (--statusBar.lastAttackDown == 0)
                    {
                        statusBar.priority = 5;
                        statusBar.FaceIndex = CalcPainOffset(statusBar, consolePlayer.Health) + Face.RampageOffset;
                        statusBar.faceCount = 1;
                        statusBar.lastAttackDown = 1;
                    }
                }
                else
                    statusBar.lastAttackDown = -1;
            }

            if (statusBar.priority < 5)
            {
                // Invulnerability.
                if ((consolePlayer.Cheats & CheatFlags.GodMode) != 0 || consolePlayer.Powers[PowerType.Invulnerability] != 0)
                {
                    statusBar.priority = 4;

                    statusBar.FaceIndex = Face.GodIndex;
                    statusBar.faceCount = 1;
                }
            }

            // Look left or look right if the facecount has timed out.
            if (statusBar.faceCount == 0)
            {
                statusBar.FaceIndex = CalcPainOffset(statusBar, consolePlayer.Health) + (statusBar.randomNumber % 3);
                statusBar.faceCount = Face.StraightFaceDuration;
                statusBar.priority = 0;
            }

            statusBar.faceCount--;
        }

        private int CalcPainOffset(int currentHealth)
        {
            var health = currentHealth;

            if (health > 100)
                health = 100;

            if (health != statusBar.oldHealth)
            {
                statusBar.lastPainOffset = Face.Stride * (((100 - health) * Face.PainFaceCount) / 101);
                statusBar.oldHealth = health;
            }

            return statusBar.lastPainOffset;
        }
    }
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