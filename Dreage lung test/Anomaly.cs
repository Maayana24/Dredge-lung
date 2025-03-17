using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public enum AnomalyType //Anomaly enum to add more types in the future
    {
        ExtraLimbs,
        Inflammation
    }
    //Anomaly class represents the anomalies that gets drawn on the fish
    public class Anomaly : ILayerable
    {
        public float LayerDepth { get; set; } = 0.7f;
        public int ZIndex { get; set; } = 0;
        public AnomalyType Type { get; private set; }
        public Texture2D Texture { get; private set; }
        public Rectangle SourceRect { get; private set; }

        public Anomaly(AnomalyType type, Texture2D texture, Rectangle sourceRect)
        {
            Type = type;
            Texture = texture;
            SourceRect = sourceRect;
        }
        public void UpdateLayerDepth() //Updating the anomaly layer depth based on the Z-index
        {
            float baseDepth = 0.7f;
            float zIndexContribution = ZIndex / 100.0f;

            LayerDepth = baseDepth + zIndexContribution;
            LayerDepth = MathHelper.Clamp(LayerDepth, 0.0f, 1.0f);
        }


        public void Draw(Vector2 position, Vector2 scale, SpriteEffects spriteEffect, float layerDepth)
    {
             try
             {
                Globals.SpriteBatch.Draw(Texture, position, SourceRect, Color.White, 0, new Vector2(SourceRect.Width / 2, SourceRect.Height / 2), scale, spriteEffect, layerDepth);
            
                int width = (int)(SourceRect.Width * scale.X * 0.5f);
                int height = (int)(SourceRect.Height * scale.Y * 0.5f);
                Rectangle bounds = new Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), width, height);
             }
             catch (Exception ex)
             {
                Debug.WriteLine($"Error drawing anomaly: {ex.Message}");
             }
    }
    }
}
