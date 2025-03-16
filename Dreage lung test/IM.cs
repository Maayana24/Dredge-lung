using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public static class IM //Input manager, static so it can be accessed anywhere
    {
        private static MouseState _lastMouseState;
        private static MouseState _currentMouseState;
        private static Vector2 _direction;
        public static Vector2 Direction => _direction;
        public static Vector2 MousePosition => _currentMouseState.Position.ToVector2();
        public static Rectangle Cursor { get; private set; }
        public static bool MouseClicked { get; private set; }

        public static void Update()
        {
            // Store the current state as the last state
            _lastMouseState = _currentMouseState;

            // Get the new current state
            _currentMouseState = Mouse.GetState();

            // Update the cursor position
            Cursor = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);

            // Updating all the keyboard inputs
            var keyboardState = Keyboard.GetState();
            _direction = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
            if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;

            // Checking mouse clicks - FIXED logic
            // MouseClicked is true when the button was NOT pressed in the previous frame
            // but IS pressed in the current frame
            bool isCurrentlyPressed = (_currentMouseState.LeftButton == ButtonState.Pressed);
            bool wasPreviouslyReleased = (_lastMouseState.LeftButton == ButtonState.Released);
            MouseClicked = isCurrentlyPressed && wasPreviouslyReleased;

            // Debug mouse clicks
            if (MouseClicked)
            {
                Debug.WriteLine($"MOUSE CLICKED at position: {MousePosition}");
            }
        }

        public static bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }
    }
}