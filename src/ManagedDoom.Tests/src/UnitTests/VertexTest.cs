using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Tests.UnitTests;

public sealed class VertexTest(WadPath wadPath) : IClassFixture<WadPath>
{
    private const double Delta = 1.0E-9;

    [Fact]
    public void LoadE1M1()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom1);
        var wad = new Wad(wadFile);
        var map = wad.GetLumpNumber("E1M1");
        var vertices = wad.CreateVertices(map + 4);

        Assert.Equal(470, vertices.Length);

        Assert.Equal(1088, vertices[0].X.ToDouble(), Delta);
        Assert.Equal(-3680, vertices[0].Y.ToDouble(), Delta);

        Assert.Equal(128, vertices[57].X.ToDouble(), Delta);
        Assert.Equal(-3008, vertices[57].Y.ToDouble(), Delta);

        Assert.Equal(2435, vertices[469].X.ToDouble(), Delta);
        Assert.Equal(-3920, vertices[469].Y.ToDouble(), Delta);
    }

    [Fact]
    public void LoadMap01()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom2);
        var wad = new Wad(wadFile);
        var map = wad.GetLumpNumber("MAP01");
        var vertices = wad.CreateVertices(map + 4);

        Assert.Equal(383, vertices.Length);

        Assert.Equal(-448, vertices[0].X.ToDouble(), Delta);
        Assert.Equal(768, vertices[0].Y.ToDouble(), Delta);

        Assert.Equal(128, vertices[57].X.ToDouble(), Delta);
        Assert.Equal(1808, vertices[57].Y.ToDouble(), Delta);

        Assert.Equal(-64, vertices[382].X.ToDouble(), Delta);
        Assert.Equal(2240, vertices[382].Y.ToDouble(), Delta);
    }
}