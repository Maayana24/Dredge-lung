using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Layer : ILayerable, IDrawable
    {
        private readonly Texture2D Texture;
        public float LayerDepth { get; set; } = 0.1f;
        public int ZIndex { get; set; } = 0;

        private Vector2 Position;
        private Vector2 Position2;


        private readonly float _depth;

        private readonly float _moveScale;

        public Layer(Texture2D texture, float depth, float moveScale)
        {
            Texture = texture;
            _depth = depth;
            _moveScale = moveScale;
            Position = new Vector2(PlayableArea.X, PlayableArea.Y + Position2.Y);
            Position2 = new Vector2(PlayableArea.X, PlayableArea.Y + Position2.Y);
        }

        public void Update(float movement)
        {
            Position.Y += movement * _moveScale * Globals.DeltaTime;
            Position.Y %= Texture.Height;

            if(Position.Y >= 0)
            {
                Position2.Y = Position.Y - Texture.Height;
            }
            else
            {
                Position2.Y = Position.Y + Texture.Height;
            }
        }

       public void UpdateLayerDepth()
{
    // Layer depth is already set by the _depth parameter in constructor
    // You might want to adjust based on ZIndex if needed
    LayerDepth = _depth + (ZIndex / 100.0f);
    LayerDepth = MathHelper.Clamp(LayerDepth, 0.0f, 1.0f);
}

        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            Globals.SpriteBatch.Draw(Texture, Position2, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
        }
    }
}
