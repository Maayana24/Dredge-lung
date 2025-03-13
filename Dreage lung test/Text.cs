using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Dredge_lung_test
{
    public class Text : UIElement
    {
        private string _text;
        private SpriteFont _font;
        private Color _color;

        public Text(SpriteFont font, string text, Vector2 position, Color color) : base(null, position) // No texture needed
        {
            _font = font;
            _text = text;
            _color = color;
        }

        public void SetText(string text)
        {
            _text = text;
        }

        public override void Update()
        {
            // Handle animations, dynamic updates, etc. (if needed)
        }

        public override void Draw()
        {
            if (IsVisible)
            {
                Globals.SpriteBatch.DrawString(_font, _text, Position, _color, 0, Vector2.One, Vector2.One, SpriteEffects.None, UILayer);
            }
        }
    }
}
