using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class CameraFrame : UIElement, IClickable
    {
        private Vector2 Origin { get; }

        private Rectangle _bounds;

        private List<Fish> _fishes;
        public CameraFrame(Vector2 position, List<Fish> fishes) : base(Globals.Content.Load<Texture2D>("UI/CameraFrame"), position)
        {

            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            Scale = new Vector2(0.5f, 0.5f);
            _bounds = new(0, 0, Texture.Width, Texture.Height);
            IsVisible = false;
            _fishes = fishes;
        }



        public override void Update()
        {
            Position = IM.MousePosition;
        }

        public override void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, _bounds, Color.White, 0, Origin, Scale, SpriteEffects.None, UILayer);
        }

        public void Click()
        {
            throw new NotImplementedException();
        }

        public bool IsMouseOver(Rectangle bounds)
        {
            foreach (var fish in _fishes)
            {
                if (bounds.Intersects(fish.Bounds))
                { return true; }
            }
            return false;
        }
    }
}
