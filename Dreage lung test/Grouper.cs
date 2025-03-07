using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Grouper : Fish
    {
        public Grouper(Vector2 position) : base(position)
        {
            // Set source rectangle for the Grouper fish on the sprite sheet
            SourceRect = new Rectangle(150, 0, 200, 150); // Adjust this based on actual position in sprite sheet

            // Set properties
            Speed = 150.0f;
            Scale = new Vector2(0.5f, 0.5f);
        }
    }
}
