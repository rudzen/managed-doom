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
using System.Collections.Generic;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;
using ManagedDoom.Silk;

namespace ManagedDoom.Doom.Intermission;

public sealed class Intermission
{
    private const int ShowNextLocDelay = 4;

    // Contains information passed into intermission.
    private readonly PlayerScores[] scores;

    // Used to accelerate or skip a stage.
    private bool accelerateStage;

    // Specifies current state.

    private readonly int[] killCount;
    private readonly int[] itemCount;
    private readonly int[] secretCount;
    private readonly int[] fragCount;
    private int pauseCount;

    private int spState;

    private int ngState;

    private int dmState;

    // Used for general timing.
    private int count;

    // Used for timing of background animation.
    private int bgCount;

    private bool completed;

    public Intermission(GameOptions options, IntermissionInfo info)
    {
        this.Options = options;
        this.Info = info;

        scores = info.PlayerScores;

        killCount = new int[Player.MaxPlayerCount];
        itemCount = new int[Player.MaxPlayerCount];
        secretCount = new int[Player.MaxPlayerCount];
        fragCount = new int[Player.MaxPlayerCount];

        DeathmatchFrags = new int[Player.MaxPlayerCount][];
        for (var i = 0; i < Player.MaxPlayerCount; i++)
            DeathmatchFrags[i] = new int[Player.MaxPlayerCount];

        DeathmatchTotals = new int[Player.MaxPlayerCount];

        if (options.Deathmatch != 0)
            InitDeathmatchStats();
        else if (options.NetGame)
            InitNetGameStats();
        else
            InitSinglePLayerStats();

        completed = false;
    }

    public GameOptions Options { get; }
    public IntermissionInfo Info { get; }
    public IntermissionState State { get; private set; }
    public ReadOnlySpan<int> KillCount => killCount.AsSpan();
    public ReadOnlySpan<int> ItemCount => itemCount.AsSpan();
    public ReadOnlySpan<int> SecretCount => secretCount.AsSpan();
    public ReadOnlySpan<int> FragCount => fragCount.AsSpan();
    public int TimeCount { get; private set; }
    public int ParCount { get; private set; }
    public int[][] DeathmatchFrags { get; }
    public int[] DeathmatchTotals { get; }
    public bool DoFrags { get; private set; }
    public DoomRandom Random { get; private set; }
    public Animation[]? Animations { get; private set; }
    public bool ShowYouAreHere { get; private set; }

    ////////////////////////////////////////////////////////////
    // Initialization
    ////////////////////////////////////////////////////////////

    private void InitSinglePLayerStats()
    {
        State = IntermissionState.StatCount;
        accelerateStage = false;
        spState = 1;
        killCount[0] = itemCount[0] = secretCount[0] = -1;
        TimeCount = ParCount = -1;
        pauseCount = GameConst.TicRate;

        InitAnimatedBack();
    }

    private void InitNetGameStats()
    {
        State = IntermissionState.StatCount;
        accelerateStage = false;
        ngState = 1;
        pauseCount = GameConst.TicRate;

        var frags = 0;
        for (var i = 0; i < Options.Players.Length; i++)
        {
            if (!Options.Players[i].InGame)
                continue;

            killCount[i] = itemCount[i] = secretCount[i] = fragCount[i] = 0;

            frags += GetFragSum(i);
        }

        DoFrags = frags > 0;

        InitAnimatedBack();
    }

    private void InitDeathmatchStats()
    {
        State = IntermissionState.StatCount;
        accelerateStage = false;
        dmState = 1;
        pauseCount = GameConst.TicRate;

        var players = Options.Players.AsSpan();
        for (var i = 0; i < players.Length; i++)
        {
            if (!players[i].InGame)
                continue;

            for (var j = 0; j < players.Length; j++)
            {
                if (players[j].InGame)
                    DeathmatchFrags[i][j] = 0;
            }

            DeathmatchTotals[i] = 0;
        }

        InitAnimatedBack();
    }

    private void InitNoState()
    {
        State = IntermissionState.NoState;
        accelerateStage = false;
        count = 10;
    }

    private void InitShowNextLoc()
    {
        State = IntermissionState.ShowNextLoc;
        accelerateStage = false;
        count = ShowNextLocDelay * GameConst.TicRate;

        InitAnimatedBack();
    }

    private void InitAnimatedBack()
    {
        if (Options.GameMode == GameMode.Commercial)
            return;

        if (Info.Episode > 2)
            return;

        if (Animations == null)
        {
            Animations = new Animation[AnimationInfo.Episodes[Info.Episode].Length];
            for (var i = 0; i < Animations.Length; i++)
                Animations[i] = new Animation(this, AnimationInfo.Episodes[Info.Episode][i], i);

            Random = new DoomRandom();
        }

        foreach (var animation in Animations)
            animation.Reset(bgCount);
    }

    ////////////////////////////////////////////////////////////
    // Update
    ////////////////////////////////////////////////////////////

    public UpdateResult Update()
    {
        // Counter for general background animation.
        bgCount++;

        CheckForAccelerate();

        if (bgCount == 1)
        {
            // intermission music
            var bgm = Options.GameMode == GameMode.Commercial ? Bgm.DM2INT : Bgm.INTER;
            Options.Music.StartMusic(bgm, PlayMode.Loop);
        }

        switch (State)
        {
            case IntermissionState.StatCount:
                if (Options.Deathmatch != 0)
                    UpdateDeathmatchStats();
                else if (Options.NetGame)
                    UpdateNetGameStats();
                else
                    UpdateSinglePlayerStats();

                break;

            case IntermissionState.ShowNextLoc:
                UpdateShowNextLoc();
                break;

            case IntermissionState.NoState:
                UpdateNoState();
                break;
        }

        if (completed)
            return UpdateResult.Completed;

        return bgCount == 1 ? UpdateResult.NeedWipe : UpdateResult.None;
    }

    private void UpdateSinglePlayerStats()
    {
        UpdateAnimatedBack();

        if (accelerateStage && spState != 10)
        {
            accelerateStage = false;
            killCount[0] = (scores[0].KillCount * 100) / Info.MaxKillCount;
            itemCount[0] = (scores[0].ItemCount * 100) / Info.MaxItemCount;
            secretCount[0] = (scores[0].SecretCount * 100) / Info.MaxSecretCount;
            TimeCount = scores[0].Time / GameConst.TicRate;
            ParCount = Info.ParTime / GameConst.TicRate;
            StartSound(Sfx.BAREXP);
            spState = 10;
        }

        if (spState == 2)
        {
            killCount[0] += 2;

            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            if (killCount[0] >= (scores[0].KillCount * 100) / Info.MaxKillCount)
            {
                killCount[0] = (scores[0].KillCount * 100) / Info.MaxKillCount;
                StartSound(Sfx.BAREXP);
                spState++;
            }
        }
        else if (spState == 4)
        {
            itemCount[0] += 2;

            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            if (itemCount[0] >= (scores[0].ItemCount * 100) / Info.MaxItemCount)
            {
                itemCount[0] = (scores[0].ItemCount * 100) / Info.MaxItemCount;
                StartSound(Sfx.BAREXP);
                spState++;
            }
        }
        else if (spState == 6)
        {
            secretCount[0] += 2;

            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            if (secretCount[0] >= (scores[0].SecretCount * 100) / Info.MaxSecretCount)
            {
                secretCount[0] = (scores[0].SecretCount * 100) / Info.MaxSecretCount;
                StartSound(Sfx.BAREXP);
                spState++;
            }
        }

        else if (spState == 8)
        {
            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            TimeCount += 3;

            if (TimeCount >= scores[0].Time / GameConst.TicRate)
                TimeCount = scores[0].Time / GameConst.TicRate;

            ParCount += 3;

            if (ParCount >= Info.ParTime / GameConst.TicRate)
            {
                ParCount = Info.ParTime / GameConst.TicRate;

                if (TimeCount >= scores[0].Time / GameConst.TicRate)
                {
                    StartSound(Sfx.BAREXP);
                    spState++;
                }
            }
        }
        else if (spState == 10)
        {
            if (accelerateStage)
            {
                StartSound(Sfx.SGCOCK);

                if (Options.GameMode == GameMode.Commercial)
                    InitNoState();
                else
                    InitShowNextLoc();
            }
        }
        else if ((spState & 1) != 0)
        {
            if (--pauseCount == 0)
            {
                spState++;
                pauseCount = GameConst.TicRate;
            }
        }
    }

    private void UpdateNetGameStats()
    {
        UpdateAnimatedBack();

        bool stillTicking;

        if (accelerateStage && ngState != 10)
        {
            accelerateStage = false;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!Options.Players[i].InGame)
                    continue;

                killCount[i] = (scores[i].KillCount * 100) / Info.MaxKillCount;
                itemCount[i] = (scores[i].ItemCount * 100) / Info.MaxItemCount;
                secretCount[i] = (scores[i].SecretCount * 100) / Info.MaxSecretCount;
            }

            StartSound(Sfx.BAREXP);

            ngState = 10;
        }

        if (ngState == 2)
        {
            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            stillTicking = false;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!Options.Players[i].InGame)
                    continue;

                killCount[i] += 2;
                if (killCount[i] >= (scores[i].KillCount * 100) / Info.MaxKillCount)
                    killCount[i] = (scores[i].KillCount * 100) / Info.MaxKillCount;
                else
                    stillTicking = true;
            }

            if (!stillTicking)
            {
                StartSound(Sfx.BAREXP);
                ngState++;
            }
        }
        else if (ngState == 4)
        {
            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            stillTicking = false;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!Options.Players[i].InGame)
                    continue;

                itemCount[i] += 2;
                if (itemCount[i] >= (scores[i].ItemCount * 100) / Info.MaxItemCount)
                    itemCount[i] = (scores[i].ItemCount * 100) / Info.MaxItemCount;
                else
                    stillTicking = true;
            }

            if (!stillTicking)
            {
                StartSound(Sfx.BAREXP);
                ngState++;
            }
        }
        else if (ngState == 6)
        {
            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            stillTicking = false;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!Options.Players[i].InGame)
                    continue;

                secretCount[i] += 2;
                if (secretCount[i] >= (scores[i].SecretCount * 100) / Info.MaxSecretCount)
                    secretCount[i] = (scores[i].SecretCount * 100) / Info.MaxSecretCount;
                else
                    stillTicking = true;
            }

            if (!stillTicking)
            {
                StartSound(Sfx.BAREXP);
                if (DoFrags)
                    ngState++;
                else
                    ngState += 3;
            }
        }
        else if (ngState == 8)
        {
            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            stillTicking = false;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!Options.Players[i].InGame)
                    continue;

                fragCount[i] += 1;
                var sum = GetFragSum(i);
                if (fragCount[i] >= sum)
                    fragCount[i] = sum;
                else
                    stillTicking = true;
            }

            if (!stillTicking)
            {
                StartSound(Sfx.PLDETH);
                ngState++;
            }
        }
        else if (ngState == 10)
        {
            if (accelerateStage)
            {
                StartSound(Sfx.SGCOCK);

                if (Options.GameMode == GameMode.Commercial)
                    InitNoState();
                else
                    InitShowNextLoc();
            }
        }
        else if ((ngState & 1) != 0)
        {
            if (--pauseCount == 0)
            {
                ngState++;
                pauseCount = GameConst.TicRate;
            }
        }
    }

    private void UpdateDeathmatchStats()
    {
        UpdateAnimatedBack();

        if (accelerateStage && dmState != 4)
        {
            accelerateStage = false;

            var players = Options.Players.AsSpan();
            for (var i = 0; i < players.Length; i++)
            {
                if (!players[i].InGame)
                    continue;

                for (var j = 0; j < players.Length; j++)
                {
                    if (players[j].InGame)
                        DeathmatchFrags[i][j] = scores[i].Frags[j];
                }

                DeathmatchTotals[i] = GetFragSum(i);
            }

            StartSound(Sfx.BAREXP);

            dmState = 4;
        }

        if (dmState == 2)
        {
            if ((bgCount & 3) == 0)
                StartSound(Sfx.PISTOL);

            var stillTicking = false;

            for (var i = 0; i < Options.Players.Length; i++)
            {
                if (!Options.Players[i].InGame)
                    continue;

                for (var j = 0; j < Options.Players.Length; j++)
                {
                    if (!Options.Players[j].InGame || DeathmatchFrags[i][j] == scores[i].Frags[j]) continue;
                    if (scores[i].Frags[j] < 0)
                        DeathmatchFrags[i][j]--;
                    else
                        DeathmatchFrags[i][j]++;

                    if (DeathmatchFrags[i][j] > 99)
                        DeathmatchFrags[i][j] = 99;

                    if (DeathmatchFrags[i][j] < -99)
                        DeathmatchFrags[i][j] = -99;

                    stillTicking = true;
                }

                DeathmatchTotals[i] = GetFragSum(i);

                if (DeathmatchTotals[i] > 99)
                    DeathmatchTotals[i] = 99;

                if (DeathmatchTotals[i] < -99)
                    DeathmatchTotals[i] = -99;
            }

            if (!stillTicking)
            {
                StartSound(Sfx.BAREXP);
                dmState++;
            }
        }
        else if (dmState == 4)
        {
            if (accelerateStage)
            {
                StartSound(Sfx.SLOP);

                if (Options.GameMode == GameMode.Commercial)
                    InitNoState();
                else
                    InitShowNextLoc();
            }
        }
        else if ((dmState & 1) != 0)
        {
            if (--pauseCount == 0)
            {
                dmState++;
                pauseCount = GameConst.TicRate;
            }
        }
    }

    private void UpdateShowNextLoc()
    {
        UpdateAnimatedBack();

        if (--count == 0 || accelerateStage)
            InitNoState();
        else
            ShowYouAreHere = (count & 31) < 20;
    }

    private void UpdateNoState()
    {
        UpdateAnimatedBack();
        completed = --count == 0;
    }

    private void UpdateAnimatedBack()
    {
        if (Options.GameMode == GameMode.Commercial)
            return;

        if (Info.Episode > 2)
            return;

        if (Animations is null)
            return;

        foreach (var a in Animations)
            a.Update(bgCount);
    }

    ////////////////////////////////////////////////////////////
    // Check for button press
    ////////////////////////////////////////////////////////////

    private void CheckForAccelerate()
    {
        // Check for button presses to skip delays.
        for (var i = 0; i < Player.MaxPlayerCount; i++)
        {
            var player = Options.Players[i];
            if (player.InGame)
            {
                if ((player.Command.Buttons & TicCommandButtons.Attack) != 0)
                {
                    if (!player.AttackDown)
                        accelerateStage = true;

                    player.AttackDown = true;
                }
                else
                    player.AttackDown = false;

                if ((player.Command.Buttons & TicCommandButtons.Use) != 0)
                {
                    if (!player.UseDown)
                        accelerateStage = true;

                    player.UseDown = true;
                }
                else
                    player.UseDown = false;
            }
        }
    }

    ////////////////////////////////////////////////////////////
    // Miscellaneous functions
    ////////////////////////////////////////////////////////////

    private int GetFragSum(int playerNumber)
    {
        var frags = 0;

        for (var i = 0; i < Options.Players.Length; i++)
        {
            if (Options.Players[i].InGame && i != playerNumber)
                frags += scores[playerNumber].Frags[i];
        }

        frags -= scores[playerNumber].Frags[playerNumber];

        return frags;
    }

    private void StartSound(Sfx sfx) => Options.Sound.StartSound(sfx);
}