using System;
using System.Windows;
using SFML.System;

namespace BouncingBalls
{
    public static class Extension
    {
        public static Vector Unit(this Vector v)
        {
            return v / v.Length;
        }

        public static double Angle(this Vector v)
        {
            return Math.Atan2(v.Y, v.X);
        }

        public static Vector2f Vector2f(this Vector v)
        {
            return new Vector2f((float)v.X, (float)v.Y);
        }

        public static Vector2f Vector2fInvY(this Vector v)
        {
            return new Vector2f((float)v.X, -(float)v.Y);
        }

        public static Vector Project(this Vector v, Vector onto)
        {
            return ((v * onto) / onto.Length) * (onto / onto.Length);
        }

        public static Vector Project(this Vector v, double angle)
        {
            return v.Project(MathUtil.VectorFromAngle(angle));
        }
    }
}