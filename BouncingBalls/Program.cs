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
            List<Wall> walls = new List<Wall>();
            List<Body> bodies = new List<Body>();

            bodies.Add(new Body(new Vector(0, 0), 0.058, 0.07, new Vector(0.02, 0)));
            walls.Add(new Wall(new Vector(-2, -1.3), new Vector(4, 0.2)));

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
                UpdatePhysics(bodies, walls, time);

                //Drawing
                Draw(window, bodies, walls);
            }
        }

        public static void UpdatePhysics(List<Body> bodies, List<Wall> walls, double time)
        {
            foreach (Body b in bodies)
            {
                //Gravity
                b.ApplyForce(new Vector(0, -9.81) * b.Mass, time);

                b.UpdateCollide(time, true, 4, walls, bodies);
            }
        }

        public static void Draw(RenderWindow window, List<Body> bodies, List<Wall> walls)
        {
            byte brightness = 230;
            window.Clear(new Color(brightness, brightness, brightness));
            
            foreach (Body b in bodies)
            {
                b.Draw(window);
            }

            foreach (Wall w in walls)
            {
                w.Draw(window);
            }

            window.Display();
        }
    }
}