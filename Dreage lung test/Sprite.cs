using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dredge_lung_test
{
    public abstract class Sprite
    {
        protected readonly Texture2D Texture;
        protected readonly Vector2 Origin;

        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; set; } = new Vector2(1, 0);
        public float Speed { get; protected set; }
        public Vector2 Scale { get; protected set; } = Vector2.One;
        public Rectangle Bounds { get; protected set; }
        public SpriteEffects SpriteEffect { get; protected set; }
        public float Rotation { get; set; } = 0f;
        public Color Color { get; set; } = Color.White;
        public float LayerDepth { get; set; } = 0f;

        public Sprite(Texture2D texture, Vector2 position, bool isMirrored = false) // Added `isMirrored`
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Position = position;
            Speed = 300;

            Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Bounds = new Rectangle(0, 0, texture.Width, texture.Height);

            // Set initial mirroring
            SpriteEffect = isMirrored ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public virtual void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, Bounds, Color.White, 0, Origin, Scale, SpriteEffect, 1);
        }
    }
}
