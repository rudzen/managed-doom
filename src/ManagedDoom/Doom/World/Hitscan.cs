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
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class Hitscan
{
    private readonly World world;
    private Fixed currentAimSlope;
    private int currentDamage;

    private Fixed currentRange;

    private Fixed bottomSlope;
    private Fixed topSlope;

    // Who got hit (or null).

    private Mobj currentShooter;
    private Fixed currentShooterZ;

    public Hitscan(World world)
    {
        this.world = world;
    }

    public Mobj? LineTarget { get; private set; }

    // Slopes to top and bottom of target.

    /// <summary>
    /// Find a thing or wall which is on the aiming line.
    /// Sets lineTaget and aimSlope when a target is aimed at.
    /// </summary>
    private bool AimTraverse(Intercept intercept)
    {
        if (intercept.Line is not null)
        {
            var line = intercept.Line;

            // Stop.
            if ((line.Flags & LineFlags.TwoSided) == 0)
                return false;

            var mc = world.MapCollision;

            // Crosses a two-sided line.
            // A two-sided line will restrict the possible target ranges.
            mc.LineOpening(line);

            // Stop.
            if (mc.OpenBottom >= mc.OpenTop)
                return false;

            var dist = currentRange * intercept.Frac;

            // The null check of the backsector below is necessary to avoid crash
            // in certain PWADs, which contain two-sided lines with no backsector.
            // These are imported from Chocolate Doom.

            if (line.BackSector == null || line.FrontSector.FloorHeight != line.BackSector.FloorHeight)
            {
                var slope = (mc.OpenBottom - currentShooterZ) / dist;
                if (slope > bottomSlope)
                    bottomSlope = slope;
            }

            if (line.BackSector == null || line.FrontSector.CeilingHeight != line.BackSector.CeilingHeight)
            {
                var slope = (mc.OpenTop - currentShooterZ) / dist;
                if (slope < topSlope)
                    topSlope = slope;
            }

            // Stop if top is lower or equal to bottom, else shot continues.
            return topSlope > bottomSlope;
        }

        // Shoot a thing.
        var thing = intercept.Thing;
        // Can't shoot self.
        if (thing == currentShooter)
            return true;

        {
            // Corpse or something.
            if ((thing!.Flags & MobjFlags.Shootable) == 0)
                return true;

            // Check angles to see if the thing can be aimed at.
            var dist = currentRange * intercept.Frac;
            var thingTopSlope = (thing.Z + thing.Height - currentShooterZ) / dist;

            // Shot over the thing.
            if (thingTopSlope < bottomSlope)
                return true;

            var thingBottomSlope = (thing.Z - currentShooterZ) / dist;

            // Shot under the thing.
            if (thingBottomSlope > topSlope)
                return true;

            // This thing can be hit!
            if (thingTopSlope > topSlope)
                thingTopSlope = topSlope;

            if (thingBottomSlope < bottomSlope)
                thingBottomSlope = bottomSlope;

            currentAimSlope = (thingTopSlope + thingBottomSlope) / 2;
            LineTarget = thing;

            // Don't go any farther.
            return false;
        }
    }

    /// <summary>
    /// Fire a hitscan bullet along the aiming line.
    /// </summary>
    private bool ShootTraverse(Intercept intercept)
    {
        var mi = world.MapInteraction;
        var pt = world.PathTraversal;

        if (intercept.Line != null)
        {
            var line = intercept.Line;

            if (line.Special != 0)
                mi.ShootSpecialLine(currentShooter, line);

            if ((line.Flags & LineFlags.TwoSided) == 0)
                goto hitLine;

            var mc = world.MapCollision;

            // Crosses a two-sided line.
            mc.LineOpening(line);

            var dist = currentRange * intercept.Frac;

            // Similar to AimTraverse, the code below is imported from Chocolate Doom.
            if (line.BackSector == null)
            {
                {
                    var slope = (mc.OpenBottom - currentShooterZ) / dist;
                    if (slope > currentAimSlope)
                        goto hitLine;
                }

                {
                    var slope = (mc.OpenTop - currentShooterZ) / dist;
                    if (slope < currentAimSlope)
                        goto hitLine;
                }
            }
            else
            {
                if (line.FrontSector.FloorHeight != line.BackSector.FloorHeight)
                {
                    var slope = (mc.OpenBottom - currentShooterZ) / dist;
                    if (slope > currentAimSlope)
                        goto hitLine;
                }

                if (line.FrontSector.CeilingHeight != line.BackSector.CeilingHeight)
                {
                    var slope = (mc.OpenTop - currentShooterZ) / dist;
                    if (slope < currentAimSlope)
                        goto hitLine;
                }
            }

            // Shot continues.
            return true;

            // Hit line.
        hitLine:

            // Position a bit closer.
            var frac = intercept.Frac - Fixed.FromInt(4) / currentRange;
            var x = pt.Trace.X + pt.Trace.Dx * frac;
            var y = pt.Trace.Y + pt.Trace.Dy * frac;
            var z = currentShooterZ + currentAimSlope * (frac * currentRange);

            if (line.FrontSector.CeilingFlat == world.Map.SkyFlatNumber)
            {
                // Don't shoot the sky!
                if (z > line.FrontSector.CeilingHeight)
                    return false;

                // It's a sky hack wall.
                if (line.BackSector != null && line.BackSector.CeilingFlat == world.Map.SkyFlatNumber)
                    return false;
            }

            // Spawn bullet puffs.
            SpawnPuff(x, y, z);

            // Don't go any farther.
            return false;
        }

        {
            // Shoot a thing.
            var thing = intercept.Thing;
            
            // Can't shoot self.
            if (thing == currentShooter)
                return true;

            // Corpse or something.
            if ((thing!.Flags & MobjFlags.Shootable) == 0)
                return true;

            // Check angles to see if the thing can be aimed at.
            var dist = currentRange * intercept.Frac;
            var thingTopSlope = (thing.Z + thing.Height - currentShooterZ) / dist;

            // Shot over the thing.
            if (thingTopSlope < currentAimSlope)
                return true;

            var thingBottomSlope = (thing.Z - currentShooterZ) / dist;

            // Shot under the thing.
            if (thingBottomSlope > currentAimSlope)
                return true;

            // Hit thing.
            // Position a bit closer.
            var frac = intercept.Frac - Fixed.FromInt(10) / currentRange;

            var x = pt.Trace.X + pt.Trace.Dx * frac;
            var y = pt.Trace.Y + pt.Trace.Dy * frac;
            var z = currentShooterZ + currentAimSlope * (frac * currentRange);

            // Spawn bullet puffs or blod spots, depending on target type.
            if ((intercept.Thing!.Flags & MobjFlags.NoBlood) != 0)
                SpawnPuff(x, y, z);
            else
                SpawnBlood(x, y, z, currentDamage);

            if (currentDamage != 0)
                world.ThingInteraction.DamageMobj(thing, currentShooter, currentShooter, currentDamage);

            // Don't go any farther.
            return false;
        }
    }

    /// <summary>
    /// Find a target on the aiming line.
    /// Sets LineTaget when a target is aimed at.
    /// </summary>
    public Fixed AimLineAttack(Mobj shooter, Angle angle, Fixed range)
    {
        shooter = world.SubstNullMobj(shooter);

        currentShooter = shooter;
        currentShooterZ = shooter.Z + (shooter.Height >> 1) + Fixed.FromInt(8);
        currentRange = range;

        var targetX = shooter.X + range.ToIntFloor() * Trig.Cos(angle);
        var targetY = shooter.Y + range.ToIntFloor() * Trig.Sin(angle);

        // Can't shoot outside view angles.
        topSlope = Fixed.FromInt(100) / 160;
        bottomSlope = Fixed.FromInt(-100) / 160;

        LineTarget = null;

        world.PathTraversal.PathTraverse(
            shooter.X, shooter.Y,
            targetX, targetY,
            PathTraverseFlags.AddLines | PathTraverseFlags.AddThings,
            AimTraverse);

        return LineTarget is not null ? currentAimSlope : Fixed.Zero;
    }

    /// <summary>
    /// Fire a hitscan bullet.
    /// If damage == 0, it is just a test trace that will leave linetarget set.
    /// </summary>
    public void LineAttack(Mobj shooter, Angle angle, Fixed range, Fixed slope, int damage)
    {
        currentShooter = shooter;
        currentShooterZ = shooter.Z + (shooter.Height >> 1) + Fixed.FromInt(8);
        currentRange = range;
        currentAimSlope = slope;
        currentDamage = damage;

        var targetX = shooter.X + range.ToIntFloor() * Trig.Cos(angle);
        var targetY = shooter.Y + range.ToIntFloor() * Trig.Sin(angle);

        world.PathTraversal.PathTraverse(
            shooter.X, shooter.Y,
            targetX, targetY,
            PathTraverseFlags.AddLines | PathTraverseFlags.AddThings,
            ShootTraverse);
    }

    /// <summary>
    /// Spawn a bullet puff.
    /// </summary>
    public void SpawnPuff(Fixed x, Fixed y, Fixed z)
    {
        var random = world.Random;

        z += new Fixed((random.Next() - random.Next()) << 10);

        var thing = world.ThingAllocation.SpawnMobj(x, y, z, MobjType.Puff);
        thing.MomZ = Fixed.One;
        thing.Tics -= random.Next() & 3;

        if (thing.Tics < 1)
            thing.Tics = 1;

        // Don't make punches spark on the wall.
        if (currentRange == WeaponBehavior.MeleeRange)
            thing.SetState(MobjState.Puff3);
    }

    /// <summary>
    /// Spawn blood.
    /// </summary>
    private void SpawnBlood(Fixed x, Fixed y, Fixed z, int damage)
    {
        var random = world.Random;

        z += new Fixed((random.Next() - random.Next()) << 10);

        var thing = world.ThingAllocation.SpawnMobj(x, y, z, MobjType.Blood);
        thing.MomZ = Fixed.FromInt(2);
        thing.Tics -= random.Next() & 3;

        if (thing.Tics < 1)
            thing.Tics = 1;

        if (damage is <= 12 and >= 9)
            thing.SetState(MobjState.Blood2);
        else if (damage < 9)
            thing.SetState(MobjState.Blood3);
    }
}