﻿namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class Miscellaneous
{
    [Fact]
    public void Altdeath()
    {
        using var content = GameContent.CreateDummy(WadPath.Doom2);
        var demoFile = Path.Combine(WadPath.DataPath, "altdeath_test.lmp");
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

        Assert.Equal(0xf598b1d9u, (uint)lastMobjHash);
        Assert.Equal(0x9f716cfau, (uint)aggMobjHash);
    }
}