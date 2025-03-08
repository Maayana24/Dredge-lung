using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public abstract class Fish : Sprite
    {
        protected readonly float FishLayer = 0.8f;
        protected Rectangle SourceRect { get; set; }
        protected Rectangle CollisionRect { get; set; }

        // List of anomalies for this fish
        public List<Anomaly> Anomalies { get; private set; }

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

        public Fish(Vector2 position) : base(Globals.Content.Load<Texture2D>("Fish1"), position)
        {
            // Generate anomalies when fish is created
            Anomalies = AnomalyManager.Instance.GenerateAnomaliesForFish(this);
        }

        public virtual void Update()
        {
            Movement();
            // Flip the sprite based on direction while preserving initial mirroring
            SpriteEffect = Direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            UpdateCollisionRect();
        }

        public virtual void Movement()
        {
            Position += Speed * Direction * Globals.DeltaTime;
        }

        protected virtual void UpdateCollisionRect()
        {
            // Update collision bounds to follow position
            int width = (int)(SourceRect.Width * Scale.X * 0.6f);
            int height = (int)(SourceRect.Height * Scale.Y * 0.6f);

            CollisionRect = new Rectangle(
                (int)(Position.X - width / 2),
                (int)(Position.Y - height / 2),
                width,
                height
            );

            Bounds = CollisionRect;
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
        }
    }

}