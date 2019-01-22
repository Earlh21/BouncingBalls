using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace BouncingBalls
{
    public static class Camera
    {
        public static View view;

        static Camera()
        {
            view = new View();
        }

        public static void SetView(View v, RenderWindow w)
        {
            w.SetView(v);
        }
    }
}