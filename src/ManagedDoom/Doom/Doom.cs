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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ManagedDoom.Audio;
using ManagedDoom.Config;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Menu;
using ManagedDoom.Doom.Opening;
using ManagedDoom.Silk;
using ManagedDoom.UserInput;
using ManagedDoom.Video;

namespace ManagedDoom.Doom;

public sealed class Doom
{
    private readonly ICommandLineArgs args;
    private readonly ConfigValues config;
    private readonly IGameContent content;
    private readonly IVideo video;
    private readonly ISound sound;
    private readonly IUserInput userInput;

    private readonly List<DoomEvent> events;

    private readonly TicCmd[] cmds;

    private DoomState nextState;
    private bool needWipe;

    private bool sendPause;

    private bool quit;

    private bool mouseGrabbed;

    public Doom(
        ICommandLineArgs args,
        ISilkConfig silkConfig,
        IGameContent content,
        IVideo? video,
        IAudioFactory audioFactory,
        IUserInput? userInput)
    {
        video ??= NullVideo.GetInstance();
        userInput ??= NullUserInput.GetInstance();

        this.args = args;
        this.config = silkConfig.Config.Values;
        this.content = content;
        this.video = video;
        this.sound = audioFactory.GetSound();
        this.userInput = userInput;

        events = [];

        Options = new GameOptions(args, content)
        {
            Video = video,
            Sound = sound,
            Music = audioFactory.GetMusic(),
            UserInput = userInput
        };

        Menu = new DoomMenu(this);

        Opening = new OpeningSequence(content, Options);

        cmds = new TicCmd[Player.MaxPlayerCount];
        for (var i = 0; i < cmds.Length; i++)
            cmds[i] = new TicCmd();

        Game = new DoomGame(content, Options);

        WipeEffect = WipeEffect.CreateInstance(video.WipeBandCount, video.WipeHeight);
        Wiping = false;

        State = DoomState.None;
        nextState = DoomState.Opening;
        needWipe = false;

        sendPause = false;

        quit = false;
        QuitMessage = null;

        mouseGrabbed = false;

        CheckGameArgs();
    }

    private void CheckGameArgs()
    {
        if (args.Warp.Present)
        {
            nextState = DoomState.Game;
            Options.Episode = args.Warp.Value!.Episode;
            Options.Map = args.Warp.Value.Map;
            Game.DeferInitNew();
        }
        else if (args.Episode.Present)
        {
            nextState = DoomState.Game;
            Options.Episode = args.Episode.Value;
            Options.Map = 1;
            Game.DeferInitNew();
        }

        if (args.Skill.Present)
            Options.Skill = (GameSkill)(args.Skill.Value - 1);

        if (args.DeathMatch.Present)
            Options.Deathmatch = 1;

        if (args.AltDeath.Present)
            Options.Deathmatch = 2;

        Options.FastMonsters = args.Fast.Present;
        Options.RespawnMonsters = args.Respawn.Present;
        Options.NoMonsters = args.NoMonsters.Present;

        if (args.LoadGame.Present)
        {
            nextState = DoomState.Game;
            Game.LoadGame(args.LoadGame.Value);
        }

        if (args.PlayDemo.Present)
        {
            nextState = DoomState.DemoPlayback;
            DemoPlayback = new DemoPlayback(args, content, Options, args.PlayDemo.Value!);
        }

        if (args.TimeDemo.Present)
        {
            nextState = DoomState.DemoPlayback;
            DemoPlayback = new DemoPlayback(args, content, Options, args.TimeDemo.Value!);
        }
    }

    public void NewGame(GameSkill skill, int episode, int map)
    {
        Game.DeferInitNew(skill, episode, map);
        nextState = DoomState.Game;
    }

    public void EndGame()
    {
        nextState = DoomState.Opening;
    }

    private void DoEvents()
    {
        if (Wiping)
            return;

        foreach (var e in events)
        {
            if (Menu.DoEvent(in e))
                continue;

            if (e.Type == EventType.KeyDown)
            {
                if (CheckFunctionKey(e.Key))
                    continue;
            }

            if (State == DoomState.Game)
            {
                if (e is { Key: DoomKey.Pause, Type: EventType.KeyDown })
                {
                    sendPause = true;
                    continue;
                }

                _ = Game.DoEvent(in e);
            }
            else if (State == DoomState.DemoPlayback && DemoPlayback is not null)
                DemoPlayback.DoEvent(in e);
        }

        events.Clear();
    }

    private bool CheckFunctionKey(DoomKey key)
    {
        switch (key)
        {
            case DoomKey.F1:
                Menu.ShowHelpScreen();
                return true;

            case DoomKey.F2:
                Menu.ShowSaveScreen();
                return true;

            case DoomKey.F3:
                Menu.ShowLoadScreen();
                return true;

            case DoomKey.F4:
                Menu.ShowVolumeControl();
                return true;

            case DoomKey.F6:
                Menu.QuickSave();
                return true;

            case DoomKey.F7:
                if (State == DoomState.Game)
                    Menu.EndGame();
                else
                    Options.Sound.StartSound(Sfx.OOF);

                return true;

            case DoomKey.F8:
                video.DisplayMessage = !video.DisplayMessage;
                if (State == DoomState.Game && Game.State == GameState.Level)
                {
                    string msg = video.DisplayMessage ? DoomInfo.Strings.MSGON : DoomInfo.Strings.MSGOFF;
                    Game.World.ConsolePlayer.SendMessage(msg);
                }

                Menu.StartSound(Sfx.SWTCHN);
                return true;

            case DoomKey.F9:
                Menu.QuickLoad();
                return true;

            case DoomKey.F10:
                Menu.Quit();
                return true;

            case DoomKey.F11:
                var gcl = video.GammaCorrectionLevel;
                gcl++;
                if (gcl > video.MaxGammaCorrectionLevel)
                    gcl = 0;

                video.GammaCorrectionLevel = gcl;
                if (State == DoomState.Game && Game.State == GameState.Level)
                {
                    var msg = gcl == 0 ? DoomInfo.Strings.GAMMALVL0 : $"Gamma correction level {gcl}";
                    Game.World.ConsolePlayer.SendMessage(msg);
                }

                return true;

            case DoomKey.Add:
            case DoomKey.Quote:
            case DoomKey.Equal:
                if (State == DoomState.Game && Game is { State: GameState.Level, World.AutoMap.Visible: true })
                    return false;

                video.WindowSize = System.Math.Min(video.WindowSize + 1, video.MaxWindowSize);
                sound.StartSound(Sfx.STNMOV);
                return true;

            case DoomKey.Subtract:
            case DoomKey.Hyphen:
            case DoomKey.Semicolon:
                if (State == DoomState.Game && Game is { State: GameState.Level, World.AutoMap.Visible: true })
                    return false;

                video.WindowSize = System.Math.Max(video.WindowSize - 1, 0);
                sound.StartSound(Sfx.STNMOV);
                return true;

            default:
                return false;
        }
    }

    public UpdateResult Update()
    {
        DoEvents();

        if (!Wiping)
        {
            Menu.Update();

            if (nextState != State)
            {
                if (nextState != DoomState.Opening)
                    Opening.Reset();

                if (nextState != DoomState.DemoPlayback)
                    DemoPlayback = null;

                State = nextState;
            }

            if (quit)
                return UpdateResult.Completed;

            if (needWipe)
            {
                needWipe = false;
                StartWipe();
            }
        }

        if (!Wiping)
        {
            switch (State)
            {
                case DoomState.Opening:
                    if (Opening.Update() == UpdateResult.NeedWipe)
                        StartWipe();

                    break;

                case DoomState.DemoPlayback:
                    if (DemoPlayback is null)
                        throw new Exception("Demo playback is not initialized!");

                    var result = DemoPlayback.Update();
                    switch (result)
                    {
                        case UpdateResult.NeedWipe:
                            StartWipe();
                            break;
                        case UpdateResult.Completed:
                            Quit($"FPS: {DemoPlayback.Fps:0.0}");
                            break;
                    }

                    break;

                case DoomState.Game:
                    userInput.BuildTicCmd(cmds[Options.ConsolePlayer]);
                    if (sendPause)
                    {
                        sendPause ^= true;
                        cmds[Options.ConsolePlayer].Buttons |= TicCmdButtons.Special | TicCmdButtons.Pause;
                    }

                    if (Game.Update(cmds) == UpdateResult.NeedWipe)
                        StartWipe();
                    break;

                default:
                    throw new Exception("Invalid application state!");
            }
        }

        if (Wiping && WipeEffect.Update() == UpdateResult.Completed)
            Wiping = false;

        sound.Update();

        CheckMouseState();

        return UpdateResult.None;
    }

    private void CheckMouseState()
    {
        var mouseShouldBeGrabbed = ShouldMouseBeGrabbed();

        if (mouseGrabbed)
        {
            if (!mouseShouldBeGrabbed)
            {
                userInput.ReleaseMouse();
                mouseGrabbed = false;
            }
        }
        else
        {
            if (mouseShouldBeGrabbed)
            {
                userInput.GrabMouse();
                mouseGrabbed = true;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ShouldMouseBeGrabbed()
    {
        return video.HasFocus() && (config.VideoFullscreen || State == DoomState.Game && !Menu.Active);
    }

    private void StartWipe()
    {
        WipeEffect.Start();
        video.InitializeWipe();
        Wiping = true;
    }

    public void PauseGame()
    {
        sendPause = !sendPause && State == DoomState.Game && Game is { State: GameState.Level, Paused: false };
    }

    public void ResumeGame()
    {
        sendPause = !sendPause && State == DoomState.Game && Game is { State: GameState.Level, Paused: true };
    }

    public bool SaveGame(int slotNumber, string description)
    {
        var shouldSave = State == DoomState.Game && Game.State == GameState.Level;
        if (shouldSave)
            Game.SaveGame(slotNumber, description);

        return shouldSave;
    }

    public void LoadGame(int slotNumber)
    {
        Game.LoadGame(slotNumber);
        nextState = DoomState.Game;
    }

    public void Quit()
    {
        quit = true;
    }

    private void Quit(string message)
    {
        quit = true;
        QuitMessage = message;
    }

    public void PostEvent(in DoomEvent e)
    {
        if (events.Count < 64)
            events.Add(e);
    }

    public DoomState State { get; private set; }

    public IOpeningSequence Opening { get; }

    public DemoPlayback? DemoPlayback { get; private set; }

    public GameOptions Options { get; }

    public DoomGame Game { get; }

    public DoomMenu Menu { get; }

    public WipeEffect WipeEffect { get; }

    public bool Wiping { get; private set; }

    public string? QuitMessage { get; private set; }
}