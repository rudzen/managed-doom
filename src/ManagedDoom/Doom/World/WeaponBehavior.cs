//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
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
using System.Runtime.CompilerServices;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class WeaponBehavior(World world)
{
    public static readonly Fixed MeleeRange = Fixed.FromInt(64);
    public static readonly Fixed MissileRange = Fixed.FromInt(32 * 64);

    private static readonly Fixed WeaponTop = Fixed.FromInt(32);
    public static readonly Fixed WeaponBottom = Fixed.FromInt(128);

    private static readonly Fixed RaiseSpeed = Fixed.FromInt(6);
    private static readonly Fixed LowerSpeed = Fixed.FromInt(6);

    private Fixed currentBulletSlope;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Light0(Player player)
    {
        player.ExtraLight = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeaponReady(Player player, PlayerSpriteDef psp)
    {
        var pb = world.PlayerBehavior;

        // Get out of attack state.
        if (player.Mobj.State == DoomInfo.States[(int)MobjState.PlayAtk1] ||
            player.Mobj.State == DoomInfo.States[(int)MobjState.PlayAtk2])
        {
            player.Mobj.SetState(MobjState.Play);
        }

        if (player.ReadyWeapon == WeaponType.Chainsaw &&
            psp.State == DoomInfo.States[(int)MobjState.Saw])
        {
            world.StartSound(player.Mobj, Sfx.SAWIDL, SfxType.Weapon);
        }

        // Check for weapon change.
        // If player is dead, put the weapon away.
        if (player.PendingWeapon != WeaponType.NoChange || player.Health == 0)
        {
            // Change weapon.
            // Pending weapon should allready be validated.
            var newState = DoomInfo.WeaponInfos[player.ReadyWeapon].DownState;
            pb.SetPlayerSprite(player, PlayerSprite.Weapon, newState);
            return;
        }

        // Check for fire.
        // The missile launcher and bfg do not auto fire.
        if ((player.Cmd.Buttons & TicCmdButtons.Attack) != 0)
        {
            if (!player.AttackDown ||
                (player.ReadyWeapon != WeaponType.Missile && player.ReadyWeapon != WeaponType.Bfg))
            {
                player.AttackDown = true;
                FireWeapon(player);
                return;
            }
        }
        else
            player.AttackDown = false;

        // Bob the weapon based on movement speed.
        var angle = (128 * player.Mobj.World.LevelTime) & Trig.FineMask;
        psp.Sx = Fixed.One + player.Bob * Trig.Cos(angle);

        angle &= Trig.FineAngleCount / 2 - 1;
        psp.Sy = WeaponTop + player.Bob * Trig.Sin(angle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckAmmo(Player player)
    {
        var ammo = DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo;

        int count;

        // Minimal amount for one shot varies.
        if (player.ReadyWeapon == WeaponTypes.Bfg)
            count = DoomInfo.DeHackEdConst.BfgCellsPerShot;
        else if (player.ReadyWeapon == WeaponTypes.SuperShotgun)
            count = 2; // double barrel
        else
            count = 1; // regular

        // Some do not need ammunition anyway.
        // Return if current ammunition sufficient.
        if (ammo == AmmoType.NoAmmo || player.Ammo[(int)ammo] >= count)
            return true;

        // Out of ammo, pick a weapon to change to.
        // Preferences are set here.
        do
        {
            if (player.WeaponOwned[WeaponType.Plasma] &&
                player.Ammo[(int)AmmoTypes.Cell] > 0 &&
                world.Options.GameMode != GameMode.Shareware)
            {
                player.PendingWeapon = WeaponType.Plasma;
            }
            else if (player.WeaponOwned[WeaponType.SuperShotgun] &&
                     player.Ammo[(int)AmmoTypes.Shell] > 2 &&
                     world.Options.GameMode == GameMode.Commercial)
            {
                player.PendingWeapon = WeaponType.SuperShotgun;
            }
            else if (player.WeaponOwned[WeaponType.Chaingun] &&
                     player.Ammo[(int)AmmoTypes.Clip] > 0)
            {
                player.PendingWeapon = WeaponType.Chaingun;
            }
            else if (player.WeaponOwned[WeaponType.Shotgun] &&
                     player.Ammo[(int)AmmoTypes.Shell] > 0)
            {
                player.PendingWeapon = WeaponType.Shotgun;
            }
            else if (player.Ammo[(int)AmmoTypes.Clip] > 0)
            {
                player.PendingWeapon = WeaponType.Pistol;
            }
            else if (player.WeaponOwned[WeaponType.Chainsaw])
            {
                player.PendingWeapon = WeaponType.Chainsaw;
            }
            else if (player.WeaponOwned[WeaponType.Missile] &&
                     player.Ammo[(int)AmmoTypes.Missile] > 0)
            {
                player.PendingWeapon = WeaponType.Missile;
            }
            else if (player.WeaponOwned[WeaponType.Bfg] &&
                     player.Ammo[(int)AmmoTypes.Cell] > DoomInfo.DeHackEdConst.BfgCellsPerShot &&
                     world.Options.GameMode != GameMode.Shareware)
            {
                player.PendingWeapon = WeaponType.Bfg;
            }
            else
            {
                // If everything fails.
                player.PendingWeapon = WeaponType.Fist;
            }
        } while (player.PendingWeapon == WeaponType.NoChange);

        // Now set appropriate weapon overlay.
        world.PlayerBehavior.SetPlayerSprite(
            player,
            PlayerSprite.Weapon,
            DoomInfo.WeaponInfos[player.ReadyWeapon].DownState);

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RecursiveSound(Sector sec, int soundblocks, Mobj soundtarget, int validCount)
    {
        // Wake up all monsters in this sector.
        if (sec.ValidCount == validCount && sec.SoundTraversed <= soundblocks + 1)
        {
            // Already flooded.
            return;
        }

        sec.ValidCount = validCount;
        sec.SoundTraversed = soundblocks + 1;
        sec.SoundTarget = soundtarget;

        var mc = world.MapCollision;

        foreach (var check in sec.Lines.AsSpan())
        {
            if ((check.Flags & LineFlags.TwoSided) == 0)
                continue;

            mc.LineOpening(check);

            // Closed door.
            if (mc.OpenRange <= Fixed.Zero)
                continue;

            var other = check.FrontSide.Sector == sec ? check.BackSide.Sector : check.FrontSide.Sector;

            if ((check.Flags & LineFlags.SoundBlock) != 0)
            {
                if (soundblocks == 0)
                    RecursiveSound(other, 1, soundtarget, validCount);
            }
            else
                RecursiveSound(other, soundblocks, soundtarget, validCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NoiseAlert(Mobj target, Mobj emmiter)
    {
        RecursiveSound(
            emmiter.Subsector.Sector,
            0,
            target,
            world.GetNewValidCount());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FireWeapon(Player player)
    {
        if (!CheckAmmo(player))
            return;

        player.Mobj.SetState(MobjState.PlayAtk1);

        var newState = DoomInfo.WeaponInfos[player.ReadyWeapon].AttackState;
        world.PlayerBehavior.SetPlayerSprite(player, PlayerSprite.Weapon, newState);

        NoiseAlert(player.Mobj, player.Mobj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Lower(Player player, PlayerSpriteDef psp)
    {
        psp.Sy += LowerSpeed;

        // Is already down.
        if (psp.Sy < WeaponBottom)
            return;

        // Player is dead.
        if (player.PlayerState == PlayerState.Dead)
        {
            psp.Sy = WeaponBottom;

            // don't bring weapon back up
            return;
        }

        var pb = world.PlayerBehavior;

        // The old weapon has been lowered off the screen,
        // so change the weapon and start raising it.
        if (player.Health == 0)
        {
            // Player is dead, so keep the weapon off screen.
            pb.SetPlayerSprite(player, PlayerSprite.Weapon, MobjState.Null);
            return;
        }

        player.ReadyWeapon = player.PendingWeapon;

        pb.BringUpWeapon(player);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Raise(Player player, PlayerSpriteDef psp)
    {
        psp.Sy -= RaiseSpeed;

        if (psp.Sy > WeaponTop)
            return;

        psp.Sy = WeaponTop;

        // The weapon has been raised all the way, so change to the ready state.
        var newState = DoomInfo.WeaponInfos[player.ReadyWeapon].ReadyState;

        world.PlayerBehavior.SetPlayerSprite(player, PlayerSprite.Weapon, newState);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Punch(Player player)
    {
        var random = world.Random;

        var damage = (random.Next() % 10 + 1) << 1;
        if (player.Powers[(int)PowerType.Strength] != 0)
            damage *= 10;

        var hs = world.Hitscan;

        var angle = player.Mobj.Angle;
        angle += new Angle((random.Next() - random.Next()) << 18);

        var slope = hs.AimLineAttack(player.Mobj, angle, MeleeRange);
        hs.LineAttack(player.Mobj, angle, MeleeRange, slope, damage);

        // Do we need to turn to face target?
        if (hs.LineTarget == null)
            return;

        world.StartSound(player.Mobj, Sfx.PUNCH, SfxType.Weapon);

        player.Mobj.Angle = Geometry.PointToAngle(
            player.Mobj.X, player.Mobj.Y,
            hs.LineTarget.X, hs.LineTarget.Y);
    }


    public void Saw(Player player)
    {
        var damage = 2 * (world.Random.Next() % 10 + 1);

        var random = world.Random;

        var attackAngle = player.Mobj.Angle;
        attackAngle += new Angle((random.Next() - random.Next()) << 18);

        var hs = world.Hitscan;

        // Use MeleeRange + Fixed.Epsilon so that the puff doesn't skip the flash.
        var slope = hs.AimLineAttack(player.Mobj, attackAngle, MeleeRange + Fixed.Epsilon);
        hs.LineAttack(player.Mobj, attackAngle, MeleeRange + Fixed.Epsilon, slope, damage);

        if (hs.LineTarget == null)
        {
            world.StartSound(player.Mobj, Sfx.SAWFUL, SfxType.Weapon);
            return;
        }

        world.StartSound(player.Mobj, Sfx.SAWHIT, SfxType.Weapon);

        // Turn to face target.
        var targetAngle = Geometry.PointToAngle(
            player.Mobj.X, player.Mobj.Y,
            hs.LineTarget.X, hs.LineTarget.Y);

        if (targetAngle - player.Mobj.Angle > Angle.Ang180)
        {
            // The code below is based on Mocha Doom's implementation.
            // It is still unclear for me why this code works like the original verion...
            if ((int)(targetAngle - player.Mobj.Angle).Data < -Angle.Ang90.Data / 20)
                player.Mobj.Angle = targetAngle + Angle.Ang90 / 21;
            else
                player.Mobj.Angle -= Angle.Ang90 / 20;
        }
        else
        {
            if (targetAngle - player.Mobj.Angle > Angle.Ang90 / 20)
                player.Mobj.Angle = targetAngle - Angle.Ang90 / 21;
            else
                player.Mobj.Angle += Angle.Ang90 / 20;
        }

        player.Mobj.Flags |= MobjFlags.JustAttacked;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReFire(Player player)
    {
        // Check for fire.
        // If a weaponchange is pending, let it go through instead.
        if ((player.Cmd.Buttons & TicCmdButtons.Attack) != 0 &&
            player.PendingWeapon == WeaponType.NoChange &&
            player.Health != 0)
        {
            player.Refire++;
            FireWeapon(player);
        }
        else
        {
            player.Refire = 0;
            CheckAmmo(player);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BulletSlope(Mobj mo)
    {
        var hs = world.Hitscan;

        // See which target is to be aimed at.
        var angle = mo.Angle;

        currentBulletSlope = hs.AimLineAttack(mo, angle, Fixed.FromInt(1024));

        if (hs.LineTarget != null)
            return;

        angle += new Angle(1 << 26);
        currentBulletSlope = hs.AimLineAttack(mo, angle, Fixed.FromInt(1024));

        if (hs.LineTarget != null)
            return;

        angle -= new Angle(2 << 26);
        currentBulletSlope = hs.AimLineAttack(mo, angle, Fixed.FromInt(1024));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GunShot(Mobj mo, bool accurate)
    {
        var random = world.Random;
        var damage = 5 * (random.Next() % 3 + 1);

        var angle = mo.Angle;
        if (!accurate)
            angle += new Angle((random.Next() - random.Next()) << 18);

        world.Hitscan.LineAttack(mo, angle, MissileRange, currentBulletSlope, damage);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FirePistol(Player player)
    {
        world.StartSound(player.Mobj, Sfx.PISTOL, SfxType.Weapon);

        player.Mobj.SetState(MobjState.PlayAtk2);

        player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo]--;

        world.PlayerBehavior.SetPlayerSprite(
            player,
            PlayerSprite.Flash,
            DoomInfo.WeaponInfos[player.ReadyWeapon].FlashState);

        BulletSlope(player.Mobj);

        GunShot(player.Mobj, player.Refire == 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Light1(Player player)
    {
        player.ExtraLight = 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FireShotgun(Player player)
    {
        world.StartSound(player.Mobj, Sfx.SHOTGN, SfxType.Weapon);

        player.Mobj.SetState(MobjState.PlayAtk2);

        player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo]--;

        world.PlayerBehavior.SetPlayerSprite(
            player,
            PlayerSprite.Flash,
            DoomInfo.WeaponInfos[player.ReadyWeapon].FlashState);

        BulletSlope(player.Mobj);

        for (var i = 0; i < 7; i++)
        {
            GunShot(player.Mobj, false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Light2(Player player)
    {
        player.ExtraLight = 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FireCGun(Player player, PlayerSpriteDef psp)
    {
        world.StartSound(player.Mobj, Sfx.PISTOL, SfxType.Weapon);

        if (player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo] == 0)
            return;

        player.Mobj.SetState(MobjState.PlayAtk2);

        player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo]--;

        world.PlayerBehavior.SetPlayerSprite(
            player,
            PlayerSprite.Flash,
            DoomInfo.WeaponInfos[player.ReadyWeapon].FlashState +
            psp.State.Number - DoomInfo.States[(int)MobjState.Chain1].Number);

        BulletSlope(player.Mobj);

        GunShot(player.Mobj, player.Refire == 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FireShotgun2(Player player)
    {
        world.StartSound(player.Mobj, Sfx.DSHTGN, SfxType.Weapon);

        player.Mobj.SetState(MobjState.PlayAtk2);

        player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo] -= 2;

        world.PlayerBehavior.SetPlayerSprite(
            player,
            PlayerSprite.Flash,
            DoomInfo.WeaponInfos[player.ReadyWeapon].FlashState);

        BulletSlope(player.Mobj);

        var random = world.Random;
        var hs = world.Hitscan;

        for (var i = 0; i < 20; i++)
        {
            var damage = 5 * (random.Next() % 3 + 1);
            var angle = player.Mobj.Angle;
            angle += new Angle((random.Next() - random.Next()) << 19);
            hs.LineAttack(
                player.Mobj,
                angle,
                MissileRange,
                currentBulletSlope + new Fixed((random.Next() - random.Next()) << 5),
                damage);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CheckReload(Player player)
    {
        CheckAmmo(player);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OpenShotgun2(Player player)
    {
        world.StartSound(player.Mobj, Sfx.DBOPN, SfxType.Weapon);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LoadShotgun2(Player player)
    {
        world.StartSound(player.Mobj, Sfx.DBLOAD, SfxType.Weapon);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CloseShotgun2(Player player)
    {
        world.StartSound(player.Mobj, Sfx.DBCLS, SfxType.Weapon);
        ReFire(player);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GunFlash(Player player)
    {
        player.Mobj.SetState(MobjState.PlayAtk2);

        world.PlayerBehavior.SetPlayerSprite(
            player,
            PlayerSprite.Flash,
            DoomInfo.WeaponInfos[player.ReadyWeapon].FlashState);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FireMissile(Player player)
    {
        player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo]--;

        world.ThingAllocation.SpawnPlayerMissile(player.Mobj, MobjType.Rocket);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FirePlasma(Player player)
    {
        player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo]--;

        world.PlayerBehavior.SetPlayerSprite(
            player,
            PlayerSprite.Flash,
            DoomInfo.WeaponInfos[player.ReadyWeapon].FlashState + (world.Random.Next() & 1));

        world.ThingAllocation.SpawnPlayerMissile(player.Mobj, MobjType.Plasma);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void A_BFGsound(Player player)
    {
        world.StartSound(player.Mobj, Sfx.BFG, SfxType.Weapon);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FireBFG(Player player)
    {
        player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo] -= DoomInfo.DeHackEdConst.BfgCellsPerShot;

        world.ThingAllocation.SpawnPlayerMissile(player.Mobj, MobjType.Bfg);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BFGSpray(Mobj bfgBall)
    {
        var hs = world.Hitscan;
        var random = world.Random;

        // Offset angles from its attack angle.
        for (var i = 0; i < 40; i++)
        {
            var an = bfgBall.Angle - Angle.Ang90 / 2 + Angle.Ang90 / 40 * (uint)i;

            // bfgBall.Target is the originator (player) of the missile.
            hs.AimLineAttack(bfgBall.Target, an, Fixed.FromInt(16 * 64));

            if (hs.LineTarget == null)
                continue;

            world.ThingAllocation.SpawnMobj(
                hs.LineTarget.X,
                hs.LineTarget.Y,
                hs.LineTarget.Z + (hs.LineTarget.Height >> 2),
                MobjType.Extrabfg);

            var damage = 0;
            for (var j = 0; j < 15; j++)
                damage += (random.Next() & 7) + 1;

            world.ThingInteraction.DamageMobj(
                hs.LineTarget,
                bfgBall.Target,
                bfgBall.Target,
                damage);
        }
    }
}