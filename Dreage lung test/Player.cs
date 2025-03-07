using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Player : Sprite
    {
        private Vector2 _velocity;
        private Vector2 _acceleration = new Vector2(0, 0);
        private float _movement = 4;

        public Player(Texture2D t, Vector2 p) : base(t, p) 
        {
            //this.Texture = tex;
            // Speed = 4;
            Scale = new Vector2(0.3f, 0.3f);
        }

        public void Update()
        {
            if(IM.Direction != Vector2.Zero)
            {
                Direction = Vector2.Grouperize(IM.Direction);
               // position += dir * speed * Globals.TotalSeconds;
                _velocity += Direction * Speed * Globals.DeltaTime;
                _acceleration.X = Speed;
            }
            if(_acceleration == Vector2.Zero)
            {
                Vector2 friction = _velocity;
                if(friction.Length() > 0)
                {
                    friction.Grouperize();
                    friction *= Speed / 2 * Globals.DeltaTime;
                    _velocity -= friction;

                    if(_velocity.Length() < 0.01f)
                    {
                        _velocity = Vector2.Zero;
                    }
                }
            }
            else
            {
                _velocity += _acceleration * Globals.DeltaTime;
            }
            Position += _velocity * Globals.DeltaTime;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0.9f);
        }
    }
}
