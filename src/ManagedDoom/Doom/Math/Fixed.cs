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
using ManagedDoom.Extensions;

namespace ManagedDoom.Doom.Math;

/// <summary>
/// Fixed-point number representation using 16.16 format (16 bits integer, 16 bits fractional).
///
/// WHY FIXED-POINT INSTEAD OF FLOATING-POINT?
/// ------------------------------------------
/// Original DOOM (1993) was designed to run on 386/486 processors without FPUs (Floating Point Units).
/// Floating-point operations were extremely slow or unavailable on these systems, making fixed-point
/// arithmetic essential for acceptable performance.
///
/// STORAGE FORMAT:
/// ---------------
/// Data is stored as a 32-bit signed integer where:
/// - Upper 16 bits (bits 31-16): Integer part (range: -32768 to 32767)
/// - Lower 16 bits (bits 15-0):  Fractional part (range: 0 to 0.9999847...)
///
/// Example: The value 5.25 is stored as:
///   5 << 16 + 0.25 * 65536 = 327680 + 16384 = 344064
///   Binary: 0000 0000 0000 0101 . 0100 0000 0000 0000
///           [    integer = 5   ] [  fraction ≈ 0.25  ]
///
/// WHY 16.16 FORMAT?
/// -----------------
/// 1. BALANCE: 16 integer bits provide range of ±32768 units (sufficient for game world coordinates)
/// 2. PRECISION: 16 fractional bits provide 1/65536 ≈ 0.000015 precision (good for smooth movement)
/// 3. EFFICIENCY: Power-of-2 alignment allows fast bit-shift operations instead of multiplication/division
/// 4. DETERMINISM: Integer arithmetic is perfectly deterministic across platforms (critical for demos/multiplayer)
///
/// ARITHMETIC ADVANTAGES:
/// ----------------------
/// - Addition/Subtraction: Direct integer ops (no adjustment needed since both operands have same scale)
/// - Multiplication: Multiply then shift right 16 bits to remove extra fractional bits
/// - Division: Scale up numerator before dividing to maintain precision
/// - Comparisons: Simple integer comparisons work directly
///
/// OPTIMIZATION OPPORTUNITIES:
/// ---------------------------
/// Modern .NET offers several potential improvements over the original fixed-point approach:
///
/// 1. SIMD VECTORIZATION:
///    - Use Vector128&lt;int&gt; or Vector256&lt;int&gt; to process multiple Fixed values simultaneously
///    - Arithmetic operations (add/sub/mul) can be vectorized for 4x-8x throughput
///    - Particularly beneficial for batch coordinate transformations and physics calculations
///    - Example: Transform 8 coordinates in parallel instead of sequential operations
///
/// 2. HARDWARE INTRINSICS:
///    - Modern CPUs have fast integer multiply-high instructions (useful for fixed-point multiply)
///    - SSE/AVX can parallelize comparisons and min/max operations across arrays
///    - Bit manipulation intrinsics (BMI1/BMI2) for efficient shifts and masks
///
/// 3. MIGRATION TO FLOATING-POINT:
///    - Modern CPUs have powerful FPUs with ~1 cycle latency for basic ops
///    - float (32-bit) provides wider range (±10^38) vs Fixed (±32768)
///    - SIMD vectorization is more mature for float operations
///    - NOTE: Would break demo compatibility due to rounding differences
///    - Consider float for new features while keeping Fixed for core game logic
///
/// 4. HYBRID APPROACH:
///    - Keep Fixed for deterministic game logic (physics, collision, demos)
///    - Use float for rendering calculations (interpolation, perspective, lighting)
///    - Convert at the boundary between game logic and rendering
///    - Best of both worlds: determinism where needed, performance where possible
///
/// 5. STRUCT LAYOUT OPTIMIZATION:
///    - Current size: 4 bytes (optimal for cache lines)
///    - Consider array-of-structs (AoS) vs struct-of-arrays (SoA) for bulk operations
///    - SoA layout: Store all X coords together, all Y coords together (better SIMD)
///
/// 6. JIT OPTIMIZATION:
///    - AggressiveInlining already applied (good!)
///    - Consider AggressiveOptimization for hot methods
///    - Profile-guided optimization (PGO) can improve branch prediction
///
/// COMPATIBILITY CONSIDERATIONS:
/// ------------------------------
/// Any optimization must maintain:
/// - Bit-exact determinism for demo playback
/// - Cross-platform reproducibility for multiplayer
/// - Numerical stability for edge cases (overflow handling)
///
/// RECOMMENDATION:
/// ---------------
/// For this modern C# implementation:
/// - Keep Fixed struct for core game logic (maintain compatibility)
/// - Add SIMD-optimized batch transformation methods for arrays of Fixed values
/// - Use float in rendering pipeline (SilkVideo, Renderer classes)
/// - Profile before optimizing - measure actual bottlenecks in real gameplay
/// </summary>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly record struct Fixed(int Data)
{
    /// <summary>Number of bits used for the fractional part (16 bits = precision of 1/65536).</summary>
    public const int FracBits = 16;

    /// <summary>
    /// The value of 1.0 in fixed-point representation (65536 = 2^16).
    /// This is the scaling factor used to convert between integer and fixed-point values.
    /// </summary>
    public const int FracUnit = 1 << FracBits;

    public static Fixed Zero => new(0);
    public static Fixed One => new(FracUnit);

    public static Fixed IntTwo => FromInt(2);

    public static Fixed MaxValue => new(int.MaxValue);
    public static Fixed MinValue => new(int.MinValue);

    public static Fixed Epsilon => new(1);
    public static Fixed OnePlusEpsilon => new(FracUnit + 1);
    public static Fixed OneMinusEpsilon => new(FracUnit - 1);

    public static Fixed OneHundred => FromInt(100);
    public static Fixed OneHundredNegative => FromInt(-100);

    /// <summary>
    /// Converts an integer to fixed-point by shifting left 16 bits.
    /// This moves the value into the integer portion of the 16.16 format.
    /// Example: 5 << 16 = 327680 (represents 5.0 in fixed-point)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed FromInt(int value)
    {
        return new Fixed(value << FracBits);
    }

    /// <summary>
    /// Converts a float to fixed-point by multiplying by 65536 (FracUnit).
    /// This scales the fractional value into the 16.16 integer representation.
    /// Example: 5.25 * 65536 = 344064
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed FromFloat(float value)
    {
        return new Fixed((int)(FracUnit * value));
    }

    /// <summary>
    /// Converts a double to fixed-point by multiplying by 65536 (FracUnit).
    /// See FromFloat for details on the conversion process.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed FromDouble(double value)
    {
        return new Fixed((int)(FracUnit * value));
    }

    /// <summary>
    /// Converts fixed-point to float by dividing the raw data by 65536.
    /// This reverses the scaling applied during conversion from float.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ToFloat()
    {
        return (float)Data / FracUnit;
    }

    /// <summary>
    /// Converts fixed-point to double by dividing the raw data by 65536.
    /// This reverses the scaling applied during conversion from double.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ToDouble()
    {
        return (double)Data / FracUnit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Abs(Fixed a)
    {
        return a.Data < 0 ? new Fixed(-a.Data) : a;
    }

    /// <summary>
    /// Unary plus operator. Returns the value unchanged.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator +(Fixed a)
    {
        return a;
    }

    /// <summary>
    /// Unary minus operator. Negates the value by negating the underlying integer.
    /// Works directly because the fixed-point format maintains sign in the MSB.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator -(Fixed a)
    {
        return new Fixed(-a.Data);
    }

    /// <summary>
    /// Addition works directly on the raw data because both values are scaled by the same factor (2^16).
    /// Example: 5.25 + 3.75 = (344064 + 245760) / 65536 = 589824 / 65536 = 9.0
    /// No adjustment needed - just add the integers!
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator +(Fixed a, Fixed b)
    {
        return new Fixed(a.Data + b.Data);
    }

    /// <summary>
    /// Subtraction works directly on the raw data for the same reason as addition.
    /// Both operands are scaled identically, so direct subtraction maintains the scale.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator -(Fixed a, Fixed b)
    {
        return new Fixed(a.Data - b.Data);
    }

    /// <summary>
    /// Multiplication requires special handling:
    /// 1. Multiply the two integers: (a * 2^16) * (b * 2^16) = result * 2^32
    /// 2. The result is scaled by 2^32, but we want 2^16
    /// 3. Shift right by 16 bits to remove the extra scaling: (result * 2^32) >> 16 = result * 2^16
    ///
    /// Uses long to prevent overflow during multiplication before the shift.
    /// Example: 2.5 * 4.0 = (163840 * 262144) / 65536 = 655360 = 10.0 in fixed-point
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator *(Fixed a, Fixed b)
    {
        return new Fixed((int)((a.Data * (long)b.Data) >> FracBits));
    }

    /// <summary>
    /// Multiplying an int by a fixed-point value is simpler:
    /// (int) * (value * 2^16) = result * 2^16
    /// The result is already correctly scaled, no shift needed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator *(int a, Fixed b)
    {
        return new Fixed(a * b.Data);
    }

    /// <summary>
    /// Multiplying a fixed-point value by an int is commutative with the above.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator *(Fixed a, int b)
    {
        return new Fixed(a.Data * b);
    }

    /// <summary>
    /// Division is the most complex operation in fixed-point arithmetic.
    /// First checks for potential overflow (if numerator >> 14 >= denominator, result would exceed 32 bits).
    /// Returns saturated min/max values on overflow to prevent crashes.
    /// Otherwise delegates to FixedDiv2 for safe division.
    /// </summary>
    public static Fixed operator /(Fixed a, Fixed b)
    {
        return CIntAbs(a.Data) >> 14 >= CIntAbs(b.Data)
            ? new Fixed((a.Data ^ b.Data) < 0 ? int.MinValue : int.MaxValue)
            : FixedDiv2(a, b);
    }

    /// <summary>
    /// Custom absolute value implementation that doesn't throw on int.MinValue (-2147483648).
    /// Math.Abs throws OverflowException for int.MinValue because its absolute value (2147483648)
    /// exceeds int.MaxValue (2147483647). This would crash visibility checks in some maps.
    ///
    /// Uses a branchless lookup table approach: creates [n, -n] and indexes with sign bit.
    /// Identical behavior to C's abs() function which simply returns the overflow value.
    /// </summary>
    private static int CIntAbs(int n)
    {
        Span<int> table = [n, -n];
        return table[(n < 0).AsByte()];
    }

    /// <summary>
    /// Performs fixed-point division with proper scaling:
    /// 1. Divide raw values: a.Data / b.Data (loses the fixed-point scale)
    /// 2. Multiply by FracUnit to restore 16.16 format
    /// 3. Check for 32-bit overflow and throw if exceeded
    ///
    /// Example: 10.0 / 2.0 = (655360 / 131072) * 65536 = 5.0 * 65536 = 327680 (5.0 in fixed-point)
    ///
    /// Uses double precision for intermediate calculation to detect overflow accurately.
    /// </summary>
    private static Fixed FixedDiv2(Fixed a, Fixed b)
    {
        var c = a.Data / (double)b.Data * FracUnit;

        if (c is >= 2147483648.0 or < -2147483648.0)
            throw new DivideByZeroException();

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

    /// <summary>
    /// Left shift multiplies the value by 2^b.
    /// Works on raw data: (value * 2^16) << b = value * 2^(16+b)
    /// Used for fast power-of-2 multiplication in original DOOM code.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator <<(Fixed a, int b)
    {
        return new Fixed(a.Data << b);
    }

    /// <summary>
    /// Right shift divides the value by 2^b.
    /// Works on raw data: (value * 2^16) >> b = value * 2^(16-b)
    /// Used for fast power-of-2 division in original DOOM code.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed operator >> (Fixed a, int b)
    {
        return new Fixed(a.Data >> b);
    }

    /// <summary>
    /// Comparison operators work directly on raw data because both operands use the same scale (2^16).
    /// If a.Data < b.Data, then a < b regardless of the fixed-point representation.
    /// This is a major advantage of fixed-point: comparisons are simple integer operations.
    /// </summary>
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
        return a < b ? a : b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fixed Max(Fixed a, Fixed b)
    {
        return a < b ? b : a;
    }

    /// <summary>
    /// Converts fixed-point to integer by shifting right 16 bits (floor operation).
    /// This extracts only the integer portion and discards the fractional part.
    /// Example: 5.75 (376832) >> 16 = 5
    /// Negative values round toward negative infinity: -5.25 becomes -6
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToIntFloor()
    {
        return Data >> FracBits;
    }

    /// <summary>
    /// Converts fixed-point to integer with ceiling operation.
    /// Adds (FracUnit - 1) before shifting to round up any fractional part.
    /// Example: 5.25 becomes 6, 5.0 stays 5
    /// This is equivalent to: if (fraction > 0) integer + 1 else integer
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToIntCeiling()
    {
        return (Data + FracUnit - 1) >> FracBits;
    }

    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    public override string ToString()
    {
        return ((double)Data / FracUnit).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}