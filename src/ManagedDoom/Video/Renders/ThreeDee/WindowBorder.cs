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
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class WindowBorder(Wad wad, IFlatLookup flats)
{
    private readonly Patch borderTopLeft = Patch.FromWad(wad, "BRDR_TL");
    private readonly Patch borderTopRight = Patch.FromWad(wad, "BRDR_TR");
    private readonly Patch borderBottomLeft = Patch.FromWad(wad, "BRDR_BL");
    private readonly Patch borderBottomRight = Patch.FromWad(wad, "BRDR_BR");
    private readonly Patch borderTop = Patch.FromWad(wad, "BRDR_T");
    private readonly Patch borderBottom = Patch.FromWad(wad, "BRDR_B");
    private readonly Patch borderLeft = Patch.FromWad(wad, "BRDR_L");
    private readonly Patch borderRight = Patch.FromWad(wad, "BRDR_R");
    private readonly Flat backFlat = wad.GameMode == GameMode.Commercial
        ? flats["GRNROCK"]
        : flats["FLOOR7_2"];

    public void FillBackScreen(
        Span<byte> screenData,
        IDrawScreen screen,
        WindowSettings windowSettings,
        int drawScale)
    {
        var screenHeight = screen.Height;
        var screenWidth = screen.Width;
        var fillHeight = screenHeight - drawScale * StatusBarRenderer.Height;
        var backFlatData = backFlat.Data.AsSpan();

        FillRect(
            screenData: screenData,
            backFlatData: backFlatData,
            x: 0,
            y: 0,
            width: windowSettings.WindowX,
            height: fillHeight,
            screenHeight: screenHeight,
            drawScale: drawScale
        );

        FillRect(
            screenData: screenData,
            backFlatData: backFlatData,
            x: screenWidth - windowSettings.WindowX,
            y: 0,
            width: windowSettings.WindowX,
            height: fillHeight,
            screenHeight: screenHeight,
            drawScale: drawScale
        );

        FillRect(
            screenData: screenData,
            backFlatData: backFlatData,
            x: windowSettings.WindowX,
            y: 0,
            width: screenWidth - 2 * windowSettings.WindowX,
            height: windowSettings.WindowY,
            screenHeight: screenHeight,
            drawScale: drawScale
        );

        FillRect(
            screenData: screenData,
            backFlatData: backFlatData,
            x: windowSettings.WindowX,
            y: fillHeight - windowSettings.WindowY,
            width: screenWidth - 2 * windowSettings.WindowX,
            height: windowSettings.WindowY,
            screenHeight: screenHeight,
            drawScale: drawScale
        );

        var step = 8 * drawScale;

        for (var x = windowSettings.WindowX; x < screenWidth - windowSettings.WindowX; x += step)
        {
            screen.DrawPatch(borderTop, x, windowSettings.WindowY - step, drawScale);
            screen.DrawPatch(borderBottom, x, fillHeight - windowSettings.WindowY, drawScale);
        }

        for (var y = windowSettings.WindowY; y < fillHeight - windowSettings.WindowY; y += step)
        {
            screen.DrawPatch(borderLeft, windowSettings.WindowX - step, y, drawScale);
            screen.DrawPatch(borderRight, screenWidth - windowSettings.WindowX, y, drawScale);
        }

        screen.DrawPatch(borderTopLeft, windowSettings.WindowX - step, windowSettings.WindowY - step, drawScale);
        screen.DrawPatch(borderTopRight, screenWidth - windowSettings.WindowX, windowSettings.WindowY - step, drawScale);
        screen.DrawPatch(borderBottomLeft, windowSettings.WindowX - step, fillHeight - windowSettings.WindowY, drawScale);
        screen.DrawPatch(borderBottomRight, screenWidth - windowSettings.WindowX, fillHeight - windowSettings.WindowY, drawScale);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FillRect(
        Span<byte> screenData,
        ReadOnlySpan<byte> backFlatData,
        int x,
        int y,
        int width,
        int height,
        int screenHeight,
        int drawScale)
    {
        var srcX = x / drawScale;
        var srcY = y / drawScale;

        var invScale = Fixed.One / drawScale;
        var xFrac = invScale - Fixed.Epsilon;

        for (var i = 0; i < width; i++)
        {
            var src = ((srcX + xFrac.ToIntFloor()) & 63) << 6;
            var dst = screenHeight * (x + i) + y;
            var yFrac = invScale - Fixed.Epsilon;
            for (var j = 0; j < height; j++)
            {
                screenData[dst + j] = backFlatData[src | ((srcY + yFrac.ToIntFloor()) & 63)];
                yFrac += invScale;
            }

            xFrac += invScale;
        }
    }
}