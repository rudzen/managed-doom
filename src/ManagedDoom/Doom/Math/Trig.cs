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
using System.Runtime.InteropServices;

namespace ManagedDoom.Doom.Math;

public static partial class Trig
{
    public const int FineAngleCount = 8192;
    public const int FineMask = FineAngleCount - 1;
    public const int AngleToFineShift = 19;

    private const int FineCosineOffset = FineAngleCount / 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Tan(Angle anglePlus90)
    {
        ref var tangent = ref MemoryMarshal.GetArrayDataReference(fineTangent);
        return new Fixed(Unsafe.Add(ref tangent, anglePlus90.Data >> AngleToFineShift));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Tan(int fineAnglePlus90)
    {
        ref var tangent = ref MemoryMarshal.GetArrayDataReference(fineTangent);
        return new Fixed(Unsafe.Add(ref tangent, fineAnglePlus90));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Sin(Angle angle)
    {
        ref var sine = ref MemoryMarshal.GetArrayDataReference(fineSine);
        return new Fixed(Unsafe.Add(ref sine, angle.Data >> AngleToFineShift));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Sin(int fineAngle)
    {
        ref var sine = ref MemoryMarshal.GetArrayDataReference(fineSine);
        return new Fixed(Unsafe.Add(ref sine, fineAngle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Cos(Angle angle)
    {
        ref var sine = ref MemoryMarshal.GetArrayDataReference(fineSine);
        return new Fixed(Unsafe.Add(ref sine, (angle.Data >> AngleToFineShift) + FineCosineOffset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Cos(int fineAngle)
    {
        ref var sine = ref MemoryMarshal.GetArrayDataReference(fineSine);
        return new Fixed(Unsafe.Add(ref sine, fineAngle + FineCosineOffset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle TanToAngle(uint tan)
    {
        return new Angle(tanToAngle[tan]);
    }
}