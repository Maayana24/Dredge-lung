using Microsoft.Xna.Framework;
using System;

namespace Dredge_lung_test
{
    public interface IClickable
    {
        void Click();
        bool IsMouseOver(Rectangle bounds);

    }
}
