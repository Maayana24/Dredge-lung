using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class GameManager
    {
        private readonly BGM _bgm = new();
        private readonly Player _player;
        private List<Fish> _fishes;
        private List<Rock> _rocks; // Added rocks list
        private readonly UIManager _ui;
        private readonly FishSpawner _fishSpawner;
        private readonly RockSpawner _rockSpawner; // Added rock spawner
        private ScoreManager _scoreManager;
        private readonly Harpoon _harpoon;

        // Game state
        private bool _isGameOver = false;

        public GameManager()
        {
            // Initialize fish and rock lists
            _fishes = new List<Fish>();
            _rocks = new List<Rock>(); // Initialize rocks list

            _ui = new UIManager(_fishes);

            // Initialize score manager
            _scoreManager = new ScoreManager(3); // Start with 3 lives
            _scoreManager.GameOver += OnGameOver;

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
            _player = new Player(Globals.Content.Load<Texture2D>("Fish/Submarine"), new Vector2(300, 300));

            // Initialize harpoon
            _harpoon = new Harpoon(_player, _fishes, _scoreManager);

            // Create fish spawner with screen dimensions
            _fishSpawner = new FishSpawner(_fishes);

            // Create rock spawner
            _rockSpawner = new RockSpawner(
                _rocks,
                Globals.Content.Load<Texture2D>("Obstacles/Rocks"), // Load rock texture
                _scoreManager
            );

            // Initial UI update
            _ui.UpdateScoreText(_scoreManager.Score);
            _ui.UpdateLivesText(_scoreManager.Lives);
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

            // Check if player collides with any rocks
            _rockSpawner.CheckPlayerCollision(_player);

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
        }

        public void Reset()
        {
            _fishes.Clear();
            _rocks.Clear(); // Clear rocks too
            _scoreManager.Reset();
            _isGameOver = false;
            _ui.ShowGameOver(false);
            _ui.UpdateScoreText(_scoreManager.Score);
            _ui.UpdateLivesText(_scoreManager.Lives);
        }

        private Texture2D LoadAndRotateTexture(string assetName) //maybe theres a simpler way?
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