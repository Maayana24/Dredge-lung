using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dredge_lung_test
{
    public delegate void FishSpawnedHandler(Fish fish);
    public delegate void FishCapturedHandler(Fish fish);
    public delegate void FishExaminedHandler(Fish fish);

    public class FishSpawner
    {
        private readonly List<Fish> _activeFishes;
        private readonly Random _random;
        private float _timeSinceLastSpawn;
        private readonly float _minSpawnTime;
        private readonly float _maxSpawnTime;
        private float _nextSpawnTime;
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private readonly Dictionary<Type, float> _fishSpawnRates = new Dictionary<Type, float>();
        private readonly List<IAnomaly> _availableAnomalies = new List<IAnomaly>();
        private float _anomalyChance = 0.2f; // 20% chance of spawning with anomaly

        // Events
        public event FishSpawnedHandler OnFishSpawned;
        public event FishCapturedHandler OnFishCaptured;
        public event FishExaminedHandler OnFishExamined;

        public FishSpawner(List<Fish> fishList, int screenWidth, int screenHeight, float minSpawnTime = 2f, float maxSpawnTime = 5f)
        {
            _activeFishes = fishList;
            _random = new Random();
            _minSpawnTime = minSpawnTime;
            _maxSpawnTime = maxSpawnTime;
            _nextSpawnTime = GetRandomSpawnTime();
            _timeSinceLastSpawn = 0;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;

            // Initialize default spawn rates
            _fishSpawnRates[typeof(Angler)] = 0.5f;
/*            _fishSpawnRates[typeof(FloaterFish)] = 0.3f;
            _fishSpawnRates[typeof(WavyFish)] = 0.2f;*/
        }

        public void Update(float deltaTime)
        {
            _timeSinceLastSpawn += deltaTime;

            // Check if it's time to spawn a new fish
            if (_timeSinceLastSpawn >= _nextSpawnTime)
            {
                SpawnRandomFish();
                _timeSinceLastSpawn = 0;
                _nextSpawnTime = GetRandomSpawnTime();
            }

            // Update all active fish
            for (int i = _activeFishes.Count - 1; i >= 0; i--)
            {
                _activeFishes[i].Update();

                // Remove fish that have left the screen
                if (IsFishOffScreen(_activeFishes[i]))
                {
                    _activeFishes.RemoveAt(i);
                }
            }
        }

        public void RegisterAnomaly(IAnomaly anomaly)
        {
            _availableAnomalies.Add(anomaly);
        }

        private void SpawnRandomFish()
        {
            // Choose a fish type based on spawn rates
            Type fishType = ChooseFishType();

            // Determine spawn position and direction based on fish type
            Vector2 position;
            Vector2 direction;

            if (fishType == typeof(Jelly))
            {
                // FloaterFish spawns at bottom and moves up
                position = new Vector2(_random.Next(100, _screenWidth - 100), _screenHeight + 50);
                direction = new Vector2(0, -1); // Up
            }
            else
            {
                // Determine if fish should go left-to-right or right-to-left
                bool goingRight = _random.NextDouble() > 0.5;

                if (goingRight)
                {
                    position = new Vector2(-100, _random.Next(100, _screenHeight - 100));
                    direction = new Vector2(1, 0); // Right
                }
                else
                {
                    position = new Vector2(_screenWidth + 100, _random.Next(100, _screenHeight - 100));
                    direction = new Vector2(-1, 0); // Left
                }
            }

            // Create the fish
            Fish newFish = CreateFish(fishType, position);
            if (newFish == null) return;

            // Set direction
            newFish.Direction = direction;

            // Set random speed and size for variety
            float speedMultiplier = (float)(_random.NextDouble() * 0.5 + 0.75); // 0.75 to 1.25
            float sizeMultiplier = (float)(_random.NextDouble() * 0.4 + 0.8);  // 0.8 to 1.2

            newFish.Speed *= speedMultiplier;
            newFish.Scale = new Vector2(sizeMultiplier, sizeMultiplier);

            // Random chance to add anomaly
            if (_availableAnomalies.Count > 0 && _random.NextDouble() < _anomalyChance)
            {
                IAnomaly anomaly = _availableAnomalies[_random.Next(_availableAnomalies.Count)];
                newFish.AddAnomaly(anomaly);
            }

            // Add to active fish list
            _activeFishes.Add(newFish);

            // Notify listeners
            OnFishSpawned?.Invoke(newFish);
        }

        private Type ChooseFishType()
        {
            double randomValue = _random.NextDouble();
            double cumulativeProbability = 0;

            foreach (var entry in _fishSpawnRates)
            {
                cumulativeProbability += entry.Value;
                if (randomValue < cumulativeProbability)
                    return entry.Key;
            }

            // Fallback to Angler if something goes wrong
            return typeof(Angler);
        }

        private Fish CreateFish(Type fishType, Vector2 position)
        {
            if (fishType == typeof(Angler))
                return new Angler(position);
 /*           else if (fishType == typeof(FloaterFish))
                return new FloaterFish(position);
            else if (fishType == typeof(WavyFish))
                return new WavyFish(position);*/

            return null;
        }

        private float GetRandomSpawnTime()
        {
            return (float)(_random.NextDouble() * (_maxSpawnTime - _minSpawnTime) + _minSpawnTime);
        }

        private bool IsFishOffScreen(Fish fish)
        {
            // Check if fish has moved completely off screen
            float buffer = 100;
            return (fish.Position.X < -buffer) ||
                   (fish.Position.X > _screenWidth + buffer) ||
                   (fish.Position.Y < -buffer) ||
                   (fish.Position.Y > _screenHeight + buffer);
        }

        public void NotifyFishCaptured(Fish fish)
        {
            OnFishCaptured?.Invoke(fish);
        }

        public void NotifyFishExamined(Fish fish)
        {
            OnFishExamined?.Invoke(fish);
        }
    }
}