using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public abstract class Fish : Sprite
    {
        public Vector2 Direction { get; set; } = new Vector2(1, 0); // Default to right
        public float Speed { get; protected set; } = 150.0f;
        protected readonly float FishLayer = 0.8f;

        // New property for the source rectangle (which part of the sprite sheet to draw)
        protected Rectangle SourceRect { get; set; }

        // Collision rectangle that follows the fish
        protected Rectangle CollisionRect { get; set; }

        public Fish(Vector2 position) : base(Globals.Content.Load<Texture2D>("Fish1"), position)
        {
            // Default source rectangle will be set by derived classes
            // Default collision rectangle
            CollisionRect = new Rectangle(0, 0, 100, 50); // Default size, will be updated in derived classes
        }

        public virtual void Update()
        {
            Movement();

            // Update sprite flip based on direction
            if (Direction.X < 0)
                SpriteEffect = SpriteEffects.FlipHorizontally;
            else if (Direction.X > 0)
                SpriteEffect = SpriteEffects.None;

            // Update collision rectangle to follow fish position
            UpdateCollisionRect();
        }

        public virtual void Movement()
        {
            Position += Speed * Direction * Globals.DeltaTime;
        }

        protected virtual void UpdateCollisionRect()
        {
            // Update collision bounds to follow position
            // Use a size appropriate for the actual fish visual (smaller than the source rect)
            // These values may need tweaking based on the actual fish visuals
            int width = (int)(SourceRect.Width * Scale.X * 0.6f);  // 60% of the source width scaled
            int height = (int)(SourceRect.Height * Scale.Y * 0.6f); // 60% of the source height scaled

            // Center the collision rectangle on the fish
            CollisionRect = new Rectangle(
                (int)(Position.X - width / 2),
                (int)(Position.Y - height / 2),
                width,
                height
            );

            // Update the Bounds property to match for compatibility with existing code
            Bounds = CollisionRect;
        }

        public override void Draw()
        {
            // Draw using the source rectangle from the sprite sheet
            Globals.SpriteBatch.Draw(
                Texture,              // The texture containing all fish
                Position,             // Position to draw at
                SourceRect,           // Source rectangle (which part of the texture)
                Color.White,          // Color tint
                0,                    // Rotation
                new Vector2(SourceRect.Width / 2, SourceRect.Height / 2), // Origin at center of source rect
                Scale,                // Scale
                SpriteEffect,         // Flip effect
                FishLayer             // Layer depth
            );

            // Debug drawing of collision rectangle
            // Comment out in production
            /*
            Texture2D debugTexture = new Texture2D(Globals.SpriteBatch.GraphicsDevice, 1, 1);
            debugTexture.SetData(new[] { Color.Red });
            Globals.SpriteBatch.Draw(
                debugTexture,
                CollisionRect,
                Color.Red * 0.3f
            );
            */
        }
    }
}