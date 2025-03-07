using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Grouper : Fish
    {

        public Grouper(Vector2 position) : base(position)
        {
            Bounds = new Rectangle(200, 50, 300, 150);

        }

        public override void Movement()
        {
            base.Movement();
        }

        public override void Draw()
        {
            base.Draw();
           // System.Diagnostics.Debug.WriteLine($"Grouper fish texture: {Texture?.Width}x{Texture?.Height}");

        }
    }
}
