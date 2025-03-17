using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    //Base class for the game's spawners to make it easy to add more in the future
    public abstract class BaseSpawner<T> : IUpdatable where T : class //All spawners should be updatable
    {
        protected readonly List<T> _activeEntities;
        protected readonly Random _random;
        protected float _spawnTimer;
        protected float _baseMinSpawnTime;
        protected float _baseMaxSpawnTime;
        protected float _currentMinSpawnTime;
        protected float _currentMaxSpawnTime;
        protected float _nextSpawnTime;
        protected float _speedMultiplier = 1.0f;

        protected BaseSpawner(List<T> entities, float baseMinSpawnTime, float baseMaxSpawnTime)
        {
            _activeEntities = entities;
            _random = new Random();
            _spawnTimer = 0;
            _baseMinSpawnTime = baseMinSpawnTime;
            _baseMaxSpawnTime = baseMaxSpawnTime;
            _currentMinSpawnTime = _baseMinSpawnTime;
            _currentMaxSpawnTime = _baseMaxSpawnTime;
            _nextSpawnTime = GetRandomSpawnTime();
        }

        public virtual void Update()
        {
            _spawnTimer += Globals.DeltaTime; //Updating the spawn timer

            //Check to see if it's time to spawn a new entity
            if (_spawnTimer >= _nextSpawnTime)
            {
                SpawnRandomEntity();
                _spawnTimer = 0;
                _nextSpawnTime = GetRandomSpawnTime();
            }

            UpdateActiveEntities(); //Update and remove entities that are out of the playeble area
        }
        public virtual void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier) //Changing speed and spawn time based on difficulty
        {
            _speedMultiplier = speedMultiplier;

            _currentMinSpawnTime = _baseMinSpawnTime / spawnRateMultiplier;
            _currentMaxSpawnTime = _baseMaxSpawnTime / spawnRateMultiplier;
        }

        protected float GetRandomSpawnTime()
        {
            return (float)(_random.NextDouble() * (_currentMaxSpawnTime - _currentMinSpawnTime) + _currentMinSpawnTime);
        }

        protected abstract void SpawnRandomEntity();

        protected virtual void UpdateActiveEntities()
        {
            List<T> entitiesToRemove = new List<T>(); //List of entities that should be removed

            foreach (T entity in _activeEntities)
            {
                //Removing entities that are out of playable area or inactive
                if (IsOutOfBounds(entity) || !IsEntityActive(entity))
                {
                    entitiesToRemove.Add(entity);
                }
            }

            foreach (T entity in entitiesToRemove)
            {
                DeactivateEntity(entity);
                _activeEntities.Remove(entity);
            }
        }

        protected bool IsOutOfBounds(T entity) //Check if entity is out of playable area
        {
            const int margin = 200; //Adding extra margin to be sure entity is completely out of playable area
            Vector2 position = GetEntityPosition(entity);

            return position.X < PlayableArea.X - margin ||
                   position.X > PlayableArea.X + PlayableArea.Width + margin ||
                   position.Y < PlayableArea.Y - margin ||
                   position.Y > PlayableArea.Y + PlayableArea.Height + margin;
        }

        protected abstract Vector2 GetEntityPosition(T entity);
        protected abstract bool IsEntityActive(T entity);
        protected abstract void DeactivateEntity(T entity);
    }
}