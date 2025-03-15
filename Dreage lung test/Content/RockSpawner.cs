using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class RockSpawner : IUpdatable
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
            new Rectangle(1225, 160, 400, 115),    // Rock type 0 - Update these values
            new Rectangle(1225, 535, 400, 140),    // Rock type 1 - with your actual 
            new Rectangle(765, 628, 240, 117),    // Rock type 2 - sprite sheet
            new Rectangle(670, 905, 480, 145),    // Rock type 3 - coordinates and
            new Rectangle(1320, 895, 300, 120),    // Rock type 4 - dimensions
            new Rectangle(1150, 1320, 600, 200)     // Rock type 5
        };

        // Scale variations for different rock types
        private readonly Vector2[] _rockScales = new Vector2[]
        {
            new Vector2(0.6f, 0.6f),  // Rock type 0
            new Vector2(0.57f, 0.57f),  // Rock type 1
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

            // Spawn at random x position within the playable area
            float xPos = _random.Next(PlayableArea.X, PlayableArea.X + PlayableArea.Width);

            // CHANGE THIS LINE: Position rocks to spawn from top, not bottom
            // They should start above the screen and move downward
            Vector2 position = new Vector2(xPos, PlayableArea.Y + PlayableArea.Height + 100);

            // Apply speed multiplier to the base speed
            float adjustedSpeed = 150 * _speedMultiplier;

            // Create the rock with the determined parameters
            Rock rock = new Rock(
                _rockTexture,
                position,
                adjustedSpeed,
                _rockRects[rockType],
                _rockScales[rockType],
                new Vector2(0, -1) // CHANGE THIS: Direction should be downward (0, 1)
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

            return rock.Position.X < PlayableArea.X - margin ||
                   rock.Position.X > PlayableArea.X + PlayableArea.Width + margin ||
                   rock.Position.Y < PlayableArea.Y - margin ||
                   rock.Position.Y > PlayableArea.Y + PlayableArea.Height + margin;
        }

        // No longer needed as this is handled by CollisionManager
        // public void CheckPlayerCollision(Player player) { ... }
    }
}