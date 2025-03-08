using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public abstract class UIElement
    {
        public Vector2 Position { get;  set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public bool IsVisible { get; set; } = true;

        protected const float UILayer = 1f;

        public Texture2D Texture;

        protected UIElement(Texture2D? texture, Vector2 position)
        {
            Texture = texture;
            Position = position;

         //   Scale = scale ?? (texture != null ? new Vector2(texture.Width, texture.Height) : Vector2.One);
        }

        public abstract void Update();
        public abstract void Draw();
    }
}
