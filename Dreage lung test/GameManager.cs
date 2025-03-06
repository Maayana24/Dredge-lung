using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class GameManager
    {
        private readonly BGM _bgm = new();
        private readonly Player _player;
        private readonly List<Fish> _fishes;
        private readonly UIManager _ui;
        private readonly FishSpawner _fishSpawner;
        private FishInfoUI _fishInfoUI;
        private bool _isPaused = false;
        private int _score = 0;

        // Constants for game configuration
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        public GameManager()
        {
            // Get screen dimensions (assumed these are available in Globals)
            _screenWidth = Globals.ScreenWidth;
            _screenHeight = Globals.ScreenHeight;

            // Initialize background
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("1"), 0.0f, 0.0f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("2"), 0.1f, 0.09f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("4"), 0.2f, 0.1f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("5"), 0.3f, 0.2f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("7"), 0.4f, 0.3f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("9"), 0.5f, 0.4f));

            // Initialize player
            _player = new(Globals.Content.Load<Texture2D>("submarine"), new(300, 300));

            // Initialize fish list and spawner
            _fishes = new List<Fish>();
            _fishSpawner = new FishSpawner(_fishes, _screenWidth, _screenHeight, 1.5f, 3.5f);

            // Initialize anomalies
            InitializeAnomalies();

            // Set up event handlers
            _fishSpawner.OnFishCaptured += OnFishCaptured;
            _fishSpawner.OnFishExamined += OnFishExamined;

            // Initialize UI
            _ui = new UIManager(_fishes);
            _ui.SetUpUI(OnFishCaptured);
        }

        private void InitializeAnomalies()
        {
            // Load anomaly textures and register them
            Texture2D anomaly1 = Globals.Content.Load<Texture2D>("Anomalies/Anomaly1");
            Texture2D anomaly2 = Globals.Content.Load<Texture2D>("Anomalies/Anomaly2");

            _fishSpawner.RegisterAnomaly(new Anomaly(anomaly1, Vector2.Zero, 1.5f));
            _fishSpawner.RegisterAnomaly(new Anomaly(anomaly2, Vector2.Zero, 2.0f));
        }

        private void OnFishCaptured(Fish fish)
        {
            // Add to score when a fish is photographed
        //    _score += (int)fish.Value;
           // _ui.UpdateScore(_score);

            // Show fish info UI
            ShowFishInfo(fish);
        }

        private void OnFishExamined(Fish fish)
        {
            // Additional logic when a fish is examined in detail
        }

        private void ShowFishInfo(Fish fish)
        {
            // Pause the game
            _isPaused = true;

            // Create and show fish info UI
            _fishInfoUI = new FishInfoUI(fish, () => {
                // Resume game when UI is closed
                _isPaused = false;
                _fishInfoUI = null;
            });
        }

        public void Update()
        {
            // If paused for fish info, only update UI
            if (_isPaused)
            {
                _fishInfoUI?.Update();
                return;
            }

            _ui.Update();
            IM.Update();
            _bgm.Update(-200);
            _player.Update();

            // Update fish spawner and all active fish
            _fishSpawner.Update(Globals.DeltaTime);
        }

        public void Draw()
        {
            _bgm.Draw();
            _player.Draw();

            // Draw all active fish
            foreach (var fish in _fishes)
            {
                fish.Draw();
            }

            _ui.Draw();

            // Draw fish info UI if visible
            _fishInfoUI?.Draw();
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