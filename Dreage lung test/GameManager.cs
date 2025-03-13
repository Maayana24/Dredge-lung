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
        private Text _scoreText;
        private Text _livesText;
        private Text _cooldownText;
        private ScoreManager _scoreManager;

        private readonly Harpoon _harpoon;

        // Game state
        private bool _isGameOver = false;

        public GameManager()
        {
            // Initialize fish list
            _fishes = new List<Fish>();

            _ui = new UIManager(_fishes);
            _ui.SetUpUI();

            // Initialize score manager
            _scoreManager = new ScoreManager(3); // Start with 3 lives
            _scoreManager.GameOver += OnGameOver;

            _scoreManager.ScoreChanged += UpdateScoreText;
            _scoreManager.LivesChanged += UpdateLivesText;


            // Create UI text elements for score and lives
            _scoreText = _ui.AddText(new Vector2(900, 40), "Score: 0", Color.White);
            _livesText = _ui.AddText(new Vector2(10, 40), "Lives: 3", Color.White);
            _cooldownText = _ui.AddText(new Vector2(Globals.ScreenWidth / 2 - 100, 40), "", Color.Yellow);
            _cooldownText.IsVisible = false;


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
            _ui.Draw();
            _player.Draw();
            _harpoon.Draw();

            // Draw all active fish
            foreach (Fish fish in _fishes)
            {
                fish.Draw();
            }


            // Draw score
            string scoreText = $"Score: {_scoreManager.Score}";
            Vector2 scorePos = new Vector2(10, 10);
            Globals.SpriteBatch.DrawString(Globals.Font, scoreText, scorePos, Color.White);

            // Draw lives
            string livesText = $"Lives: {_scoreManager.Lives}";
            Vector2 livesPos = new Vector2(10, 40);
            Globals.SpriteBatch.DrawString(Globals.Font, livesText, livesPos, Color.White);

            // Draw game over message if applicable
            if (_isGameOver)
            {
                string gameOverText = "GAME OVER";
                Vector2 textSize = Globals.Font.MeasureString(gameOverText);
                Vector2 gameOverPos = new Vector2((Globals.ScreenWidth - textSize.X) / 2, (Globals.ScreenHeight - textSize.Y) / 2);
                Globals.SpriteBatch.DrawString(Globals.Font, gameOverText, gameOverPos, Color.Red);
            }
        }

        private void UpdateScoreText(object sender, EventArgs e)
        {
            _scoreText.SetText($"Score: {_scoreManager.Score}");
        }

        private void UpdateLivesText(object sender, EventArgs e)
        {
            _livesText.SetText($"Lives: {_scoreManager.Lives}");
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            _isGameOver = true;
        }

        public void Reset()
        {
            _fishes.Clear();
            _scoreManager.Reset();
            _isGameOver = false;
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