using System;

namespace Dredge_lung_test
{
    public class DifficultyManager
    {
        // Delegate for difficulty changes
        public delegate void DifficultyChangedHandler(int level, float speedMultiplier, float spawnRateMultiplier);

        // Event that will be triggered when difficulty changes
        public event DifficultyChangedHandler DifficultyChanged;

        private int _currentLevel = 1;
        private float _elapsedTime = 0f;
        private float _timeToNextLevel = 30f; // Increase difficulty every 30 seconds

        // Difficulty multipliers
        public float SpeedMultiplier => 1f + (_currentLevel - 1) * 0.1f; // Increase speed by 10% per level
        public float SpawnRateMultiplier => 1f + (_currentLevel - 1) * 0.15f; // Increase spawn rate by 15% per level

        public int CurrentLevel => _currentLevel;

        public void Update()
        {
            _elapsedTime += Globals.DeltaTime;

            if (_elapsedTime >= _timeToNextLevel)
            {
                _elapsedTime = 0f;
                IncreaseDifficulty();
            }
        }

        public void IncreaseDifficulty()
        {
            _currentLevel++;

            // Notify subscribers about the difficulty change
            DifficultyChanged?.Invoke(_currentLevel, SpeedMultiplier, SpawnRateMultiplier);
        }

        public void Reset()
        {
            _currentLevel = 1;
            _elapsedTime = 0f;

            // Notify subscribers about the reset
            DifficultyChanged?.Invoke(_currentLevel, SpeedMultiplier, SpawnRateMultiplier);
        }
    }
}