using System;
using System.IO;

namespace Dredge_lung_test
{
    public class ScoreManager
    {
        private const string HighScoreFileName = "highscore.txt";

        // Game state properties
        private int _score;
        private int _lives;
        private int _maxLives;
        private int _highScore;

        // Events
        public event EventHandler GameOver;
        public event EventHandler ScoreChanged;
        public event EventHandler LivesChanged;
        public event EventHandler HighScoreChanged;

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

                    // Check if current score is a new high score
                    if (_score > _highScore)
                    {
                        HighScore = _score;
                    }
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

        public int HighScore
        {
            get { return _highScore; }
            private set
            {
                if (_highScore != value)
                {
                    _highScore = value;
                    OnHighScoreChanged();
                    SaveHighScore();
                }
            }
        }

        // Constructor
        public ScoreManager(int initialLives = 3)
        {
            _score = 0;
            _lives = initialLives;
            _maxLives = initialLives;
            LoadHighScore();
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

        // High score file operations
        private void LoadHighScore()
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, HighScoreFileName);

            if (!File.Exists(filePath))
            {
                _highScore = 0;
                return;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line = reader.ReadLine();
                    if (int.TryParse(line, out int highScore))
                    {
                        _highScore = highScore;
                    }
                    else
                    {
                        _highScore = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading high score: {ex.Message}");
                _highScore = 0;
            }
        }

        private void SaveHighScore()
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, HighScoreFileName);

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(_highScore);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving high score: {ex.Message}");
            }
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

        protected virtual void OnHighScoreChanged()
        {
            HighScoreChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}