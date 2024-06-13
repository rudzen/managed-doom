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

namespace ManagedDoom.Audio;

public sealed class NullMusic : IMusic
{
    private static NullMusic? instance;

    public int MaxVolume => 15;

    public int Volume
    {
        get => 0;
        set { }
    }

    public static IMusic GetInstance()
    {
        return instance ??= new NullMusic();
    }

    public void StartMusic(Bgm bgm, bool loop)
    {
    }

    public void Dispose()
    {
    }
}