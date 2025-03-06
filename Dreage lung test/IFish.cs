using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    // Interface for all fish
    public interface IFish
    {
        Vector2 Position { get; }
        Rectangle Bounds { get; }
        Vector2 Direction { get; set; }
        float Speed { get; set; }
        bool HasBeenPhotographed { get; }
        float Value { get; }

        void Update();
        void Draw();
        void Movement();
        void OnPhotographed();
        bool IsInFrame(Rectangle frameBounds);
    }

}