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
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ManagedDoom.Config;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Intermission;
using ManagedDoom.Extensions;
using ManagedDoom.Silk;

namespace ManagedDoom.Doom.Game;

public sealed class DoomGame
{
    private readonly GameContent content;

    private GameAction gameAction;

    private int loadGameSlotNumber;
    private int saveGameSlotNumber;
    private string? saveGameDescription;

    private enum GameAction
    {
        Nothing,
        LoadLevel,
        NewGame,
        LoadGame,
        SaveGame,
        Completed,
        Victory,
        WorldDone
    }

    public DoomGame(GameContent content, GameOptions options)
    {
        this.content = content;
        this.Options = options;

        gameAction = GameAction.Nothing;

        GameTic = 0;
    }

    public GameOptions Options { get; }

    public GameState State { get; private set; }

    public int GameTic { get; private set; }

    public World.World World { get; private set; }

    public Intermission.Intermission? Intermission { get; private set; }

    public Finale Finale { get; private set; }

    public bool Paused { get; private set; }

    ////////////////////////////////////////////////////////////
    // Public methods to control the game state
    ////////////////////////////////////////////////////////////

    /// <summary>
    /// Start a new game.
    /// Can be called by the startup code or the menu task.
    /// </summary>
    public void DeferInitNew()
    {
        gameAction = GameAction.NewGame;
    }

    /// <summary>
    /// Start a new game.
    /// Can be called by the startup code or the menu task.
    /// </summary>
    public void DeferInitNew(GameSkill skill, int episode, int map)
    {
        Options.Skill = skill;
        Options.Episode = episode;
        Options.Map = map;
        gameAction = GameAction.NewGame;
    }

    /// <summary>
    /// Load the saved game at the given slot number.
    /// Can be called by the startup code or the menu task.
    /// </summary>
    public void LoadGame(int slotNumber)
    {
        loadGameSlotNumber = slotNumber;
        gameAction = GameAction.LoadGame;
    }

    /// <summary>
    /// Save the game at the given slot number.
    /// Can be called by the startup code or the menu task.
    /// </summary>
    public void SaveGame(int slotNumber, string description)
    {
        saveGameSlotNumber = slotNumber;
        saveGameDescription = description;
        gameAction = GameAction.SaveGame;
    }

    /// <summary>
    /// Advance the game one frame.
    /// </summary>
    public UpdateResult Update(ReadOnlySpan<TicCommand> cmds)
    {
        // Do player reborns if needed.
        var players = Options.Players.AsSpan();
        ref var playersRef = ref MemoryMarshal.GetReference(players);

        for (var i = 0; i < players.Length; i++)
        {
            ref var player = ref Unsafe.Add(ref playersRef, i);
            if (player is { InGame: true, PlayerState: PlayerState.Reborn })
                DoReborn(i);
        }

        // Do things to change the game state.
        while (gameAction != GameAction.Nothing)
        {
            switch (gameAction)
            {
                case GameAction.LoadLevel:
                    DoLoadLevel();
                    break;
                case GameAction.NewGame:
                    DoNewGame();
                    break;
                case GameAction.LoadGame:
                    DoLoadGame();
                    break;
                case GameAction.SaveGame:
                    DoSaveGame();
                    break;
                case GameAction.Completed:
                    DoCompleted();
                    break;
                case GameAction.Victory:
                    DoFinale();
                    break;
                case GameAction.WorldDone:
                    DoWorldDone();
                    break;
                case GameAction.Nothing:
                    break;
            }
        }

        for (var i = 0; i < players.Length; i++)
        {
            ref var player = ref Unsafe.Add(ref playersRef, i);

            if (!player.InGame)
                continue;

            var cmd = player.Command;
            cmd.CopyFrom(cmds[i]);

            /*
                if (demorecording)
                {
                    G_WriteDemoTiccmd(cmd);
                }
                */

            // Check for turbo cheats.
            if (cmd.ForwardMove > GameConst.TurboThreshold &&
                (World.LevelTime & 31) == 0 &&
                ((World.LevelTime >> 5) & 3) == i)
            {
                ref var consolePlayer = ref Unsafe.Add(ref playersRef, Options.ConsolePlayer);
                consolePlayer.SendMessage($"{player.Name} is turbo!");
            }
        }

        // Check for special buttons.
        for (var i = 0; i < players.Length; i++)
        {
            ref var player = ref Unsafe.Add(ref playersRef, i);
            if (!player.InGame)
                continue;

            if ((player.Command.Buttons & TicCommandButtons.Special) == 0)
                continue;

            if ((player.Command.Buttons & TicCommandButtons.SpecialMask) != TicCommandButtons.Pause)
                continue;

            Paused ^= true;
            if (Paused)
                Options.Sound.Pause();
            else
                Options.Sound.Resume();
        }

        // Do main actions.
        var result = UpdateResult.None;
        switch (State)
        {
            case GameState.Level:
                if (!Paused || World.FirstTicIsNotYetDone)
                {
                    result = World.Update();
                    if (result == UpdateResult.Completed)
                        gameAction = GameAction.Completed;
                }

                break;

            case GameState.Intermission:
                result = Intermission!.Update();
                if (result == UpdateResult.Completed)
                {
                    gameAction = GameAction.WorldDone;
                    Unsafe.Add(ref playersRef, Options.ConsolePlayer).DidSecret = World.SecretExit;

                    if (Options.GameMode == GameMode.Commercial)
                    {
                        switch (Options.Map)
                        {
                            case 6:
                            case 11:
                            case 20:
                            case 30:
                                DoFinale();
                                result = UpdateResult.NeedWipe;
                                break;

                            case 15:
                            case 31:
                                if (World.SecretExit)
                                {
                                    DoFinale();
                                    result = UpdateResult.NeedWipe;
                                }

                                break;
                        }
                    }
                }

                break;

            case GameState.Finale:
                result = Finale.Update();
                if (result == UpdateResult.Completed)
                    gameAction = GameAction.WorldDone;

                break;
        }

        GameTic++;

        return result == UpdateResult.NeedWipe ? UpdateResult.NeedWipe : UpdateResult.None;
    }


    ////////////////////////////////////////////////////////////
    // Actual game actions
    ////////////////////////////////////////////////////////////

    // It seems that these methods should not be called directly
    // from outside for some reason.
    // So if you want to start a new game or do load / save, use
    // the following public methods.
    //
    //     - DeferedInitNew
    //     - LoadGame
    //     - SaveGame

    private void DoLoadLevel()
    {
        gameAction = GameAction.Nothing;

        State = GameState.Level;

        var players = Options.Players.AsSpan();
        ref var playersRef = ref MemoryMarshal.GetReference(players);

        for (var i = 0; i < players.Length; i++)
        {
            ref var player = ref Unsafe.Add(ref playersRef, i);
            if (player is { InGame: true, PlayerState: PlayerState.Dead })
                player.PlayerState = PlayerState.Reborn;

            player.Frags.AsSpan().Clear();
        }

        Intermission = null;

        Options.Sound.Reset();

        World = new World.World(content, Options, this);

        Options.UserInput.Reset();
    }

    private void DoNewGame()
    {
        gameAction = GameAction.Nothing;

        InitNew(Options.Skill, Options.Episode, Options.Map);
    }

    private void DoLoadGame()
    {
        gameAction = GameAction.Nothing;

        var directory = ConfigUtilities.GetExeDirectory;
        var file = $"doomsav{loadGameSlotNumber}.dsg";
        var path = Path.Combine(directory, file);
        SaveAndLoad.Load(this, path);
    }

    private void DoSaveGame()
    {
        gameAction = GameAction.Nothing;

        var directory = ConfigUtilities.GetExeDirectory;
        var file = $"doomsav{saveGameSlotNumber}.dsg";
        var path = Path.Combine(directory, file);
        SaveAndLoad.Save(this, saveGameDescription!, path);
        World.ConsolePlayer.SendMessage(DoomInfo.Strings.GGSAVED);
    }

    private void DoCompleted()
    {
        gameAction = GameAction.Nothing;

        var players = Options.Players.AsSpan();
        ref var playersRef = ref MemoryMarshal.GetReference(players);

        for (var i = 0; i < players.Length; i++)
        {
            ref var player = ref Unsafe.Add(ref playersRef, i);
            // Take away cards and stuff.
            if (player.InGame)
                player.FinishLevel();
        }

        if (Options.GameMode != GameMode.Commercial)
        {
            switch (Options.Map)
            {
                case 8:
                    gameAction = GameAction.Victory;
                    return;
                case 9:
                    for (var i = 0; i < players.Length; i++)
                        Unsafe.Add(ref playersRef, i).DidSecret = true;

                    break;
            }
        }

        var imInfo = Options.IntermissionInfo;

        imInfo.DidSecret = Options.Players[Options.ConsolePlayer].DidSecret;
        imInfo.Episode = Options.Episode - 1;
        imInfo.LastLevel = Options.Map - 1;

        // IntermissionInfo.Next is 0 biased, unlike GameOptions.Map.
        if (Options.GameMode == GameMode.Commercial)
        {
            if (World.SecretExit)
            {
                imInfo.NextLevel = Options.Map switch
                {
                    15 => 30,
                    31 => 31,
                    _  => imInfo.NextLevel
                };
            }
            else
            {
                imInfo.NextLevel = Options.Map switch
                {
                    31 or 32 => 15,
                    _        => Options.Map
                };
            }
        }
        else
        {
            // Go to secret level.
            if (World.SecretExit)
                imInfo.NextLevel = 8;
            // Returning from secret level.
            else if (Options.Map == 9)
            {
                imInfo.NextLevel = Options.Episode switch
                {
                    1 => 3,
                    2 => 5,
                    3 => 6,
                    4 => 2,
                    _ => imInfo.NextLevel
                };
            }
            // Go to next level.
            else
                imInfo.NextLevel = Options.Map;
        }

        imInfo.MaxKillCount = World.TotalKills;
        imInfo.MaxItemCount = World.TotalItems;
        imInfo.MaxSecretCount = World.TotalSecrets;
        imInfo.TotalFrags = 0;

        var parTime = Options.GameMode == GameMode.Commercial
            ? 35 * DoomInfo.ParTimes.Doom2[Options.Map - 1]
            : 35 * DoomInfo.ParTimes.Doom1[Options.Episode - 1][Options.Map - 1];
        imInfo.ParTime = parTime;

        var playerScores = imInfo.PlayerScores.AsSpan();
        ref var playerScoreRef = ref MemoryMarshal.GetReference(playerScores);

        for (var i = 0; i < players.Length; i++)
        {
            ref var player = ref Unsafe.Add(ref playersRef, i);
            ref var playerScore = ref Unsafe.Add(ref playerScoreRef, i);
            playerScore.InGame = player.InGame;
            playerScore.KillCount = player.KillCount;
            playerScore.ItemCount = player.ItemCount;
            playerScore.SecretCount = player.SecretCount;
            playerScore.Time = World.LevelTime;
            player.Frags.AsSpan().CopyTo(playerScore.Frags);
        }

        State = GameState.Intermission;
        Intermission = new Intermission.Intermission(Options, imInfo);
    }

    private void DoWorldDone()
    {
        gameAction = GameAction.Nothing;

        State = GameState.Level;
        Options.Map = Options.IntermissionInfo.NextLevel + 1;
        DoLoadLevel();
    }

    private void DoFinale()
    {
        gameAction = GameAction.Nothing;

        State = GameState.Finale;
        Finale = new Finale(Options);
    }


    ////////////////////////////////////////////////////////////
    // Miscellaneous things
    ////////////////////////////////////////////////////////////

    public void InitNew(GameSkill skill, int episode, int map)
    {
        Options.Skill = (GameSkill)System.Math.Clamp((int)skill, (int)GameSkill.Baby, (int)GameSkill.Nightmare);
        Options.Episode = Options.GameMode switch
        {
            GameMode.Retail    => System.Math.Clamp(episode, 1, 4),
            GameMode.Shareware => 1,
            _                  => System.Math.Clamp(episode, 1, 4)
        };

        Span<int> maxMapCap = [9, 32];
        var maxMap = maxMapCap[(Options.GameMode == GameMode.Commercial).AsByte()];
        Options.Map = System.Math.Clamp(map, 1, maxMap);

        Options.Random.Clear();

        // Force players to be initialized upon first level load.
        for (var i = 0; i < Player.MaxPlayerCount; i++)
            Options.Players[i].PlayerState = PlayerState.Reborn;

        DoLoadLevel();
    }

    public bool DoEvent(DoomEvent e)
    {
        return State switch
        {
            GameState.Level  => World.DoEvent(e),
            GameState.Finale => Finale.DoEvent(e),
            _                => false
        };
    }

    private void DoReborn(int playerNumber)
    {
        // Reload the level from scratch.
        if (!Options.NetGame)
            gameAction = GameAction.LoadLevel;
        else
        {
            // Respawn at the start.

            // First dissasociate the corpse.
            Options.Players[playerNumber].Mobj!.Player = null;

            var ta = World.ThingAllocation;

            // Spawn at random spot if in death match.
            if (Options.Deathmatch != 0)
            {
                ta.DeathMatchSpawnPlayer(playerNumber);
                return;
            }

            if (ta.CheckSpot(playerNumber, ta.PlayerStarts[playerNumber]))
            {
                ta.SpawnPlayer(ta.PlayerStarts[playerNumber]);
                return;
            }

            // Try to spawn at one of the other players spots.
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!ta.CheckSpot(playerNumber, ta.PlayerStarts[i]))
                    continue;

                // Fake as other player.
                ta.PlayerStarts[i].Type = playerNumber + 1;

                World.ThingAllocation.SpawnPlayer(ta.PlayerStarts[i]);

                // Restore.
                ta.PlayerStarts[i].Type = i + 1;

                return;
            }

            // He's going to be inside something.
            // Too bad.
            World.ThingAllocation.SpawnPlayer(ta.PlayerStarts[playerNumber]);
        }
    }
}