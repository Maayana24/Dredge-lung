using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Dredge_lung_test
{
    //Class representing all UI text
    public class Text : UIElement
    {
        private SpriteFont _font;
        private string _text;
        private Color _color;

        public Text(SpriteFont font, string text, Vector2 position, Color color, float scale = 1.0f) : base (null, position)
        {
            _font = font;
            _text = text;
            Position = position;
            _color = color;
            Scale = new Vector2(scale, scale); //Parent class use Vector scale so we need to convert to float
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
                Globals.SpriteBatch.DrawString(_font, _text, Position, _color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0.95f);
            }
        }

        public override void Update()
        {
            //Text is not using update but is still a UI element so keeping this method empty
        }
    }
}
