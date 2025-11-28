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

public readonly record struct Arg(bool Present);

public readonly record struct Arg<T>
{
    public readonly bool Present;
    public readonly T? Value;

    public Arg(T value, bool present) : this()
    {
        Present = present;
        Value = value;
    }
}

public static class ArgExtensions
{
    public static Arg<T> Success<T>(T value) => new(value, true);

    public static Arg<T> Failure<T>() => new(default!, false);
}
