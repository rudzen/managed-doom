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

namespace ManagedDoom.Doom.Map;

public enum LineSpecial
{
    Normal = 0,

    SlidingDoorUpAndDown = 124,

    VerticalDoorManual = 1,
    BlueLockedDoorManual = 26,
    YellowLockedDoorManual = 27,
    RedLockedDoorManual = 28,

    DoorOpenManual = 31,
    BlueLockedDoorOpenManual = 32,
    RedLockedDoorOpenManual = 33,
    YellowLockedDoorOpenManual = 34,

    BlazingDoorRaiseManual = 117,
    BlazingDoorOpenManual = 118,

    // Switches
    BuildStairsSwitch = 7,
    ChangeDonutSwitch = 9,
    ExitLevelSwitch = 11,
    RaiseFloor32AndChangeTextureSwitch = 14,
    RaiseFloor24AndChangeTextureSwitch = 15,

    RaiseFloorToNextHighestFloorSwitch = 18,
    RaisePlatformNextHighestFloorAndChangeTextureSwitch = 20,
    PlatformDownWaitUpAndStaySwitch = 21,
    LowerFloorToLowestSwitch = 23,
    RaiseDoorSwitch = 29,

    LowerCeilingToFloorSwitch = 41,

    TurboLowerFloorSwitch = 71,

    CeilingCrushAndRaiseSwitch = 49,
    CloseDoorSwitch = 50,
    SecretExitSwitch = 51,
    RaiseFloorCrushSwitch = 55,

    RaiseFloorSwitch = 101,
    LowerFloorToSurroundingFloorHeightSwitch = 102,
    OpenDoorSwitch = 103,

    BlazingDoorRaiseFastSwitch = 111,
    BlazingDoorOpenFastSwitch = 112,
    BlazingDoorCloseFastSwitch = 113,

    BlazingPlatformDownWaitUpAndStaySwitch = 122,
    BuildStairsTurbo16Switch = 127,
    RaiseFloorTurboSwitch = 131,

    BlazingOpenDoorBlueSwitch = 133,
    BlazingDoorOpenRedSwitch = 135,
    BlazingDoorOpenYellowSwitch = 137,

    RaiseFloor512Switch = 140,

    // Buttons
    CloseDoorButton = 42,
    LowerCeilingToFloorButton = 43,
    LowerFloorToSurroundingFloorHeightButton = 45,
    LowerFloorToLowestButton = 60,
    OpenDoorButton = 61,
    PlatformDownWaitUpAndStayButton = 62,
    RaiseDoorButton = 63,
    RaiseFloorToCeilingButton = 64,
    RaiseFloor24AndChangeTextureButton = 66,
    RaiseFloor32AndChangeTextureButton = 67,
    RaiseFloorCrushButton = 65,
    RaisePlatformToNextHighestFloorAndChangeTextureButton = 68,
    RaiseFloorToNextHighestFloorButton = 69,
    TurboLowerFloorButton = 70,
    BlazingDoorRaiseButton = 114,
    BlazingDoorOpenButton = 115,
    BlazingDoorCloseButton = 116,
    BlazingPlatformDownWaitUpAndStayButton = 123,
    RaiseFloorTurboButton = 132,
    BlazingOpenDoorBlueButton = 99,
    BlazingOpenDoorRedButton = 134,
    BlazingOpenDoorYellowButton = 136,
    LightTurnOnButton = 138,
    LightTurnOffButton = 139,

    // Triggers
    OpenDoorTrigger = 2,
    CloseDoorTrigger = 3,
    RaiseDoorTrigger = 4,
    RaiseFloorTrigger = 5,
    FastCeilingCrushAndRaiseTrigger = 6,
    BuildStairsTrigger = 8,
    PlatformDownWaitUpAndStayTrigger = 10,
    LightTurnOnBrightestNearTrigger = 12,
    LightTurnOn255Trigger = 13,
    CloseDoor30Trigger = 16,
    StartLightStrobingTrigger = 17,
    LowerFloorTrigger = 19,
    RaiseFloorToNearestHeightAndChangeTextureTrigger = 22,
    CeilingCrushAndRaiseTrigger = 25,
    RaiseFloorToShortestTextureHeightOnEitherSideOfLinesTrigger = 30,
    LightsVeryDarkTrigger = 35,
    LowerFloorTurboTrigger = 36,
    LowerFloorAndChangeTrigger = 37,
    LowerFloorToLowestTrigger = 38,
    DoTeleportTrigger = 39,
    RaiseCeilingAndLowerFloorTrigger = 40,
    CeilingCrushTrigger = 44,
    DoExitTrigger = 52,
    PerpetualPlatformRaiseTrigger = 53,
    PlatformStopTrigger = 54,
    RaiseFloorCrushTrigger = 56,
    CeilingCrushStopTrigger = 57,
    RaiseFloor24Trigger = 58,
    RaiseFloor24AndChangeTrigger = 59,
    TurnLightsOffInSectorTagTrigger = 104,
    BlazingDoorRaiseTrigger = 108,
    BlazingDoorOpenTrigger = 109,
    BuildStairsTurbo16Trigger = 100,
    BlazingDoorCloseTrigger = 110,
    RaiseFloorToNearestSurroundingFloorTrigger = 119,
    BlazingPlatformDownWaitUpAndStayTrigger = 121,
    SecretExitTrigger = 124,
    TeleportMonsterOnlyTrigger = 125,
    RaiseFloorTurboTrigger = 130,
    SilentCeilingCrushAndRaiseTrigger = 141,

    // Re-Triggers
    CeilingCrushReTrigger = 72,
    CeilingCrushAndRaiseReTrigger = 73,
    CeilingCrushStopReTrigger = 74,
    CloseDoorReTrigger = 75,
    CloseDoor30ReTrigger = 76,
    FastCeilingCrushAndRaiseReTrigger = 77,
    LightsVeryDarkReTrigger = 79,
    LightTurnOnBrightestNearReTrigger = 80,
    LightTurnOn255ReTrigger = 81,
    LowerFloorToLowestReTrigger = 82,
    LowerFloorReTrigger = 83,
    LowerAndChangeReTrigger = 84,
    OpenDoorReTrigger = 86,
    PerpetualPlatformRaiseReTrigger = 87,
    PlatformDownWaitUpAndStayReTrigger = 88,
    PlatformStopReTrigger = 89,
    RaiseDoorReTrigger = 90,
    RaiseFloorReTrigger = 91,
    RaiseFloor24ReTrigger = 92,
    RaiseFloor24AndChangeReTrigger = 93,
    RaiseFloorCrushReTrigger = 94,
    RaiseFloorToNearestHeightAndChangeTextureReTrigger = 95,
    RaiseFloorToTheShortestTextureHeightOnEitherSideOfLinesReTrigger = 96,
    DoTeleportReTrigger = 97,
    LowerFloorTurboReTrigger = 98,
    BlazingDoorRaiseReTrigger = 105,
    BlazingDoorOpenReTrigger = 106,
    BlazingDoorCloseReTrigger = 107,
    BlazingPlatformDownWaitUpAndStayReTrigger = 120,
    TeleportMonsterOnlyReTrigger = 126,
    RaiseToNearestFloorReTrigger = 128,
    RaiseFloorTurboReTrigger = 129,
    
    // Shoots special line
    RaiseFloorShoot = 24,
    OpenDoorImpactShoot = 46,
    RaiseFloorNearAndChangeShoot = 47,
    
    // Other
    TextureScroll = 48
}