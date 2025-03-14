using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class UIManager
    {
        private readonly List<UIElement> _uiElements = new List<UIElement>();
        private readonly List<IClickable> _clickables = new List<IClickable>();
        private List<Fish> _fishes;

        // UI text elements
        public Text ScoreText { get; private set; }
        public Text LivesText { get; private set; }
        public Text CooldownText { get; private set; }
        public Text GameOverText { get; private set; }

        public Text HighScoreText { get; private set; }

        public UIManager(List<Fish> fishes)
        {
            _fishes = fishes;
        }

        public void SetUpUI()
        {
            // Create score text in the top right
            ScoreText = AddText(new Vector2(850, 40), "Score: 0", Color.White, 1.5f);

            HighScoreText = AddText(new Vector2(730, 80), "High Score: 0", Color.Gold, 1.5f);

            // Create lives text in the top left
            LivesText = AddText(new Vector2(10, 40), "Lives: 3", Color.White, 1.5f);

            // Create cooldown text at the top center (initially invisible)
            CooldownText = AddText(new Vector2(Globals.ScreenWidth / 2 - 180, 40), "", Color.Yellow, 1.3f);
            CooldownText.IsVisible = false;

            // Create game over text (initially invisible)
            GameOverText = AddText(new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2), "GAME OVER", Color.Red, 2);
            GameOverText.IsVisible = false;

            // Center the game over text
            Vector2 gameOverSize = Globals.Font.MeasureString("GAME OVER");
            GameOverText.Position = new Vector2((Globals.ScreenWidth - gameOverSize.X) / 2, (Globals.ScreenHeight - gameOverSize.Y) / 2);
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
            GameOverText.IsVisible = isGameOver;
        }

        // Existing methods remain the same
        public Button AddButton(Vector2 position, Texture2D texture, Color color, Action onClick)
        {
            var button = new Button(position, texture, color, onClick);
            _uiElements.Add(button);
            _clickables.Add(button);
            return button;
        }

        public Text AddText(Vector2 position, string text, Color color, float scale)
        {
            Text textElement = new Text(Globals.Font, text, position, color, scale);
            AddElement(textElement);
            return textElement;
        }

        public void Update()
        {
            foreach (var element in _uiElements)
            {
                if (element.IsVisible)
                {
                    element.Update();
                }
            }
            if (IM.MouseClicked)
            {
                // System.Diagnostics.Debug.WriteLine("Mouse clicked at " + IM.Cursor.Center);
                // ProcessMouseClick();
            }
        }

        public void Draw()
        {
            // Sort elements by depth if needed for proper layering
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