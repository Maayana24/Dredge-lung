using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public abstract class Fish : Sprite
    {
        public Vector2 Direction { get; set; } = new Vector2(1, 0); // Default to right
        public float Speed { get; protected set; } = 150.0f;

        public Fish(Vector2 position) : base(null, position)
        {
            // The texture will be loaded in derived classes
            // Each fish type will have its own texture
        }

        public virtual void Update()
        {
            Movement();

            // Update sprite flip based on direction
            if (Direction.X < 0)
                SpriteEffect = SpriteEffects.FlipHorizontally;
            else if (Direction.X > 0)
                SpriteEffect = SpriteEffects.None;
        }

        public virtual void Movement()
        {
            Position += Speed * Direction * Globals.DeltaTime;

            // Update bounds to follow position
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Bounds.Width, Bounds.Height);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}