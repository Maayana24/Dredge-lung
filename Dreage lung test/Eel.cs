using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Eel : Fish
    {
        public Eel(Vector2 position) : base(position)
        {
            Bounds = new Rectangle(450, 300, 800, 300);
            Scale = new Vector2(0.5f, 0.5f);
        }

        public override void Movement()
        {
            base.Movement();
        }
    }
}
