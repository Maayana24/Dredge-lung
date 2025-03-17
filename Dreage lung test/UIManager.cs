using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//Class to manage the game's UI
namespace Dredge_lung_test
{
    public class UIManager : IUpdatable, IDrawable
    {
        private static UIManager _instance;
        public static UIManager Instance => _instance ??= new UIManager();

        private readonly List<UIElement> _uiElements = new List<UIElement>();
        private readonly List<IClickable> _clickables = new List<IClickable>();

        public event EventHandler ReplayClicked;

        //Text elements
        public Text ScoreText { get; private set; }
        public Text LivesText { get; private set; }
        public Text CooldownText { get; private set; }
        public Text GameOverText { get; private set; }
        public Text HighScoreText { get; private set; }

        //Button elements
        public Button ReplayButton { get; private set; }

        private UIManager() { }

        public void SetUpUI()
        {
            //Get the screen dimensions from Globals
            float screenWidth = Globals.ScreenWidth;
            float screenHeight = Globals.ScreenHeight;
            Texture2D buttonTexture = Globals.Content.Load<Texture2D>("UI/Button"); //Default button texture


            //Creating UI texts
            ScoreText = AddText(new Vector2(screenWidth * 0.85f, screenHeight * 0.05f), "Score: 0", Color.White, 1.5f);
            HighScoreText = AddText(new Vector2(screenWidth * 0.79f, screenHeight * 0.11f), "High Score: 0", Color.Gold, 1.5f);
            LivesText = AddText(new Vector2(screenWidth * 0.01f, screenHeight * 0.05f), "Lives: 3", Color.White, 1.5f);
            CooldownText = AddText(new Vector2(screenWidth * 0.5f - 180, screenHeight * 0.05f), "", Color.Yellow, 1.3f);
            GameOverText = AddText(new Vector2(screenWidth * 0.44f - 180, screenHeight * 0.3f), "GAME OVER", Color.Red, 4);

            //Adding buttons
            ReplayButton = AddButton(
                new Vector2((screenWidth - buttonTexture.Width * 0.4f) / 2,
                (screenHeight - buttonTexture.Height * 2f) / 2 + screenHeight * 0.3f),
                buttonTexture,
                Color.White,
                OnReplayClicked,
                0.4f);

            //Setting up replay button text
            ReplayButton.SetText("Play Again", Color.White, 1.4f);
            ReplayButton.SetTextPosition(new Vector2((screenWidth - buttonTexture.Width * 0.25f) / 2, (screenHeight - buttonTexture.Height * 2f) / 2 + screenHeight * 0.43f));

            //Initially invisible texts
            CooldownText.IsVisible = false;
            GameOverText.IsVisible = false;
            ReplayButton.IsVisible = false;
        }
        private void OnReplayClicked() //Replay button callback
        {
            if (ReplayClicked != null)
            {
                ReplayClicked.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Debug.WriteLine("ReplayClicked has no subscribers!");
            }
        }

        //Methods to update the texts
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

        public void ShowGameOver(bool isGameOver) //Show game over UI
        {
            GameOverText.IsVisible = isGameOver;
            ReplayButton.IsVisible = isGameOver;
        }

        //Button creation method
        public Button AddButton(Vector2 position, Texture2D texture, Color color, Action onClick, float scale = 1.0f)
        {
            var button = new Button(position, texture, color, onClick, scale);
            _uiElements.Add(button);
            _clickables.Add(button);
            return button;
        }

        //Text creation method
        public Text AddText(Vector2 position, string text, Color color, float scale)
        {
            Text textElement = new Text(Globals.Font, text, position, color, scale);
            AddElement(textElement);
            return textElement;
        }

        public void Update()
        {
            //Updating all the UI elements
            foreach (var element in _uiElements)
            {
                if (element.IsVisible)
                {
                    element.Update();
                }
            }

            //Reacting to mouse clicks
            if (IM.MouseClicked)
            {
                MouseClicking();
            }
        }

        private void MouseClicking() //Reaction to the mouse being clicked
        {
            foreach (var clickable in _clickables)
            {
                if (clickable is UIElement element && element.IsVisible) //Checking if the clickable object is UI and visible
                {
                    if (clickable.IsMouseOver())
                    {
                        clickable.Click();
                        break; //Stop after the top element
                    }
                }
            }
        }

        //Adding and removing UI elements to the correct lists
        public void AddElement(UIElement element)
        {
            _uiElements.Add(element);
            if (element is IClickable clickable)
            {
                _clickables.Add(clickable);
            }
        }

        public void RemoveElement(UIElement element) //Not being used but worth keeping
        {
            _uiElements.Remove(element);
            if (element is IClickable clickable)
            {
                _clickables.Remove(clickable);
            }
        }
        public void Draw()
        {
            //Drawing the visible UI elements
            foreach (var element in _uiElements)
            {
                if (element.IsVisible)
                {
                    element.Draw();
                }
            }
        }
    }
}