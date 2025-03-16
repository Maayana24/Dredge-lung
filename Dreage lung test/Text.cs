using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Dredge_lung_test
{
    public class Text : UIElement
    {
        private SpriteFont _font;
        private string _text;
        private Color _color;

        // Constructor with direct float scale parameter
        public Text(SpriteFont font, string text, Vector2 position, Color color, float scale = 1.0f) : base (null, position)
        {
            _font = font;
            _text = text;
            Position = position;
            _color = color;
            Scale = new Vector2(scale, scale); // Convert float to Vector2 immediately
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
                Globals.SpriteBatch.DrawString(_font, _text, Position, _color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0.9f);
            }
        }

        public override void Update()
        {
            //Empty???
        }
    }
}
