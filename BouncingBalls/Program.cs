﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;
using System.Threading;

namespace BouncingBalls
{
    internal class Program
    {
        public static List<Polygon> polygons;
        public static List<Ball> balls;
        public static List<IResolvable> pairs;

        public static bool throwing = false;
        public static Vector throw_start;

        public static RenderWindow window;
        
        public static void Main(string[] args)
        {
            polygons = new List<Polygon>();
            balls = new List<Ball>();
            pairs = new List<IResolvable>();
            throw_start = new Vector(0,0);

            Random R = new Random();
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    //Vector position = new Vector(-2 + i * 0.2, j * 0.3);
                    //RandomBall(balls, position, R);
                }
            }
            
            string exe_path = AppDomain.CurrentDomain.BaseDirectory;
            string file_path = exe_path + "\\scene.txt";
            LoadScene(polygons, file_path);
            
            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            Clock C = new Clock();
            window = new RenderWindow(new VideoMode(800, 600), "SFML window with OpenGL", Styles.Default,
                contextSettings);
            window.SetView(Camera.view);

            Camera.SetView(new View(new Vector2f(0, 0), new Vector2f(2, 1.5f) * 5), window);

            double max_time = 1.0 / 60.0;
            C.Restart();
            
            window.Closed += (sender, e) => { ((Window)sender).Close(); };
            
            
            while (window.IsOpen)
            {
                window.DispatchEvents();

                UserInput();
                
                //Physics logic
                double time = C.ElapsedTime.AsSeconds();
                if (time > max_time)
                {
                    time = max_time;
                }

                C.Restart();
                UpdatePhysics(time);

                //Drawing
                Draw(window);
            }
        }

        public static void UserInput()
        {
            if (!throwing && Mouse.IsButtonPressed(0))
            {
                throwing = true;
                throw_start = MouseWorldPosition();
            }

            if (throwing && !Mouse.IsButtonPressed(0))
            {
                throwing = false;
                Vector throw_end = MouseWorldPosition();
                Vector diff = (throw_end - throw_start) * 3;
                    
                balls.Add(new Ball(throw_start, 0.058, 0.1, diff * 0.058, 0.3));
            }
        }
        
        public static void RandomBall(List<Ball> balls, Vector position, Random R)
        {
            double range = 0.2;
            double rrange = 0;
            double t = (R.NextDouble() - 0.5) * 2;
            double t2 = (R.NextDouble() - 0.5) * 2;
            double t3 = (R.NextDouble() - 0.5) * 2;
            Vector momentum = new Vector(t, t2) * range;
            double density = 0.058 / (Math.PI * 0.1 * 0.1);
            double radius = 0.1 + rrange * t3;
            double area = Math.PI * radius * radius;
            double mass = density * area;

            balls.Add(new Ball(position, 0.058, 0.1, momentum, 0.1));
        }

        public static Vector MouseWorldPosition()
        {
            Vector v = window.MapPixelToCoords(Mouse.GetPosition(window)).Vector();
            v.Y *= -1;
            return v;
        }
        
        public static void UpdatePhysics(double time)
        {
            foreach (Ball b in balls)
            {
                b.UpdateMove(false, time);
                b.UpdateCollide(polygons, balls, pairs, time);
                
            }

            ResolveCollisions();
            pairs.Clear();
        }

        public static void ResolvePairs(List<IResolvable> pairs)
        {
            
        }
        
        public static void ResolveCollisions()
        {
            int cores = Environment.ProcessorCount;
            List<List<IResolvable>> splits = SplitList(pairs, cores);

            List<Thread> threads = new List<Thread>();
            
            foreach (List<IResolvable> l in splits)
            {
                threads.Add();new Thread(delegate () {
                    ResolvePairs(l);
                }).Start();
            }
            
            
            foreach (IResolvable collision in pairs)
            {
                collision.ResolveCollision();
            }

            pairs.Clear();
        }
        
        public static void Draw(RenderWindow window)
        {
            byte brightness = 230;
            window.Clear(new Color(brightness, brightness, brightness));

            foreach (Ball b in balls)
            {
                b.Draw(window);
            }

            foreach (Polygon p in polygons)
            {
                p.DrawOutline(window);
            }

            if (throwing)
            {
                Vertex[] line = new Vertex[2];
                line[0] = new Vertex(throw_start.Vector2fInvY(), Color.Green);
                line[1] = new Vertex(MouseWorldPosition().Vector2fInvY(), Color.Green);
                
                window.Draw(line, PrimitiveType.Lines);
            }
            
            window.Display();
        }

        public static Vector ReadVector(string text)
        {
            string[] elements = text.Split(',');
            return new Vector(Convert.ToDouble(elements[0]), Convert.ToDouble(elements[1]));
        }

        public static Polygon ReadPolygon(string text)
        {
            string[] data = text.Split(' ');
            List<Vector> points_v = new List<Vector>();
            for (int i = 1, w = data.Length; i < w; i++)
            {
                points_v.Add(ReadVector(data[i]));
            }

            return new Polygon(Convert.ToDouble(data[0]), points_v);
        }

        public static List<List<IResolvable>> SplitList(List<IResolvable> pairs, int partitions)
        {
            int per_list = pairs.Count / partitions;
            List < List < IResolvable >> splits = new List<List<IResolvable>>();

            if (pairs.Count <= partitions)
            {
                for (int i = 0, w = pairs.Count; i < w; i++)
                {
                    splits.Add(pairs.GetRange(i, 1));
                }

                return splits;
            }
            
            for (int i = 0; i < partitions; i++)
            {
                if (i == partitions - 1)
                {
                    splits.Add(pairs.GetRange(i * per_list, pairs.Count - i * per_list));
                }
                else
                {
                    splits.Add(pairs.GetRange(i * per_list, per_list));
                }
            }

            return splits;
        }

        public static void LoadScene(List<Polygon> polygons, string file)
        {
            StreamReader read = new StreamReader(file);

            string line = read.ReadLine();
            while (!String.IsNullOrEmpty(line))
            {
                polygons.Add(ReadPolygon(line));

                line = read.ReadLine();
            }
        }
    }
}