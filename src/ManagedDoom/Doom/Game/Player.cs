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
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Game;

public sealed class Player
{
    public const int MaxPlayerCount = 4;

    public static readonly Fixed NormalViewHeight = Fixed.FromInt(41);

    private static readonly string[] defaultPlayerNames =
    [
        "Green",
        "Indigo",
        "Brown",
        "Red"
    ];

    // Determine POV, including viewpoint bobbing during movement.
    // Focal origin above mobj.Z.

    // Base height above floor for viewz.

    // Bob / squat speed.

    // Bounded / scaled total momentum.

    // This is only used between levels,
    // mobj.Health is used during levels.

    // Armor type is 0-2.

    // Power ups. invinc and invis are tic counters.

    // Frags, kills of other players.

    // Is WeanponType.NoChange if not changing.

    // True if button down last tic.

    // Bit flags, for cheats and debug.

    // Refired shots are less accurate.

    // For intermission stats.

    // Hint messages.

    // For screen flashing (red or bright).

    // Who did damage (null for floors / ceilings).

    // So gun flashes light up areas.

    // Current PLAYPAL, ???
    // can be set to REDCOLORMAP for pain, etc.

    // Player skin colorshift,
    // 0-3 for which color to draw player.

    // Overlay view sprites (gun, etc).

    // True if secret level has been done.

    // For frame interpolation.
    private bool interpolate;
    private Fixed oldViewZ;
    private Angle oldAngle;

    public Player(int number)
    {
        this.Number = number;

        Name = defaultPlayerNames[number];

        Cmd = new TicCmd();

        Powers = new int[PowerType.Count];
        Cards = CardType.None;

        Frags = new int[MaxPlayerCount];

        Ammo = new int[AmmoType.Count];
        MaxAmmo = new int[AmmoType.Count];

        PlayerSprites = [new PlayerSpriteDef(), new PlayerSpriteDef()];
    }

    public int Number { get; }

    public string Name { get; }

    public bool InGame { get; set; }

    public Mobj? Mobj { get; set; }

    public PlayerState PlayerState { get; set; }

    public TicCmd Cmd { get; }

    public Fixed ViewZ { get; set; }

    public Fixed ViewHeight { get; set; }

    public Fixed DeltaViewHeight { get; set; }

    public Fixed Bob { get; set; }

    public int Health { get; set; }

    public int ArmorPoints { get; set; }

    public int ArmorType { get; set; }

    public int[] Powers { get; }

    public CardType Cards { get; set; }

    public bool Backpack { get; set; }

    public int[] Frags { get; }

    public WeaponTypes ReadyWeapon { get; set; }

    public WeaponTypes PendingWeapon { get; set; }

    public WeaponTypes WeaponOwned { get; set; }

    public int[] Ammo { get; }

    public int[] MaxAmmo { get; }

    public bool AttackDown { get; set; }

    public bool UseDown { get; set; }

    public CheatFlags Cheats { get; set; }

    public int Refire { get; set; }

    public int KillCount { get; set; }

    public int ItemCount { get; set; }

    public int SecretCount { get; set; }

    public string? Message { get; set; }

    public int MessageTime { get; set; }

    public int DamageCount { get; set; }

    public int BonusCount { get; set; }

    public Mobj? Attacker { get; set; }

    public int ExtraLight { get; set; }

    public int FixedColorMap { get; set; }

    public int ColorMap { get; set; }

    public PlayerSpriteDef[] PlayerSprites { get; }

    public bool DidSecret { get; set; }

    public void Clear()
    {
        Mobj = null;
        PlayerState = 0;
        Cmd.Clear();

        ViewZ = Fixed.Zero;
        ViewHeight = Fixed.Zero;
        DeltaViewHeight = Fixed.Zero;
        Bob = Fixed.Zero;

        Health = 0;
        ArmorPoints = 0;
        ArmorType = 0;

        Array.Clear(Powers);
        Cards = CardType.None;
        Backpack = false;

        Array.Clear(Frags);

        ReadyWeapon = WeaponTypes.Fist;
        PendingWeapon = WeaponTypes.Fist;
        WeaponOwned = WeaponTypes.None;

        Array.Clear(Ammo);
        Array.Clear(MaxAmmo);

        UseDown = false;
        AttackDown = false;

        Cheats = 0;

        Refire = 0;

        KillCount = 0;
        ItemCount = 0;
        SecretCount = 0;

        Message = null;
        MessageTime = 0;

        DamageCount = 0;
        BonusCount = 0;

        Attacker = null;

        ExtraLight = 0;

        FixedColorMap = 0;

        ColorMap = 0;

        foreach (var psp in PlayerSprites)
            psp.Clear();

        DidSecret = false;

        interpolate = false;
        oldViewZ = Fixed.Zero;
        oldAngle = Angle.Ang0;
    }

    public void Reborn()
    {
        Mobj = null;
        PlayerState = PlayerState.Live;
        Cmd.Clear();

        ViewZ = Fixed.Zero;
        ViewHeight = Fixed.Zero;
        DeltaViewHeight = Fixed.Zero;
        Bob = Fixed.Zero;

        Health = DoomInfo.DeHackEdConst.InitialHealth;
        ArmorPoints = 0;
        ArmorType = 0;

        Array.Clear(Powers);
        Cards = CardType.None;
        Backpack = false;

        ReadyWeapon = WeaponTypes.Pistol;
        PendingWeapon = WeaponTypes.Pistol;
        WeaponOwned = WeaponTypes.Fist | WeaponTypes.Pistol;

        Array.Clear(Ammo);
        Array.Clear(MaxAmmo);

        Ammo[AmmoType.Clip] = DoomInfo.DeHackEdConst.InitialBullets;
        for (var i = 0; i < AmmoType.Count; i++)
            MaxAmmo[i] = AmmoType.AmmoMax[i];

        // Don't do anything immediately.
        UseDown = true;
        AttackDown = true;

        Cheats = 0;

        Refire = 0;

        Message = null;
        MessageTime = 0;

        DamageCount = 0;
        BonusCount = 0;

        Attacker = null;

        ExtraLight = 0;

        FixedColorMap = 0;

        ColorMap = 0;

        foreach (var psp in PlayerSprites)
            psp.Clear();

        DidSecret = false;

        interpolate = false;
        oldViewZ = Fixed.Zero;
        oldAngle = Angle.Ang0;
    }

    public void FinishLevel()
    {
        Array.Clear(Powers);
        Cards = CardType.None;

        // Cancel invisibility.
        if (Mobj is not null)
            Mobj.Flags &= ~MobjFlags.Shadow;

        // Cancel gun flashes.
        ExtraLight = 0;

        // Cancel ir gogles.
        FixedColorMap = 0;

        // No palette changes.
        DamageCount = 0;
        BonusCount = 0;
    }

    public void SendMessage(string message)
    {
        if (ReferenceEquals(this.Message, (string)DoomInfo.Strings.MSGOFF) &&
            !ReferenceEquals(message, (string)DoomInfo.Strings.MSGON))
        {
            return;
        }

        this.Message = message;
        MessageTime = 4 * GameConst.TicRate;
    }

    public void UpdateFrameInterpolationInfo()
    {
        interpolate = true;
        oldViewZ = ViewZ;
        oldAngle = Mobj!.Angle;
    }

    public void DisableFrameInterpolationForOneFrame()
    {
        interpolate = false;
    }

    public Fixed GetInterpolatedViewZ(Fixed frameFrac)
    {
        // Without the second condition, flicker will occur on the first frame.
        return interpolate && Mobj!.World.LevelTime > 1
            ? oldViewZ + frameFrac * (ViewZ - oldViewZ)
            : ViewZ;
    }

    public Angle GetInterpolatedAngle(Fixed frameFrac)
    {
        if (!interpolate) return Mobj!.Angle;
        var delta = Mobj!.Angle - oldAngle;

        return delta < Angle.Ang180
            ? oldAngle + Angle.FromDegree(frameFrac.ToDouble() * delta.ToDegree())
            : oldAngle - Angle.FromDegree(frameFrac.ToDouble() * (360.0 - delta.ToDegree()));
    }
}