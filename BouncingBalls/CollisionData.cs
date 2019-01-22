namespace BouncingBalls
{
    public struct CollisionData
    {
        public enum CollideType
        {
            Left,
            Top,
            Bottom,
            Right,
            None
        };

        private CollideType collide_type;
        private double displacement;
        
        public CollideType CollisionType
        {
            get { return collide_type; }
        }
        public double Displacement
        {
            get { return displacement; }
        }

        public CollisionData(double displacement, CollideType collide_type)
        {
            this.displacement = displacement;
            this.collide_type = collide_type;
        }
    }
}