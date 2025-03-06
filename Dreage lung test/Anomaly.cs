using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Anomaly : IAnomaly
    {
        private readonly Texture2D _texture;
        private readonly Vector2 _offset;
        private readonly float _valueMultiplier;

        public Anomaly(Texture2D texture, Vector2 offset = default, float valueMultiplier = 1.5f)
        {
            _texture = texture;
            _offset = offset;
            _valueMultiplier = valueMultiplier;
        }

        public void ApplyTo(Fish fish)
        {
            // Modify the fish's properties based on the anomaly
            // For example, increase its value
            fish._value *= _valueMultiplier;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale, SpriteEffects effect)
        {
            // Draw the anomaly sprite on top of the fish
            // Adjust position based on offset if needed
            Vector2 drawPosition = position + (_offset * scale);
            spriteBatch.Draw(_texture, drawPosition, null, Color.White, 0f,
                new Vector2(_texture.Width / 2f, _texture.Height / 2f), scale, effect, 0);
        }
    }
}