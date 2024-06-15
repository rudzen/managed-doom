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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Doom.Game;

/// <summary>
/// Load game
/// </summary>
[SuppressMessage("Style", "IDE0017:Simplify object initialization")]
[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public static partial class SaveAndLoad
{
    private static int ValidateHeader(Span<byte> data)
    {
        var ptr = 0;

        ptr = ReadDescription(data, ptr, out var description);

        Console.WriteLine($"Loading game: {description}");

        ptr = ReadVersion(data, ptr, out var version);

        if (!string.Equals(version, "VERSION 109", StringComparison.OrdinalIgnoreCase))
            throw new Exception("Unsupported version!");

        return ptr;
    }

    private static void Load(DoomGame game, Span<byte> data, int ptr)
    {
        var options = game.World.Options;
        options.Skill = (GameSkill)data[ptr++];
        options.Episode = data[ptr++];
        options.Map = data[ptr++];

        foreach (var player in options.Players)
            player.InGame = data[ptr++] != 0;

        game.InitNew(options.Skill, options.Episode, options.Map);

        var a = data[ptr++];
        var b = data[ptr++];
        var c = data[ptr++];
        var levelTime = (a << 16) + (b << 8) + c;

        ptr = UnArchivePlayers(game.World, data, ptr);
        ptr = UnArchiveWorld(game.World, data, ptr);
        ptr = UnArchiveThinkers(game.World, data, ptr);
        ptr = UnArchiveSpecials(game.World, data, ptr);

        if (data[ptr] != 0x1d)
            throw new Exception("Bad save game!");

        game.World.LevelTime = levelTime;

        options.Sound.SetListener(game.World.ConsolePlayer.Mobj!);
    }

    private static int ReadDescription(Span<byte> data, int ptr, out string description)
    {
        description = DoomInterop.ToString(data.Slice(ptr, DescriptionSize));
        return ptr + DescriptionSize;
    }

    private static int ReadVersion(Span<byte> data, int ptr, out string version)
    {
        version = DoomInterop.ToString(data.Slice(ptr, VersionSize));
        return ptr + VersionSize;
    }

    private static int UnArchivePlayers(World.World world, Span<byte> data, int ptr)
    {
        var players = world.Options.Players.AsSpan();
        foreach (var player in players)
        {
            if (!player.InGame)
                continue;

            ptr = PadPointer(ptr);
            ptr = UnArchivePlayer(player, data, ptr);
        }

        return ptr;
    }

    private static int UnArchiveWorld(World.World world, Span<byte> data, int ptr)
    {
        var sectors = world.Map.Sectors.AsSpan();
        foreach (var sector in sectors)
            ptr += LoadSector(sector, data[ptr..]);

        var lines = world.Map.Lines.AsSpan();
        foreach (var line in lines)
            ptr += LoadLine(line, data[ptr..]);

        return ptr;
    }

    private static int UnArchiveThinkers(World.World world, Span<byte> data, int ptr)
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
            var thinker = (ThinkerClass)data[ptr++];

            if (thinker == ThinkerClass.End)
                return ptr;

            ptr = PadPointer(ptr);

            if (thinker != ThinkerClass.Mobj)
                throw new Exception($"Unknown thinker class in savegame! thinker={thinker}");

            var mobj = new Mobj(world);
            mobj.ThinkerState = ReadThinkerState(data[(ptr + 8)..]);
            mobj.X = new Fixed(BitConverter.ToInt32(data[(ptr + 12)..]));
            mobj.Y = new Fixed(BitConverter.ToInt32(data[(ptr + 16)..]));
            mobj.Z = new Fixed(BitConverter.ToInt32(data[(ptr + 20)..]));
            mobj.Angle = new Angle(BitConverter.ToInt32(data[(ptr + 32)..]));
            mobj.Sprite = (Sprite)BitConverter.ToInt32(data[(ptr + 36)..]);
            mobj.Frame = BitConverter.ToInt32(data[(ptr + 40)..]);
            mobj.FloorZ = new Fixed(BitConverter.ToInt32(data[(ptr + 56)..]));
            mobj.CeilingZ = new Fixed(BitConverter.ToInt32(data[(ptr + 60)..]));
            mobj.Radius = new Fixed(BitConverter.ToInt32(data[(ptr + 64)..]));
            mobj.Height = new Fixed(BitConverter.ToInt32(data[(ptr + 68)..]));
            mobj.MomX = new Fixed(BitConverter.ToInt32(data[(ptr + 72)..]));
            mobj.MomY = new Fixed(BitConverter.ToInt32(data[(ptr + 76)..]));
            mobj.MomZ = new Fixed(BitConverter.ToInt32(data[(ptr + 80)..]));
            mobj.Type = (MobjType)BitConverter.ToInt32(data[(ptr + 88)..]);
            mobj.Info = DoomInfo.MobjInfos[(int)mobj.Type];
            mobj.Tics = BitConverter.ToInt32(data[(ptr + 96)..]);
            mobj.State = DoomInfo.States[BitConverter.ToInt32(data[(ptr + 100)..])];
            mobj.Flags = (MobjFlags)BitConverter.ToInt32(data[(ptr + 104)..]);
            mobj.Health = BitConverter.ToInt32(data[(ptr + 108)..]);
            mobj.MoveDir = (Direction)BitConverter.ToInt32(data[(ptr + 112)..]);
            mobj.MoveCount = BitConverter.ToInt32(data[(ptr + 116)..]);
            mobj.ReactionTime = BitConverter.ToInt32(data[(ptr + 124)..]);
            mobj.Threshold = BitConverter.ToInt32(data[(ptr + 128)..]);
            var playerNumber = BitConverter.ToInt32(data[(ptr + 132)..]);
            if (playerNumber != 0)
            {
                mobj.Player = world.Options.Players[playerNumber - 1];
                mobj.Player.Mobj = mobj;
            }

            mobj.LastLook = BitConverter.ToInt32(data[(ptr + 136)..]);

            var x = Fixed.FromInt(BitConverter.ToInt16(data[(ptr + 140)..]));
            var y = Fixed.FromInt(BitConverter.ToInt16(data[(ptr + 142)..]));
            var angle = new Angle(Angle.Ang45.Data * (uint)(BitConverter.ToInt16(data[(ptr + 144)..]) / 45));
            var type = BitConverter.ToInt16(data[(ptr + 146)..]);
            var flags = (ThingFlags)BitConverter.ToInt16(data[(ptr + 148)..]);

            mobj.SpawnPoint = new MapThing(x, y, angle, type, flags);

            ptr += 154;

            world.ThingMovement.SetThingPosition(mobj);
            // mobj.FloorZ = mobj.Subsector.Sector.FloorHeight;
            // mobj.CeilingZ = mobj.Subsector.Sector.CeilingHeight;
            thinkers.Add(mobj);
        }
    }

    private static int UnArchiveSpecials(World.World world, Span<byte> data, int ptr)
    {
        var thinkers = world.Thinkers;
        var sa = world.SectorAction;

        // Read in saved thinkers.
        while (true)
        {
            var thinkClass = (SpecialClass)data[ptr++];

            if (thinkClass == SpecialClass.EndSpecials)
                return ptr;

            ptr = PadPointer(ptr);

            if (thinkClass == SpecialClass.Ceiling)
            {
                ptr += LoadCeilingMove(world, data[ptr..], out var ceilingMove);
                thinkers.Add(ceilingMove);
                sa.AddActiveCeiling(ceilingMove);
            }
            else if (thinkClass == SpecialClass.Door)
            {
                ptr += LoadVerticalDoor(world, data[ptr..], out var doorMove);
                thinkers.Add(doorMove);
            }
            else if (thinkClass == SpecialClass.Floor)
            {
                ptr += LoadFloorMove(world, data[ptr..], out var floorMove);
                thinkers.Add(floorMove);
            }
            else if (thinkClass == SpecialClass.Plat)
            {
                ptr += LoadPlatform(world, data[ptr..], out var platform);
                thinkers.Add(platform);
                sa.AddActivePlatform(platform);
            }
            else if (thinkClass == SpecialClass.Flash)
            {
                ptr += LoadLightFlash(world, data[ptr..], out var flash);
                thinkers.Add(flash);
            }
            else if (thinkClass == SpecialClass.Strobe)
            {
                ptr += LoadStrobeFlash(world, data[ptr..], out var strobe);
                thinkers.Add(strobe);
            }
            else if (thinkClass == SpecialClass.Glow)
            {
                ptr += LoadGlowingLight(world, data[ptr..], out var glow);
                thinkers.Add(glow);
            }
            else
                throw new Exception($"Unknown special in savegame! special={thinkClass}");
        }
    }

    private static int LoadCeilingMove(World.World world, ReadOnlySpan<byte> data, out CeilingMove ceilingMove)
    {
        const int dataSize = 48;
        var ceilingData = data[..dataSize];

        ceilingMove = new CeilingMove(world);
        ceilingMove.ThinkerState = ReadThinkerState(ceilingData.Slice(8, 4));
        ceilingMove.Type = (CeilingMoveType)BitConverter.ToInt32(ceilingData.Slice(12, 4));
        ceilingMove.Sector = world.Map.Sectors[BitConverter.ToInt32(ceilingData.Slice(16, 4))];
        ceilingMove.BottomHeight = new Fixed(BitConverter.ToInt32(ceilingData.Slice(20, 4)));
        ceilingMove.TopHeight = new Fixed(BitConverter.ToInt32(ceilingData.Slice(24, 4)));
        ceilingMove.Speed = new Fixed(BitConverter.ToInt32(ceilingData.Slice(28, 4)));
        ceilingMove.Crush = BitConverter.ToInt32(ceilingData.Slice(32, 4)) != 0;
        ceilingMove.Direction = BitConverter.ToInt32(ceilingData.Slice(36, 4));
        ceilingMove.Tag = BitConverter.ToInt32(ceilingData.Slice(40, 4));
        ceilingMove.OldDirection = BitConverter.ToInt32(ceilingData.Slice(44, 4));
        ceilingMove.Sector.SpecialData = ceilingMove;

        return dataSize;
    }

    private static int LoadVerticalDoor(World.World world, ReadOnlySpan<byte> data, out VerticalDoor verticalDoor)
    {
        const int dataSize = 40;
        var doorData = data[..dataSize];

        verticalDoor = new VerticalDoor(world);
        verticalDoor.ThinkerState = ReadThinkerState(doorData.Slice(8, 4));
        verticalDoor.Type = (VerticalDoorType)BitConverter.ToInt32(doorData.Slice(12, 4));
        verticalDoor.Sector = world.Map.Sectors[BitConverter.ToInt32(doorData.Slice(16, 4))];
        verticalDoor.TopHeight = new Fixed(BitConverter.ToInt32(doorData.Slice(20, 4)));
        verticalDoor.Speed = new Fixed(BitConverter.ToInt32(doorData.Slice(24, 4)));
        verticalDoor.Direction = BitConverter.ToInt32(doorData.Slice(28, 4));
        verticalDoor.TopWait = BitConverter.ToInt32(doorData.Slice(32, 4));
        verticalDoor.TopCountDown = BitConverter.ToInt32(doorData.Slice(36, 4));
        verticalDoor.Sector.SpecialData = verticalDoor;

        return dataSize;
    }

    private static int LoadFloorMove(World.World world, ReadOnlySpan<byte> data, out FloorMove floor)
    {
        const int dataSize = 44;
        var floorData = data[..dataSize];

        floor = new FloorMove(world);
        floor.ThinkerState = ReadThinkerState(floorData.Slice(8, 4));
        floor.Type = (FloorMoveType)BitConverter.ToInt32(floorData.Slice(12, 4));
        floor.Crush = BitConverter.ToInt32(floorData.Slice(16, 4)) != 0;
        floor.Sector = world.Map.Sectors[BitConverter.ToInt32(floorData.Slice(20, 4))];
        floor.Direction = BitConverter.ToInt32(floorData.Slice(24, 4));
        floor.NewSpecial = (SectorSpecial)BitConverter.ToInt32(floorData.Slice(28, 4));
        floor.Texture = BitConverter.ToInt32(floorData.Slice(32, 4));
        floor.FloorDestHeight = new Fixed(BitConverter.ToInt32(floorData.Slice(36, 4)));
        floor.Speed = new Fixed(BitConverter.ToInt32(floorData.Slice(40, 4)));
        floor.Sector.SpecialData = floor;

        return dataSize;
    }

    private static int LoadPlatform(World.World world, ReadOnlySpan<byte> data, out Platform plat)
    {
        const int dataSize = 56;
        var platformData = data[..dataSize];

        plat = new Platform(world);
        plat.ThinkerState = ReadThinkerState(platformData.Slice(8, 4));
        plat.Sector = world.Map.Sectors[BitConverter.ToInt32(platformData.Slice(12, 4))];
        plat.Speed = new Fixed(BitConverter.ToInt32(platformData.Slice(16, 4)));
        plat.Low = new Fixed(BitConverter.ToInt32(platformData.Slice(20, 4)));
        plat.High = new Fixed(BitConverter.ToInt32(platformData.Slice(24, 4)));
        plat.Wait = BitConverter.ToInt32(platformData.Slice(28, 4));
        plat.Count = BitConverter.ToInt32(platformData.Slice(32, 4));
        plat.Status = (PlatformState)BitConverter.ToInt32(platformData.Slice(36, 4));
        plat.OldStatus = (PlatformState)BitConverter.ToInt32(platformData.Slice(40, 4));
        plat.Crush = BitConverter.ToInt32(platformData.Slice(44, 4)) != 0;
        plat.Tag = BitConverter.ToInt32(platformData.Slice(48, 4));
        plat.Type = (PlatformType)BitConverter.ToInt32(platformData.Slice(52, 4));
        plat.Sector.SpecialData = plat;

        return dataSize;
    }

    private static int LoadLightFlash(World.World world, ReadOnlySpan<byte> data, out LightFlash flash)
    {
        const int dataSize = 36;
        var flashData = data[..dataSize];

        flash = new LightFlash(world.Random);
        flash.ThinkerState = ReadThinkerState(flashData.Slice(8, 4));
        flash.Sector = world.Map.Sectors[BitConverter.ToInt32(flashData.Slice(12, 4))];
        flash.Count = BitConverter.ToInt32(flashData.Slice(16, 4));
        flash.MaxLight = BitConverter.ToInt32(flashData.Slice(20, 4));
        flash.MinLight = BitConverter.ToInt32(flashData.Slice(24, 4));
        flash.MaxTime = BitConverter.ToInt32(flashData.Slice(28, 4));
        flash.MinTime = BitConverter.ToInt32(flashData.Slice(32, 4));

        return dataSize;
    }

    private static int LoadStrobeFlash(World.World world, ReadOnlySpan<byte> data, out StrobeFlash strobe)
    {
        const int dataSize = 36;
        var strobeData = data[..dataSize];

        strobe = new StrobeFlash();
        strobe.ThinkerState = ReadThinkerState(strobeData.Slice(8, 4));
        strobe.Sector = world.Map.Sectors[BitConverter.ToInt32(strobeData.Slice(12, 4))];
        strobe.Count = BitConverter.ToInt32(strobeData.Slice(16, 4));
        strobe.MinLight = BitConverter.ToInt32(strobeData.Slice(20, 4));
        strobe.MaxLight = BitConverter.ToInt32(strobeData.Slice(24, 4));
        strobe.DarkTime = BitConverter.ToInt32(strobeData.Slice(28, 4));
        strobe.BrightTime = BitConverter.ToInt32(strobeData.Slice(32, 4));

        return dataSize;
    }

    private static int LoadGlowingLight(World.World world, ReadOnlySpan<byte> data, out GlowingLight glow)
    {
        const int dataSize = 28;
        var glowData = data[..dataSize];

        glow = new GlowingLight();
        glow.ThinkerState = ReadThinkerState(glowData.Slice(8, 4));
        glow.Sector = world.Map.Sectors[BitConverter.ToInt32(glowData.Slice(12, 4))];
        glow.MinLight = BitConverter.ToInt32(glowData.Slice(16, 4));
        glow.MaxLight = BitConverter.ToInt32(glowData.Slice(20, 4));
        glow.Direction = BitConverter.ToInt32(glowData.Slice(24, 4));

        return dataSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ThinkerState ReadThinkerState(ReadOnlySpan<byte> data)
    {
        return BitConverter.ToInt32(data) switch
        {
            0 => ThinkerState.InStasis,
            _ => ThinkerState.Active
        };
    }

    private static int UnArchivePlayer(Player player, Span<byte> data, int p)
    {
        player.Clear();

        player.PlayerState = (PlayerState)BitConverter.ToInt32(data[(p + 4)..]);
        player.ViewZ = new Fixed(BitConverter.ToInt32(data[(p + 16)..]));
        player.ViewHeight = new Fixed(BitConverter.ToInt32(data[(p + 20)..]));
        player.DeltaViewHeight = new Fixed(BitConverter.ToInt32(data[(p + 24)..]));
        player.Bob = new Fixed(BitConverter.ToInt32(data[(p + 28)..]));
        player.Health = BitConverter.ToInt32(data[(p + 32)..]);
        player.ArmorPoints = BitConverter.ToInt32(data[(p + 36)..]);
        player.ArmorType = BitConverter.ToInt32(data[(p + 40)..]);

        for (var i = 0; i < player.Powers.Length; i++)
            player.Powers[i] = BitConverter.ToInt32(data[(p + 44 + 4 * i)..]);

        for (var i = 0; i < player.Cards.Length; i++)
            player.Cards[i] = BitConverter.ToInt32(data[(p + 68 + 4 * i)..]) != 0;

        player.Backpack = BitConverter.ToInt32(data[(p + 92)..]) != 0;

        for (var i = 0; i < player.Frags.Length; i++)
            player.Frags[i] = BitConverter.ToInt32(data[(p + 96 + 4 * i)..]);

        player.ReadyWeapon = new(BitConverter.ToInt32(data[(p + 112)..]));
        player.PendingWeapon = new(BitConverter.ToInt32(data[(p + 116)..]));

        for (var i = 0; i < player.WeaponOwned.Length; i++)
            player.WeaponOwned[i] = BitConverter.ToInt32(data[(p + 120 + 4 * i)..]) != 0;

        for (var i = 0; i < player.Ammo.Length; i++)
            player.Ammo[i] = BitConverter.ToInt32(data[(p + 156 + 4 * i)..]);

        for (var i = 0; i < player.MaxAmmo.Length; i++)
            player.MaxAmmo[i] = BitConverter.ToInt32(data[(p + 172 + 4 * i)..]);

        player.AttackDown = BitConverter.ToInt32(data[(p + 188)..]) != 0;
        player.UseDown = BitConverter.ToInt32(data[(p + 192)..]) != 0;
        player.Cheats = (CheatFlags)BitConverter.ToInt32(data[(p + 196)..]);
        player.Refire = BitConverter.ToInt32(data[(p + 200)..]);
        player.KillCount = BitConverter.ToInt32(data[(p + 204)..]);
        player.ItemCount = BitConverter.ToInt32(data[(p + 208)..]);
        player.SecretCount = BitConverter.ToInt32(data[(p + 212)..]);
        player.DamageCount = BitConverter.ToInt32(data[(p + 220)..]);
        player.BonusCount = BitConverter.ToInt32(data[(p + 224)..]);
        player.ExtraLight = BitConverter.ToInt32(data[(p + 232)..]);
        player.FixedColorMap = BitConverter.ToInt32(data[(p + 236)..]);
        player.ColorMap = BitConverter.ToInt32(data[(p + 240)..]);
        for (var i = 0; i < player.PlayerSprites.Length; i++)
        {
            player.PlayerSprites[i].State = DoomInfo.States[BitConverter.ToInt32(data[(p + 244 + 16 * i)..])];
            if (player.PlayerSprites[i].State?.Number == (int)MobjState.Null)
                player.PlayerSprites[i].State = null;

            player.PlayerSprites[i].Tics = BitConverter.ToInt32(data[(p + 244 + 16 * i + 4)..]);
            player.PlayerSprites[i].Sx = new Fixed(BitConverter.ToInt32(data[(p + 244 + 16 * i + 8)..]));
            player.PlayerSprites[i].Sy = new Fixed(BitConverter.ToInt32(data[(p + 244 + 16 * i + 12)..]));
        }

        player.DidSecret = BitConverter.ToInt32(data[(p + 276)..]) != 0;

        return p + 280;
    }

    private static int LoadSector(Sector sector, ReadOnlySpan<byte> data)
    {
        const int dataSize = 14;
        var sectorData = data[..dataSize];

        sector.FloorHeight = Fixed.FromInt(BitConverter.ToInt16(sectorData[..2]));
        sector.CeilingHeight = Fixed.FromInt(BitConverter.ToInt16(sectorData.Slice(2, 2)));
        sector.FloorFlat = BitConverter.ToInt16(sectorData.Slice(4, 2));
        sector.CeilingFlat = BitConverter.ToInt16(sectorData.Slice(6, 2));
        sector.LightLevel = BitConverter.ToInt16(sectorData.Slice(8, 2));
        sector.Special = (SectorSpecial)BitConverter.ToInt16(sectorData.Slice(10, 2));
        sector.Tag = BitConverter.ToInt16(sectorData.Slice(12, 2));
        sector.SpecialData = null;
        sector.SoundTarget = null;

        return dataSize;
    }

    private static int LoadLine(LineDef line, ReadOnlySpan<byte> data)
    {
        const int dataSize = 6;
        var lineData = data[..dataSize];

        line.Flags = (LineFlags)BitConverter.ToInt16(lineData[..2]);
        line.Special = (LineSpecial)BitConverter.ToInt16(lineData.Slice(2, 2));
        line.Tag = BitConverter.ToInt16(lineData.Slice(4, 2));

        var readData = dataSize;

        if (line.FrontSide is not null)
            readData += LoadSide(line.FrontSide, data.Slice(readData, 10));

        if (line.BackSide is not null)
            readData += LoadSide(line.BackSide, data.Slice(readData, 10));

        return readData;
    }

    private static int LoadSide(SideDef side, ReadOnlySpan<byte> sideData)
    {
        const int dataSize = 10;

        side.TextureOffset = Fixed.FromInt(BitConverter.ToInt16(sideData[..2]));
        side.RowOffset = Fixed.FromInt(BitConverter.ToInt16(sideData.Slice(2, 2)));
        side.TopTexture = BitConverter.ToInt16(sideData.Slice(4, 2));
        side.BottomTexture = BitConverter.ToInt16(sideData.Slice(6, 2));
        side.MiddleTexture = BitConverter.ToInt16(sideData.Slice(8, 2));

        return dataSize;
    }
}