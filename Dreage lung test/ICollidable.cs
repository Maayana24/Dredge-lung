using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    //Interface for all elements with collision
    public interface ICollidable
    {
        Rectangle Bounds { get; }
        bool IsActive { get; }
        void OnCollision(ICollidable other);
    }
}