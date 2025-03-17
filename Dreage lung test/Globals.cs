using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test;

public static class Globals //Global attributes I want access from everywhere
{
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }
    public static GraphicsDevice GraphicsDevice { get; set; }
    public static float DeltaTime { get; set; }

    public static SpriteFont Font { get; set; }
    public static int ScreenWidth { get; set; }
    public static int ScreenHeight { get; set; }

    public static void Update(GameTime gameTime)
    {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}