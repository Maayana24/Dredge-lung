using Microsoft.Xna.Framework;
using System;

namespace Dredge_lung_test
{
    //Interface for all elements drawn to keep track of layer depth
    public interface IClickable
    {
        void Click(); //For click events
        bool IsMouseOver(); //For logic when mouse is over 

    }
}
