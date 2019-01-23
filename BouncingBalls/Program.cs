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
            List<Ball> bodies = new List<Ball>();

            bodies.Add(new Ball(new Vector(-1, 3), 0.058, 0.07, new Vector(0, -1)));

            List<Vector> points = new List<Vector>();
            points.Add(new Vector(-400, 300));
            points.Add(new Vector(-400, -3));
            points.Add(new Vector(0, -3));
            polygons.Add(new Polygon(points));
            
            List<Vector> points2 = new List<Vector>();
            points.Add(new Vector(400, 300));
            points.Add(new Vector(400, -3));
            points.Add(new Vector(0, -3));
            polygons.Add(new Polygon(points));

            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            Clock C = new Clock();
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "SFML window with OpenGL", Styles.Default,
                contextSettings);
            window.SetView(Camera.view);
            
            Camera.SetView(new View(new Vector2f(0,0), new Vector2f(8, 6f)), window);

            C.Restart();
            while (window.IsOpen)
            {
                window.DispatchEvents();

                //Physics logic
                double time = C.ElapsedTime.AsSeconds();
                C.Restart();
                UpdatePhysics(bodies, polygons, time);

                //Drawing
                Draw(window, bodies, polygons);
            }
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