using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace Dredge_lung_test
{
    public class BGM : ILayerable, IUpdatable, IDrawable
    {
        // Singleton pattern
        private static BGM _instance;
        public static BGM Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BGM();
                return _instance;
            }
        }
        private readonly List<Layer> _layers;
        private float _speedMultiplier = 1.0f;
        private const float _baseMovement = -200.0f;  // Default base movement speed
        public float LayerDepth { get; set; } = 0.1f;
        public int ZIndex { get; set; } = 0;

        // Border mask properties
        private Texture2D _blankTexture;
        private Color _maskColor = Color.DarkGray; // Match your background color
        private float _borderLayerDepth = 0.85f;

        private BGM()
        {
            _layers = new List<Layer>();
            ZIndex = 0;

            // Initialize the blank texture for border masks
            _blankTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });
        }

        public void InitializeBackground()
        {
            // Clear any existing layers
            _layers.Clear();
            // Add background layers with parallax effects
            AddLayer(new Layer(LoadAndRotateTexture("1"), 0.0f, 0.0f));
            AddLayer(new Layer(LoadAndRotateTexture("2"), 0.1f, 0.09f));
            AddLayer(new Layer(LoadAndRotateTexture("4"), 0.2f, 0.1f));
            AddLayer(new Layer(LoadAndRotateTexture("5"), 0.3f, 0.2f));
            AddLayer(new Layer(LoadAndRotateTexture("7"), 0.4f, 0.3f));
            AddLayer(new Layer(LoadAndRotateTexture("9"), 0.5f, 0.4f));
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

            // Update border mask depth based on ZIndex
            _borderLayerDepth = 0.7f + (ZIndex / 100.0f);
            _borderLayerDepth = MathHelper.Clamp(_borderLayerDepth, 0.0f, 1.0f);
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

        // Add method to set border mask color
        public void SetBorderColor(Color color)
        {
            _maskColor = color;
        }

        public void Update()
        {
            // Apply speed multiplier to the base movement
            float adjustedMovement = _baseMovement * _speedMultiplier; // Fixed the asterisks
            foreach (var layer in _layers)
            {
                layer.Update(adjustedMovement);
            }
        }

        public void Draw()
        {
            // Draw background layers
            foreach (var layer in _layers)
            {
                layer.Draw();
            }
        }

        // New method to draw the border masks
        public void DrawBorderMasks()
        {
            // Left side mask
            Globals.SpriteBatch.Draw(
                _blankTexture,
                new Rectangle(0, 0, PlayableArea.X, Globals.ScreenHeight),
                null,
                _maskColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                _borderLayerDepth
            );

            // Right side mask
            Globals.SpriteBatch.Draw(
                _blankTexture,
                new Rectangle(PlayableArea.X + PlayableArea.Width, 0,
                              Globals.ScreenWidth - (PlayableArea.X + PlayableArea.Width),
                              Globals.ScreenHeight),
                null,
                _maskColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                _borderLayerDepth
            );

            // Top mask (if needed)
            Globals.SpriteBatch.Draw(
                _blankTexture,
                new Rectangle(PlayableArea.X, 0, PlayableArea.Width, PlayableArea.Y),
                null,
                _maskColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                _borderLayerDepth
            );

            // Bottom mask (if needed)
            Globals.SpriteBatch.Draw(
                _blankTexture,
                new Rectangle(PlayableArea.X, PlayableArea.Y + PlayableArea.Height,
                              PlayableArea.Width,
                              Globals.ScreenHeight - (PlayableArea.Y + PlayableArea.Height)),
                null,
                _maskColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                _borderLayerDepth
            );
        }

        private Texture2D LoadAndRotateTexture(string assetName)
        {
            Texture2D original = Globals.Content.Load<Texture2D>("Background/" + assetName);
            RenderTarget2D rotated = new RenderTarget2D(Globals.SpriteBatch.GraphicsDevice, original.Height, original.Width);
            Globals.SpriteBatch.GraphicsDevice.SetRenderTarget(rotated);
            Globals.SpriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(original, new Vector2(original.Height, 0), null, Color.White, MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            Globals.SpriteBatch.End();
            Globals.SpriteBatch.GraphicsDevice.SetRenderTarget(null);
            return rotated;
        }
    }
}