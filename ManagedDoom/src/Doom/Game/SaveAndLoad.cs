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
using System.IO;
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Game
{
    /// <summary>
    /// Vanilla-compatible save and load, full of messy binary handling code.
    /// </summary>
    public static class SaveAndLoad
    {
        public const int DescriptionSize = 24;

        private const int versionSize = 16;
        private const int saveBufferSize = 360 * 1024;

        private enum ThinkerClass
        {
            End,
            Mobj
        }

        private enum SpecialClass
        {
            Ceiling,
            Door,
            Floor,
            Plat,
            Flash,
            Strobe,
            Glow,
            EndSpecials
        }

        public static void Save(DoomGame game, string description, string path)
        {
            var sg = new SaveGame(description);
            sg.Save(game, path);
        }

        public static void Load(DoomGame game, string path)
        {
            var options = game.Options;
            game.InitNew(options.Skill, options.Episode, options.Map);

            var lg = new LoadGame(File.ReadAllBytes(path));
            lg.Load(game);
        }


        ////////////////////////////////////////////////////////////
        // Save game
        ////////////////////////////////////////////////////////////

        private class SaveGame
        {
            private readonly byte[] data;
            private int ptr;

            public SaveGame(string description)
            {
                data = new byte[saveBufferSize];
                ptr = 0;

                WriteDescription(description);
                WriteVersion();
            }

            private void WriteDescription(string description)
            {
                for (var i = 0; i < description.Length; i++)
                {
                    data[i] = (byte)description[i];
                }

                ptr += DescriptionSize;
            }

            private void WriteVersion()
            {
                const string version = "version 109";
                for (var i = 0; i < version.Length; i++)
                {
                    data[ptr + i] = (byte)version[i];
                }

                ptr += versionSize;
            }

            public void Save(DoomGame game, string path)
            {
                var options = game.World.Options;
                data[ptr++] = (byte)options.Skill;
                data[ptr++] = (byte)options.Episode;
                data[ptr++] = (byte)options.Map;
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    data[ptr++] = options.Players[i].InGame ? (byte)1 : (byte)0;
                }

                data[ptr++] = (byte)(game.World.LevelTime >> 16);
                data[ptr++] = (byte)(game.World.LevelTime >> 8);
                data[ptr++] = (byte)(game.World.LevelTime);

                ArchivePlayers(game.World);
                ArchiveWorld(game.World);
                ArchiveThinkers(game.World);
                ArchiveSpecials(game.World);

                data[ptr++] = 0x1d;

                using var writer = new FileStream(path, FileMode.Create, FileAccess.Write);
                writer.Write(data, 0, ptr);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int PadPointer(int p)
            {
                return (4 - (p & 3)) & 3;
            }

            private void ArchivePlayers(World.World world)
            {
                var players = world.Options.Players;
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (!players[i].InGame)
                        continue;

                    ptr = PadPointer(ptr);
                    ptr = ArchivePlayer(players[i], data, ptr);
                }
            }

            private void ArchiveWorld(World.World world)
            {
                // Do sectors.
                var sectors = world.Map.Sectors;
                foreach (var sector in sectors)
                    ptr = ArchiveSector(sector, data, ptr);

                // Do lines.
                var lines = world.Map.Lines;
                foreach (var line in lines)
                    ptr = ArchiveLine(line, data, ptr);
            }

            private void ArchiveThinkers(World.World world)
            {
                var thinkers = world.Thinkers;

                // Read in saved thinkers.
                foreach (var thinker in thinkers)
                {
                    if (thinker is Mobj mobj)
                    {
                        data[ptr++] = (byte)ThinkerClass.Mobj;
                        ptr = PadPointer(ptr);

                        WriteThinkerState(data, ptr + 8, mobj.ThinkerState);
                        Write(data, ptr + 12, mobj.X.Data);
                        Write(data, ptr + 16, mobj.Y.Data);
                        Write(data, ptr + 20, mobj.Z.Data);
                        Write(data, ptr + 32, mobj.Angle.Data);
                        Write(data, ptr + 36, (int)mobj.Sprite);
                        Write(data, ptr + 40, mobj.Frame);
                        Write(data, ptr + 56, mobj.FloorZ.Data);
                        Write(data, ptr + 60, mobj.CeilingZ.Data);
                        Write(data, ptr + 64, mobj.Radius.Data);
                        Write(data, ptr + 68, mobj.Height.Data);
                        Write(data, ptr + 72, mobj.MomX.Data);
                        Write(data, ptr + 76, mobj.MomY.Data);
                        Write(data, ptr + 80, mobj.MomZ.Data);
                        Write(data, ptr + 88, (int)mobj.Type);
                        Write(data, ptr + 96, mobj.Tics);
                        Write(data, ptr + 100, mobj.State.Number);
                        Write(data, ptr + 104, (int)mobj.Flags);
                        Write(data, ptr + 108, mobj.Health);
                        Write(data, ptr + 112, (int)mobj.MoveDir);
                        Write(data, ptr + 116, mobj.MoveCount);
                        Write(data, ptr + 124, mobj.ReactionTime);
                        Write(data, ptr + 128, mobj.Threshold);
                        if (mobj.Player == null)
                        {
                            Write(data, ptr + 132, 0);
                        }
                        else
                        {
                            Write(data, ptr + 132, mobj.Player.Number + 1);
                        }

                        Write(data, ptr + 136, mobj.LastLook);
                        if (mobj.SpawnPoint == null)
                        {
                            Write(data, ptr + 140, (short)0);
                            Write(data, ptr + 142, (short)0);
                            Write(data, ptr + 144, (short)0);
                            Write(data, ptr + 146, (short)0);
                            Write(data, ptr + 148, (short)0);
                        }
                        else
                        {
                            Write(data, ptr + 140, (short)mobj.SpawnPoint.X.ToIntFloor());
                            Write(data, ptr + 142, (short)mobj.SpawnPoint.Y.ToIntFloor());
                            Write(data, ptr + 144, (short)System.Math.Round(mobj.SpawnPoint.Angle.ToDegree()));
                            Write(data, ptr + 146, (short)mobj.SpawnPoint.Type);
                            Write(data, ptr + 148, (short)mobj.SpawnPoint.Flags);
                        }

                        ptr += 154;
                    }
                }

                data[ptr++] = (byte)ThinkerClass.End;
            }

            private void ArchiveSpecials(World.World world)
            {
                var thinkers = world.Thinkers;
                var sa = world.SectorAction;

                // Read in saved thinkers.
                foreach (var thinker in thinkers)
                {
                    if (thinker.ThinkerState == ThinkerState.InStasis)
                    {
                        if (thinker is not CeilingMove ceiling)
                            continue;

                        if (sa.CheckActiveCeiling(ceiling))
                        {
                            data[ptr++] = (byte)SpecialClass.Ceiling;
                            ptr = PadPointer(ptr);
                            WriteThinkerState(data, ptr + 8, ceiling.ThinkerState);
                            Write(data, ptr + 12, (int)ceiling.Type);
                            Write(data, ptr + 16, ceiling.Sector.Number);
                            Write(data, ptr + 20, ceiling.BottomHeight.Data);
                            Write(data, ptr + 24, ceiling.TopHeight.Data);
                            Write(data, ptr + 28, ceiling.Speed.Data);
                            Write(data, ptr + 32, ceiling.Crush ? 1 : 0);
                            Write(data, ptr + 36, ceiling.Direction);
                            Write(data, ptr + 40, ceiling.Tag);
                            Write(data, ptr + 44, ceiling.OldDirection);
                            ptr += 48;
                        }

                        continue;
                    }

                    switch (thinker)
                    {
                        case CeilingMove ceilingMove:
                            WriteCeilingMove(ceilingMove);
                            continue;
                        case VerticalDoor door:
                            WriteVerticalDoor(door);
                            continue;
                        case FloorMove floor:
                            WriteFloorMove(floor);
                            continue;
                        case Platform plat:
                            WritePlatform(plat);
                            continue;
                        case LightFlash flash:
                            WriteLightFlash(flash);
                            continue;
                        case StrobeFlash strobe:
                            WriteStrobeFlash(strobe);
                            continue;
                        case GlowingLight glow:
                            WriteGlowingLight(glow);
                            break;
                    }
                }

                data[ptr++] = (byte)SpecialClass.EndSpecials;
            }

            private void WriteCeilingMove(CeilingMove ceilingMove)
            {
                data[ptr++] = (byte)SpecialClass.Ceiling;
                ptr = PadPointer(ptr);
                WriteThinkerState(data, ptr + 8, ceilingMove.ThinkerState);
                Write(data, ptr + 12, (int)ceilingMove.Type);
                Write(data, ptr + 16, ceilingMove.Sector.Number);
                Write(data, ptr + 20, ceilingMove.BottomHeight.Data);
                Write(data, ptr + 24, ceilingMove.TopHeight.Data);
                Write(data, ptr + 28, ceilingMove.Speed.Data);
                Write(data, ptr + 32, ceilingMove.Crush ? 1 : 0);
                Write(data, ptr + 36, ceilingMove.Direction);
                Write(data, ptr + 40, ceilingMove.Tag);
                Write(data, ptr + 44, ceilingMove.OldDirection);
                ptr += 48;
            }

            private void WriteVerticalDoor(VerticalDoor door)
            {
                data[ptr++] = (byte)SpecialClass.Door;
                ptr = PadPointer(ptr);
                WriteThinkerState(data, ptr + 8, door.ThinkerState);
                Write(data, ptr + 12, (int)door.Type);
                Write(data, ptr + 16, door.Sector.Number);
                Write(data, ptr + 20, door.TopHeight.Data);
                Write(data, ptr + 24, door.Speed.Data);
                Write(data, ptr + 28, door.Direction);
                Write(data, ptr + 32, door.TopWait);
                Write(data, ptr + 36, door.TopCountDown);
                ptr += 40;
            }

            private void WriteFloorMove(FloorMove floor)
            {
                data[ptr++] = (byte)SpecialClass.Floor;
                ptr = PadPointer(ptr);
                WriteThinkerState(data, ptr + 8, floor.ThinkerState);
                Write(data, ptr + 12, (int)floor.Type);
                Write(data, ptr + 16, floor.Crush ? 1 : 0);
                Write(data, ptr + 20, floor.Sector.Number);
                Write(data, ptr + 24, floor.Direction);
                Write(data, ptr + 28, (int)floor.NewSpecial);
                Write(data, ptr + 32, floor.Texture);
                Write(data, ptr + 36, floor.FloorDestHeight.Data);
                Write(data, ptr + 40, floor.Speed.Data);
                ptr += 44;
            }

            private void WritePlatform(Platform plat)
            {
                data[ptr++] = (byte)SpecialClass.Plat;
                ptr = PadPointer(ptr);
                WriteThinkerState(data, ptr + 8, plat.ThinkerState);
                Write(data, ptr + 12, plat.Sector.Number);
                Write(data, ptr + 16, plat.Speed.Data);
                Write(data, ptr + 20, plat.Low.Data);
                Write(data, ptr + 24, plat.High.Data);
                Write(data, ptr + 28, plat.Wait);
                Write(data, ptr + 32, plat.Count);
                Write(data, ptr + 36, (int)plat.Status);
                Write(data, ptr + 40, (int)plat.OldStatus);
                Write(data, ptr + 44, plat.Crush ? 1 : 0);
                Write(data, ptr + 48, plat.Tag);
                Write(data, ptr + 52, (int)plat.Type);
                ptr += 56;
            }

            private void WriteLightFlash(LightFlash flash)
            {
                data[ptr++] = (byte)SpecialClass.Flash;
                ptr = PadPointer(ptr);
                WriteThinkerState(data, ptr + 8, flash.ThinkerState);
                Write(data, ptr + 12, flash.Sector.Number);
                Write(data, ptr + 16, flash.Count);
                Write(data, ptr + 20, flash.MaxLight);
                Write(data, ptr + 24, flash.MinLight);
                Write(data, ptr + 28, flash.MaxTime);
                Write(data, ptr + 32, flash.MinTime);
                ptr += 36;
            }

            private void WriteStrobeFlash(StrobeFlash strobe)
            {
                data[ptr++] = (byte)SpecialClass.Strobe;
                ptr = PadPointer(ptr);
                WriteThinkerState(data, ptr + 8, strobe.ThinkerState);
                Write(data, ptr + 12, strobe.Sector.Number);
                Write(data, ptr + 16, strobe.Count);
                Write(data, ptr + 20, strobe.MinLight);
                Write(data, ptr + 24, strobe.MaxLight);
                Write(data, ptr + 28, strobe.DarkTime);
                Write(data, ptr + 32, strobe.BrightTime);
                ptr += 36;
            }

            private void WriteGlowingLight(GlowingLight glow)
            {
                data[ptr++] = (byte)SpecialClass.Glow;
                ptr = PadPointer(ptr);
                WriteThinkerState(data, ptr + 8, glow.ThinkerState);
                Write(data, ptr + 12, glow.Sector.Number);
                Write(data, ptr + 16, glow.MinLight);
                Write(data, ptr + 20, glow.MaxLight);
                Write(data, ptr + 24, glow.Direction);
                ptr += 28;
            }

            private static int ArchivePlayer(Player player, byte[] data, int p)
            {
                Write(data, p + 4, (int)player.PlayerState);
                Write(data, p + 16, player.ViewZ.Data);
                Write(data, p + 20, player.ViewHeight.Data);
                Write(data, p + 24, player.DeltaViewHeight.Data);
                Write(data, p + 28, player.Bob.Data);
                Write(data, p + 32, player.Health);
                Write(data, p + 36, player.ArmorPoints);
                Write(data, p + 40, player.ArmorType);
                for (var i = 0; i < (int)PowerType.Count; i++)
                {
                    Write(data, p + 44 + 4 * i, player.Powers[i]);
                }

                for (var i = 0; i < (int)PowerType.Count; i++)
                {
                    Write(data, p + 68 + 4 * i, player.Cards[i] ? 1 : 0);
                }

                Write(data, p + 92, player.Backpack ? 1 : 0);
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    Write(data, p + 96 + 4 * i, player.Frags[i]);
                }

                Write(data, p + 112, (int)player.ReadyWeapon);
                Write(data, p + 116, (int)player.PendingWeapon);
                for (var i = 0; i < (int)WeaponTypes.Count; i++)
                {
                    Write(data, p + 120 + 4 * i, player.WeaponOwned[i] ? 1 : 0);
                }

                for (var i = 0; i < (int)AmmoType.Count; i++)
                    Write(data, p + 156 + 4 * i, player.Ammo[i]);

                for (var i = 0; i < (int)AmmoType.Count; i++)
                    Write(data, p + 172 + 4 * i, player.MaxAmmo[i]);

                Write(data, p + 188, player.AttackDown ? 1 : 0);
                Write(data, p + 192, player.UseDown ? 1 : 0);
                Write(data, p + 196, (int)player.Cheats);
                Write(data, p + 200, player.Refire);
                Write(data, p + 204, player.KillCount);
                Write(data, p + 208, player.ItemCount);
                Write(data, p + 212, player.SecretCount);
                Write(data, p + 220, player.DamageCount);
                Write(data, p + 224, player.BonusCount);
                Write(data, p + 232, player.ExtraLight);
                Write(data, p + 236, player.FixedColorMap);
                Write(data, p + 240, player.ColorMap);
                for (var i = 0; i < (int)PlayerSprite.Count; i++)
                {
                    var number = player.PlayerSprites[i].State == null ? 0 : player.PlayerSprites[i].State.Number;
                    Write(data, p + 244 + 16 * i, number);
                    Write(data, p + 244 + 16 * i + 4, player.PlayerSprites[i].Tics);
                    Write(data, p + 244 + 16 * i + 8, player.PlayerSprites[i].Sx.Data);
                    Write(data, p + 244 + 16 * i + 12, player.PlayerSprites[i].Sy.Data);
                }

                Write(data, p + 276, player.DidSecret ? 1 : 0);

                return p + 280;
            }

            private static int ArchiveSector(Sector sector, byte[] data, int p)
            {
                Write(data, p, (short)(sector.FloorHeight.ToIntFloor()));
                Write(data, p + 2, (short)(sector.CeilingHeight.ToIntFloor()));
                Write(data, p + 4, (short)sector.FloorFlat);
                Write(data, p + 6, (short)sector.CeilingFlat);
                Write(data, p + 8, (short)sector.LightLevel);
                Write(data, p + 10, (short)sector.Special);
                Write(data, p + 12, (short)sector.Tag);
                return p + 14;
            }

            private static int ArchiveLine(LineDef line, byte[] data, int p)
            {
                Write(data, p, (short)line.Flags);
                Write(data, p + 2, (short)line.Special);
                Write(data, p + 4, line.Tag);
                p += 6;

                if (line.FrontSide != null)
                {
                    var side = line.FrontSide;
                    Write(data, p, (short)side.TextureOffset.ToIntFloor());
                    Write(data, p + 2, (short)side.RowOffset.ToIntFloor());
                    Write(data, p + 4, (short)side.TopTexture);
                    Write(data, p + 6, (short)side.BottomTexture);
                    Write(data, p + 8, (short)side.MiddleTexture);
                    p += 10;
                }

                if (line.BackSide != null)
                {
                    var side = line.BackSide;
                    Write(data, p, (short)side.TextureOffset.ToIntFloor());
                    Write(data, p + 2, (short)side.RowOffset.ToIntFloor());
                    Write(data, p + 4, (short)side.TopTexture);
                    Write(data, p + 6, (short)side.BottomTexture);
                    Write(data, p + 8, (short)side.MiddleTexture);
                    p += 10;
                }

                return p;
            }

            private static void Write(byte[] data, int p, int value)
            {
                // Unsafe.As<byte, int>(ref data[p]) = value;
                data[p] = (byte)value;
                data[p + 1] = (byte)(value >> 8);
                data[p + 2] = (byte)(value >> 16);
                data[p + 3] = (byte)(value >> 24);
            }

            private static void Write(byte[] data, int p, uint value)
            {
                var b = BitConverter.GetBytes(value);
                // Unsafe.As<byte, uint>(ref data[p]) = value;
                data[p] = (byte)value;
                data[p + 1] = (byte)(value >> 8);
                data[p + 2] = (byte)(value >> 16);
                data[p + 3] = (byte)(value >> 24);
                Console.WriteLine("");
            }

            private static void Write(byte[] data, int p, short value)
            {
                data[p] = (byte)value;
                data[p + 1] = (byte)(value >> 8);
            }

            private static void WriteThinkerState(byte[] data, int p, ThinkerState state)
            {
                switch (state)
                {
                    case ThinkerState.InStasis:
                        Write(data, p, 0);
                        break;
                    default:
                        Write(data, p, 1);
                        break;
                }
            }
        }


        ////////////////////////////////////////////////////////////
        // Load game
        ////////////////////////////////////////////////////////////

        private class LoadGame
        {
            private readonly byte[] data;
            private int ptr;

            public LoadGame(byte[] data)
            {
                this.data = data;
                ptr = 0;

                ReadDescription();

                var version = ReadVersion();
                if (version != "VERSION 109")
                    throw new Exception("Unsupported version!");
            }

            public void Load(DoomGame game)
            {
                var options = game.World.Options;
                options.Skill = (GameSkill)data[ptr++];
                options.Episode = data[ptr++];
                options.Map = data[ptr++];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                    options.Players[i].InGame = data[ptr++] != 0;

                game.InitNew(options.Skill, options.Episode, options.Map);

                var a = data[ptr++];
                var b = data[ptr++];
                var c = data[ptr++];
                var levelTime = (a << 16) + (b << 8) + c;

                UnArchivePlayers(game.World);
                UnArchiveWorld(game.World);
                UnArchiveThinkers(game.World);
                UnArchiveSpecials(game.World);

                if (data[ptr] != 0x1d)
                    throw new Exception("Bad savegame!");

                game.World.LevelTime = levelTime;

                options.Sound.SetListener(game.World.ConsolePlayer.Mobj);
            }

            [MethodImpl(MethodImplOptions.AggressiveOptimization)]
            private void PadPointer()
            {
                ptr += (4 - (ptr & 3)) & 3;
            }

            private string ReadDescription()
            {
                var value = DoomInterop.ToString(data.AsSpan(ptr, DescriptionSize));
                ptr += DescriptionSize;
                return value;
            }

            private string ReadVersion()
            {
                var value = DoomInterop.ToString(data.AsSpan(ptr, versionSize));
                ptr += versionSize;
                return value;
            }

            private void UnArchivePlayers(World.World world)
            {
                var players = world.Options.Players;
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (!players[i].InGame)
                        continue;

                    PadPointer();

                    ptr = UnArchivePlayer(players[i], data, ptr);
                }
            }

            private void UnArchiveWorld(World.World world)
            {
                // Do sectors.
                foreach (var sector in world.Map.Sectors.AsSpan())
                    ptr = UnArchiveSector(sector, data, ptr);

                // Do lines.
                foreach (var line in world.Map.Lines.AsSpan())
                    ptr = UnArchiveLine(line, data, ptr);
            }

            private void UnArchiveThinkers(World.World world)
            {
                var thinkers = world.Thinkers;
                var ta = world.ThingAllocation;

                // Remove all the current thinkers.
                foreach (var thinker in thinkers)
                {
                    if (thinker is Mobj mobj)
                        ta.RemoveMobj(mobj);
                }

                thinkers.Reset();

                // Read in saved thinkers.
                while (true)
                {
                    var tclass = (ThinkerClass)data[ptr++];
                    switch (tclass)
                    {
                        case ThinkerClass.End:
                            // End of list.
                            return;

                        case ThinkerClass.Mobj:
                            PadPointer();
                            var mobj = new Mobj(world);
                            mobj.ThinkerState = ReadThinkerState(data, ptr + 8);
                            mobj.X = new Fixed(BitConverter.ToInt32(data, ptr + 12));
                            mobj.Y = new Fixed(BitConverter.ToInt32(data, ptr + 16));
                            mobj.Z = new Fixed(BitConverter.ToInt32(data, ptr + 20));
                            mobj.Angle = new Angle(BitConverter.ToInt32(data, ptr + 32));
                            mobj.Sprite = (Sprite)BitConverter.ToInt32(data, ptr + 36);
                            mobj.Frame = BitConverter.ToInt32(data, ptr + 40);
                            mobj.FloorZ = new Fixed(BitConverter.ToInt32(data, ptr + 56));
                            mobj.CeilingZ = new Fixed(BitConverter.ToInt32(data, ptr + 60));
                            mobj.Radius = new Fixed(BitConverter.ToInt32(data, ptr + 64));
                            mobj.Height = new Fixed(BitConverter.ToInt32(data, ptr + 68));
                            mobj.MomX = new Fixed(BitConverter.ToInt32(data, ptr + 72));
                            mobj.MomY = new Fixed(BitConverter.ToInt32(data, ptr + 76));
                            mobj.MomZ = new Fixed(BitConverter.ToInt32(data, ptr + 80));
                            mobj.Type = (MobjType)BitConverter.ToInt32(data, ptr + 88);
                            mobj.Info = DoomInfo.MobjInfos[(int)mobj.Type];
                            mobj.Tics = BitConverter.ToInt32(data, ptr + 96);
                            mobj.State = DoomInfo.States[BitConverter.ToInt32(data, ptr + 100)];
                            mobj.Flags = (MobjFlags)BitConverter.ToInt32(data, ptr + 104);
                            mobj.Health = BitConverter.ToInt32(data, ptr + 108);
                            mobj.MoveDir = (Direction)BitConverter.ToInt32(data, ptr + 112);
                            mobj.MoveCount = BitConverter.ToInt32(data, ptr + 116);
                            mobj.ReactionTime = BitConverter.ToInt32(data, ptr + 124);
                            mobj.Threshold = BitConverter.ToInt32(data, ptr + 128);
                            var playerNumber = BitConverter.ToInt32(data, ptr + 132);
                            if (playerNumber != 0)
                            {
                                mobj.Player = world.Options.Players[playerNumber - 1];
                                mobj.Player.Mobj = mobj;
                            }

                            mobj.LastLook = BitConverter.ToInt32(data, ptr + 136);
                            mobj.SpawnPoint = new MapThing(
                                Fixed.FromInt(BitConverter.ToInt16(data, ptr + 140)),
                                Fixed.FromInt(BitConverter.ToInt16(data, ptr + 142)),
                                new Angle(Angle.Ang45.Data * (uint)(BitConverter.ToInt16(data, ptr + 144) / 45)),
                                BitConverter.ToInt16(data, ptr + 146),
                                (ThingFlags)BitConverter.ToInt16(data, ptr + 148));
                            ptr += 154;

                            world.ThingMovement.SetThingPosition(mobj);
                            // mobj.FloorZ = mobj.Subsector.Sector.FloorHeight;
                            // mobj.CeilingZ = mobj.Subsector.Sector.CeilingHeight;
                            thinkers.Add(mobj);
                            break;

                        default:
                            throw new Exception("Unknown thinker class in savegame!");
                    }
                }
            }

            private void UnArchiveSpecials(World.World world)
            {
                var thinkers = world.Thinkers;
                var sa = world.SectorAction;
                var dataSpan = data.AsSpan();

                // Read in saved thinkers.
                while (true)
                {
                    var tclass = (SpecialClass)data[ptr++];
                    switch (tclass)
                    {
                        case SpecialClass.EndSpecials:
                            // End of list.
                            return;

                        case SpecialClass.Ceiling:
                            PadPointer();
                            var ceilingData = dataSpan.Slice(ptr + 8, 48);
                            var ceiling = new CeilingMove(world);
                            ceiling.ThinkerState = ReadThinkerState(ceilingData.Slice(ptr, 4));
                            ceiling.Type = (CeilingMoveType)BitConverter.ToInt32(ceilingData.Slice(4, 4));
                            ceiling.Sector = world.Map.Sectors[BitConverter.ToInt32(ceilingData.Slice(8, 4))];
                            ceiling.Sector.SpecialData = ceiling;
                            ceiling.BottomHeight = new Fixed(BitConverter.ToInt32(ceilingData.Slice(12, 4)));
                            ceiling.TopHeight = new Fixed(BitConverter.ToInt32(ceilingData.Slice(16, 4)));
                            ceiling.Speed = new Fixed(BitConverter.ToInt32(ceilingData.Slice(20, 4)));
                            ceiling.Crush = BitConverter.ToInt32(ceilingData.Slice(24, 4)) != 0;
                            ceiling.Direction = BitConverter.ToInt32(ceilingData.Slice(28, 4));
                            ceiling.Tag = BitConverter.ToInt32(ceilingData.Slice(32, 4));
                            ceiling.OldDirection = BitConverter.ToInt32(ceilingData.Slice(36, 4));
                            ptr += 48;

                            thinkers.Add(ceiling);
                            sa.AddActiveCeiling(ceiling);
                            break;

                        case SpecialClass.Door:
                            PadPointer();
                            var door = new VerticalDoor(world);
                            door.ThinkerState = ReadThinkerState(data, ptr + 8);
                            door.Type = (VerticalDoorType)BitConverter.ToInt32(data, ptr + 12);
                            door.Sector = world.Map.Sectors[BitConverter.ToInt32(data, ptr + 16)];
                            door.Sector.SpecialData = door;
                            door.TopHeight = new Fixed(BitConverter.ToInt32(data, ptr + 20));
                            door.Speed = new Fixed(BitConverter.ToInt32(data, ptr + 24));
                            door.Direction = BitConverter.ToInt32(data, ptr + 28);
                            door.TopWait = BitConverter.ToInt32(data, ptr + 32);
                            door.TopCountDown = BitConverter.ToInt32(data, ptr + 36);
                            ptr += 40;

                            thinkers.Add(door);
                            break;

                        case SpecialClass.Floor:
                            PadPointer();
                            var floor = new FloorMove(world);
                            floor.ThinkerState = ReadThinkerState(data, ptr + 8);
                            floor.Type = (FloorMoveType)BitConverter.ToInt32(data, ptr + 12);
                            floor.Crush = BitConverter.ToInt32(data, ptr + 16) != 0;
                            floor.Sector = world.Map.Sectors[BitConverter.ToInt32(data, ptr + 20)];
                            floor.Sector.SpecialData = floor;
                            floor.Direction = BitConverter.ToInt32(data, ptr + 24);
                            floor.NewSpecial = (SectorSpecial)BitConverter.ToInt32(data, ptr + 28);
                            floor.Texture = BitConverter.ToInt32(data, ptr + 32);
                            floor.FloorDestHeight = new Fixed(BitConverter.ToInt32(data, ptr + 36));
                            floor.Speed = new Fixed(BitConverter.ToInt32(data, ptr + 40));
                            ptr += 44;

                            thinkers.Add(floor);
                            break;

                        case SpecialClass.Plat:
                            PadPointer();
                            var plat = new Platform(world);
                            plat.ThinkerState = ReadThinkerState(data, ptr + 8);
                            plat.Sector = world.Map.Sectors[BitConverter.ToInt32(data, ptr + 12)];
                            plat.Sector.SpecialData = plat;
                            plat.Speed = new Fixed(BitConverter.ToInt32(data, ptr + 16));
                            plat.Low = new Fixed(BitConverter.ToInt32(data, ptr + 20));
                            plat.High = new Fixed(BitConverter.ToInt32(data, ptr + 24));
                            plat.Wait = BitConverter.ToInt32(data, ptr + 28);
                            plat.Count = BitConverter.ToInt32(data, ptr + 32);
                            plat.Status = (PlatformState)BitConverter.ToInt32(data, ptr + 36);
                            plat.OldStatus = (PlatformState)BitConverter.ToInt32(data, ptr + 40);
                            plat.Crush = BitConverter.ToInt32(data, ptr + 44) != 0;
                            plat.Tag = BitConverter.ToInt32(data, ptr + 48);
                            plat.Type = (PlatformType)BitConverter.ToInt32(data, ptr + 52);
                            ptr += 56;

                            thinkers.Add(plat);
                            sa.AddActivePlatform(plat);
                            break;

                        case SpecialClass.Flash:
                            PadPointer();
                            var flash = new LightFlash(world);
                            flash.ThinkerState = ReadThinkerState(data, ptr + 8);
                            flash.Sector = world.Map.Sectors[BitConverter.ToInt32(data, ptr + 12)];
                            flash.Count = BitConverter.ToInt32(data, ptr + 16);
                            flash.MaxLight = BitConverter.ToInt32(data, ptr + 20);
                            flash.MinLight = BitConverter.ToInt32(data, ptr + 24);
                            flash.MaxTime = BitConverter.ToInt32(data, ptr + 28);
                            flash.MinTime = BitConverter.ToInt32(data, ptr + 32);
                            ptr += 36;

                            thinkers.Add(flash);
                            break;

                        case SpecialClass.Strobe:
                            PadPointer();
                            var strobe = new StrobeFlash(world);
                            strobe.ThinkerState = ReadThinkerState(data, ptr + 8);
                            strobe.Sector = world.Map.Sectors[BitConverter.ToInt32(data, ptr + 12)];
                            strobe.Count = BitConverter.ToInt32(data, ptr + 16);
                            strobe.MinLight = BitConverter.ToInt32(data, ptr + 20);
                            strobe.MaxLight = BitConverter.ToInt32(data, ptr + 24);
                            strobe.DarkTime = BitConverter.ToInt32(data, ptr + 28);
                            strobe.BrightTime = BitConverter.ToInt32(data, ptr + 32);
                            ptr += 36;

                            thinkers.Add(strobe);
                            break;

                        case SpecialClass.Glow:
                            PadPointer();
                            var glow = new GlowingLight(world);
                            glow.ThinkerState = ReadThinkerState(data, ptr + 8);
                            glow.Sector = world.Map.Sectors[BitConverter.ToInt32(data, ptr + 12)];
                            glow.MinLight = BitConverter.ToInt32(data, ptr + 16);
                            glow.MaxLight = BitConverter.ToInt32(data, ptr + 20);
                            glow.Direction = BitConverter.ToInt32(data, ptr + 24);
                            ptr += 28;

                            thinkers.Add(glow);
                            break;

                        default:
                            throw new Exception("Unknown special in savegame!");
                    }
                }
            }

            private static ThinkerState ReadThinkerState(byte[] data, int p)
            {
                switch (BitConverter.ToInt32(data, p))
                {
                    case 0:
                        return ThinkerState.InStasis;
                    default:
                        return ThinkerState.Active;
                }
            }

            private static ThinkerState ReadThinkerState(ReadOnlySpan<byte> data)
            {
                return BitConverter.ToInt32(data[..4]) switch
                {
                    0 => ThinkerState.InStasis,
                    _ => ThinkerState.Active
                };
            }

            private static int UnArchivePlayer(Player player, byte[] data, int p)
            {
                player.Clear();

                player.PlayerState = (PlayerState)BitConverter.ToInt32(data, p + 4);
                player.ViewZ = new Fixed(BitConverter.ToInt32(data, p + 16));
                player.ViewHeight = new Fixed(BitConverter.ToInt32(data, p + 20));
                player.DeltaViewHeight = new Fixed(BitConverter.ToInt32(data, p + 24));
                player.Bob = new Fixed(BitConverter.ToInt32(data, p + 28));
                player.Health = BitConverter.ToInt32(data, p + 32);
                player.ArmorPoints = BitConverter.ToInt32(data, p + 36);
                player.ArmorType = BitConverter.ToInt32(data, p + 40);
                for (var i = 0; i < (int)PowerType.Count; i++)
                {
                    player.Powers[i] = BitConverter.ToInt32(data, p + 44 + 4 * i);
                }

                for (var i = 0; i < (int)PowerType.Count; i++)
                {
                    player.Cards[i] = BitConverter.ToInt32(data, p + 68 + 4 * i) != 0;
                }

                player.Backpack = BitConverter.ToInt32(data, p + 92) != 0;
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    player.Frags[i] = BitConverter.ToInt32(data, p + 96 + 4 * i);
                }

                player.ReadyWeapon = new WeaponType(BitConverter.ToInt32(data, p + 112));
                player.PendingWeapon = new WeaponType(BitConverter.ToInt32(data, p + 116));
                for (var i = 0; i < (int)WeaponTypes.Count; i++)
                {
                    player.WeaponOwned[i] = BitConverter.ToInt32(data, p + 120 + 4 * i) != 0;
                }

                for (var i = 0; i < (int)AmmoType.Count; i++)
                {
                    player.Ammo[i] = BitConverter.ToInt32(data, p + 156 + 4 * i);
                }

                for (var i = 0; i < (int)AmmoType.Count; i++)
                {
                    player.MaxAmmo[i] = BitConverter.ToInt32(data, p + 172 + 4 * i);
                }

                player.AttackDown = BitConverter.ToInt32(data, p + 188) != 0;
                player.UseDown = BitConverter.ToInt32(data, p + 192) != 0;
                player.Cheats = (CheatFlags)BitConverter.ToInt32(data, p + 196);
                player.Refire = BitConverter.ToInt32(data, p + 200);
                player.KillCount = BitConverter.ToInt32(data, p + 204);
                player.ItemCount = BitConverter.ToInt32(data, p + 208);
                player.SecretCount = BitConverter.ToInt32(data, p + 212);
                player.DamageCount = BitConverter.ToInt32(data, p + 220);
                player.BonusCount = BitConverter.ToInt32(data, p + 224);
                player.ExtraLight = BitConverter.ToInt32(data, p + 232);
                player.FixedColorMap = BitConverter.ToInt32(data, p + 236);
                player.ColorMap = BitConverter.ToInt32(data, p + 240);
                for (var i = 0; i < (int)PlayerSprite.Count; i++)
                {
                    player.PlayerSprites[i].State = DoomInfo.States[BitConverter.ToInt32(data, p + 244 + 16 * i)];
                    if (player.PlayerSprites[i].State.Number == (int)MobjState.Null)
                    {
                        player.PlayerSprites[i].State = null;
                    }

                    player.PlayerSprites[i].Tics = BitConverter.ToInt32(data, p + 244 + 16 * i + 4);
                    player.PlayerSprites[i].Sx = new Fixed(BitConverter.ToInt32(data, p + 244 + 16 * i + 8));
                    player.PlayerSprites[i].Sy = new Fixed(BitConverter.ToInt32(data, p + 244 + 16 * i + 12));
                }

                player.DidSecret = BitConverter.ToInt32(data, p + 276) != 0;

                return p + 280;
            }

            private static int UnArchiveSector(Sector sector, ReadOnlySpan<byte> data, int ptr)
            {
                var root = data.Slice(ptr, 14);
                sector.FloorHeight = Fixed.FromInt(BitConverter.ToInt16(root[..2]));
                sector.CeilingHeight = Fixed.FromInt(BitConverter.ToInt16(root.Slice(2, 2)));
                sector.FloorFlat = BitConverter.ToInt16(root.Slice(4, 2));
                sector.CeilingFlat = BitConverter.ToInt16(root.Slice(6, 2));
                sector.LightLevel = BitConverter.ToInt16(root.Slice(8, 2));
                sector.Special = (SectorSpecial)BitConverter.ToInt16(root.Slice(10, 2));
                sector.Tag = BitConverter.ToInt16(root.Slice(12, 2));
                sector.SpecialData = null;
                sector.SoundTarget = null;
                return ptr + 14;
            }

            private static int UnArchiveLine(LineDef line, ReadOnlySpan<byte> data, int p)
            {
                var root = data.Slice(p, 6);
                line.Flags = (LineFlags)BitConverter.ToInt16(root[..2]);
                line.Special = (LineSpecial)BitConverter.ToInt16(root.Slice(2, 2));
                line.Tag = BitConverter.ToInt16(root.Slice(4, 2));
                p += 6;

                if (line.FrontSide != null)
                    p = PopulateLine(line.FrontSide, data, p);

                if (line.BackSide != null)
                    p = PopulateLine(line.BackSide, data, p);

                return p;

                static int PopulateLine(SideDef side, ReadOnlySpan<byte> root, int p)
                {
                    root = root.Slice(p, 10);
                    side.TextureOffset = Fixed.FromInt(BitConverter.ToInt16(root[..2]));
                    side.RowOffset = Fixed.FromInt(BitConverter.ToInt16(root.Slice(2, 2)));
                    side.TopTexture = BitConverter.ToInt16(root.Slice(4, 2));
                    side.BottomTexture = BitConverter.ToInt16(root.Slice(6, 2));
                    side.MiddleTexture = BitConverter.ToInt16(root.Slice(8, 2));
                    return p + 10;
                }
            }
        }
    }
}