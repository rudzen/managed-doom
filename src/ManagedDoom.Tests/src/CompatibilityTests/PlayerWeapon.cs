using ManagedDoom.Doom.Game;

namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class PlayerWeapon(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void PunchTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "punch_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "punch_test.lmp");
        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0x3d6c0f49u, (uint)lastHash);
        Assert.Equal(0x97d3aa02u, (uint)aggHash);
    }

    [Fact]
    public void ChainsawTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "chainsaw_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "chainsaw_test.lmp");
        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0x5db30e69u, (uint)lastHash);
        Assert.Equal(0xed598949u, (uint)aggHash);
    }

    [Fact]
    public void ShotgunTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "shotgun_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "shotgun_test.lmp");
        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0x3dd50799u, (uint)lastHash);
        Assert.Equal(0x4ddd814fu, (uint)aggHash);
    }

    [Fact]
    public void SuperShotgunTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "supershotgun_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "supershotgun_test.lmp");
        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0xe2f7936eu, (uint)lastHash);
        Assert.Equal(0x538061e4u, (uint)aggHash);
    }

    [Fact]
    public void ChaingunTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "chaingun_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "chaingun_test.lmp");
        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0x0b30e14bu, (uint)lastHash);
        Assert.Equal(0xb2104158u, (uint)aggHash);
    }

    [Fact]
    public void RocketTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "rocket_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "rocket_test.lmp");
        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0x8dce774bu, (uint)lastHash);
        Assert.Equal(0x87f45b5bu, (uint)aggHash);
    }

    [Fact]
    public void PlasmaTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "plasma_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "plasma_test.lmp");

        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0x116924d3u, (uint)lastHash);
        Assert.Equal(0x88fc9e68u, (uint)aggHash);
    }

    [Fact]
    public void BfgTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "bfg_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "bfg_test.lmp");

        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0xdeaf403fu, (uint)lastHash);
        Assert.Equal(0xb2c67368u, (uint)aggHash);
    }

    [Fact]
    public void SkyShootTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "sky_shoot_test.wad")];
        var demoFile = Path.Combine(WadPath.DataPath, "sky_shoot_test.lmp");

        using var content = GameContent.CreateDummy(wads);
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCommand()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferInitNew();

        var lastHash = 0;
        var aggHash = 0;

        while (true)
        {
            if (!demo.ReadCmd(ticCommands))
                break;

            game.Update(ticCommands);
            lastHash = DoomDebug.GetMobjHash(game.World);
            aggHash = DoomDebug.CombineHash(aggHash, lastHash);
        }

        Assert.Equal(0xfe794466u, (uint)lastHash);
        Assert.Equal(0xc71f30b2u, (uint)aggHash);
    }
}