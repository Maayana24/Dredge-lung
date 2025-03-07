using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Shark : Fish
    {
        public Shark(Vector2 position) : base(position)
        {
            // Set source rectangle for the Shark fish on the sprite sheet
            SourceRect = new Rectangle(0, 550, 1000, 800);

            // Set properties - Sharks are slower but larger
            Speed = 100.0f;
            Scale = new Vector2(0.5f, 0.5f);
        }
    }
}
