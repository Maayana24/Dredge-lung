using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Dredge_lung_test
{
    public class HighScoreManager
    {
        private const string HighScoreFileName = "highscore.txt";
        private const int MaxHighScoreEntries = 5; // Store top 5 scores

        private List<int> _highScores;

        public HighScoreManager()
        {
            _highScores = new List<int>();
            LoadHighScores();
        }

        public void AddScore(int score)
        {
            _highScores.Add(score);
            _highScores = _highScores.OrderByDescending(s => s).Take(MaxHighScoreEntries).ToList();
            SaveHighScores();
        }

        public int GetHighestScore()
        {
            return _highScores.Count > 0 ? _highScores[0] : 0;
        }

        public List<int> GetAllHighScores()
        {
            return new List<int>(_highScores);
        }

        private void LoadHighScores()
        {
            _highScores.Clear();
            string filePath = Path.Combine(Environment.CurrentDirectory, HighScoreFileName);

            if (!File.Exists(filePath))
            {
                // File doesn't exist yet, which is fine for a new game
                return;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (int.TryParse(line, out int score))
                        {
                            _highScores.Add(score);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid score format: {line}");
                        }
                    }
                } // StreamReader is automatically closed here

                // Sort in descending order
                _highScores = _highScores.OrderByDescending(s => s).Take(MaxHighScoreEntries).ToList();
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"IO Error reading high scores: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Access denied when reading high scores: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error loading high scores: {ex.Message}");
            }
        }

        private void SaveHighScores()
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, HighScoreFileName);

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (int score in _highScores)
                    {
                        writer.WriteLine(score);
                    }
                } // StreamWriter is automatically closed here
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"IO Error writing high scores: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Access denied when writing high scores: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error saving high scores: {ex.Message}");
            }
        }
    }
}