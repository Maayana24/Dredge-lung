using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Dredge_lung_test
{
    public enum AnomalyType
    {
        ExtraLimbs,
        Inflammation
    }

    public class Anomaly
    {
        public AnomalyType Type { get; private set; }
        public bool IsDeadly { get; private set; }
        public Texture2D Texture { get; private set; }
        public Rectangle SourceRect { get; private set; }
        public Color FallbackColor { get; private set; }

        // Static counter for unique IDs
        private static int _idCounter = 0;
        private int _id;

        // Constructor with fallback color option
        public Anomaly(AnomalyType type, bool isDeadly, Texture2D texture, Rectangle sourceRect, Color fallbackColor)
        {
            Type = type;
            IsDeadly = isDeadly;
            Texture = texture;
            SourceRect = sourceRect;
            FallbackColor = fallbackColor;
            _id = _idCounter++;

            // Debug output on creation
            Console.WriteLine($"Anomaly #{_id} created: Type={type}, Deadly={isDeadly}, SourceRect={sourceRect}");
        }

        // Overload without fallback color
        public Anomaly(AnomalyType type, bool isDeadly, Texture2D texture, Rectangle sourceRect)
            : this(type, isDeadly, texture, sourceRect, Color.White)
        {
        }

        public void Draw(Vector2 position, Vector2 scale, SpriteEffects spriteEffect, float layerDepth)
        {
            try
            {
                // Debug every 60 frames (about once per second at 60fps)

                // Try to draw using texture and source rectangle first
                Globals.SpriteBatch.Draw(
                    Texture,
                    position,
                    SourceRect,
                    Color.White,
                    0,
                    new Vector2(SourceRect.Width / 2, SourceRect.Height / 2),
                    scale,
                    spriteEffect,
                    layerDepth + 0.01f
                );

                // ALTERNATE DRAWING METHOD: Draw a colored rectangle as fallback
                // This creates a simple visible indicator that the anomaly exists
                int width = (int)(SourceRect.Width * scale.X * 0.5f);  // Make it smaller than fish
                int height = (int)(SourceRect.Height * scale.Y * 0.5f);
                Rectangle bounds = new Rectangle(
                    (int)(position.X - width / 2),
                    (int)(position.Y - height / 2),
                    width,
                    height
                );

                // Draw fallback rectangle
                Texture2D pixel = new Texture2D(Globals.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                Globals.SpriteBatch.Draw(pixel, bounds, FallbackColor * 0.5f);  // Semi-transparent

                // Always draw debug outline
                Color debugColor = IsDeadly ? Color.Red : Color.Yellow;
                DebugRenderer.DrawRectangle(bounds, debugColor, 0.02f);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR drawing anomaly #{_id}: {ex.Message}");
            }
        }
    }
}