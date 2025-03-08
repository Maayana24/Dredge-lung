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
        private List<Fish> _detectedFish = new List<Fish>();
        private GameManager _gameManager;

        public CameraFrame(Vector2 position, List<Fish> fishes, GameManager gameManager) : base(Globals.Content.Load<Texture2D>("UI/CameraFrame"), position)
        {
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            Scale = new Vector2(0.5f, 0.5f);
            _bounds = new(0, 0, Texture.Width, Texture.Height);
            IsVisible = false;
            _fishes = fishes;
            _gameManager = gameManager;
        }

        public override void Update()
        {
            Position = IM.MousePosition;
            _detectedFish.Clear();

            if (IsVisible)
            {
                foreach (var fish in _fishes)
                {
                    if (IsFishInFrame(fish))
                    {
                        _detectedFish.Add(fish);
                    }
                }
            }
        }

        public override void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, _bounds, Color.White, 0, Origin, Scale, SpriteEffects.None, UILayer);
        }

        public void Click()
        {
            if (IsVisible && _detectedFish.Count > 0)
            {
                // Open the inspection screen for the first detected fish
                // In a more complex implementation, you might want to handle multiple fish differently
                Fish selectedFish = _detectedFish[0];

                // Open the fish inspection screen
                _gameManager.InspectFish(selectedFish);

                // Hide the camera frame after capturing
                IsVisible = false;
            }
        }

        private bool IsFishInFrame(Fish fish)
        {
            // Calculate the actual bounds of the camera frame based on position, origin and scale
            Rectangle frameBounds = new Rectangle(
                (int)(Position.X - Origin.X * Scale.X),
                (int)(Position.Y - Origin.Y * Scale.Y),
                (int)(Texture.Width * Scale.X),
                (int)(Texture.Height * Scale.Y)
            );
            return frameBounds.Intersects(fish.Bounds);
        }

        public bool IsMouseOver(Rectangle bounds)
        {
            if (!IsVisible)
                return false;

            foreach (var fish in _fishes)
            {
                if (IsFishInFrame(fish) && bounds.Intersects(fish.Bounds))
                {
                    return true;
                }
            }
            return false;
        }

        public List<Fish> GetDetectedFish()
        {
            return _detectedFish;
        }
    }
}