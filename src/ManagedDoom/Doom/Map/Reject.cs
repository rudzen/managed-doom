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

using System.Runtime.CompilerServices;

namespace ManagedDoom.Doom.Map;

public sealed record Reject(byte[] Data, int SectorCount);

public static class RejectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Check(this Reject reject, Sector sector1, Sector sector2)
    {
        var s1 = sector1.Number;
        var s2 = sector2.Number;

        var p = s1 * reject.SectorCount + s2;
        var byteIndex = p >> 3;
        var bitIndex = 1 << (p & 7);

        return (reject.Data[byteIndex] & bitIndex) != 0;
    }
}