using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public interface ICollidable
    {
        Rectangle Bounds { get; }
        bool IsActive { get; }
        void OnCollision(ICollidable other);
    }
}