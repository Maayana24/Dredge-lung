using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dredge_lung_test
{
    public class FishInspectionScreen : UIElement, IClickable
    {
        private Fish _inspectedFish;
        private Button _deadlyButton;
        private Button _notDeadlyButton;
        private bool _isActive = false;
        private readonly Vector2 _windowSize;
        private readonly Vector2 _modalOrigin;
        private readonly Vector2 _fishDisplayPosition;
        private readonly float _fishDisplayScale = 1.5f;
        private Rectangle _modalBounds;

        // Statistics tracking
        private int _correctAnswers = 0;
        private int _incorrectAnswers = 0;

        public FishInspectionScreen() : base(Globals.Content.Load<Texture2D>("UI/InspectionBG"), Vector2.Zero)
        {
            // Modal window size - make it smaller than the screen
            _windowSize = new Vector2(900, 1400); // Slightly smaller than the 1080x1920 screen

            // Always position at center of screen, regardless of fish position
            Position = new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2);
            _modalOrigin = new Vector2(_windowSize.X / 2, _windowSize.Y / 2);

            // Fish display position (centered in the upper part of the modal)
            _fishDisplayPosition = new Vector2(Position.X, Position.Y - 400);

            // Set modal bounds
            _modalBounds = new Rectangle(
                (int)(Position.X - _modalOrigin.X),
                (int)(Position.Y - _modalOrigin.Y),
                (int)_windowSize.X,
                (int)_windowSize.Y);

            // Create buttons using textures from content
            _deadlyButton = CreateDeadlyButton();
            _notDeadlyButton = CreateNotDeadlyButton();

            IsVisible = false;
        }

        private Button CreateDeadlyButton()
        {
            // Load button texture from content
            Texture2D buttonTexture = Globals.Content.Load<Texture2D>("UI/Button");
            Vector2 buttonPosition = new Vector2(Position.X - 150, Position.Y + 200);

            return new Button(buttonPosition, buttonTexture, Color.Red, () =>
            {
                System.Diagnostics.Debug.WriteLine("Deadly button clicked");
                EvaluateAnswer(true);
                CloseInspection();
            });
        }

        private Button CreateNotDeadlyButton()
        {
            // Load button texture from content
            Texture2D buttonTexture = Globals.Content.Load<Texture2D>("UI/Button");
            Vector2 buttonPosition = new Vector2(Position.X + 50, Position.Y + 100);

            return new Button(buttonPosition, buttonTexture, Color.Green, () =>
            {
                System.Diagnostics.Debug.WriteLine("Not deadly button clicked");
                EvaluateAnswer(false);
                CloseInspection();
            });
        }

        public void OpenInspection(Fish fish)
        {
            _inspectedFish = fish;
            IsVisible = true;
            _isActive = true;

            // Add debug info
            System.Diagnostics.Debug.WriteLine("Inspection screen opened");
        }

        private void CloseInspection()
        {
            IsVisible = false;
            _isActive = false;
            _inspectedFish = null;

            // Add debug info
            System.Diagnostics.Debug.WriteLine("Inspection screen closed");
        }

        private void EvaluateAnswer(bool selectedDeadly)
        {
            bool isCorrect = (selectedDeadly == _inspectedFish.HasDeadlyAnomaly);

            if (isCorrect)
            {
                _correctAnswers++;
                System.Diagnostics.Debug.WriteLine("Correct answer! The fish " +
                    (selectedDeadly ? "has" : "does not have") + " a deadly anomaly.");
            }
            else
            {
                _incorrectAnswers++;
                System.Diagnostics.Debug.WriteLine("Incorrect answer! The fish " +
                    (_inspectedFish.HasDeadlyAnomaly ? "has" : "does not have") + " a deadly anomaly.");
            }

            System.Diagnostics.Debug.WriteLine($"Stats: Correct: {_correctAnswers}, Incorrect: {_incorrectAnswers}");
        }

        public override void Update()
        {
            if (!IsVisible) return;

            _deadlyButton.Update();
            _notDeadlyButton.Update();
        }

        public override void Draw()
        {
            if (!IsVisible) return;

            // Add debug info
            System.Diagnostics.Debug.WriteLine("Drawing inspection screen");

            // Draw modal background
            Globals.SpriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.White * 0.95f,
                0f,
                _modalOrigin,
                new Vector2(_windowSize.X / Texture.Width, _windowSize.Y / Texture.Height),
                SpriteEffects.None,
                0.9f);

            // Draw the fish with its anomalies
            if (_inspectedFish != null)
            {
                DrawFishWithAnomalies();
            }

            // Draw buttons with higher layer depth to ensure they're on top
            _deadlyButton.Draw();
            _notDeadlyButton.Draw();

            // Draw text labels
            DrawText("Deadly", _deadlyButton.Position, Color.White);
            DrawText("Not Deadly", _notDeadlyButton.Position, Color.White);
            DrawText("Inspect Fish", new Vector2(Position.X, Position.Y - 600), Color.Black);
        }

        private void DrawFishWithAnomalies()
        {
            // Draw the base fish
            Fish fish = _inspectedFish;

            // Get the source rect from the fish
            Rectangle sourceRect = GetSourceRectFromFish(fish);

            // Draw the base fish
            Globals.SpriteBatch.Draw(
                fish.Texture,
                _fishDisplayPosition,
                sourceRect,
                Color.White,
                0,
                new Vector2(sourceRect.Width / 2, sourceRect.Height / 2),
                fish.Scale * _fishDisplayScale,
                fish.SpriteEffect,
                0.91f
            );

            // Draw all anomalies on the fish
            foreach (var anomaly in fish.Anomalies)
            {
                anomaly.Draw(_fishDisplayPosition, fish.Scale * _fishDisplayScale, fish.SpriteEffect, 0.92f);
            }
        }

        private Rectangle GetSourceRectFromFish(Fish fish)
        {
            // Try to get sourceRect property using reflection since it's protected
            var sourceRectProperty = fish.GetType().GetProperty("SourceRect",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.GetProperty);

            if (sourceRectProperty != null)
            {
                return (Rectangle)sourceRectProperty.GetValue(fish);
            }

            // Fallback if we can't get it - return a default rectangle
            return new Rectangle(0, 0, fish.Texture.Width, fish.Texture.Height);
        }

        private void DrawText(string text, Vector2 position, Color color)
        {
            // Assuming you have a SpriteFont loaded in Globals or UIManager
            if (Globals.Font != null)
            {
                Vector2 textSize = Globals.Font.MeasureString(text);
                Globals.SpriteBatch.DrawString(
                    Globals.Font,
                    text,
                    new Vector2(position.X - textSize.X / 2, position.Y - textSize.Y / 2),
                    color,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.95f);
            }
        }

        public bool IsMouseOver(Rectangle cursorBounds)
        {
            if (!IsVisible) return false;

            // Debug info
            bool isOver = _modalBounds.Contains(cursorBounds.Center);
            if (isOver)
            {
                System.Diagnostics.Debug.WriteLine("Mouse is over inspection screen");
            }

            return isOver;
        }

        public void Click()
        {
            System.Diagnostics.Debug.WriteLine("Inspection screen clicked");

            // Check if clicking on deadly button
            if (_deadlyButton.IsMouseOver(IM.Cursor))
            {
                System.Diagnostics.Debug.WriteLine("Clicking deadly button");
                _deadlyButton.Click();
                return;
            }

            // Check if clicking on not deadly button
            if (_notDeadlyButton.IsMouseOver(IM.Cursor))
            {
                System.Diagnostics.Debug.WriteLine("Clicking not deadly button");
                _notDeadlyButton.Click();
                return;
            }
        }

        // Add getters for statistics if needed
        public int GetCorrectAnswers() => _correctAnswers;
        public int GetIncorrectAnswers() => _incorrectAnswers;
    }
}