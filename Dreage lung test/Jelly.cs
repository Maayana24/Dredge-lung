using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Jelly : Fish
    {
        public Jelly(Vector2 position) : base(position)
        {
            // Set source rectangle for the Jelly fish on the sprite sheet
            SourceRect = new Rectangle(0, 200, 400, 350);

            // Set properties
            Speed = 120.0f;
            Scale = new Vector2(0.5f, 0.5f);

            // Set vertical movement direction
            Direction = new Vector2(0, -1);
        }
    }
}
