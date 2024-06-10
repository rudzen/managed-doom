using System;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class ColorTranslation
{
    private readonly byte[] greenToGray;
    private readonly byte[] greenToBrown;
    private readonly byte[] greenToRed;

    public ColorTranslation()
    {
        greenToGray = new byte[256];
        greenToBrown = new byte[256];
        greenToRed = new byte[256];
        for (var i = 0; i < 256; i++)
        {
            greenToGray[i] = (byte)i;
            greenToBrown[i] = (byte)i;
            greenToRed[i] = (byte)i;
        }

        for (var i = 112; i < 128; i++)
        {
            greenToGray[i] -= 16;
            greenToBrown[i] -= 48;
            greenToRed[i] -= 80;
        }
    }

    public ReadOnlySpan<byte> GetTranslation(MobjFlags flags)
    {
        return ((int)(flags & MobjFlags.Translation) >> (int)MobjFlags.TransShift) switch
        {
            1 => greenToGray.AsSpan(),
            2 => greenToBrown.AsSpan(),
            _ => greenToRed.AsSpan()
        };
    }
}