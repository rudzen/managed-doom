//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
// Copyright (C)      2024 Rudy Alex Kohn
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset(WindowSettings windowSettings)
    {
        const short clipDataDefaultValue = -1;
        var clipData = ClipData.AsSpan();
        clipData[..windowSettings.WindowWidth].Fill(clipDataDefaultValue);
        clipData.Slice(windowSettings.WindowWidth, windowSettings.WindowWidth).Fill((short)windowSettings.WindowHeight);
        NegOneArray = 0;
        WindowHeightArray = windowSettings.WindowWidth;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear(WindowSettings windowSettings)
    {
        const short upperClipDefaultValue = -1;
        UpperClip.AsSpan(0, windowSettings.WindowWidth).Fill(upperClipDefaultValue);
        LowerClip.AsSpan(0, windowSettings.WindowWidth).Fill((short)windowSettings.WindowHeight);

        ClipRanges[0].First = -0x7fffffff;
        ClipRanges[0].Last = -1;
        ClipRanges[1].First = windowSettings.WindowWidth;
        ClipRanges[1].Last = 0x7fffffff;

        ClipRangeCount = 2;

        ClipDataLength = 2 * windowSettings.WindowWidth;

        VisWallRangeCount = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool VisualRangeExceeded() => VisWallRangeCount == VisWallRanges.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsClipIntoBufferSufficient(int range) => ClipDataLength + 3 * range >= ClipData.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref VisWallRange GetAndIncrementVisWallRange() => ref VisWallRanges[VisWallRangeCount++];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<VisWallRange> GetCurrentVisWallRanges() => VisWallRanges.AsSpan(0, VisWallRangeCount);
}