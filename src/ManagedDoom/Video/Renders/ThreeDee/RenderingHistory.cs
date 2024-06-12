using System;
using System.Runtime.CompilerServices;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class RenderingHistory
{
    public RenderingHistory(int screenWidth)
    {
        UpperClip = new short[screenWidth];
        LowerClip = new short[screenWidth];

        ClipRanges = new ClipRange[256];
        for (var i = 0; i < ClipRanges.Length; i++)
            ClipRanges[i] = new ClipRange();

        ClipData = new short[128 * screenWidth];

        VisWallRanges = new VisWallRange[512];
        for (var i = 0; i < VisWallRanges.Length; i++)
            VisWallRanges[i] = new VisWallRange();
    }

    public short[] UpperClip { get; }
    public short[] LowerClip { get; }

    public int NegOneArray { get; private set; }
    public int WindowHeightArray { get; private set; }

    public int ClipRangeCount { get; set; }
    public ClipRange[] ClipRanges { get; }

    public int ClipDataLength { get; set; }
    public short[] ClipData { get; }

    public int VisWallRangeCount { get; private set; }
    public VisWallRange[] VisWallRanges { get; }


    public void Reset(WindowSettings windowSettings)
    {
        Array.Fill(ClipData, (short)-1, 0, windowSettings.WindowWidth);
        NegOneArray = 0;
        Array.Fill(ClipData, (short)windowSettings.WindowHeight, windowSettings.WindowWidth, windowSettings.WindowWidth);
        WindowHeightArray = windowSettings.WindowWidth;
    }

    public void Clear(WindowSettings windowSettings)
    {
        const short upperClipDefaultValue = -1;
        Array.Fill(UpperClip, upperClipDefaultValue, 0, windowSettings.WindowWidth);
        Array.Fill(LowerClip, (short)windowSettings.WindowHeight, 0, windowSettings.WindowWidth);

        ClipRanges[0].First = -0x7fffffff;
        ClipRanges[0].Last = -1;
        ClipRanges[1].First = windowSettings.WindowWidth;
        ClipRanges[1].Last = 0x7fffffff;

        ClipRangeCount = 2;

        ClipDataLength = 2 * windowSettings.WindowWidth;

        VisWallRangeCount = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool VisualRangeExceeded()
    {
        return VisWallRangeCount == VisWallRanges.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsClipIntoBufferSufficient(int range)
    {
        return ClipDataLength + 3 * range >= ClipData.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref VisWallRange GetAndIncrementVisWallRange()
    {
        return ref VisWallRanges[VisWallRangeCount++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<VisWallRange> GetCurrentVisWallRanges()
    {
        return VisWallRanges.AsSpan(0, VisWallRangeCount);
    }
}