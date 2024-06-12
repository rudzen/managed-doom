using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class PlayerMovement(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void PlayerMovementTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "player_movement_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "player_movement_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
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

        Assert.Equal(0xe9a6d7d2u, (uint)lastHash);
        Assert.Equal(0x5e70c62du, (uint)aggHash);
    }

    [Fact]
    public void ThingCollisionTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "thing_collision_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "thing_collision_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
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

        Assert.Equal(0x63ff9173u, (uint)lastHash);
        Assert.Equal(0xb9cd0f6fu, (uint)aggHash);
    }

    [Fact]
    public void AutoAimTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "autoaim_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "autoaim_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
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

        Assert.Equal(0xe0d5d327u, (uint)lastHash);
        Assert.Equal(0x1a00fde9u, (uint)aggHash);
    }
}