using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    //Interface for all elements drawn to keep track of layer depth
    public interface ILayerable
    {
        float LayerDepth { get; set; }
        int ZIndex { get; set; }
        void UpdateLayerDepth();
    }
}