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
    }
}