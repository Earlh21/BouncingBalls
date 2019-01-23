using System.Windows;

namespace BouncingBalls
{
    public struct CollisionData
    {
        private double angle;
        private Vector displacement;
        
        public double Angle
        {
            get { return angle; }
        }
        
        public Vector Displacement
        {
            get { return displacement; }
        }

        public CollisionData(Vector displacement, double angle)
        {
            this.displacement = displacement;
            this.angle = angle;
        }
    }
}