using ManagedDoom.Doom.Game;

namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class Monsters(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void NightmareTest()
    {
        var wad = wadPath.GetWadPath(WadFile.Doom2);
        using var content = GameContent.CreateDummy(wad);
        var demoFile = Path.Combine(WadPath.DataPath, "nightmare_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x9278a07au, (uint)lastHash);
        Assert.Equal(0xb2d9a9a0u, (uint)aggHash);
    }

    [Fact]
    public void BarrelTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "barrel_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "barrel_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xfb76dc03u, (uint)lastHash);
        Assert.Equal(0xccc38bc3u, (uint)aggHash);
    }

    [Fact]
    public void ZombiemanTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "zombieman_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "zombieman_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xe6ce947cu, (uint)lastHash);
        Assert.Equal(0xb4b0d9a0u, (uint)aggHash);
    }

    [Fact]
    public void ZombiemanTest2()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "zombieman_test2.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "zombieman_test2.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x97af3257u, (uint)lastHash);
        Assert.Equal(0x994fe30au, (uint)aggHash);
    }

    [Fact]
    public void ShotgunguyTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "shotgunguy_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "shotgunguy_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x7bc7cdbau, (uint)lastHash);
        Assert.Equal(0x8010e4ffu, (uint)aggHash);
    }

    [Fact]
    public void ChaingunguyTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "chaingunguy_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "chaingunguy_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xc135229fu, (uint)lastHash);
        Assert.Equal(0x7b9590d8u, (uint)aggHash);
    }

    [Fact]
    public void ImpTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "imp_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "imp_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xaeee7433u, (uint)lastHash);
        Assert.Equal(0x64f0da30u, (uint)aggHash);
    }

    [Fact]
    public void FastImpTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "imp_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "fast_imp_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x314b23f3u, (uint)lastHash);
        Assert.Equal(0x7ffd501du, (uint)aggHash);
    }

    [Fact]
    public void DemonTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "demon_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "demon_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xcfdcb5d1u, (uint)lastHash);
        Assert.Equal(0x37ad1000u, (uint)aggHash);
    }

    [Fact]
    public void FastDemonTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "demon_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "fast_demon_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x195cbb15u, (uint)lastHash);
        Assert.Equal(0x18bdbd50u, (uint)aggHash);
    }

    [Fact]
    public void LostSoulTest_Final2()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "lostsoul_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "lostsoul_test_final2.lmp");
        var demo = new Demo(demoFile)
        {
            Options =
            {
                GameVersion = GameVersion.Final2
            }
        };
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x2cdb1c94u, (uint)lastHash);
        Assert.Equal(0x99d18c88u, (uint)aggHash);
    }

    [Fact]
    public void CacoDemonTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "cacodemon_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "cacodemon_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x76c0d9f4u, (uint)lastHash);
        Assert.Equal(0xf40d2331u, (uint)aggHash);
    }

    [Fact]
    public void FastCacoDemonTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "cacodemon_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "fast_cacodemon_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x73316e3bu, (uint)lastHash);
        Assert.Equal(0x7219647fu, (uint)aggHash);
    }

    [Fact]
    public void BaronTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "baron_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "baron_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x3b6c14d3u, (uint)lastHash);
        Assert.Equal(0xdb003628u, (uint)aggHash);
    }

    [Fact]
    public void FastBaronTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "baron_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "fast_baron_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x79fb12efu, (uint)lastHash);
        Assert.Equal(0x1f5070bdu, (uint)aggHash);
    }

    [Fact]
    public void RevenantTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "revenant_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "revenant_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x8b9fe3aeu, (uint)lastHash);
        Assert.Equal(0x24e038d7u, (uint)aggHash);
    }

    [Fact]
    public void FatsoTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "fatso_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "fatso_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xadc6371eu, (uint)lastHash);
        Assert.Equal(0x196eebe6u, (uint)aggHash);
    }

    [Fact]
    public void ArachnotronTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "arachnotron_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "arachnotron_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xa630a85eu, (uint)lastHash);
        Assert.Equal(0x9881a8ffu, (uint)aggHash);
    }

    [Fact]
    public void PainElementalTest_Final2()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "painelemental_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "painelemental_test_final2.lmp");
        var demo = new Demo(demoFile)
        {
            Options =
            {
                GameVersion = GameVersion.Final2
            }
        };
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x6984f76fu, (uint)lastHash);
        Assert.Equal(0x50ba7933u, (uint)aggHash);
    }

    [Fact]
    public void ArchvileTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "archvile_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "archvile_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0xaa5531f2u, (uint)lastHash);
        Assert.Equal(0xeb4456c4u, (uint)aggHash);
    }

    [Fact]
    public void TelefragTest()
    {
        string[] wads = [wadPath.GetWadPath(WadFile.Doom2), Path.Combine(WadPath.DataPath, "telefrag_test.wad")];
        using var content = GameContent.CreateDummy(wads);
        var demoFile = Path.Combine(WadPath.DataPath, "telefrag_test.lmp");
        var demo = new Demo(demoFile);
        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(_ => new TicCmd()).ToArray();
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

        Assert.Equal(0x4c27ebc9u, (uint)lastHash);
        Assert.Equal(0xa93ecd0eu, (uint)aggHash);
    }
}