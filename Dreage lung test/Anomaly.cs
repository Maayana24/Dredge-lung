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

    public class Anomaly : ILayerable
    {
        public float LayerDepth { get; set; } = 0.7f;
        public int ZIndex { get; set; } = 0;
        public AnomalyType Type { get; private set; }
        public Texture2D Texture { get; private set; }
        public Rectangle SourceRect { get; private set; }

        // Static counter for unique IDs
        private static int _idCounter = 0;
        private int _id;

        // Constructor
        public Anomaly(AnomalyType type, Texture2D texture, Rectangle sourceRect)
        {
            Type = type;
            Texture = texture;
            SourceRect = sourceRect;
            _id = _idCounter++;
        }
        public void UpdateLayerDepth()
        {
            // Base anomaly depth
            float baseDepth = 0.7f;
            float zIndexContribution = ZIndex / 100.0f;

            LayerDepth = baseDepth + zIndexContribution;
            LayerDepth = MathHelper.Clamp(LayerDepth, 0.0f, 1.0f);
        }


        public void Draw(Vector2 position, Vector2 scale, SpriteEffects spriteEffect, float layerDepth)
        {
            try
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
                LayerDepth 
                );

                int width = (int)(SourceRect.Width * scale.X * 0.5f);
                int height = (int)(SourceRect.Height * scale.Y * 0.5f);
                Rectangle bounds = new Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), width, height);


                // Always draw debug outline in a neutral color
                DebugRenderer.DrawRectangle(bounds, Color.Yellow, 0.02f);
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"ERROR drawing anomaly #{_id}: {ex.Message}");
            }
        }
    }
}
