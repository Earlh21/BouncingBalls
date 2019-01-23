using System;
using System.Windows;

namespace BouncingBalls
{
    public class Line
    {
        public Vector Start { get; set; }
        public Vector End { get; set; }
        
        public double Angle
        {
            get { return Math.Atan2(Start.Y - End.Y, Start.X - End.X); }
        }
        
        public double Length
        {
            get { return (Start - End).Length; }
        }
        
        public double LengthSquared
        {
            get { return (Start - End).LengthSquared; }
        }

        public Line(Vector start, Vector end)
        {
            Start = start;
            End = end;
        }

        public Vector GetClosestPoint(Vector position)
        {
            Vector start_to_position = position - Start;
            Vector start_to_end = End - Start;

            double dot = start_to_position * start_to_end;
            double distance = dot / LengthSquared;

            if (distance < 0)
            {
                return Start;
            }
            else if (distance > 1)
            {
                return End;
            }
            else
            {
                return Start + start_to_end * distance;
            }
        }
    }
}