using System;
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.World;
using ManagedDoom.Extensions;

namespace ManagedDoom.Doom.Game;

public static partial class SaveAndLoad
{
    private static int SaveHeader(string description, Span<byte> data)
    {
        var ptr = WriteDescription(data, description);
        return WriteVersion(data, ptr);
    }

    private static int WriteDescription(Span<byte> data, string description)
    {
        for (var i = 0; i < description.Length; i++)
            data[i] = (byte)description[i];

        return DescriptionSize;
    }

    private static int WriteVersion(Span<byte> data, int ptr)
    {
        const string version = "version 109";
        for (var i = 0; i < version.Length; i++)
            data[ptr + i] = (byte)version[i];

        return ptr + VersionSize;
    }

    private static int Save(DoomGame game, Span<byte> data, int ptr)
    {
        var options = game.World.Options;
        data[ptr++] = (byte)options.Skill;
        data[ptr++] = (byte)options.Episode;
        data[ptr++] = (byte)options.Map;
        for (var i = 0; i < Player.MaxPlayerCount; i++)
            data[ptr++] = options.Players[i].InGame.AsByte();

        data[ptr++] = (byte)(game.World.LevelTime >> 16);
        data[ptr++] = (byte)(game.World.LevelTime >> 8);
        data[ptr++] = (byte)game.World.LevelTime;

        ptr = ArchivePlayers(game.World.Options.Players, data, ptr);
        ptr = ArchiveWorld(game.World, data, ptr);
        ptr = ArchiveThinkers(game.World, data, ptr);
        ptr = ArchiveSpecials(game.World, data, ptr);

        data[ptr++] = 0x1d;

        return ptr;
    }

    private static int ArchivePlayers(ReadOnlySpan<Player> players, Span<byte> data, int ptr)
    {
        for (var i = 0; i < Player.MaxPlayerCount; i++)
        {
            if (!players[i].InGame)
                continue;

            ptr = PadPointer(ptr);
            ptr = ArchivePlayer(players[i], data, ptr);
        }

        return ptr;
    }

    private static int ArchiveWorld(World.World world, Span<byte> data, int ptr)
    {
        // Do sectors.
        var sectors = world.Map.Sectors.AsSpan();
        foreach (var sector in sectors)
            ptr = ArchiveSector(sector, data, ptr);

        // Do lines.
        var lines = world.Map.Lines.AsSpan();
        foreach (var line in lines)
            ptr = ArchiveLine(line, data, ptr);

        return ptr;
    }

    private static int ArchiveThinkers(World.World world, Span<byte> data, int ptr)
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

        return ptr;
    }

    private static int ArchiveSpecials(World.World world, Span<byte> data, int ptr)
    {
        var thinkers = world.Thinkers;
        var sa = world.SectorAction;

        // Read in saved thinkers.
        foreach (var thinker in thinkers)
        {
            switch (thinker)
            {
                // check for stasis state
                case CeilingMove ceilingMove when thinker.ThinkerState == ThinkerState.InStasis:
                {
                    if (sa.CheckActiveCeiling(ceilingMove))
                        ptr = WriteCeilingMove(ceilingMove, data, ptr);

                    continue;
                }
                case CeilingMove ceilingMove:
                    ptr = WriteCeilingMove(ceilingMove, data, ptr);
                    continue;
                case VerticalDoor door:
                    ptr = WriteVerticalDoor(door, data, ptr);
                    continue;
                case FloorMove floor:
                    ptr = WriteFloorMove(floor, data, ptr);
                    continue;
                case Platform plat:
                    ptr = WritePlatform(plat, data, ptr);
                    continue;
                case LightFlash flash:
                    WriteLightFlash(flash, data, ptr);
                    continue;
                case StrobeFlash strobe:
                    ptr = WriteStrobeFlash(strobe, data, ptr);
                    continue;
                case GlowingLight glow:
                    ptr = WriteGlowingLight(glow, data, ptr);
                    break;
            }
        }

        data[ptr++] = (byte)SpecialClass.EndSpecials;

        return ptr;
    }

    private static int WriteCeilingMove(CeilingMove ceiling, Span<byte> data, int ptr)
    {
        data[ptr++] = (byte)SpecialClass.Ceiling;
        ptr = PadPointer(ptr);
        WriteThinkerState(data, ptr + 8, ceiling.ThinkerState);
        Write(data, ptr + 12, (int)ceiling.Type);
        Write(data, ptr + 16, ceiling.Sector.Number);
        Write(data, ptr + 20, ceiling.BottomHeight.Data);
        Write(data, ptr + 24, ceiling.TopHeight.Data);
        Write(data, ptr + 28, ceiling.Speed.Data);
        Write(data, ptr + 32, ceiling.Crush.AsInt());
        Write(data, ptr + 36, ceiling.Direction);
        Write(data, ptr + 40, ceiling.Tag);
        Write(data, ptr + 44, ceiling.OldDirection);
        return ptr + 48;
    }

    private static int WriteVerticalDoor(VerticalDoor door, Span<byte> data, int ptr)
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
        return ptr + 40;
    }

    private static int WriteFloorMove(FloorMove floor, Span<byte> data, int ptr)
    {
        data[ptr++] = (byte)SpecialClass.Floor;
        ptr = PadPointer(ptr);
        WriteThinkerState(data, ptr + 8, floor.ThinkerState);
        Write(data, ptr + 12, (int)floor.Type);
        Write(data, ptr + 16, floor.Crush.AsInt());
        Write(data, ptr + 20, floor.Sector.Number);
        Write(data, ptr + 24, floor.Direction);
        Write(data, ptr + 28, (int)floor.NewSpecial);
        Write(data, ptr + 32, floor.Texture);
        Write(data, ptr + 36, floor.FloorDestHeight.Data);
        Write(data, ptr + 40, floor.Speed.Data);
        return ptr + 44;
    }

    private static int WritePlatform(Platform plat, Span<byte> data, int ptr)
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
        Write(data, ptr + 44, plat.Crush.AsInt());
        Write(data, ptr + 48, plat.Tag);
        Write(data, ptr + 52, (int)plat.Type);
        return ptr + 56;
    }

    private static int WriteLightFlash(LightFlash flash, Span<byte> data, int ptr)
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
        return ptr + 36;
    }

    private static int WriteStrobeFlash(StrobeFlash strobe, Span<byte> data, int ptr)
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
        return ptr + 36;
    }

    private static int WriteGlowingLight(GlowingLight glow, Span<byte> data, int ptr)
    {
        data[ptr++] = (byte)SpecialClass.Glow;
        ptr = PadPointer(ptr);
        WriteThinkerState(data, ptr + 8, glow.ThinkerState);
        Write(data, ptr + 12, glow.Sector.Number);
        Write(data, ptr + 16, glow.MinLight);
        Write(data, ptr + 20, glow.MaxLight);
        Write(data, ptr + 24, glow.Direction);
        return ptr + 28;
    }

    private static int ArchivePlayer(Player player, Span<byte> data, int p)
    {
        Write(data, p + 4, (int)player.PlayerState);
        Write(data, p + 16, player.ViewZ.Data);
        Write(data, p + 20, player.ViewHeight.Data);
        Write(data, p + 24, player.DeltaViewHeight.Data);
        Write(data, p + 28, player.Bob.Data);
        Write(data, p + 32, player.Health);
        Write(data, p + 36, player.ArmorPoints);
        Write(data, p + 40, player.ArmorType);

        for (var i = 0; i < player.Powers.Length; i++)
            Write(data, p + 44 + 4 * i, player.Powers[i]);

        for (var i = 0; i < player.Cards.Length; i++)
            Write(data, p + 68 + 4 * i, player.Cards[i].AsByte());

        Write(data, p + 92, player.Backpack.AsByte());

        for (var i = 0; i < player.Frags.Length; i++)
            Write(data, p + 96 + 4 * i, player.Frags[i]);

        Write(data, p + 112, (int)player.ReadyWeapon);
        Write(data, p + 116, (int)player.PendingWeapon);
        for (var i = 0; i < player.WeaponOwned.Length; i++)
        {
            Write(data, p + 120 + 4 * i, player.WeaponOwned[i].AsInt());
        }

        for (var i = 0; i < player.Ammo.Length; i++)
        {
            Write(data, p + 156 + 4 * i, player.Ammo[i]);
        }

        for (var i = 0; i < player.MaxAmmo.Length; i++)
        {
            Write(data, p + 172 + 4 * i, player.MaxAmmo[i]);
        }

        Write(data, p + 188, player.AttackDown.AsInt());
        Write(data, p + 192, player.UseDown.AsInt());
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
        var playerSprites = player.PlayerSprites.AsSpan();
        for (var i = 0; i < playerSprites.Length; i++)
        {
            var sprite = playerSprites[i];
            var spriteState = sprite.State is null ? 0 : sprite.State!.Number;
            Write(data, p + 244 + 16 * i, spriteState);
            Write(data, p + 244 + 16 * i + 4, sprite.Tics);
            Write(data, p + 244 + 16 * i + 8, sprite.Sx.Data);
            Write(data, p + 244 + 16 * i + 12, sprite.Sy.Data);
        }

        Write(data, p + 276, player.DidSecret.AsByte());

        return p + 280;
    }

    private static int ArchiveSector(Sector sector, Span<byte> data, int p)
    {
        Write(data, p, (short)sector.FloorHeight.ToIntFloor());
        Write(data, p + 2, (short)sector.CeilingHeight.ToIntFloor());
        Write(data, p + 4, (short)sector.FloorFlat);
        Write(data, p + 6, (short)sector.CeilingFlat);
        Write(data, p + 8, (short)sector.LightLevel);
        Write(data, p + 10, (short)sector.Special);
        Write(data, p + 12, (short)sector.Tag);
        return p + 14;
    }

    private static int ArchiveLine(LineDef line, Span<byte> data, int p)
    {
        Write(data, p, (short)line.Flags);
        Write(data, p + 2, (short)line.Special);
        Write(data, p + 4, line.Tag);
        p += 6;

        if (line.FrontSide is not null)
            p = WriteSideDef(line.FrontSide, data, p);

        if (line.BackSide is not null)
            p = WriteSideDef(line.BackSide, data, p);

        return p;
    }

    private static int WriteSideDef(SideDef side, Span<byte> data, int p)
    {
        Write(data, p, (short)side.TextureOffset.ToIntFloor());
        Write(data, p + 2, (short)side.RowOffset.ToIntFloor());
        Write(data, p + 4, (short)side.TopTexture);
        Write(data, p + 6, (short)side.BottomTexture);
        Write(data, p + 8, (short)side.MiddleTexture);
        return p + 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(Span<byte> data, int p, int value)
    {
        data[p] = (byte)value;
        data[p + 1] = (byte)(value >> 8);
        data[p + 2] = (byte)(value >> 16);
        data[p + 3] = (byte)(value >> 24);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(Span<byte> data, int p, uint value)
    {
        data[p] = (byte)value;
        data[p + 1] = (byte)(value >> 8);
        data[p + 2] = (byte)(value >> 16);
        data[p + 3] = (byte)(value >> 24);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(Span<byte> data, int p, short value)
    {
        data[p] = (byte)value;
        data[p + 1] = (byte)(value >> 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteThinkerState(Span<byte> data, int p, ThinkerState state)
    {
        Write(data, p, (state != ThinkerState.InStasis).AsByte());
    }
}