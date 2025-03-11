using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Player : Sprite
    {
        private Vector2 _velocity;
        private Vector2 _acceleration = new Vector2(0, 0);
        private float _movement = 4;
        public static bool ShowCollisionRects = true;

        public Player(Texture2D texture, Vector2 position) : base(texture, position) 
        {
            //this.Texture = tex;
            Speed = 400;
            Scale = new Vector2(0.15f, 0.15f);
            Position = position;
        }

        public void Update()
        {

            //Direction = Vector2.Normalize(IM.Direction);
            if (IM.Direction != Vector2.Zero)
                Position += IM.Direction * Speed * Globals.DeltaTime;

            Bounds = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * Scale.X), (int)(Texture.Height * Scale.Y));
            /*            if(IM.Direction != Vector2.Zero)
                        {
                            Direction = Vector2.Normalize(IM.Direction);
                           // position += dir * speed * Globals.TotalSeconds;
                            _velocity += Direction * Speed * Globals.DeltaTime;
                            _acceleration.X = Speed;
                        }
                        if(_acceleration == Vector2.Zero)
                        {
                            Vector2 friction = _velocity;
                            if(friction.Length() > 0)
                            {
                                friction.Normalize();
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
                        Position += _velocity * Globals.DeltaTime;*/

        }

        public override void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0.9f);

            if (ShowCollisionRects)
            {
                Color debugColor = Color.Red;
                DebugRenderer.DrawRectangle(Bounds, debugColor, 0.9f);
            }
        }
    }
}
