using Dredge_lung_test;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;

//Class representing a button in UI
public class Button : UIElement, IClickable
{
    private Action _onClick;
    private Color _color;
    private Texture2D _texture;
    public Text _buttonText;
    private float _scale;
    private bool _useManualTextPosition = false;

    public Button(Vector2 position, Texture2D texture, Color color, Action onClick, float scale = 1.0f) : base(texture, position)
    {
        _onClick = onClick;
        _color = color;
        _texture = texture;
        _scale = scale;
    }
    public override void Update()
    {
        if (!IsVisible) return;

        _color = IsMouseOver() ? Color.LightGray : Color.White; //Darker color if the mouse is over

        if (IsMouseOver() && IM.MouseClicked) //Call click method if the button is clicked
        {
            Click();
        }
    }

    public void SetText(string text, Color textColor, float scale = 1.0f) //Setting the text on the button if it has any
    {
        if (Globals.Font == null)
            return;

        if (_buttonText == null) //If button text is null create one
        {
            _buttonText = new Text(Globals.Font, text, Vector2.Zero, textColor, scale);
            _buttonText.IsVisible = true;
        }
        else
        {
            _buttonText.SetText(text);
            _buttonText.Scale = new Vector2(scale, scale);
        }

        if (!_useManualTextPosition) //Automatically center the text on the button
        {
            CenterText();
        }
    }

    public void SetTextPosition(Vector2 position) //If the text should have a specific position adjust position manually
    {
        if (_buttonText != null)
        {
            _useManualTextPosition = true;
            _buttonText.Position = position;
        }
    }

    private void CenterText() //Centering the text on the button
    {
        if (_buttonText != null && _texture != null)
        {
            Vector2 textSize = Globals.Font.MeasureString(_buttonText.GetText()) * _buttonText.Scale.X;

            //Calculating the center of the button
            float buttonCenterX = Position.X + (_texture.Width * _scale / 2);
            float buttonCenterY = Position.Y + (_texture.Height * _scale / 2);

            //Positioning the text so its center aligns with the button's center
            Vector2 centeredPosition = new Vector2(buttonCenterX - (textSize.X / 2), buttonCenterY - (textSize.Y / 2f));
            _buttonText.Position = centeredPosition;
        }
    }
    public bool IsMouseOver() //Check if the mouse is over the button
    {
        Rectangle buttonRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(_texture.Width * _scale), (int)(_texture.Height * _scale));
        bool isOver = buttonRect.Contains((int)IM.MousePosition.X, (int)IM.MousePosition.Y);

        return isOver;
    }

    public void Click() //Invoking click event
    {
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
}