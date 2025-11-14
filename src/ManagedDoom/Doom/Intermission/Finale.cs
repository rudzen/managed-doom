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
using ManagedDoom.Audio;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Intermission;

public sealed class Finale
{
    public const int TextSpeed = 3;
    private const int TextWait = 250;

    // Stage of animation:
    // 0 = text, 1 = art screen, 2 = character cast.

    // For bunny scroll.

    private UpdateResult updateResult;

    public Finale(IGameOptions options)
    {
        this.Options = options;

        string c1Text;
        string c2Text;
        string c3Text;
        string c4Text;
        string c5Text;
        string c6Text;
        switch (options.MissionPack)
        {
            case MissionPack.Plutonia:
                c1Text = DoomInfo.Strings.P1TEXT;
                c2Text = DoomInfo.Strings.P2TEXT;
                c3Text = DoomInfo.Strings.P3TEXT;
                c4Text = DoomInfo.Strings.P4TEXT;
                c5Text = DoomInfo.Strings.P5TEXT;
                c6Text = DoomInfo.Strings.P6TEXT;
                break;

            case MissionPack.Tnt:
                c1Text = DoomInfo.Strings.T1TEXT;
                c2Text = DoomInfo.Strings.T2TEXT;
                c3Text = DoomInfo.Strings.T3TEXT;
                c4Text = DoomInfo.Strings.T4TEXT;
                c5Text = DoomInfo.Strings.T5TEXT;
                c6Text = DoomInfo.Strings.T6TEXT;
                break;

            default:
                c1Text = DoomInfo.Strings.C1TEXT;
                c2Text = DoomInfo.Strings.C2TEXT;
                c3Text = DoomInfo.Strings.C3TEXT;
                c4Text = DoomInfo.Strings.C4TEXT;
                c5Text = DoomInfo.Strings.C5TEXT;
                c6Text = DoomInfo.Strings.C6TEXT;
                break;
        }

        switch (options.GameMode)
        {
            case GameMode.Shareware:
            case GameMode.Registered:
            case GameMode.Retail:
                options.Music.StartMusic(Bgm.VICTOR, PlayMode.Loop);
                switch (options.Episode)
                {
                    case 1:
                        Flat = "FLOOR4_8";
                        Text = DoomInfo.Strings.E1TEXT;
                        break;

                    case 2:
                        Flat = "SFLR6_1";
                        Text = DoomInfo.Strings.E2TEXT;
                        break;

                    case 3:
                        Flat = "MFLR8_4";
                        Text = DoomInfo.Strings.E3TEXT;
                        break;

                    case 4:
                        Flat = "MFLR8_3";
                        Text = DoomInfo.Strings.E4TEXT;
                        break;

                    default:
                        break;
                }

                break;

            case GameMode.Commercial:
                options.Music.StartMusic(Bgm.READ_M, PlayMode.Loop);
                switch (options.Map)
                {
                    case 6:
                        Flat = "SLIME16";
                        Text = c1Text;
                        break;

                    case 11:
                        Flat = "RROCK14";
                        Text = c2Text;
                        break;

                    case 20:
                        Flat = "RROCK07";
                        Text = c3Text;
                        break;

                    case 30:
                        Flat = "RROCK17";
                        Text = c4Text;
                        break;

                    case 15:
                        Flat = "RROCK13";
                        Text = c5Text;
                        break;

                    case 31:
                        Flat = "RROCK19";
                        Text = c6Text;
                        break;

                    default:
                        break;
                }

                break;

            default:
                options.Music.StartMusic(Bgm.READ_M, PlayMode.Loop);
                Flat = "F_SKY1";
                Text = DoomInfo.Strings.C1TEXT;
                break;
        }

        Stage = 0;
        Count = 0;

        Scrolled = 0;
        ShowTheEnd = false;
        TheEndIndex = 0;
    }

    public IGameOptions Options { get; }
    public string Flat { get; }
    public string Text { get; }
    public int Count { get; private set; }

    public int Stage { get; private set; }

    // For cast.
    public string CastName => castOrder[castNumber].Name;

    public MobjStateDef CastState { get; private set; }

    // For bunny scroll.
    public int Scrolled { get; private set; }
    public int TheEndIndex { get; private set; }
    public bool ShowTheEnd { get; private set; }

    private sealed record CastInfo(string Name, MobjType Type);


    public UpdateResult Update()
    {
        updateResult = UpdateResult.None;

        // Check for skipping.
        if (Options.GameMode == GameMode.Commercial && Count > 50)
        {
            int i;

            // Go on to the next level.
            for (i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (Options.Players[i].Command.Buttons != 0)
                    break;
            }

            if (i < Player.MaxPlayerCount && Stage != 2)
            {
                if (Options.Map == 30)
                    StartCast();
                else
                    return UpdateResult.Completed;
            }
        }

        // Advance animation.
        Count++;

        if (Stage == 2)
        {
            UpdateCast();
            return updateResult;
        }

        if (Options.GameMode == GameMode.Commercial)
            return updateResult;

        if (Stage == 0 && Count > Text.Length * TextSpeed + TextWait)
        {
            Count = 0;
            Stage = 1;
            updateResult = UpdateResult.NeedWipe;
            if (Options.Episode == 3)
                Options.Music.StartMusic(Bgm.BUNNY, PlayMode.Loop);
        }

        if (Stage == 1 && Options.Episode == 3)
            BunnyScroll();

        return updateResult;
    }

    private void BunnyScroll()
    {
        Scrolled = 320 - (Count - 230) / 2;
        if (Scrolled > 320)
            Scrolled = 320;

        if (Scrolled < 0)
            Scrolled = 0;

        if (Count < 1130)
            return;

        ShowTheEnd = true;

        if (Count < 1180)
        {
            TheEndIndex = 0;
            return;
        }

        var stage = (Count - 1180) / 5;
        if (stage > 6)
            stage = 6;

        if (stage > TheEndIndex)
        {
            StartSound(Sfx.PISTOL);
            TheEndIndex = stage;
        }
    }


    private static readonly CastInfo[] castOrder =
    [
        new(DoomInfo.Strings.CC_ZOMBIE, MobjType.Possessed),
        new(DoomInfo.Strings.CC_SHOTGUN, MobjType.Shotguy),
        new(DoomInfo.Strings.CC_HEAVY, MobjType.Chainguy),
        new(DoomInfo.Strings.CC_IMP, MobjType.Troop),
        new(DoomInfo.Strings.CC_DEMON, MobjType.Sergeant),
        new(DoomInfo.Strings.CC_LOST, MobjType.Skull),
        new(DoomInfo.Strings.CC_CACO, MobjType.Head),
        new(DoomInfo.Strings.CC_HELL, MobjType.Knight),
        new(DoomInfo.Strings.CC_BARON, MobjType.Bruiser),
        new(DoomInfo.Strings.CC_ARACH, MobjType.Baby),
        new(DoomInfo.Strings.CC_PAIN, MobjType.Pain),
        new(DoomInfo.Strings.CC_REVEN, MobjType.Undead),
        new(DoomInfo.Strings.CC_MANCU, MobjType.Fatso),
        new(DoomInfo.Strings.CC_ARCH, MobjType.Vile),
        new(DoomInfo.Strings.CC_SPIDER, MobjType.Spider),
        new(DoomInfo.Strings.CC_CYBER, MobjType.Cyborg),
        new(DoomInfo.Strings.CC_HERO, MobjType.Player)
    ];

    private int castNumber;
    private int castTics;
    private int castFrames;
    private bool castDeath;
    private bool castOnMelee;
    private bool castAttacking;

    private void StartCast()
    {
        Stage = 2;

        castNumber = 0;
        CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castOrder[castNumber].Type].SeeState];
        castTics = CastState.Tics;
        castFrames = 0;
        castDeath = false;
        castOnMelee = false;
        castAttacking = false;

        updateResult = UpdateResult.NeedWipe;

        Options.Music.StartMusic(Bgm.EVIL, PlayMode.Loop);
    }

    private void UpdateCast()
    {
        // Not time to change state yet.
        if (--castTics > 0)
            return;

        var mobjInfoSpan = DoomInfo.MobjInfos.AsSpan();

        if (CastState.Tics == -1 || CastState.Next == MobjState.Null)
        {
            // Switch from deathstate to next monster.
            castNumber++;
            castDeath = false;
            if (castNumber == castOrder.Length)
                castNumber = 0;

            if (mobjInfoSpan[(int)castOrder[castNumber].Type].SeeSound != 0)
                StartSound(mobjInfoSpan[(int)castOrder[castNumber].Type].SeeSound);

            CastState = DoomInfo.States[(int)mobjInfoSpan[(int)castOrder[castNumber].Type].SeeState];
            castFrames = 0;
        }
        else
        {
            // Just advance to next state in animation.
            if (CastState == DoomInfo.States[(int)MobjState.PlayAtk1])
            {
                // Oh, gross hack!
                castAttacking = false;
                CastState = DoomInfo.States[(int)mobjInfoSpan[(int)castOrder[castNumber].Type].SeeState];
                castFrames = 0;
                goto stopAttack;
            }

            var st = CastState.Next;
            CastState = DoomInfo.States[(int)st];
            castFrames++;

            // Sound hacks....
            Sfx sfx = st switch
            {
                MobjState.PlayAtk1                                                => Sfx.DSHTGN,
                MobjState.PossAtk2                                                => Sfx.PISTOL,
                MobjState.SposAtk2                                                => Sfx.SHOTGN,
                MobjState.VileAtk2                                                => Sfx.VILATK,
                MobjState.SkelFist2                                               => Sfx.SKESWG,
                MobjState.SkelFist4                                               => Sfx.SKEPCH,
                MobjState.SkelMiss2                                               => Sfx.SKEATK,
                MobjState.FattAtk8 or MobjState.FattAtk5 or MobjState.FattAtk2    => Sfx.FIRSHT,
                MobjState.CposAtk2 or MobjState.CposAtk3 or MobjState.CposAtk4    => Sfx.SHOTGN,
                MobjState.TrooAtk3                                                => Sfx.CLAW,
                MobjState.SargAtk2                                                => Sfx.SGTATK,
                MobjState.BossAtk2 or MobjState.Bos2Atk2 or MobjState.HeadAtk2    => Sfx.FIRSHT,
                MobjState.SkullAtk2                                               => Sfx.SKLATK,
                MobjState.SpidAtk2 or MobjState.SpidAtk3                          => Sfx.SHOTGN,
                MobjState.BspiAtk2                                                => Sfx.PLASMA,
                MobjState.CyberAtk2 or MobjState.CyberAtk4 or MobjState.CyberAtk6 => Sfx.RLAUNC,
                MobjState.PainAtk3                                                => Sfx.SKLATK,
                _                                                                 => 0
            };

            if (sfx != 0)
                StartSound(sfx);
        }

        if (castFrames == 12)
        {
            // Go into attack frame.
            castAttacking = true;
            var infoIndex = (int)castOrder[castNumber].Type;
            var mobjInfo = mobjInfoSpan[infoIndex];
            var stateIndex = castOnMelee ? mobjInfo.MeleeState : mobjInfo.MissileState;
            CastState = DoomInfo.States[(int)stateIndex];

            castOnMelee ^= true;
            if (CastState == DoomInfo.States[(int)MobjState.Null])
            {
                stateIndex = castOnMelee ? mobjInfo.MeleeState : mobjInfo.MissileState;
                CastState = DoomInfo.States[(int)stateIndex];
            }
        }

        if (castAttacking)
        {
            if (castFrames == 24 ||
                CastState == DoomInfo.States[(int)mobjInfoSpan[(int)castOrder[castNumber].Type].SeeState])
            {
                castAttacking = false;
                CastState = DoomInfo.States[(int)mobjInfoSpan[(int)castOrder[castNumber].Type].SeeState];
                castFrames = 0;
            }
        }

    stopAttack:

        castTics = CastState.Tics;
        if (castTics == -1)
            castTics = 15;
    }

    public bool DoEvent(in DoomEvent e)
    {
        if (Stage != 2)
            return false;

        if (e.Type != EventType.KeyDown)
            return false;

        // Already in dying frames.
        if (castDeath)
            return true;

        var mobjInfoIndex = (int)castOrder[castNumber].Type;
        var mobjInfo = DoomInfo.MobjInfos[mobjInfoIndex];

        // Go into death frame.
        castDeath = true;
        CastState = DoomInfo.States[(int)mobjInfo.DeathState];
        castTics = CastState.Tics;
        castFrames = 0;
        castAttacking = false;

        if (mobjInfo.DeathSound != 0)
            StartSound(mobjInfo.DeathSound);

        return true;
    }

    private void StartSound(Sfx sfx) => Options.Sound.StartSound(sfx);
}