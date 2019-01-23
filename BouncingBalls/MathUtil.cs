using System;
using System.Security.Cryptography;
using System.Windows;

namespace BouncingBalls
{
    public static class MathUtil
    {
        public static double Lerp(double a, double b, double t)
        {
            return (1 - t) * a + t * b;
        }

        public static double InvLerp(double a, double b, double c)
        {
            return (c - a) / (b - a);
        }
        
        public static Vector VectorFromAngle(double angle)
        {
            return new Vector(Math.Cos(angle), Math.Sin(angle));
        }
    }
}