using Dredge_lung_test;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


public static class DebugRenderer
{
    // A simple texture for drawing rectangles
    private static Texture2D _pixel;

    public static void Initialize(GraphicsDevice graphicsDevice)
    {
        // Create a 1x1 white texture
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public static void DrawRectangle(Rectangle rectangle, Color color, float layerDepth = 0f)
    {
        // Draw outline only
        int thickness = 2;

        // Top line
        Globals.SpriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), null, color, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
        // Bottom line
        Globals.SpriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), null, color, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
        // Left line
        Globals.SpriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), null, color, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
        // Right line
        Globals.SpriteBatch.Draw(_pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), null, color, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
    }
}