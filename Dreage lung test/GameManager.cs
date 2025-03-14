using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Dredge_lung_test
{
    public class GameManager
    {
        private List<ILayerable> _layerableObjects = new List<ILayerable>();
        private readonly BGM _bgm = new();
        private readonly Player _player;
        private List<Fish> _fishes;
        private List<Rock> _rocks;
        private readonly UIManager _ui;
        private readonly FishSpawner _fishSpawner;
        private readonly RockSpawner _rockSpawner;
        private ScoreManager _scoreManager;
        private readonly Harpoon _harpoon;
        private DifficultyManager _difficultyManager;
        private HighScoreManager _highScoreManager;

        // Game state
        private bool _isGameOver = false;

        public GameManager()
        {
            // Initialize fish and rock lists
            _fishes = new List<Fish>();
            _rocks = new List<Rock>();
            _highScoreManager = new HighScoreManager();
            _ui = new UIManager(_fishes);

            // Initialize score manager
            _scoreManager = new ScoreManager(3); // Start with 3 lives
            _scoreManager.GameOver += OnGameOver;

            Globals.Font = Globals.Content.Load<SpriteFont>("Fonts/Defult");


            InitializeLayerableObjects();

            _difficultyManager = new DifficultyManager();

            // Set up UI after score manager is initialized
            _ui.SetUpUI();


            // Connect score manager events to UI updates
            _scoreManager.ScoreChanged += (sender, e) => _ui.UpdateScoreText(_scoreManager.Score);
            _scoreManager.LivesChanged += (sender, e) => _ui.UpdateLivesText(_scoreManager.Lives);

            //Background layers
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("1"), 0.0f, 0.0f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("2"), 0.1f, 0.09f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("4"), 0.2f, 0.1f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("5"), 0.3f, 0.2f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("7"), 0.4f, 0.3f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("9"), 0.5f, 0.4f));

            // Create player
            _player = new Player(Globals.Content.Load<Texture2D>("Fish/Submarine"), new Vector2(300, 300), _scoreManager);

            // Initialize harpoon
            _harpoon = new Harpoon(_player, _fishes, _scoreManager);

            // Create fish spawner
            _fishSpawner = new FishSpawner(_fishes);

            // Create rock spawner
            _rockSpawner = new RockSpawner(
                _rocks,
                Globals.Content.Load<Texture2D>("Rocks"),
                _scoreManager
            );

            _difficultyManager.DifficultyChanged += _fishSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += _rockSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += OnBGMDifficultyChanged;

            // Initial UI update
            _ui.UpdateScoreText(_scoreManager.Score);
            _ui.UpdateLivesText(_scoreManager.Lives);
            _ui.UpdateHighScoreText(_highScoreManager.GetHighestScore());
        }

        public void Update()
        {
            // Don't update if game is over
            if (_isGameOver)
                return;

            _ui.Update();
            IM.Update();
            _bgm.Update(-200);
            _player.Update();
            _difficultyManager.Update();

            // Update collision manager
            CollisionManager.Instance.CheckCollisions();

            // Update harpoon
            _harpoon.Update();

            // Update cooldown text in UI
            if (_harpoon.IsInCooldown)
            {
                float remainingTime = _harpoon.CooldownDuration - _harpoon.CooldownTimer;
                _ui.UpdateCooldownText(true, (int)remainingTime);
            }
            else
            {
                _ui.UpdateCooldownText(false, 0);
            }

            // Update fish spawner
            _fishSpawner.Update();

            // Update rock spawner
            _rockSpawner.Update();

            // Update all active fish
            foreach (Fish fish in new List<Fish>(_fishes)) // Create a copy to avoid collection modified exception
            {
                fish.Update();
            }

            // Update all active rocks
            foreach (Rock rock in new List<Rock>(_rocks)) // Create a copy to avoid collection modified exception
            {
                rock.Update();
            }
        }

        private void OnBGMDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier)
        {
            // Update BGM speed to match the rock speed multiplier
            _bgm.SetSpeedMultiplier(speedMultiplier);
        }

        private void InitializeLayerableObjects()
        {
            // Add all layerable objects to the list
            _layerableObjects.Add(_bgm); // Add single instance, not as a collection

            // Add collections of objects
            foreach (var fish in _fishes)
            {
                if (fish is ILayerable layerable)
                    _layerableObjects.Add(layerable);
            }

            foreach (var rock in _rocks)
            {
                if (rock is ILayerable layerable)
                    _layerableObjects.Add(layerable);
            }

            // Add other layerable objects

            // Update layer depths
            UpdateAllLayerDepths();
        }

        private void UpdateAllLayerDepths()
        {
            // Sort by ZIndex
            _layerableObjects = _layerableObjects.OrderBy(l => l.ZIndex).ToList();

            // Update all layer depths
            foreach (var layer in _layerableObjects)
            {
                layer.UpdateLayerDepth();
            }
        }

        public void Draw()
        {
            _bgm.Draw();

            // Draw all active rocks
            foreach (Rock rock in _rocks)
            {
                rock.Draw();
            }

            _player.Draw();
            _harpoon.Draw();

            // Draw all active fish
            foreach (Fish fish in _fishes)
            {
                fish.Draw();
            }

            // Draw UI elements (includes all text)
            _ui.Draw();
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            _isGameOver = true;
            _ui.ShowGameOver(true);

            // Save the score when game is over
            _highScoreManager.AddScore(_scoreManager.Score);

            // Update UI to show high score
            _ui.UpdateHighScoreText(_highScoreManager.GetHighestScore());
        }

        public void Reset()
        {
            // Unregister all collidables (requires adding this code to all collidable classes)
            foreach (Rock rock in _rocks)
            {
                CollisionManager.Instance.Unregister(rock);
            }

            _difficultyManager.Reset();
            _fishes.Clear();
            _rocks.Clear();
            _scoreManager.Reset();
            _isGameOver = false;
            _ui.ShowGameOver(false);
            _ui.UpdateScoreText(_scoreManager.Score);
            _ui.UpdateLivesText(_scoreManager.Lives);
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