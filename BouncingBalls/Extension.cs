using System;
using System.Windows;
using SFML.Graphics;
using SFML.System;

namespace BouncingBalls
{
    public static class Extension
    {
        public static Vector Unit(this Vector v)
        {
            double l = v.Length;
            
            if (v.Length == 0)
            {
                return new Vector(0, 0);
            }
            return v / l;
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

        public static double AngleTo(this Vector v, Vector v2)
        {
            return Math.Atan2(v.Y - v2.Y, v.X - v2.X);
        }

        public static Vector Vector(this Vector2f v)
        {
            return new Vector(v.X, v.Y);
        }

        public static Vector Vector(this Vector2i v)
        {
            return new Vector(v.X, v.Y);
        }
    }
}