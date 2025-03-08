using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Dredge_lung_test
{
    // Enum to represent all possible anomaly types
    public enum AnomalyType
    {
        ExtraLimbs,
        Inflammation,
        // Future types can be added here:
        // Glow,
        // Other anomalies...
    }

    // Class to represent an anomaly
    public class Anomaly
    {
        public AnomalyType Type { get; private set; }
        public bool IsDeadly { get; private set; }
        public Texture2D Texture { get; private set; }
        public Rectangle SourceRect { get; private set; }

        public Anomaly(AnomalyType type, bool isDeadly, Texture2D texture, Rectangle sourceRect)
        {
            Type = type;
            IsDeadly = isDeadly;
            Texture = texture;
            SourceRect = sourceRect;
        }

        public void Draw(Vector2 position, Vector2 scale, SpriteEffects spriteEffect, float layerDepth)
        {
            Globals.SpriteBatch.Draw(
                Texture,
                position,
                SourceRect,
                Color.White,
                0,
                new Vector2(SourceRect.Width / 2, SourceRect.Height / 2),
                scale,
                spriteEffect,
                layerDepth + 0.01f  // Draw slightly above the fish
            );
        }
    }
}
