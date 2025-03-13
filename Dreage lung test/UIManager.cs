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
        private readonly CameraFrame _frame;
        private readonly FishInspectionScreen _inspectionScreen;

        public UIManager(List<Fish> fishes)
        {
            _fishes = fishes;

            // Create the fish inspection screen first
            _inspectionScreen = new FishInspectionScreen();
            _uiElements.Add(_inspectionScreen);
            _clickables.Add(_inspectionScreen);

            // Then create the camera frame with a reference to the inspection screen
            _frame = new CameraFrame(IM.MousePosition, _fishes, _inspectionScreen);
            _uiElements.Add(_frame);
            _clickables.Add(_frame);
        }

        public void SetUpUI()
        {
            AddButton(new Vector2(90, 50), Globals.Content.Load<Texture2D>("UI/cameraIcon"), Color.White, () =>
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
        }

        public Button AddButton(Vector2 position, Texture2D texture, Color color, Action onClick)
        {
            var button = new Button(position, texture, color, onClick);
            _uiElements.Add(button);
            _clickables.Add(button);
            return button;
        }

        public Text AddText(Vector2 position, string text, Color color)
        {
            Text textElement = new Text(Globals.Font, text, position, color);
           // _uiElements.Add(textElement);
            AddElement(textElement);
            return textElement;
        }

        public void Update()
        {
            // Debug output for inspection screen
            if (_inspectionScreen.IsVisible)
            {
                //System.Diagnostics.Debug.WriteLine("Inspection screen is visible in UIManager.Update");
            }

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
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            // Handle clicks for inspection screen first if it's visible and mouse is over it
            if (_inspectionScreen.IsVisible && _inspectionScreen.IsMouseOver(IM.Cursor))
            {
               // System.Diagnostics.Debug.WriteLine("Handling click on inspection screen");
                _inspectionScreen.Click();
                return;
            }

            // Then handle camera frame if it's visible
            if (_frame.IsVisible && _frame.IsMouseOver(IM.Cursor))
            {
               // System.Diagnostics.Debug.WriteLine("Handling click on camera frame");
                _frame.Click();
                return;
            }

            // Finally check other clickable elements
            foreach (var clickable in _clickables)
            {
                // Skip the inspection screen and camera frame as we already checked them
                if (clickable == _inspectionScreen || clickable == _frame)
                    continue;

                if (clickable is UIElement element && element.IsVisible && clickable.IsMouseOver(IM.Cursor))
                {
                    System.Diagnostics.Debug.WriteLine("Handling click on other UI element");
                    clickable.Click();
                    return;
                }
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

        // Getter for statistics if needed elsewhere
        public int GetCorrectAnswers() => _inspectionScreen.GetCorrectAnswers();
        public int GetIncorrectAnswers() => _inspectionScreen.GetIncorrectAnswers();
    }
}