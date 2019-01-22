using System.Windows;
using SFML.Graphics;
using SFML.System;

namespace BouncingBalls
{
    public class Wall
    {
        public Vector Position { get; set; }
        public Vector Size { get; set; }

        public Wall(Vector position, Vector size)
        {
            Position = position;
            Size = size;
        }
        
        public void Draw(RenderWindow window)
        {
            RectangleShape r = new RectangleShape(new Vector2f((float)Size.X, (float)Size.Y));
            r.Position = new Vector2f((float) Position.X, -(float) Position.Y);
            r.FillColor = new Color(20, 20, 20);
            window.Draw(r);
        }
    }
}