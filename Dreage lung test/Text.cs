using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public class Text : UIElement
    {
        private readonly SpriteFont _font;
        private string _text;

        // Properties for the Button class to access
        public string Content => _text;
        public Color Color { get; set; }
        public float Scale { get; set; }

        // Constructor with direct float scale parameter
        public Text(SpriteFont font, string text, Vector2 position, Color color, float scale = 1.0f)
            : base(null, position)
        {
            _font = font;
            _text = text;
            Position = position;
            Color = color;
            Scale = scale; // Store original scale value for easier access
            base.Scale = new Vector2(scale, scale); // Set Vector2 scale for base class

            // Set layer depth to ensure text appears above backgrounds
            LayerDepth = 0.9f;
        }

        public void SetText(string text)
        {
            _text = text;
        }

        public string GetText()
        {
            return _text;
        }

        public override void Draw()
        {
            if (IsVisible)
            {
                Globals.SpriteBatch.DrawString(
                    _font,
                    _text,
                    Position,
                    Color,
                    0f,
                    Vector2.Zero,
                    base.Scale,
                    SpriteEffects.None,
                    LayerDepth);
            }
        }

        public override void Update()
        {
            // No update logic needed for basic text
        }

        // Helper to update scale if needed
        public void SetScale(float newScale)
        {
            Scale = newScale;
            base.Scale = new Vector2(newScale, newScale);
        }
    }
}