using System.Collections.Generic;
namespace Dredge_lung_test
{
    public class BGM : ILayerable
    {
        private readonly List<Layer> _layers;
        private float _speedMultiplier = 1.0f;
        public float LayerDepth { get; set; } = 0.1f;
        public int ZIndex { get; set; } = 0;

        public BGM()
        {
            _layers = new();
            ZIndex = 0;
        }

        public void UpdateLayerDepth()
        {
            // Assign layer depths to all background layers
            float baseDepth = LayerDepth;
            int layerCount = _layers.Count;

            for (int i = 0; i < layerCount; i++)
            {
                // Distribute layer depths between LayerDepth and LayerDepth + 0.1f
                float depthStep = 0.1f / layerCount;
                _layers[i].LayerDepth = baseDepth + (i * depthStep);
                _layers[i].UpdateLayerDepth(); // Propagate update
            }
        }

        public void AddLayer(Layer layer)
        {
            _layers.Add(layer);
        }

        // Add method to set speed multiplier
        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        public void Update(float baseMovement)
        {
            // Apply speed multiplier to the base movement
            float adjustedMovement = baseMovement * _speedMultiplier;

            foreach (var layer in _layers)
            {
                layer.Update(adjustedMovement);
            }
        }

        public void Draw()
        {
            foreach (var layer in _layers)
            {
                layer.Draw();
            }
        }
    }
}