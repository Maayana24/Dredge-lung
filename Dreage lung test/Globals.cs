using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test;

public delegate void PassFish(Fish fish);
public delegate object PassAndReturnFish(object obj);
public static class Globals
{
    public static PassFish passFish;
    public static ContentManager Content { get;  set; }
    public static SpriteBatch SpriteBatch { get;  set; }
    public static float DeltaTime { get;  set; }

    public static int ScreenWidth = 1080;

    public static int ScreenHeight = 1920;

    public static SpriteFont Font;

    public static void LoadContent(ContentManager content)
    {
        Content = content;
        Font = Content.Load<SpriteFont>("DefaultFont"); // Ensure the name is correct
    }

    public static Vector2 Position(int x, int y)
        { return new Vector2(x, y); }
    public static void Update(GameTime gameTime)
    {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}