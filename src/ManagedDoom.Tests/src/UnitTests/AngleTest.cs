using ManagedDoom.Doom.Math;

namespace ManagedDoom.Tests.UnitTests;

public sealed class AngleTest
{
    private const double Delta = 1.0E-3;

    [Fact]
    public void ToRadian()
    {
        Assert.Equal(0.00 * Math.PI, Angle.Ang0.ToRadian(), Delta);
        Assert.Equal(0.25 * Math.PI, Angle.Ang45.ToRadian(), Delta);
        Assert.Equal(0.50 * Math.PI, Angle.Ang90.ToRadian(), Delta);
        Assert.Equal(1.00 * Math.PI, Angle.Ang180.ToRadian(), Delta);
        Assert.Equal(1.50 * Math.PI, Angle.Ang270.ToRadian(), Delta);
    }

    [Fact]
    public void FromDegrees()
    {
        for (var deg = -720; deg <= 720; deg++)
        {
            var expectedSin = Math.Sin(2 * Math.PI * deg / 360);
            var expectedCos = Math.Cos(2 * Math.PI * deg / 360);

            var angle = Angle.FromDegree(deg);
            var actualSin = Math.Sin(angle.ToRadian());
            var actualCos = Math.Cos(angle.ToRadian());

            Assert.Equal(expectedSin, actualSin, Delta);
            Assert.Equal(expectedCos, actualCos, Delta);
        }
    }

    [Fact]
    public void FromRadianToDegrees()
    {
        Assert.Equal(0, Angle.FromRadian(0.00 * Math.PI).ToDegree(), Delta);
        Assert.Equal(45, Angle.FromRadian(0.25 * Math.PI).ToDegree(), Delta);
        Assert.Equal(90, Angle.FromRadian(0.50 * Math.PI).ToDegree(), Delta);
        Assert.Equal(180, Angle.FromRadian(1.00 * Math.PI).ToDegree(), Delta);
        Assert.Equal(270, Angle.FromRadian(1.50 * Math.PI).ToDegree(), Delta);
    }

    [Fact]
    public void Sign()
    {
        var random = new Random(666);
        for (var i = 0; i < 100; i++)
        {
            var a = random.Next(1440) - 720;
            var b = +a;
            var c = -a;

            var aa = Angle.FromDegree(a);
            var ab = +aa;
            var ac = -aa;

            {
                var expectedSin = Math.Sin(2 * Math.PI * b / 360);
                var expectedCos = Math.Cos(2 * Math.PI * b / 360);

                var actualSin = Math.Sin(ab.ToRadian());
                var actualCos = Math.Cos(ab.ToRadian());

                Assert.Equal(expectedSin, actualSin, Delta);
                Assert.Equal(expectedCos, actualCos, Delta);
            }

            {
                var expectedSin = Math.Sin(2 * Math.PI * c / 360);
                var expectedCos = Math.Cos(2 * Math.PI * c / 360);

                var actualSin = Math.Sin(ac.ToRadian());
                var actualCos = Math.Cos(ac.ToRadian());

                Assert.Equal(expectedSin, actualSin, Delta);
                Assert.Equal(expectedCos, actualCos, Delta);
            }
        }
    }

    [Fact]
    public void Abs()
    {
        var random = new Random(666);
        for (var i = 0; i < 100; i++)
        {
            var a = random.Next(120) - 60;
            var b = Math.Abs(a);

            var aa = Angle.FromDegree(a);
            var ab = Angle.Abs(aa);

            var expectedSin = Math.Sin(2 * Math.PI * b / 360);
            var expectedCos = Math.Cos(2 * Math.PI * b / 360);

            var actualSin = Math.Sin(ab.ToRadian());
            var actualCos = Math.Cos(ab.ToRadian());

            Assert.Equal(expectedSin, actualSin, Delta);
            Assert.Equal(expectedCos, actualCos, Delta);
        }
    }

    [Fact]
    public void Addition()
    {
        var random = new Random(666);
        for (var i = 0; i < 100; i++)
        {
            var a = random.Next(1440) - 720;
            var b = random.Next(1440) - 720;
            var c = a + b;

            var fa = Angle.FromDegree(a);
            var fb = Angle.FromDegree(b);
            var fc = fa + fb;

            var expectedSin = Math.Sin(2 * Math.PI * c / 360);
            var expectedCos = Math.Cos(2 * Math.PI * c / 360);

            var actualSin = Math.Sin(fc.ToRadian());
            var actualCos = Math.Cos(fc.ToRadian());

            Assert.Equal(expectedSin, actualSin, Delta);
            Assert.Equal(expectedCos, actualCos, Delta);
        }
    }

    [Fact]
    public void Subtraction()
    {
        var random = new Random(666);
        for (var i = 0; i < 100; i++)
        {
            var a = random.Next(1440) - 720;
            var b = random.Next(1440) - 720;
            var c = a - b;

            var fa = Angle.FromDegree(a);
            var fb = Angle.FromDegree(b);
            var fc = fa - fb;

            var expectedSin = Math.Sin(2 * Math.PI * c / 360);
            var expectedCos = Math.Cos(2 * Math.PI * c / 360);

            var actualSin = Math.Sin(fc.ToRadian());
            var actualCos = Math.Cos(fc.ToRadian());

            Assert.Equal(expectedSin, actualSin, Delta);
            Assert.Equal(expectedCos, actualCos, Delta);
        }
    }

    [Fact]
    public void Multiplication1()
    {
        var random = new Random(666);
        for (var i = 0; i < 100; i++)
        {
            var a = (uint)random.Next(30);
            var b = (uint)random.Next(12);
            var c = a * b;

            var fa = Angle.FromDegree(a);
            var fc = fa * b;

            Assert.Equal(c, fc.ToDegree(), Delta);
        }
    }

    [Fact]
    public void Multiplication2()
    {
        var random = new Random(666);
        for (var i = 0; i < 100; i++)
        {
            var a = (uint)random.Next(30);
            var b = (uint)random.Next(12);
            var c = a * b;

            var fb = Angle.FromDegree(b);
            var fc = a * fb;

            Assert.Equal(c, fc.ToDegree(), Delta);
        }
    }

    [Fact]
    public void Division()
    {
        var random = new Random(666);
        for (var i = 0; i < 100; i++)
        {
            var a = (double)random.Next(360);
            var b = (uint)(random.Next(30) + 1);
            var c = a / b;

            var fa = Angle.FromDegree(a);
            var fc = fa / b;

            Assert.Equal(c, fc.ToDegree(), Delta);
        }
    }

    [Fact]
    public void Comparison()
    {
        var random = new Random(666);
        for (var i = 0; i < 10000; i++)
        {
            var a = random.Next(1140) - 720;
            var b = random.Next(1140) - 720;

            var fa = Angle.FromDegree(a);
            var fb = Angle.FromDegree(b);

            a = ((a % 360) + 360) % 360;
            b = ((b % 360) + 360) % 360;

            Assert.True((a == b) == (fa == fb));
            Assert.True((a != b) == (fa != fb));
            Assert.True((a < b) == (fa < fb));
            Assert.True((a > b) == (fa > fb));
            Assert.True((a <= b) == (fa <= fb));
            Assert.True((a >= b) == (fa >= fb));
        }
    }
}