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
using System.Linq;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class ThingMovement(World world)
{
    ////////////////////////////////////////////////////////////
    // General thing movement
    ////////////////////////////////////////////////////////////

    public static readonly Fixed FloatSpeed = Fixed.FromInt(4);

    private const int MaxSpecialCrossCount = 64;
    private static readonly Fixed maxMove = Fixed.FromInt(30);
    private static readonly Fixed gravity = Fixed.One;

    private Mobj? currentThing;
    private MobjFlags currentFlags;
    private Fixed currentX;
    private Fixed currentY;
    private readonly Fixed[] currentBox = new Fixed[4];

    private LineDef? currentCeilingLine;

    public int crossedSpecialCount;
    public readonly LineDef[]? crossedSpecials = new LineDef[MaxSpecialCrossCount];

    /// <summary>
    /// Links a thing into both a block and a subsector based on
    /// its x and y. Sets thing.Subsector properly.
    /// </summary>
    public void SetThingPosition(Mobj thing)
    {
        var map = world.Map;

        var subSector = Geometry.PointInSubsector(thing.X, thing.Y, map);

        thing.Subsector = subSector;

        // Invisible things don't go into the sector links.
        if ((thing.Flags & MobjFlags.NoSector) == 0)
        {
            var sector = subSector.Sector;
            sector.ThingList.Insert(0, thing); // Insert at head to maintain original linked-list order
        }

        // Inert things don't need to be in blockmap.
        if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
        {
            var index = map.BlockMap.GetIndex(thing.X, thing.Y);

            if (index != -1)
                map.BlockMap.ThingLists[index].Insert(0, thing); // Insert at head to maintain original linked-list order
        }
    }

    /// <summary>
    /// Unlinks a thing from block map and sectors.
    /// On each position change, BLOCKMAP and other lookups
    /// maintaining lists ot things inside these structures
    /// need to be updated.
    /// </summary>
    public void UnsetThingPosition(Mobj thing)
    {
        var map = world.Map;

        // Invisible things don't go into the sector links.
        // Remove from sector list
        if ((thing.Flags & MobjFlags.NoSector) == 0)
            thing.Subsector.Sector.ThingList.Remove(thing);

        // Inert things don't need to be in blockmap.
        if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
        {
            var index = map.BlockMap.GetIndex(thing.X, thing.Y);

            if (index != -1)
                map.BlockMap.ThingLists[index].Remove(thing);
        }
    }


    /// <summary>
    /// Adjusts currentFloorZ and currentCeilingZ as lines are contacted.
    /// </summary>
    private bool CheckLine(LineDef line)
    {
        var mc = world.MapCollision;

        if (currentBox[Box.Right] <= line.BoundingBox[Box.Left] ||
            currentBox[Box.Left] >= line.BoundingBox[Box.Right] ||
            currentBox[Box.Top] <= line.BoundingBox[Box.Bottom] ||
            currentBox[Box.Bottom] >= line.BoundingBox[Box.Top])
        {
            return true;
        }

        if (Geometry.BoxOnLineSide(currentBox, line) != -1)
            return true;

        // A line has been hit.
        //
        // The moving thing's destination position will cross the given line.
        // If this should not be allowed, return false.
        // If the line is special, keep track of it to process later if the move is proven ok.
        //
        // NOTE:
        //     specials are NOT sorted by order, so two special lines that are only 8 pixels
        //     apart could be crossed in either order.

        // One-sided line.
        if (line.BackSector == null)
            return false;

        if ((currentThing!.Flags & MobjFlags.Missile) == 0)
        {
            // Explicitly blocking everything.
            if ((line.Flags & LineFlags.Blocking) != 0)
                return false;

            // Block monsters only.
            if (currentThing.Player == null && (line.Flags & LineFlags.BlockMonsters) != 0)
                return false;
        }

        // Set openrange, opentop, openbottom.
        mc.LineOpening(line);

        // Adjust floor / ceiling heights.
        if (mc.OpenTop < CurrentCeilingZ)
        {
            CurrentCeilingZ = mc.OpenTop;
            currentCeilingLine = line;
        }

        if (mc.OpenBottom > CurrentFloorZ)
            CurrentFloorZ = mc.OpenBottom;

        if (mc.LowFloor < CurrentDropoffZ)
            CurrentDropoffZ = mc.LowFloor;

        // If contacted a special line, add it to the list.
        if (line.Special != 0)
        {
            crossedSpecials![crossedSpecialCount] = line;
            crossedSpecialCount++;
        }

        return true;
    }

    private bool CheckThing(Mobj thing)
    {
        if ((thing.Flags & (MobjFlags.Solid | MobjFlags.Special | MobjFlags.Shootable)) == 0)
            return true;

        var blockDist = thing.Radius + currentThing!.Radius;

        // Didn't hit it.
        if (Fixed.Abs(thing.X - currentX) >= blockDist || Fixed.Abs(thing.Y - currentY) >= blockDist)
            return true;

        // Don't clip against self.
        if (thing == currentThing)
            return true;

        // Check for skulls slamming into things.
        if ((currentThing.Flags & MobjFlags.SkullFly) != 0)
        {
            var damage = ((world.Random.Next() % 8) + 1) * currentThing.Info.Damage;

            world.ThingInteraction.DamageMobj(thing, currentThing, currentThing, damage);

            currentThing.Flags &= ~MobjFlags.SkullFly;
            currentThing.MomX = currentThing.MomY = currentThing.MomZ = Fixed.Zero;

            currentThing.SetState(currentThing.Info.SpawnState);

            // Stop moving.
            return false;
        }

        // Missiles can hit other things.
        if ((currentThing.Flags & MobjFlags.Missile) != 0)
        {
            // See if it went over / under.
            
            // Overhead.
            if (currentThing.Z > thing.Z + thing.Height)
                return true;

            // Underneath.
            if (currentThing.Z + currentThing.Height < thing.Z)
                return true;

            if (currentThing.Target != null &&
                (currentThing.Target.Type == thing.Type ||
                 (currentThing.Target.Type == MobjType.Knight && thing.Type == MobjType.Bruiser) ||
                 (currentThing.Target.Type == MobjType.Bruiser && thing.Type == MobjType.Knight)))
            {
                // Don't hit same species as originator.
                if (thing == currentThing.Target)
                    return true;

                // Explode, but do no damage.
                // Let players missile other players.
                if (thing.Type != MobjType.Player && !DoomInfo.DeHackEdConst.MonstersInfight)
                    return false;
            }

            // Didn't do any damage.
            if ((thing.Flags & MobjFlags.Shootable) == 0)
                return (thing.Flags & MobjFlags.Solid) == 0;

            // Damage / explode.
            var damage = ((world.Random.Next() % 8) + 1) * currentThing.Info.Damage;
            world.ThingInteraction.DamageMobj(thing, currentThing, currentThing.Target, damage);

            // Don't traverse anymore.
            return false;
        }

        // Check for special pickup.
        if ((thing.Flags & MobjFlags.Special) != 0)
        {
            var solid = (thing.Flags & MobjFlags.Solid) != 0;
            if ((currentFlags & MobjFlags.PickUp) != 0)
            {
                // Can remove thing.
                world.ItemPickup.TouchSpecialThing(thing, currentThing);
            }

            return !solid;
        }

        return (thing.Flags & MobjFlags.Solid) == 0;
    }

    /// <summary>
    /// This is purely informative, nothing is modified
    /// (except things picked up).
    ///
    /// In:
    ///     A Mobj (can be valid or invalid)
    ///     A position to be checked
    ///     (doesn't need to be related to the mobj.X and Y)
    ///
    /// During:
    ///     Special things are touched if MobjFlags.PickUp
    ///     Early out on solid lines?
    ///
    /// Out:
    ///     New subsector
    ///     CurrentFloorZ
    ///     CurrentCeilingZ
    ///     CurrentDropoffZ
    ///     The lowest point contacted
    ///     (monsters won't move to a dropoff)
    ///     crossedSpecials[]
    ///     crossedSpecialCount
    /// </summary>
    public bool CheckPosition(Mobj thing, Fixed x, Fixed y)
    {
        var map = world.Map;
        var bm = map.BlockMap;

        currentThing = thing;
        currentFlags = thing.Flags;

        currentX = x;
        currentY = y;

        currentBox[Box.Top] = y + currentThing.Radius;
        currentBox[Box.Bottom] = y - currentThing.Radius;
        currentBox[Box.Right] = x + currentThing.Radius;
        currentBox[Box.Left] = x - currentThing.Radius;

        var newSubsector = Geometry.PointInSubsector(x, y, map);

        currentCeilingLine = null;

        // The base floor / ceiling is from the subsector that contains the point.
        // Any contacted lines the step closer together will adjust them.
        CurrentFloorZ = CurrentDropoffZ = newSubsector.Sector.FloorHeight;
        CurrentCeilingZ = newSubsector.Sector.CeilingHeight;

        var validCount = world.GetNewValidCount();

        crossedSpecialCount = 0;

        if ((currentFlags & MobjFlags.NoClip) != 0)
            return true;

        // Check things first, possibly picking things up.
        // The bounding box is extended by MaxThingRadius because mobj_ts are grouped into
        // mapblocks based on their origin point, and can overlap into adjacent blocks by up
        // to MaxThingRadius units.
        {
            var blockX1 = bm.GetBlockX(currentBox[Box.Left] - GameConst.MaxThingRadius);
            var blockX2 = bm.GetBlockX(currentBox[Box.Right] + GameConst.MaxThingRadius);
            var blockY1 = bm.GetBlockY(currentBox[Box.Bottom] - GameConst.MaxThingRadius);
            var blockY2 = bm.GetBlockY(currentBox[Box.Top] + GameConst.MaxThingRadius);

            for (var bx = blockX1; bx <= blockX2; bx++)
            {
                for (var by = blockY1; by <= blockY2; by++)
                {
                    // Early out.
                    if (!bm.IterateThings(bx, by, CheckThing))
                        return false;
                }
            }
        }

        // Check lines.
        {
            var blockX1 = bm.GetBlockX(currentBox[Box.Left]);
            var blockX2 = bm.GetBlockX(currentBox[Box.Right]);
            var blockY1 = bm.GetBlockY(currentBox[Box.Bottom]);
            var blockY2 = bm.GetBlockY(currentBox[Box.Top]);

            for (var bx = blockX1; bx <= blockX2; bx++)
            {
                for (var by = blockY1; by <= blockY2; by++)
                {
                    var lines = bm.IterateLines(bx, by, validCount);
                    // Early out.
                    if (lines.Any(lineDef => !CheckLine(lineDef)))
                        return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Attempt to move to a new position, crossing special lines unless
    /// MobjFlags.Teleport is set.
    /// </summary>
    public bool TryMove(Mobj thing, Fixed x, Fixed y)
    {
        FloatOk = false;

        // Solid wall or thing.
        if (!CheckPosition(thing, x, y))
            return false;

        if ((thing.Flags & MobjFlags.NoClip) == 0)
        {
            // Doesn't fit.
            if (CurrentCeilingZ - CurrentFloorZ < thing.Height)
                return false;

            FloatOk = true;

            // Mobj must lower itself to fit.
            if ((thing.Flags & MobjFlags.Teleport) == 0 && CurrentCeilingZ - thing.Z < thing.Height)
                return false;

            // Too big a step-up.
            if ((thing.Flags & MobjFlags.Teleport) == 0 && CurrentFloorZ - thing.Z > Fixed.FromInt(24))
                return false;

            // Don't stand over a drop-off.
            if ((thing.Flags & (MobjFlags.DropOff | MobjFlags.Float)) == 0 && CurrentFloorZ - CurrentDropoffZ > Fixed.FromInt(24))
                return false;
        }

        // The move is ok,
        // so link the thing into its new position.
        UnsetThingPosition(thing);

        var oldX = thing.X;
        var oldY = thing.Y;
        thing.FloorZ = CurrentFloorZ;
        thing.CeilingZ = CurrentCeilingZ;
        thing.X = x;
        thing.Y = y;

        SetThingPosition(thing);

        // If any special lines were hit, do the effect.
        var specialLineHit = (thing.Flags & (MobjFlags.Teleport | MobjFlags.NoClip)) == 0;

        if (!specialLineHit)
            return true;

        while (crossedSpecialCount-- > 0)
        {
            // See if the line was crossed.
            var line = crossedSpecials![crossedSpecialCount];
            var newSide = Geometry.PointOnLineSide(thing.X, thing.Y, line);
            var oldSide = Geometry.PointOnLineSide(oldX, oldY, line);
            if (newSide != oldSide)
            {
                if (line.Special != 0)
                    world.MapInteraction.CrossSpecialLine(line, oldSide, thing);
            }
        }

        return true;
    }


    private static readonly Fixed stopSpeed = new(0x1000);
    private static readonly Fixed friction = new(0xe800);

    public void XYMovement(Mobj thing)
    {
        if (thing.MomX == Fixed.Zero && thing.MomY == Fixed.Zero)
        {
            if ((thing.Flags & MobjFlags.SkullFly) != 0)
            {
                // The skull slammed into something.
                thing.Flags &= ~MobjFlags.SkullFly;
                thing.MomX = thing.MomY = thing.MomZ = Fixed.Zero;

                thing.SetState(thing.Info.SpawnState);
            }

            return;
        }

        var player = thing.Player;

        if (thing.MomX > maxMove)
            thing.MomX = maxMove;
        else if (thing.MomX < -maxMove)
            thing.MomX = -maxMove;

        if (thing.MomY > maxMove)
            thing.MomY = maxMove;
        else if (thing.MomY < -maxMove)
            thing.MomY = -maxMove;

        var moveX = thing.MomX;
        var moveY = thing.MomY;

        do
        {
            Fixed pMoveX;
            Fixed pMoveY;

            if (moveX > maxMove / 2 || moveY > maxMove / 2)
            {
                pMoveX = thing.X + moveX / 2;
                pMoveY = thing.Y + moveY / 2;
                moveX >>= 1;
                moveY >>= 1;
            }
            else
            {
                pMoveX = thing.X + moveX;
                pMoveY = thing.Y + moveY;
                moveX = moveY = Fixed.Zero;
            }

            if (!TryMove(thing, pMoveX, pMoveY))
            {
                // Blocked move.
                if (thing.Player != null)
                {
                    // Try to slide along it.
                    SlideMove(thing);
                }
                else if ((thing.Flags & MobjFlags.Missile) != 0)
                {
                    // Explode a missile.
                    if (currentCeilingLine is { BackSector: not null } &&
                        currentCeilingLine.BackSector.CeilingFlat == world.Map.SkyFlatNumber)
                    {
                        // Hack to prevent missiles exploding against the sky.
                        // Does not handle sky floors.
                        world.ThingAllocation.RemoveMobj(thing);
                        return;
                    }

                    world.ThingInteraction.ExplodeMissile(thing);
                }
                else
                    thing.MomX = thing.MomY = Fixed.Zero;
            }
        } while (moveX != Fixed.Zero || moveY != Fixed.Zero);

        // Slow down.
        if (player != null && (player.Cheats & CheatFlags.NoMomentum) != 0)
        {
            // Debug option for no sliding at all.
            thing.MomX = thing.MomY = Fixed.Zero;
            return;
        }

        // No friction for missiles ever.
        if ((thing.Flags & (MobjFlags.Missile | MobjFlags.SkullFly)) != 0)
            return;

        // No friction when airborne.
        if (thing.Z > thing.FloorZ)
            return;

        if ((thing.Flags & MobjFlags.Corpse) != 0)
        {
            // Do not stop sliding if halfway off a step with some momentum.
            if (thing.MomX > Fixed.One / 4 ||
                thing.MomX < -Fixed.One / 4 ||
                thing.MomY > Fixed.One / 4 ||
                thing.MomY < -Fixed.One / 4)
            {
                if (thing.FloorZ != thing.Subsector.Sector.FloorHeight)
                    return;
            }
        }

        if (thing.MomX > -stopSpeed &&
            thing.MomX < stopSpeed &&
            thing.MomY > -stopSpeed &&
            thing.MomY < stopSpeed &&
            (player == null || player.Command is { ForwardMove: 0, SideMove: 0 }))
        {
            // If in a walking frame, stop moving.
            if (player != null && (player.Mobj!.State.Number - (int)MobjState.PlayRun1) < 4)
                player.Mobj.SetState(MobjState.Play);

            thing.MomX = Fixed.Zero;
            thing.MomY = Fixed.Zero;
        }
        else
        {
            thing.MomX *= friction;
            thing.MomY *= friction;
        }
    }

    public void ZMovement(Mobj thing)
    {
        // Check for smooth step up.
        if (thing.Player != null && thing.Z < thing.FloorZ)
        {
            thing.Player.ViewHeight -= thing.FloorZ - thing.Z;

            thing.Player.DeltaViewHeight =
                (Player.NormalViewHeight - thing.Player.ViewHeight) >> 3;
        }

        // Adjust height.
        thing.Z += thing.MomZ;

        if ((thing.Flags & MobjFlags.Float) != 0 && thing.Target != null)
        {
            // Float down towards target if too close.
            if ((thing.Flags & MobjFlags.SkullFly) == 0 &&
                (thing.Flags & MobjFlags.InFloat) == 0)
            {
                var dist = Geometry.AproxDistance(
                    thing.X - thing.Target.X,
                    thing.Y - thing.Target.Y);

                var delta = (thing.Target.Z + (thing.Height >> 1)) - thing.Z;

                if (delta < Fixed.Zero && dist < -(delta * 3))
                    thing.Z -= FloatSpeed;
                else if (delta > Fixed.Zero && dist < (delta * 3))
                    thing.Z += FloatSpeed;
            }
        }

        // Clip movement.
        if (thing.Z <= thing.FloorZ)
        {
            // Hit the floor.

            //
            // The lost soul bounce fix below is based on Chocolate Doom's implementation.
            //

            var correctLostSoulBounce = world.Options.GameVersion >= GameVersion.Ultimate;

            // The skull slammed into something.
            if (correctLostSoulBounce && (thing.Flags & MobjFlags.SkullFly) != 0)
                thing.MomZ = -thing.MomZ;

            if (thing.MomZ < Fixed.Zero)
            {
                if (thing.Player != null && thing.MomZ < -gravity * 8)
                {
                    // Squat down.
                    // Decrease view height for a moment after hitting the ground (hard),
                    // and utter appropriate sound.
                    thing.Player.DeltaViewHeight = (thing.MomZ >> 3);
                    world.StartSound(thing, Sfx.OOF, SfxType.Voice);
                }

                thing.MomZ = Fixed.Zero;
            }

            thing.Z = thing.FloorZ;

            if (!correctLostSoulBounce && (thing.Flags & MobjFlags.SkullFly) != 0)
                thing.MomZ = -thing.MomZ;

            if ((thing.Flags & MobjFlags.Missile) != 0 &&
                (thing.Flags & MobjFlags.NoClip) == 0)
            {
                world.ThingInteraction.ExplodeMissile(thing);
                return;
            }
        }
        else if ((thing.Flags & MobjFlags.NoGravity) == 0)
        {
            if (thing.MomZ == Fixed.Zero)
                thing.MomZ = -gravity * 2;
            else
                thing.MomZ -= gravity;
        }

        if (thing.Z + thing.Height > thing.CeilingZ)
        {
            // Hit the ceiling.
            if (thing.MomZ > Fixed.Zero)
                thing.MomZ = Fixed.Zero;

            thing.Z = thing.CeilingZ - thing.Height;

            // The skull slammed into something.
            if ((thing.Flags & MobjFlags.SkullFly) != 0)
                thing.MomZ = -thing.MomZ;

            if ((thing.Flags & MobjFlags.Missile) != 0 && (thing.Flags & MobjFlags.NoClip) == 0)
                world.ThingInteraction.ExplodeMissile(thing);
        }
    }


    public Fixed CurrentFloorZ { get; private set; }

    public Fixed CurrentCeilingZ { get; private set; }

    public bool FloatOk { get; private set; }
    private Fixed CurrentDropoffZ { get; set; }


    ////////////////////////////////////////////////////////////
    // Player's slide movement
    ////////////////////////////////////////////////////////////

    private Fixed bestSlideFrac;
    private LineDef bestSlideLine;

    private Mobj slideThing;
    private Fixed slideMoveX;
    private Fixed slideMoveY;

    /// <summary>
    /// Adjusts the x and y movement so that the next move will
    /// slide along the wall.
    /// </summary>
    private void HitSlideLine(LineDef line)
    {
        if (line.SlopeType == SlopeType.Horizontal)
        {
            slideMoveY = Fixed.Zero;
            return;
        }

        if (line.SlopeType == SlopeType.Vertical)
        {
            slideMoveX = Fixed.Zero;
            return;
        }

        var side = Geometry.PointOnLineSide(slideThing.X, slideThing.Y, line);

        var lineAngle = Geometry.PointToAngle(Fixed.Zero, Fixed.Zero, line.Dx, line.Dy);
        if (side == 1)
            lineAngle += Angle.Ang180;

        var moveAngle = Geometry.PointToAngle(Fixed.Zero, Fixed.Zero, slideMoveX, slideMoveY);

        var deltaAngle = moveAngle - lineAngle;
        if (deltaAngle > Angle.Ang180)
            deltaAngle += Angle.Ang180;

        var moveDist = Geometry.AproxDistance(slideMoveX, slideMoveY);
        var newDist = moveDist * Trig.Cos(deltaAngle);

        slideMoveX = newDist * Trig.Cos(lineAngle);
        slideMoveY = newDist * Trig.Sin(lineAngle);
    }

    private bool SlideTraverse(Intercept intercept)
    {
        var mc = world.MapCollision;

        if (intercept.Line == null)
        {
            throw new Exception("ThingMovement.SlideTraverse: Not a line?");
        }

        var line = intercept.Line;

        if ((line.Flags & LineFlags.TwoSided) == 0)
        {
            // Don't hit the back side.
            if (Geometry.PointOnLineSide(slideThing.X, slideThing.Y, line) != 0)
                return true;

            goto isBlocking;
        }

        // Set openrange, opentop, openbottom.
        mc.LineOpening(line);

        // Doesn't fit.
        if (mc.OpenRange < slideThing.Height)
            goto isBlocking;

        // Mobj is too high.
        if (mc.OpenTop - slideThing.Z < slideThing.Height)
            goto isBlocking;

        // Too big a step-up.
        if (mc.OpenBottom - slideThing.Z > Fixed.FromInt(24))
            goto isBlocking;

        // This line doesn't block movement.
        return true;

        // The line does block movement, see if it is closer than best so far.
    isBlocking:
        if (intercept.Frac < bestSlideFrac)
        {
            bestSlideFrac = intercept.Frac;
            bestSlideLine = line;
        }

        // Stop.
        return false;
    }

    /// <summary>
    /// The MomX / MomY move is bad, so try to slide along a wall.
    /// Find the first line hit, move flush to it, and slide along it.
    /// This is a kludgy mess.
    /// </summary>
    private void SlideMove(Mobj thing)
    {
        var pt = world.PathTraversal;

        slideThing = thing;

        var hitCount = 0;

    retry:
        // Don't loop forever.
        if (++hitCount == 3)
        {
            // The move most have hit the middle, so stairstep.
            StairStep(thing);
            return;
        }

        Fixed leadX;
        Fixed leadY;
        Fixed trailX;
        Fixed trailY;

        // Trace along the three leading corners.
        if (thing.MomX > Fixed.Zero)
        {
            leadX = thing.X + thing.Radius;
            trailX = thing.X - thing.Radius;
        }
        else
        {
            leadX = thing.X - thing.Radius;
            trailX = thing.X + thing.Radius;
        }

        if (thing.MomY > Fixed.Zero)
        {
            leadY = thing.Y + thing.Radius;
            trailY = thing.Y - thing.Radius;
        }
        else
        {
            leadY = thing.Y - thing.Radius;
            trailY = thing.Y + thing.Radius;
        }

        bestSlideFrac = Fixed.OnePlusEpsilon;

        pt.PathTraverse(
            leadX, leadY, leadX + thing.MomX, leadY + thing.MomY,
            PathTraverseFlags.AddLines, SlideTraverse);

        pt.PathTraverse(
            trailX, leadY, trailX + thing.MomX, leadY + thing.MomY,
            PathTraverseFlags.AddLines, SlideTraverse);

        pt.PathTraverse(
            leadX, trailY, leadX + thing.MomX, trailY + thing.MomY,
            PathTraverseFlags.AddLines, SlideTraverse);

        // Move up to the wall.
        if (bestSlideFrac == Fixed.OnePlusEpsilon)
        {
            // The move most have hit the middle, so stairstep.
            StairStep(thing);
            return;
        }

        // Fudge a bit to make sure it doesn't hit.
        bestSlideFrac = new Fixed(bestSlideFrac.Data - 0x800);
        if (bestSlideFrac > Fixed.Zero)
        {
            var newX = thing.MomX * bestSlideFrac;
            var newY = thing.MomY * bestSlideFrac;

            if (!TryMove(thing, thing.X + newX, thing.Y + newY))
            {
                // The move most have hit the middle, so stairstep.
                StairStep(thing);
                return;
            }
        }

        // Now continue along the wall.
        // First calculate remainder.
        bestSlideFrac = new Fixed(Fixed.FracUnit - (bestSlideFrac.Data + 0x800));

        if (bestSlideFrac > Fixed.One)
            bestSlideFrac = Fixed.One;

        if (bestSlideFrac <= Fixed.Zero)
            return;

        slideMoveX = thing.MomX * bestSlideFrac;
        slideMoveY = thing.MomY * bestSlideFrac;

        // Clip the moves.
        HitSlideLine(bestSlideLine);

        thing.MomX = slideMoveX;
        thing.MomY = slideMoveY;

        if (!TryMove(thing, thing.X + slideMoveX, thing.Y + slideMoveY))
            goto retry;
    }

    private void StairStep(Mobj thing)
    {
        if (!TryMove(thing, thing.X, thing.Y + thing.MomY))
            TryMove(thing, thing.X + thing.MomX, thing.Y);
    }


    ////////////////////////////////////////////////////////////
    // Teleport movement
    ////////////////////////////////////////////////////////////

    private bool StompThing(Mobj thing)
    {
        if ((thing.Flags & MobjFlags.Shootable) == 0)
            return true;

        var blockDist = thing.Radius + currentThing!.Radius;
        var dx = Fixed.Abs(thing.X - currentX);
        var dy = Fixed.Abs(thing.Y - currentY);
        
        // Didn't hit it.
        if (dx >= blockDist || dy >= blockDist)
            return true;

        // Don't clip against self.
        if (thing == currentThing)
            return true;

        // Monsters don't stomp things except on boss level.
        if (currentThing.Player == null && world.Options.Map != 30)
            return false;

        world.ThingInteraction.DamageMobj(thing, currentThing, currentThing, 10000);

        return true;
    }

    public bool TeleportMove(Mobj thing, Fixed x, Fixed y)
    {
        // Kill anything occupying the position.
        currentThing = thing;
        currentFlags = thing.Flags;

        currentX = x;
        currentY = y;

        currentBox![Box.Top] = y + currentThing.Radius;
        currentBox[Box.Bottom] = y - currentThing.Radius;
        currentBox[Box.Right] = x + currentThing.Radius;
        currentBox[Box.Left] = x - currentThing.Radius;

        var ss = Geometry.PointInSubsector(x, y, world.Map);

        currentCeilingLine = null;

        // The base floor / ceiling is from the subsector that contains the point.
        // Any contacted lines the step closer together will adjust them.
        CurrentFloorZ = CurrentDropoffZ = ss.Sector.FloorHeight;
        CurrentCeilingZ = ss.Sector.CeilingHeight;

        crossedSpecialCount = 0;

        // Stomp on any things contacted.
        var bm = world.Map.BlockMap;
        var blockX1 = bm.GetBlockX(currentBox[Box.Left] - GameConst.MaxThingRadius);
        var blockX2 = bm.GetBlockX(currentBox[Box.Right] + GameConst.MaxThingRadius);
        var blockY1 = bm.GetBlockY(currentBox[Box.Bottom] - GameConst.MaxThingRadius);
        var blockY2 = bm.GetBlockY(currentBox[Box.Top] + GameConst.MaxThingRadius);

        for (var bx = blockX1; bx <= blockX2; bx++)
        {
            for (var by = blockY1; by <= blockY2; by++)
            {
                if (!bm.IterateThings(bx, by, StompThing))
                    return false;
            }
        }

        // the move is ok, so link the thing into its new position
        UnsetThingPosition(thing);

        thing.FloorZ = CurrentFloorZ;
        thing.CeilingZ = CurrentCeilingZ;
        thing.X = x;
        thing.Y = y;

        SetThingPosition(thing);

        return true;
    }
}