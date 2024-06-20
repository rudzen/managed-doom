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
using System.Runtime.CompilerServices;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Math;
using ManagedDoom.Extensions;

namespace ManagedDoom.Doom.World;

public sealed class ItemPickup(World world)
{
    private const int BonusAdd = 6;

    /// <summary>
    /// Give the player the ammo.
    /// </summary>
    /// <param name="player">The player that picks up the item</param>
    /// <param name="ammo">The ammo type that should be picked up</param>
    /// <param name="amount">
    /// The number of clip loads, not the individual count (0 = 1/2 clip).
    /// </param>
    /// <returns>
    /// False if the ammo can't be picked up at all.
    /// </returns>
    private bool GiveAmmo(Player player, AmmoType ammo, int amount)
    {
        if (ammo == AmmoType.NoAmmo)
            return false;

        if (ammo > AmmoType.Count)
            throw new Exception($"Bad ammo type: {ammo}");

        if (player.Ammo[ammo] == player.MaxAmmo[ammo])
            return false;

        if (amount != 0)
            amount *= ammo.ClipMax;
        else
            amount = ammo.ClipMax / 2;

        // Give double ammo in trainer mode, you'll need in nightmare.
        if (world.Options.Skill is GameSkill.Baby or GameSkill.Nightmare)
            amount <<= 1;

        var oldAmmo = player.Ammo[ammo];
        player.Ammo[ammo] += amount;

        if (player.Ammo[ammo] > player.MaxAmmo[ammo])
            player.Ammo[ammo] = player.MaxAmmo[ammo];

        // If non-zero ammo, don't change up weapons, player was lower on purpose.
        if (oldAmmo != 0)
            return true;

        // We were down to zero, so select a new weapon.
        // Preferences are not user selectable.
        if (ammo == AmmoType.Clip)
        {
            if (player.ReadyWeapon == WeaponTypes.Fist)
            {
                player.PendingWeapon = player.WeaponOwned.Has(WeaponTypes.Chaingun)
                    ? WeaponTypes.Chaingun
                    : WeaponTypes.Pistol;
            }
        }
        else if (ammo == AmmoType.Shell)
        {
            const WeaponTypes fistPistol = WeaponTypes.Fist | WeaponTypes.Pistol;
            if (player.ReadyWeapon.Has(fistPistol))
            {
                if (player.WeaponOwned.Has(WeaponTypes.Shotgun))
                    player.PendingWeapon = WeaponTypes.Shotgun;
            }
        }
        else if (ammo == AmmoType.Cell)
        {
            const WeaponTypes fistPistol = WeaponTypes.Fist | WeaponTypes.Pistol;
            if (player.ReadyWeapon.Has(fistPistol))
            {
                if (player.WeaponOwned.Has(WeaponTypes.Plasma))
                    player.PendingWeapon = WeaponTypes.Plasma;
            }
        }
        else if (ammo == AmmoType.Missile)
        {
            if (player.ReadyWeapon.Has(WeaponTypes.Fist))
            {
                if (player.WeaponOwned.Has(WeaponTypes.Missile))
                    player.PendingWeapon = WeaponTypes.Missile;
            }
        }

        return true;
    }

    /// <summary>
    /// Give the weapon to the player.
    /// </summary>
    /// <param name="player">The player for which to give weapon to</param>
    /// <param name="weapon">The weapon to give to the player</param>
    /// <param name="dropped">True if the weapons is dropped by a monster/// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool GiveWeapon(Player player, WeaponTypes weapon, bool dropped)
    {
        if (world.Options.NetGame && world.Options.Deathmatch != 2 && !dropped)
            return GiveWeaponNet(player, weapon);

        var gaveAmmo = GaveAmmo(player, weapon, dropped);
        var gaveWeapon = GaveWeapon(player, weapon);

        return gaveWeapon | gaveAmmo;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GaveWeapon(Player player, WeaponTypes weapon)
    {
        if (player.WeaponOwned.Has(weapon))
            return false;

        player.WeaponOwned |= weapon;
        player.PendingWeapon = weapon;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool GaveAmmo(Player player, WeaponTypes weapon, bool dropped)
    {
        bool gaveAmmo;
        if (weapon.WeaponInfo().Ammo != AmmoType.NoAmmo)
        {
            // Give one clip with a dropped weapon, two clips with a found weapon.

            // non-branching amount calculation
            // dropped = true, int value = 1
            // dropped = false, int value = 0
            // flipped to false + 1 = 1
            // flipped to true + 1 = 2
            var amount = (!dropped).AsInt() + 1;
            gaveAmmo = GiveAmmo(player, weapon.WeaponInfo().Ammo, amount);
        }
        else
            gaveAmmo = false;

        return gaveAmmo;
    }

    private bool GiveWeaponNet(Player player, WeaponTypes weapon)
    {
        // Leave placed weapons forever on net games.
        if (player.WeaponOwned.Has(weapon))
            return false;

        player.BonusCount += BonusAdd;
        player.WeaponOwned |= weapon;

        var amountToGive = world.Options.Deathmatch != 0 ? 5 : 2;
        GiveAmmo(player, weapon.WeaponInfo().Ammo, amountToGive);

        player.PendingWeapon = weapon;

        if (player == world.ConsolePlayer)
            world.StartSound(player.Mobj!, Sfx.WPNUP, SfxType.Misc);

        return false;
    }

    /// <summary>
    /// Give the health point to the player.
    /// </summary>
    /// <returns>
    /// False if the health point isn't needed at all.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GiveHealth(Player player, int amount)
    {
        if (player.Health >= DoomInfo.DeHackEdConst.InitialHealth)
            return false;

        player.Health += amount;
        if (player.Health > DoomInfo.DeHackEdConst.InitialHealth)
            player.Health = DoomInfo.DeHackEdConst.InitialHealth;

        player.Mobj!.Health = player.Health;

        return true;
    }

    /// <summary>
    /// Give the armor to the player.
    /// </summary>
    /// <returns>
    /// Returns false if the armor is worse than the current armor.
    /// </returns>
    private static bool GiveArmor(Player player, int type)
    {
        var hits = type * 100;

        // Don't pick up.
        if (player.ArmorPoints >= hits)
            return false;

        player.ArmorType = type;
        player.ArmorPoints = hits;

        return true;
    }

    /// <summary>
    /// Give the card to the player.
    /// </summary>
    private static void GiveCard(Player player, CardType card)
    {
        if (player.Cards.Has(card))
            return;

        player.BonusCount = BonusAdd;
        player.Cards |= card;
    }

    /// <summary>
    /// Give the power up to the player.
    /// </summary>
    /// <returns>
    /// False if the power up is not necessary.
    /// </returns>
    private static bool GivePower(Player player, PowerType types)
    {
        if (types == PowerType.Invulnerability)
        {
            player.Powers[types] = DoomInfo.PowerDuration.Invulnerability;
            return true;
        }

        if (types == PowerType.Invisibility)
        {
            player.Powers[types] = DoomInfo.PowerDuration.Invisibility;
            player.Mobj!.Flags |= MobjFlags.Shadow;
            return true;
        }

        if (types == PowerType.Infrared)
        {
            player.Powers[types] = DoomInfo.PowerDuration.Infrared;
            return true;
        }

        if (types == PowerType.IronFeet)
        {
            player.Powers[types] = DoomInfo.PowerDuration.IronFeet;
            return true;
        }

        if (types == PowerType.Strength)
        {
            GiveHealth(player, 100);
            player.Powers[types] = 1;
            return true;
        }

        // Already got it.
        if (player.Powers[types] != 0)
            return false;

        player.Powers[types] = 1;

        return true;
    }

    /// <summary>
    /// Check for item pickup.
    /// </summary>
    // ReSharper disable once CognitiveComplexity
    public void TouchSpecialThing(Mobj special, Mobj toucher)
    {
        var delta = special.Z - toucher.Z;

        // Out of reach.
        if (delta > toucher.Height || delta < Fixed.FromInt(-8))
            return;

        var sound = Sfx.ITEMUP;
        var player = toucher.Player;

        if (player is null)
            return;

        // Dead thing touching.
        // Can happen with a sliding player corpse.
        if (toucher.Health <= 0)
            return;

        // Identify by sprite.
        switch (special.Sprite)
        {
            // Armor.
            case Sprite.ARM1:
                if (!GiveArmor(player, DoomInfo.DeHackEdConst.GreenArmorClass))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTARMOR);
                break;

            case Sprite.ARM2:
                if (!GiveArmor(player, DoomInfo.DeHackEdConst.BlueArmorClass))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTMEGA);
                break;

            // Bonus items.
            case Sprite.BON1:
                // Can go over 100%.
                player.Health++;
                if (player.Health > DoomInfo.DeHackEdConst.MaxHealth)
                    player.Health = DoomInfo.DeHackEdConst.MaxHealth;

                player.Mobj!.Health = player.Health;
                player.SendMessage(DoomInfo.Strings.GOTHTHBONUS);
                break;

            case Sprite.BON2:
                // Can go over 100%.
                player.ArmorPoints++;
                if (player.ArmorPoints > DoomInfo.DeHackEdConst.MaxArmor)
                    player.ArmorPoints = DoomInfo.DeHackEdConst.MaxArmor;

                if (player.ArmorType == 0)
                    player.ArmorType = DoomInfo.DeHackEdConst.GreenArmorClass;

                player.SendMessage(DoomInfo.Strings.GOTARMBONUS);
                break;

            case Sprite.SOUL:
                player.Health += DoomInfo.DeHackEdConst.SoulsphereHealth;
                if (player.Health > DoomInfo.DeHackEdConst.MaxSoulsphere)
                    player.Health = DoomInfo.DeHackEdConst.MaxSoulsphere;

                player.Mobj!.Health = player.Health;
                player.SendMessage(DoomInfo.Strings.GOTSUPER);
                sound = Sfx.GETPOW;
                break;

            case Sprite.MEGA:
                if (world.Options.GameMode != GameMode.Commercial)
                    return;

                player.Health = DoomInfo.DeHackEdConst.MegasphereHealth;
                player.Mobj!.Health = player.Health;
                GiveArmor(player, DoomInfo.DeHackEdConst.BlueArmorClass);
                player.SendMessage(DoomInfo.Strings.GOTMSPHERE);
                sound = Sfx.GETPOW;
                break;

            // Cards.
            // Leave cards for everyone.
            case Sprite.BKEY:
                if (!player.Cards.Has(CardType.BlueCard))
                    player.SendMessage(DoomInfo.Strings.GOTBLUECARD);

                GiveCard(player, CardType.BlueCard);
                if (!world.Options.NetGame)
                    break;

                return;

            case Sprite.YKEY:
                if (!player.Cards.Has(CardType.YellowCard))
                    player.SendMessage(DoomInfo.Strings.GOTYELWCARD);

                GiveCard(player, CardType.YellowCard);
                if (!world.Options.NetGame)
                    break;

                return;

            case Sprite.RKEY:
                if (!player.Cards.Has(CardType.RedCard))
                    player.SendMessage(DoomInfo.Strings.GOTREDCARD);

                GiveCard(player, CardType.RedCard);
                if (!world.Options.NetGame)
                    break;

                return;

            case Sprite.BSKU:
                if (!player.Cards.Has(CardType.BlueSkull))
                    player.SendMessage(DoomInfo.Strings.GOTBLUESKUL);

                GiveCard(player, CardType.BlueSkull);
                if (!world.Options.NetGame)
                    break;

                return;

            case Sprite.YSKU:
                if (!player.Cards.Has(CardType.YellowSkull))
                    player.SendMessage(DoomInfo.Strings.GOTYELWSKUL);

                GiveCard(player, CardType.YellowSkull);
                if (!world.Options.NetGame)
                    break;

                return;

            case Sprite.RSKU:
                if (!player.Cards.Has(CardType.RedSkull))
                    player.SendMessage(DoomInfo.Strings.GOTREDSKULL);

                GiveCard(player, CardType.RedSkull);
                if (!world.Options.NetGame)
                    break;

                return;

            // Medikits, heals.
            case Sprite.STIM:
                if (!GiveHealth(player, 10))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTSTIM);
                break;

            case Sprite.MEDI:
                if (!GiveHealth(player, 25))
                    return;

                var msg = player.Health < 25
                    ? DoomInfo.Strings.GOTMEDINEED
                    : DoomInfo.Strings.GOTMEDIKIT;

                player.SendMessage(msg);
                break;

            // Power ups.
            case Sprite.PINV:
                if (!GivePower(player, PowerType.Invulnerability))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTINVUL);
                sound = Sfx.GETPOW;
                break;

            case Sprite.PSTR:
                if (!GivePower(player, PowerType.Strength))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTBERSERK);
                if (player.ReadyWeapon != WeaponTypes.Fist)
                    player.PendingWeapon = WeaponTypes.Fist;

                sound = Sfx.GETPOW;
                break;

            case Sprite.PINS:
                if (!GivePower(player, PowerType.Invisibility))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTINVIS);
                sound = Sfx.GETPOW;
                break;

            case Sprite.SUIT:
                if (!GivePower(player, PowerType.IronFeet))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTSUIT);
                sound = Sfx.GETPOW;
                break;

            case Sprite.PMAP:
                if (!GivePower(player, PowerType.AllMap))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTMAP);
                sound = Sfx.GETPOW;
                break;

            case Sprite.PVIS:
                if (!GivePower(player, PowerType.Infrared))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTVISOR);
                sound = Sfx.GETPOW;
                break;

            // Ammo.
            case Sprite.CLIP:
                if ((special.Flags & MobjFlags.Dropped) != 0)
                {
                    if (!GiveAmmo(player, AmmoType.Clip, 0))
                        return;
                }
                else
                {
                    if (!GiveAmmo(player, AmmoType.Clip, 1))
                        return;
                }

                player.SendMessage(DoomInfo.Strings.GOTCLIP);
                break;

            case Sprite.AMMO:
                if (!GiveAmmo(player, AmmoType.Clip, 5))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTCLIPBOX);
                break;

            case Sprite.ROCK:
                if (!GiveAmmo(player, AmmoType.Missile, 1))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTROCKET);
                break;

            case Sprite.BROK:
                if (!GiveAmmo(player, AmmoType.Missile, 5))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTROCKBOX);
                break;

            case Sprite.CELL:
                if (!GiveAmmo(player, AmmoType.Cell, 1))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTCELL);
                break;

            case Sprite.CELP:
                if (!GiveAmmo(player, AmmoType.Cell, 5))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTCELLBOX);
                break;

            case Sprite.SHEL:
                if (!GiveAmmo(player, AmmoType.Shell, 1))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTSHELLS);
                break;

            case Sprite.SBOX:
                if (!GiveAmmo(player, AmmoType.Shell, 5))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTSHELLBOX);
                break;

            case Sprite.BPAK:
                if (!player.Backpack)
                {
                    for (var i = 0; i < AmmoType.Count; i++)
                        player.MaxAmmo[i] *= 2;

                    player.Backpack = true;
                }

                for (var i = AmmoTypes.Clip; i < AmmoTypes.Count; i++)
                    GiveAmmo(player, new AmmoType(i), 1);

                player.SendMessage(DoomInfo.Strings.GOTBACKPACK);
                break;

            // Weapons.
            case Sprite.BFUG:
                if (!GiveWeapon(player, WeaponTypes.Bfg, false))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTBFG9000);
                sound = Sfx.WPNUP;
                break;

            case Sprite.MGUN:
                if (!GiveWeapon(player, WeaponTypes.Chaingun, (special.Flags & MobjFlags.Dropped) != 0))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTCHAINGUN);
                sound = Sfx.WPNUP;
                break;

            case Sprite.CSAW:
                if (!GiveWeapon(player, WeaponTypes.Chainsaw, false))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTCHAINSAW);
                sound = Sfx.WPNUP;
                break;

            case Sprite.LAUN:
                if (!GiveWeapon(player, WeaponTypes.Missile, false))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTLAUNCHER);
                sound = Sfx.WPNUP;
                break;

            case Sprite.PLAS:
                if (!GiveWeapon(player, WeaponTypes.Plasma, false))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTPLASMA);
                sound = Sfx.WPNUP;
                break;

            case Sprite.SHOT:
                if (!GiveWeapon(player, WeaponTypes.Shotgun, (special.Flags & MobjFlags.Dropped) != 0))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTSHOTGUN);
                sound = Sfx.WPNUP;
                break;

            case Sprite.SGN2:
                if (!GiveWeapon(player, WeaponTypes.SuperShotgun, (special.Flags & MobjFlags.Dropped) != 0))
                    return;

                player.SendMessage(DoomInfo.Strings.GOTSHOTGUN2);
                sound = Sfx.WPNUP;
                break;

            default:
                throw new Exception("Unknown gettable thing!");
        }

        if ((special.Flags & MobjFlags.CountItem) != 0)
            player.ItemCount++;

        world.ThingAllocation.RemoveMobj(special);

        player.BonusCount += BonusAdd;

        if (player == world.ConsolePlayer)
            world.StartSound(player.Mobj!, sound, SfxType.Misc);
    }
}