using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class PlaneRender(int screenWidth, int screenHeight)
{
    public Fixed[] PlaneYSlope { get; } = new Fixed[screenHeight];
    public Fixed[] PlaneDistScale { get; } = new Fixed[screenWidth];
    public Fixed PlaneBaseXScale { get; private set; }
    public Fixed PlaneBaseYScale { get; private set; }

    public Sector? CeilingPrevSector { get; set; }
    public int CeilingPrevX { get; set; }
    public int CeilingPrevY1 { get; set; }
    public int CeilingPrevY2 { get; set; }
    public Fixed[] CeilingXFrac { get; } = new Fixed[screenHeight];
    public Fixed[] CeilingYFrac { get; } = new Fixed[screenHeight];
    public Fixed[] CeilingXStep { get; } = new Fixed[screenHeight];
    public Fixed[] CeilingYStep { get; } = new Fixed[screenHeight];
    public byte[][] CeilingLights { get; } = new byte[screenHeight][];

    public Sector? FloorPrevSector { get; set; }
    public int FloorPrevX { get; set; }
    public int FloorPrevY1 { get; set; }
    public int FloorPrevY2 { get; set; }
    public Fixed[] FloorXFrac { get; } = new Fixed[screenHeight];
    public Fixed[] FloorYFrac { get; } = new Fixed[screenHeight];
    public Fixed[] FloorXStep { get; } = new Fixed[screenHeight];
    public Fixed[] FloorYStep { get; } = new Fixed[screenHeight];
    public byte[][] FloorLights { get; } = new byte[screenHeight][];

    public void Reset(int windowWidth, int windowHeight, WallRender wallRender)
    {
        for (var i = 0; i < windowHeight; i++)
        {
            var dy = Fixed.FromInt(i - windowHeight / 2) + Fixed.One / 2;
            dy = Fixed.Abs(dy);
            PlaneYSlope[i] = Fixed.FromInt(windowWidth / 2) / dy;
        }

        for (var i = 0; i < windowWidth; i++)
        {
            var cos = Fixed.Abs(Trig.Cos(wallRender.XToAngle[i]));
            PlaneDistScale[i] = Fixed.One / cos;
        }
    }

    public void Clear(Angle viewAngle, Fixed centerXFrac)
    {
        var angle = viewAngle - Angle.Ang90;
        PlaneBaseXScale = Trig.Cos(angle) / centerXFrac;
        PlaneBaseYScale = -(Trig.Sin(angle) / centerXFrac);

        CeilingPrevSector = null;
        CeilingPrevX = int.MaxValue;

        FloorPrevSector = null;
        FloorPrevX = int.MaxValue;
    }
}