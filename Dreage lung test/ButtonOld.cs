/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Dredge_lung_test.IM;
using System;

namespace Dredge_lung_test
{
    public class ButtonOld : UIElement, IClickable
    {

        private string _text;
        private readonly SpriteFont _font;
        private Color _textColor = Color.White;
        private Color _buttonColor;
        private Color _hoverColor;
        private Rectangle Bounds;
        private bool _isHoverd;

        public event Action Clicked;

        //private readonly Rectangle _rec;

        public ButtonOld(Vector2 position, Vector2 size, string text, SpriteFont font, Texture2D texture) : base(position, size, texture, UILayer)
        {
            //Texture = texture;
            _text = text;
            _font = font;
            _textColor = Color.White;
            _buttonColor = Color.White;
            _hoverColor = Color.DarkGray;
            _isHoverd = false;

            Bounds = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public ButtonOld(Vector2 position, Vector2 size, Texture2D texture) : base(position, size, texture, UILayer)
        {
            //Texture = texture;
            _textColor = Color.White;
            _buttonColor = Color.White;
            _hoverColor = Color.DarkGray;
            _isHoverd = false;

            Bounds = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }
        public override Vector2 Position
        {
            get => new Vector2(Bounds.X, Bounds.Y);
            set
            {
                Bounds.X = (int)value.X;
                Bounds.Y = (int)value.Y;
                base.Position = value;
            }
        }

        // Update the bounds when size changes
        public override Vector2 Size
        {
            get => new Vector2(Bounds.Width, Bounds.Height);
            set
            {
                Bounds.Width = (int)value.X;
                Bounds.Height = (int)value.Y;
                base.Size = value;
            }
        }

        public bool IsMouseOver()
        {
            // return new Rectangle(Position.ToPoint(), Size.ToPoint()).Contains(MouseState.Position);
            Rectangle buttonBounds = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

            // Get current mouse position and check if it's inside the button bounds
            Point mousePosition = MouseState.Position;
            return buttonBounds.Contains(mousePosition);
        }

        public void Click()
        {
            Clicked?.Invoke();
        }

        public override void Update()
        {
            System.Diagnostics.Debug.WriteLine($"Mouse: {MouseState.Position}, Button: {Position} -> {Position + Size}");
            _isHoverd = IsMouseOver();

            if (_isHoverd)
            {
                System.Diagnostics.Debug.WriteLine("Hovering!");
            }
                if (_isHoverd && MouseClicked)
            {
               Click();
            }
        }

        public override void Draw()
        {
            if(!IsVisible) return;

            Color drawColor = _isHoverd ? _hoverColor : _buttonColor;
            Globals.SpriteBatch.Draw(Texture, Position, null, drawColor, 0, Vector2.Zero, Size, SpriteEffects.None, UILayer);
            if (_text != null && _font != null)
            {
                Vector2 textSize = _font.MeasureString(_text);
                Vector2 textPos = Position + (Size - textSize) / 2;
                Globals.SpriteBatch.DrawString(_font, _text, textPos, _textColor);
            }
        }

    }
}
*/