using System;

namespace Dredge_lung_test
{
    public class ScoreManager
    {
        // Game state properties
        private int _score;
        private int _lives;
        private int _maxLives;

        // Events
        public event EventHandler GameOver;
        public event EventHandler ScoreChanged;
        public event EventHandler LivesChanged;

        // Properties
        public int Score
        {
            get { return _score; }
            private set
            {
                if (_score != value)
                {
                    _score = value;
                    OnScoreChanged();
                }
            }
        }

        public int Lives
        {
            get { return _lives; }
            private set
            {
                if (_lives != value)
                {
                    _lives = value;
                    OnLivesChanged();

                    // Check for game over
                    if (_lives <= 0)
                    {
                        OnGameOver();
                    }
                }
            }
        }

        // Constructor
        public ScoreManager(int initialLives = 3)
        {
            _score = 0;
            _lives = initialLives;
            _maxLives = initialLives;
        }

        // Methods to modify score and lives
        public void AddPoints(int points)
        {
            if (points > 0)
            {
                Score += points;
            }
        }

        public void RemoveLife()
        {
            Lives--;
        }

        public void AddLife()
        {
            if (Lives < _maxLives)
            {
                Lives++;
            }
        }

        public void Reset()
        {
            Score = 0;
            Lives = _maxLives;
        }

        // Event triggers
        protected virtual void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnScoreChanged()
        {
            ScoreChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLivesChanged()
        {
            LivesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}