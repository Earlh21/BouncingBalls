using System;
using System.Collections.Generic;
using System.Windows;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace BouncingBalls
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            List<Polygon> polygons = new List<Polygon>();
            List<Ball> balls = new List<Ball>();

            List<Vector> points = new List<Vector>();
            points.Add(new Vector(-400, 400));
            points.Add(new Vector(-400, -(1.5 * 5 / 2)));
            points.Add(new Vector(0, -(1.5 * 5 / 2)));
            polygons.Add(new Polygon(0.1, points));
            
            List<Vector> points2 = new List<Vector>();
            points2.Add(new Vector(400, 400));
            points2.Add(new Vector(400, -1.5 * 5 / 2));
            points2.Add(new Vector(0, -1.5 * 5 / 2));
            polygons.Add(new Polygon(0.1, points2));

            Random R = new Random();
            
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector position = new Vector(-4 + i * 0.2, 1.5 * 5 / 2 - 0.8 + j * 0.2);
                    RandomBall(balls, position, R);
                }
            }
            
            
            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            Clock C = new Clock();
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "SFML window with OpenGL", Styles.Default,
                contextSettings);
            window.SetView(Camera.view);
            
            Camera.SetView(new View(new Vector2f(0,0), new Vector2f(2, 1.5f) * 5), window);

            double max_time = 1.0 / 60.0;
            C.Restart();
            while (window.IsOpen)
            {
                window.DispatchEvents();

                //Physics logic
                double time = C.ElapsedTime.AsSeconds();
                if (time > max_time)
                {
                    time = max_time;
                }
                C.Restart();
                UpdatePhysics(balls, polygons, time);

                //Drawing
                Draw(window, balls, polygons);
            }
        }

        public static void RandomBall(List<Ball> balls, Vector position, Random R)
        {
            double range = 1;
            double t = (R.NextDouble() - 0.5) * 2;
            double t2 = (R.NextDouble() - 0.5) * 2;
            Vector momentum = new Vector(t, t2) * range;
            
            balls.Add(new Ball(position, 0.058, 0.1, momentum, 0.1));
        }
            
        public static void UpdatePhysics(List<Ball> bodies, List<Polygon> polygons, double time)
        {
            foreach (Ball b in bodies)
            {
                b.UpdateCollide(time, true, 4, polygons, bodies);
            }
        }

        public static void Draw(RenderWindow window, List<Ball> bodies, List<Polygon> polygons)
        {
            byte brightness = 230;
            window.Clear(new Color(brightness, brightness, brightness));
            
            foreach (Ball b in bodies)
            {
                b.Draw(window);
            }

            foreach (Polygon p in polygons)
            {
                p.DrawOutline(window);
            }

            window.Display();
        }
    }
}