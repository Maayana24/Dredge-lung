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

        public UIManager(List<Fish> fishes)
        {
            _fishes = fishes;

        }

        public void SetUpUI()
        {

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