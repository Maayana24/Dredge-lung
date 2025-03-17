using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Dredge_lung_test
{
    public class Rock : Sprite, ICollidable, ILayerable
    {
        public Rectangle SourceRect { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Rock(Texture2D texture, Vector2 position, float speed, Rectangle sourceRect, Vector2 scale, Vector2 direction) : base(Globals.Content.Load<Texture2D>("Rocks"), position)
        {
            Speed = speed;
            Scale = scale;
            Direction = direction;
            SourceRect = sourceRect;

            // Create collision rectangle based on the scaled bounds
            float collisionWidth = sourceRect.Width * scale.X * 0.8f; // Slightly smaller than visual
            float collisionHeight = sourceRect.Height * scale.Y * 0.8f;
            // Center the collision rectangle
            float offsetX = (sourceRect.Width * scale.X - collisionWidth) / 2;
            float offsetY = (sourceRect.Height * scale.Y - collisionHeight) / 2;

            ZIndex = 20; // Higher priority than fish
            UpdateLayerDepth();

            // Override the collision bounds
            Bounds = new Rectangle(
                (int)(position.X + offsetX),
                (int)(position.Y + offsetY),
                (int)collisionWidth,
                (int)collisionHeight
            );

            // Register with collision manager
            CollisionManager.Instance.Register(this);
        }

        public override void Update()
        {
            if (!IsActive) return;

            // Move the rock according to direction and speed
            Position += Direction * Speed * Globals.DeltaTime;

            // Update collision rectangle position
            Bounds = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Bounds.Width,
                Bounds.Height
            );
        }

        public override void Draw()
        {
            if (!IsActive) return;

            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                SourceRect,
                Color,
                Rotation,
                Vector2.Zero, // Drawing from top-left corner
                Scale,
                SpriteEffect,
                LayerDepth
            );

            // Debug collision rectangle
            if (Player.ShowCollisionRects)
            {
                DebugRenderer.DrawRectangle(Bounds, Color.Yellow, 0.9f);
            }
        }

        public void OnCollision(ICollidable other)
        {
            // Handle collisions with player
            if (other is Player || other is Harpoon)
            {
                Deactivate();
                // Note: ScoreManager interactions are now handled in Player class's OnCollision method
            }

        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            // Unregister from collision manager
            CollisionManager.Instance.Unregister(this);
        }

        // Added property to store source rectangle
    }
}