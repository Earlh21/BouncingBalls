using System;
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
        /// Checks for collision with a wall and applies appropriate collision physics
        /// </summary>
        /// <param name="wall">Wall to check</param>
        public void Collide(Wall wall)
        {
            
        }

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
            
            ApplyForce(Velocity * -1 * force, time);
        }

        public void ApplyForce(Vector force, double time)
        {
            Momentum += force * time;
        }
        

        public void Update(double time, bool air)
        {
            Position += Momentum / Mass * time;
            Rotation += AngularMomentum / Mass * time;

            if (air)
            {
                ApplyDrag(time);
            }
        }
    }
}