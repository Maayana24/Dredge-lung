using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public class Fish : Sprite
    {
        private readonly float FishLayer = 0.8f;
        // Changed from private to public to ensure it's accessible
        public Rectangle SourceRect { get; private set; }
        private Rectangle CollisionRect { get; set; }
        public string Name { get; private set; }
        // List of anomalies for this fish
        public List<Anomaly> Anomalies { get; private set; }
        public static bool ShowCollisionRects = true;

        // Property to check if the fish has any deadly anomalies
        public bool HasDeadlyAnomaly
        {
            get
            {
                foreach (var anomaly in Anomalies)
                {
                    if (anomaly.IsDeadly)
                        return true;
                }
                return false;
            }
        }

        public Fish(string name, Vector2 position, float speed = 150, Rectangle sourceRect = default, Vector2? scale = null, Vector2? direction = null) : base(Globals.Content.Load<Texture2D>("Fish/FishTemplate"), position)
        {
            Name = name;
            Speed = speed;
            Direction = direction ?? Direction;

            // Initialize SourceRect properly - ensure it has valid dimensions
            SourceRect = sourceRect.Width > 0 && sourceRect.Height > 0 ?
                sourceRect : new Rectangle(0, 0, 100, 100);

            Scale = scale ?? Scale;

            // Debug to verify source rectangle is set correctly
            Debug.WriteLine($"Fish '{Name}' created with source rect: {SourceRect}");

            // Generate anomalies after fish is fully initialized
            Anomalies = AnomalyManager.Instance.GenerateAnomaliesForFish(this);
        }

        public virtual void Update()
        {
            Movement();
            // Flip the sprite based on direction while preserving initial mirroring
            SpriteEffect = Direction.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            UpdateCollisionRect();
        }

        public virtual void Movement()
        {
            Position += Speed * Direction * Globals.DeltaTime;
        }

        protected virtual void UpdateCollisionRect()
        {
            // Use full dimensions of the source rectangle for collision
            int width = (int)(SourceRect.Width * Scale.X);
            int height = (int)(SourceRect.Height * Scale.Y);
            CollisionRect = new Rectangle(
                (int)(Position.X - width / 2),
                (int)(Position.Y - height / 2),
                width,
                height
            );
            Bounds = CollisionRect;
        }

        public Rectangle GetSourceRect()
        {
            // Debug output to verify what's being returned
            Debug.WriteLine($"GetSourceRect called for {Name}: {SourceRect}");
            return SourceRect;
        }

        public override void Draw()
        {
            // Draw the base fish
            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                SourceRect,
                Color.White,
                0,
                new Vector2(SourceRect.Width / 2, SourceRect.Height / 2),
                Scale,
                SpriteEffect,
                FishLayer
            );

            // Draw all anomalies on top of the fish
            foreach (var anomaly in Anomalies)
            {
                anomaly.Draw(Position, Scale, SpriteEffect, FishLayer);
            }

            if (ShowCollisionRects)
            {
                Color debugColor = HasDeadlyAnomaly ? Color.Red : Color.Green;
                DebugRenderer.DrawRectangle(CollisionRect, debugColor, 0.1f);
            }
        }
    }
}