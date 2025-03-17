using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public class FishSpawner : BaseSpawner<Fish>
    {
        private const int JellySpawnChance = 10; //Chance to spawn a Jelly

        public FishSpawner(List<Fish> fishes) : base(fishes, 0.5f, 2.0f) //baseMinSpawnTime, baseMaxSpawnTime
        {
            AnomalyManager.Instance.ToString(); //Initialize anomaly manager
        }

        public override void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier) //Override OnDifficultyChanged for the custom fish values
        {
            _speedMultiplier = speedMultiplier;
            _currentMinSpawnTime = _baseMinSpawnTime / (spawnRateMultiplier / 2);
            _currentMaxSpawnTime = _baseMaxSpawnTime / (spawnRateMultiplier / 2);
        }

        protected override void SpawnRandomEntity()
        {
            if (_random.Next(JellySpawnChance) == 0) //Spawn jellyfish according to the spawn chance
            {
                SpawnJelly();
            }
            else //If not a jelly fish spawn a different type
            {
                int fishType = _random.Next(4); //Number of fish types (not including jellyfish)
                bool leftToRight = _random.Next(2) == 0; //50% chance for each direction

                SpawnFish(fishType, leftToRight);
            }
        }

        private void SpawnFish(int fishType, bool leftToRight) //Spawn a fish by type and spawn direction
        {
            //Set starting position based on direction (off-screen)
            float xPos = leftToRight ? PlayableArea.X - 100 : PlayableArea.X + PlayableArea.Width + 100;
            float yPos = _random.Next(PlayableArea.Y + 200, PlayableArea.Y + PlayableArea.Height - 200);
            Vector2 position = new Vector2(xPos, yPos);

            (string fishName, Rectangle sourceRect, Vector2 scale, float baseSpeed) = GetFishAttributes(fishType); //Gets the attributes of the chosen fish type

            float adjustedSpeed = baseSpeed * _speedMultiplier; //Change speed based on speed multiplier

            Fish fish = new Fish(fishName, position, adjustedSpeed, sourceRect, scale); //Create the fish with the chosen attributes

            fish.Direction = leftToRight ? new Vector2(1, 0) : new Vector2(-1, 0); //Set the direction based on spawn side

            _activeEntities.Add(fish);
        }

        private void SpawnJelly()
        {
            //Spawn at random x position at the bottom of the screen
            int xPos = _random.Next(PlayableArea.X + 100, PlayableArea.X + PlayableArea.Width - 100);
            Vector2 position = new Vector2(xPos, PlayableArea.Y + PlayableArea.Height + 50);

            Rectangle sourceRect = new Rectangle(150, 210, 250, 300);

            float adjustedSpeed = 120 * _speedMultiplier; //Change speed based on speed multiplier

            Fish jelly = new Fish("Jelly", position, adjustedSpeed, sourceRect, new Vector2(0.4f, 0.4f), new Vector2(0, -1));
            _activeEntities.Add(jelly);
        }

        private (string fishName, Rectangle sourceRect, Vector2 scale, float speed) GetFishAttributes(int fishType) //Returns the correct attributes based on the fish type
        {
            switch (fishType)
            {
                case 0:
                    return ("Grouper", new Rectangle(275, 50, 175, 105), new Vector2(0.45f, 0.45f), 150);
                case 1:
                    return ("Angler", new Rectangle(600, 10, 250, 175), new Vector2(0.5f, 0.5f), 180);
                case 2:
                    return ("Eel", new Rectangle(450, 250, 350, 200), new Vector2(0.37f, 0.37f), 220);
                case 3:
                    return ("Shark", new Rectangle(150, 550, 600, 200), new Vector2(0.4f, 0.4f), 100);
                default:
                    return ("Grouper", new Rectangle(275, 50, 175, 105), new Vector2(0.45f, 0.45f), 150);
            }
        }

        protected override Vector2 GetEntityPosition(Fish fish)
        {
            return fish.Position;
        }

        protected override bool IsEntityActive(Fish fish)
        {
            return fish.IsActive;
        }

        protected override void DeactivateEntity(Fish fish)
        {
            fish.Deactivate();
        }
    }
}