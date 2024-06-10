using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class WallRender(int screenWidth)
{
    private const int FineFov = 2048;

    // wall rendering

    public int[] AngleToX { get; set; } = new int[Trig.FineAngleCount / 2];

    public Angle[] XToAngle { get; set; } = new Angle[screenWidth];

    public Angle ClipAngle { get; private set; } = Angle.Ang0;

    public Angle ClipAngle2 { get; private set; } = Angle.Ang0;

    public void Reset(Fixed centerXFrac, int windowWidth)
    {
        var focalLength = centerXFrac / Trig.Tan(Trig.FineAngleCount / 4 + FineFov / 2);

        for (var i = 0; i < Trig.FineAngleCount / 2; i++)
        {
            int t;

            if (Trig.Tan(i) > Fixed.IntTwo)
                t = -1;
            else if (Trig.Tan(i) < Fixed.FromInt(-2))
                t = windowWidth + 1;
            else
            {
                t = (centerXFrac - Trig.Tan(i) * focalLength).ToIntCeiling();

                if (t < -1)
                    t = -1;
                else if (t > windowWidth + 1)
                    t = windowWidth + 1;
            }

            AngleToX[i] = t;
        }

        for (var x = 0; x < windowWidth; x++)
        {
            var i = 0;
            while (AngleToX[i] > x)
                i++;

            XToAngle[x] = new Angle((uint)(i << Trig.AngleToFineShift)) - Angle.Ang90;
        }

        for (var i = 0; i < Trig.FineAngleCount / 2; i++)
        {
            if (AngleToX[i] == -1)
                AngleToX[i] = 0;
            else if (AngleToX[i] == windowWidth + 1)
                AngleToX[i] = windowWidth;
        }

        ClipAngle = XToAngle[0];
        ClipAngle2 = ClipAngle * 2;
    }
}