using Dredge_lung_test;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

public class Button : UIElement, IClickable
{
    private Action _onClick;
    private Color _color;
    private Texture2D _texture;
    public Text _buttonText;
    private float _scale;

    public Button(Vector2 position, Texture2D texture, Color color, Action onClick, float scale = 1.0f) : base(texture, position)
    {
        _onClick = onClick;
        _color = color;
        _texture = texture;
        _scale = scale;
    }

    public void SetText(string text, Color textColor, float scale = 1.0f)
    {
        if (Globals.Font == null)
            return;

        if (_buttonText == null)
        {
            _buttonText = new Text(Globals.Font, text, Vector2.Zero, textColor, scale);
            _buttonText.IsVisible = true;
        }
        else
        {
            _buttonText.SetText(text);
           // _buttonText.Color = textColor;
            _buttonText.Scale = new Vector2(scale, scale);
        }

        CenterText();
    }

    private void CenterText()
    {
        if (_buttonText != null && _texture != null)
        {
            Vector2 textSize = Globals.Font.MeasureString(_buttonText.GetText()) * _buttonText.Scale.X;

            // Calculate the center position of the button
            float buttonCenterX = Position.X + (_texture.Width * _scale / 2);
            float buttonCenterY = Position.Y + (_texture.Height * _scale / 2);

            // Position the text so its center aligns with the button's center
            Vector2 centeredPosition = new Vector2(
                buttonCenterX - (textSize.X / 2),
                buttonCenterY - (textSize.Y / 2)
            );

            _buttonText.Position = centeredPosition;

            // Debug output to see text positioning
            Debug.WriteLine($"Button Position: {Position}, Size: {_texture.Width * _scale}x{_texture.Height * _scale}");
            Debug.WriteLine($"Text Size: {textSize.X}x{textSize.Y}, Centered Position: {centeredPosition}");
        }
    }

    public bool IsMouseOver()
    {
        Rectangle buttonRect = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(_texture.Width * _scale),
            (int)(_texture.Height * _scale)
        );

        bool isOver = buttonRect.Contains((int)IM.MousePosition.X, (int)IM.MousePosition.Y);

        // Debug output when mouse is over button
        if (isOver && IM.MouseClicked)
        {
            Debug.WriteLine("Button clicked - IsMouseOver: true");
        }

        return isOver;
    }

    public void Click()
    {
        Debug.WriteLine("Button.Click() called");
        _onClick?.Invoke();
    }

    public override void Draw()
    {
        if (IsVisible)
        {
            Globals.SpriteBatch.Draw(_texture, Position, null, _color, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.9f);

            if (_buttonText != null && _buttonText.IsVisible)
            {
                _buttonText.Draw();
            }
        }
    }

    public override void Update()
    {
        if (!IsVisible) return;

        // Update button color based on mouse hover
        _color = IsMouseOver() ? Color.LightGray : Color.White;

        // Check for mouse click
        if (IsMouseOver() && IM.MouseClicked)
        {
            Debug.WriteLine("Button detected click in Update");
            Click();
        }
    }
}