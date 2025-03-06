using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public interface IAnomaly
    {
        void ApplyTo(Fish fish);
        void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale, SpriteEffects effect);
    }
}
