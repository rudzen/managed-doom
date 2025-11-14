using System.Runtime.CompilerServices;

namespace ManagedDoom.Doom.Math;

/// <summary>
/// Represents an angle using binary angle measurement (BAM) where a full circle is represented by 2^32 units.
/// This provides precise angle calculations without floating-point errors and efficient wrapping behavior.
/// </summary>
/// <param name="Data">The raw angle data as an unsigned 32-bit integer.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly record struct Angle(uint Data)
{
    /// <summary>Represents 0 degrees (0 radians).</summary>
    public static readonly Angle Ang0 = new(0x00000000);

    /// <summary>Represents 45 degrees (π/4 radians).</summary>
    public static readonly Angle Ang45 = new(0x20000000);

    /// <summary>Represents 90 degrees (π/2 radians).</summary>
    public static readonly Angle Ang90 = new(0x40000000);

    /// <summary>Represents 180 degrees (π radians).</summary>
    public static readonly Angle Ang180 = new(0x80000000);

    /// <summary>Represents 270 degrees (3π/2 radians).</summary>
    public static readonly Angle Ang270 = new(0xC0000000);

    /// <summary>
    /// Initializes a new instance of the <see cref="Angle"/> struct from a signed integer.
    /// </summary>
    /// <param name="data">The signed integer value to convert to an angle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle(int data) : this((uint)data)
    {
    }

    /// <summary>
    /// Creates an <see cref="Angle"/> from a radian value.
    /// </summary>
    /// <param name="radian">The angle in radians.</param>
    /// <returns>An <see cref="Angle"/> representing the specified radian value.</returns>
    public static Angle FromRadian(in double radian)
    {
        var data = System.Math.Round(0x100000000 * (radian / System.Math.Tau));
        return new Angle((uint)(long)data);
    }

    /// <summary>
    /// Creates an <see cref="Angle"/> from a degree value.
    /// </summary>
    /// <param name="degree">The angle in degrees.</param>
    /// <returns>An <see cref="Angle"/> representing the specified degree value.</returns>
    public static Angle FromDegree(in double degree)
    {
        var data = System.Math.Round(0x100000000 * (degree / 360));
        return new Angle((uint)(long)data);
    }

    /// <summary>
    /// Converts this angle to radians.
    /// </summary>
    /// <returns>The angle in radians.</returns>
    public double ToRadian()
    {
        return System.Math.Tau * ((double)Data / 0x100000000);
    }

    /// <summary>
    /// Converts this angle to degrees.
    /// </summary>
    /// <returns>The angle in degrees.</returns>
    public double ToDegree()
    {
        return 360 * ((double)Data / 0x100000000);
    }

    /// <summary>
    /// Returns the absolute value of an angle, treating it as a signed value.
    /// </summary>
    /// <param name="angle">The angle to get the absolute value of.</param>
    /// <returns>The absolute value of the angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle Abs(Angle angle)
    {
        var data = (int)angle.Data;
        return data < 0 ? -angle : angle;
    }

    /// <summary>
    /// Returns the angle unchanged (unary plus operator).
    /// </summary>
    /// <param name="a">The angle.</param>
    /// <returns>The same angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator +(Angle a)
    {
        return a;
    }

    /// <summary>
    /// Negates the angle (unary minus operator).
    /// </summary>
    /// <param name="a">The angle to negate.</param>
    /// <returns>The negated angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator -(Angle a)
    {
        return new Angle((uint)-(int)a.Data);
    }

    /// <summary>
    /// Adds two angles together with automatic wrapping.
    /// </summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns>The sum of the two angles.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator +(Angle a, Angle b)
    {
        return new Angle(a.Data + b.Data);
    }

    /// <summary>
    /// Subtracts one angle from another with automatic wrapping.
    /// </summary>
    /// <param name="a">The angle to subtract from.</param>
    /// <param name="b">The angle to subtract.</param>
    /// <returns>The difference between the two angles.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator -(Angle a, Angle b)
    {
        return new Angle(a.Data - b.Data);
    }

    /// <summary>
    /// Multiplies an angle by an unsigned integer scalar.
    /// </summary>
    /// <param name="a">The scalar multiplier.</param>
    /// <param name="b">The angle to multiply.</param>
    /// <returns>The multiplied angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator *(uint a, Angle b)
    {
        return new Angle(a * b.Data);
    }

    /// <summary>
    /// Multiplies an angle by an unsigned integer scalar.
    /// </summary>
    /// <param name="a">The angle to multiply.</param>
    /// <param name="b">The scalar multiplier.</param>
    /// <returns>The multiplied angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator *(Angle a, uint b)
    {
        return new Angle(a.Data * b);
    }

    /// <summary>
    /// Divides an angle by an unsigned integer scalar.
    /// </summary>
    /// <param name="a">The angle to divide.</param>
    /// <param name="b">The scalar divisor.</param>
    /// <returns>The divided angle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator /(Angle a, uint b)
    {
        return new Angle(a.Data / b);
    }

    /// <summary>
    /// Determines whether one angle is less than another.
    /// </summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns>true if a is less than b; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Angle a, Angle b)
    {
        return a.Data < b.Data;
    }

    /// <summary>
    /// Determines whether one angle is greater than another.
    /// </summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns>true if a is greater than b; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Angle a, Angle b)
    {
        return a.Data > b.Data;
    }

    /// <summary>
    /// Determines whether one angle is less than or equal to another.
    /// </summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns>true if a is less than or equal to b; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Angle a, Angle b)
    {
        return a.Data <= b.Data;
    }

    /// <summary>
    /// Determines whether one angle is greater than or equal to another.
    /// </summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <returns>true if a is greater than or equal to b; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Angle a, Angle b)
    {
        return a.Data >= b.Data;
    }

    /// <summary>
    /// Returns the hash code for this angle.
    /// </summary>
    /// <returns>A hash code for the current angle.</returns>
    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    /// <summary>
    /// Converts the angle to its string representation in degrees.
    /// </summary>
    /// <returns>A string that represents the angle in degrees.</returns>
    public override string ToString()
    {
        return ToDegree().ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
