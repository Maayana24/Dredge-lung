using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public class Button : UIElement, IClickable
    {
        private readonly Action _onClick;
        private Color _color;
        private readonly float _scale;
        public Rectangle _bounds;
        public Text _labelText;
        private bool _isHovered;

        public Button(Vector2 position, Texture2D texture, Color color, Action onClick, float scale = 1.0f)
            : base(texture, position)
        {
            _onClick = onClick;
            _color = color;
            _scale = scale;
            Scale = new Vector2(scale);
            IsVisible = true;

            // Set even higher layer depth to ensure visibility
            LayerDepth = 0.95f;

            // Calculate bounds based on texture size and scale
            if (Texture != null)
            {
                _bounds = new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)(Texture.Width * scale),
                    (int)(Texture.Height * scale)
                );
            }

            Debug.WriteLine($"Button created at position: {position}, with bounds: {_bounds}, LayerDepth: {LayerDepth}");
        }

        public void SetText(string text, Color textColor, float textScale = 1.0f)
        {
            // Create text object if it doesn't exist
            if (_labelText == null)
            {
                _labelText = new Text(Globals.Font, text, Vector2.Zero, textColor, textScale);
                // Make text layer depth slightly higher than button
                _labelText.LayerDepth = 0.96f;
            }
            else
            {
                _labelText.SetText(text);
                _labelText.Color = textColor;
                _labelText.SetScale(textScale);
            }

            // Center text on button
            CenterText();

            Debug.WriteLine($"Button text set: '{text}', Position: {_labelText.Position}, Button Position: {Position}");
        }

        private void CenterText()
        {
            if (_labelText != null && Texture != null)
            {
                // Measure text dimensions
                Vector2 textSize = Globals.Font.MeasureString(_labelText.Content) * _labelText.Scale;

                // Calculate center of button
                Vector2 buttonCenter = new Vector2(
                    Position.X + (Texture.Width * _scale) / 2,
                    Position.Y + (Texture.Height * _scale) / 2
                );

                // Center text position
                _labelText.Position = new Vector2(
                    buttonCenter.X - textSize.X / 2,
                    buttonCenter.Y - textSize.Y / 2
                );

                // Ensure text has proper visibility and layer depth
                _labelText.IsVisible = IsVisible;
                _labelText.LayerDepth = LayerDepth + 0.01f; // Slightly higher than button

                Debug.WriteLine($"Text positioned at: {_labelText.Position} with size {textSize}");
                Debug.WriteLine($"Button center: {buttonCenter}, Button bounds: {_bounds}");
            }
        }

        public override void Update()
        {
            // Check if button is visible
            if (!IsVisible)
                return;

            // Check if mouse is over the button
            _isHovered = IsMouseOver();

            // Add mouse position debugging every frame
            Debug.WriteLine($"Mouse position: {IM.MousePosition}, Button bounds: {_bounds}, IsHovered: {_isHovered}, IsVisible: {IsVisible}");

            // Check for click
            if (_isHovered && IM.MouseClicked)
            {
                Debug.WriteLine("CLICK DETECTED ON BUTTON!");
                Click();
            }

            // Optional: Apply hover effect (like slight color change)
            if (_isHovered)
            {
                // Lighten the color when hovered
                _color = new Color(
                    (int)MathHelper.Min(255, _color.R * 1.2f),
                    (int)MathHelper.Min(255, _color.G * 1.2f),
                    (int)MathHelper.Min(255, _color.B * 1.2f),
                    _color.A
                );
            }
            else
            {
                // Reset to original color when not hovered
                _color = Color.White;
            }

            // Update text if needed
            if (_labelText != null)
            {
                _labelText.Update();
            }
        }

        public override void Draw()
        {
            if (!IsVisible) return;

            // Draw the button
            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                null,
                _color,
                0f,
                Vector2.Zero,
                _scale,
                SpriteEffects.None,
                LayerDepth
            );

            // Draw the text label if present
            if (_labelText != null && _labelText.IsVisible)
            {
                _labelText.Draw();
                Debug.WriteLine($"Drawing button text at {_labelText.Position} with depth {_labelText.LayerDepth}");
            }
        }

        public void Click()
        {
            Debug.WriteLine("Button.Click() method called - invoking onClick action");
            _onClick?.Invoke();
        }

        public bool IsMouseOver()
        {
            bool result = IsVisible && _bounds.Contains(IM.MousePosition);
            Debug.WriteLine($"IsMouseOver check - Position: {IM.MousePosition}, Bounds: {_bounds}, Result: {result}");
            return result;
        }

        // Update the button position (and recalculate bounds)
        public void SetPosition(Vector2 newPosition)
        {
            Position = newPosition;

            // Update bounds
            _bounds = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                (int)(Texture.Width * _scale),
                (int)(Texture.Height * _scale)
            );

            // Update text position if it exists
            CenterText();
        }
    }
}