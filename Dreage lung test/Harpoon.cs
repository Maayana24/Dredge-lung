using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public enum HarpoonState
    {
        READY,
        FIRING,
        RETRACTING,
        COOLDOWN
    }

    public class Harpoon
    {
        // References
        private Player _player;
        private List<Fish> _fishes;
        private ScoreManager _scoreManager;

        // Harpoon properties
        private Vector2 _position;
        private Vector2 _origin;
        private Vector2 _direction;
        private Vector2 _tip;
        private float _length;
        private float _maxLength;
        private float _speed;
        private HarpoonState _state;
        private float _cooldownTimer;
        private float _cooldownDuration;
        private Fish _caughtFish;
        private bool _showCollisionRect = true;
        private Rectangle _collisionRect;
        private float _tipSize = 10f; // Size of the harpoon tip for collision

        // Cached pixel texture for drawing
        private Texture2D _pixelTexture;

        // Constants
        private readonly float _harpoonThickness = 3f;
        private readonly Color _harpoonColor = Color.Black;
        private readonly float _harpoonLayer = 0.85f;

        public Harpoon(Player player, List<Fish> fishes, ScoreManager scoreManager)
        {
            _player = player;
            _fishes = fishes;
            _scoreManager = scoreManager;
            _position = player.Position;
            _origin = _position;
            _direction = Vector2.Zero;
            _length = 0f;
            _maxLength = 500f; // Maximum range
            _speed = 600f; // Speed of the harpoon
            _state = HarpoonState.READY;
            _cooldownTimer = 0f;
            _cooldownDuration = 3f; // 3 second cooldown
            _caughtFish = null;

            // Create pixel texture once
            _pixelTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void Update()
        {
            Debug.WriteLine("State: " + _state);

            // Handle cooldown first
            if (_state == HarpoonState.COOLDOWN)
            {
                _cooldownTimer += Globals.DeltaTime;
                if (_cooldownTimer >= _cooldownDuration)
                {
                    _state = HarpoonState.READY;
                    _cooldownTimer = 0f;
                }
                return; // Skip the rest of the update if in cooldown
            }

            // Update origin position based on player position
            _origin = new Vector2(
                _player.Position.X + _player.Bounds.Width / 2,
                _player.Position.Y + _player.Bounds.Height / 2
            );

            // Update direction based on mouse position if in READY state
            if (_state == HarpoonState.READY)
            {
                UpdateAiming();

                // Fire harpoon on click
                if (IM.MouseClicked)
                {
                    Fire();
                }
            }
            // Handle firing state
            else if (_state == HarpoonState.FIRING)
            {
                _length += _speed * Globals.DeltaTime;
                _tip = _origin + _direction * _length;
                UpdateCollisionRect();

                // Check for fish collision
                CheckFishCollision();

                // Check if max length reached
                if (_length >= _maxLength)
                {
                    _state = HarpoonState.RETRACTING;
                }
            }
            // Handle retracting state
            else if (_state == HarpoonState.RETRACTING)
            {
                _length -= _speed * 1.5f * Globals.DeltaTime; // Retract faster
                _tip = _origin + _direction * _length;
                UpdateCollisionRect();

                // Update caught fish position if any
                if (_caughtFish != null)
                {
                    _caughtFish.Position = _tip;
                }

                // Check if fully retracted
                if (_length <= 0)
                {
                    _length = 0;
                    _state = HarpoonState.COOLDOWN;

                    // Process caught fish
                    if (_caughtFish != null)
                    {
                        ProcessCaughtFish();
                    }
                }
            }
        }

        private void UpdateAiming()
        {
            // Get mouse position
            Vector2 mousePos = IM.MousePosition;

            // Calculate direction vector from player to mouse
            Vector2 dirToMouse = mousePos - _origin;
            dirToMouse.Normalize();

            // Restrict to bottom half-circle (y ≥ 0 in player's local space)
            if (dirToMouse.Y < 0)
            {
                // If pointing above horizon, clamp to horizon
                if (dirToMouse.X > 0)
                    dirToMouse = new Vector2(1, 0);
                else if (dirToMouse.X < 0)
                    dirToMouse = new Vector2(-1, 0);
                else
                    dirToMouse = new Vector2(1, 0); // Default if directly above
            }

            _direction = dirToMouse;
        }

        private void Fire()
        {
            if (_state == HarpoonState.READY)
            {
                _state = HarpoonState.FIRING;
                _length = 0f;
                _caughtFish = null;
            }
        }

        private void UpdateCollisionRect()
        {
            _collisionRect = new Rectangle(
                (int)(_tip.X - _tipSize / 2),
                (int)(_tip.Y - _tipSize / 2),
                (int)_tipSize,
                (int)_tipSize
            );
        }

        private void CheckFishCollision()
        {
            if (_state != HarpoonState.FIRING || _caughtFish != null)
                return;

            foreach (Fish fish in _fishes)
            {
                if (_collisionRect.Intersects(fish.Bounds))
                {
                    _caughtFish = fish;
                    _state = HarpoonState.RETRACTING;
                    break;
                }
            }
        }

        private void ProcessCaughtFish()
        {
            if (_caughtFish != null)
            {
                // Determine if the fish has anomalies
                bool hasAnomalies = _caughtFish.HasDeadlyAnomaly;

                // Award points or deduct lives
                if (hasAnomalies)
                {
                    _scoreManager.AddPoints(1);
                }
                else
                {
                    _scoreManager.RemoveLife();
                }

                // Remove the fish from the game
                _fishes.Remove(_caughtFish);
                _caughtFish = null;
            }
        }

        public void Draw()
        {
            // Don't draw anything if in cooldown state
            if (_state == HarpoonState.COOLDOWN)
                return;

            // Don't draw if in READY state with no aim
            if (_state == HarpoonState.READY && _direction == Vector2.Zero)
                return;

            // Calculate the line's start and end points
            Vector2 start = _origin;
            Vector2 end = _state == HarpoonState.READY
                ? _origin + _direction * 50 // Show aim line
                : _tip;

            // Calculate the line length and angle
            float lineLength = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(_direction.Y, _direction.X);

            // Draw the line
            Globals.SpriteBatch.Draw(
                _pixelTexture,
                start,
                null,
                _harpoonColor,
                angle,
                Vector2.Zero,
                new Vector2(lineLength, _harpoonThickness),
                SpriteEffects.None,
                _harpoonLayer
            );

            // Draw a slightly larger tip at the end of the harpoon
            if (_state != HarpoonState.READY)
            {
                Globals.SpriteBatch.Draw(
                    _pixelTexture,
                    end,
                    null,
                    _harpoonColor,
                    0,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(_tipSize),
                    SpriteEffects.None,
                    _harpoonLayer
                );

                // Draw collision rectangle for debugging
                if (_showCollisionRect)
                {
                    DebugRenderer.DrawRectangle(_collisionRect, Color.Yellow, _harpoonLayer + 0.01f);
                }
            }
        }
    }
}