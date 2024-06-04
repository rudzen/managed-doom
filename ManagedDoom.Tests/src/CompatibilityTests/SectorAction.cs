namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class SectorAction(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void TeleporterTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "teleporter_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "teleporter_test.lmp");
        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastMobjHash = DoomDebug.GetMobjHash(game.World);
            aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
        }

        Assert.Equal(0x3450bb23u, (uint)lastMobjHash);
        Assert.Equal(0x2669e089u, (uint)aggMobjHash);
    }

    [Fact]
    public void LocalDoorTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "localdoor_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "localdoor_test.lmp");

        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;
        var lastSectorHash = 0;
        var aggSectorHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastMobjHash = DoomDebug.GetMobjHash(game.World);
            aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            lastSectorHash = DoomDebug.GetSectorHash(game.World);
            aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
        }

        Assert.Equal(0x9d6c0abeu, (uint)lastMobjHash);
        Assert.Equal(0x7e1bb5f2u, (uint)aggMobjHash);
        Assert.Equal(0xfdf3e7a0u, (uint)lastSectorHash);
        Assert.Equal(0x0a0f1980u, (uint)aggSectorHash);
    }

    [Fact]
    public void PlatformTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "platform_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "platform_test.lmp");

        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;
        var lastSectorHash = 0;
        var aggSectorHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastMobjHash = DoomDebug.GetMobjHash(game.World);
            aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            lastSectorHash = DoomDebug.GetSectorHash(game.World);
            aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
        }

        Assert.Equal(0x3da2f507u, (uint)lastMobjHash);
        Assert.Equal(0x3402f715u, (uint)aggMobjHash);
        Assert.Equal(0xc71b4d00u, (uint)lastSectorHash);
        Assert.Equal(0x2fb8dd00u, (uint)aggSectorHash);
    }

    [Fact]
    public void SilentCrusherTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "silent_crusher_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "silent_crusher_test.lmp");

        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;
        var lastSectorHash = 0;
        var aggSectorHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastMobjHash = DoomDebug.GetMobjHash(game.World);
            aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            lastSectorHash = DoomDebug.GetSectorHash(game.World);
            aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
        }

        Assert.Equal(0xee31a164u, (uint)lastMobjHash);
        Assert.Equal(0x1f3fc7b4u, (uint)aggMobjHash);
        Assert.Equal(0x6d6a1f20u, (uint)lastSectorHash);
        Assert.Equal(0x34b4f740u, (uint)aggSectorHash);
    }
}