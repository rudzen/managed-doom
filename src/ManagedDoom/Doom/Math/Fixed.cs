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
using System.Runtime.CompilerServices;

namespace ManagedDoom.Doom.Math;

public readonly struct Fixed
{
    public const int FracBits = 16;
    public const int FracUnit = 1 << FracBits;

    public static Fixed Zero => new(0);
    public static Fixed One => new(FracUnit);

    public static Fixed IntTwo => FromInt(2);
    public static Fixed IntMinusTwo => FromInt(-2);

    public static Fixed MaxValue => new(int.MaxValue);
    public static Fixed MinValue => new(int.MinValue);

    public static Fixed Epsilon => new(1);
    public static Fixed OnePlusEpsilon => new(FracUnit + 1);
    public static Fixed OneMinusEpsilon => new(FracUnit - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fixed(int data)
    {
        this.Data = data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed FromInt(int value)
    {
        return new Fixed(value << FracBits);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed FromFloat(float value)
    {
        return new Fixed((int)(FracUnit * value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed FromDouble(double value)
    {
        return new Fixed((int)(FracUnit * value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ToFloat()
    {
        return (float)Data / FracUnit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ToDouble()
    {
        return (double)Data / FracUnit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Abs(Fixed a)
    {
        if (a.Data < 0)
        {
            return new Fixed(-a.Data);
        }

        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator +(Fixed a)
    {
        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator -(Fixed a)
    {
        return new Fixed(-a.Data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator +(Fixed a, Fixed b)
    {
        return new Fixed(a.Data + b.Data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator -(Fixed a, Fixed b)
    {
        return new Fixed(a.Data - b.Data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator *(Fixed a, Fixed b)
    {
        return new Fixed((int)((a.Data * (long)b.Data) >> FracBits));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator *(int a, Fixed b)
    {
        return new Fixed(a * b.Data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator *(Fixed a, int b)
    {
        return new Fixed(a.Data * b);
    }

    public static Fixed operator /(Fixed a, Fixed b)
    {
        if ((CIntAbs(a.Data) >> 14) >= CIntAbs(b.Data))
        {
            return new Fixed((a.Data ^ b.Data) < 0 ? int.MinValue : int.MaxValue);
        }

        return FixedDiv2(a, b);
    }

    // The Math.Abs method throws an exception if the input value is -2147483648.
    // Due to this behavior, the visibility check can crash in some maps.
    // So I implemented another Abs method, which is identical to C's one.
    private static int CIntAbs(int n)
    {
        return n < 0 ? -n : n;
    }

    private static Fixed FixedDiv2(Fixed a, Fixed b)
    {
        var c = a.Data / ((double)b.Data) * FracUnit;

        if (c is >= 2147483648.0 or < -2147483648.0)
        {
            throw new DivideByZeroException();
        }

        return new Fixed((int)c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator /(int a, Fixed b)
    {
        return FromInt(a) / b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator /(Fixed a, int b)
    {
        return new Fixed(a.Data / b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator <<(Fixed a, int b)
    {
        return new Fixed(a.Data << b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator >> (Fixed a, int b)
    {
        return new Fixed(a.Data >> b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Fixed a, Fixed b)
    {
        return a.Data == b.Data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Fixed a, Fixed b)
    {
        return a.Data != b.Data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Fixed a, Fixed b)
    {
        return a.Data < b.Data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Fixed a, Fixed b)
    {
        return a.Data > b.Data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Fixed a, Fixed b)
    {
        return a.Data <= b.Data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Fixed a, Fixed b)
    {
        return a.Data >= b.Data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Min(Fixed a, Fixed b)
    {
        if (a < b)
        {
            return a;
        }

        return b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Max(Fixed a, Fixed b)
    {
        if (a < b)
        {
            return b;
        }

        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToIntFloor()
    {
        return Data >> FracBits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToIntCeiling()
    {
        return (Data + FracUnit - 1) >> FracBits;
    }

    public override bool Equals(object? obj)
    {
        throw new NotSupportedException();
    }

    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    public override string ToString()
    {
        return ((double)Data / FracUnit).ToString();
    }

    public int Data
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}