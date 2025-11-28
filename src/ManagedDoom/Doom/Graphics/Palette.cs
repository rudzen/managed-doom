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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace ManagedDoom.Doom.Graphics;

public sealed class Palette
{
    public const int DamageStart = 1;
    public const int DamageCount = 8;

    public const int BonusStart = 9;
    public const int BonusCount = 4;

    public const int IronFeet = 13;

    private readonly byte[] data;

    private readonly uint[][] palettes;

    public Palette(Wad.Wad wad)
    {
        try
        {
            Console.Write("Load palette: ");
            var start = Stopwatch.GetTimestamp();

            data = wad.ReadLump("PLAYPAL");

            var count = data.Length / (3 * 256);
            palettes = new uint[count][];
            for (var i = 0; i < palettes.Length; i++)
                palettes[i] = new uint[256];

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK [{end}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }
    }

    public uint[] this[int paletteNumber] => palettes[paletteNumber];

    [SkipLocalsInit]
    public void ResetColors(in double p)
    {
        // build lookup table for corrected byte values (0..255) for this p
        Span<byte> lut = stackalloc byte[256];
        for (var v = 0; v < lut.Length; v++)
            lut[v] = (byte)System.Math.Round(255 * CorrectionCurve(v / 255.0, in p));

        const uint alpha = 255u << 24;
        var palettesCount = palettes.Length;
        var dataSpanAll = data.AsSpan();

        for (var pi = 0; pi < palettesCount; pi++)
        {
            var paletteOffset = 3 * 256 * pi;
            var src = dataSpanAll.Slice(paletteOffset, 3 * 256);
            var dest = palettes[pi].AsSpan();

            for (var j = 0; j < 256; j++)
            {
                var idx = j * 3;
                var r = lut[src[idx]];
                var g = lut[src[idx + 1]];
                var b = lut[src[idx + 2]];

                dest[j] = r | ((uint)g << 8) | ((uint)b << 16) | alpha;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double CorrectionCurve(double x, in double p)
    {
        return System.Math.Pow(x, p);
    }
}