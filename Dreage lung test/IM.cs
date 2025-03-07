using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Dredge_lung_test
{
    public static class IM
    {
        private static MouseState _lastMouseState;
        private static Vector2 _direction;
        public static Vector2 Direction => _direction;

        private static Vector2 _directionArrows;

        public static Vector2 DirectionArrows => _directionArrows;
        public static Vector2 MousePosition => Mouse.GetState().Position.ToVector2();

        public static MouseState MouseState;

        public static Rectangle Cursor { get; set; }

        public static bool MouseClicked { get; private set; }

        public static void Update()
        {
            Cursor = new((int)MousePosition.X, (int)MousePosition.Y, 1, 1);

            var keyboardState = Keyboard.GetState();

            _direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
            if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;
            MouseState = Mouse.GetState();


            MouseClicked = (MouseState.LeftButton == ButtonState.Pressed) && (_lastMouseState.LeftButton == ButtonState.Released);
            _lastMouseState = Mouse.GetState();


        }
    }
}