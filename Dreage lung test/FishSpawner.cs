using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class FishSpawner
    {
        private readonly List<Fish> _activeFishes;
        private readonly Random _random;
        private float _spawnTimer;
        private readonly float _minSpawnTime = 0.5f;
        private readonly float _maxSpawnTime = 2.0f;
        private float _nextSpawnTime;

        private readonly int _screenWidth;
        private readonly int _screenHeight;

        public FishSpawner(List<Fish> fishes, int screenWidth, int screenHeight)
        {
            _activeFishes = fishes;
            _random = new Random();
            _spawnTimer = 0;
            _nextSpawnTime = GetRandomSpawnTime();
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;

            // Ensure AnomalyManager is initialized
            AnomalyManager.Instance.ToString(); // Just to trigger initialization
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

        private float GetRandomSpawnTime()
        {
            return (float)(_random.NextDouble() * (_maxSpawnTime - _minSpawnTime) + _minSpawnTime);
        }

        private void SpawnRandomFish()
        {
            // 1 in 10 chance to spawn a Jelly (bottom to top)
            bool spawnJelly = _random.Next(10) == 0;

            if (spawnJelly)
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

        private void SpawnJelly()
        {
            // Spawn at random x position at the bottom of the screen
            int xPos = _random.Next(100, _screenWidth - 100);
            Vector2 position = new Vector2(xPos, _screenHeight + 50); // Start below the screen

            Jelly jelly = new Jelly(position);
            _activeFishes.Add(jelly);
        }

        private void SpawnFish(int fishType, bool leftToRight)
        {
            // Set starting position based on direction (off-screen)
            float xPos = leftToRight ? -100 : _screenWidth + 100; // Off-screen position
            float yPos = _random.Next(200, _screenHeight - 200); // Random y position
            Vector2 position = new Vector2(xPos, yPos);

            Fish fish = new Grouper(position);

/*            switch (fishType)
            {
                case 0:
                    fish = new Grouper(position);
                    break;
                case 1:
                    fish = new Angler(position);
                    break;
                case 2:
                    fish = new Eel(position);
                    break;
                case 3:
                    fish = new Shark(position);
                    break;
                default:
                    fish = new Grouper(position);
                    break;
            }*/

            // Set direction based on spawn side
            fish.Direction = leftToRight ? new Vector2(1, 0) : new Vector2(-1, 0);

            _activeFishes.Add(fish);
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
                   fish.Position.X > _screenWidth + margin ||
                   fish.Position.Y < -margin ||
                   fish.Position.Y > _screenHeight + margin;
        }
    }
}