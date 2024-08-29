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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ManagedDoom.Doom.Common;

public sealed class DoomString
{
    private static readonly Dictionary<string, DoomString> valueTable = [];
    private static readonly Dictionary<string, DoomString> nameTable = [];

    private string original;
    private string replaced;

    public DoomString(string original)
    {
        this.original = original;
        replaced = original;

        ref var current = ref CollectionsMarshal.GetValueRefOrAddDefault(valueTable, original, out var exists);
        if (!exists)
            current = this;
    }

    public DoomString(string name, string original) : this(original)
    {
        nameTable[name] = this;
    }

    public override string ToString()
    {
        return replaced;
    }

    public static implicit operator string(DoomString ds)
    {
        return ds.replaced;
    }

    public static void ReplaceByValue(string original, string replaced)
    {
        ref var ds = ref CollectionsMarshal.GetValueRefOrNullRef(valueTable, original);
        if (!Unsafe.IsNullRef(ref ds))
            ds.replaced = replaced;
    }

    public static void ReplaceByName(string name, string value)
    {
        ref var ds = ref CollectionsMarshal.GetValueRefOrNullRef(nameTable, name);
        if (!Unsafe.IsNullRef(ref ds))
            ds.replaced = value;
    }
}