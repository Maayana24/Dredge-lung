using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public static class PlayableArea
    {
        // The actual playable area dimensions
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        // Positions of the playable area within the screen
        public static int X { get; private set; }
        public static int Y { get; private set; }

        // Rectangle representing the playable area
        public static Rectangle Bounds { get; private set; }

        public static void Initialize(int width, int height)
        {
            Width = width;
            Height = height;

            // Center the playable area in the screen
            X = (Globals.ScreenWidth - Width) / 2;
            Y = (Globals.ScreenHeight - Height) / 2;

            Bounds = new Rectangle(X, Y, Width, Height);
        }

        // Helper method to check if a position is within the playable area
        public static bool Contains(Vector2 position)
        {
            return Bounds.Contains(position.ToPoint());
        }

        // Helper method to constrain a position to the playable area
        public static Vector2 Constrain(Vector2 position)
        {
            return new Vector2(
                MathHelper.Clamp(position.X, X, X + Width),
                MathHelper.Clamp(position.Y, Y, Y + Height)
            );
        }
    }
}