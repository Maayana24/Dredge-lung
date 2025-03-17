using System;

namespace Dredge_lung_test
{
    //Class to manage the difficulty progression
    public class DifficultyManager : IUpdatable
    {
        public delegate void DifficultyChangedHandler(int level, float speedMultiplier, float spawnRateMultiplier); //Delegate for difficulty changes

        public event DifficultyChangedHandler DifficultyChanged; //The event that'll get triggered when the difficulty progress

        private static readonly DifficultyManager _instance = new DifficultyManager();
        public static DifficultyManager Instance => _instance;

        private DifficultyManager() { }

        private int _currentLevel = 0;
        private float _elapsedTime = 0f;
        private float _timeToNextLevel = 30f; //Increase the difficulty every 30 seconds

        public float SpeedMultiplier => 1f + (_currentLevel - 1) * 0.1f; //Increase speed by 10% per level
        public float SpawnRateMultiplier => 1f + (_currentLevel - 1) * 0.15f; //Increase spawn rate by 15% per level

        public int CurrentLevel => _currentLevel;

        public void Update()
        {
            _elapsedTime += Globals.DeltaTime;

            while (_elapsedTime >= _timeToNextLevel) //Change the difficulty when the timer reach the end
            {
                _elapsedTime -= _timeToNextLevel;
                IncreaseDifficulty();
            }
        }

        public void IncreaseDifficulty()
        {
            _currentLevel++;
            DifficultyChanged?.Invoke(_currentLevel, SpeedMultiplier, SpawnRateMultiplier); //Notify all subscribers about the difficulty change
        }

        public void Reset() //resetting the difficulty when a new game starts
        {
            _currentLevel = 1;
            _elapsedTime = 0f;

            DifficultyChanged?.Invoke(_currentLevel, SpeedMultiplier, SpawnRateMultiplier); //Notify subscribers about the reset

        }
    }
}