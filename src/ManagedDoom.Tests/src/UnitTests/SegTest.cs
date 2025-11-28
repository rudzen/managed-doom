using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Graphics.Dummy;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Wad;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace ManagedDoom.Tests.UnitTests;

public sealed class SegTest(WadPath wadPath) : IClassFixture<WadPath>
{
    private const double Delta = 1.0E-9;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double ToRadian(int angle)
    {
        if (angle < 0)
            angle += 0x10000;

        return 2 * Math.PI * ((double)angle / 0x10000);
    }

    [Fact]
    [SuppressMessage("Assertions", "xUnit2024:Do not use boolean asserts for simple equality tests")]
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

        Assert.Equal(747, segments.Length);

        Assert.True(segments[0].Vertex1 == vertices[132]);
        Assert.True(segments[0].Vertex2 == vertices[133]);
        Assert.Equal(ToRadian(4156), segments[0].Angle.ToRadian(), Delta);
        Assert.True(segments[0].LineDef == lines[160]);
        Assert.True((segments[0].LineDef.Flags & LineFlags.TwoSided) != 0);
        Assert.True(segments[0].FrontSector == segments[0].LineDef.FrontSide.Sector);
        Assert.True(segments[0].BackSector == segments[0].LineDef.BackSide.Sector);
        Assert.Equal(0, segments[0].Offset.ToDouble(), Delta);

        Assert.True(segments[28].Vertex1 == vertices[390]);
        Assert.True(segments[28].Vertex2 == vertices[131]);
        Assert.Equal(ToRadian(-32768), segments[28].Angle.ToRadian(), Delta);
        Assert.True(segments[28].LineDef == lines[480]);
        Assert.True((segments[0].LineDef.Flags & LineFlags.TwoSided) != 0);
        Assert.True(segments[28].FrontSector == segments[28].LineDef.BackSide.Sector);
        Assert.True(segments[28].BackSector == segments[28].LineDef.FrontSide.Sector);
        Assert.Equal(0, segments[28].Offset.ToDouble(), Delta);

        Assert.True(segments[744].Vertex1 == vertices[446]);
        Assert.True(segments[744].Vertex2 == vertices[374]);
        Assert.Equal(ToRadian(-16384), segments[744].Angle.ToRadian(), Delta);
        Assert.True(segments[744].LineDef == lines[452]);
        Assert.True((segments[744].LineDef.Flags & LineFlags.TwoSided) == 0);
        Assert.True(segments[744].FrontSector == segments[744].LineDef.FrontSide.Sector);
        Assert.Null(segments[744].BackSector);
        Assert.Equal(154, segments[744].Offset.ToDouble(), Delta);

        Assert.True(segments[746].Vertex1 == vertices[374]);
        Assert.True(segments[746].Vertex2 == vertices[368]);
        Assert.Equal(ToRadian(-13828), segments[746].Angle.ToRadian(), Delta);
        Assert.True(segments[746].LineDef == lines[451]);
        Assert.True((segments[746].LineDef.Flags & LineFlags.TwoSided) == 0);
        Assert.True(segments[746].FrontSector == segments[746].LineDef.FrontSide.Sector);
        Assert.Null(segments[746].BackSector);
        Assert.Equal(0, segments[746].Offset.ToDouble(), Delta);
    }

    [Fact]
    [SuppressMessage("Assertions", "xUnit2024:Do not use boolean asserts for simple equality tests")]
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

        Assert.Equal(601, segments.Length);

        Assert.True(segments[0].Vertex1 == vertices[9]);
        Assert.True(segments[0].Vertex2 == vertices[316]);
        Assert.Equal(ToRadian(-32768), segments[0].Angle.ToRadian(), Delta);
        Assert.True(segments[0].LineDef == lines[8]);
        Assert.True((segments[0].LineDef!.Flags & LineFlags.TwoSided) != 0);
        Assert.True(segments[0].FrontSector == segments[0].LineDef!.FrontSide!.Sector);
        Assert.True(segments[0].BackSector == segments[0].LineDef!.BackSide!.Sector);
        Assert.Equal(0, segments[0].Offset.ToDouble(), Delta);

        Assert.True(segments[42].Vertex1 == vertices[26]);
        Assert.True(segments[42].Vertex2 == vertices[320]);
        Assert.Equal(ToRadian(-22209), segments[42].Angle.ToRadian(), Delta);
        Assert.True(segments[42].LineDef == lines[33]);
        Assert.True((segments[42].LineDef.Flags & LineFlags.TwoSided) != 0);
        Assert.True(segments[42].FrontSector == segments[42].LineDef?.BackSide?.Sector);
        Assert.True(segments[42].BackSector == segments[42].LineDef?.FrontSide?.Sector);
        Assert.Equal(0, segments[42].Offset.ToDouble(), Delta);

        Assert.True(segments[103].Vertex1 == vertices[331]);
        Assert.True(segments[103].Vertex2 == vertices[329]);
        Assert.Equal(ToRadian(16384), segments[103].Angle.ToRadian(), Delta);
        Assert.True(segments[103].LineDef == lines[347]);
        Assert.True((segments[103].LineDef.Flags & LineFlags.TwoSided) == 0);
        Assert.True(segments[103].FrontSector == segments[103].LineDef?.FrontSide?.Sector);
        Assert.Null(segments[103].BackSector);
        Assert.Equal(64, segments[103].Offset.ToDouble(), Delta);

        Assert.True(segments[600].Vertex1 == vertices[231]);
        Assert.True(segments[600].Vertex2 == vertices[237]);
        Assert.Equal(ToRadian(-16384), segments[600].Angle.ToRadian(), Delta);
        Assert.True(segments[600].LineDef == lines[271]);
        Assert.True((segments[600].LineDef.Flags & LineFlags.TwoSided) != 0);
        Assert.True(segments[600].FrontSector == segments[600].LineDef?.BackSide?.Sector);
        Assert.True(segments[600].BackSector == segments[600].LineDef?.FrontSide?.Sector);
        Assert.Equal(0, segments[600].Offset.ToDouble(), Delta);
    }
}