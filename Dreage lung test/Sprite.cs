using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dredge_lung_test
{
    public abstract class Sprite : ILayerable
    {
        public readonly Texture2D Texture;
        protected readonly Vector2 Origin;

        public int ZIndex { get; set; } = 0;
        public Vector2 Position { get;  set; }
        public Vector2 Direction { get; set; } = new Vector2(1, 0);
        public float Speed { get; protected set; }
        public Vector2 Scale { get; protected set; } = Vector2.One;
        public Rectangle Bounds { get; protected set; }
        public SpriteEffects SpriteEffect { get; protected set; } = SpriteEffects.None;
        public float Rotation { get; set; } = 0f;
        public Color Color { get; set; } = Color.White;
        public float LayerDepth { get; set; } = 0f;

        public Sprite(Texture2D texture, Vector2 position)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Position = position;
            Speed = 300;

            Origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Bounds = new Rectangle(0, 0, texture.Width, texture.Height);

        }

        public virtual void UpdateLayerDepth()
        {
            // Convert ZIndex to layer depth (0.0-1.0 range)
            // Higher ZIndex = lower layer depth (closer to 1.0)
            // Lower ZIndex = higher layer depth (closer to 0.0)
            LayerDepth = MathHelper.Clamp(1.0f - (ZIndex / 100.0f), 0.0f, 1.0f);
        }   
        public abstract void Update();

        public virtual void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, Bounds, Color.White, 0, Origin, Scale, SpriteEffect, 1);
        }

    }
}
