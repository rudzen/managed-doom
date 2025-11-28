using ManagedDoom.Doom.Graphics.Dummy;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Tests.UnitTests;

public sealed class SubsectorTest(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void LoadE1M1()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom1);
        var wad = new Wad(wadFile);
        var flats = new DummyFlatLookup(wad);
        var textures = new DummyTextureLookup(wad);
        var map = wad.GetLumpNumber("E1M1");
        var vertices = wad.CreateVertices(map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
        var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
        var segments = Seg.FromWad(wad, map + 5, vertices, lines);
        var subSectors = Subsector.FromWad(wad, map + 6, segments);

        Assert.Equal(239, subSectors.Length);

        Assert.Equal(8, subSectors[0].SegCount);
        for (var i = 0; i < 8; i++)
            Assert.True(segments[subSectors[0].FirstSeg + i] == segments[0 + i]);

        Assert.Equal(1, subSectors[54].SegCount);
        for (var i = 0; i < 1; i++)
            Assert.True(segments[subSectors[54].FirstSeg + i] == segments[181 + i]);

        Assert.Equal(2, subSectors[238].SegCount);
        for (var i = 0; i < 2; i++)
            Assert.True(segments[subSectors[238].FirstSeg + i] == segments[745 + i]);
    }

    [Fact]
    public void LoadMap01()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom2);
        var wad = new Wad(wadFile);
        var flats = new DummyFlatLookup(wad);
        var textures = new DummyTextureLookup(wad);
        var map = wad.GetLumpNumber("MAP01");
        var vertices = wad.CreateVertices(map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
        var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
        var segments = Seg.FromWad(wad, map + 5, vertices, lines);
        var subSectors = Subsector.FromWad(wad, map + 6, segments);

        Assert.Equal(194, subSectors.Length);

        Assert.Equal(4, subSectors[0].SegCount);
        for (var i = 0; i < 4; i++)
            Assert.True(segments[subSectors[0].FirstSeg + i] == segments[i]);

        Assert.Equal(4, subSectors[57].SegCount);
        for (var i = 0; i < 4; i++)
            Assert.True(segments[subSectors[57].FirstSeg + i] == segments[179 + i]);

        Assert.Equal(4, subSectors[193].SegCount);
        for (var i = 0; i < 4; i++)
            Assert.True(segments[subSectors[193].FirstSeg + i] == segments[597 + i]);
    }
}