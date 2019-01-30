using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using SFML.Graphics;
using SFML.System;

namespace BouncingBalls
{
    public class Polygon
    {
        private List<Vector> points;
        
        private List<Line> lines;

        private Rect AABB;
        
        public List<Line> Lines
        {
            get { return lines; }
        }

        public double Friction { get; set; }

        public List<Vector> Points
        {
            get { return points;}
            set
            {
                points = value;
                lines = new List<Line>();
                for (int i = 0, w = points.Count; i < w; i++)
                {
                    lines.Add(GenerateLine(i));
                }
            }
        }

        private Rectangle GenerateAABB()
        {
            Vector top_right = new Vector(Double.MinValue, Double.MinValue);
            Vector bottom_left = new Vector(Double.MaxValue, Double.MaxValue);

            foreach (Point p in points)
            {
                if (p.X > top_right.X)
                {
                    top_right.X = p.X;
                }

                if (p.Y > top_right.Y)
                {
                    top_right.Y = p.Y;
                }

                if (p.X < bottom_left.X)
                {
                    bottom_left.X = p.X;
                }

                if (p.Y < bottom_left.Y)
                {
                    bottom_left.Y = p.Y;
                }
            }
            
            return new Rectangle(bottom_left, top_right);
        }

        public Polygon(double friction)
        {
            points = new List<Vector>();
            Friction = friction;
        }
        
        public Polygon(double friction, List<Vector> points)
        {
            Points = points;
            Friction = friction;
        }

        private Line GenerateLine(int index)
        {
            if (index > Points.Count - 1 || index < 0)
            {
                throw new ArgumentException("Line index out of bounds");
            }

            if (index == Points.Count - 1)
            {
                return new Line(points[points.Count - 1], points[0]);
            }
            
            return new Line(Points[index], Points[index + 1]);
        }

        public void DrawOutline(RenderWindow window)
        {
            Vertex[] vertices = new Vertex[Lines.Count * 2];

            int i = 0;
            foreach (Line line in Lines)
            {
                vertices[i].Position = line.Start.Vector2fInvY();
                vertices[i].Color = Color.Black;
                vertices[i + 1].Position = line.End.Vector2fInvY();
                vertices[i + 1].Color = Color.Black;

                i += 2;
            }
            
            window.Draw(vertices, PrimitiveType.Lines);
        }
    }
}