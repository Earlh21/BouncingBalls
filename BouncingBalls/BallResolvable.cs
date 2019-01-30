using System;
using System.Security.Policy;
using System.Windows;

namespace BouncingBalls
{
    public class BallPair : IResolvable
    {
        private Ball a;
        private Ball b;

        public Ball A
        {
            get { return a; }
        }

        public Ball B
        {
            get { return b; }
        }

        public BallPair(Ball a, Ball b)
        {
            this.a = a;
            this.b = b;
        }
        
        private Vector GetNewV(Ball a, Ball b)
        {
            double first = 2 * b.Mass / (a.Mass + b.Mass);
            double second = (a.Velocity - b.Velocity) * (a.Position - b.Position) / a.DistanceSquared(b.Position);
            Vector third = a.Position - b.Position;
            return Ball.cor * first * second * third;
        }
        
        public void ResolveCollision()
        {
            lock(a)
            {
                lock (b)
                {
                    Vector displace = b.Position - a.Position;
                    Vector displace_unit = displace.Unit();
                    double overlap = a.Distance(b.Position) - (a.Radius + b.Radius);

                    double ratio = a.Mass / (a.Mass + b.Mass);
                    a.Position += overlap * displace_unit * (1 - ratio);
                    b.Position -= overlap * displace_unit * (ratio);

                    Vector new_v_a = a.Velocity - GetNewV(a, b);
                    Vector new_v_b = b.Velocity - GetNewV(b, a);

                    a.Momentum = new_v_a * a.Mass;
                    b.Momentum = new_v_b * b.Mass;
                }
            }
        }
    }
}