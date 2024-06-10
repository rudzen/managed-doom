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


using ManagedDoom.Audio;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class MobjInfo
{
    public MobjInfo(
        int doomEdNum,
        MobjState spawnState,
        int spawnHealth,
        MobjState seeState,
        Sfx seeSound,
        int reactionTime,
        Sfx attackSound,
        MobjState painState,
        int painChance,
        Sfx painSound,
        MobjState meleeState,
        MobjState missileState,
        MobjState deathState,
        MobjState xdeathState,
        Sfx deathSound,
        int speed,
        Fixed radius,
        Fixed height,
        int mass,
        int damage,
        Sfx activeSound,
        MobjFlags flags,
        MobjState raiseState)
    {
        this.DoomEdNum = doomEdNum;
        this.SpawnState = spawnState;
        this.SpawnHealth = spawnHealth;
        this.SeeState = seeState;
        this.SeeSound = seeSound;
        this.ReactionTime = reactionTime;
        this.AttackSound = attackSound;
        this.PainState = painState;
        this.PainChance = painChance;
        this.PainSound = painSound;
        this.MeleeState = meleeState;
        this.MissileState = missileState;
        this.DeathState = deathState;
        this.XdeathState = xdeathState;
        this.DeathSound = deathSound;
        this.Speed = speed;
        this.Radius = radius;
        this.Height = height;
        this.Mass = mass;
        this.Damage = damage;
        this.ActiveSound = activeSound;
        this.Flags = flags;
        this.Raisestate = raiseState;
    }

    public int DoomEdNum { get; set; }

    public MobjState SpawnState { get; set; }

    public int SpawnHealth { get; set; }

    public MobjState SeeState { get; set; }

    public Sfx SeeSound { get; set; }

    public int ReactionTime { get; set; }

    public Sfx AttackSound { get; set; }

    public MobjState PainState { get; set; }

    public int PainChance { get; set; }

    public Sfx PainSound { get; set; }

    public MobjState MeleeState { get; set; }

    public MobjState MissileState { get; set; }

    public MobjState DeathState { get; set; }

    public MobjState XdeathState { get; set; }

    public Sfx DeathSound { get; set; }

    public int Speed { get; set; }

    public Fixed Radius { get; set; }

    public Fixed Height { get; set; }

    public int Mass { get; set; }

    public int Damage { get; set; }

    public Sfx ActiveSound { get; set; }

    public MobjFlags Flags { get; set; }

    public MobjState Raisestate { get; set; }
}