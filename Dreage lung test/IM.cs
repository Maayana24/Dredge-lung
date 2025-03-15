using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public static class IM
    {
        private static MouseState _lastMouseState;
        private static MouseState _currentMouseState;
        private static Vector2 _direction;
        public static Vector2 Direction => _direction;
        private static Vector2 _directionArrows;
        public static Vector2 DirectionArrows => _directionArrows;
        public static Vector2 MousePosition => _currentMouseState.Position.ToVector2();
        public static Rectangle Cursor { get; private set; }
        public static bool MouseClicked { get; private set; }

        public static void Update()
        {
            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            // Update cursor position
            Cursor = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);

            // Update keyboard input
            var keyboardState = Keyboard.GetState();
            _direction = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
            if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;

            // Check for mouse click (button pressed this frame, but not last frame)
            MouseClicked = (_currentMouseState.LeftButton == ButtonState.Pressed) &&
                           (_lastMouseState.LeftButton == ButtonState.Released);

            // Debug output for mouse clicks
            if (MouseClicked)
            {
                Debug.WriteLine($"Mouse clicked at position: {MousePosition}");
            }
        }

        public static bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }
    }
}