using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public abstract class Fish : Sprite, IFish
    {
        protected float _value = 10; // Default score value for a fish
        protected bool _hasBeenPhotographed = false;
        protected MovementPattern _movementPattern;
        protected List<IAnomaly> _anomalies = new List<IAnomaly>();

        public float Speed { get; set; } = 100f; // Base speed
        public Vector2 Direction { get; set; } = new Vector2(1, 0); // Default direction
        public float Value => _value;
        public bool HasBeenPhotographed => _hasBeenPhotographed;
        public MovementPattern MovementPattern
        {
            get => _movementPattern;
            set => _movementPattern = value;
        }

        public Fish(Vector2 position) : base (position)
        {
            // Default movement from left to right
            _movementPattern = MovementPattern.Horizontal;
            UpdateBounds();
        }

        public override void Update()
        {
            Movement();
            UpdateBounds();
        }

        public virtual void Movement()
        {
            // Apply movement based on pattern
            switch (_movementPattern)
            {
                case MovementPattern.Horizontal:
                    Position += Speed * Direction * Globals.DeltaTime;
                    break;
                case MovementPattern.Vertical:
                    Position += Speed * Direction * Globals.DeltaTime;
                    break;
                case MovementPattern.Wavy:
                    // Basic wavy pattern (horizontal with vertical sine wave)
                    Position += new Vector2(
                        Speed * Direction.X * Globals.DeltaTime,
                        (float)System.Math.Sin(Position.X * 0.01) * 0.5f
                    );
                    break;
            }
        }

        public override void Draw()
        {
            // Determine if fish should be flipped based on direction
            SpriteEffects effect = Direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw the base fish sprite
            Globals.SpriteBatch.Draw(Texture, Position, null, Color.White, 0f, Origin, Scale, effect, 0);

            // Draw any anomalies layered on top
            foreach (var anomaly in _anomalies)
            {
                anomaly.Draw(Globals.SpriteBatch, Position, Scale, effect);
            }
        }

        // Update bounds to match current position
        protected void UpdateBounds()
        {
            // Adjust bounds based on the fish's current position
            Bounds = new Rectangle(
                (int)(Position.X - (Texture.Width * Scale.X / 2)),
                (int)(Position.Y - (Texture.Height * Scale.Y / 2)),
                (int)(Texture.Width * Scale.X),
                (int)(Texture.Height * Scale.Y)
            );
        }

        // Implementation of IFish methods
        public bool IsInFrame(Rectangle frameBounds)
        {
            return Bounds.Intersects(frameBounds);
        }

        public virtual void OnPhotographed()
        {
            _hasBeenPhotographed = true;
            // Base for pausing and showing UI
            ShowFishInfoUI();
        }

        protected virtual void ShowFishInfoUI()
        {
            // This will be expanded in your UI implementation
            // For now, it's just a placeholder for the pausing and UI display logic
        }

        public void AddAnomaly(IAnomaly anomaly)
        {
            anomaly.ApplyTo(this);
            _anomalies.Add(anomaly);
        }
    }

    // Movement patterns for different fish types
    public enum MovementPattern
    {
        Horizontal,
        Vertical,
        Wavy
    }
}
