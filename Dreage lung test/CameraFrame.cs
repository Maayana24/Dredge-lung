using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dredge_lung_test
{
    public class CameraFrame : UIElement, IClickable
    {
        private Vector2 Origin { get; }
        private Rectangle _frameBounds;
        private readonly List<Fish> _fishes;
        private readonly Action<Fish> _onFishCaptured;

        // Visual feedback
        private bool _isCapturing;
        private float _captureTimer;
        private const float CaptureEffectDuration = 0.2f;

        public CameraFrame(Vector2 position, List<Fish> fishes, Action<Fish> onFishCaptured = null) : base(Globals.Content.Load<Texture2D>("UI/CameraFrame"), position)
        {
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            Scale = new Vector2(0.5f, 0.5f);
            _fishes = fishes;
            _onFishCaptured = onFishCaptured;
            IsVisible = false;
            UpdateFrameBounds();
        }

        public override void Update()
        {
            Position = IM.MousePosition;
            UpdateFrameBounds();

            // Handle capture effect timer
            if (_isCapturing)
            {
                _captureTimer -= Globals.DeltaTime;
                if (_captureTimer <= 0)
                {
                    _isCapturing = false;
                }
            }
        }

        private void UpdateFrameBounds()
        {
            // Update the actual frame rectangle based on current position and scale
            _frameBounds = new Rectangle(
                (int)(Position.X - Origin.X * Scale.X),
                (int)(Position.Y - Origin.Y * Scale.Y),
                (int)(Texture.Width * Scale.X),
                (int)(Texture.Height * Scale.Y)
            );
        }

        public override void Draw()
        {
            Color frameColor = _isCapturing ? Color.LightYellow : Color.White;
            Globals.SpriteBatch.Draw(Texture, Position, null, frameColor, 0, Origin, Scale, SpriteEffects.None, UILayer);
        }

        public void Click()
        {
            if (!IsVisible)
                return;

            // Capture effect
            _isCapturing = true;
            _captureTimer = CaptureEffectDuration;

            // Check for fish in frame
            var capturedFish = GetFishInFrame();
            if (capturedFish.Count > 0)
            {
                var fish = capturedFish[0]; // Take the first fish if multiple in frame
                if (!fish.HasBeenPhotographed)
                {
                    fish.OnPhotographed();
                    _onFishCaptured?.Invoke(fish);
                }
            }
        }

        private List<Fish> GetFishInFrame()
        {
            return _fishes.Where(fish => fish.IsInFrame(_frameBounds) && !fish.HasBeenPhotographed).ToList();
        }

        public bool IsMouseOver(Rectangle bounds)
        {
            // This checks if the cursor is over this UI element
            return bounds.Contains(IM.MousePosition);
        }

        public bool AnyFishInFrame()
        {
            return _fishes.Any(fish => fish.IsInFrame(_frameBounds) && !fish.HasBeenPhotographed);
        }
    }
}
