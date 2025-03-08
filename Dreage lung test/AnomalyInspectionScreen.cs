using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dredge_lung_test
{
    public class AnomalyInspectionScreen : UIElement, IClickable
    {
        public Fish _inspectedFish;
        private Button _deadlyButton;
        private Button _nonDeadlyButton;
        private Button _closeButton;
        private Rectangle _bounds;
        private Vector2 _fishDisplayPosition;
        private Vector2 _fishDisplayScale;
        private GameManager _gameManager;
        private bool _decisionMade = false;

        public AnomalyInspectionScreen(GameManager gameManager) : base(Globals.Content.Load<Texture2D>("UI/InspectionBG"), new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2))
        {
            _gameManager = gameManager;
            IsVisible = false;

            // Calculate the bounds of the inspection screen
            int width = (int)(Texture.Width * Scale.X);
            int height = (int)(Texture.Height * Scale.Y);
            _bounds = new Rectangle(
                (int)(Position.X - width / 2),
                (int)(Position.Y - height / 2),
                width,
                height
            );

            // Calculate where to display the fish
            _fishDisplayPosition = new Vector2(Position.X, Position.Y - 50);

            // Initialize buttons (positions will be adjusted in SetupButtons())
            _deadlyButton = new Button(Vector2.Zero, Globals.Content.Load<Texture2D>("UI/DButton"), () => MakeDecision(true), Color.Red);
            _nonDeadlyButton = new Button(Vector2.Zero, Globals.Content.Load<Texture2D>("UI/DButton"), () => MakeDecision(false), Color.Green);
            _closeButton = new Button(Vector2.Zero, Globals.Content.Load<Texture2D>("UI/DButton"), Close, Color.Black);

            SetupButtons();
        }

        private void SetupButtons()
        {
            // Position the buttons at the bottom of the screen
            int buttonSpacing = 50;
            int buttonsY = (int)(Position.Y + 100);

            _deadlyButton.Position = new Vector2(Position.X - buttonSpacing, buttonsY);
            _nonDeadlyButton.Position = new Vector2(Position.X + buttonSpacing, buttonsY);
            _closeButton.Position = new Vector2(Position.X, Position.Y + 150);
        }

        public void Open(Fish fish)
        {
            _inspectedFish = fish;
            _decisionMade = false;
            IsVisible = true;

            // Calculate scale to display fish properly
            _fishDisplayScale = new Vector2(0.5f, 0.5f); // Adjust as needed
        }

        private void Close()
        {
            IsVisible = false;
            _inspectedFish = null;
        }

        private void MakeDecision(bool playerSaysDeadly)
        {
            if (_decisionMade || _inspectedFish == null)
                return;

            bool isCorrect = (playerSaysDeadly == _inspectedFish.HasDeadlyAnomaly);

            // Record the player's decision
            _gameManager.RecordAnomalyDecision(isCorrect);

            // Show feedback before closing
            _decisionMade = true;

            // Close the screen after a delay (you might want to show feedback first)
            // In a real implementation, you'd use a timer, but for simplicity:
            Close();
        }

        public override void Update()
        {
            if (!IsVisible)
                return;

            _deadlyButton.Update();
            _nonDeadlyButton.Update();
            _closeButton.Update();
        }

        public override void Draw()
        {
            if (!IsVisible)
                return;

            // Draw background
            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.White,
                0f,
                new Vector2(Texture.Width / 2, Texture.Height / 2),
                Scale,
                SpriteEffects.None,
                UILayer
            );

            // Draw the fish if available
            if (_inspectedFish != null)
            {
                // Draw fish at the center of the inspection screen
                Rectangle fishSource = _inspectedFish.SourceRect;

                // Draw the fish without anomalies first
                Globals.SpriteBatch.Draw(
                    _inspectedFish.Texture,
                    _fishDisplayPosition,
                    fishSource,
                    Color.White,
                    0f,
                    new Vector2(fishSource.Width / 2, fishSource.Height / 2),
                    _fishDisplayScale,
                    SpriteEffects.None,
                    UILayer + 0.01f
                );

                // Draw each anomaly over the fish
                foreach (var anomaly in _inspectedFish.Anomalies)
                {
                    anomaly.Draw(_fishDisplayPosition, _fishDisplayScale, SpriteEffects.None, UILayer + 0.02f);
                }

                // Draw decision prompt text
                string promptText = "Does this fish have a deadly anomaly?";
                Vector2 textSize = Globals.Font.MeasureString(promptText);
                Globals.SpriteBatch.DrawString(
                    Globals.Font,
                    promptText,
                    new Vector2(Position.X - textSize.X / 2, Position.Y + 50),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    UILayer + 0.03f
                );

                // If decision was made, show feedback
                if (_decisionMade)
                {
                    bool isCorrect = (_inspectedFish.HasDeadlyAnomaly == true); // Placeholder
                    string feedbackText = isCorrect ? "Correct!" : "Incorrect!";
                    Color feedbackColor = isCorrect ? Color.Green : Color.Red;

                    Vector2 feedbackSize = Globals.Font.MeasureString(feedbackText);
                    Globals.SpriteBatch.DrawString(
                        Globals.Font,
                        feedbackText,
                        new Vector2(Position.X - feedbackSize.X / 2, Position.Y + 20),
                        feedbackColor,
                        0f,
                        Vector2.Zero,
                        1.5f,
                        SpriteEffects.None,
                        UILayer + 0.04f
                    );
                }
            }

            // Draw buttons
            _deadlyButton.Draw();
            _nonDeadlyButton.Draw();
            _closeButton.Draw();
        }

        public bool IsMouseOver(Rectangle bounds)
        {
            return IsVisible && _bounds.Contains(bounds.Center);
        }

        public void Click()
        {
            // Handle clicks within the screen's area
            if (!IsVisible)
                return;

            if (_deadlyButton.IsMouseOver(IM.Cursor))
            {
                _deadlyButton.Click();
            }
            else if (_nonDeadlyButton.IsMouseOver(IM.Cursor))
            {
                _nonDeadlyButton.Click();
            }
            else if (_closeButton.IsMouseOver(IM.Cursor))
            {
                _closeButton.Click();
            }
        }
    }
}