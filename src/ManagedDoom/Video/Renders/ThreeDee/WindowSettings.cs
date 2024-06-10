using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class WindowSettings
{
    public int WindowX { get; private set; }
    public int WindowY { get; private set; }
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    public int CenterY { get; private set; }
    public Fixed CenterXFrac { get; private set; }
    public Fixed CenterYFrac { get; private set; }
    public Fixed Projection { get; private set; }

    public void Reset(int x, int y, int width, int height)
    {
        WindowX = x;
        WindowY = y;
        WindowWidth = width;
        WindowHeight = height;
        var centerX = WindowWidth / 2;
        CenterY = WindowHeight / 2;
        CenterXFrac = Fixed.FromInt(centerX);
        CenterYFrac = Fixed.FromInt(CenterY);
        Projection = CenterXFrac;
    }
}