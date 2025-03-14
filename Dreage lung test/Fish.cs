using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
namespace Dredge_lung_test
{
    public class Fish : Sprite, ICollidable, ILayerable
    {
        private readonly float FishLayer = 0.8f;
        public Rectangle SourceRect { get; private set; }
        private Rectangle CollisionRect { get; set; }
        public string Name { get; private set; }
        public List<Anomaly> Anomalies { get; private set; }
        public static bool ShowCollisionRects = true;
        public bool IsActive { get; private set; } = true;

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
            // Generate anomalies for this fish
            Anomalies = AnomalyManager.Instance.GenerateAnomaliesForFish(this);

            ZIndex = 30; // Medium priority
            UpdateLayerDepth();

            // Initial collision rect setup
            UpdateCollisionRect();

            // Register with collision manager
            CollisionManager.Instance.Register(this);
        }

        public override void Update()
        {
            if (!IsActive) return;

            Movement();
            // Flip the sprite based on direction
            SpriteEffect = Direction.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            UpdateCollisionRect();
        }

        public override void UpdateLayerDepth()
        {
            base.UpdateLayerDepth();

            // Update anomalies layer depth to be slightly above the fish
            foreach (var anomaly in Anomalies)
            {
                // This assumes Anomaly implements ILayerable or has a LayerDepth property
                anomaly.LayerDepth = LayerDepth + 0.01f;
            }
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
            return SourceRect;
        }

        public override void Draw()
        {
            if (!IsActive) return;

            // Draw the base fish - use the LayerDepth property instead of FishLayer constant
            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                SourceRect,
                Color.White,
                0,
                new Vector2(SourceRect.Width / 2, SourceRect.Height / 2),
                Scale,
                SpriteEffect,
                LayerDepth // Use the property instead of the constant
            );

            // Draw anomalies on top of the fish
            foreach (var anomaly in Anomalies)
            {
                // The anomaly's LayerDepth property is already set in UpdateLayerDepth()
                anomaly.Draw(Position, Scale, SpriteEffect, anomaly.LayerDepth);
            }

            // Debug: Draw collision rectangle
            if (ShowCollisionRects)
            {
                DebugRenderer.DrawRectangle(CollisionRect, Color.Green, LayerDepth + 0.02f);
            }
        }

        public void OnCollision(ICollidable other)
        {
            // Handle collision with harpoon
            if (other is Harpoon && IsActive)
            {
                // Collision handling is primarily done in the Harpoon class
                // Fish could react here if needed
            }
        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            // Unregister from collision manager
            CollisionManager.Instance.Unregister(this);
        }
    }
}