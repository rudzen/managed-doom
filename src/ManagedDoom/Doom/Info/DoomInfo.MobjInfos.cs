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
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Info;

public static partial class DoomInfo
{
    public static readonly MobjInfo[] MobjInfos =
    [
        new(                                                                                                               // MobjType.Player
            doomEdNum: -1,                                                                                                 // doomEdNum
            spawnState: MobjState.Play,                                                                                    // spawnState
            spawnHealth: 100,                                                                                              // spawnHealth
            seeState: MobjState.PlayRun1,                                                                                  // seeState
            seeSound: Sfx.NONE,                                                                                            // seeSound
            reactionTime: 0,                                                                                               // reactionTime
            attackSound: Sfx.NONE,                                                                                         // attackSound
            painState: MobjState.PlayPain,                                                                                 // painState
            painChance: 255,                                                                                               // painChance
            painSound: Sfx.PLPAIN,                                                                                         // painSound
            meleeState: MobjState.Null,                                                                                    // meleeState
            missileState: MobjState.PlayAtk1,                                                                              // missileState
            deathState: MobjState.PlayDie1,                                                                                // deathState
            xdeathState: MobjState.PlayXdie1,                                                                              // xdeathState
            deathSound: Sfx.PLDETH,                                                                                        // deathSound
            speed: 0,                                                                                                      // speed
            radius: Fixed.FromInt(value: 16),                                                                              // radius
            height: Fixed.FromInt(value: 56),                                                                              // height
            mass: 100,                                                                                                     // mass
            damage: 0,                                                                                                     // damage
            activeSound: Sfx.NONE,                                                                                         // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.DropOff | MobjFlags.PickUp | MobjFlags.NotDeathmatch, // flags
            raiseState: MobjState.Null                                                                                     // raiseState
        ),

        new(                                                                    // MobjType.Possessed
            doomEdNum: 3004,                                                    // doomEdNum
            spawnState: MobjState.PossStnd,                                     // spawnState
            spawnHealth: 20,                                                    // spawnHealth
            seeState: MobjState.PossRun1,                                       // seeState
            seeSound: Sfx.POSIT1,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.PISTOL,                                            // attackSound
            painState: MobjState.PossPain,                                      // painState
            painChance: 200,                                                    // painChance
            painSound: Sfx.POPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.PossAtk1,                                   // missileState
            deathState: MobjState.PossDie1,                                     // deathState
            xdeathState: MobjState.PossXdie1,                                   // xdeathState
            deathSound: Sfx.PODTH1,                                             // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 20),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 100,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.POSACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.PossRaise1                                    // raiseState
        ),

        new(                                                                    // MobjType.Shotguy
            doomEdNum: 9,                                                       // doomEdNum
            spawnState: MobjState.SposStnd,                                     // spawnState
            spawnHealth: 30,                                                    // spawnHealth
            seeState: MobjState.SposRun1,                                       // seeState
            seeSound: Sfx.POSIT2,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.SposPain,                                      // painState
            painChance: 170,                                                    // painChance
            painSound: Sfx.POPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.SposAtk1,                                   // missileState
            deathState: MobjState.SposDie1,                                     // deathState
            xdeathState: MobjState.SposXdie1,                                   // xdeathState
            deathSound: Sfx.PODTH2,                                             // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 20),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 100,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.POSACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.SposRaise1                                    // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Vile
            doomEdNum: 64,                                                      // doomEdNum
            spawnState: MobjState.VileStnd,                                     // spawnState
            spawnHealth: 700,                                                   // spawnHealth
            seeState: MobjState.VileRun1,                                       // seeState
            seeSound: Sfx.VILSIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.VilePain,                                      // painState
            painChance: 10,                                                     // painChance
            painSound: Sfx.VIPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.VileAtk1,                                   // missileState
            deathState: MobjState.VileDie1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.VILDTH,                                             // deathSound
            speed: 15,                                                          // speed
            radius: Fixed.FromInt(value: 20),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 500,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.VILACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.Null                                          // raiseState
        ),

        new MobjInfo(                                          // MobjType.Fire
            doomEdNum: -1,                                     // doomEdNum
            spawnState: MobjState.Fire1,                       // spawnState
            spawnHealth: 1000,                                 // spawnHealth
            seeState: MobjState.Null,                          // seeState
            seeSound: Sfx.NONE,                                // seeSound
            reactionTime: 8,                                   // reactionTime
            attackSound: Sfx.NONE,                             // attackSound
            painState: MobjState.Null,                         // painState
            painChance: 0,                                     // painChance
            painSound: Sfx.NONE,                               // painSound
            meleeState: MobjState.Null,                        // meleeState
            missileState: MobjState.Null,                      // missileState
            deathState: MobjState.Null,                        // deathState
            xdeathState: MobjState.Null,                       // xdeathState
            deathSound: Sfx.NONE,                              // deathSound
            speed: 0,                                          // speed
            radius: Fixed.FromInt(value: 20),                  // radius
            height: Fixed.FromInt(value: 16),                  // height
            mass: 100,                                         // mass
            damage: 0,                                         // damage
            activeSound: Sfx.NONE,                             // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                         // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Undead
            doomEdNum: 66,                                                      // doomEdNum
            spawnState: MobjState.SkelStnd,                                     // spawnState
            spawnHealth: 300,                                                   // spawnHealth
            seeState: MobjState.SkelRun1,                                       // seeState
            seeSound: Sfx.SKESIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.SkelPain,                                      // painState
            painChance: 100,                                                    // painChance
            painSound: Sfx.POPAIN,                                              // painSound
            meleeState: MobjState.SkelFist1,                                    // meleeState
            missileState: MobjState.SkelMiss1,                                  // missileState
            deathState: MobjState.SkelDie1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.SKEDTH,                                             // deathSound
            speed: 10,                                                          // speed
            radius: Fixed.FromInt(value: 20),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 500,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.SKEACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.SkelRaise1                                    // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Tracer
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Tracer,                                                              // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.SKEATK,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Traceexp1,                                                           // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.BAREXP,                                                                    // deathSound
            speed: 10 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 11),                                                          // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 10,                                                                                // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                          // MobjType.Smoke
            doomEdNum: -1,                                     // doomEdNum
            spawnState: MobjState.Smoke1,                      // spawnState
            spawnHealth: 1000,                                 // spawnHealth
            seeState: MobjState.Null,                          // seeState
            seeSound: Sfx.NONE,                                // seeSound
            reactionTime: 8,                                   // reactionTime
            attackSound: Sfx.NONE,                             // attackSound
            painState: MobjState.Null,                         // painState
            painChance: 0,                                     // painChance
            painSound: Sfx.NONE,                               // painSound
            meleeState: MobjState.Null,                        // meleeState
            missileState: MobjState.Null,                      // missileState
            deathState: MobjState.Null,                        // deathState
            xdeathState: MobjState.Null,                       // xdeathState
            deathSound: Sfx.NONE,                              // deathSound
            speed: 0,                                          // speed
            radius: Fixed.FromInt(value: 20),                  // radius
            height: Fixed.FromInt(value: 16),                  // height
            mass: 100,                                         // mass
            damage: 0,                                         // damage
            activeSound: Sfx.NONE,                             // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                         // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Fatso
            doomEdNum: 67,                                                      // doomEdNum
            spawnState: MobjState.FattStnd,                                     // spawnState
            spawnHealth: 600,                                                   // spawnHealth
            seeState: MobjState.FattRun1,                                       // seeState
            seeSound: Sfx.MANSIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.FattPain,                                      // painState
            painChance: 80,                                                     // painChance
            painSound: Sfx.MNPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.FattAtk1,                                   // missileState
            deathState: MobjState.FattDie1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.MANDTH,                                             // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 48),                                   // radius
            height: Fixed.FromInt(value: 64),                                   // height
            mass: 1000,                                                         // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.POSACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.FattRaise1                                    // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Fatshot
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Fatshot1,                                                            // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.FIRSHT,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Fatshotx1,                                                           // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.FIRXPL,                                                                    // deathSound
            speed: 20 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 6),                                                           // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 8,                                                                                 // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Chainguy
            doomEdNum: 65,                                                      // doomEdNum
            spawnState: MobjState.CposStnd,                                     // spawnState
            spawnHealth: 70,                                                    // spawnHealth
            seeState: MobjState.CposRun1,                                       // seeState
            seeSound: Sfx.POSIT2,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.CposPain,                                      // painState
            painChance: 170,                                                    // painChance
            painSound: Sfx.POPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.CposAtk1,                                   // missileState
            deathState: MobjState.CposDie1,                                     // deathState
            xdeathState: MobjState.CposXdie1,                                   // xdeathState
            deathSound: Sfx.PODTH2,                                             // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 20),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 100,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.POSACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.CposRaise1                                    // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Troop
            doomEdNum: 3001,                                                    // doomEdNum
            spawnState: MobjState.TrooStnd,                                     // spawnState
            spawnHealth: 60,                                                    // spawnHealth
            seeState: MobjState.TrooRun1,                                       // seeState
            seeSound: Sfx.BGSIT1,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.TrooPain,                                      // painState
            painChance: 200,                                                    // painChance
            painSound: Sfx.POPAIN,                                              // painSound
            meleeState: MobjState.TrooAtk1,                                     // meleeState
            missileState: MobjState.TrooAtk1,                                   // missileState
            deathState: MobjState.TrooDie1,                                     // deathState
            xdeathState: MobjState.TrooXdie1,                                   // xdeathState
            deathSound: Sfx.BGDTH1,                                             // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 20),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 100,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.BGACT,                                             // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.TrooRaise1                                    // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Sergeant
            doomEdNum: 3002,                                                    // doomEdNum
            spawnState: MobjState.SargStnd,                                     // spawnState
            spawnHealth: 150,                                                   // spawnHealth
            seeState: MobjState.SargRun1,                                       // seeState
            seeSound: Sfx.SGTSIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.SGTATK,                                            // attackSound
            painState: MobjState.SargPain,                                      // painState
            painChance: 180,                                                    // painChance
            painSound: Sfx.DMPAIN,                                              // painSound
            meleeState: MobjState.SargAtk1,                                     // meleeState
            missileState: MobjState.Null,                                       // missileState
            deathState: MobjState.SargDie1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.SGTDTH,                                             // deathSound
            speed: 10,                                                          // speed
            radius: Fixed.FromInt(value: 30),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 400,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.DMACT,                                             // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.SargRaise1                                    // raiseState
        ),

        new MobjInfo(                                                                              // MobjType.Shadows
            doomEdNum: 58,                                                                         // doomEdNum
            spawnState: MobjState.SargStnd,                                                        // spawnState
            spawnHealth: 150,                                                                      // spawnHealth
            seeState: MobjState.SargRun1,                                                          // seeState
            seeSound: Sfx.SGTSIT,                                                                  // seeSound
            reactionTime: 8,                                                                       // reactionTime
            attackSound: Sfx.SGTATK,                                                               // attackSound
            painState: MobjState.SargPain,                                                         // painState
            painChance: 180,                                                                       // painChance
            painSound: Sfx.DMPAIN,                                                                 // painSound
            meleeState: MobjState.SargAtk1,                                                        // meleeState
            missileState: MobjState.Null,                                                          // missileState
            deathState: MobjState.SargDie1,                                                        // deathState
            xdeathState: MobjState.Null,                                                           // xdeathState
            deathSound: Sfx.SGTDTH,                                                                // deathSound
            speed: 10,                                                                             // speed
            radius: Fixed.FromInt(value: 30),                                                      // radius
            height: Fixed.FromInt(value: 56),                                                      // height
            mass: 400,                                                                             // mass
            damage: 0,                                                                             // damage
            activeSound: Sfx.DMACT,                                                                // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Shadow | MobjFlags.CountKill, // flags
            raiseState: MobjState.SargRaise1                                                       // raiseState
        ),

        new MobjInfo(                                                                                                   // MobjType.Head
            doomEdNum: 3005,                                                                                            // doomEdNum
            spawnState: MobjState.HeadStnd,                                                                             // spawnState
            spawnHealth: 400,                                                                                           // spawnHealth
            seeState: MobjState.HeadRun1,                                                                               // seeState
            seeSound: Sfx.CACSIT,                                                                                       // seeSound
            reactionTime: 8,                                                                                            // reactionTime
            attackSound: Sfx.NONE,                                                                                      // attackSound
            painState: MobjState.HeadPain,                                                                              // painState
            painChance: 128,                                                                                            // painChance
            painSound: Sfx.DMPAIN,                                                                                      // painSound
            meleeState: MobjState.Null,                                                                                 // meleeState
            missileState: MobjState.HeadAtk1,                                                                           // missileState
            deathState: MobjState.HeadDie1,                                                                             // deathState
            xdeathState: MobjState.Null,                                                                                // xdeathState
            deathSound: Sfx.CACDTH,                                                                                     // deathSound
            speed: 8,                                                                                                   // speed
            radius: Fixed.FromInt(value: 31),                                                                           // radius
            height: Fixed.FromInt(value: 56),                                                                           // height
            mass: 400,                                                                                                  // mass
            damage: 0,                                                                                                  // damage
            activeSound: Sfx.DMACT,                                                                                     // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity | MobjFlags.CountKill, // flags
            raiseState: MobjState.HeadRaise1                                                                            // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Bruiser
            doomEdNum: 3003,                                                    // doomEdNum
            spawnState: MobjState.BossStnd,                                     // spawnState
            spawnHealth: 1000,                                                  // spawnHealth
            seeState: MobjState.BossRun1,                                       // seeState
            seeSound: Sfx.BRSSIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.BossPain,                                      // painState
            painChance: 50,                                                     // painChance
            painSound: Sfx.DMPAIN,                                              // painSound
            meleeState: MobjState.BossAtk1,                                     // meleeState
            missileState: MobjState.BossAtk1,                                   // missileState
            deathState: MobjState.BossDie1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.BRSDTH,                                             // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 24),                                   // radius
            height: Fixed.FromInt(value: 64),                                   // height
            mass: 1000,                                                         // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.DMACT,                                             // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.BossRaise1                                    // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Bruisershot
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Brball1,                                                             // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.FIRSHT,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Brballx1,                                                            // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.FIRXPL,                                                                    // deathSound
            speed: 15 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 6),                                                           // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 8,                                                                                 // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Knight
            doomEdNum: 69,                                                      // doomEdNum
            spawnState: MobjState.Bos2Stnd,                                     // spawnState
            spawnHealth: 500,                                                   // spawnHealth
            seeState: MobjState.Bos2Run1,                                       // seeState
            seeSound: Sfx.KNTSIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.Bos2Pain,                                      // painState
            painChance: 50,                                                     // painChance
            painSound: Sfx.DMPAIN,                                              // painSound
            meleeState: MobjState.Bos2Atk1,                                     // meleeState
            missileState: MobjState.Bos2Atk1,                                   // missileState
            deathState: MobjState.Bos2Die1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.KNTDTH,                                             // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 24),                                   // radius
            height: Fixed.FromInt(value: 64),                                   // height
            mass: 1000,                                                         // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.DMACT,                                             // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.Bos2Raise1                                    // raiseState
        ),

        new MobjInfo(                                                                             // MobjType.Skull
            doomEdNum: 3006,                                                                      // doomEdNum
            spawnState: MobjState.SkullStnd,                                                      // spawnState
            spawnHealth: 100,                                                                     // spawnHealth
            seeState: MobjState.SkullRun1,                                                        // seeState
            seeSound: Sfx.NONE,                                                                   // seeSound
            reactionTime: 8,                                                                      // reactionTime
            attackSound: Sfx.SKLATK,                                                              // attackSound
            painState: MobjState.SkullPain,                                                       // painState
            painChance: 256,                                                                      // painChance
            painSound: Sfx.DMPAIN,                                                                // painSound
            meleeState: MobjState.Null,                                                           // meleeState
            missileState: MobjState.SkullAtk1,                                                    // missileState
            deathState: MobjState.SkullDie1,                                                      // deathState
            xdeathState: MobjState.Null,                                                          // xdeathState
            deathSound: Sfx.FIRXPL,                                                               // deathSound
            speed: 8,                                                                             // speed
            radius: Fixed.FromInt(value: 16),                                                     // radius
            height: Fixed.FromInt(value: 56),                                                     // height
            mass: 50,                                                                             // mass
            damage: 3,                                                                            // damage
            activeSound: Sfx.DMACT,                                                               // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                            // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Spider
            doomEdNum: 7,                                                       // doomEdNum
            spawnState: MobjState.SpidStnd,                                     // spawnState
            spawnHealth: 3000,                                                  // spawnHealth
            seeState: MobjState.SpidRun1,                                       // seeState
            seeSound: Sfx.SPISIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.SHOTGN,                                            // attackSound
            painState: MobjState.SpidPain,                                      // painState
            painChance: 40,                                                     // painChance
            painSound: Sfx.DMPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.SpidAtk1,                                   // missileState
            deathState: MobjState.SpidDie1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.SPIDTH,                                             // deathSound
            speed: 12,                                                          // speed
            radius: Fixed.FromInt(value: 128),                                  // radius
            height: Fixed.FromInt(value: 100),                                  // height
            mass: 1000,                                                         // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.DMACT,                                             // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.Null                                          // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Baby
            doomEdNum: 68,                                                      // doomEdNum
            spawnState: MobjState.BspiStnd,                                     // spawnState
            spawnHealth: 500,                                                   // spawnHealth
            seeState: MobjState.BspiSight,                                      // seeState
            seeSound: Sfx.BSPSIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.BspiPain,                                      // painState
            painChance: 128,                                                    // painChance
            painSound: Sfx.DMPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.BspiAtk1,                                   // missileState
            deathState: MobjState.BspiDie1,                                     // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.BSPDTH,                                             // deathSound
            speed: 12,                                                          // speed
            radius: Fixed.FromInt(value: 64),                                   // radius
            height: Fixed.FromInt(value: 64),                                   // height
            mass: 600,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.BSPACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.BspiRaise1                                    // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Cyborg
            doomEdNum: 16,                                                      // doomEdNum
            spawnState: MobjState.CyberStnd,                                    // spawnState
            spawnHealth: 4000,                                                  // spawnHealth
            seeState: MobjState.CyberRun1,                                      // seeState
            seeSound: Sfx.CYBSIT,                                               // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.CyberPain,                                     // painState
            painChance: 20,                                                     // painChance
            painSound: Sfx.DMPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.CyberAtk1,                                  // missileState
            deathState: MobjState.CyberDie1,                                    // deathState
            xdeathState: MobjState.Null,                                        // xdeathState
            deathSound: Sfx.CYBDTH,                                             // deathSound
            speed: 16,                                                          // speed
            radius: Fixed.FromInt(value: 40),                                   // radius
            height: Fixed.FromInt(value: 110),                                  // height
            mass: 1000,                                                         // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.DMACT,                                             // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.Null                                          // raiseState
        ),

        new MobjInfo(                                                                                                   // MobjType.Pain
            doomEdNum: 71,                                                                                              // doomEdNum
            spawnState: MobjState.PainStnd,                                                                             // spawnState
            spawnHealth: 400,                                                                                           // spawnHealth
            seeState: MobjState.PainRun1,                                                                               // seeState
            seeSound: Sfx.PESIT,                                                                                        // seeSound
            reactionTime: 8,                                                                                            // reactionTime
            attackSound: Sfx.NONE,                                                                                      // attackSound
            painState: MobjState.PainPain,                                                                              // painState
            painChance: 128,                                                                                            // painChance
            painSound: Sfx.PEPAIN,                                                                                      // painSound
            meleeState: MobjState.Null,                                                                                 // meleeState
            missileState: MobjState.PainAtk1,                                                                           // missileState
            deathState: MobjState.PainDie1,                                                                             // deathState
            xdeathState: MobjState.Null,                                                                                // xdeathState
            deathSound: Sfx.PEDTH,                                                                                      // deathSound
            speed: 8,                                                                                                   // speed
            radius: Fixed.FromInt(value: 31),                                                                           // radius
            height: Fixed.FromInt(value: 56),                                                                           // height
            mass: 400,                                                                                                  // mass
            damage: 0,                                                                                                  // damage
            activeSound: Sfx.DMACT,                                                                                     // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity | MobjFlags.CountKill, // flags
            raiseState: MobjState.PainRaise1                                                                            // raiseState
        ),

        new MobjInfo(                                                           // MobjType.Wolfss
            doomEdNum: 84,                                                      // doomEdNum
            spawnState: MobjState.SswvStnd,                                     // spawnState
            spawnHealth: 50,                                                    // spawnHealth
            seeState: MobjState.SswvRun1,                                       // seeState
            seeSound: Sfx.SSSIT,                                                // seeSound
            reactionTime: 8,                                                    // reactionTime
            attackSound: Sfx.NONE,                                              // attackSound
            painState: MobjState.SswvPain,                                      // painState
            painChance: 170,                                                    // painChance
            painSound: Sfx.POPAIN,                                              // painSound
            meleeState: MobjState.Null,                                         // meleeState
            missileState: MobjState.SswvAtk1,                                   // missileState
            deathState: MobjState.SswvDie1,                                     // deathState
            xdeathState: MobjState.SswvXdie1,                                   // xdeathState
            deathSound: Sfx.SSDTH,                                              // deathSound
            speed: 8,                                                           // speed
            radius: Fixed.FromInt(value: 20),                                   // radius
            height: Fixed.FromInt(value: 56),                                   // height
            mass: 100,                                                          // mass
            damage: 0,                                                          // damage
            activeSound: Sfx.POSACT,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.SswvRaise1                                    // raiseState
        ),

        new MobjInfo(                                                                                                          // MobjType.Keen
            doomEdNum: 72,                                                                                                     // doomEdNum
            spawnState: MobjState.Keenstnd,                                                                                    // spawnState
            spawnHealth: 100,                                                                                                  // spawnHealth
            seeState: MobjState.Null,                                                                                          // seeState
            seeSound: Sfx.NONE,                                                                                                // seeSound
            reactionTime: 8,                                                                                                   // reactionTime
            attackSound: Sfx.NONE,                                                                                             // attackSound
            painState: MobjState.Keenpain,                                                                                     // painState
            painChance: 256,                                                                                                   // painChance
            painSound: Sfx.KEENPN,                                                                                             // painSound
            meleeState: MobjState.Null,                                                                                        // meleeState
            missileState: MobjState.Null,                                                                                      // missileState
            deathState: MobjState.Commkeen,                                                                                    // deathState
            xdeathState: MobjState.Null,                                                                                       // xdeathState
            deathSound: Sfx.KEENDT,                                                                                            // deathSound
            speed: 0,                                                                                                          // speed
            radius: Fixed.FromInt(value: 16),                                                                                  // radius
            height: Fixed.FromInt(value: 72),                                                                                  // height
            mass: 10000000,                                                                                                    // mass
            damage: 0,                                                                                                         // damage
            activeSound: Sfx.NONE,                                                                                             // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity | MobjFlags.Shootable | MobjFlags.CountKill, // flags
            raiseState: MobjState.Null                                                                                         // raiseState
        ),

        new MobjInfo(                                     // MobjType.Bossbrain
            doomEdNum: 88,                                // doomEdNum
            spawnState: MobjState.Brain,                  // spawnState
            spawnHealth: 250,                             // spawnHealth
            seeState: MobjState.Null,                     // seeState
            seeSound: Sfx.NONE,                           // seeSound
            reactionTime: 8,                              // reactionTime
            attackSound: Sfx.NONE,                        // attackSound
            painState: MobjState.BrainPain,               // painState
            painChance: 255,                              // painChance
            painSound: Sfx.BOSPN,                         // painSound
            meleeState: MobjState.Null,                   // meleeState
            missileState: MobjState.Null,                 // missileState
            deathState: MobjState.BrainDie1,              // deathState
            xdeathState: MobjState.Null,                  // xdeathState
            deathSound: Sfx.BOSDTH,                       // deathSound
            speed: 0,                                     // speed
            radius: Fixed.FromInt(value: 16),             // radius
            height: Fixed.FromInt(value: 16),             // height
            mass: 10000000,                               // mass
            damage: 0,                                    // damage
            activeSound: Sfx.NONE,                        // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable, // flags
            raiseState: MobjState.Null                    // raiseState
        ),

        new MobjInfo(                                         // MobjType.Bossspit
            doomEdNum: 89,                                    // doomEdNum
            spawnState: MobjState.Braineye,                   // spawnState
            spawnHealth: 1000,                                // spawnHealth
            seeState: MobjState.Braineyesee,                  // seeState
            seeSound: Sfx.NONE,                               // seeSound
            reactionTime: 8,                                  // reactionTime
            attackSound: Sfx.NONE,                            // attackSound
            painState: MobjState.Null,                        // painState
            painChance: 0,                                    // painChance
            painSound: Sfx.NONE,                              // painSound
            meleeState: MobjState.Null,                       // meleeState
            missileState: MobjState.Null,                     // missileState
            deathState: MobjState.Null,                       // deathState
            xdeathState: MobjState.Null,                      // xdeathState
            deathSound: Sfx.NONE,                             // deathSound
            speed: 0,                                         // speed
            radius: Fixed.FromInt(value: 20),                 // radius
            height: Fixed.FromInt(value: 32),                 // height
            mass: 100,                                        // mass
            damage: 0,                                        // damage
            activeSound: Sfx.NONE,                            // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
            raiseState: MobjState.Null                        // raiseState
        ),

        new MobjInfo(                                         // MobjType.Bosstarget
            doomEdNum: 87,                                    // doomEdNum
            spawnState: MobjState.Null,                       // spawnState
            spawnHealth: 1000,                                // spawnHealth
            seeState: MobjState.Null,                         // seeState
            seeSound: Sfx.NONE,                               // seeSound
            reactionTime: 8,                                  // reactionTime
            attackSound: Sfx.NONE,                            // attackSound
            painState: MobjState.Null,                        // painState
            painChance: 0,                                    // painChance
            painSound: Sfx.NONE,                              // painSound
            meleeState: MobjState.Null,                       // meleeState
            missileState: MobjState.Null,                     // missileState
            deathState: MobjState.Null,                       // deathState
            xdeathState: MobjState.Null,                      // xdeathState
            deathSound: Sfx.NONE,                             // deathSound
            speed: 0,                                         // speed
            radius: Fixed.FromInt(value: 20),                 // radius
            height: Fixed.FromInt(value: 32),                 // height
            mass: 100,                                        // mass
            damage: 0,                                        // damage
            activeSound: Sfx.NONE,                            // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
            raiseState: MobjState.Null                        // raiseState
        ),

        new MobjInfo(                                                                                                     // MobjType.Spawnshot
            doomEdNum: -1,                                                                                                // doomEdNum
            spawnState: MobjState.Spawn1,                                                                                 // spawnState
            spawnHealth: 1000,                                                                                            // spawnHealth
            seeState: MobjState.Null,                                                                                     // seeState
            seeSound: Sfx.BOSPIT,                                                                                         // seeSound
            reactionTime: 8,                                                                                              // reactionTime
            attackSound: Sfx.NONE,                                                                                        // attackSound
            painState: MobjState.Null,                                                                                    // painState
            painChance: 0,                                                                                                // painChance
            painSound: Sfx.NONE,                                                                                          // painSound
            meleeState: MobjState.Null,                                                                                   // meleeState
            missileState: MobjState.Null,                                                                                 // missileState
            deathState: MobjState.Null,                                                                                   // deathState
            xdeathState: MobjState.Null,                                                                                  // xdeathState
            deathSound: Sfx.FIRXPL,                                                                                       // deathSound
            speed: 10 * Fixed.FracUnit,                                                                                   // speed
            radius: Fixed.FromInt(value: 6),                                                                              // radius
            height: Fixed.FromInt(value: 32),                                                                             // height
            mass: 100,                                                                                                    // mass
            damage: 3,                                                                                                    // damage
            activeSound: Sfx.NONE,                                                                                        // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity | MobjFlags.NoClip, // flags
            raiseState: MobjState.Null                                                                                    // raiseState
        ),

        new MobjInfo(                                          // MobjType.Spawnfire
            doomEdNum: -1,                                     // doomEdNum
            spawnState: MobjState.Spawnfire1,                  // spawnState
            spawnHealth: 1000,                                 // spawnHealth
            seeState: MobjState.Null,                          // seeState
            seeSound: Sfx.NONE,                                // seeSound
            reactionTime: 8,                                   // reactionTime
            attackSound: Sfx.NONE,                             // attackSound
            painState: MobjState.Null,                         // painState
            painChance: 0,                                     // painChance
            painSound: Sfx.NONE,                               // painSound
            meleeState: MobjState.Null,                        // meleeState
            missileState: MobjState.Null,                      // missileState
            deathState: MobjState.Null,                        // deathState
            xdeathState: MobjState.Null,                       // xdeathState
            deathSound: Sfx.NONE,                              // deathSound
            speed: 0,                                          // speed
            radius: Fixed.FromInt(value: 20),                  // radius
            height: Fixed.FromInt(value: 16),                  // height
            mass: 100,                                         // mass
            damage: 0,                                         // damage
            activeSound: Sfx.NONE,                             // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                         // raiseState
        ),

        new MobjInfo(                                                         // MobjType.Barrel
            doomEdNum: 2035,                                                  // doomEdNum
            spawnState: MobjState.Bar1,                                       // spawnState
            spawnHealth: 20,                                                  // spawnHealth
            seeState: MobjState.Null,                                         // seeState
            seeSound: Sfx.NONE,                                               // seeSound
            reactionTime: 8,                                                  // reactionTime
            attackSound: Sfx.NONE,                                            // attackSound
            painState: MobjState.Null,                                        // painState
            painChance: 0,                                                    // painChance
            painSound: Sfx.NONE,                                              // painSound
            meleeState: MobjState.Null,                                       // meleeState
            missileState: MobjState.Null,                                     // missileState
            deathState: MobjState.Bexp,                                       // deathState
            xdeathState: MobjState.Null,                                      // xdeathState
            deathSound: Sfx.BAREXP,                                           // deathSound
            speed: 0,                                                         // speed
            radius: Fixed.FromInt(value: 10),                                 // radius
            height: Fixed.FromInt(value: 42),                                 // height
            mass: 100,                                                        // mass
            damage: 0,                                                        // damage
            activeSound: Sfx.NONE,                                            // activeSound
            flags: MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.NoBlood, // flags
            raiseState: MobjState.Null                                        // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Troopshot
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Tball1,                                                              // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.FIRSHT,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Tballx1,                                                             // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.FIRXPL,                                                                    // deathSound
            speed: 10 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 6),                                                           // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 3,                                                                                 // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Headshot
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Rball1,                                                              // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.FIRSHT,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Rballx1,                                                             // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.FIRXPL,                                                                    // deathSound
            speed: 10 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 6),                                                           // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 5,                                                                                 // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Rocket
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Rocket,                                                              // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.RLAUNC,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Explode1,                                                            // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.BAREXP,                                                                    // deathSound
            speed: 20 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 11),                                                          // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 20,                                                                                // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Plasma
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Plasball,                                                            // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.PLASMA,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Plasexp,                                                             // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.FIRXPL,                                                                    // deathSound
            speed: 25 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 13),                                                          // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 5,                                                                                 // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Bfg
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.Bfgshot,                                                             // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.NONE,                                                                        // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.Bfgland,                                                             // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.RXPLOD,                                                                    // deathSound
            speed: 25 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 13),                                                          // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 100,                                                                               // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                                                                  // MobjType.Arachplaz
            doomEdNum: -1,                                                                             // doomEdNum
            spawnState: MobjState.ArachPlaz,                                                           // spawnState
            spawnHealth: 1000,                                                                         // spawnHealth
            seeState: MobjState.Null,                                                                  // seeState
            seeSound: Sfx.PLASMA,                                                                      // seeSound
            reactionTime: 8,                                                                           // reactionTime
            attackSound: Sfx.NONE,                                                                     // attackSound
            painState: MobjState.Null,                                                                 // painState
            painChance: 0,                                                                             // painChance
            painSound: Sfx.NONE,                                                                       // painSound
            meleeState: MobjState.Null,                                                                // meleeState
            missileState: MobjState.Null,                                                              // missileState
            deathState: MobjState.ArachPlex,                                                           // deathState
            xdeathState: MobjState.Null,                                                               // xdeathState
            deathSound: Sfx.FIRXPL,                                                                    // deathSound
            speed: 25 * Fixed.FracUnit,                                                                // speed
            radius: Fixed.FromInt(value: 13),                                                          // radius
            height: Fixed.FromInt(value: 8),                                                           // height
            mass: 100,                                                                                 // mass
            damage: 5,                                                                                 // damage
            activeSound: Sfx.NONE,                                                                     // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                                                 // raiseState
        ),

        new MobjInfo(                                          // MobjType.Puff
            doomEdNum: -1,                                     // doomEdNum
            spawnState: MobjState.Puff1,                       // spawnState
            spawnHealth: 1000,                                 // spawnHealth
            seeState: MobjState.Null,                          // seeState
            seeSound: Sfx.NONE,                                // seeSound
            reactionTime: 8,                                   // reactionTime
            attackSound: Sfx.NONE,                             // attackSound
            painState: MobjState.Null,                         // painState
            painChance: 0,                                     // painChance
            painSound: Sfx.NONE,                               // painSound
            meleeState: MobjState.Null,                        // meleeState
            missileState: MobjState.Null,                      // missileState
            deathState: MobjState.Null,                        // deathState
            xdeathState: MobjState.Null,                       // xdeathState
            deathSound: Sfx.NONE,                              // deathSound
            speed: 0,                                          // speed
            radius: Fixed.FromInt(value: 20),                  // radius
            height: Fixed.FromInt(value: 16),                  // height
            mass: 100,                                         // mass
            damage: 0,                                         // damage
            activeSound: Sfx.NONE,                             // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                         // raiseState
        ),

        new MobjInfo(                         // MobjType.Blood
            doomEdNum: -1,                    // doomEdNum
            spawnState: MobjState.Blood1,     // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.NoBlockMap,      // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                                          // MobjType.Tfog
            doomEdNum: -1,                                     // doomEdNum
            spawnState: MobjState.Tfog,                        // spawnState
            spawnHealth: 1000,                                 // spawnHealth
            seeState: MobjState.Null,                          // seeState
            seeSound: Sfx.NONE,                                // seeSound
            reactionTime: 8,                                   // reactionTime
            attackSound: Sfx.NONE,                             // attackSound
            painState: MobjState.Null,                         // painState
            painChance: 0,                                     // painChance
            painSound: Sfx.NONE,                               // painSound
            meleeState: MobjState.Null,                        // meleeState
            missileState: MobjState.Null,                      // missileState
            deathState: MobjState.Null,                        // deathState
            xdeathState: MobjState.Null,                       // xdeathState
            deathSound: Sfx.NONE,                              // deathSound
            speed: 0,                                          // speed
            radius: Fixed.FromInt(value: 20),                  // radius
            height: Fixed.FromInt(value: 16),                  // height
            mass: 100,                                         // mass
            damage: 0,                                         // damage
            activeSound: Sfx.NONE,                             // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                         // raiseState
        ),

        new MobjInfo(                                          // MobjType.Ifog
            doomEdNum: -1,                                     // doomEdNum
            spawnState: MobjState.Ifog,                        // spawnState
            spawnHealth: 1000,                                 // spawnHealth
            seeState: MobjState.Null,                          // seeState
            seeSound: Sfx.NONE,                                // seeSound
            reactionTime: 8,                                   // reactionTime
            attackSound: Sfx.NONE,                             // attackSound
            painState: MobjState.Null,                         // painState
            painChance: 0,                                     // painChance
            painSound: Sfx.NONE,                               // painSound
            meleeState: MobjState.Null,                        // meleeState
            missileState: MobjState.Null,                      // missileState
            deathState: MobjState.Null,                        // deathState
            xdeathState: MobjState.Null,                       // xdeathState
            deathSound: Sfx.NONE,                              // deathSound
            speed: 0,                                          // speed
            radius: Fixed.FromInt(value: 20),                  // radius
            height: Fixed.FromInt(value: 16),                  // height
            mass: 100,                                         // mass
            damage: 0,                                         // damage
            activeSound: Sfx.NONE,                             // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                         // raiseState
        ),

        new MobjInfo(                                         // MobjType.Teleportman
            doomEdNum: 14,                                    // doomEdNum
            spawnState: MobjState.Null,                       // spawnState
            spawnHealth: 1000,                                // spawnHealth
            seeState: MobjState.Null,                         // seeState
            seeSound: Sfx.NONE,                               // seeSound
            reactionTime: 8,                                  // reactionTime
            attackSound: Sfx.NONE,                            // attackSound
            painState: MobjState.Null,                        // painState
            painChance: 0,                                    // painChance
            painSound: Sfx.NONE,                              // painSound
            meleeState: MobjState.Null,                       // meleeState
            missileState: MobjState.Null,                     // missileState
            deathState: MobjState.Null,                       // deathState
            xdeathState: MobjState.Null,                      // xdeathState
            deathSound: Sfx.NONE,                             // deathSound
            speed: 0,                                         // speed
            radius: Fixed.FromInt(value: 20),                 // radius
            height: Fixed.FromInt(value: 16),                 // height
            mass: 100,                                        // mass
            damage: 0,                                        // damage
            activeSound: Sfx.NONE,                            // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
            raiseState: MobjState.Null                        // raiseState
        ),

        new MobjInfo(                                          // MobjType.Extrabfg
            doomEdNum: -1,                                     // doomEdNum
            spawnState: MobjState.Bfgexp,                      // spawnState
            spawnHealth: 1000,                                 // spawnHealth
            seeState: MobjState.Null,                          // seeState
            seeSound: Sfx.NONE,                                // seeSound
            reactionTime: 8,                                   // reactionTime
            attackSound: Sfx.NONE,                             // attackSound
            painState: MobjState.Null,                         // painState
            painChance: 0,                                     // painChance
            painSound: Sfx.NONE,                               // painSound
            meleeState: MobjState.Null,                        // meleeState
            missileState: MobjState.Null,                      // missileState
            deathState: MobjState.Null,                        // deathState
            xdeathState: MobjState.Null,                       // xdeathState
            deathSound: Sfx.NONE,                              // deathSound
            speed: 0,                                          // speed
            radius: Fixed.FromInt(value: 20),                  // radius
            height: Fixed.FromInt(value: 16),                  // height
            mass: 100,                                         // mass
            damage: 0,                                         // damage
            activeSound: Sfx.NONE,                             // activeSound
            flags: MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                         // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc0
            doomEdNum: 2018,                  // doomEdNum
            spawnState: MobjState.Arm1,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc1
            doomEdNum: 2019,                  // doomEdNum
            spawnState: MobjState.Arm2,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                                       // MobjType.Misc2
            doomEdNum: 2014,                                // doomEdNum
            spawnState: MobjState.Bon1,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                                       // MobjType.Misc3
            doomEdNum: 2015,                                // doomEdNum
            spawnState: MobjState.Bon2,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                                           // MobjType.Misc4
            doomEdNum: 5,                                       // doomEdNum
            spawnState: MobjState.Bkey,                         // spawnState
            spawnHealth: 1000,                                  // spawnHealth
            seeState: MobjState.Null,                           // seeState
            seeSound: Sfx.NONE,                                 // seeSound
            reactionTime: 8,                                    // reactionTime
            attackSound: Sfx.NONE,                              // attackSound
            painState: MobjState.Null,                          // painState
            painChance: 0,                                      // painChance
            painSound: Sfx.NONE,                                // painSound
            meleeState: MobjState.Null,                         // meleeState
            missileState: MobjState.Null,                       // missileState
            deathState: MobjState.Null,                         // deathState
            xdeathState: MobjState.Null,                        // xdeathState
            deathSound: Sfx.NONE,                               // deathSound
            speed: 0,                                           // speed
            radius: Fixed.FromInt(value: 20),                   // radius
            height: Fixed.FromInt(value: 16),                   // height
            mass: 100,                                          // mass
            damage: 0,                                          // damage
            activeSound: Sfx.NONE,                              // activeSound
            flags: MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
            raiseState: MobjState.Null                          // raiseState
        ),

        new MobjInfo(                                           // MobjType.Misc5
            doomEdNum: 13,                                      // doomEdNum
            spawnState: MobjState.Rkey,                         // spawnState
            spawnHealth: 1000,                                  // spawnHealth
            seeState: MobjState.Null,                           // seeState
            seeSound: Sfx.NONE,                                 // seeSound
            reactionTime: 8,                                    // reactionTime
            attackSound: Sfx.NONE,                              // attackSound
            painState: MobjState.Null,                          // painState
            painChance: 0,                                      // painChance
            painSound: Sfx.NONE,                                // painSound
            meleeState: MobjState.Null,                         // meleeState
            missileState: MobjState.Null,                       // missileState
            deathState: MobjState.Null,                         // deathState
            xdeathState: MobjState.Null,                        // xdeathState
            deathSound: Sfx.NONE,                               // deathSound
            speed: 0,                                           // speed
            radius: Fixed.FromInt(value: 20),                   // radius
            height: Fixed.FromInt(value: 16),                   // height
            mass: 100,                                          // mass
            damage: 0,                                          // damage
            activeSound: Sfx.NONE,                              // activeSound
            flags: MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
            raiseState: MobjState.Null                          // raiseState
        ),

        new MobjInfo(                                           // MobjType.Misc6
            doomEdNum: 6,                                       // doomEdNum
            spawnState: MobjState.Ykey,                         // spawnState
            spawnHealth: 1000,                                  // spawnHealth
            seeState: MobjState.Null,                           // seeState
            seeSound: Sfx.NONE,                                 // seeSound
            reactionTime: 8,                                    // reactionTime
            attackSound: Sfx.NONE,                              // attackSound
            painState: MobjState.Null,                          // painState
            painChance: 0,                                      // painChance
            painSound: Sfx.NONE,                                // painSound
            meleeState: MobjState.Null,                         // meleeState
            missileState: MobjState.Null,                       // missileState
            deathState: MobjState.Null,                         // deathState
            xdeathState: MobjState.Null,                        // xdeathState
            deathSound: Sfx.NONE,                               // deathSound
            speed: 0,                                           // speed
            radius: Fixed.FromInt(value: 20),                   // radius
            height: Fixed.FromInt(value: 16),                   // height
            mass: 100,                                          // mass
            damage: 0,                                          // damage
            activeSound: Sfx.NONE,                              // activeSound
            flags: MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
            raiseState: MobjState.Null                          // raiseState
        ),

        new MobjInfo(                                           // MobjType.Misc7
            doomEdNum: 39,                                      // doomEdNum
            spawnState: MobjState.Yskull,                       // spawnState
            spawnHealth: 1000,                                  // spawnHealth
            seeState: MobjState.Null,                           // seeState
            seeSound: Sfx.NONE,                                 // seeSound
            reactionTime: 8,                                    // reactionTime
            attackSound: Sfx.NONE,                              // attackSound
            painState: MobjState.Null,                          // painState
            painChance: 0,                                      // painChance
            painSound: Sfx.NONE,                                // painSound
            meleeState: MobjState.Null,                         // meleeState
            missileState: MobjState.Null,                       // missileState
            deathState: MobjState.Null,                         // deathState
            xdeathState: MobjState.Null,                        // xdeathState
            deathSound: Sfx.NONE,                               // deathSound
            speed: 0,                                           // speed
            radius: Fixed.FromInt(value: 20),                   // radius
            height: Fixed.FromInt(value: 16),                   // height
            mass: 100,                                          // mass
            damage: 0,                                          // damage
            activeSound: Sfx.NONE,                              // activeSound
            flags: MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
            raiseState: MobjState.Null                          // raiseState
        ),

        new MobjInfo(                                           // MobjType.Misc8
            doomEdNum: 38,                                      // doomEdNum
            spawnState: MobjState.Rskull,                       // spawnState
            spawnHealth: 1000,                                  // spawnHealth
            seeState: MobjState.Null,                           // seeState
            seeSound: Sfx.NONE,                                 // seeSound
            reactionTime: 8,                                    // reactionTime
            attackSound: Sfx.NONE,                              // attackSound
            painState: MobjState.Null,                          // painState
            painChance: 0,                                      // painChance
            painSound: Sfx.NONE,                                // painSound
            meleeState: MobjState.Null,                         // meleeState
            missileState: MobjState.Null,                       // missileState
            deathState: MobjState.Null,                         // deathState
            xdeathState: MobjState.Null,                        // xdeathState
            deathSound: Sfx.NONE,                               // deathSound
            speed: 0,                                           // speed
            radius: Fixed.FromInt(value: 20),                   // radius
            height: Fixed.FromInt(value: 16),                   // height
            mass: 100,                                          // mass
            damage: 0,                                          // damage
            activeSound: Sfx.NONE,                              // activeSound
            flags: MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
            raiseState: MobjState.Null                          // raiseState
        ),

        new MobjInfo(                                           // MobjType.Misc9
            doomEdNum: 40,                                      // doomEdNum
            spawnState: MobjState.Bskull,                       // spawnState
            spawnHealth: 1000,                                  // spawnHealth
            seeState: MobjState.Null,                           // seeState
            seeSound: Sfx.NONE,                                 // seeSound
            reactionTime: 8,                                    // reactionTime
            attackSound: Sfx.NONE,                              // attackSound
            painState: MobjState.Null,                          // painState
            painChance: 0,                                      // painChance
            painSound: Sfx.NONE,                                // painSound
            meleeState: MobjState.Null,                         // meleeState
            missileState: MobjState.Null,                       // missileState
            deathState: MobjState.Null,                         // deathState
            xdeathState: MobjState.Null,                        // xdeathState
            deathSound: Sfx.NONE,                               // deathSound
            speed: 0,                                           // speed
            radius: Fixed.FromInt(value: 20),                   // radius
            height: Fixed.FromInt(value: 16),                   // height
            mass: 100,                                          // mass
            damage: 0,                                          // damage
            activeSound: Sfx.NONE,                              // activeSound
            flags: MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
            raiseState: MobjState.Null                          // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc10
            doomEdNum: 2011,                  // doomEdNum
            spawnState: MobjState.Stim,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc11
            doomEdNum: 2012,                  // doomEdNum
            spawnState: MobjState.Medi,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                                       // MobjType.Misc12
            doomEdNum: 2013,                                // doomEdNum
            spawnState: MobjState.Soul,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                                       // MobjType.Inv
            doomEdNum: 2022,                                // doomEdNum
            spawnState: MobjState.Pinv,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                                       // MobjType.Misc13
            doomEdNum: 2023,                                // doomEdNum
            spawnState: MobjState.Pstr,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                                       // MobjType.Ins
            doomEdNum: 2024,                                // doomEdNum
            spawnState: MobjState.Pins,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc14
            doomEdNum: 2025,                  // doomEdNum
            spawnState: MobjState.Suit,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                                       // MobjType.Misc15
            doomEdNum: 2026,                                // doomEdNum
            spawnState: MobjState.Pmap,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                                       // MobjType.Misc16
            doomEdNum: 2045,                                // doomEdNum
            spawnState: MobjState.Pvis,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                                       // MobjType.Mega
            doomEdNum: 83,                                  // doomEdNum
            spawnState: MobjState.Mega,                     // spawnState
            spawnHealth: 1000,                              // spawnHealth
            seeState: MobjState.Null,                       // seeState
            seeSound: Sfx.NONE,                             // seeSound
            reactionTime: 8,                                // reactionTime
            attackSound: Sfx.NONE,                          // attackSound
            painState: MobjState.Null,                      // painState
            painChance: 0,                                  // painChance
            painSound: Sfx.NONE,                            // painSound
            meleeState: MobjState.Null,                     // meleeState
            missileState: MobjState.Null,                   // missileState
            deathState: MobjState.Null,                     // deathState
            xdeathState: MobjState.Null,                    // xdeathState
            deathSound: Sfx.NONE,                           // deathSound
            speed: 0,                                       // speed
            radius: Fixed.FromInt(value: 20),               // radius
            height: Fixed.FromInt(value: 16),               // height
            mass: 100,                                      // mass
            damage: 0,                                      // damage
            activeSound: Sfx.NONE,                          // activeSound
            flags: MobjFlags.Special | MobjFlags.CountItem, // flags
            raiseState: MobjState.Null                      // raiseState
        ),

        new MobjInfo(                         // MobjType.Clip
            doomEdNum: 2007,                  // doomEdNum
            spawnState: MobjState.Clip,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc17
            doomEdNum: 2048,                  // doomEdNum
            spawnState: MobjState.Ammo,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc18
            doomEdNum: 2010,                  // doomEdNum
            spawnState: MobjState.Rock,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc19
            doomEdNum: 2046,                  // doomEdNum
            spawnState: MobjState.Brok,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc20
            doomEdNum: 2047,                  // doomEdNum
            spawnState: MobjState.Cell,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc21
            doomEdNum: 17,                    // doomEdNum
            spawnState: MobjState.Celp,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc22
            doomEdNum: 2008,                  // doomEdNum
            spawnState: MobjState.Shel,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc23
            doomEdNum: 2049,                  // doomEdNum
            spawnState: MobjState.Sbox,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc24
            doomEdNum: 8,                     // doomEdNum
            spawnState: MobjState.Bpak,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc25
            doomEdNum: 2006,                  // doomEdNum
            spawnState: MobjState.Bfug,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Chaingun
            doomEdNum: 2002,                  // doomEdNum
            spawnState: MobjState.Mgun,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc26
            doomEdNum: 2005,                  // doomEdNum
            spawnState: MobjState.Csaw,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc27
            doomEdNum: 2003,                  // doomEdNum
            spawnState: MobjState.Laun,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc28
            doomEdNum: 2004,                  // doomEdNum
            spawnState: MobjState.Plas,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Shotgun
            doomEdNum: 2001,                  // doomEdNum
            spawnState: MobjState.Shot,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Supershotgun
            doomEdNum: 82,                    // doomEdNum
            spawnState: MobjState.Shot2,      // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Special,         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc29
            doomEdNum: 85,                    // doomEdNum
            spawnState: MobjState.Techlamp,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc30
            doomEdNum: 86,                    // doomEdNum
            spawnState: MobjState.Tech2Lamp,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc31
            doomEdNum: 2028,                  // doomEdNum
            spawnState: MobjState.Colu,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc32
            doomEdNum: 30,                    // doomEdNum
            spawnState: MobjState.Tallgrncol, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc33
            doomEdNum: 31,                    // doomEdNum
            spawnState: MobjState.Shrtgrncol, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc34
            doomEdNum: 32,                    // doomEdNum
            spawnState: MobjState.Tallredcol, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc35
            doomEdNum: 33,                    // doomEdNum
            spawnState: MobjState.Shrtredcol, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc36
            doomEdNum: 37,                    // doomEdNum
            spawnState: MobjState.Skullcol,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc37
            doomEdNum: 36,                    // doomEdNum
            spawnState: MobjState.Heartcol,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc38
            doomEdNum: 41,                    // doomEdNum
            spawnState: MobjState.Evileye,    // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc39
            doomEdNum: 42,                    // doomEdNum
            spawnState: MobjState.Floatskull, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc40
            doomEdNum: 43,                    // doomEdNum
            spawnState: MobjState.Torchtree,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc41
            doomEdNum: 44,                    // doomEdNum
            spawnState: MobjState.Bluetorch,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc42
            doomEdNum: 45,                    // doomEdNum
            spawnState: MobjState.Greentorch, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc43
            doomEdNum: 46,                    // doomEdNum
            spawnState: MobjState.Redtorch,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc44
            doomEdNum: 55,                    // doomEdNum
            spawnState: MobjState.Btorchshrt, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc45
            doomEdNum: 56,                    // doomEdNum
            spawnState: MobjState.Gtorchshrt, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc46
            doomEdNum: 57,                    // doomEdNum
            spawnState: MobjState.Rtorchshrt, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc47
            doomEdNum: 47,                    // doomEdNum
            spawnState: MobjState.Stalagtite, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc48
            doomEdNum: 48,                    // doomEdNum
            spawnState: MobjState.Techpillar, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc49
            doomEdNum: 34,                    // doomEdNum
            spawnState: MobjState.Candlestik, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc50
            doomEdNum: 35,                    // doomEdNum
            spawnState: MobjState.Candelabra, // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc51
            doomEdNum: 49,                                                         // doomEdNum
            spawnState: MobjState.Bloodytwitch,                                    // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 68),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc52
            doomEdNum: 50,                                                         // doomEdNum
            spawnState: MobjState.Meat2,                                           // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 84),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc53
            doomEdNum: 51,                                                         // doomEdNum
            spawnState: MobjState.Meat3,                                           // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 84),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc54
            doomEdNum: 52,                                                         // doomEdNum
            spawnState: MobjState.Meat4,                                           // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 68),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc55
            doomEdNum: 53,                                                         // doomEdNum
            spawnState: MobjState.Meat5,                                           // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 52),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                            // MobjType.Misc56
            doomEdNum: 59,                                       // doomEdNum
            spawnState: MobjState.Meat2,                         // spawnState
            spawnHealth: 1000,                                   // spawnHealth
            seeState: MobjState.Null,                            // seeState
            seeSound: Sfx.NONE,                                  // seeSound
            reactionTime: 8,                                     // reactionTime
            attackSound: Sfx.NONE,                               // attackSound
            painState: MobjState.Null,                           // painState
            painChance: 0,                                       // painChance
            painSound: Sfx.NONE,                                 // painSound
            meleeState: MobjState.Null,                          // meleeState
            missileState: MobjState.Null,                        // missileState
            deathState: MobjState.Null,                          // deathState
            xdeathState: MobjState.Null,                         // xdeathState
            deathSound: Sfx.NONE,                                // deathSound
            speed: 0,                                            // speed
            radius: Fixed.FromInt(value: 20),                    // radius
            height: Fixed.FromInt(value: 84),                    // height
            mass: 100,                                           // mass
            damage: 0,                                           // damage
            activeSound: Sfx.NONE,                               // activeSound
            flags: MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                           // raiseState
        ),

        new MobjInfo(                                            // MobjType.Misc57
            doomEdNum: 60,                                       // doomEdNum
            spawnState: MobjState.Meat4,                         // spawnState
            spawnHealth: 1000,                                   // spawnHealth
            seeState: MobjState.Null,                            // seeState
            seeSound: Sfx.NONE,                                  // seeSound
            reactionTime: 8,                                     // reactionTime
            attackSound: Sfx.NONE,                               // attackSound
            painState: MobjState.Null,                           // painState
            painChance: 0,                                       // painChance
            painSound: Sfx.NONE,                                 // painSound
            meleeState: MobjState.Null,                          // meleeState
            missileState: MobjState.Null,                        // missileState
            deathState: MobjState.Null,                          // deathState
            xdeathState: MobjState.Null,                         // xdeathState
            deathSound: Sfx.NONE,                                // deathSound
            speed: 0,                                            // speed
            radius: Fixed.FromInt(value: 20),                    // radius
            height: Fixed.FromInt(value: 68),                    // height
            mass: 100,                                           // mass
            damage: 0,                                           // damage
            activeSound: Sfx.NONE,                               // activeSound
            flags: MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                           // raiseState
        ),

        new MobjInfo(                                            // MobjType.Misc58
            doomEdNum: 61,                                       // doomEdNum
            spawnState: MobjState.Meat3,                         // spawnState
            spawnHealth: 1000,                                   // spawnHealth
            seeState: MobjState.Null,                            // seeState
            seeSound: Sfx.NONE,                                  // seeSound
            reactionTime: 8,                                     // reactionTime
            attackSound: Sfx.NONE,                               // attackSound
            painState: MobjState.Null,                           // painState
            painChance: 0,                                       // painChance
            painSound: Sfx.NONE,                                 // painSound
            meleeState: MobjState.Null,                          // meleeState
            missileState: MobjState.Null,                        // missileState
            deathState: MobjState.Null,                          // deathState
            xdeathState: MobjState.Null,                         // xdeathState
            deathSound: Sfx.NONE,                                // deathSound
            speed: 0,                                            // speed
            radius: Fixed.FromInt(value: 20),                    // radius
            height: Fixed.FromInt(value: 52),                    // height
            mass: 100,                                           // mass
            damage: 0,                                           // damage
            activeSound: Sfx.NONE,                               // activeSound
            flags: MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                           // raiseState
        ),

        new MobjInfo(                                            // MobjType.Misc59
            doomEdNum: 62,                                       // doomEdNum
            spawnState: MobjState.Meat5,                         // spawnState
            spawnHealth: 1000,                                   // spawnHealth
            seeState: MobjState.Null,                            // seeState
            seeSound: Sfx.NONE,                                  // seeSound
            reactionTime: 8,                                     // reactionTime
            attackSound: Sfx.NONE,                               // attackSound
            painState: MobjState.Null,                           // painState
            painChance: 0,                                       // painChance
            painSound: Sfx.NONE,                                 // painSound
            meleeState: MobjState.Null,                          // meleeState
            missileState: MobjState.Null,                        // missileState
            deathState: MobjState.Null,                          // deathState
            xdeathState: MobjState.Null,                         // xdeathState
            deathSound: Sfx.NONE,                                // deathSound
            speed: 0,                                            // speed
            radius: Fixed.FromInt(value: 20),                    // radius
            height: Fixed.FromInt(value: 52),                    // height
            mass: 100,                                           // mass
            damage: 0,                                           // damage
            activeSound: Sfx.NONE,                               // activeSound
            flags: MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                           // raiseState
        ),

        new MobjInfo(                                            // MobjType.Misc60
            doomEdNum: 63,                                       // doomEdNum
            spawnState: MobjState.Bloodytwitch,                  // spawnState
            spawnHealth: 1000,                                   // spawnHealth
            seeState: MobjState.Null,                            // seeState
            seeSound: Sfx.NONE,                                  // seeSound
            reactionTime: 8,                                     // reactionTime
            attackSound: Sfx.NONE,                               // attackSound
            painState: MobjState.Null,                           // painState
            painChance: 0,                                       // painChance
            painSound: Sfx.NONE,                                 // painSound
            meleeState: MobjState.Null,                          // meleeState
            missileState: MobjState.Null,                        // missileState
            deathState: MobjState.Null,                          // deathState
            xdeathState: MobjState.Null,                         // xdeathState
            deathSound: Sfx.NONE,                                // deathSound
            speed: 0,                                            // speed
            radius: Fixed.FromInt(value: 20),                    // radius
            height: Fixed.FromInt(value: 68),                    // height
            mass: 100,                                           // mass
            damage: 0,                                           // damage
            activeSound: Sfx.NONE,                               // activeSound
            flags: MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                           // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc61
            doomEdNum: 22,                    // doomEdNum
            spawnState: MobjState.HeadDie6,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc62
            doomEdNum: 15,                    // doomEdNum
            spawnState: MobjState.PlayDie7,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc63
            doomEdNum: 18,                    // doomEdNum
            spawnState: MobjState.PossDie5,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc64
            doomEdNum: 21,                    // doomEdNum
            spawnState: MobjState.SargDie6,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc65
            doomEdNum: 23,                    // doomEdNum
            spawnState: MobjState.SkullDie6,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc66
            doomEdNum: 20,                    // doomEdNum
            spawnState: MobjState.TrooDie5,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc67
            doomEdNum: 19,                    // doomEdNum
            spawnState: MobjState.SposDie5,   // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc68
            doomEdNum: 10,                    // doomEdNum
            spawnState: MobjState.PlayXdie9,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc69
            doomEdNum: 12,                    // doomEdNum
            spawnState: MobjState.PlayXdie9,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                           // MobjType.Misc70
            doomEdNum: 28,                      // doomEdNum
            spawnState: MobjState.Headsonstick, // spawnState
            spawnHealth: 1000,                  // spawnHealth
            seeState: MobjState.Null,           // seeState
            seeSound: Sfx.NONE,                 // seeSound
            reactionTime: 8,                    // reactionTime
            attackSound: Sfx.NONE,              // attackSound
            painState: MobjState.Null,          // painState
            painChance: 0,                      // painChance
            painSound: Sfx.NONE,                // painSound
            meleeState: MobjState.Null,         // meleeState
            missileState: MobjState.Null,       // missileState
            deathState: MobjState.Null,         // deathState
            xdeathState: MobjState.Null,        // xdeathState
            deathSound: Sfx.NONE,               // deathSound
            speed: 0,                           // speed
            radius: Fixed.FromInt(value: 16),   // radius
            height: Fixed.FromInt(value: 16),   // height
            mass: 100,                          // mass
            damage: 0,                          // damage
            activeSound: Sfx.NONE,              // activeSound
            flags: MobjFlags.Solid,             // flags
            raiseState: MobjState.Null          // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc71
            doomEdNum: 24,                    // doomEdNum
            spawnState: MobjState.Gibs,       // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: 0,                         // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                           // MobjType.Misc72
            doomEdNum: 27,                      // doomEdNum
            spawnState: MobjState.Headonastick, // spawnState
            spawnHealth: 1000,                  // spawnHealth
            seeState: MobjState.Null,           // seeState
            seeSound: Sfx.NONE,                 // seeSound
            reactionTime: 8,                    // reactionTime
            attackSound: Sfx.NONE,              // attackSound
            painState: MobjState.Null,          // painState
            painChance: 0,                      // painChance
            painSound: Sfx.NONE,                // painSound
            meleeState: MobjState.Null,         // meleeState
            missileState: MobjState.Null,       // missileState
            deathState: MobjState.Null,         // deathState
            xdeathState: MobjState.Null,        // xdeathState
            deathSound: Sfx.NONE,               // deathSound
            speed: 0,                           // speed
            radius: Fixed.FromInt(value: 16),   // radius
            height: Fixed.FromInt(value: 16),   // height
            mass: 100,                          // mass
            damage: 0,                          // damage
            activeSound: Sfx.NONE,              // activeSound
            flags: MobjFlags.Solid,             // flags
            raiseState: MobjState.Null          // raiseState
        ),

        new MobjInfo(                          // MobjType.Misc73
            doomEdNum: 29,                     // doomEdNum
            spawnState: MobjState.Headcandles, // spawnState
            spawnHealth: 1000,                 // spawnHealth
            seeState: MobjState.Null,          // seeState
            seeSound: Sfx.NONE,                // seeSound
            reactionTime: 8,                   // reactionTime
            attackSound: Sfx.NONE,             // attackSound
            painState: MobjState.Null,         // painState
            painChance: 0,                     // painChance
            painSound: Sfx.NONE,               // painSound
            meleeState: MobjState.Null,        // meleeState
            missileState: MobjState.Null,      // missileState
            deathState: MobjState.Null,        // deathState
            xdeathState: MobjState.Null,       // xdeathState
            deathSound: Sfx.NONE,              // deathSound
            speed: 0,                          // speed
            radius: Fixed.FromInt(value: 16),  // radius
            height: Fixed.FromInt(value: 16),  // height
            mass: 100,                         // mass
            damage: 0,                         // damage
            activeSound: Sfx.NONE,             // activeSound
            flags: MobjFlags.Solid,            // flags
            raiseState: MobjState.Null         // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc74
            doomEdNum: 25,                    // doomEdNum
            spawnState: MobjState.Deadstick,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc75
            doomEdNum: 26,                    // doomEdNum
            spawnState: MobjState.Livestick,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc76
            doomEdNum: 54,                    // doomEdNum
            spawnState: MobjState.Bigtree,    // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 32), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc77
            doomEdNum: 70,                    // doomEdNum
            spawnState: MobjState.Bbar1,      // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 16), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.Solid,           // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc78
            doomEdNum: 73,                                                         // doomEdNum
            spawnState: MobjState.Hangnoguts,                                      // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 88),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc79
            doomEdNum: 74,                                                         // doomEdNum
            spawnState: MobjState.Hangbnobrain,                                    // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 88),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc80
            doomEdNum: 75,                                                         // doomEdNum
            spawnState: MobjState.Hangtlookdn,                                     // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 64),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc81
            doomEdNum: 76,                                                         // doomEdNum
            spawnState: MobjState.Hangtskull,                                      // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 64),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc82
            doomEdNum: 77,                                                         // doomEdNum
            spawnState: MobjState.Hangtlookup,                                     // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 64),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                                                              // MobjType.Misc83
            doomEdNum: 78,                                                         // doomEdNum
            spawnState: MobjState.Hangtnobrain,                                    // spawnState
            spawnHealth: 1000,                                                     // spawnHealth
            seeState: MobjState.Null,                                              // seeState
            seeSound: Sfx.NONE,                                                    // seeSound
            reactionTime: 8,                                                       // reactionTime
            attackSound: Sfx.NONE,                                                 // attackSound
            painState: MobjState.Null,                                             // painState
            painChance: 0,                                                         // painChance
            painSound: Sfx.NONE,                                                   // painSound
            meleeState: MobjState.Null,                                            // meleeState
            missileState: MobjState.Null,                                          // missileState
            deathState: MobjState.Null,                                            // deathState
            xdeathState: MobjState.Null,                                           // xdeathState
            deathSound: Sfx.NONE,                                                  // deathSound
            speed: 0,                                                              // speed
            radius: Fixed.FromInt(value: 16),                                      // radius
            height: Fixed.FromInt(value: 64),                                      // height
            mass: 100,                                                             // mass
            damage: 0,                                                             // damage
            activeSound: Sfx.NONE,                                                 // activeSound
            flags: MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
            raiseState: MobjState.Null                                             // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc84
            doomEdNum: 79,                    // doomEdNum
            spawnState: MobjState.Colongibs,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.NoBlockMap,      // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc85
            doomEdNum: 80,                    // doomEdNum
            spawnState: MobjState.Smallpool,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.NoBlockMap,      // flags
            raiseState: MobjState.Null        // raiseState
        ),

        new MobjInfo(                         // MobjType.Misc86
            doomEdNum: 81,                    // doomEdNum
            spawnState: MobjState.Brainstem,  // spawnState
            spawnHealth: 1000,                // spawnHealth
            seeState: MobjState.Null,         // seeState
            seeSound: Sfx.NONE,               // seeSound
            reactionTime: 8,                  // reactionTime
            attackSound: Sfx.NONE,            // attackSound
            painState: MobjState.Null,        // painState
            painChance: 0,                    // painChance
            painSound: Sfx.NONE,              // painSound
            meleeState: MobjState.Null,       // meleeState
            missileState: MobjState.Null,     // missileState
            deathState: MobjState.Null,       // deathState
            xdeathState: MobjState.Null,      // xdeathState
            deathSound: Sfx.NONE,             // deathSound
            speed: 0,                         // speed
            radius: Fixed.FromInt(value: 20), // radius
            height: Fixed.FromInt(value: 16), // height
            mass: 100,                        // mass
            damage: 0,                        // damage
            activeSound: Sfx.NONE,            // activeSound
            flags: MobjFlags.NoBlockMap,      // flags
            raiseState: MobjState.Null        // raiseState
        )
    ];
}