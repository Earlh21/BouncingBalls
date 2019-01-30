using System;
using System.Windows;

namespace BouncingBalls
{
	public class PointCollision : IResolvable
	{
		private Ball b;
		private CollisionData c;
		
		
		public Ball B
		{
			get { return b; }
		}
		
		public CollisionData C
		{
			get { return c; }
		}

		public PointCollision(Ball b, CollisionData c)
		{
			this.b = b;
			this.c = c;
		}

		public void ResolveCollision()
		{
			lock (b)
			{
				b.Position += c.Displacement * 1.1;

				Vector normal_momentum = b.Momentum.Project(c.Angle + Math.PI);
				Vector tangent_momentum =
					b.Momentum.Project(c.Angle + Math.PI / 2 * Math.Sign(b.Momentum.Angle() - c.Angle));

				b.Momentum = normal_momentum * -Ball.cor + tangent_momentum;
			}
		}
	}
}