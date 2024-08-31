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

namespace ManagedDoom.Doom.Opening;

public sealed class OpeningSequence : IOpeningSequence
{
    private readonly IGameContent content;
    private readonly IGameOptions options;

    private int currentStage;
    private int nextStage;

    private int count;
    private int timer;

    private readonly TicCommand[] ticCommands;
    private Demo? demo;

    private bool reset;

    public OpeningSequence(IGameContent content, IGameOptions options)
    {
        this.content = content;
        this.options = options;

        ticCommands = new TicCommand[Player.MaxPlayerCount];
        for (var i = 0; i < ticCommands.Length; i++)
            ticCommands[i] = new TicCommand();

        currentStage = 0;
        nextStage = 0;

        reset = false;

        StartTitleScreen();
    }

    public OpeningSequenceState State { get; private set; }
    public DoomGame? DemoGame { get; private set; }

    public void Reset()
    {
        currentStage = 0;
        nextStage = 0;

        demo = null;
        DemoGame = null;

        reset = true;

        StartTitleScreen();
    }

    public UpdateResult Update()
    {
        var updateResult = UpdateResult.None;

        if (nextStage != currentStage)
        {
            switch (nextStage)
            {
                case 0:
                    StartTitleScreen();
                    break;
                case 1:
                    StartDemo("DEMO1");
                    break;
                case 2:
                    StartCreditScreen();
                    break;
                case 3:
                    StartDemo("DEMO2");
                    break;
                case 4:
                    StartTitleScreen();
                    break;
                case 5:
                    StartDemo("DEMO3");
                    break;
                case 6:
                    StartCreditScreen();
                    break;
                case 7:
                    StartDemo("DEMO4");
                    break;
            }

            currentStage = nextStage;
            updateResult = UpdateResult.NeedWipe;
        }

        switch (currentStage)
        {
            case 0:
                count++;
                if (count == timer)
                    nextStage = 1;

                break;

            case 1:
                if (!demo.ReadCmd(ticCommands))
                    nextStage = 2;
                else
                    DemoGame.Update(ticCommands);

                break;

            case 2:
                count++;
                if (count == timer)
                    nextStage = 3;

                break;

            case 3:
                if (!demo.ReadCmd(ticCommands))
                    nextStage = 4;
                else
                    DemoGame.Update(ticCommands);

                break;

            case 4:
                count++;
                if (count == timer)
                    nextStage = 5;

                break;

            case 5:
                if (!demo.ReadCmd(ticCommands))
                    nextStage = content.Wad.GetLumpNumber("DEMO4") == -1 ? 0 : 6;
                else
                    DemoGame.Update(ticCommands);

                break;

            case 6:
                count++;
                if (count == timer)
                    nextStage = 7;

                break;

            case 7:
                if (!demo.ReadCmd(ticCommands))
                    nextStage = 0;
                else
                    DemoGame.Update(ticCommands);

                break;
        }

        if (State == OpeningSequenceState.Title && count == 1)
        {
            var bgm = options.GameMode switch
            {
                GameMode.Commercial => Bgm.DM2TTL,
                _                   => Bgm.INTRO
            };
            options.Music.StartMusic(bgm, PlayMode.Once);
        }

        if (reset)
        {
            reset = false;
            return UpdateResult.NeedWipe;
        }

        return updateResult;
    }

    private void StartTitleScreen()
    {
        State = OpeningSequenceState.Title;
        count = 0;
        timer = options.GameMode.DemoTimer();
    }

    private void StartCreditScreen()
    {
        State = OpeningSequenceState.Credit;

        count = 0;
        timer = 200;
    }

    private void StartDemo(string lump)
    {
        State = OpeningSequenceState.Demo;

        demo = new Demo(content.Wad.ReadLump(lump))
        {
            Options =
            {
                GameVersion = options.GameVersion,
                GameMode = options.GameMode,
                MissionPack = options.MissionPack,
                Video = options.Video,
                Sound = options.Sound,
                Music = options.Music
            }
        };

        DemoGame = new DoomGame(content, demo.Options);
        DemoGame.DeferInitNew();
    }
}