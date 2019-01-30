using System.Windows;

namespace BouncingBalls
{
    public class Rectangle
    {
        private Vector bottom_left;
        private Vector top_right;

        public Vector Center
        {
            get { return (bottom_left + top_right) / 2; }
        }

        public double Width
        {
            get { return top_right.X - bottom_left.X; }
        }

        public double Height
        {
            get { return top_right.Y - bottom_left.Y; }
        }

        public Vector Size
        {
            get {return new Vector(Width, Height);}
        }
        
        public Rectangle(Vector bottom_left, Vector top_right)
        {
            this.bottom_left = bottom_left;
            this.top_right = top_right;
        }
    }
}