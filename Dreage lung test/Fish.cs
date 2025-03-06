using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public abstract class Fish : Sprite
    {
        public Fish(Vector2 position) : base(Globals.Content.Load<Texture2D>("Fish1"), position)
        {
        }
        public virtual void Update()
        {
            Movement();
        }
        public virtual void Movement()
        {
            Position += Speed * Direction * Globals.DeltaTime;
        }

        public virtual void Draw()
        {
            base.Draw();
        }

    }
}
