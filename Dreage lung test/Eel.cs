using Dredge_lung_test;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;

namespace Dredge_lung_test
{
   public class Eel : Fish
    {
        public Eel(Vector2 position) : base(position)
        {
            // Set source rectangle for the Eel fish on the sprite sheet
            SourceRect = new Rectangle(450, 250, 350, 200);
            
            // Set properties - Eels are faster
            Speed = 220.0f;
            Scale = new Vector2(0.5f, 0.5f);
        }
    }
}

