using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    //Class to manage the playable area of the game where the player can move and objects spawn
    public static class PlayableArea
    {
        //Playable area dimensions
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        //Positions of playable area in the screen
        public static int X { get; private set; }
        public static int Y { get; private set; }

        public static Rectangle Bounds { get; private set; }

        public static void Initialize(int width, int height)
        {
            Width = width;
            Height = height;

            //Center the playable area in the screen
            X = (Globals.ScreenWidth - Width) / 2;
            Y = (Globals.ScreenHeight - Height) / 2;

            Bounds = new Rectangle(X, Y, Width, Height);
        }
    }
}