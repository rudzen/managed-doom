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

using ManagedDoom.Doom.Graphics;

namespace ManagedDoom.Doom.Map;

public sealed record Map(
    ITextureLookup Textures,
    IFlatLookup Flats,
    TextureAnimationInfo[] Animations,
    Vertex[] Vertices,
    Sector[] Sectors,
    LineDef[] Lines,
    Seg[] Segs,
    Subsector[] Subsectors,
    Node[] Nodes,
    MapThing[] Things,
    BlockMap BlockMap,
    Reject Reject,
    Texture SkyTexture,
    string Title
)
{
    public int SkyFlatNumber => Flats.SkyFlatNumber;
}