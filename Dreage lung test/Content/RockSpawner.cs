using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class RockSpawner
    {
        private readonly List<Rock> _activeRocks;
        private readonly Random _random;
        private float _spawnTimer;
        private float _baseMinSpawnTime = 3;
        private float _baseMaxSpawnTime = 4;
        private float _currentMinSpawnTime;
        private float _currentMaxSpawnTime;
        private float _nextSpawnTime;
        private readonly Texture2D _rockTexture;
        private readonly ScoreManager _scoreManager;
        private float _speedMultiplier = 1;

        // Rock attributes - manually defined rectangles for each rock type
        private readonly Rectangle[] _rockRects = new Rectangle[]
        {
            new Rectangle(50, 50, 200, 200),    // Rock type 0 - Update these values
            new Rectangle(50, 50, 200, 200),    // Rock type 1 - with your actual 
            new Rectangle(50, 50, 200, 200),    // Rock type 2 - sprite sheet
            new Rectangle(50, 50, 200, 200),    // Rock type 3 - coordinates and
            new Rectangle(50, 50, 200, 200),    // Rock type 4 - dimensions
            new Rectangle(50, 50, 200, 200)     // Rock type 5
        };

        // Scale variations for different rock types
        private readonly Vector2[] _rockScales = new Vector2[]
        {
            new Vector2(0.5f, 0.5f),  // Rock type 0
            new Vector2(0.5f, 0.5f),  // Rock type 1
            new Vector2(0.5f, 0.5f),  // Rock type 2
            new Vector2(0.5f, 0.5f),  // Rock type 3
            new Vector2(0.5f, 0.5f),  // Rock type 4
            new Vector2(0.5f, 0.5f)   // Rock type 5
        };

        public RockSpawner(List<Rock> rocks, Texture2D rockTexture, ScoreManager scoreManager)
        {
            _activeRocks = rocks;
            _random = new Random();
            _spawnTimer = 0;
            _currentMinSpawnTime = _baseMinSpawnTime;
            _currentMaxSpawnTime = _baseMaxSpawnTime;
            _nextSpawnTime = GetRandomSpawnTime();
            _rockTexture = rockTexture;
            _scoreManager = scoreManager;
        }

        public void Update()
        {
            // Update spawn timer
            _spawnTimer += Globals.DeltaTime;

            // Check if it's time to spawn a new rock
            if (_spawnTimer >= _nextSpawnTime)
            {
                SpawnRandomRock();
                _spawnTimer = 0;
                _nextSpawnTime = GetRandomSpawnTime();
            }

            // Update and remove rocks that are out of bounds
            UpdateActiveRocks();
        }

        public void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier)
        {
            _speedMultiplier = speedMultiplier;

            // Decrease spawn times (faster spawning) as difficulty increases
            _currentMinSpawnTime = _baseMinSpawnTime / spawnRateMultiplier;
            _currentMaxSpawnTime = _baseMaxSpawnTime / spawnRateMultiplier;
        }

        private float GetRandomSpawnTime()
        {
            return (float)(_random.NextDouble() * (_currentMaxSpawnTime - _currentMinSpawnTime) + _currentMinSpawnTime);
        }

        private void SpawnRandomRock()
        {
            // Choose a random rock type (0-5)
            int rockType = _random.Next(6);

            // Spawn at random x position at the bottom of the screen
            int xPos = _random.Next(100, Globals.ScreenWidth - 100);
            Vector2 position = new Vector2(xPos, Globals.ScreenHeight + 50); // Start below the screen

            // Apply speed multiplier to the base speed
            float adjustedSpeed = 150 * _speedMultiplier;

            // Create the rock with the determined parameters
            Rock rock = new Rock(
                _rockTexture,
                position,
                adjustedSpeed, // Use adjusted speed
                _rockRects[rockType],
                _rockScales[rockType],
                new Vector2(0, -1) // Moving upward
            );

            _activeRocks.Add(rock);
        }

        private void UpdateActiveRocks()
        {
            // Create a list for rocks to remove
            List<Rock> rocksToRemove = new List<Rock>();

            foreach (Rock rock in _activeRocks)
            {
                // Check if rock is out of bounds
                if (IsOutOfBounds(rock) || !rock.IsActive)
                {
                    rocksToRemove.Add(rock);
                }
            }

            // Remove rocks that are out of bounds or inactive
            foreach (Rock rock in rocksToRemove)
            {
                // Make sure rock is deactivated (which unregisters from CollisionManager)
                rock.Deactivate();
                _activeRocks.Remove(rock);
            }
        }

        private bool IsOutOfBounds(Rock rock)
        {
            // Add extra margin to ensure rock is completely off-screen
            const int margin = 200;

            return rock.Position.X < -margin ||
                   rock.Position.X > Globals.ScreenWidth + margin ||
                   rock.Position.Y < -margin ||
                   rock.Position.Y > Globals.ScreenHeight + margin;
        }

        // No longer needed as this is handled by CollisionManager
        // public void CheckPlayerCollision(Player player) { ... }
    }
}