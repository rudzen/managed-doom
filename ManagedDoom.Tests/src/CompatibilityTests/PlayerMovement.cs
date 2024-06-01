namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class PlayerMovement
{
    [Fact]
    public void PlayerMovementTest()
    {
        var wad = Path.Combine(WadPath.DataPath, "player_movement_test.wad");
        using var content = GameContent.CreateDummy(WadPath.Doom2, wad);
        var demoFile = Path.Combine(WadPath.DataPath, "player_movement_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

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
        var wad = Path.Combine(WadPath.DataPath, "thing_collision_test.wad");
        using var content = GameContent.CreateDummy(WadPath.Doom2, wad);
        var demoFile = Path.Combine(WadPath.DataPath, "thing_collision_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

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
        var wad = Path.Combine(WadPath.DataPath, "autoaim_test.wad");
        using var content = GameContent.CreateDummy(WadPath.Doom2, wad);
        var demoFile = Path.Combine(WadPath.DataPath, "autoaim_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

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