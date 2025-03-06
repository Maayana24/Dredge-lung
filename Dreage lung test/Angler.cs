using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Angler : Fish
    {
        public Angler(Vector2 position) : base(position)
        {
            Bounds = new Rectangle(550, 0, 700, 300);
            Scale = new Vector2(0.5f, 0.5f);
        }

        public override void Movement()
        {
            base.Movement();
        }
    }
}
