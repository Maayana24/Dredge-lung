using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
namespace Dredge_lung_test
{
    public class UIManager
    {
        private readonly List<UIElement> _uiElements = new List<UIElement>();
        private readonly List<IClickable> _clickables = new List<IClickable>();
        private List<Fish> _fishes;
        private readonly CameraFrame _frame;
        private readonly GameManager _gameManager;

        public UIManager(List<Fish> fishes, GameManager gameManager)
        {
            _fishes = fishes;
            _gameManager = gameManager;
            _frame = new CameraFrame(IM.MousePosition, _fishes, _gameManager);
            _uiElements.Add(_frame);
            _clickables.Add(_frame);
        }

        public void SetUpUI()
        {
            AddButton(new Vector2(90, 50), Globals.Content.Load<Texture2D>("UI/cameraIcon"), () =>
            {
                if (!_frame.IsVisible)
                {
                    _frame.IsVisible = true;
                }
                else
                {
                    _frame.IsVisible = false;
                }
            });

            // Add an accuracy display button/label (optional)
/*            AddButton(new Vector2(200, 50), Globals.Content.Load<Texture2D>("UI/statsIcon"), () =>
            {
                // Display accuracy stats
                System.Diagnostics.Debug.WriteLine($"Accuracy: {_gameManager.GetAccuracy() * 100:F1}%");
            });*/
        }

        public Button AddButton(Vector2 position, Texture2D texture, Action onClick)
        {
            var button = new Button(position, texture, onClick, Color.White);
            _uiElements.Add(button);
            _clickables.Add(button);
            return button;
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
                // First check if any UI elements were clicked
                bool clickHandled = false;

                foreach (var clickable in _clickables)
                {
                    if (clickable is UIElement element && element.IsVisible)
                    {
                        if (clickable.IsMouseOver(IM.Cursor))
                        {
                            clickable.Click();
                            clickHandled = true;
                            break;
                        }
                    }
                }

                // If no regular UI was clicked, handle camera frame separately
                if (!clickHandled && _frame.IsVisible)
                {
                    _frame.Click();
                }
            }
        }

        public void Draw()
        {
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