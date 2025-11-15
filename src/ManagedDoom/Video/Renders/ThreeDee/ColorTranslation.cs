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
using ManagedDoom.Doom.World;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class ColorTranslation
{
    public const int TranslationChunkSize = 256;
    public const int GrayIndexStart = 0;
    public const int BrownIndexStart = TranslationChunkSize;
    public const int RedIndexStart = TranslationChunkSize * 2;
    private const int TranslationSize = TranslationChunkSize * 3;

    public readonly byte[] TranslationTable = new byte[TranslationSize];
}

public static class ColorTranslationExtensions
{
    extension(ColorTranslation translation)
    {
        public void InitTranslations()
        {
            var greenToGray = translation.TranslationTable.AsSpan(ColorTranslation.GrayIndexStart, ColorTranslation.TranslationChunkSize);
            var greenToBrown = translation.TranslationTable.AsSpan(ColorTranslation.BrownIndexStart, ColorTranslation.TranslationChunkSize);
            var greenToRed = translation.TranslationTable.AsSpan(ColorTranslation.RedIndexStart, ColorTranslation.TranslationChunkSize);

            for (var i = 0; i < ColorTranslation.TranslationChunkSize; i++)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> GetTranslation(MobjFlags flags)
        {
            return ((int)(flags & MobjFlags.Translation) >> (int)MobjFlags.TransShift) switch
            {
                1 => translation.TranslationTable.AsSpan(ColorTranslation.GrayIndexStart, 256),
                2 => translation.TranslationTable.AsSpan(ColorTranslation.BrownIndexStart, 256),
                _ => translation.TranslationTable.AsSpan(ColorTranslation.RedIndexStart, 256)
            };
        }
    }
}