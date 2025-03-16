using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public abstract class BaseSpawner<T> : IUpdatable where T : class
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
            // Update spawn timer
            _spawnTimer += Globals.DeltaTime;

            // Check if it's time to spawn a new entity
            if (_spawnTimer >= _nextSpawnTime)
            {
                SpawnRandomEntity();
                _spawnTimer = 0;
                _nextSpawnTime = GetRandomSpawnTime();
            }

            // Update and remove entities that are out of bounds
            UpdateActiveEntities();
        }

        public virtual void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier)
        {
            _speedMultiplier = speedMultiplier;

            // Decrease spawn times (faster spawning) as difficulty increases
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
            // Create a list for entities to remove
            List<T> entitiesToRemove = new List<T>();

            foreach (T entity in _activeEntities)
            {
                // Check if entity is out of bounds
                if (IsOutOfBounds(entity) || !IsEntityActive(entity))
                {
                    entitiesToRemove.Add(entity);
                }
            }

            // Remove entities that are out of bounds or inactive
            foreach (T entity in entitiesToRemove)
            {
                DeactivateEntity(entity);
                _activeEntities.Remove(entity);
            }
        }

        protected bool IsOutOfBounds(T entity)
        {
            // Add extra margin to ensure entity is completely off-screen
            const int margin = 200;
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