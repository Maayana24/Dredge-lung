﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Dredge_lung_test
{
    //Class representing the player submarine character
    public class Player : Sprite, ICollidable
    {
        private ScoreManager _scoreManager => ScoreManager.Instance;
        public bool IsHarpoonFiring { get; set; }

        private Vector2 _velocity = Vector2.Zero; //Current speed
        private Vector2 _acceleration = Vector2.Zero; 
        private float _accelerationRate = 600; //Acceleration applied when moving left or right
        private float _frictionRate = 500; //Force applied when there's no input to slow the player
        private float _returnSpeed = 50; //How quickly to return to default Y

        private bool _isReturning = false;
        private bool _isAtDefaultPosition = false;
        private float _defaultY; //Default Y position for the player to return when not moving vertically


        private float _spriteWidth;
        private float _spriteHeight;

        public static bool ShowCollisionRects = false; //Debug

        public Rectangle Bounds { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Player(Vector2 position) : base(Globals.Content.Load<Texture2D>("Fish/Submarine"), position)
        {
            Speed = 400;
            Scale = new Vector2(0.17f, 0.17f);
            Position = position;
            Direction = IM.Direction;
            _defaultY = position.Y;
            _spriteWidth = Texture.Width * Scale.X;
            _spriteHeight = Texture.Height * Scale.Y;

            ZIndex = 10;
            UpdateLayerDepth();

            CollisionManager.Instance.Register(this);
        }

        public override void Update()
        {
            Movement();
            Bounds = new Rectangle((int)Position.X + 15, (int)Position.Y + 8, 175, 106);
            Position = new Vector2(MathHelper.Clamp(Position.X, PlayableArea.X, PlayableArea.X + PlayableArea.Width - _spriteWidth), MathHelper.Clamp(Position.Y, PlayableArea.Y, PlayableArea.Y + PlayableArea.Height - _spriteHeight));
        }

        private void Movement()
        {
            Vector2 inputDirection = IM.Direction;

            //Restrict movement if harpoon is firing
            if (IsHarpoonFiring)
            {
                _acceleration.X = 0; //Disable horizontal movement

                //Apply horizontal friction to gradually stop
                if (_velocity.X != 0)
                {
                    float frictionX = Math.Sign(_velocity.X) * _frictionRate * Globals.DeltaTime;

                    if (Math.Abs(frictionX) > Math.Abs(_velocity.X)) //To not overshoot zero
                        _velocity.X = 0;
                    else
                        _velocity.X -= frictionX;
                }
                
                inputDirection.Y = 0; //Disable vertical input but still allow return to default Y
            }

            //Vertical movement only if not firing
            if (!IsHarpoonFiring && IM.IsKeyPressed(Keys.W))
            {
                _acceleration.Y = -_accelerationRate;
                _isReturning = false;
                _isAtDefaultPosition = false;
            }
            else if (!IsHarpoonFiring && IM.IsKeyPressed(Keys.S))
            {
                _acceleration.Y = _accelerationRate;
                _isReturning = false;
                _isAtDefaultPosition = false;
            }
            else
            {
                //If no vertical input, return to default position
                if (Position.Y < _defaultY - 1) //Above default
                {
                    //Return downward to default Y
                    if (!_isReturning)
                    {
                        _isReturning = true;
                        _velocity.Y = _returnSpeed; //Set a fixed downward velocity
                    }
                }
                else if (Position.Y > _defaultY + 1) //Below default
                {
                    //return upward to default Y
                    if (!_isReturning)
                    {
                        _isReturning = true;
                        _velocity.Y = -_returnSpeed; //Set a fixed upward velocity
                    }
                }
                else
                {                    
                    Position = new Vector2(Position.X, _defaultY);
                    _velocity.Y = 0;
                    _acceleration.Y = 0;
                    _isReturning = false;
                    _isAtDefaultPosition = true;
                }
            }

            //horizontal acceleration when not firing
            if (!IsHarpoonFiring && inputDirection.X != 0)
            {
                _acceleration.X = inputDirection.X * _accelerationRate; //Apply horizontal acceleration

            }
            else if (!IsHarpoonFiring)
            {
                //Apply horizontal friction
                if (_velocity.X != 0)
                {
                    float frictionX = Math.Sign(_velocity.X) * _frictionRate * Globals.DeltaTime;

                    
                    if (Math.Abs(frictionX) > Math.Abs(_velocity.X)) //To not overshoot zero
                        _velocity.X = 0;
                    else
                        _velocity.X -= frictionX;
                }

                _acceleration.X = 0;
            }

            //vertical movement when return to default Y
            if (_isReturning)
            {
                // Check if we've returned to default Y
                if ((_velocity.Y < 0 && Position.Y <= _defaultY) || (_velocity.Y > 0 && Position.Y >= _defaultY))
                {
                    //When returned to default stop movement
                    Position = new Vector2(Position.X, _defaultY);
                    _velocity.Y = 0;
                    _isReturning = false;
                    _isAtDefaultPosition = true;
                }
            }
            else if (!_isAtDefaultPosition)
            {   
                _velocity.Y += _acceleration.Y * Globals.DeltaTime; //Apply acceleration to vertical velocity when not returning

                //Apply friction for vertical movement when no keys pressed
                if (_acceleration.Y == 0 && _velocity.Y != 0 && !_isReturning)
                {
                    float frictionY = Math.Sign(_velocity.Y) * _frictionRate * Globals.DeltaTime;

                    if (Math.Abs(frictionY) > Math.Abs(_velocity.Y)) //To not overshoot zero
                        _velocity.Y = 0;
                    else
                        _velocity.Y -= frictionY;
                }
            }

            _velocity.X += _acceleration.X * Globals.DeltaTime; //Apply acceleration to horizontal velocity

            //Clamp velocity to max speed
            if (_velocity.Length() > Speed)
            {
                _velocity = Vector2.Normalize(_velocity) * Speed;
            }
            
            Position += _velocity * Globals.DeltaTime; //Update player position

            //Calculate sprite dimensions
            _spriteWidth = Texture.Width * Scale.X;
            _spriteHeight = Texture.Height * Scale.Y;

            //Keep player in playable area
            Position = new Vector2(MathHelper.Clamp(Position.X, 0, Globals.ScreenWidth - _spriteWidth), MathHelper.Clamp(Position.Y, 0, Globals.ScreenHeight - _spriteHeight));
        }

        public void OnCollision(ICollidable other)
        {
            if (other is Rock)
            {
                _scoreManager.RemoveLife(); //Lose a life when colliding with rock
            }
        }
        public override void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth);

            if (ShowCollisionRects) //Debug
            {
                Color debugColor = Color.Red;
                DebugRenderer.DrawRectangle(Bounds, debugColor, LayerDepth - 0.01f);
            }
        }
    }
}