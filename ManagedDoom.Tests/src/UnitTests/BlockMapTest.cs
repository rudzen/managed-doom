namespace ManagedDoom.Tests.UnitTests;

public sealed class BlockMapTest(WadPath wadPath) : IClassFixture<WadPath>
{
    [Fact]
    public void LoadE1M1()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom1);
        using var wad = new Wad(wadFile);
        var flats = new DummyFlatLookup(wad);
        var textures = new DummyTextureLookup(wad);
        var map = wad.GetLumpNumber("E1M1");
        var vertices = Vertex.FromWad(wad, map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
        var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
        var blockMap = BlockMap.FromWad(wad, map + 10, lines);

        {
            var minX = vertices.Select(v => v.X.ToDouble()).Min();
            var maxX = vertices.Select(v => v.X.ToDouble()).Max();
            var minY = vertices.Select(v => v.Y.ToDouble()).Min();
            var maxY = vertices.Select(v => v.Y.ToDouble()).Max();

            Assert.Equal(blockMap.OriginX.ToDouble(), minX, 64D);
            Assert.Equal(blockMap.OriginY.ToDouble(), minY, 64D);
            Assert.Equal((blockMap.OriginX + BlockMap.BlockSize * blockMap.Width).ToDouble(), maxX, 128D);
            Assert.Equal((blockMap.OriginY + BlockMap.BlockSize * blockMap.Height).ToDouble(), maxY, 128D);
        }

        var spots = new List<Tuple<int, int>>();
        for (var blockY = -2; blockY < blockMap.Height + 2; blockY++)
        {
            for (var blockX = -2; blockX < blockMap.Width + 2; blockX++)
                spots.Add(Tuple.Create(blockX, blockY));
        }

        var random = new Random(666);

        for (var i = 0; i < 50; i++)
        {
            var ordered = spots.OrderBy(spot => random.NextDouble()).ToArray();

            var total = 0;

            foreach (var (blockX, blockY) in ordered)
            {
                var minX = double.MaxValue;
                var maxX = double.MinValue;
                var minY = double.MaxValue;
                var maxY = double.MinValue;
                var count = 0;

                blockMap.IterateLines(
                    blockX,
                    blockY,
                    line =>
                    {
                        if (count != 0)
                        {
                            minX = Math.Min(Math.Min(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), minX);
                            maxX = Math.Max(Math.Max(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), maxX);
                            minY = Math.Min(Math.Min(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), minY);
                            maxY = Math.Max(Math.Max(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), maxY);
                        }

                        count++;
                        return true;
                    },
                    i + 1);

                if (count > 1)
                {
                    Assert.True(minX <= (blockMap.OriginX + BlockMap.BlockSize * (blockX + 1)).ToDouble());
                    Assert.True(maxX >= (blockMap.OriginX + BlockMap.BlockSize * blockX).ToDouble());
                    Assert.True(minY <= (blockMap.OriginY + BlockMap.BlockSize * (blockY + 1)).ToDouble());
                    Assert.True(maxY >= (blockMap.OriginY + BlockMap.BlockSize * blockY).ToDouble());
                }

                total += count;
            }

            Assert.Equal(lines.Length, total);
        }
    }

    [Fact]
    public void LoadMap01()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom2);
        using var wad = new Wad(wadFile);
        var flats = new DummyFlatLookup(wad);
        var textures = new DummyTextureLookup(wad);
        var map = wad.GetLumpNumber("MAP01");
        var vertices = Vertex.FromWad(wad, map + 4);
        var sectors = Sector.FromWad(wad, map + 8, flats);
        var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
        var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
        var blockMap = BlockMap.FromWad(wad, map + 10, lines);

        {
            var minX = vertices.Select(v => v.X.ToDouble()).Min();
            var maxX = vertices.Select(v => v.X.ToDouble()).Max();
            var minY = vertices.Select(v => v.Y.ToDouble()).Min();
            var maxY = vertices.Select(v => v.Y.ToDouble()).Max();

            Assert.Equal(blockMap.OriginX.ToDouble(), minX, 64D);
            Assert.Equal(blockMap.OriginY.ToDouble(), minY, 64D);
            Assert.Equal((blockMap.OriginX + BlockMap.BlockSize * blockMap.Width).ToDouble(), maxX, 128D);
            Assert.Equal((blockMap.OriginY + BlockMap.BlockSize * blockMap.Height).ToDouble(), maxY, 128D);
        }

        var spots = new List<Tuple<int, int>>();
        for (var blockY = -2; blockY < blockMap.Height + 2; blockY++)
        {
            for (var blockX = -2; blockX < blockMap.Width + 2; blockX++)
            {
                spots.Add(Tuple.Create(blockX, blockY));
            }
        }

        var random = new Random(666);

        for (var i = 0; i < 50; i++)
        {
            var ordered = spots.OrderBy(spot => random.NextDouble()).ToArray();

            var total = 0;

            foreach (var (blockX, blockY) in ordered)
            {
                var minX = double.MaxValue;
                var maxX = double.MinValue;
                var minY = double.MaxValue;
                var maxY = double.MinValue;
                var count = 0;

                blockMap.IterateLines(
                    blockX,
                    blockY,
                    line =>
                    {
                        if (count != 0)
                        {
                            minX = Math.Min(Math.Min(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), minX);
                            maxX = Math.Max(Math.Max(line.Vertex1.X.ToDouble(), line.Vertex2.X.ToDouble()), maxX);
                            minY = Math.Min(Math.Min(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), minY);
                            maxY = Math.Max(Math.Max(line.Vertex1.Y.ToDouble(), line.Vertex2.Y.ToDouble()), maxY);
                        }

                        count++;
                        return true;
                    },
                    i + 1);

                if (count > 1)
                {
                    Assert.True(minX <= (blockMap.OriginX + BlockMap.BlockSize * (blockX + 1)).ToDouble());
                    Assert.True(maxX >= (blockMap.OriginX + BlockMap.BlockSize * blockX).ToDouble());
                    Assert.True(minY <= (blockMap.OriginY + BlockMap.BlockSize * (blockY + 1)).ToDouble());
                    Assert.True(maxY >= (blockMap.OriginY + BlockMap.BlockSize * blockY).ToDouble());
                }

                total += count;
            }

            Assert.Equal(lines.Length, total);
        }
    }
}