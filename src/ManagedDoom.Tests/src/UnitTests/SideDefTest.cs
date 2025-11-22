using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Graphics.Dummy;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Tests.UnitTests;

public sealed class SideDefTest(WadPath wadPath) : IClassFixture<WadPath>
{
    private const double Delta = 1.0E-9;

    [Fact]
    public void LoadE1M1()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom1);
        var wad = new Wad(wadFile);
        var flats = new DummyFlatLookup(wad);
        var textures = new TextureLookup(wad);
        var map = wad.GetLumpNumber("E1M1");
        _ = wad.CreateVertices(map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);

        Assert.Equal(666, sides.Length);

        Assert.Equal(0, sides[0].TextureOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[0].RowOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[0].TopTexture);
        Assert.Equal(0, sides[0].BottomTexture);
        Assert.Equal("DOOR3", textures[sides[0].MiddleTexture].Name);
        Assert.True(sides[0].Sector == sectors[30]);

        Assert.Equal(32, sides[480].TextureOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[480].RowOffset.ToDouble(), Delta);
        Assert.Equal("EXITSIGN", textures[sides[480].TopTexture].Name);
        Assert.Equal(0, sides[480].BottomTexture);
        Assert.Equal(0, sides[480].MiddleTexture);
        Assert.True(sides[480].Sector == sectors[70]);

        Assert.Equal(0, sides[650].TextureOffset.ToDouble(), Delta);
        Assert.Equal(88, sides[650].RowOffset.ToDouble(), Delta);
        Assert.Equal("STARTAN3", textures[sides[650].TopTexture].Name);
        Assert.Equal("STARTAN3", textures[sides[650].BottomTexture].Name);
        Assert.Equal(0, sides[650].MiddleTexture);
        Assert.True(sides[650].Sector == sectors[1]);

        Assert.Equal(0, sides[665].TextureOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[665].RowOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[665].TopTexture);
        Assert.Equal(0, sides[665].BottomTexture);
        Assert.Equal(0, sides[665].MiddleTexture);
        Assert.True(sides[665].Sector == sectors[23]);
    }

    [Fact]
    public void LoadMap01()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom2);
        var wad = new Wad(wadFile);
        var flats = new DummyFlatLookup(wad);
        var textures = new TextureLookup(wad);
        var map = wad.GetLumpNumber("MAP01");
        _ = wad.CreateVertices(map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);

        Assert.Equal(529, sides.Length);

        Assert.Equal(0, sides[0].TextureOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[0].RowOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[0].TopTexture);
        Assert.Equal(0, sides[0].BottomTexture);
        Assert.Equal("BRONZE1", textures[sides[0].MiddleTexture].Name);
        Assert.True(sides[0].Sector == sectors[9]);

        Assert.Equal(0, sides[312].TextureOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[312].RowOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[312].TopTexture);
        Assert.Equal(0, sides[312].BottomTexture);
        Assert.Equal("DOORTRAK", textures[sides[312].MiddleTexture].Name);
        Assert.True(sides[312].Sector == sectors[31]);

        Assert.Equal(24, sides[512].TextureOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[512].RowOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[512].TopTexture);
        Assert.Equal(0, sides[512].BottomTexture);
        Assert.Equal("SUPPORT2", textures[sides[512].MiddleTexture].Name);
        Assert.True(sides[512].Sector == sectors[52]);

        Assert.Equal(0, sides[528].TextureOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[528].RowOffset.ToDouble(), Delta);
        Assert.Equal(0, sides[528].TopTexture);
        Assert.Equal(0, sides[528].BottomTexture);
        Assert.Equal(0, sides[528].MiddleTexture);
        Assert.True(sides[528].Sector == sectors[11]);
    }
}