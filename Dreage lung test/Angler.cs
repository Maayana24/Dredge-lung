using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Angler : Fish
    {
        public Angler(Vector2 position) : base(position)
        {
            SourceRect = new Rectangle(550, 0, 700, 300);
            Speed = 180.0f;
            Scale = new Vector2(0.5f, 0.5f);
            Direction = new Vector2(-1, 0); // Start moving left (mirror)
        }
    }
}

