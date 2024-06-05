using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Tests.UnitTests;

public sealed class MapTest(WadPath wadPath) : IClassFixture<WadPath>
{
    private const double MaxRadius = 32;

    [Fact]
    public void LoadE1M1()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom1);
        using var content = GameContent.CreateDummy(wadFile);
        var options = new GameOptions();
        var world = new World(content, options, null);
        var map = new Map(content, world);

        var mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
        var mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
        var mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
        var mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

        foreach (var sector in map.Sectors)
        {
            var sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

            Assert.Equal(sLines, sector.Lines);

            var minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - MaxRadius;
            minX = Math.Max(minX, mapMinX);
            var maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + MaxRadius;
            maxX = Math.Min(maxX, mapMaxX);
            var minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - MaxRadius;
            minY = Math.Max(minY, mapMinY);
            var maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + MaxRadius;
            maxY = Math.Min(maxY, mapMaxY);

            var bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
            var bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
            var bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
            var bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

            Assert.True(bboxLeft <= minX);
            Assert.True(bboxRight >= maxX);
            Assert.True(bboxTop >= maxY);
            Assert.True(bboxBottom <= minY);

            Assert.True(Math.Abs(bboxLeft - minX) <= 128);
            Assert.True(Math.Abs(bboxRight - maxX) <= 128);
            Assert.True(Math.Abs(bboxTop - maxY) <= 128);
            Assert.True(Math.Abs(bboxBottom - minY) <= 128);
        }
    }

    [Fact]
    public void LoadMap01()
    {
        var wadFile = wadPath.GetWadPath(WadFile.Doom2);
        using var content = GameContent.CreateDummy(wadFile);
        var options = new GameOptions();
        var world = new World(content, options, null);
        var map = new Map(content, world);

        var mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
        var mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
        var mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
        var mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

        foreach (var sector in map.Sectors)
        {
            var sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

            Assert.Equal(sLines, sector.Lines);

            var minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - MaxRadius;
            minX = Math.Max(minX, mapMinX);
            var maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + MaxRadius;
            maxX = Math.Min(maxX, mapMaxX);
            var minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - MaxRadius;
            minY = Math.Max(minY, mapMinY);
            var maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + MaxRadius;
            maxY = Math.Min(maxY, mapMaxY);

            var bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
            var bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
            var bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
            var bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

            Assert.True(bboxLeft <= minX);
            Assert.True(bboxRight >= maxX);
            Assert.True(bboxTop >= maxY);
            Assert.True(bboxBottom <= minY);

            Assert.True(Math.Abs(bboxLeft - minX) <= 128);
            Assert.True(Math.Abs(bboxRight - maxX) <= 128);
            Assert.True(Math.Abs(bboxTop - maxY) <= 128);
            Assert.True(Math.Abs(bboxBottom - minY) <= 128);
        }
    }
}