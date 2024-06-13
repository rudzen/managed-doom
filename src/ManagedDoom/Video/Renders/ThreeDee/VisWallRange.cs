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

using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class VisWallRange
{
    public Seg Seg;

    public int X1;
    public int X2;

    public Fixed Scale1;
    public Fixed Scale2;
    public Fixed ScaleStep;

    public Silhouette Silhouette;
    public Fixed UpperSilHeight;
    public Fixed LowerSilHeight;

    public int UpperClip;
    public int LowerClip;
    public int MaskedTextureColumn;

    public Fixed FrontSectorFloorHeight;
    public Fixed FrontSectorCeilingHeight;
    public Fixed BackSectorFloorHeight;
    public Fixed BackSectorCeilingHeight;
}