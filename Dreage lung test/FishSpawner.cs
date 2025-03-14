using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class FishSpawner
    {
            private readonly List<Fish> _activeFishes;
            private readonly Random _random;
            private float _spawnTimer;
            private float _baseMinSpawnTime = 0.5f;
            private float _baseMaxSpawnTime = 2.0f;
            private float _currentMinSpawnTime;
            private float _currentMaxSpawnTime;
            private float _nextSpawnTime;
            private float _speedMultiplier = 1.0f;

            private const int JellySpawnChance = 10; // Chance to spawn a Jelly

            public FishSpawner(List<Fish> fishes)
            {
                _activeFishes = fishes;
                _random = new Random();
                _spawnTimer = 0;
                _currentMinSpawnTime = _baseMinSpawnTime;
                _currentMaxSpawnTime = _baseMaxSpawnTime;
                _nextSpawnTime = GetRandomSpawnTime();

                // Ensure AnomalyManager is initialized
                AnomalyManager.Instance.ToString();
            }

            // Method to handle difficulty changes
            public void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier)
            {
                _speedMultiplier = speedMultiplier;

                // Decrease spawn times (faster spawning) as difficulty increases
                _currentMinSpawnTime = _baseMinSpawnTime / (spawnRateMultiplier / 2);
                _currentMaxSpawnTime = _baseMaxSpawnTime / (spawnRateMultiplier / 2);
            }

            private float GetRandomSpawnTime()
            {
                return (float)(_random.NextDouble() * (_currentMaxSpawnTime - _currentMinSpawnTime) + _currentMinSpawnTime);
            }

            // Modify SpawnFish to use the speed multiplier
            private void SpawnFish(int fishType, bool leftToRight)
            {
                // Set starting position based on direction (off-screen)
                float xPos = leftToRight ? -100 : Globals.ScreenWidth + 100; // Off-screen position
                float yPos = _random.Next(200, Globals.ScreenHeight - 200); // Random y position
                Vector2 position = new Vector2(xPos, yPos);

                (string fishName, Rectangle sourceRect, Vector2 scale, float baseSpeed) = GetFishAttributes(fishType);

                // Apply speed multiplier to the base speed
                float adjustedSpeed = baseSpeed * _speedMultiplier;

                // Create the fish with the determined parameters
                Fish fish = new Fish(fishName, position, adjustedSpeed, sourceRect, scale);

                // Set direction based on spawn side
                fish.Direction = leftToRight ? new Vector2(1, 0) : new Vector2(-1, 0);

                _activeFishes.Add(fish);
            }

            // Similarly modify SpawnJelly to use the speed multiplier
            private void SpawnJelly()
            {
                // Spawn at random x position at the bottom of the screen
                int xPos = _random.Next(100, Globals.ScreenWidth - 100);
                Vector2 position = new Vector2(xPos, Globals.ScreenHeight + 50); // Start below the screen

                Rectangle sourceRect = new Rectangle(150, 210, 250, 300);

                // Apply speed multiplier to the base speed
                float adjustedSpeed = 120 * _speedMultiplier;

                Fish jelly = new Fish("Jelly", position, adjustedSpeed, sourceRect, new Vector2(0.5f, 0.5f), new Vector2(0, -1));
                _activeFishes.Add(jelly);
            }

            public void Update()
        {
            // Update spawn timer
            _spawnTimer += Globals.DeltaTime;

            // Check if it's time to spawn a new fish
            if (_spawnTimer >= _nextSpawnTime)
            {
                SpawnRandomFish();
                _spawnTimer = 0;
                _nextSpawnTime = GetRandomSpawnTime();
            }

            // Update and remove fish that are out of bounds
            UpdateActiveFishes();
        }

        private void SpawnRandomFish()
        {
            // 1 in 10 chance to spawn a Jelly
            if (_random.Next(JellySpawnChance) == 0)
            {
                SpawnJelly();
            }
            else
            {
                // Choose a random horizontal fish type (0=Grouper, 1=Angler, 2=Eel, 3=Shark)
                int fishType = _random.Next(4);
                bool leftToRight = _random.Next(2) == 0; // 50% chance for each direction

                SpawnFish(fishType, leftToRight);
            }
        }

        private (string fishName, Rectangle sourceRect, Vector2 scale, float speed) GetFishAttributes(int fishType)
        {
            switch (fishType)
            {
                case 0:
                    return ("Grouper", new Rectangle(275, 50, 175, 105), new Vector2(0.6f, 0.6f), 150);
                case 1:
                    return ("Angler", new Rectangle(600, 10, 250, 175), new Vector2(0.5f, 0.5f), 180);
                case 2:
                    return ("Eel", new Rectangle(450, 250, 350, 200), new Vector2(0.5f, 0.5f), 220);
                case 3:
                    return ("Shark", new Rectangle(150, 550, 600, 200), new Vector2(0.3f, 0.3f), 100);
                default:
                    return ("Grouper", new Rectangle(275, 50, 175, 105), new Vector2(0.6f, 0.6f), 150);
            }
        }

        private void UpdateActiveFishes()
        {
            // Create a list for fish to remove
            List<Fish> fishesToRemove = new List<Fish>();

            foreach (Fish fish in _activeFishes)
            {
                // Check if fish is out of bounds
                if (IsOutOfBounds(fish))
                {
                    fishesToRemove.Add(fish);
                }
            }

            // Remove fish that are out of bounds
            foreach (Fish fish in fishesToRemove)
            {
                _activeFishes.Remove(fish);
            }
        }

        private bool IsOutOfBounds(Fish fish)
        {
            // Add extra margin to ensure fish is completely off-screen
            const int margin = 200;

            return fish.Position.X < -margin ||
                   fish.Position.X > Globals.ScreenWidth + margin ||
                   fish.Position.Y < -margin ||
                   fish.Position.Y > Globals.ScreenHeight + margin;
        }
    }
}
