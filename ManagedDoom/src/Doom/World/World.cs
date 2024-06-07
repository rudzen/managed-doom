﻿//
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


using ManagedDoom.Audio;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.World;

public sealed class World
{
    // This is for vanilla compatibility.
    // See SubstNullMobj().
    private readonly Mobj dummy;
    private bool completed;

    private int displayPlayer;
    private bool doneFirstTic;

    private int validCount;

    public World(GameContent resources, GameOptions options, DoomGame game)
    {
        this.Options = options;
        this.Game = game;
        this.Random = options.Random;

        Map = new Map.Map(resources, this);

        Thinkers = new Thinkers(this);
        Specials = new Specials(this);
        ThingAllocation = new ThingAllocation(this);
        ThingMovement = new ThingMovement(this);
        ThingInteraction = new ThingInteraction(this);
        MapCollision = new MapCollision();
        MapInteraction = new MapInteraction(this);
        PathTraversal = new PathTraversal(this);
        Hitscan = new Hitscan(this);
        VisibilityCheck = new VisibilityCheck(this);
        SectorAction = new SectorAction(this);
        PlayerBehavior = new PlayerBehavior(this);
        ItemPickup = new ItemPickup(this);
        WeaponBehavior = new WeaponBehavior(this);
        MonsterBehavior = new MonsterBehavior(this);
        LightingChange = new LightingChange(this);
        StatusBar = new StatusBar(this);
        AutoMap = new AutoMap(this);
        Cheat = new Cheat(this);

        options.IntermissionInfo.TotalFrags = 0;
        options.IntermissionInfo.ParTime = 180;

        for (var i = 0; i < Player.MaxPlayerCount; i++)
        {
            options.Players[i].KillCount = 0;
            options.Players[i].SecretCount = 0;
            options.Players[i].ItemCount = 0;
        }

        // Initial height of view will be set by player think.
        options.Players[options.ConsolePlayer].ViewZ = Fixed.Epsilon;

        TotalKills = 0;
        TotalItems = 0;
        TotalSecrets = 0;

        LoadThings();

        // If deathmatch, randomly spawn the active players.
        if (options.Deathmatch != 0)
        {
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (options.Players[i].InGame)
                {
                    options.Players[i].Mobj = null;
                    ThingAllocation.DeathMatchSpawnPlayer(i);
                }
            }
        }

        Specials.SpawnSpecials();

        LevelTime = 0;
        doneFirstTic = false;
        SecretExit = false;
        completed = false;

        validCount = 0;

        displayPlayer = options.ConsolePlayer;

        dummy = new Mobj(this);

        options.Music.StartMusic(ManagedDoom.Doom.Map.Map.GetMapBgm(options), true);
    }

    public GameOptions Options { get; }

    public DoomGame Game { get; }

    public DoomRandom Random { get; }

    public Map.Map Map { get; }

    public Thinkers Thinkers { get; }

    public Specials Specials { get; }

    public ThingAllocation ThingAllocation { get; }

    public ThingMovement ThingMovement { get; }

    public ThingInteraction ThingInteraction { get; }

    public MapCollision MapCollision { get; }

    public MapInteraction MapInteraction { get; }

    public PathTraversal PathTraversal { get; }

    public Hitscan Hitscan { get; }

    public VisibilityCheck VisibilityCheck { get; }

    public SectorAction SectorAction { get; }

    public PlayerBehavior PlayerBehavior { get; }

    public ItemPickup ItemPickup { get; }

    public WeaponBehavior WeaponBehavior { get; }

    public MonsterBehavior MonsterBehavior { get; }

    public LightingChange LightingChange { get; }

    public StatusBar StatusBar { get; }

    public AutoMap AutoMap { get; }

    public Cheat Cheat { get; }

    public int TotalKills { get; set; }

    public int TotalItems { get; set; }

    public int TotalSecrets { get; set; }

    public int LevelTime { get; set; }

    public int GameTic => Game.GameTic;

    public bool SecretExit { get; private set; }

    public Player ConsolePlayer => Options.Players[Options.ConsolePlayer];
    public Player DisplayPlayer => Options.Players[displayPlayer];
    public bool FirstTicIsNotYetDone => ConsolePlayer.ViewZ == Fixed.Epsilon;

    public UpdateResult Update()
    {
        var players = Options.Players;

        for (var i = 0; i < Player.MaxPlayerCount; i++)
            if (players[i].InGame)
                players[i].UpdateFrameInterpolationInfo();

        Thinkers.UpdateFrameInterpolationInfo();

        foreach (var sector in Map.Sectors)
            sector.UpdateFrameInterpolationInfo();

        for (var i = 0; i < Player.MaxPlayerCount; i++)
            if (players[i].InGame)
                PlayerBehavior.PlayerThink(players[i]);

        Thinkers.Run();
        Specials.Update();
        ThingAllocation.RespawnSpecials();

        StatusBar.Update();
        AutoMap.Update();

        LevelTime++;

        if (completed)
            return UpdateResult.Completed;

        if (doneFirstTic)
            return UpdateResult.None;

        doneFirstTic = true;
        return UpdateResult.NeedWipe;
    }

    private void LoadThings()
    {
        foreach (var mt in Map.Things)
        {
            var spawn = true;

            // Do not spawn cool, new monsters if not commercial.
            if (Options.GameMode != GameMode.Commercial)
            {
                switch (mt.Type)
                {
                    case 68: // Arachnotron
                    case 64: // Archvile
                    case 88: // Boss Brain
                    case 89: // Boss Shooter
                    case 69: // Hell Knight
                    case 67: // Mancubus
                    case 71: // Pain Elemental
                    case 65: // Former Human Commando
                    case 66: // Revenant
                    case 84: // Wolf SS
                        spawn = false;
                        break;
                }
            }

            if (!spawn)
                break;

            ThingAllocation.SpawnMapThing(mt);
        }
    }

    public void ExitLevel()
    {
        SecretExit = false;
        completed = true;
    }

    public void SecretExitLevel()
    {
        SecretExit = true;
        completed = true;
    }

    public void StartSound(Mobj mobj, Sfx sfx, SfxType type)
    {
        Options.Sound.StartSound(mobj, sfx, type);
    }

    public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume)
    {
        Options.Sound.StartSound(mobj, sfx, type, volume);
    }

    public void StopSound(Mobj mobj)
    {
        Options.Sound.StopSound(mobj);
    }

    public int GetNewValidCount()
    {
        validCount++;
        return validCount;
    }

    public bool DoEvent(in DoomEvent e)
    {
        if (!Options.NetGame && !Options.DemoPlayback)
        {
            Cheat.DoEvent(e);
        }

        if (AutoMap.Visible && AutoMap.DoEvent(e)) return true;

        switch (e)
        {
            case { Key: DoomKey.Tab, Type: EventType.KeyDown }:
            {
                if (AutoMap.Visible)
                    AutoMap.Close();
                else
                    AutoMap.Open();
                return true;
            }
            case { Key: DoomKey.F12, Type: EventType.KeyDown }:
            {
                if (Options.DemoPlayback || Options.Deathmatch == 0)
                    ChangeDisplayPlayer();
                return true;
            }
            default:
                return false;
        }
    }

    public void ChangeDisplayPlayer()
    {
        displayPlayer++;
        if (displayPlayer == Player.MaxPlayerCount ||
            !Options.Players[displayPlayer].InGame)
        {
            displayPlayer = 0;
        }
    }

    /// <summary>
    /// In vanilla Doom, some action functions have possibilities
    /// to access null pointers.
    /// This function returns a dummy object if the pointer is null
    /// so that we can avoid crash.
    /// This safeguard is imported from Chocolate Doom.
    /// </summary>
    public Mobj SubstNullMobj(Mobj mobj)
    {
        if (mobj == null)
        {
            dummy.X = Fixed.Zero;
            dummy.Y = Fixed.Zero;
            dummy.Z = Fixed.Zero;
            dummy.Flags = 0;
            return dummy;
        }

        return mobj;
    }
}