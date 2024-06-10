using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class SkyRender
{
    public const int angleToSkyShift = 22;
    public Fixed SkyTextureAlt { get; } = Fixed.FromInt(100);
    public Fixed SkyInvScale { get; private set; } = Fixed.Zero;

    public void Reset(int windowWidth, int screenWidth, int screenHeight)
    {
        // The code below is based on PrBoom+' sky rendering implementation.
        var num = (long)Fixed.FracUnit * screenWidth * 200;
        var den = windowWidth * screenHeight;
        SkyInvScale = new Fixed((int)(num / den));
    }
}