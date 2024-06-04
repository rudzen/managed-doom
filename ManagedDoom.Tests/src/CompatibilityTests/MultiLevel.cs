﻿namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class MultiLevel(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void MultiLevelTest_Doom1()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom1), Path.Combine(WadPath.DataPath, "multilevel_test_doom1.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "multilevel_test_doom1.lmp");
        var demo = new Demo(demoFile)
        {
            Options =
            {
                GameMode = GameMode.Retail
            }
        };
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;

        // E1M1
        {
            for (var i = 0; i < 456; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xd76283ddu, (uint)lastMobjHash);
            Assert.Equal(0x8e50483eu, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 523; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // E1M2
        {
            for (var i = 0; i < 492; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0x4499dad4u, (uint)lastMobjHash);
            Assert.Equal(0xf2bdb9dfu, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 368; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // E1M3
        {
            for (var i = 0; i < 424; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0x807bef69u, (uint)lastMobjHash);
            Assert.Equal(0x7fcd8281u, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 28; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // E1M4
        {
            for (var i = 0; i < 507; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xd79ad915u, (uint)lastMobjHash);
            Assert.Equal(0x504e3b27u, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 253; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // E1M5
        {
            for (var i = 0; i < 532; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0x233e5471u, (uint)lastMobjHash);
            Assert.Equal(0xc5060c4eu, (uint)aggMobjHash);
        }
    }

    [Fact]
    public void MultiLevelTest_Doom2()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "multilevel_test_doom2.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "multilevel_test_doom2.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;

        // MAP01
        {
            for (var i = 0; i < 801; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xc88d8c72u, (uint)lastMobjHash);
            Assert.Equal(0x50d51db4u, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 378; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // MAP02
        {
            for (var i = 0; i < 334; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xeb24ae67u, (uint)lastMobjHash);
            Assert.Equal(0xa08bf6ceu, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 116; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // MAP03
        {
            for (var i = 0; i < 653; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xb4d74694u, (uint)lastMobjHash);
            Assert.Equal(0xd813ac0fu, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 131; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // MAP04
        {
            for (var i = 0; i < 469; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xaf214214u, (uint)lastMobjHash);
            Assert.Equal(0xad054ab5u, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 236; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // MAP05
        {
            for (var i = 0; i < 312; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xeb01a1fau, (uint)lastMobjHash);
            Assert.Equal(0x0e4e66ffu, (uint)aggMobjHash);
        }
    }

    [Fact]
    public void FinaleTest1()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "finale_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "finale_test1.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;

        // MAP06
        {
            for (var i = 0; i < 430; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0x8acb1b96u, (uint)lastMobjHash);
            Assert.Equal(0xf67d1a1bu, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 156; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // Finale
        {
            for (var i = 0; i < 237; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
            }
        }

        // MAP07
        {
            for (var i = 0; i < 872; i++)
            {
                demo.ReadCmd(ticCommands);
                game.Update(ticCommands);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xfed7679fu, (uint)lastMobjHash);
            Assert.Equal(0x21340096u, (uint)aggMobjHash);
        }
    }

    [Fact]
    public void FinaleTest2()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "finale_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "finale_test2.lmp");
        var demo = new Demo(demoFile);
        var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, demo.Options);
        game.DeferedInitNew();

        var lastMobjHash = 0;
        var aggMobjHash = 0;

        // MAP06
        {
            for (var i = 0; i < 475; i++)
            {
                demo.ReadCmd(cmds);
                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0x4ea33dfbu, (uint)lastMobjHash);
            Assert.Equal(0x7dd3df51u, (uint)aggMobjHash);
        }

        // Intermission
        {
            for (var i = 0; i < 21; i++)
            {
                demo.ReadCmd(cmds);
                game.Update(cmds);
            }
        }

        // Finale
        {
            for (var i = 0; i < 52; i++)
            {
                demo.ReadCmd(cmds);
                game.Update(cmds);
            }
        }

        // MAP07
        {
            for (var i = 0; i < 494; i++)
            {
                demo.ReadCmd(cmds);
                game.Update(cmds);
                lastMobjHash = DoomDebug.GetMobjHash(game.World);
                aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
            }

            Assert.Equal(0xa7c0a6eau, (uint)lastMobjHash);
            Assert.Equal(0xe89c0706u, (uint)aggMobjHash);
        }
    }
}