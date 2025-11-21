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

public enum PlayerState
{
    // Playing or camping.
    Live,

    // Dead on the ground, view follows killer.
    Dead,

    // Ready to restart / respawn???
    Reborn
}

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

    // For frame interpolation.
    private bool interpolate;
    private Fixed oldViewZ;
    private Angle oldAngle;

    public Player(int number)
    {
        this.Number = number;

        Name = defaultPlayerNames[number];

        Command = new TicCommand();

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

    public TicCommand Command { get; }

    /// <summary>
    /// Determine POV, including viewpoint bobbing during movement.
    /// Focal origin above mobj.Z.
    /// </summary>
    public Fixed ViewZ { get; set; }

    /// <summary>
    /// Base height above floor for viewz.
    /// </summary>
    public Fixed ViewHeight { get; set; }

    /// <summary>
    /// Bob / squat speed.
    /// </summary>
    public Fixed DeltaViewHeight { get; set; }

    /// <summary>
    /// Bounded / scaled total momentum.
    /// </summary>
    public Fixed Bob { get; set; }

    /// <summary>
    /// This is only used between levels,
    /// mobj.Health is used during levels.
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// This is only used between levels,
    /// mobj.Health is used during levels.
    /// </summary>
    public int ArmorPoints { get; set; }

    /// <summary>
    /// Armor type is 0-2.
    /// </summary>
    public int ArmorType { get; set; }

    /// <summary>
    /// Power ups. invinc and invis are tic counters.
    /// </summary>
    public int[] Powers { get; }

    public CardType Cards { get; set; }

    public bool Backpack { get; set; }

    /// <summary>
    /// Frags, kills of other players.
    /// </summary>
    public int[] Frags { get; }

    public WeaponTypes ReadyWeapon { get; set; }

    /// <summary>
    /// Is WeaponTypes.None if not changing.
    /// </summary>
    public WeaponTypes PendingWeapon { get; set; }

    public WeaponTypes WeaponOwned { get; set; }

    public int[] Ammo { get; }

    public int[] MaxAmmo { get; }

    /// <summary>
    /// True if button down last tic.
    /// </summary>
    public bool AttackDown { get; set; }

    public bool UseDown { get; set; }

    /// <summary>
    /// Bit flags, for cheats and debug.
    /// </summary>
    public CheatFlags Cheats { get; set; }

    /// <summary>
    /// Refired shots are less accurate.
    /// </summary>
    public int Refire { get; set; }

    /// <summary>
    /// For intermission stats.
    /// </summary>
    public int KillCount { get; set; }

    /// <summary>
    /// For intermission stats.
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// For intermission stats.
    /// </summary>
    public int SecretCount { get; set; }

    /// <summary>
    /// Hint messages.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Hint message time to live.
    /// </summary>
    public int MessageTime { get; set; }

    /// <summary>
    /// For screen flashing (red).
    /// </summary>
    public int DamageCount { get; set; }

    /// <summary>
    /// For screen flashing (bright).
    /// </summary>
    public int BonusCount { get; set; }

    /// <summary>
    /// Who did damage (null for floors / ceilings)
    /// </summary>
    public Mobj? Attacker { get; set; }

    /// <summary>
    /// So gun flashes light up areas.
    /// </summary>
    public int ExtraLight { get; set; }

    /// <summary>
    /// Current PLAYPAL, ???
    /// can be set to REDCOLORMAP for pain, etc.
    /// </summary>
    public int FixedColorMap { get; set; }

    /// <summary>
    /// Player skin colorshift,
    /// 0-3 for which color to draw player.
    /// </summary>
    public int ColorMap { get; set; }

    /// <summary>
    /// Overlay view sprites (gun, etc).
    /// </summary>
    public PlayerSpriteDef[] PlayerSprites { get; }

    /// <summary>
    /// True if secret level has been done.
    /// </summary>
    public bool DidSecret { get; set; }

    public void Clear()
    {
        Mobj = null;
        PlayerState = 0;
        Command.Clear();

        ViewZ = Fixed.Zero;
        ViewHeight = Fixed.Zero;
        DeltaViewHeight = Fixed.Zero;
        Bob = Fixed.Zero;

        Health = 0;
        ArmorPoints = 0;
        ArmorType = 0;

        Powers.AsSpan().Clear();
        Cards = CardType.None;
        Backpack = false;

        Frags.AsSpan().Clear();

        ReadyWeapon = WeaponTypes.Fist;
        PendingWeapon = WeaponTypes.Fist;
        WeaponOwned = WeaponTypes.None;

        Ammo.AsSpan().Clear();
        MaxAmmo.AsSpan().Clear();

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
        Command.Clear();

        ViewZ = Fixed.Zero;
        ViewHeight = Fixed.Zero;
        DeltaViewHeight = Fixed.Zero;
        Bob = Fixed.Zero;

        Health = DoomInfo.DeHackEdConst.InitialHealth;
        ArmorPoints = 0;
        ArmorType = 0;

        Powers.AsSpan().Clear();
        Cards = CardType.None;
        Backpack = false;

        ReadyWeapon = WeaponTypes.Pistol;
        PendingWeapon = WeaponTypes.Pistol;
        WeaponOwned = WeaponTypes.Fist | WeaponTypes.Pistol;

        Ammo.AsSpan().Clear();
        MaxAmmo.AsSpan().Clear();

        Ammo[AmmoType.Clip] = DoomInfo.DeHackEdConst.InitialBullets;
        AmmoType.AmmoMax.AsSpan().CopyTo(MaxAmmo);

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
        Powers.AsSpan().Clear();
        Cards = CardType.None;

        // Cancel invisibility.
        Mobj?.Flags &= ~MobjFlags.Shadow;

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