using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Dredge_lung_test
{
    public class Player : Sprite
    {
        public bool IsHarpoonFiring { get; set; }

        private Vector2 _velocity = Vector2.Zero;
        private Vector2 _acceleration = Vector2.Zero;
        private float _accelerationRate = 600;
        private float _frictionRate = 500;
        private float _returnSpeed = 50; // How quickly to return to default Y\
        private Rectangle _collision;
        private bool _isReturning = false;
        private bool _isAtDefaultPosition = false;
        private float _spriteWidth;
        private float _spriteHeight;
        private float _defaultY; // Store the default Y position
        public static bool ShowCollisionRects = true;

        // Property to allow access to the Bounds
        public new Rectangle Bounds { get; private set; }

        public Player(Texture2D texture, Vector2 position) : base(texture, position)
        {
            Speed = 400;
            Scale = new Vector2(0.15f, 0.15f);
            Position = position;
            Direction = IM.Direction;
            _defaultY = position.Y;
            _spriteWidth = Texture.Width * Scale.X;
            _spriteHeight = Texture.Height * Scale.Y;
        }

        public void Update()
        {
           Movement();
            Bounds = new Rectangle((int)Position.X + 15, (int)Position.Y + 8, 175, 106);
        }

        private void Movement()
        {
            Vector2 inputDirection = IM.Direction;

            // Check if harpoon is firing - if so, restrict movement
            if (IsHarpoonFiring)
            {
                // Disable horizontal movement completely
                _acceleration.X = 0;

                // Apply horizontal friction to gradually stop
                if (_velocity.X != 0)
                {
                    float frictionX = Math.Sign(_velocity.X) * _frictionRate * Globals.DeltaTime;

                    // Ensure we don't overshoot zero
                    if (Math.Abs(frictionX) > Math.Abs(_velocity.X))
                        _velocity.X = 0;
                    else
                        _velocity.X -= frictionX;
                }

                // Disable vertical input but allow return to default Y
                inputDirection.Y = 0;
            }

            // Handle vertical movement with W and S keys - only if not firing
            if (!IsHarpoonFiring && IM.IsKeyPressed(Keys.W))
            {
                // Move up with the same acceleration feel as horizontal
                _acceleration.Y = -_accelerationRate;
                _isReturning = false;
                _isAtDefaultPosition = false;
            }
            else if (!IsHarpoonFiring && IM.IsKeyPressed(Keys.S))
            {
                // Move down with the same acceleration feel as horizontal
                _acceleration.Y = _accelerationRate;
                _isReturning = false;
                _isAtDefaultPosition = false;
            }
            else
            {
                // No vertical input, handle return to default position
                if (Position.Y < _defaultY - 1) // Above default (with small tolerance)
                {
                    // Should return downward to default Y
                    if (!_isReturning)
                    {
                        _isReturning = true;
                        _velocity.Y = _returnSpeed; // Set a fixed downward velocity
                    }
                }
                else if (Position.Y > _defaultY + 1) // Below default (with small tolerance)
                {
                    // Should return upward to default Y
                    if (!_isReturning)
                    {
                        _isReturning = true;
                        _velocity.Y = -_returnSpeed; // Set a fixed upward velocity
                    }
                }
                else
                {
                    // We're at default position (within tolerance)
                    Position = new Vector2(Position.X, _defaultY);
                    _velocity.Y = 0;
                    _acceleration.Y = 0;
                    _isReturning = false;
                    _isAtDefaultPosition = true;
                }
            }

            // Handle horizontal acceleration - only if not firing
            if (!IsHarpoonFiring && inputDirection.X != 0)
            {
                // Apply horizontal acceleration
                _acceleration.X = inputDirection.X * _accelerationRate;
            }
            else if (!IsHarpoonFiring)
            {
                // Apply horizontal friction
                if (_velocity.X != 0)
                {
                    float frictionX = Math.Sign(_velocity.X) * _frictionRate * Globals.DeltaTime;

                    // Ensure we don't overshoot zero
                    if (Math.Abs(frictionX) > Math.Abs(_velocity.X))
                        _velocity.X = 0;
                    else
                        _velocity.X -= frictionX;
                }

                _acceleration.X = 0;
            }


            // Handle vertical movement during return to default
            if (_isReturning)
            {
                // Keep the return velocity constant
                // Check if we've returned to default Y
                if ((_velocity.Y < 0 && Position.Y <= _defaultY) ||
                    (_velocity.Y > 0 && Position.Y >= _defaultY))
                {
                    // We've returned to default, stop the movement
                    Position = new Vector2(Position.X, _defaultY);
                    _velocity.Y = 0;
                    _isReturning = false;
                    _isAtDefaultPosition = true;
                }
            }
            else if (!_isAtDefaultPosition)
            {
                // Apply acceleration to vertical velocity when not returning
                _velocity.Y += _acceleration.Y * Globals.DeltaTime;

                // Apply friction for vertical movement when no keys pressed
                if (_acceleration.Y == 0 && _velocity.Y != 0 && !_isReturning)
                {
                    float frictionY = Math.Sign(_velocity.Y) * _frictionRate * Globals.DeltaTime;

                    // Ensure we don't overshoot zero
                    if (Math.Abs(frictionY) > Math.Abs(_velocity.Y))
                        _velocity.Y = 0;
                    else
                        _velocity.Y -= frictionY;
                }
            }

            // Apply acceleration to horizontal velocity
            _velocity.X += _acceleration.X * Globals.DeltaTime;

            // Clamp velocity to max speed
            if (_velocity.Length() > Speed)
            {
                _velocity = Vector2.Normalize(_velocity) * Speed;
            }

            // Update player position
            Position += _velocity * Globals.DeltaTime;

            // Calculate sprite dimensions
            _spriteWidth = Texture.Width * Scale.X;
            _spriteHeight = Texture.Height * Scale.Y;

            // Keep player within screen bounds
            Position = new Vector2(
                MathHelper.Clamp(Position.X, 0, Globals.ScreenWidth - _spriteWidth),
                MathHelper.Clamp(Position.Y, 0, Globals.ScreenHeight - _spriteHeight)
            );
        }

        public override void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0.9f);

            if (ShowCollisionRects)
            {
                Color debugColor = Color.Red;
                DebugRenderer.DrawRectangle(Bounds, debugColor, 0.9f);
            }
        }
    }
}