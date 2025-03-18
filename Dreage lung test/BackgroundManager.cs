using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
namespace Dredge_lung_test
{
    //Class to manage the game's background
    public class BackgroundManager : ILayerable, IUpdatable, IDrawable
    {
        private static readonly Lazy<BackgroundManager> _instance = new(() => new BackgroundManager());
        public static BackgroundManager Instance => _instance.Value;

        private readonly List<Layer> _layers;
        private float _speedMultiplier = 1.0f;
        private const float _baseMovement = -200.0f; //Default base movement speed
        public float LayerDepth { get; set; } = 0.1f;
        public int ZIndex { get; set; } = 0;

        //Border mask properties
        private Texture2D _blankTexture;
        private Color _maskColor = Color.Black;
        private float _borderLayerDepth = 0.85f; //Below UI but above fish

        private BackgroundManager()
        {
            _layers = new List<Layer>();
            ZIndex = 0;
            InitializeBackground();
            _blankTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });
        }

        public void InitializeBackground()
        {
            //Clear any existing layers if there are any
            _layers.Clear();
            //Add background layers to the parallax background
            AddLayer(new Layer(LoadAndRotateTexture("1"), 0.0f, 0.0f));
            AddLayer(new Layer(LoadAndRotateTexture("2"), 0.1f, 0.09f));
            AddLayer(new Layer(LoadAndRotateTexture("4"), 0.2f, 0.1f));
            AddLayer(new Layer(LoadAndRotateTexture("5"), 0.3f, 0.2f));
            AddLayer(new Layer(LoadAndRotateTexture("7"), 0.4f, 0.3f));
            AddLayer(new Layer(LoadAndRotateTexture("9"), 0.5f, 0.4f));
        }

        public void UpdateLayerDepth()
        {
            //Assigning the layer depths to background layers
            float baseDepth = LayerDepth;
            int layerCount = _layers.Count;
            for (int i = 0; i < layerCount; i++)
            {
                //Distribute layer depths between LayerDepth and LayerDepth + 0.1f
                float depthStep = 0.1f / layerCount;
                _layers[i].LayerDepth = baseDepth + (i * depthStep);
                _layers[i].UpdateLayerDepth();
            }

            //Updating the border mask depth based on ZIndex
            _borderLayerDepth = 0.7f + (ZIndex / 100.0f);
            _borderLayerDepth = MathHelper.Clamp(_borderLayerDepth, 0.0f, 1.0f);
        }

        public void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier)
        {
            //Update Background speed to match the difficulty level to move the background the same speed as the rocks
            SetSpeedMultiplier(speedMultiplier);
        }
        public void AddLayer(Layer layer)
        {
            _layers.Add(layer);
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        public void Update()
        {
            //Adding the speed multiplier to the base movement
            float adjustedMovement = _baseMovement * _speedMultiplier;
            foreach (var layer in _layers)
            {
                layer.Update(adjustedMovement);
            }
        }

        private Texture2D LoadAndRotateTexture(string assetName) //Rotating the original Background texture to the correct direction for the game
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

        public void DrawBorderMasks() //Drawing a border mask to hide the spawning fish until they reach the playable area
        {
            //Left side
            Globals.SpriteBatch.Draw
            (
                _blankTexture,
                new Rectangle(0, 0, PlayableArea.X, Globals.ScreenHeight),
                null,
                _maskColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                _borderLayerDepth
            );

            //Right side
            Globals.SpriteBatch.Draw
            (
                _blankTexture,
                new Rectangle(PlayableArea.X + PlayableArea.Width, 0, Globals.ScreenWidth - (PlayableArea.X + PlayableArea.Width),
                Globals.ScreenHeight),
                null,
                _maskColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                _borderLayerDepth
            );
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