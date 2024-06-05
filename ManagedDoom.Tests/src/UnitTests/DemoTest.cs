using ManagedDoom.Doom.Game;

namespace ManagedDoom.Tests.UnitTests;

public sealed class DemoTest(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void Doom2()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom2);

        using var content = GameContent.CreateDummy(wadFile);
        {
            var demo = new Demo(content.Wad.ReadLump("DEMO1"));
            Assert.Equal(11, demo.Options.Map);
            Assert.Equal(0, demo.Options.ConsolePlayer);
            Assert.True(demo.Options.Players[0].InGame);
            Assert.False(demo.Options.Players[1].InGame);
            Assert.False(demo.Options.Players[2].InGame);
            Assert.False(demo.Options.Players[3].InGame);
        }

        {
            var demo = new Demo(content.Wad.ReadLump("DEMO2"));
            Assert.Equal(5, demo.Options.Map);
            Assert.Equal(0, demo.Options.ConsolePlayer);
            Assert.True(demo.Options.Players[0].InGame);
            Assert.False(demo.Options.Players[1].InGame);
            Assert.False(demo.Options.Players[2].InGame);
            Assert.False(demo.Options.Players[3].InGame);
        }

        {
            var demo = new Demo(content.Wad.ReadLump("DEMO3"));
            Assert.Equal(26, demo.Options.Map);
            Assert.Equal(0, demo.Options.ConsolePlayer);
            Assert.True(demo.Options.Players[0].InGame);
            Assert.False(demo.Options.Players[1].InGame);
            Assert.False(demo.Options.Players[2].InGame);
            Assert.False(demo.Options.Players[3].InGame);
        }
    }
}