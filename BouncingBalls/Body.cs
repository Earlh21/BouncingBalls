using System;
using System.Collections.Generic;
using SFML.System;
using System.Windows;

namespace BouncingBalls
{
    /// <summary>
    /// Represents a 2D, physics-enabled ball
    /// </summary>
    public class Body
    {
        
        //Air values
        public static double air_density = 1.2;
        public static double dynamic_viscosity = 1.8e-5;

        public static double cor = 0.8;
        public static double friction = 0.01;
        
        public const double drag_coefficient = 0.47;
        
        private double radius;
        private double radius2;
        
        public double Radius
        {
            get { return radius;}
            set
            {
                radius = value;
                radius2 = (double)Math.Pow(radius, 2);
            }
        }
        
        public double Radius2
        {
            get { return radius2; }
        }

        public double Diameter
        {
            get { return radius * 2; }
        }

        public double Area
        {
            get { return (double)Math.PI * radius2; }
        }

        public double Speed
        {
            get { return (Momentum / Mass).Length; }
        }

        public double Speed2
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

        /// <summary>
        /// Returns the distance to another Body
        /// </summary>
        /// <param name="other">Body to calculate distance to</param>
        /// <returns>Distance</returns>
        public double Distance(Body other)
        {
            return (Position - other.Position).Length;
        }

        
        /// <summary>
        /// Returns the distance squared to another Body
        /// </summary>
        /// <param name="other">Body to calculate distance to</param>
        /// <returns>Distance squared</returns>
        public double Distance2(Body other)
        {
            return (Position - other.Position).LengthSquared;
        }
        
        /// <summary>
        /// Checks for collision with another Body and applies appropriate collision physics
        /// </summary>
        /// <param name="other">Body to check</param>
        public void Collide(Body other)
        {
            if (radius2 + other.radius2 < Distance2(other))
            {
                
            }
        }

        /// <summary>
        /// Checks for collision with a Wall and applies appropriate collision physics if a collision is found
        /// </summary>
        /// <param name="wall">Wall to check</param>
        public void Collide(Wall wall)
        {
            CollisionData collide_data = CheckCollide(wall);
            if (collide_data.CollisionType == CollisionData.CollideType.None)
            {
                return;
            }

            if (collide_data.CollisionType == CollisionData.CollideType.Top || collide_data.CollisionType == CollisionData.CollideType.Bottom)
            {
                Momentum = new Vector(Momentum.X * (1 - friction), Momentum.Y * -cor);
                Position += new Vector(0, collide_data.Displacement);
            }

            if (collide_data.CollisionType == CollisionData.CollideType.Left || collide_data.CollisionType == CollisionData.CollideType.Right)
            {
                Momentum = new Vector(Momentum.X * -cor, Momentum.Y * (1 - friction));
                Position += new Vector(collide_data.Displacement, 0);
            }
        }

        /// <summary>
        /// Checks for collision with a Wall
        /// </summary>
        /// <param name="wall">Wall to check</param>
        /// <returns>Whether and where the Body is colliding with the Wall or not</returns>
        private CollisionData CheckCollide(Wall wall)
        {
            double bottom_point = Position.Y - radius;
            if (bottom_point < wall.Position.Y && bottom_point > wall.Position.Y - wall.Size.Y)
            {
                return new CollisionData(wall.Position.Y - bottom_point, CollisionData.CollideType.Bottom);
            }
            
            double right_point = Position.X + radius;
            if (right_point > wall.Position.X && right_point < wall.Position.X + wall.Size.X)
            {
                return new CollisionData(wall.Position.X - right_point, CollisionData.CollideType.Right);
            }

            double left_point = Position.X - radius;
            if (left_point < wall.Position.X + wall.Size.X && left_point > wall.Position.X)
            {
                return new CollisionData(wall.Position.X + wall.Size.X - left_point, CollisionData.CollideType.Left);
            }

            double top_point = Position.Y + radius;
            if (top_point > wall.Position.Y + wall.Size.Y && top_point < wall.Position.Y)
            {
                return new CollisionData(wall.Position.Y + wall.Size.Y - top_point, CollisionData.CollideType.Top);
            }

            return new CollisionData(0, CollisionData.CollideType.None);
        }

        /// <summary>
        /// Applies drag to the ball for a specified amount of time
        /// </summary>
        /// <param name="time">Amount of time to apply drag</param>
        private void ApplyDrag(double time)
        {
            double force;
            
            if (ReynoldsNumber > 1.0f)
            {
                //Drag equation
                force = air_density * drag_coefficient * Area * Speed2;
                
            }
            else
            {
                //Stokes' law
                force = 6 * (double) Math.PI * dynamic_viscosity * radius * Speed;
            }
            
            
            ApplyForce(Velocity.Unit() * -1 * force, time);
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
        

        /// <summary>
        /// Moves the Body for a specified amount of time.
        /// </summary>
        /// <param name="time">Amount of time to move</param>
        /// <param name="air">Whether to calculate air resistance or not</param>
        public void UpdatePosition(double time, bool air)
        {
            if (air)
            {
                ApplyDrag(time);
            }
            
            Position += Momentum / Mass * time;
            Rotation += AngularMomentum / Mass * time;
        }

        /// <summary>
        /// Moves the Body with a specified number of sub-steps, checking for collisions along the way
        /// </summary>
        /// <param name="time">Amount of time to move</param>
        /// <param name="air">Whether to calculate air resistance or not</param>
        /// <param name="precision">Amount of sub-steps to calculate</param>
        public void UpdateCollide(double time, bool air, int precision, List<Wall> walls, List<Body> bodies)
        {
            if (air)
            {
                ApplyDrag(time);
            }
            
            for (int i = 0; i < precision; i++)
            {
                Position += Momentum / Mass * time / precision;
                Rotation += AngularMomentum / Mass * time / precision;

                foreach (Wall w in walls)
                {
                    Collide(w);
                }

                foreach (Body b in bodies)
                {
                    Collide(b);
                }
            }
        }
    }
}