using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Shark : Fish
    {
        public Shark(Vector2 position) : base(position)
        {
            Bounds = new Rectangle(0, 550, 1000, 800);

        }

        public override void Movement()
        {
            base.Movement();
        }

        public override void Draw()
        {
            base.Draw();
            System.Diagnostics.Debug.WriteLine($"Grouper fish texture: {Texture?.Width}x{Texture?.Height}");

        }
    }
}
