using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class RockSpawner : BaseSpawner<Rock>
    {
        private readonly Texture2D _rockTexture;
        private readonly ScoreManager _scoreManager;

        // Rock attributes - manually defined rectangles for each rock type
        private readonly Rectangle[] _rockRects = new Rectangle[]
        {
            new Rectangle(1225, 160, 400, 115),    // Rock type 0
            new Rectangle(1225, 535, 400, 140),    // Rock type 1
            new Rectangle(765, 628, 240, 117),     // Rock type 2
            new Rectangle(670, 905, 480, 145),     // Rock type 3
            new Rectangle(1320, 895, 300, 120),    // Rock type 4
            new Rectangle(1150, 1320, 600, 200)    // Rock type 5
        };

        // Scale variations for different rock types
        private readonly Vector2[] _rockScales = new Vector2[]
        {
            new Vector2(0.6f, 0.6f),   // Rock type 0
            new Vector2(0.57f, 0.57f), // Rock type 1
            new Vector2(0.5f, 0.5f),   // Rock type 2
            new Vector2(0.5f, 0.5f),   // Rock type 3
            new Vector2(0.5f, 0.5f),   // Rock type 4
            new Vector2(0.5f, 0.5f)    // Rock type 5
        };

        public RockSpawner(List<Rock> rocks, Texture2D rockTexture, ScoreManager scoreManager)
            : base(rocks, 3.0f, 4.0f) // baseMinSpawnTime, baseMaxSpawnTime
        {
            _rockTexture = rockTexture;
            _scoreManager = scoreManager;
        }

        protected override void SpawnRandomEntity()
        {
            // Choose a random rock type (0-5)
            int rockType = _random.Next(6);

            // Spawn at random x position within the playable area
            float xPos = _random.Next(PlayableArea.X, PlayableArea.X + PlayableArea.Width);

            // Position rocks to spawn from bottom of the screen
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
                new Vector2(0, -1) // Direction is upward
            );

            _activeEntities.Add(rock);
        }

        protected override Vector2 GetEntityPosition(Rock rock)
        {
            return rock.Position;
        }

        protected override bool IsEntityActive(Rock rock)
        {
            return rock.IsActive;
        }

        protected override void DeactivateEntity(Rock rock)
        {
            // Make sure rock is deactivated (which unregisters from CollisionManager)
            rock.Deactivate();
        }
    }
}