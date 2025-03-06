using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class UIManager
    {
        private readonly List<UIElement> _uiElements = new List<UIElement>();
        private readonly List<IClickable> _buttons = new List<IClickable>();
        private List<Fish> _fishes;


        private readonly CameraFrame _frame;

        public UIManager(List<Fish> fishes)
        {
            _fishes = fishes;
            _uiElements.Add(_frame = new(IM.MousePosition, _fishes));
        }

        public void SetUpUI(Action<Fish> fish)
        {

            AddButton(new(90, 50), Globals.Content.Load<Texture2D>("UI/cameraIcon"), () =>
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

        public Button AddButton(Vector2 position, Texture2D texture, Action onClick)
        {
            var button = new Button(position, texture, onClick);
            _uiElements.Add(button);
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
        }

        public void RemoveElement(UIElement element)
        {
            _uiElements.Remove(element);
        }
    }
}