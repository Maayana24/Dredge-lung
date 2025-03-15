using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Dredge_lung_test
{
    public class GameManager
    {
        private readonly Player _player;
        private List<Fish> _fishes;
        private List<Rock> _rocks;
        private readonly UIManager _ui;
        private readonly FishSpawner _fishSpawner;
        private readonly RockSpawner _rockSpawner;
        private ScoreManager _scoreManager;
        private readonly Harpoon _harpoon;
        private DifficultyManager _difficultyManager;
        private readonly BGM _bgm;
        private List<IUpdatable> _updatables = new List<IUpdatable>();
        private List<IDrawable> _drawables = new List<IDrawable>();
        private Song song;


        // Game state
        private bool _isGameOver = false;

        public GameManager()
        {
            // Load global resources
            Globals.Font = Globals.Content.Load<SpriteFont>("Fonts/Defult");
            song = Globals.Content.Load<Song>("Audio/Drowned");

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            // Initialize collections
            _fishes = new List<Fish>();
            _rocks = new List<Rock>();
            _updatables = new List<IUpdatable>();

            // Initialize managers
            _scoreManager = new ScoreManager(1); // Start with 3 lives
            _scoreManager.GameOver += OnGameOver;
            _difficultyManager = new DifficultyManager();

            // Initialize UI
            _ui = new UIManager(_fishes);
            _ui.SetUpUI();

            // Initialize background
            _bgm = BGM.Instance;
            _bgm.InitializeBackground();
            _bgm.ZIndex = 0;

            // Initialize game objects
            _player = new Player(Globals.Content.Load<Texture2D>("Fish/Submarine"), new Vector2(300, 300), _scoreManager);
            _harpoon = new Harpoon(_player, _fishes, _scoreManager);
            _fishSpawner = new FishSpawner(_fishes);
            _rockSpawner = new RockSpawner(
                _rocks,
                Globals.Content.Load<Texture2D>("Rocks"),
                _scoreManager
            );

            // Connect event handlers
            ConnectEventHandlers();

            // Register updatable components
            RegisterUpdatables();
            RegisterDrawables();

            // Initial UI updates
            UpdateInitialUI();
        }

        private void ConnectEventHandlers()
        {
            // Score and lives events
            _scoreManager.ScoreChanged += (sender, e) => _ui.UpdateScoreText(_scoreManager.Score);
            _scoreManager.LivesChanged += (sender, e) => _ui.UpdateLivesText(_scoreManager.Lives);
            // Add high score event handler
            _scoreManager.HighScoreChanged += (sender, e) => _ui.UpdateHighScoreText(_scoreManager.HighScore);

            // Difficulty events
            _difficultyManager.DifficultyChanged += _fishSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += _rockSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += OnDifficultyChanged;

            _ui.ReplayClicked += (sender, e) => Reset();
        }

        private void RegisterUpdatables()
        {
            _updatables.Add(_player);
            _updatables.Add(_ui);
            _updatables.Add(_fishSpawner);
            _updatables.Add(_rockSpawner);
            _updatables.Add(_difficultyManager);
            _updatables.Add(_bgm);
            _updatables.Add(_harpoon);
        }
        private void RegisterDrawables()
        {
            // Register background first (drawn first)
            _drawables.Add(_bgm);

            // Register rocks - assuming Rock implements IDrawable
            // Don't add individual rocks here, they will be drawn directly from the list

            // Register player and harpoon
            _drawables.Add(_player);
            _drawables.Add(_harpoon);

            // Don't add individual fish here, they will be drawn directly from the list

            // Register UI last (drawn on top)
            _drawables.Add(_ui);
        }

        private void UpdateInitialUI()
        {
            _ui.UpdateScoreText(_scoreManager.Score);
            _ui.UpdateLivesText(_scoreManager.Lives);
            // Add high score update
            _ui.UpdateHighScoreText(_scoreManager.HighScore);
        }

        public void Update()
        {
            // Don't update if game is over
            if (_isGameOver)
                return;

            // Update input and UI
            IM.Update();
            foreach (var updatable in _updatables)
            {
                updatable.Update();
            }

            // Update collision manager
            CollisionManager.Instance.CheckCollisions();

            // Update cooldown text in UI
            UpdateHarpoonCooldownUI();

            // Update all active fish
            UpdateEntities(_fishes);

            // Update all active rocks
            UpdateEntities(_rocks);
        }

        private void UpdateHarpoonCooldownUI()
        {
            if (_harpoon.IsInCooldown)
            {
                float remainingTime = _harpoon.CooldownDuration - _harpoon.CooldownTimer;
                _ui.UpdateCooldownText(true, (int)remainingTime);
            }
            else
            {
                _ui.UpdateCooldownText(false, 0);
            }
        }

        private void UpdateEntities<T>(List<T> entities) where T : class
        {
            foreach (var entity in new List<T>(entities)) // Create a copy to avoid collection modified exception
            {
                if (entity is IUpdatable updatable)
                {
                    updatable.Update();
                }
            }
        }

        private void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier)
        {
            // Update BGM speed to match the difficulty level
            _bgm.SetSpeedMultiplier(speedMultiplier);
        }

        public void Draw()
        {
            // Draw all registered drawables in order of their registration
            foreach (var drawable in _drawables)
            {
                drawable.Draw();
            }

            // Draw the border masks - this is special and should be drawn after backgrounds
            // but before most game elements
            BGM.Instance.DrawBorderMasks();

            // Draw debug visualization
            DebugRenderer.DrawRectangle(PlayableArea.Bounds, Color.Yellow * 0.5f, 0.9f);

            // Draw all active rocks
            foreach (Rock rock in _rocks)
            {
                rock.Draw();
            }

            // Draw all active fish
            foreach (Fish fish in _fishes)
            {
                fish.Draw();
            }
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            _isGameOver = true;
            _ui.ShowGameOver(true);
        }

        public void Reset()
        {
            // Unregister all collidables
            foreach (Rock rock in _rocks)
            {
                CollisionManager.Instance.Unregister(rock);
            }

            // Reset game components
            _difficultyManager.Reset();
            _fishes.Clear();
            _rocks.Clear();
            _scoreManager.Reset();

            // Reset game state
            _isGameOver = false;

            // Update UI
            _ui.ShowGameOver(false);
            _ui.UpdateScoreText(_scoreManager.Score);
            _ui.UpdateLivesText(_scoreManager.Lives);
            // High score doesn't need to be updated here as Reset() doesn't affect the high score
        }
    }
}