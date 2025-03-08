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

        public UIManager(List<Fish> fishes)
        {
            _fishes = fishes;
            _frame = new CameraFrame(IM.MousePosition, _fishes);
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
        }

        public Button AddButton(Vector2 position, Texture2D texture, Action onClick)
        {
            var button = new Button(position, texture, onClick);
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

                if (_frame.IsVisible)
                {
                    _frame.Click();
                }
                else
                {
                    foreach (var clickable in _clickables)
                    {
                        if (clickable is UIElement element && element.IsVisible)
                        {
                            if (clickable.IsMouseOver(IM.Cursor))
                            {
                                clickable.Click();
                                break;
                            }
                        }
                    }
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