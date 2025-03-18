using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Dredge_lung_test
{
    //A class representing the rocks in the game
    public class Rock : Sprite, ICollidable
    {
        public Rectangle SourceRect { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Rock(Texture2D texture, Vector2 position, float speed, Rectangle sourceRect, Vector2 scale, Vector2 direction) : base(Globals.Content.Load<Texture2D>("Rocks"), position)
        {
            Speed = speed;
            Scale = scale;
            Direction = direction;
            SourceRect = sourceRect;

            //Create collision rectangle based on the scaled bounds
            float collisionWidth = sourceRect.Width * scale.X * 0.8f;
            float collisionHeight = sourceRect.Height * scale.Y * 0.8f;

            //Center the collision rectangle
            float offsetX = (sourceRect.Width * scale.X - collisionWidth) / 2;
            float offsetY = (sourceRect.Height * scale.Y - collisionHeight) / 2;

            ZIndex = 20; //Higher priority than fish
            UpdateLayerDepth();

            //Override the collision bounds
            Bounds = new Rectangle((int)(position.X + offsetX), (int)(position.Y + offsetY), (int)collisionWidth, (int)collisionHeight);

            CollisionManager.Instance.Register(this);
        }

        public override void Update()
        {
            if (!IsActive) return;

            Position += Direction * Speed * Globals.DeltaTime; //Move the rock according to direction and speed            
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Bounds.Width, Bounds.Height); //Update collision rectangle position
        }

        public void OnCollision(ICollidable other)
        {
            if (other is Player || other is Harpoon)
            {
                Deactivate(); //Disappears when colliding with player or the harpoon
            }

        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            CollisionManager.Instance.Unregister(this);
        }

        public override void Draw()
        {
            if (!IsActive) return;

            Globals.SpriteBatch.Draw(Texture, Position, SourceRect, Color, Rotation, Vector2.Zero, Scale, SpriteEffect, LayerDepth);

            //Debug collision rectangle
            if (Player.ShowCollisionRects)
            {
                DebugRenderer.DrawRectangle(Bounds, Color.Yellow, 0.9f);
            }
        }
    }
}