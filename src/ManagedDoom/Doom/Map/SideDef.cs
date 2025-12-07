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

using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.Map;

public sealed class SideDef
{
    public SideDef(
        Fixed textureOffset,
        Fixed rowOffset,
        int topTexture,
        int bottomTexture,
        int middleTexture,
        Sector sector)
    {
        this.TextureOffset = textureOffset;
        this.RowOffset = rowOffset;
        this.TopTexture = topTexture;
        this.BottomTexture = bottomTexture;
        this.MiddleTexture = middleTexture;
        this.Sector = sector;
    }

    public Fixed TextureOffset { get; set; }

    public Fixed RowOffset { get; set; }

    public int TopTexture { get; set; }

    public int BottomTexture { get; set; }

    public int MiddleTexture { get; set; }

    public Sector Sector { get; }
}