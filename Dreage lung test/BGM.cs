
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class BGM
    {
        private readonly List<Layer> _layers;

        public BGM()
        {
            _layers = new();
        }

        public void AddLayer(Layer layer)
        {
            _layers.Add(layer);
        }

        public void Update(float movement)
        {
            foreach (var layer in _layers)
            {
                layer.Update(movement);
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
