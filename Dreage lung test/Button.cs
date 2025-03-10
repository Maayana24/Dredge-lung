using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dredge_lung_test
{
    public class Button : UIElement, IClickable
    {
        private Color _shade = Color.White;
        private Color _defaultShade = Color.White;
        private Rectangle _bounds;
        private readonly Action _onClick;

        public Button(Vector2 position, Texture2D texture, Color color, Action onClick) : base(texture, position)
        {
            _onClick = onClick;
            _defaultShade = color;
            _shade = color;
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            // Create bounds centered on the position
            _bounds = new Rectangle(
                (int)(Position.X - Texture.Width / 2),
                (int)(Position.Y - Texture.Height / 2),
                Texture.Width,
                Texture.Height);
        }

        public override void Update()
        {
            // Update bounds if position changed
            UpdateBounds();

            if (IsMouseOver(IM.Cursor))
            {
                // Slightly darken the color when hovering
                _shade = new Color(
                    (int)(_defaultShade.R * 0.8f),
                    (int)(_defaultShade.G * 0.8f),
                    (int)(_defaultShade.B * 0.8f),
                    _defaultShade.A);

                // Debug info
                System.Diagnostics.Debug.WriteLine($"Mouse over button at {Position}");
            }
            else
            {
                _shade = _defaultShade;
            }
        }

        public override void Draw()
        {
            // Draw using the center of the texture as the origin
            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                null,
                _shade,
                0f,
                new Vector2(Texture.Width / 2, Texture.Height / 2),
                Scale,
                SpriteEffects.None,
                0.95f);  // Use a high layer depth to ensure buttons are on top

            // Debug draw of button bounds
            /*
            var boundsTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            boundsTexture.SetData(new[] { Color.White });
            Globals.SpriteBatch.Draw(
                boundsTexture,
                _bounds,
                null,
                Color.Red * 0.3f,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.96f
            );
            */
        }

        public void Click()
        {
            // Debug info
            System.Diagnostics.Debug.WriteLine($"Button clicked at {Position}");
            _onClick?.Invoke();
        }

        public bool IsMouseOver(Rectangle cursorBounds)
        {
            return _bounds.Intersects(cursorBounds);
        }
    }
}