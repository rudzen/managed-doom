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
using ManagedDoom.Doom.Graphics;

namespace ManagedDoom.Video;

public interface IDrawScreen
{
    byte[] Data { get; }
    int Width { get; }
    int Height { get; }
    void DrawPatch(Patch patch, int x, int y, int scale);
    void DrawPatchFlip(Patch patch, int x, int y, int scale);
    void DrawText(ReadOnlySpan<char> text, int x, int y, int scale);
    void DrawText(string text, int x, int y, int scale);
    void DrawChar(char ch, int x, int y, int scale);
    int MeasureChar(char ch, int scale);
    int MeasureText(ReadOnlySpan<char> text, int scale);
    int MeasureText(string text, int scale);
    void FillRect(int x, int y, int w, int h, int color);
    void DrawLine(float x1, float y1, float x2, float y2, int color);
}