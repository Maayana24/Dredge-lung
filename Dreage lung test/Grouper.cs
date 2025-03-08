using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Grouper : Fish
    {
        public Grouper(Vector2 position) : base(position)
        {
            SourceRect = new Rectangle(100, 0, 600, 280);
            Speed = 150.0f;
            Scale = new Vector2(0.6f, 0.6f);
            Direction = new Vector2(-1, 0); // Start moving left (mirror)
        }
    }
}
