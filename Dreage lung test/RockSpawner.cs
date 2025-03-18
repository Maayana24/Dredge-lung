using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class RockSpawner : BaseSpawner<Rock>
    {
        private readonly Texture2D _rockTexture;

        //Manually defined rectangles for each rock type
        private readonly Rectangle[] _rockRects = new Rectangle[]
        {
            new Rectangle(1225, 160, 400, 115),
            new Rectangle(1225, 535, 400, 140),
            new Rectangle(765, 628, 240, 117),
            new Rectangle(670, 905, 480, 145),
            new Rectangle(1320, 895, 300, 120),
            new Rectangle(1150, 1320, 600, 200)
        };

        //Scale for each rock types
        private readonly Vector2[] _rockScales = new Vector2[]
        {
            new Vector2(0.6f, 0.6f),
            new Vector2(0.57f, 0.57f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f)
        };

        public RockSpawner(List<Rock> rocks) : base(rocks, 3.0f, 4.0f) //baseMinSpawnTime, baseMaxSpawnTime
        {
        }

        protected override void SpawnRandomEntity()
        {
            int rockType = _random.Next(6);
            
            float xPos = _random.Next(PlayableArea.X, PlayableArea.X + PlayableArea.Width); //Spawn at random x position inside the playable area

            Vector2 position = new Vector2(xPos, PlayableArea.Y + PlayableArea.Height + 100); //Position rocks to spawn from bottom of the screen

            float adjustedSpeed = 150 * _speedMultiplier;

            //Create the rock with the chosen attributes
            Rock rock = new Rock(_rockTexture, position, adjustedSpeed, _rockRects[rockType], _rockScales[rockType], new Vector2(0, -1));

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
            rock.Deactivate();
        }
    }
}