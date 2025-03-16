using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public class UIManager : IUpdatable, IDrawable
    {
        private readonly List<UIElement> _uiElements = new List<UIElement>();
        private readonly List<IClickable> _clickables = new List<IClickable>();
        private List<Fish> _fishes;

        public event EventHandler ReplayClicked;

        // UI text elements
        public Text ScoreText { get; private set; }
        public Text LivesText { get; private set; }
        public Text CooldownText { get; private set; }
        public Text GameOverText { get; private set; }
        public Text HighScoreText { get; private set; }

        // Button elements
        public Button ReplayButton { get; private set; }

        public UIManager(List<Fish> fishes)
        {
            _fishes = fishes;
        }

        public void SetUpUI()
        {
            // Get screen dimensions
            float screenWidth = Globals.ScreenWidth;
            float screenHeight = Globals.ScreenHeight;

            // Create score text in the top right (85% of screen width)
            ScoreText = AddText(
                new Vector2(screenWidth * 0.85f, screenHeight * 0.05f),
                "Score: 0",
                Color.White,
                1.5f);

            // Create high score text (slight offset below score)
            HighScoreText = AddText(
                new Vector2(screenWidth * 0.79f, screenHeight * 0.11f),
                "High Score: 0",
                Color.Gold,
                1.5f);

            // Create lives text in the top left (1% from left edge)
            LivesText = AddText(
                new Vector2(screenWidth * 0.01f, screenHeight * 0.05f),
                "Lives: 3",
                Color.White,
                1.5f);

            // Create cooldown text at the top center
            CooldownText = AddText(
                new Vector2(screenWidth * 0.5f - 180, screenHeight * 0.05f),
                "",
                Color.Yellow,
                1.3f);
            CooldownText.IsVisible = false;

            // Create game over text (initially invisible)
            GameOverText = AddText(
                new Vector2(0, 0),
                "GAME OVER",
                Color.Red,
                2);
            GameOverText.IsVisible = false;

            // Center the game over text properly
            Vector2 gameOverSize = Globals.Font.MeasureString("GAME OVER") * 2; // Account for scale
            GameOverText.Position = new Vector2(
                (screenWidth - gameOverSize.X) / 2,
                (screenHeight - gameOverSize.Y) / 2 - screenHeight * 0.07f); // Position higher (7% of screen)

            // Initialize the ReplayButton with a smaller scale
            Texture2D buttonTexture = Globals.Content.Load<Texture2D>("UI/Button");
            float buttonScale = 0.4f;

            // Position the button below the game over text
            ReplayButton = AddButton(
        new Vector2(
            (screenWidth - buttonTexture.Width * buttonScale) / 2,
            (screenHeight - buttonTexture.Height * buttonScale) / 2 + screenHeight * 0.07f), // 7% below center
        buttonTexture,
        Color.White,
        OnReplayClicked,
        buttonScale);

            ReplayButton.LayerDepth = 0.9f;
            ReplayButton.SetText("Play Again", Color.Black, 1.2f);
            ReplayButton.IsVisible = false;

            // Debug to verify button setup
            Debug.WriteLine($"ReplayButton added to clickables: {_clickables.Contains(ReplayButton)}");
            Debug.WriteLine($"ReplayButton bounds: {((Button)ReplayButton)._bounds}");
        }

        // Callback for replay button
        private void OnReplayClicked()
        {
            Debug.WriteLine("OnReplayClicked method called");

            // If there are any subscribers, invoke the event
            if (ReplayClicked != null)
            {
                Debug.WriteLine("Invoking ReplayClicked event");
                ReplayClicked.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Debug.WriteLine("WARNING: ReplayClicked event has no subscribers!");
            }
        }

        public void UpdateScoreText(int score)
        {
            ScoreText.SetText($"Score: {score}");
        }

        public void UpdateHighScoreText(int highScore)
        {
            HighScoreText.SetText($"High Score: {highScore}");
        }

        public void UpdateLivesText(int lives)
        {
            LivesText.SetText($"Lives: {lives}");
        }

        public void UpdateCooldownText(bool isInCooldown, int remainingTime)
        {
            if (isInCooldown)
            {
                CooldownText.SetText($"Harpoon Cooldown: {remainingTime}");
                CooldownText.IsVisible = true;
            }
            else
            {
                CooldownText.IsVisible = false;
            }
        }

        public void ShowGameOver(bool isGameOver)
        {
            Debug.WriteLine($"ShowGameOver: {isGameOver}");
            GameOverText.IsVisible = isGameOver;
            ReplayButton.IsVisible = isGameOver;

            if (isGameOver)
            {
                Debug.WriteLine($"Game over UI elements are now visible - Button position: {ReplayButton.Position}, Button visible: {ReplayButton.IsVisible}");

                // Force the button to be in front
                ReplayButton.LayerDepth = 0.99f;
                if (ReplayButton is Button btn && btn._labelText != null)
                {
                    btn._labelText.LayerDepth = 0.995f;
                }
            }
        }

        // Button creation method
        public Button AddButton(Vector2 position, Texture2D texture, Color color, Action onClick, float scale = 1.0f)
        {
            var button = new Button(position, texture, color, onClick, scale);
            _uiElements.Add(button);
            _clickables.Add(button);
            return button;
        }

        // Text creation method
        public Text AddText(Vector2 position, string text, Color color, float scale)
        {
            Text textElement = new Text(Globals.Font, text, position, color, scale);
            AddElement(textElement);
            return textElement;
        }

        public void Update()
        {
            // Update all UI elements
            foreach (var element in _uiElements)
            {
                if (element.IsVisible)
                {
                    element.Update();
                }
            }

            // Add debug to verify check for mouse clicks is happening

            // Process clicks
            if (IM.MouseClicked)
            {
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            // Debug output for mouse click
            Debug.WriteLine("ProcessMouseClick method called!");
            Debug.WriteLine($"Mouse clicked at: {IM.MousePosition}");
            Debug.WriteLine($"Number of clickables: {_clickables.Count}");

            // Additional debug to help diagnose the issue
            Debug.WriteLine("Clickable elements:");
            foreach (var clickable in _clickables)
            {
                if (clickable is UIElement element)
                {
                    bool isMouseOver = clickable.IsMouseOver();
                    Debug.WriteLine($"- {element.GetType().Name}: Visible={element.IsVisible}, MouseOver={isMouseOver}, Position={element.Position}");
                }
            }

            // Process click events for all visible clickable elements
            foreach (var clickable in _clickables)
            {
                // Check if the element is visible and if it's a UIElement
                if (clickable is UIElement element && element.IsVisible)
                {
                    if (clickable.IsMouseOver())
                    {
                        Debug.WriteLine($"CLICKING ELEMENT: {element.GetType().Name}");
                        clickable.Click();
                        break; // Process only the topmost element
                    }
                }
            }
        }

        public void Draw()
        {
            // Draw all visible UI elements
            foreach (var element in _uiElements)
            {
                if (element.IsVisible)
                {
                    element.Draw();
                }
            }
        }

        public void AddElement(UIElement element)
        {
            _uiElements.Add(element);
            if (element is IClickable clickable)
            {
                _clickables.Add(clickable);
            }
        }

        public void RemoveElement(UIElement element)
        {
            _uiElements.Remove(element);
            if (element is IClickable clickable)
            {
                _clickables.Remove(clickable);
            }
        }
    }
}