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

namespace ManagedDoom.Config;

public interface ICommandLineArgs
{
    Arg<string> Iwad { get; }
    Arg<string[]> File { get; }
    Arg<string[]> Deh { get; }
    Arg<Warp> Warp { get; }
    Arg<int> Episode { get; }
    Arg<int> Skill { get; }
    Arg DeathMatch { get; }
    Arg AltDeath { get; }
    Arg Fast { get; }
    Arg Respawn { get; }
    Arg NoMonsters { get; }
    Arg SoloNet { get; }
    Arg<string> PlayDemo { get; }
    Arg<string> TimeDemo { get; }
    Arg<int> LoadGame { get; }
    Arg NoMouse { get; }
    Arg NoSound { get; }
    Arg NoSfx { get; }
    Arg NoMusic { get; }
    Arg NoDeh { get; }
}