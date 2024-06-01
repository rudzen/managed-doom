﻿namespace ManagedDoom.Tests.UnitTests;

public sealed class RejectTest
{
    [Fact]
    public void LoadE1M1()
    {
        using var wad = new Wad(WadPath.Doom1);
        var flats = new DummyFlatLookup(wad);
        var textures = new DummyTextureLookup(wad);
        var map = wad.GetLumpNumber("E1M1");
        var vertices = Vertex.FromWad(wad, map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
        var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
        var reject = Reject.FromWad(wad, map + 9, sectors);

        foreach (var sector in sectors)
            Assert.False(reject.Check(sector, sector));

        foreach (var line in lines)
        {
            if (line.BackSector != null)
                Assert.False(reject.Check(line.FrontSector, line.BackSector));
        }

        foreach (var s1 in sectors)
        {
            foreach (var s2 in sectors)
            {
                var result1 = reject.Check(s1, s2);
                var result2 = reject.Check(s2, s1);
                Assert.Equal(result1, result2);
            }
        }

        Assert.True(reject.Check(sectors[41], sectors[70]));
        Assert.True(reject.Check(sectors[60], sectors[79]));
        Assert.True(reject.Check(sectors[24], sectors[80]));
    }

    [Fact]
    public void LoadMap01()
    {
        using var wad = new Wad(WadPath.Doom2);
        var flats = new DummyFlatLookup(wad);
        var textures = new DummyTextureLookup(wad);
        var map = wad.GetLumpNumber("MAP01");
        var vertices = Vertex.FromWad(wad, map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
        var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
        var reject = Reject.FromWad(wad, map + 9, sectors);

        foreach (var sector in sectors)
            Assert.False(reject.Check(sector, sector));

        foreach (var line in lines)
        {
            if (line.BackSector != null)
                Assert.False(reject.Check(line.FrontSector, line.BackSector));
        }

        foreach (var s1 in sectors)
        {
            foreach (var s2 in sectors)
            {
                var result1 = reject.Check(s1, s2);
                var result2 = reject.Check(s2, s1);
                Assert.Equal(result1, result2);
            }
        }

        Assert.True(reject.Check(sectors[10], sectors[49]));
        Assert.True(reject.Check(sectors[7], sectors[36]));
        Assert.True(reject.Check(sectors[17], sectors[57]));
    }
}