using System;
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

        options.Sound.SetListener(game.World.ConsolePlayer.Mobj);
    }

    private static int PadPointer(int ptr)
    {
        ptr += (4 - (ptr & 3)) & 3;
        return ptr;
    }

    private static int ReadDescription(Span<byte> data, int ptr, out string description)
    {
        description = DoomInterop.ToString(data.Slice(ptr, DescriptionSize));
        return ptr + DescriptionSize;
    }

    private static int ReadVersion(Span<byte> data, int ptr, out string version)
    {
        version = DoomInterop.ToString(data.Slice(ptr, versionSize));
        return ptr + versionSize;
    }

    private static int UnArchivePlayers(World.World world, Span<byte> data, int ptr)
    {
        var players = world.Options.Players;
        for (var i = 0; i < Player.MaxPlayerCount; i++)
        {
            if (!players[i].InGame)
                continue;

            ptr = PadPointer(ptr);
            ptr = UnArchivePlayer(players[i], data, ptr);
        }

        return ptr;
    }

    private static int UnArchiveWorld(World.World world, Span<byte> data, int ptr)
    {
        // Do sectors.
        var sectors = world.Map.Sectors.AsSpan();
        foreach (var sector in sectors)
            ptr = UnArchiveSector(sector, data, ptr);

        // Do lines.
        var lines = world.Map.Lines.AsSpan();
        foreach (var line in lines)
            ptr = UnArchiveLine(line, data, ptr);

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
            switch (thinker)
            {
                case ThinkerClass.End:
                    // End of list.
                    return ptr;

                case ThinkerClass.Mobj:
                    ptr = PadPointer(ptr);
                    var mobj = new Mobj(world);
                    mobj.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    mobj.X = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 12)));
                    mobj.Y = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 16)));
                    mobj.Z = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 20)));
                    mobj.Angle = new Angle(BitConverter.ToInt32(data.Slice(ptr + 32)));
                    mobj.Sprite = (Sprite)BitConverter.ToInt32(data.Slice(ptr + 36));
                    mobj.Frame = BitConverter.ToInt32(data.Slice(ptr + 40));
                    mobj.FloorZ = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 56)));
                    mobj.CeilingZ = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 60)));
                    mobj.Radius = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 64)));
                    mobj.Height = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 68)));
                    mobj.MomX = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 72)));
                    mobj.MomY = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 76)));
                    mobj.MomZ = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 80)));
                    mobj.Type = (MobjType)BitConverter.ToInt32(data.Slice(ptr + 88));
                    mobj.Info = DoomInfo.MobjInfos[(int)mobj.Type];
                    mobj.Tics = BitConverter.ToInt32(data.Slice(ptr + 96));
                    mobj.State = DoomInfo.States[BitConverter.ToInt32(data.Slice(ptr + 100))];
                    mobj.Flags = (MobjFlags)BitConverter.ToInt32(data.Slice(ptr + 104));
                    mobj.Health = BitConverter.ToInt32(data.Slice(ptr + 108));
                    mobj.MoveDir = (Direction)BitConverter.ToInt32(data.Slice(ptr + 112));
                    mobj.MoveCount = BitConverter.ToInt32(data.Slice(ptr + 116));
                    mobj.ReactionTime = BitConverter.ToInt32(data.Slice(ptr + 124));
                    mobj.Threshold = BitConverter.ToInt32(data.Slice(ptr + 128));
                    var playerNumber = BitConverter.ToInt32(data.Slice(ptr + 132));
                    if (playerNumber != 0)
                    {
                        mobj.Player = world.Options.Players[playerNumber - 1];
                        mobj.Player.Mobj = mobj;
                    }

                    mobj.LastLook = BitConverter.ToInt32(data.Slice(ptr + 136));

                    var x = Fixed.FromInt(BitConverter.ToInt16(data.Slice(ptr + 140)));
                    var y = Fixed.FromInt(BitConverter.ToInt16(data.Slice(ptr + 142)));
                    var angle = new Angle(Angle.Ang45.Data * (uint)(BitConverter.ToInt16(data.Slice(ptr + 144)) / 45));
                    var type = BitConverter.ToInt16(data.Slice(ptr + 146));
                    var flags = (ThingFlags)BitConverter.ToInt16(data.Slice(ptr + 148));

                    mobj.SpawnPoint = new MapThing(x, y, angle, type, flags);

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

    private static int UnArchiveSpecials(World.World world, Span<byte> data, int ptr)
    {
        var thinkers = world.Thinkers;
        var sa = world.SectorAction;

        // Read in saved thinkers.
        while (true)
        {
            var tclass = (SpecialClass)data[ptr++];
            switch (tclass)
            {
                case SpecialClass.EndSpecials:
                    // End of list.
                    return ptr;

                case SpecialClass.Ceiling:
                    ptr = PadPointer(ptr);
                    var ceiling = new CeilingMove(world);
                    ceiling.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    ceiling.Type = (CeilingMoveType)BitConverter.ToInt32(data.Slice(ptr + 12));
                    ceiling.Sector = world.Map.Sectors[BitConverter.ToInt32(data.Slice(ptr + 16))];
                    ceiling.Sector.SpecialData = ceiling;
                    ceiling.BottomHeight = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 20)));
                    ceiling.TopHeight = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 24)));
                    ceiling.Speed = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 28)));
                    ceiling.Crush = BitConverter.ToInt32(data.Slice(ptr + 32)) != 0;
                    ceiling.Direction = BitConverter.ToInt32(data.Slice(ptr + 36));
                    ceiling.Tag = BitConverter.ToInt32(data.Slice(ptr + 40));
                    ceiling.OldDirection = BitConverter.ToInt32(data.Slice(ptr + 44));
                    ptr += 48;

                    thinkers.Add(ceiling);
                    sa.AddActiveCeiling(ceiling);
                    break;

                case SpecialClass.Door:
                    ptr = PadPointer(ptr);
                    var door = new VerticalDoor(world);
                    door.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    door.Type = (VerticalDoorType)BitConverter.ToInt32(data.Slice(ptr + 12));
                    door.Sector = world.Map.Sectors[BitConverter.ToInt32(data.Slice(ptr + 16))];
                    door.Sector.SpecialData = door;
                    door.TopHeight = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 20)));
                    door.Speed = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 24)));
                    door.Direction = BitConverter.ToInt32(data.Slice(ptr + 28));
                    door.TopWait = BitConverter.ToInt32(data.Slice(ptr + 32));
                    door.TopCountDown = BitConverter.ToInt32(data.Slice(ptr + 36));
                    ptr += 40;

                    thinkers.Add(door);
                    break;

                case SpecialClass.Floor:
                    ptr = PadPointer(ptr);
                    var floor = new FloorMove(world);
                    floor.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    floor.Type = (FloorMoveType)BitConverter.ToInt32(data.Slice(ptr + 12));
                    floor.Crush = BitConverter.ToInt32(data.Slice(ptr + 16)) != 0;
                    floor.Sector = world.Map.Sectors[BitConverter.ToInt32(data.Slice(ptr + 20))];
                    floor.Sector.SpecialData = floor;
                    floor.Direction = BitConverter.ToInt32(data.Slice(ptr + 24));
                    floor.NewSpecial = (SectorSpecial)BitConverter.ToInt32(data.Slice(ptr + 28));
                    floor.Texture = BitConverter.ToInt32(data.Slice(ptr + 32));
                    floor.FloorDestHeight = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 36)));
                    floor.Speed = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 40)));
                    ptr += 44;

                    thinkers.Add(floor);
                    break;

                case SpecialClass.Plat:
                    ptr = PadPointer(ptr);
                    var plat = new Platform(world);
                    plat.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    plat.Sector = world.Map.Sectors[BitConverter.ToInt32(data.Slice(ptr + 12))];
                    plat.Sector.SpecialData = plat;
                    plat.Speed = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 16)));
                    plat.Low = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 20)));
                    plat.High = new Fixed(BitConverter.ToInt32(data.Slice(ptr + 24)));
                    plat.Wait = BitConverter.ToInt32(data.Slice(ptr + 28));
                    plat.Count = BitConverter.ToInt32(data.Slice(ptr + 32));
                    plat.Status = (PlatformState)BitConverter.ToInt32(data.Slice(ptr + 36));
                    plat.OldStatus = (PlatformState)BitConverter.ToInt32(data.Slice(ptr + 40));
                    plat.Crush = BitConverter.ToInt32(data.Slice(ptr + 44)) != 0;
                    plat.Tag = BitConverter.ToInt32(data.Slice(ptr + 48));
                    plat.Type = (PlatformType)BitConverter.ToInt32(data.Slice(ptr + 52));
                    ptr += 56;

                    thinkers.Add(plat);
                    sa.AddActivePlatform(plat);
                    break;

                case SpecialClass.Flash:
                    ptr = PadPointer(ptr);
                    var flash = new LightFlash(world);
                    flash.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    flash.Sector = world.Map.Sectors[BitConverter.ToInt32(data.Slice(ptr + 12))];
                    flash.Count = BitConverter.ToInt32(data.Slice(ptr + 16));
                    flash.MaxLight = BitConverter.ToInt32(data.Slice(ptr + 20));
                    flash.MinLight = BitConverter.ToInt32(data.Slice(ptr + 24));
                    flash.MaxTime = BitConverter.ToInt32(data.Slice(ptr + 28));
                    flash.MinTime = BitConverter.ToInt32(data.Slice(ptr + 32));
                    ptr += 36;

                    thinkers.Add(flash);
                    break;

                case SpecialClass.Strobe:
                    ptr = PadPointer(ptr);
                    var strobe = new StrobeFlash(world);
                    strobe.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    strobe.Sector = world.Map.Sectors[BitConverter.ToInt32(data.Slice(ptr + 12))];
                    strobe.Count = BitConverter.ToInt32(data.Slice(ptr + 16));
                    strobe.MinLight = BitConverter.ToInt32(data.Slice(ptr + 20));
                    strobe.MaxLight = BitConverter.ToInt32(data.Slice(ptr + 24));
                    strobe.DarkTime = BitConverter.ToInt32(data.Slice(ptr + 28));
                    strobe.BrightTime = BitConverter.ToInt32(data.Slice(ptr + 32));
                    ptr += 36;

                    thinkers.Add(strobe);
                    break;

                case SpecialClass.Glow:
                    ptr = PadPointer(ptr);
                    var glow = new GlowingLight(world);
                    glow.ThinkerState = ReadThinkerState(data.Slice(ptr + 8));
                    glow.Sector = world.Map.Sectors[BitConverter.ToInt32(data.Slice(ptr + 12))];
                    glow.MinLight = BitConverter.ToInt32(data.Slice(ptr + 16));
                    glow.MaxLight = BitConverter.ToInt32(data.Slice(ptr + 20));
                    glow.Direction = BitConverter.ToInt32(data.Slice(ptr + 24));
                    ptr += 28;

                    thinkers.Add(glow);
                    break;

                default:
                    throw new Exception("Unknown special in savegame!");
            }
        }
    }

    private static ThinkerState ReadThinkerState(Span<byte> data)
    {
        switch (BitConverter.ToInt32(data))
        {
            case 0:
                return ThinkerState.InStasis;
            default:
                return ThinkerState.Active;
        }
    }

    private static int UnArchivePlayer(Player player, Span<byte> data, int p)
    {
        player.Clear();

        player.PlayerState = (PlayerState)BitConverter.ToInt32(data.Slice(p + 4));
        player.ViewZ = new Fixed(BitConverter.ToInt32(data.Slice(p + 16)));
        player.ViewHeight = new Fixed(BitConverter.ToInt32(data.Slice(p + 20)));
        player.DeltaViewHeight = new Fixed(BitConverter.ToInt32(data.Slice(p + 24)));
        player.Bob = new Fixed(BitConverter.ToInt32(data.Slice(p + 28)));
        player.Health = BitConverter.ToInt32(data.Slice(p + 32));
        player.ArmorPoints = BitConverter.ToInt32(data.Slice(p + 36));
        player.ArmorType = BitConverter.ToInt32(data.Slice(p + 40));

        for (var i = 0; i < (int)PowerType.Count; i++)
            player.Powers[i] = BitConverter.ToInt32(data.Slice(p + 44 + 4 * i));

        for (var i = 0; i < (int)PowerType.Count; i++)
            player.Cards[i] = BitConverter.ToInt32(data.Slice(p + 68 + 4 * i)) != 0;

        player.Backpack = BitConverter.ToInt32(data.Slice(p + 92)) != 0;

        for (var i = 0; i < Player.MaxPlayerCount; i++)
            player.Frags[i] = BitConverter.ToInt32(data.Slice(p + 96 + 4 * i));

        player.ReadyWeapon = new(BitConverter.ToInt32(data.Slice(p + 112)));
        player.PendingWeapon = new(BitConverter.ToInt32(data.Slice(p + 116)));

        for (var i = 0; i < WeaponType.Count; i++)
            player.WeaponOwned[i] = BitConverter.ToInt32(data.Slice(p + 120 + 4 * i)) != 0;

        for (var i = 0; i < (int)AmmoType.Count; i++)
            player.Ammo[i] = BitConverter.ToInt32(data.Slice(p + 156 + 4 * i));

        for (var i = 0; i < (int)AmmoType.Count; i++)
            player.MaxAmmo[i] = BitConverter.ToInt32(data.Slice(p + 172 + 4 * i));

        player.AttackDown = BitConverter.ToInt32(data.Slice(p + 188)) != 0;
        player.UseDown = BitConverter.ToInt32(data.Slice(p + 192)) != 0;
        player.Cheats = (CheatFlags)BitConverter.ToInt32(data.Slice(p + 196));
        player.Refire = BitConverter.ToInt32(data.Slice(p + 200));
        player.KillCount = BitConverter.ToInt32(data.Slice(p + 204));
        player.ItemCount = BitConverter.ToInt32(data.Slice(p + 208));
        player.SecretCount = BitConverter.ToInt32(data.Slice(p + 212));
        player.DamageCount = BitConverter.ToInt32(data.Slice(p + 220));
        player.BonusCount = BitConverter.ToInt32(data.Slice(p + 224));
        player.ExtraLight = BitConverter.ToInt32(data.Slice(p + 232));
        player.FixedColorMap = BitConverter.ToInt32(data.Slice(p + 236));
        player.ColorMap = BitConverter.ToInt32(data.Slice(p + 240));
        for (var i = 0; i < (int)PlayerSprite.Count; i++)
        {
            player.PlayerSprites[i].State = DoomInfo.States[BitConverter.ToInt32(data.Slice(p + 244 + 16 * i))];
            if (player.PlayerSprites[i].State.Number == (int)MobjState.Null)
            {
                player.PlayerSprites[i].State = null;
            }

            player.PlayerSprites[i].Tics = BitConverter.ToInt32(data.Slice(p + 244 + 16 * i + 4));
            player.PlayerSprites[i].Sx = new Fixed(BitConverter.ToInt32(data.Slice(p + 244 + 16 * i + 8)));
            player.PlayerSprites[i].Sy = new Fixed(BitConverter.ToInt32(data.Slice(p + 244 + 16 * i + 12)));
        }

        player.DidSecret = BitConverter.ToInt32(data.Slice(p + 276)) != 0;

        return p + 280;
    }

    private static int UnArchiveSector(Sector sector, Span<byte> data, int ptr)
    {
        sector.FloorHeight = Fixed.FromInt(BitConverter.ToInt16(data.Slice(ptr)));
        sector.CeilingHeight = Fixed.FromInt(BitConverter.ToInt16(data.Slice(ptr + 2)));
        sector.FloorFlat = BitConverter.ToInt16(data.Slice(ptr + 4));
        sector.CeilingFlat = BitConverter.ToInt16(data.Slice(ptr + 6));
        sector.LightLevel = BitConverter.ToInt16(data.Slice(ptr + 8));
        sector.Special = (SectorSpecial)BitConverter.ToInt16(data.Slice(ptr + 10));
        sector.Tag = BitConverter.ToInt16(data.Slice(ptr + 12));
        sector.SpecialData = null;
        sector.SoundTarget = null;
        return ptr + 14;
    }

    private static int UnArchiveLine(LineDef line, Span<byte> data, int p)
    {
        line.Flags = (LineFlags)BitConverter.ToInt16(data.Slice(p));
        line.Special = (LineSpecial)BitConverter.ToInt16(data.Slice(p + 2));
        line.Tag = BitConverter.ToInt16(data.Slice(p + 4));
        p += 6;

        if (line.FrontSide != null)
        {
            var side = line.FrontSide;
            side.TextureOffset = Fixed.FromInt(BitConverter.ToInt16(data.Slice(p)));
            side.RowOffset = Fixed.FromInt(BitConverter.ToInt16(data.Slice(p + 2)));
            side.TopTexture = BitConverter.ToInt16(data.Slice(p + 4));
            side.BottomTexture = BitConverter.ToInt16(data.Slice(p + 6));
            side.MiddleTexture = BitConverter.ToInt16(data.Slice(p + 8));
            p += 10;
        }

        if (line.BackSide != null)
        {
            var side = line.BackSide;
            side.TextureOffset = Fixed.FromInt(BitConverter.ToInt16(data.Slice(p)));
            side.RowOffset = Fixed.FromInt(BitConverter.ToInt16(data.Slice(p + 2)));
            side.TopTexture = BitConverter.ToInt16(data.Slice(p + 4));
            side.BottomTexture = BitConverter.ToInt16(data.Slice(p + 6));
            side.MiddleTexture = BitConverter.ToInt16(data.Slice(p + 8));
            p += 10;
        }

        return p;
    }
}