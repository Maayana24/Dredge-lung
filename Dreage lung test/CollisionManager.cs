using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    //Class to manage collision conditions and checks
    public class CollisionManager
    {
        private static CollisionManager _instance;
        private HashSet<ICollidable> _collidables = new HashSet<ICollidable>();
        private CollisionManager() { }

        public static CollisionManager Instance => _instance ??= new CollisionManager();

        public void Register(ICollidable collidable)
        {
            _collidables.Add(collidable);
        }

        public void Unregister(ICollidable collidable)
        {
            _collidables.Remove(collidable);
        }

        public void CheckCollisions() //Checking collision for all collidable objects
        {
            List<ICollidable> activeCollidables = new List<ICollidable>();

            foreach (var collidable in _collidables) 
            {
                if (collidable.IsActive) //Check only active objects
                {
                    activeCollidables.Add(collidable);
                }
            }

            //Checking each collidable against all other ones
            for (int i = 0; i < activeCollidables.Count; i++)
            {
                for (int j = i + 1; j < activeCollidables.Count; j++)
                {
                    ICollidable a = activeCollidables[i];
                    ICollidable b = activeCollidables[j];

                    if (!a.IsActive || !b.IsActive) //skip if both of them are inactive
                        continue;

                    if (a.Bounds.Intersects(b.Bounds)) //Check if there's collision
                    {
                        //Notify both objects of their collision
                        a.OnCollision(b);
                        b.OnCollision(a);
                    }
                }
            }
        }
    }
}