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
        private readonly float _minSpawnTime = 1.5f; // Slower spawn rate than fish
        private readonly float _maxSpawnTime = 4.0f;
        private float _nextSpawnTime;
        private readonly Texture2D _rockTexture;
        private readonly ScoreManager _scoreManager;

        // Rock attributes - manually defined rectangles for each rock type
        private readonly Rectangle[] _rockRects = new Rectangle[]
        {
            new Rectangle(50, 50, 200, 200),    // Rock type 0 - Update these values
            new Rectangle(50, 50, 200, 200),  // Rock type 1 - with your actual 
            new Rectangle(50, 50, 200, 200),  // Rock type 2 - sprite sheet
            new Rectangle(50, 50, 200, 200),  // Rock type 3 - coordinates and
            new Rectangle(50, 50, 200, 200),    // Rock type 4 - dimensions
            new Rectangle(50, 50, 200, 200)   // Rock type 5
        };

        // Scale variations for different rock types
        private readonly Vector2[] _rockScales = new Vector2[]
        {
            new Vector2(1f, 1f),  // Rock type 0
            new Vector2(1f, 1f),  // Rock type 1
            new Vector2(1f, 1f),  // Rock type 2
            new Vector2(1f, 1f),  // Rock type 3
            new Vector2(1f, 1f),  // Rock type 4
            new Vector2(1f, 1f)   // Rock type 5
        };

        public RockSpawner(List<Rock> rocks, Texture2D rockTexture, ScoreManager scoreManager)
        {
            _activeRocks = rocks;
            _random = new Random();
            _spawnTimer = 0;
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

        private float GetRandomSpawnTime()
        {
            return (float)(_random.NextDouble() * (_maxSpawnTime - _minSpawnTime) + _minSpawnTime);
        }

        private void SpawnRandomRock()
        {
            // Choose a random rock type (0-5)
            int rockType = _random.Next(6);

            // Spawn at random x position at the bottom of the screen
            int xPos = _random.Next(100, Globals.ScreenWidth - 100);
            Vector2 position = new Vector2(xPos, Globals.ScreenHeight + 50); // Start below the screen

            // Create the rock with the determined parameters
            Rock rock = new Rock(
                _rockTexture,
                position,
                150, // Base speed
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

        public void CheckPlayerCollision(Player player)
        {
            foreach (Rock rock in _activeRocks)
            {
                if (rock.CheckCollision(player.Bounds))
                {
                    // Deactivate the rock
                    rock.Deactivate();

                    // Remove a life from the player
                    _scoreManager.RemoveLife();

                    // Could add effects or sounds here

                    // No need to check other rocks once hit
                    break;
                }
            }
        }
    }
}