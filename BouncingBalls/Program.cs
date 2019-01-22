using System.Collections.Generic;
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

            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            Clock C = new Clock();
            RenderWindow window = new RenderWindow(new VideoMode(640, 480), "SFML window with OpenGL", Styles.Default, contextSettings);

            while (window.IsOpen)
            {
                //Physics logic
                UpdatePhysics(bodies, walls, C.ElapsedTime.AsSeconds());

                //Drawing
                Draw(window, bodies, walls);
                
                C.Restart();
            }
        }

        public static void UpdatePhysics(List<Body> bodies, List<Wall> walls, double time)
        {
            foreach (Body b in bodies)
            {
                b.UpdateCollide(time,true, 4, walls, bodies);
            }
        }

        public static void Draw(RenderWindow window, List<Body> bodies, List<Wall> walls)
        {
            window.Clear(new Color(200, 200, 200));

            foreach (Body b in bodies)
            {
                b.Draw(window);
            }

            window.Display();
        }
    }
}