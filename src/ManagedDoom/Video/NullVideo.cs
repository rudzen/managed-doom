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
using ManagedDoom.Doom.Math;
using ManagedDoom.Video.Renders.ThreeDee;

namespace ManagedDoom.Video;

public class NullVideo : IVideo
{
    private static NullVideo? instance;

    public void Render(Doom.Doom doom, Fixed frameFrac, in long fps)
    {
    }

    public void InitializeWipe()
    {
    }

    public bool HasFocus()
    {
        return true;
    }

    public int MaxWindowSize => ThreeDeeRenderer.MaxScreenSize;

    public int WindowSize
    {
        get => 7;
        set { }
    }

    public bool DisplayMessage
    {
        get => true;
        set { }
    }

    public int MaxGammaCorrectionLevel => 10;

    public int GammaCorrectionLevel
    {
        get => 2;
        set { }
    }

    public int WipeBandCount => 321;
    public int WipeHeight => 200;

    public static NullVideo GetInstance()
    {
        return instance ??= new NullVideo();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        // TODO release managed resources here
    }
}