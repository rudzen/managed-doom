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

using ManagedDoom.Audio;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class Mobj : Thinker
{
    //
    // NOTES: mobj_t
    //
    // mobj_ts are used to tell the refresh where to draw an image,
    // tell the world simulation when objects are contacted,
    // and tell the sound driver how to position a sound.
    //
    // The refresh uses the next and prev links to follow
    // lists of things in sectors as they are being drawn.
    // The sprite, frame, and angle elements determine which patch_t
    // is used to draw the sprite if it is visible.
    // The sprite and frame values are allmost allways set
    // from state_t structures.
    // The statescr.exe utility generates the states.h and states.c
    // files that contain the sprite/frame numbers from the
    // statescr.txt source file.
    // The xyz origin point represents a point at the bottom middle
    // of the sprite (between the feet of a biped).
    // This is the default origin position for patch_ts grabbed
    // with lumpy.exe.
    // A walking creature will have its z equal to the floor
    // it is standing on.
    //
    // The sound code uses the x,y, and subsector fields
    // to do stereo positioning of any sound effited by the mobj_t.
    //
    // The play simulation uses the blocklinks, x,y,z, radius, height
    // to determine when mobj_ts are touching each other,
    // touching lines in the map, or hit by trace lines (gunshots,
    // lines of sight, etc).
    // The mobj_t->flags element has various bit flags
    // used by the simulation.
    //
    // Every mobj_t is linked into a single sector
    // based on its origin coordinates.
    // The subsector_t is found with R_PointInSubsector(x,y),
    // and the sector_t can be found with subsector->sector.
    // The sector links are only used by the rendering code,
    // the play simulation does not care about them at all.
    //
    // Any mobj_t that needs to be acted upon by something else
    // in the play world (block movement, be shot, etc) will also
    // need to be linked into the blockmap.
    // If the thing has the MF_NOBLOCK flag set, it will not use
    // the block links. It can still interact with other things,
    // but only as the instigator (missiles will run into other
    // things, but nothing can run into a missile).
    // Each block in the grid is 128*128 units, and knows about
    // every line_t that it contains a piece of, and every
    // interactable mobj_t that has its origin contained.  
    //
    // A valid mobj_t is a mobj_t that has the proper subsector_t
    // filled in for its xy coordinates and is linked into the
    // sector from which the subsector was made, or has the
    // MF_NOSECTOR flag set (the subsector_t needs to be valid
    // even if MF_NOSECTOR is set), and is linked into a blockmap
    // block or has the MF_NOBLOCKMAP flag set.
    // Links should only be modified by the P_[Un]SetThingPosition()
    // functions.
    // Do not change the MF_NO? flags while a thing is valid.
    //
    // Any questions?
    //

    public static readonly Fixed OnFloorZ = Fixed.MinValue;
    public static readonly Fixed OnCeilingZ = Fixed.MaxValue;

    // Info for drawing: position.

    // More list: links in sector (if needed).

    // More drawing info: to determine current sprite.

    // Interaction info, by BLOCKMAP.
    // Links in blocks (if needed).

    // The closest interval over all contacted Sectors.

    // For movement checking.

    // Momentums, used to update position.

    // If == validCount, already checked.

    // Movement direction, movement generation (zig-zagging).

    // Thing being chased / attacked (or null),
    // also the originator for missiles.

    // Reaction time: if non 0, don't attack yet.
    // Used by player to freeze a bit after teleporting.

    // If >0, the target will be chased
    // no matter what (even if shot).

    // Additional info record for player avatars only.
    // Only valid if type == MT_PLAYER

    // Player number last looked for.

    // For nightmare respawn.

    // Thing being chased/attacked for tracers.

    // For frame interpolation.
    private bool interpolate;
    private Fixed oldX;
    private Fixed oldY;
    private Fixed oldZ;

    public Mobj(World world)
    {
        this.World = world;
    }

    public World World { get; }

    public Fixed X { get; set; }

    public Fixed Y { get; set; }

    public Fixed Z { get; set; }

    public Mobj? SectorNext { get; set; }

    public Mobj? SectorPrev { get; set; }

    public Angle Angle { get; set; }

    public Sprite Sprite { get; set; }

    public int Frame { get; set; }

    public Mobj? BlockNext { get; set; }

    public Mobj? BlockPrev { get; set; }

    public Subsector Subsector { get; set; }

    public Fixed FloorZ { get; set; }

    public Fixed CeilingZ { get; set; }

    public Fixed Radius { get; set; }

    public Fixed Height { get; set; }

    public Fixed MomX { get; set; }

    public Fixed MomY { get; set; }

    public Fixed MomZ { get; set; }

    public int ValidCount { get; set; }

    public MobjType Type { get; set; }

    public MobjInfo Info { get; set; }

    public int Tics { get; set; }

    public MobjStateDef State { get; set; }

    public MobjFlags Flags { get; set; }

    public int Health { get; set; }

    public Direction MoveDir { get; set; }

    public int MoveCount { get; set; }

    public Mobj? Target { get; set; }

    public int ReactionTime { get; set; }

    public int Threshold { get; set; }

    public Player? Player { get; set; }

    public int LastLook { get; set; }

    public MapThing SpawnPoint { get; set; }

    public Mobj Tracer { get; set; }

    public override void Run()
    {
        // Momentum movement.
        if (MomX != Fixed.Zero || MomY != Fixed.Zero ||
            (Flags & MobjFlags.SkullFly) != 0)
        {
            World.ThingMovement.XYMovement(this);

            if (ThinkerState == ThinkerState.Removed)
            {
                // Mobj was removed.
                return;
            }
        }

        if ((Z != FloorZ) || MomZ != Fixed.Zero)
        {
            World.ThingMovement.ZMovement(this);

            if (ThinkerState == ThinkerState.Removed)
            {
                // Mobj was removed.
                return;
            }
        }

        // Cycle through states,
        // calling action functions at transitions.
        if (Tics != -1)
        {
            Tics--;

            // You can cycle through multiple states in a tic.
            if (Tics == 0)
            {
                // Freed itself.
                if (!SetState(State.Next))
                    return;
            }
        }
        else
        {
            // Check for nightmare respawn.
            if ((Flags & MobjFlags.CountKill) == 0)
                return;

            var options = World.Options;
            if (!(options.Skill == GameSkill.Nightmare || options.RespawnMonsters))
                return;

            MoveCount++;

            if (MoveCount < 12 * 35)
                return;

            if ((World.LevelTime & 31) != 0)
                return;

            if (World.Random.Next() > 4)
                return;

            NightmareRespawn();
        }
    }

    public bool SetState(MobjState state)
    {
        do
        {
            if (state == MobjState.Null)
            {
                this.State = DoomInfo.States[(int)MobjState.Null];
                World.ThingAllocation.RemoveMobj(this);
                return false;
            }

            var st = DoomInfo.States[(int)state];
            this.State = st;
            Tics = GetTics(st);
            Sprite = st.Sprite;
            Frame = st.Frame;

            // Modified handling.
            // Call action functions when the state is set.
            st.MobjAction?.Invoke(World, this);

            state = st.Next;
        } while (Tics == 0);

        return true;
    }

    private int GetTics(MobjStateDef state)
    {
        var options = World.Options;
        if (options.FastMonsters || options.Skill == GameSkill.Nightmare)
        {
            return state.Number is >= (int)MobjState.SargRun1 and <= (int)MobjState.SargPain2
                ? state.Tics >> 1
                : state.Tics;
        }

        return state.Tics;
    }

    private void NightmareRespawn()
    {
        var sp = SpawnPoint ?? MapThing.Empty;

        // Somthing is occupying it's position?
        if (!World.ThingMovement.CheckPosition(this, sp.X, sp.Y))
        {
            // No respwan.
            return;
        }

        var ta = World.ThingAllocation;

        // Spawn a teleport fog at old spot.
        var fog1 = ta.SpawnMobj(
            x: X,
            y: Y,
            z: Subsector.Sector.FloorHeight,
            type: MobjType.Tfog
        );

        // Initiate teleport sound.
        World.StartSound(fog1, Sfx.TELEPT, SfxType.Misc);

        // Spawn a teleport fog at the new spot.
        var ss = Geometry.PointInSubsector(sp.X, sp.Y, World.Map);

        var fog2 = ta.SpawnMobj(
            x: sp.X,
            y: sp.Y,
            z: ss.Sector.FloorHeight,
            type: MobjType.Tfog
        );

        World.StartSound(fog2, Sfx.TELEPT, SfxType.Misc);

        // Spawn the new monster.
        var z = (Info.Flags & MobjFlags.SpawnCeiling) != 0 ? OnCeilingZ : OnFloorZ;

        // Inherit attributes from deceased one.
        var mobj = ta.SpawnMobj(sp.X, sp.Y, z, Type);
        mobj.SpawnPoint = SpawnPoint!;
        mobj.Angle = sp.Angle;

        if ((sp.Flags & ThingFlags.Ambush) != 0)
            mobj.Flags |= MobjFlags.Ambush;

        mobj.ReactionTime = 18;

        // Remove the old monster.
        World.ThingAllocation.RemoveMobj(this);
    }

    public override void UpdateFrameInterpolationInfo()
    {
        interpolate = true;
        oldX = X;
        oldY = Y;
        oldZ = Z;
    }

    public void DisableFrameInterpolationForOneFrame()
    {
        interpolate = false;
    }

    public Fixed GetInterpolatedX(Fixed frameFrac)
    {
        return interpolate ? oldX + frameFrac * (X - oldX) : X;
    }

    public Fixed GetInterpolatedY(Fixed frameFrac)
    {
        return interpolate ? oldY + frameFrac * (Y - oldY) : Y;
    }

    public Fixed GetInterpolatedZ(Fixed frameFrac)
    {
        return interpolate ? oldZ + frameFrac * (Z - oldZ) : Z;
    }
}