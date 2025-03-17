using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    //Base class for every UI element in the game
    public abstract class UIElement : ILayerable, IUpdatable, IDrawable
    {
        public float LayerDepth { get; set; } = 0.0f; //UI should be at the front
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
        }
        public virtual void UpdateLayerDepth()
        {
            LayerDepth = 0.0f; //UI always at the front
        }
        public abstract void Update();
        public abstract void Draw();
    }
}
