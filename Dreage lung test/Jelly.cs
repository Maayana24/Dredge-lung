using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Jelly : Fish
    {
        public Jelly(Vector2 position) : base(position)
        {
            Bounds = new Rectangle(0, 200, 400, 350);
            Scale = new Vector2(0.5f, 0.5f);

        }

        public override void Movement()
        {
            base.Movement();
        }
    }
}
