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
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class MapInteraction
{
    private static readonly Fixed useRange = Fixed.FromInt(64);

    private readonly World world;

    public MapInteraction(World world)
    {
        this.world = world;
    }

    #region Line use

    private Mobj useThing;

    private bool UseTraverse(Intercept intercept)
    {
        var mc = world.MapCollision;
        var line = intercept.Line!;

        if (line.Special == 0)
        {
            mc.LineOpening(line);
            if (mc.OpenRange <= Fixed.Zero)
            {
                world.StartSound(useThing, Sfx.NOWAY, SfxType.Voice);

                // Can't use through a wall.
                return false;
            }

            // Not a special line, but keep checking.
            return true;
        }

        var side = Geometry.PointOnLineSide(useThing.X, useThing.Y, line) == 1 ? 1 : 0;

        UseSpecialLine(useThing, line, side);

        // Can't use for than one special line in a row.
        return false;
    }

    /// <summary>
    /// Looks for special lines in front of the player to activate.
    /// </summary>
    public void UseLines(Player player)
    {
        var pt = world.PathTraversal;

        useThing = player.Mobj!;

        var angle = useThing.Angle;

        var x1 = useThing.X;
        var y1 = useThing.Y;
        var x2 = x1 + useRange.ToIntFloor() * Trig.Cos(angle);
        var y2 = y1 + useRange.ToIntFloor() * Trig.Sin(angle);

        pt.PathTraverse(x1, y1, x2, y2, PathTraverseFlags.AddLines, UseTraverse);
    }

    /// <summary>
    /// Called when a thing uses a special line.
    /// Only the front sides of lines are usable.
    /// </summary>
    public bool UseSpecialLine(Mobj thing, LineDef line, int side)
    {
        var specials = world.Specials;
        var sa = world.SectorAction;

        // Err...
        // Use the back sides of VERY SPECIAL lines...
        // Sliding door open and close (unused).
        if (side != 0 && line.Special != LineSpecial.SlidingDoorUpAndDown)
            return false;

        // Switches that other things can activate.
        if (thing.Player is null)
        {
            // Never open secret doors.
            if ((line.Flags & LineFlags.Secret) != 0)
                return false;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            if (line.Special is not (LineSpecial.VerticalDoorManual
                or LineSpecial.BlueLockedDoorOpenManual
                or LineSpecial.RedLockedDoorOpenManual
                or LineSpecial.YellowLockedDoorOpenManual))
            {
                return false;
            }
        }

        // Do something.
        // MANUALS
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (line.Special is LineSpecial.VerticalDoorManual
            or LineSpecial.BlueLockedDoorManual
            or LineSpecial.YellowLockedDoorManual
            or LineSpecial.RedLockedDoorManual
            or LineSpecial.DoorOpenManual
            or LineSpecial.BlueLockedDoorOpenManual
            or LineSpecial.RedLockedDoorOpenManual
            or LineSpecial.YellowLockedDoorOpenManual
            or LineSpecial.BlazingDoorRaiseManual
            or LineSpecial.BlazingDoorOpenManual)
        {
            sa.DoLocalDoor(line, thing);
        }
        // SWITCHES
        else if (line.Special == LineSpecial.BuildStairsSwitch)
        {
            if (sa.BuildStairs(line, StairType.Build8))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.ChangeDonutSwitch)
        {
            if (sa.DoDonut(line))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.ExitLevelSwitch)
        {
            specials.ChangeSwitchTexture(line, false);
            world.ExitLevel();
        }
        else if (line.Special == LineSpecial.RaiseFloor32AndChangeTextureSwitch)
        {
            if (sa.DoPlatform(line, PlatformType.RaiseAndChange, 32))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.RaiseFloor24AndChangeTextureSwitch)
        {
            if (sa.DoPlatform(line, PlatformType.RaiseAndChange, 24))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.RaiseFloorToNextHighestFloorSwitch)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloorToNearest))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.RaisePlatformNextHighestFloorAndChangeTextureSwitch)
        {
            if (sa.DoPlatform(line, PlatformType.RaiseToNearestAndChange, 0))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.PlatformDownWaitUpAndStaySwitch)
        {
            if (sa.DoPlatform(line, PlatformType.DownWaitUpStay, 0))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.LowerFloorToLowestSwitch)
        {
            if (sa.DoFloor(line, FloorMoveType.LowerFloorToLowest))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.RaiseDoorSwitch)
        {
            if (sa.DoDoor(line, VerticalDoorType.Normal))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.LowerCeilingToFloorSwitch)
        {
            if (sa.DoCeiling(line, CeilingMoveType.LowerToFloor))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.TurboLowerFloorSwitch)
        {
            if (sa.DoFloor(line, FloorMoveType.TurboLower))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.CeilingCrushAndRaiseSwitch)
        {
            if (sa.DoCeiling(line, CeilingMoveType.CrushAndRaise))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.CloseDoorSwitch)
        {
            if (sa.DoDoor(line, VerticalDoorType.Close))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.SecretExitSwitch)
        {
            specials.ChangeSwitchTexture(line, false);
            world.SecretExitLevel();
        }
        else if (line.Special == LineSpecial.RaiseFloorCrushSwitch)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloorCrush))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.RaiseFloorSwitch)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloor))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.LowerFloorToSurroundingFloorHeightSwitch)
        {
            if (sa.DoFloor(line, FloorMoveType.LowerFloor))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.OpenDoorSwitch)
        {
            if (sa.DoDoor(line, VerticalDoorType.Open))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.BlazingDoorRaiseFastSwitch)
        {
            if (sa.DoDoor(line, VerticalDoorType.BlazeRaise))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.BlazingDoorOpenFastSwitch)
        {
            if (sa.DoDoor(line, VerticalDoorType.BlazeOpen))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.BlazingDoorCloseFastSwitch)
        {
            // Blazing door close (faster than turbo).
            if (sa.DoDoor(line, VerticalDoorType.BlazeClose))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.BlazingPlatformDownWaitUpAndStaySwitch)
        {
            if (sa.DoPlatform(line, PlatformType.BlazeDwus, 0))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.BuildStairsTurbo16Switch)
        {
            if (sa.BuildStairs(line, StairType.Turbo16))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.RaiseFloorTurboSwitch)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloorTurbo))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special is LineSpecial.BlazingOpenDoorBlueSwitch or LineSpecial.BlazingDoorOpenRedSwitch or LineSpecial.BlazingDoorOpenYellowSwitch)
        {
            if (sa.DoLockedDoor(line, VerticalDoorType.BlazeOpen, thing))
                specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.RaiseFloor512Switch)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloor512))
                specials.ChangeSwitchTexture(line, false);
        }
        // BUTTONS
        else if (line.Special == LineSpecial.CloseDoorButton)
        {
            if (sa.DoDoor(line, VerticalDoorType.Close))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.LowerCeilingToFloorButton)
        {
            if (sa.DoCeiling(line, CeilingMoveType.LowerToFloor))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.LowerFloorToSurroundingFloorHeightButton)
        {
            if (sa.DoFloor(line, FloorMoveType.LowerFloor))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.LowerFloorToLowestButton)
        {
            if (sa.DoFloor(line, FloorMoveType.LowerFloorToLowest))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.OpenDoorButton)
        {
            if (sa.DoDoor(line, VerticalDoorType.Open))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.PlatformDownWaitUpAndStayButton)
        {
            if (sa.DoPlatform(line, PlatformType.DownWaitUpStay, 1))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseDoorButton)
        {
            if (sa.DoDoor(line, VerticalDoorType.Normal))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseFloorToCeilingButton)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloor))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseFloor24AndChangeTextureButton)
        {
            if (sa.DoPlatform(line, PlatformType.RaiseAndChange, 24))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseFloor32AndChangeTextureButton)
        {
            if (sa.DoPlatform(line, PlatformType.RaiseAndChange, 32))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseFloorCrushButton)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloorCrush))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaisePlatformToNextHighestFloorAndChangeTextureButton)
        {
            if (sa.DoPlatform(line, PlatformType.RaiseToNearestAndChange, 0))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseFloorToNextHighestFloorButton)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloorToNearest))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.TurboLowerFloorButton)
        {
            if (sa.DoFloor(line, FloorMoveType.TurboLower))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.BlazingDoorRaiseButton)
        {
            if (sa.DoDoor(line, VerticalDoorType.BlazeRaise))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.BlazingDoorOpenButton)
        {
            if (sa.DoDoor(line, VerticalDoorType.BlazeOpen))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.BlazingDoorCloseButton)
        {
            if (sa.DoDoor(line, VerticalDoorType.BlazeClose))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.BlazingPlatformDownWaitUpAndStayButton)
        {
            if (sa.DoPlatform(line, PlatformType.BlazeDwus, 0))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseFloorTurboButton)
        {
            if (sa.DoFloor(line, FloorMoveType.RaiseFloorTurbo))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special is LineSpecial.BlazingOpenDoorBlueButton or LineSpecial.BlazingOpenDoorRedButton or LineSpecial.BlazingOpenDoorYellowButton)
        {
            if (sa.DoLockedDoor(line, VerticalDoorType.BlazeOpen, thing))
                specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.LightTurnOnButton)
        {
            sa.LightTurnOn(line, 255);
            specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.LightTurnOffButton)
        {
            sa.LightTurnOn(line, 35);
            specials.ChangeSwitchTexture(line, true);
        }

        return true;
    }

    #endregion Line use

    #region Line crossing

    /// <summary>
    /// Called every time a thing origin is about to cross a line
    /// with a non-zero special.
    /// </summary>
    public void CrossSpecialLine(LineDef line, int side, Mobj thing)
    {
        //	Triggers that other things can activate.
        if (thing.Player == null)
        {
            // Things that should NOT trigger specials...
            switch (thing.Type)
            {
                case MobjType.Rocket:
                case MobjType.Plasma:
                case MobjType.Bfg:
                case MobjType.Troopshot:
                case MobjType.Headshot:
                case MobjType.Bruisershot:
                    return;
            }

            var ok = false;
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (line.Special)
            {
                case LineSpecial.DoTeleportTrigger:
                case LineSpecial.DoTeleportReTrigger:
                case LineSpecial.TeleportMonsterOnlyTrigger:
                case LineSpecial.TeleportMonsterOnlyReTrigger:
                case LineSpecial.RaiseDoorTrigger:
                case LineSpecial.PlatformDownWaitUpAndStayTrigger:
                case LineSpecial.PlatformDownWaitUpAndStayReTrigger:
                    ok = true;
                    break;
            }

            if (!ok)
                return;
        }

        var sa = world.SectorAction;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (line.Special)
        {
            // TRIGGERS.
            // All from here to RETRIGGERS.
            case LineSpecial.OpenDoorTrigger:
                sa.DoDoor(line, VerticalDoorType.Open);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.CloseDoorTrigger:
                sa.DoDoor(line, VerticalDoorType.Close);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseDoorTrigger:
                sa.DoDoor(line, VerticalDoorType.Normal);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseFloorTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloor);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.FastCeilingCrushAndRaiseTrigger:
                sa.DoCeiling(line, CeilingMoveType.FastCrushAndRaise);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.BuildStairsTrigger:
                sa.BuildStairs(line, StairType.Build8);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.PlatformDownWaitUpAndStayTrigger:
                sa.DoPlatform(line, PlatformType.DownWaitUpStay, 0);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.LightTurnOnBrightestNearTrigger:
                sa.LightTurnOn(line, 0);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.LightTurnOn255Trigger:
                sa.LightTurnOn(line, byte.MaxValue);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.CloseDoor30Trigger:
                sa.DoDoor(line, VerticalDoorType.Close30ThenOpen);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.StartLightStrobingTrigger:
                sa.StartLightStrobing(line);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.LowerFloorTrigger:
                sa.DoFloor(line, FloorMoveType.LowerFloor);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseFloorToNearestHeightAndChangeTextureTrigger:
                sa.DoPlatform(line, PlatformType.RaiseToNearestAndChange, 0);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.CeilingCrushAndRaiseTrigger:
                sa.DoCeiling(line, CeilingMoveType.CrushAndRaise);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseFloorToShortestTextureHeightOnEitherSideOfLinesTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseToTexture);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.LightsVeryDarkTrigger:
                sa.LightTurnOn(line, 35);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.LowerFloorTurboTrigger:
                sa.DoFloor(line, FloorMoveType.TurboLower);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.LowerFloorAndChangeTrigger:
                sa.DoFloor(line, FloorMoveType.LowerAndChange);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.LowerFloorToLowestTrigger:
                sa.DoFloor(line, FloorMoveType.LowerFloorToLowest);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.DoTeleportTrigger:
                sa.Teleport(line, side, thing);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseCeilingAndLowerFloorTrigger:
                sa.DoCeiling(line, CeilingMoveType.RaiseToHighest);
                sa.DoFloor(line, FloorMoveType.LowerFloorToLowest);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.CeilingCrushTrigger:
                sa.DoCeiling(line, CeilingMoveType.LowerAndCrush);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.DoExitTrigger:
                world.ExitLevel();
                break;

            case LineSpecial.PerpetualPlatformRaiseTrigger:
                sa.DoPlatform(line, PlatformType.PerpetualRaise, 0);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.PlatformStopTrigger:
                sa.StopPlatform(line);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseFloorCrushTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloorCrush);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.CeilingCrushStopTrigger:
                sa.CeilingCrushStop(line);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseFloor24Trigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloor24);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseFloor24AndChangeTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloor24AndChange);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.TurnLightsOffInSectorTagTrigger:
                sa.TurnTagLightsOff(line);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.BlazingDoorRaiseTrigger:
                sa.DoDoor(line, VerticalDoorType.BlazeRaise);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.BlazingDoorOpenTrigger:
                sa.DoDoor(line, VerticalDoorType.BlazeOpen);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.BuildStairsTurbo16Trigger:
                sa.BuildStairs(line, StairType.Turbo16);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.BlazingDoorCloseTrigger:
                sa.DoDoor(line, VerticalDoorType.BlazeClose);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.RaiseFloorToNearestSurroundingFloorTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloorToNearest);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.BlazingPlatformDownWaitUpAndStayTrigger:
                sa.DoPlatform(line, PlatformType.BlazeDwus, 0);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.SecretExitTrigger:
                world.SecretExitLevel();
                break;

            case LineSpecial.TeleportMonsterOnlyTrigger:
                if (thing.Player is not null)
                    break;

                sa.Teleport(line, side, thing);
                line.Special = LineSpecial.Normal;

                break;

            case LineSpecial.RaiseFloorTurboTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloorTurbo);
                line.Special = LineSpecial.Normal;
                break;

            case LineSpecial.SilentCeilingCrushAndRaiseTrigger:
                sa.DoCeiling(line, CeilingMoveType.SilentCrushAndRaise);
                line.Special = LineSpecial.Normal;
                break;

            // RETRIGGERS. All from here till end.
            case LineSpecial.CeilingCrushReTrigger:
                sa.DoCeiling(line, CeilingMoveType.LowerAndCrush);
                break;

            case LineSpecial.CeilingCrushAndRaiseReTrigger:
                sa.DoCeiling(line, CeilingMoveType.CrushAndRaise);
                break;

            case LineSpecial.CeilingCrushStopReTrigger:
                sa.CeilingCrushStop(line);
                break;

            case LineSpecial.CloseDoorReTrigger:
                sa.DoDoor(line, VerticalDoorType.Close);
                break;

            case LineSpecial.CloseDoor30ReTrigger:
                sa.DoDoor(line, VerticalDoorType.Close30ThenOpen);
                break;

            case LineSpecial.FastCeilingCrushAndRaiseReTrigger:
                sa.DoCeiling(line, CeilingMoveType.FastCrushAndRaise);
                break;

            case LineSpecial.LightsVeryDarkReTrigger:
                sa.LightTurnOn(line, 35);
                break;

            case LineSpecial.LightTurnOnBrightestNearReTrigger:
                sa.LightTurnOn(line, 0);
                break;

            case LineSpecial.LightTurnOn255ReTrigger:
                sa.LightTurnOn(line, 255);
                break;

            case LineSpecial.LowerFloorToLowestReTrigger:
                sa.DoFloor(line, FloorMoveType.LowerFloorToLowest);
                break;

            case LineSpecial.LowerFloorReTrigger:
                sa.DoFloor(line, FloorMoveType.LowerFloor);
                break;

            case LineSpecial.LowerAndChangeReTrigger:
                sa.DoFloor(line, FloorMoveType.LowerAndChange);
                break;

            case LineSpecial.OpenDoorReTrigger:
                sa.DoDoor(line, VerticalDoorType.Open);
                break;

            case LineSpecial.PerpetualPlatformRaiseReTrigger:
                sa.DoPlatform(line, PlatformType.PerpetualRaise, 0);
                break;

            case LineSpecial.PlatformDownWaitUpAndStayReTrigger:
                sa.DoPlatform(line, PlatformType.DownWaitUpStay, 0);
                break;

            case LineSpecial.PlatformStopReTrigger:
                sa.StopPlatform(line);
                break;

            case LineSpecial.RaiseDoorReTrigger:
                sa.DoDoor(line, VerticalDoorType.Normal);
                break;

            case LineSpecial.RaiseFloorReTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloor);
                break;

            case LineSpecial.RaiseFloor24ReTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloor24);
                break;

            case LineSpecial.RaiseFloor24AndChangeReTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloor24AndChange);
                break;

            case LineSpecial.RaiseFloorCrushReTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloorCrush);
                break;

            case LineSpecial.RaiseFloorToNearestHeightAndChangeTextureReTrigger:
                sa.DoPlatform(line, PlatformType.RaiseToNearestAndChange, 0);
                break;

            case LineSpecial.RaiseFloorToTheShortestTextureHeightOnEitherSideOfLinesReTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseToTexture);
                break;

            case LineSpecial.DoTeleportReTrigger:
                sa.Teleport(line, side, thing);
                break;

            case LineSpecial.LowerFloorTurboReTrigger:
                sa.DoFloor(line, FloorMoveType.TurboLower);
                break;

            case LineSpecial.BlazingDoorRaiseReTrigger:
                sa.DoDoor(line, VerticalDoorType.BlazeRaise);
                break;

            case LineSpecial.BlazingDoorOpenReTrigger:
                sa.DoDoor(line, VerticalDoorType.BlazeOpen);
                break;

            case LineSpecial.BlazingDoorCloseReTrigger:
                sa.DoDoor(line, VerticalDoorType.BlazeClose);
                break;

            case LineSpecial.BlazingPlatformDownWaitUpAndStayReTrigger:
                sa.DoPlatform(line, PlatformType.BlazeDwus, 0);
                break;

            case LineSpecial.TeleportMonsterOnlyReTrigger:
                if (thing.Player is null)
                    sa.Teleport(line, side, thing);

                break;

            case LineSpecial.RaiseToNearestFloorReTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloorToNearest);
                break;

            case LineSpecial.RaiseFloorTurboReTrigger:
                sa.DoFloor(line, FloorMoveType.RaiseFloorTurbo);
                break;
        }
    }

    #endregion Line crossing

    #region Line shoot

    /// <summary>
    /// Called when a thing shoots a special line.
    /// </summary>
    public void ShootSpecialLine(Mobj thing, LineDef line)
    {
        //	Impacts that other things can activate.
        if (thing.Player is null && line.Special != LineSpecial.OpenDoorImpactShoot)
            return;

        if (line.Special == LineSpecial.RaiseFloorShoot)
        {
            world.SectorAction.DoFloor(line, FloorMoveType.RaiseFloor);
            world.Specials.ChangeSwitchTexture(line, false);
        }
        else if (line.Special == LineSpecial.OpenDoorImpactShoot)
        {
            world.SectorAction.DoDoor(line, VerticalDoorType.Open);
            world.Specials.ChangeSwitchTexture(line, true);
        }
        else if (line.Special == LineSpecial.RaiseFloorNearAndChangeShoot)
        {
            world.SectorAction.DoPlatform(line, PlatformType.RaiseToNearestAndChange, 0);
            world.Specials.ChangeSwitchTexture(line, false);
        }
    }

    #endregion Line shoot
}