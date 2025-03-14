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
        private readonly UIManager _ui;
        private readonly FishSpawner _fishSpawner;
        private ScoreManager _scoreManager;
        private readonly Harpoon _harpoon;

        // Game state
        private bool _isGameOver = false;

        public GameManager()
        {
            // Initialize fish list
            _fishes = new List<Fish>();

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

            // Update all active fish
            foreach (Fish fish in new List<Fish>(_fishes)) // Create a copy to avoid collection modified exception
            {
                fish.Update();
            }
        }

        public void Draw()
        {
            _bgm.Draw();
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