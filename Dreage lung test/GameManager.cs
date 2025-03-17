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

        private readonly UIManager _uiManager = UIManager.Instance;
        private readonly ScoreManager _scoreManager = ScoreManager.Instance;
        private readonly BackgroundManager _backgroundManager = BackgroundManager.Instance;
        private readonly DifficultyManager _difficultyManager = DifficultyManager.Instance;

        private readonly FishSpawner _fishSpawner;
        private readonly RockSpawner _rockSpawner;

        private Song song;

        //the game state
        private bool _isGameOver = false;

        public GameManager()
        {
            //Load the global resources
            Globals.Font = Globals.Content.Load<SpriteFont>("Fonts/Defult"); //SPELLING
            song = Globals.Content.Load<Song>("Audio/Drowned");

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            //Initializing the lists
            _fishes = new List<Fish>();
            _rocks = new List<Rock>();
            _updatables = new List<IUpdatable>();
            _drawables = new List<IDrawable>();

            _scoreManager.GameOver += OnGameOver;

            //Initializing the UI after fonts
            _uiManager.SetUpUI();

            //Initializing game objects
            _player = new Player(new Vector2(300, 300));
            _harpoon = new Harpoon(_player, _fishes);
            _fishSpawner = new FishSpawner(_fishes);
            _rockSpawner = new RockSpawner(_rocks);

            //Connect events
            ConnectEvents();

            //Add updatable and drawable components to their lists
            AddUpdatables();
            AddDrawables();

            //Initial UI updates
            UpdateInitialUI();
        }

        private void ConnectEvents()
        {
            //Score manager events
            _scoreManager.ScoreChanged += (sender, e) => _uiManager.UpdateScoreText(_scoreManager.Score);
            _scoreManager.LivesChanged += (sender, e) => _uiManager.UpdateLivesText(_scoreManager.Lives);
            _scoreManager.HighScoreChanged += (sender, e) => _uiManager.UpdateHighScoreText(_scoreManager.HighScore);

            //Difficulty manager events
            _difficultyManager.DifficultyChanged += _fishSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += _rockSpawner.OnDifficultyChanged;
            _difficultyManager.DifficultyChanged += _backgroundManager.OnDifficultyChanged;

            //UI manager events
            _uiManager.ReplayClicked += (sender, e) => Reset();
        }

        private void AddUpdatables() //Adding updatable class to updatable list
        {
            _updatables.Add(_player);
            _updatables.Add(_uiManager);
            _updatables.Add(_fishSpawner);
            _updatables.Add(_rockSpawner);
            _updatables.Add(_difficultyManager);
            _updatables.Add(_backgroundManager);
            _updatables.Add(_harpoon);
        }
        private void AddDrawables() //Adding drawable class to drawable list
        {
            //Adding in the order of what gets drawn first to last
            _drawables.Add(_backgroundManager);
            _drawables.Add(_player);
            _drawables.Add(_harpoon);
            _drawables.Add(_uiManager);
        }

        private void UpdateInitialUI() //Initializing UI texts
        {
            _uiManager.UpdateScoreText(_scoreManager.Score);
            _uiManager.UpdateLivesText(_scoreManager.Lives);
            _uiManager.UpdateHighScoreText(_scoreManager.HighScore);
        }

        public void Update()
        {
            IM.Update(); //Updating the input

            _uiManager.Update();


            //Stop updating when the game is over
            if (_isGameOver)
                return;


            //Updating classes in updatable list
            foreach (var updatable in _updatables)
            {
                updatable.Update();
            }

            //Updating the collision manager
            CollisionManager.Instance.CheckCollisions();

            //Update harpoon cooldown text
            UpdateHarpoonCooldownUI();

            //Updating active fish and rocks
            UpdateEntities(_fishes);
            UpdateEntities(_rocks);
        }

        private void UpdateHarpoonCooldownUI() //Updating the harpoon cooldown text
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
            foreach (var entity in new List<T>(entities)) //Copying list to avoid exception
            {
                if (entity is IUpdatable updatable)
                {
                    updatable.Update();
                }
            }
        }

        private void OnGameOver(object sender, EventArgs e) //Handles game over event
        {
            _isGameOver = true;
            _uiManager.ShowGameOver(true);
        }

        public void Reset() //Resets game for a new run
        {
            //Unregistering the rocks and fish
            foreach (Rock rock in _rocks)
            {
                CollisionManager.Instance.Unregister(rock);
            }
            foreach (Fish fish in _fishes)
            {
                CollisionManager.Instance.Unregister(fish);
            }

            //Reset game components
            _difficultyManager.Reset();
            _fishes.Clear();
            _rocks.Clear();
            _scoreManager.Reset();

            //Reset game state
            _isGameOver = false;

            //Update UI
            _uiManager.ShowGameOver(false);
            _uiManager.UpdateScoreText(_scoreManager.Score);
            _uiManager.UpdateLivesText(_scoreManager.Lives);
        }

        public void Draw()
        {
            //Drawing registered drawables in the order they were added
            foreach (var drawable in _drawables)
            {
                drawable.Draw();
            }

            BackgroundManager.Instance.DrawBorderMasks(); //Drawing the border mask 

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
    }
}