using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class CollisionManager
    {
        private static CollisionManager _instance;
        private HashSet<ICollidable> _collidables = new HashSet<ICollidable>();

        // Singleton pattern
        private CollisionManager() { }

        public static CollisionManager Instance => _instance ??= new CollisionManager();

        // Register a collidable object
        public void Register(ICollidable collidable)
        {
            _collidables.Add(collidable);
        }

        // Unregister a collidable object
        public void Unregister(ICollidable collidable)
        {
            _collidables.Remove(collidable);
        }

        // Check for collisions between all registered objects
        public void CheckCollisions()
        {
            // Create a temporary list of active collidables
            List<ICollidable> activeCollidables = new List<ICollidable>();

            // Only process active collidables
            foreach (var collidable in _collidables)
            {
                if (collidable.IsActive)
                {
                    activeCollidables.Add(collidable);
                }
            }

            // Check each collidable against all others
            for (int i = 0; i < activeCollidables.Count; i++)
            {
                for (int j = i + 1; j < activeCollidables.Count; j++)
                {
                    ICollidable a = activeCollidables[i];
                    ICollidable b = activeCollidables[j];

                    // Skip if either object is no longer active
                    if (!a.IsActive || !b.IsActive)
                        continue;

                    // Check for collision
                    if (a.Bounds.Intersects(b.Bounds))
                    {
                        // Notify both objects of the collision
                        a.OnCollision(b);
                        b.OnCollision(a);
                    }
                }
            }
        }

        // Check if a specific object collides with any other
        public bool CheckCollision(ICollidable source, out ICollidable collisionTarget)
        {
            collisionTarget = null;

            if (!source.IsActive)
                return false;

            foreach (var target in _collidables)
            {
                if (target != source && target.IsActive && source.Bounds.Intersects(target.Bounds))
                {
                    collisionTarget = target;
                    return true;
                }
            }

            return false;
        }
    }
}