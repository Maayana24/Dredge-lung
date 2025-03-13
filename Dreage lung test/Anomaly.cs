using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public enum AnomalyType
    {
        ExtraLimbs,
        Inflammation
    }

    public class Anomaly
    {
        public AnomalyType Type { get; }
        public Texture2D Texture { get; }
        public Rectangle SourceRect { get; }

        private static int _idCounter = 0;
        private readonly int _id;

        public Anomaly(AnomalyType type, Texture2D texture, Rectangle sourceRect)
        {
            Type = type;
            Texture = texture;
            SourceRect = sourceRect;
            _id = _idCounter++;
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
                layerDepth + 0.01f
            );

            int width = (int)(SourceRect.Width * scale.X * 0.5f);
            int height = (int)(SourceRect.Height * scale.Y * 0.5f);
            Rectangle bounds = new Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), width, height);

            DrawFallbackRectangle(bounds);
            DebugRenderer.DrawRectangle(bounds, Color.Yellow, 0.02f);
        }

        private void DrawFallbackRectangle(Rectangle bounds)
        {
            Texture2D pixel = new Texture2D(Globals.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            Globals.SpriteBatch.Draw(pixel, bounds, Color.White * 0.5f);
        }
    }
}
