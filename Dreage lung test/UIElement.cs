using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public abstract class UIElement : ILayerable, IUpdatable, IDrawable
    {
        public float LayerDepth { get; set; } = 0.0f; // UI should be at the front (0.0f)
        public int ZIndex { get; set; } = 0;
        public Vector2 Position { get;  set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public bool IsVisible { get; set; } = true;

        protected const float UILayer = 1f;

        protected readonly Texture2D Texture;

        protected UIElement(Texture2D? texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            ZIndex = 0;
            UpdateLayerDepth();


            //   Scale = scale ?? (texture != null ? new Vector2(texture.Width, texture.Height) : Vector2.One);
        }
        public virtual void UpdateLayerDepth()
        {
            // UI elements are always at the front (0.0f is foreground in XNA/MonoGame)
            LayerDepth = 0.0f;
        }
        public abstract void Update();
        public abstract void Draw();
    }
}
