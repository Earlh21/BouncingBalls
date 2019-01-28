using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection.Emit;
using SFML.System;
using System.Windows;
using SFML.Graphics;

namespace BouncingBalls
{
    /// <summary>
    /// Represents a 2D, physics-enabled ball
    /// </summary>
    public class Ball
    {
        //TODO: Make some of these constants instance values instead
        
        //Air values
        public static double air_density = 1.2;
        public static double dynamic_viscosity = 1.8e-5;

        public static double cor = 0.8;
        
        public const double drag_coefficient = 0.47;

        public const double spring_stiffness = 10;
        
        private double radius;
        private double radius_squared;
        
        public double Radius
        {
            get { return radius;}
            set
            {
                radius = value;
                radius_squared = Math.Pow(radius, 2);
            }
        }
        
        public double Friction { get; set; }
        
        public double RadiusSquared
        {
            get { return radius_squared; }
        }

        public double Diameter
        {
            get { return radius * 2; }
        }

        public double Area
        {
            get { return Math.PI * radius_squared; }
        }

        public double Speed
        {
            get { return (Momentum / Mass).Length; }
        }

        public double SpeedSquared
        {
            get { return (Momentum / Mass).LengthSquared; }
        }

        public double ReynoldsNumber
        {
            get { return air_density * Diameter * Speed / dynamic_viscosity; }
        }
        
        public double Mass { get; set; }
        public Vector Momentum { get; set; }
        public Vector Velocity
        {
            get { return Momentum / Mass; }
        }
        public double AngularMomentum { get; set; }
        
        public double AngularVelocity
        {
            get { return AngularMomentum / Mass; }
        }
        public Vector Position { get; set; }
        public double Rotation { get; set; }

        public double Distance(Vector point)
        {
            return (Position - point).Length;
        }

        
        public double DistanceSquared(Vector point)
        {
            return (Position - point).LengthSquared;
        }

        public Vector GravityForce {get { return new Vector(0, -9.81) * Mass;}}
        
        private void Collide(CollisionData collision_data)
        {
            Position += collision_data.Displacement;

            Vector normal_momentum = Momentum.Project(collision_data.Angle + Math.PI);
            Vector tangent_momentum = Momentum.Project(collision_data.Angle + Math.PI / 2 * Math.Sign(Momentum.Angle() - collision_data.Angle));

            Momentum = normal_momentum * -cor + tangent_momentum;
        }

        private void Collide(Polygon polygon)
        {
            CollisionData col = CheckCollide(polygon, radius);
            if (col != null)
            {
                Collide((CollisionData)col);
            }
        }

        private void Collide(Ball ball)
        {
            CollisionData col = CheckCollide(ball, radius);
            if (col != null)
            {
                ResolveCollision(this, ball);
            }
        }
        
        private CollisionData CheckCollide(Ball ball, double radius)
        {
            if (Math.Pow(Radius + ball.Radius, 2) > DistanceSquared(ball.Position))
            {
                return new CollisionData(Position - ball.Position, Position.AngleTo(ball.Position));
            }

            return null;
        }
        
        private CollisionData CheckCollide(Polygon polygon, double radius)
        {
            foreach (Vector p in polygon.Points)
            {
                CollisionData col = CheckCollide(p, radius);
                if (col != null)
                {
                    return col;
                }
            }

            foreach (Line l in polygon.Lines)
            {
                CollisionData col = CheckCollide(l, radius);
                
                
                if (col != null)
                {
                    return col;
                }
            }
            
            return null;
        }

        private CollisionData CheckCollide(Vector point, double radius)
        {
            double distance2 = DistanceSquared(point);
            if (distance2 < radius_squared)
            {
                double distance = Distance(point);
                double angle = Position.AngleTo(point);
                return new CollisionData((Position - point).Unit() * (radius - distance), angle);
            }

            return null;
        }

        private CollisionData CheckCollide(Line line, double radius)
        {
            Vector closest_point = line.GetClosestPoint(Position);
            return CheckCollide(closest_point, radius);
        }
        
        /// <summary>
        /// Applies a force for a specified amount of time
        /// </summary>
        /// <param name="force">Amount of force to apply</param>
        /// <param name="time">Amount of time to apply force</param>
        public void ApplyForce(Vector force, double time)
        {
            Momentum += force * time;
        }

        public void Slow(double force, double time)
        {
            int sign_x = Math.Sign(Momentum.X);
            int sign_y = Math.Sign(Momentum.Y);
            ApplyForce(-Momentum.Unit() * force, time);

            double new_mom_x = Momentum.X;
            double new_mom_y = Momentum.Y;
            
            if(Math.Sign(Momentum.X) != sign_x)
            {
                new_mom_x = 0;
            }

            if (Math.Sign(Momentum.Y) != sign_y)
            {
                new_mom_y = 0;
            }
            
            Momentum = new Vector(new_mom_x, new_mom_y);
        }

        private Vector NormalForce(double angle)
        {
            return GravityForce.Project(angle);
        }
        
        private void DoFriction(List<Polygon> polygons, double time)
        {
            CollisionData col = null;

            double friction = 0;
            
            foreach (Polygon polygon in polygons)
            {
                col = CheckCollide(polygon, radius * 1.2);
                if (col != null)
                {
                    friction = polygon.Friction;
                    break;
                }
            }
            
            if (col == null)
            {
                return;
            }
            
            Slow(NormalForce(col.Angle + Math.PI).Length * friction, time);
        }

        private void DoGravity(List<Polygon> polygons, double time)
        {
            CollisionData col = null;
            
            foreach (Polygon polygon in polygons)
            {
                foreach (Line l in polygon.Lines)
                {
                    col = CheckCollide(l, radius * 1.1);
                    if (col != null)
                    {
                        break;
                    }
                }
            }

            if (col != null)
            {
                CollisionData c = col;

                double angle = c.Angle;
                while (angle < 0)
                {
                    angle += 2 * Math.PI;
                }
            
                if (angle > Math.PI / 2 && angle < Math.PI)
                {
                    Vector direction = MathUtil.VectorFromAngle(angle + Math.PI / 2);
                    
                    ApplyForce(GravityForce.Project(direction), time);
                    return;
                }
                else if(angle < Math.PI)
                {
                    Vector direction = MathUtil.VectorFromAngle(angle - Math.PI / 2);
                    
                    ApplyForce(GravityForce.Project(direction), time);
                    return;
                }
            }

            ApplyForce(new Vector(0, -9.81) * Mass, time);
        }

        private static Vector GetNewV(Ball a, Ball b)
        {
            double first = 2 * b.Mass / (a.Mass + b.Mass);
            double second = (a.Velocity - b.Velocity) * (a.Position - b.Position) / a.DistanceSquared(b.Position);
            Vector third = a.Position - b.Position;
            return cor * first * second * third;
        }
        
        public static void ResolveCollision(Ball a, Ball b)
        {
            Vector displace = b.Position - a.Position;
            Vector displace_unit = displace.Unit();
            double overlap = a.Distance(b.Position) - (a.radius + b.radius);

            double ratio = a.Mass / (a.Mass + b.Mass);
            a.Position += overlap * displace_unit * (1 - ratio);
            b.Position -= overlap * displace_unit * (ratio);

            Vector new_v_a = a.Velocity - GetNewV(a, b);
            Vector new_v_b = b.Velocity - GetNewV(b, a);

            a.Momentum = new_v_a * a.Mass;
            b.Momentum = new_v_b * b.Mass;
        }
        
        /// <summary>
        /// Moves the Body with a specified number of sub-steps, checking for collisions along the way
        /// </summary>
        /// <param name="time">Amount of time to move</param>
        /// <param name="air">Whether to calculate air resistance or not</param>
        /// <param name="precision">Amount of sub-steps to calculate</param>
        public void UpdateCollide(double time, bool air, int precision, List<Polygon> polygons, List<Ball> balls)
        {
            DoFriction(polygons, time);
            DoGravity(polygons, time);
            
            for (int i = 0; i < precision; i++)
            {
                foreach (Polygon polygon in polygons)
                {
                    Collide(polygon);
                }

                foreach (Ball ball in balls)
                {
                    if (ball != this)
                    {
                        Collide(ball);
                    }
                }
                
                Position += Momentum / Mass * time / precision;
                Rotation += AngularMomentum / Mass * time / precision;
            }
        }

        public void Draw(RenderWindow window)
        {
            CircleShape shape = new CircleShape((float)radius);
            shape.Position = new Vector2f((float)(Position.X - radius), -(float)(Position.Y + radius));
            shape.FillColor = Color.Green;
            window.Draw(shape);
        }

        public Ball(Vector position, double mass, double radius, Vector momentum, double friction)
        {
            Position = position;
            Mass = mass;
            Momentum = momentum;
            Radius = radius;
            Friction = friction;
        }
    }
}