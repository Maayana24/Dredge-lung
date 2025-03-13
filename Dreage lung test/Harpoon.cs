using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public enum HarpoonState
    {
        Ready,
        Firing,
        Retracting,
        Cooldown
    }

    public class Harpoon
    {
        // References
        private readonly Player _player;
        private readonly List<Fish> _fishes;
        private readonly ScoreManager _scoreManager;

        public float CooldownTimer { get; private set; }
        public float CooldownDuration { get; private set; }
        public HarpoonState State { get; private set; }
        public bool IsInCooldown => State == HarpoonState.Cooldown;

        // Harpoon properties
        private Vector2 _position;
        private Vector2 _origin;
        private Vector2 _direction;
        private Vector2 _tip;
        private float _length;
        private float _maxLength;
        private float _speed;
        private Fish _caughtFish;
        private bool _showCollisionRect = true;
        private Rectangle _collisionRect;
        private float _tipSize = 10f; // Size of the harpoon tip for collision

        // Cached pixel texture for drawing
        private readonly Texture2D _pixelTexture;

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
            _maxLength = 400; // Maximum range
            _speed = 300; // Speed of the harpoon
            State = HarpoonState.Ready;
            CooldownTimer = 0f;
            CooldownDuration = 3f; // 3 second Cooldown
            _caughtFish = null;

            // Create pixel texture once
            _pixelTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void Update()
        {
            // Handle Cooldown first
            if (State == HarpoonState.Cooldown)
            {
                _player.IsHarpoonFiring = false;
                CooldownTimer += Globals.DeltaTime;

                if (CooldownTimer >= CooldownDuration)
                {
                    State = HarpoonState.Ready;
                    CooldownTimer = 0f;
                }
                return; // Skip the rest of the update if in Cooldown
            }

            // Update origin position based on player position
            _origin = new Vector2(_player.Bounds.X + _player.Bounds.Width / 2f, _player.Bounds.Y + _player.Bounds.Height / 1.3f);

            // Update direction based on mouse position if in Ready state
            if (State == HarpoonState.Ready)
            {
                UpdateAiming();

                // Fire harpoon on click
                if (IM.MouseClicked)
                {
                    Fire();
                }
            }
            // Handle Firing state
            else if (State == HarpoonState.Firing)
            {
                _length += _speed * Globals.DeltaTime;
                UpdateTipPosition();

                // Check for fish collision
                CheckFishCollision();

                // Check if max length reached
                if (_length >= _maxLength)
                {
                    State = HarpoonState.Retracting;
                }
            }
            // Handle Retracting state
            else if (State == HarpoonState.Retracting)
            {
                _length -= _speed * 1.8f * Globals.DeltaTime; // Retract faster
                UpdateTipPosition();

                // Update caught fish position if any
                if (_caughtFish != null)
                {
                    _caughtFish.Position = _tip;
                }

                // Check if fully retracted
                if (_length <= 0)
                {
                    _length = 0;
                    State = HarpoonState.Cooldown;

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

            // Avoid division by zero
            if (dirToMouse.Length() > 0)
            {
                dirToMouse.Normalize();
            }
            else
            {
                dirToMouse = new Vector2(1, 0); // Default direction
                return;
            }

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
            if (State == HarpoonState.Ready)
            {
                State = HarpoonState.Firing;
                _player.IsHarpoonFiring = true;
                _length = 0f;  // Start with zero length
                _caughtFish = null;
                UpdateTipPosition(); // Update tip position immediately
            }
        }

        private void UpdateTipPosition()
        {
            _tip = _origin + _direction * _length;
            UpdateCollisionRect();
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
            if (State != HarpoonState.Firing || _caughtFish != null)
                return;

            foreach (Fish fish in _fishes)
            {
                if (_collisionRect.Intersects(fish.Bounds))
                {
                    _caughtFish = fish;
                    State = HarpoonState.Retracting;
                    break;
                }
            }
        }

        private void ProcessCaughtFish()
        {
            _player.IsHarpoonFiring = false;

            if (_caughtFish != null)
            {
                // Determine if the fish has anomalies
                bool hasAnomalies = _caughtFish.HasAnomalies;

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
            // Don't draw anything if in Cooldown state
            if (State == HarpoonState.Cooldown)
                return;

            // Don't draw if in Ready state with no aim
            if (State == HarpoonState.Ready && _direction == Vector2.Zero)
                return;

            // Calculate the line's start and end points
            Vector2 start = _origin;
            Vector2 end = State == HarpoonState.Ready
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
            if (State != HarpoonState.Ready)
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