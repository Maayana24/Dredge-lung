using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dredge_lung_test
{
    public class Angler : Fish
    {
        public Angler(Vector2 position) : base(Globals.Content.Load<Texture2D>("Fish1"), position)
        {
            // Customize Angler properties
            _value = 20; // Anglers are worth more points
            Scale = new Vector2(0.5f, 0.5f);
            Speed = 150f; // Anglers move faster
            MovementPattern = MovementPattern.Horizontal;
        }

        public override void Movement()
        {
            // You could add special movement patterns for Angler fish here
            base.Movement();
        }

        protected override void ShowFishInfoUI()
        {
            base.ShowFishInfoUI();
            // Add Angler-specific UI information here
        }
    }
}
