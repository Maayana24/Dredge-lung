using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
     public class Shark : Fish
    {
        public Shark(Vector2 position) : base(position)
        {
            // Set source rectangle for the Shark fish on the sprite sheet
            SourceRect = new Rectangle(150, 550, 600, 200);
            
            // Set properties - Sharks are slower but larger
            Speed = 100.0f;
            Scale = new Vector2(0.3f, 0.3f);
        }
    }
}
