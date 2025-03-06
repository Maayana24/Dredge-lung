using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Layer
    {
        private readonly Texture2D Texture;

        private Vector2 Position;
        private Vector2 Position2;


        private readonly float _depth;

        private readonly float _moveScale;

        public Layer(Texture2D texture, float depth, float moveScale)
        {
            Texture = texture;
            _depth = depth;
            _moveScale = moveScale;
            Position = Vector2.Zero;
            Position2 = Vector2.Zero;
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

        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            Globals.SpriteBatch.Draw(Texture, Position2, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
        }
    }
}
