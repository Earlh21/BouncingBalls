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
                    lines.Add(GetLine(i));
                }
            }
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

        private Line GetLine(int index)
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