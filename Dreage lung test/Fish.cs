using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public class Fish : Sprite
    {
        private readonly float FishLayer = 0.8f;
        public Rectangle SourceRect { get; private set; }
        private Rectangle CollisionRect { get; set; }
        public string Name { get; private set; }
        public List<Anomaly> Anomalies { get; private set; }
        public static bool ShowCollisionRects = true;

        // Property to check if the fish has any anomalies
        public bool HasAnomalies => Anomalies.Count > 0;

        public Fish(string name, Vector2 position, float speed = 150, Rectangle sourceRect = default, Vector2? scale = null, Vector2? direction = null)
            : base(Globals.Content.Load<Texture2D>("Fish/FishTemplate"), position)
        {
            Name = name;
            Speed = speed;
            Direction = direction ?? Direction;

            // Ensure valid source rectangle
            SourceRect = sourceRect.Width > 0 && sourceRect.Height > 0 ?
                sourceRect : new Rectangle(0, 0, 100, 100);

            Scale = scale ?? Scale;

            Debug.WriteLine($"Fish '{Name}' created with source rect: {SourceRect}");

            // Generate anomalies for this fish
            Anomalies = AnomalyManager.Instance.GenerateAnomaliesForFish(this);
        }

        public virtual void Update()
        {
            Movement();
            // Flip the sprite based on direction
            SpriteEffect = Direction.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            UpdateCollisionRect();
        }

        public virtual void Movement()
        {
            Position += Speed * Direction * Globals.DeltaTime;
        }

        protected virtual void UpdateCollisionRect()
        {
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

            // Draw anomalies on top of the fish
            foreach (var anomaly in Anomalies)
            {
                anomaly.Draw(Position, Scale, SpriteEffect, FishLayer);
            }

            // Debug: Draw collision rectangle (always green now)
            if (ShowCollisionRects)
            {
                DebugRenderer.DrawRectangle(CollisionRect, Color.Green, 0.1f);
            }
        }
    }
}
