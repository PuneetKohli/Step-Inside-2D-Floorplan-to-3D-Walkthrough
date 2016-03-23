using UnityEngine;
using System.Collections;

public class Vector
{
    public double X;
    public double Y;

    // Constructors.
    public Vector(double x, double y) { X = x; Y = y; }
    public Vector() : this(double.NaN, double.NaN) { }

    public static Vector operator -(Vector v, Vector w)
    {
        return new Vector(v.X - w.X, v.Y - w.Y);
    }

    public static Vector operator +(Vector v, Vector w)
    {
        return new Vector(v.X + w.X, v.Y + w.Y);
    }

    public static double operator *(Vector v, Vector w)
    {
        return v.X * w.X + v.Y * w.Y;
    }

    public static Vector operator *(Vector v, double mult)
    {
        return new Vector(v.X * mult, v.Y * mult);
    }

    public static Vector operator *(double mult, Vector v)
    {
        return new Vector(v.X * mult, v.Y * mult);
    }

    public double Cross(Vector v)
    {
        return X * v.Y - Y * v.X;
    }

    public override bool Equals(object obj)
    {
        var v = (Vector)obj;
        return (X - v.X).IsZero() && (Y - v.Y).IsZero();
    }
}

public static class Extensions
{
    private const double Epsilon = 1e-10;

    public static bool IsZero(this double d)
    {
        return Mathf.Abs((float)d) < Epsilon;
    }
}