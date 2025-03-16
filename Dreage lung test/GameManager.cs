using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class GameManager
    {
        //ORDER
        private List<Fish> _fishes;
        private List<Rock> _rocks;

        private readonly List<IUpdatable> _updatables;
        private readonly List<IDrawable> _drawables;

        private readonly Player _player;
        private readonly Harpoon _harpoon;

        private readonly UIManager _uiManager;
        private readonly ScoreManager _scoreManager;
        private readonly DifficultyManager _difficultyManager;
        private readonly BGM _bgm; //CHANGE NAME

        private readonly FishSpawner _fishSpawner;
        private readonly RockSpawner _rockSpawner;

        private Song song;

        //the game state
        private bool _isGameOver = false;

        public GameManager()
        {
            //Load the global resources
            Globals.Font = Globals.Content.Load<SpriteFont>("Fonts/Defult"); //SPELLING
            song = Globals.Content.Load<Song>("Audio/Drowned"); //MOVE TO GLOBALS

            MediaPlayer.Play(song); //WIP
            MediaPlayer.IsRepeating = true;

            //Initializing the lists
            _fishes = new List<Fish>();
            _rocks = new List<Rock>();
            _updatables = new List<IUpdatable>();
            _drawables = new List<IDrawable>();

            //Initializing the managers
            _scoreManager = new ScoreManager(3); //passing number of lives for the player
            _difficultyManager = new DifficultyManager();

            _scoreManager.GameOver += OnGameOver; //WIP

            //Initializing the UI
            _uiManager = new UIManager(_fishes);
            _uiManager.SetUpUI();

            //Initializing the background
            _bgm = BGM.Instance;
            _bgm.InitializeBackground();
            _bgm.ZIndex = 0; //Should be last layer

            //Initializing game objects
            _player = new Player(Globals.Content.Load<Texture2D>("Fish/Submarine"), new Vector2(300, 300), _scoreManager);
            _harpoon = new Harpoon(_player, _fishes, _scoreManager);
            _fishSpawner = new FishSpawner(_fishes);
            //WHY ROCK DIFFIRENT
            _rockSpawner = new RockSpawner(
                _rocks,
                Globals.Content.Load<Texture2D>("Rocks"),
                _scoreManager
            );

            //WIP FROM DOWN HERE

            //Connect event handlers
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
            _scoreManager.ScoreChanged += (sender, e) => _uiManager.UpdateScoreText(_scoreManager.Score);
            _scoreManager.LivesChanged += (sender, e) => _uiManager.UpdateLivesText(_scoreManager.Lives);
            _scoreManager.HighScoreChanged += (sender, e) => _uiManager.UpdateHighScoreText(_scoreManager.HighScore);

            // Difficulty events
            _difficultyManager.DifficultyChanged += _fishSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += _rockSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += OnDifficultyChanged;

           // _uiManager.ReplayClicked += (sender, e) => Reset();
        }

        private void RegisterUpdatables() //Adding updatable class to updatable list
        {
            _updatables.Add(_player);
            _updatables.Add(_uiManager);
            _updatables.Add(_fishSpawner);
            _updatables.Add(_rockSpawner);
            _updatables.Add(_difficultyManager);
            _updatables.Add(_bgm);
            _updatables.Add(_harpoon);
        }
        private void RegisterDrawables()
        {
            //Adding in order of what gets drawn first to last
            // Register background first (drawn first)
            _drawables.Add(_bgm);

            // Register rocks - assuming Rock implements IDrawable
            // Don't add individual rocks here, they will be drawn directly from the list

            // Register player and harpoon
            _drawables.Add(_player);
            _drawables.Add(_harpoon);

            // Don't add individual fish here, they will be drawn directly from the list

            // Register UI last (drawn on top)
            _drawables.Add(_uiManager);
        }

        //CHANGE FROM UI TO TEXT
        private void UpdateInitialUI() //Initializing UI texts
        {
            _uiManager.UpdateScoreText(_scoreManager.Score);
            _uiManager.UpdateLivesText(_scoreManager.Lives);
            _uiManager.UpdateHighScoreText(_scoreManager.HighScore);
        }

        public void Update()
        {
            //Stop updating when the game is over
            if (_isGameOver)
                return;

            IM.Update(); //Updating the input

            //Updating classes in updatable list
            foreach (var updatable in _updatables)
            {
                updatable.Update();
            }

            //Updating the collision manager
            CollisionManager.Instance.CheckCollisions();

            // Update cooldown text in UI
            UpdateHarpoonCooldownUI(); //WIP

            UpdateEntities(_fishes); //Updating active fish
            UpdateEntities(_rocks); //Updating active rocks
        }

        private void UpdateHarpoonCooldownUI()
        {
            if (_harpoon.IsInCooldown)
            {
                float remainingTime = _harpoon.CooldownDuration - _harpoon.CooldownTimer;
                _uiManager.UpdateCooldownText(true, (int)remainingTime);
            }
            else
            {
                _uiManager.UpdateCooldownText(false, 0);
            }
        }

        private void UpdateEntities<T>(List<T> entities) where T : class //for updating spawnble game objects
        {
            foreach (var entity in new List<T>(entities)) // Create a copy to avoid collection modified exception???
            {
                if (entity is IUpdatable updatable)
                {
                    updatable.Update();
                }
            }
        }

        private void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier) //MOVE WITH FRIENDS
        {
            // Update BGM speed to match the difficulty level
            _bgm.SetSpeedMultiplier(speedMultiplier);
        }

        public void Draw()
        {
            //Drawing registered drawables in the order they were added
            foreach (var drawable in _drawables)
            {
                drawable.Draw();
            }

            // Draw the border masks - this is special and should be drawn after backgrounds
            // but before most game elements
            BGM.Instance.DrawBorderMasks();

            DebugRenderer.DrawRectangle(PlayableArea.Bounds, Color.Yellow * 0.5f, 0.9f); //Drawing debug


            //Drawing active rocks
            foreach (Rock rock in _rocks)
            {
                rock.Draw();
            }

            //Drawing active fish
            foreach (Fish fish in _fishes)
            {
                fish.Draw();
            }
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            _isGameOver = true;
            _uiManager.ShowGameOver(true);
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
            _uiManager.ShowGameOver(false);
            _uiManager.UpdateScoreText(_scoreManager.Score);
            _uiManager.UpdateLivesText(_scoreManager.Lives);
            // High score doesn't need to be updated here as Reset() doesn't affect the high score
        }
    }
}