﻿//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
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

namespace ManagedDoom
{
    public sealed class Palette
    {
        public const int DamageStart = 1;
        public const int DamageCount = 8;

        public const int BonusStart = 9;
        public const int BonusCount = 4;

        public const int IronFeet = 13;

        private readonly byte[] data;

        private readonly uint[][] palettes;

        public Palette(Wad wad)
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

                Console.WriteLine("OK [" + Stopwatch.GetElapsedTime(start) + ']');
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void ResetColors(in double p)
        {
            for (var i = 0; i < palettes.Length; i++)
            {
                var paletteOffset = (3 * 256) * i;
                for (var j = 0; j < 256; j++)
                {
                    var colorOffset = paletteOffset + 3 * j;

                    var r = data[colorOffset];
                    var g = data[colorOffset + 1];
                    var b = data[colorOffset + 2];

                    r = (byte)Math.Round(255 * CorrectionCurve(r / 255.0, p));
                    g = (byte)Math.Round(255 * CorrectionCurve(g / 255.0, p));
                    b = (byte)Math.Round(255 * CorrectionCurve(b / 255.0, p));

                    palettes[i][j] = (uint)((r << 0) | (g << 8) | (b << 16) | (255 << 24));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CorrectionCurve(double x, double p)
        {
            return Math.Pow(x, p);
        }

        public uint[] this[int paletteNumber] => palettes[paletteNumber];
    }
}
