namespace BouncingBalls
{
    public class Pair
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

        public Pair(Ball a, Ball b)
        {
            this.a = a;
            this.b = b;
        }
    }
}