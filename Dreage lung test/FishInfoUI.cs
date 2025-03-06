using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dredge_lung_test
{
    public class FishInfoUI : UIElement
    {
        private readonly Fish _fish;
        private readonly Action _onClose;
        private Button _closeButton;

        public FishInfoUI(Fish fish, Action onClose) : base(Globals.Content.Load<Texture2D>("UI/Panel"), Vector2.Zero) //add texture
        {
            _fish = fish;
            _onClose = onClose;
            IsVisible = true;


            // Center on screen
            Position = new Vector2(
                Globals.ScreenWidth / 2f,
                Globals.ScreenHeight / 2f
            );

            // Set up close button
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Create close button
            _closeButton = new Button(
                new Vector2(Position.X + 150, Position.Y - 100),
                Globals.Content.Load<Texture2D>("UI/CloseButton"),
                () => {
                    IsVisible = false;
                    _onClose?.Invoke();
                }
            );
        }

        public override void Update()
        {
            if (!IsVisible) return;

            _closeButton.Update();
        }

        public override void Draw()
        {
            if (!IsVisible) return;

            // Draw darkened background
            Globals.SpriteBatch.Draw(
                Globals.Content.Load<Texture2D>("UI/Pixel"), // 1x1 white pixel texture
                new Rectangle(0, 0, Globals.ScreenWidth, Globals.ScreenHeight),
                Color.Black * 0.7f // Semi-transparent black
            );

            // Draw panel background
            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.White,
                0f,
                new Vector2(Texture.Width / 2f, Texture.Height / 2f),
                1.0f,
                SpriteEffects.None,
                0.9f
            );

            // Draw fish image
            Globals.SpriteBatch.Draw(
                _fish.Texture,
                new Vector2(Position.X - 120, Position.Y),
                null,
                Color.White,
                0f,
                new Vector2(_fish.Texture.Width / 2f, _fish.Texture.Height / 2f),
                0.8f,
                SpriteEffects.None,
                0.91f
            );

            // Draw fish info text
            string fishType = _fish.GetType().Name;
            string fishValue = $"Value: {_fish.Value} points";

            SpriteFont font = Globals.Content.Load<SpriteFont>("DefaultFont");

            Globals.SpriteBatch.DrawString(
                font,
                fishType,
                new Vector2(Position.X + 20, Position.Y - 60),
                Color.White,
                0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0.92f
            );

            Globals.SpriteBatch.DrawString(
                font,
                fishValue,
                new Vector2(Position.X + 20, Position.Y - 30),
                Color.White,
                0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0.92f
            );

            // Draw additional info based on fish type
            if (_fish is Angler)
            {
                DrawFishSpecificInfo("Angler fish are deep sea predators...");
            }
/*            else if (_fish is FloaterFish)
            {
                DrawFishSpecificInfo("Floater fish move vertically through the water...");
            }
            else if (_fish is WavyFish)
            {
                DrawFishSpecificInfo("Wavy fish are known for their undulating movement...");
            }*/

            _closeButton.Draw();
        }

        private void DrawFishSpecificInfo(string info)
        {
            SpriteFont font = Globals.Content.Load<SpriteFont>("DefaultFont");

            Globals.SpriteBatch.DrawString(
                font,
                info,
                new Vector2(Position.X + 20, Position.Y),
                Color.White,
                0f,
                Vector2.Zero,
                0.8f,
                SpriteEffects.None,
                0.92f
            );
        }
    }
}