using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace Dredge_lung_test
{
    public class Button : UIElement, IClickable
    {
        private Color _shade = Color.White;
        private readonly Rectangle _bounds;
        private readonly Action _onClick;

        public Button(Vector2 position, Texture2D texture, Action onClick) : base(texture, position)
        {
            _bounds = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            _onClick = onClick;
        }

        public override void Update()
        {
            if (IsMouseOver(_bounds))
            {
                if (IM.MouseClicked)
                {
                    _onClick?.Invoke();
                }
                else
                {
                    _shade = Color.DarkGray;
                }
            }
            else
            {
                _shade = Color.White;
            }
        }

        public override void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, null, _shade, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, UILayer);
        }

        public void Click()
        {
        }

        public bool IsMouseOver(Rectangle bounds)
        {
            return IM.Cursor.Intersects(bounds);
        }
    }
}