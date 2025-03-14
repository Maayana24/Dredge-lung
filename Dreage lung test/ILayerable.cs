using Microsoft.Xna.Framework;

namespace Dredge_lung_test
{
    public interface ILayerable
    {
        float LayerDepth { get; set; }
        int ZIndex { get; set; }
        void UpdateLayerDepth();
    }
}