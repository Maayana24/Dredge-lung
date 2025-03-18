using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dredge_lung_test
{
    public enum HarpoonState //Enum for harpoon states
    {
        Ready,
        Firing,
        Retracting,
        Cooldown
    }
    //Representing the harpoon, the player's weapon
    public class Harpoon : Sprite, ICollidable
    {
        private readonly Player _player;
        private readonly List<Fish> _fishes;
        private ScoreManager _scoreManager => ScoreManager.Instance;
        public float CooldownTimer { get; private set; }
        public float CooldownDuration { get; private set; }
        public HarpoonState State { get; private set; }
        public bool IsInCooldown => State == HarpoonState.Cooldown;
        public bool IsActive => State == HarpoonState.Firing || State == HarpoonState.Retracting;

        private Vector2 _origin;
        private Vector2 _tip;
        private float _length;
        private float _maxLength;
        private Fish _caughtFish;
        private bool _showCollisionRect = false;
        private Rectangle _collisionRect;
        private float _tipSize = 10f; //Size of the harpoon tip for collision

        private readonly Texture2D _pixelTexture; //Pixel texture for drawing

        private readonly float _harpoonThickness = 3f;
        private readonly Color _harpoonColor = Color.Black;

        public Harpoon(Player player, List<Fish> fishes) : base(new Texture2D(Globals.GraphicsDevice, 1, 1), player.Position)
        {
            _player = player;
            _fishes = fishes;
            _origin = player.Position;
            _length = 0f;
            _maxLength = 400; //Maximum range
            Speed = 300; //Speed of the harpoon
            State = HarpoonState.Ready;
            CooldownTimer = 0f;
            CooldownDuration = 3f; //3 second Cooldown
            _caughtFish = null;

            ZIndex = _player.ZIndex + 1; //Slightly below player
            UpdateLayerDepth();

            //Creating pixel texture
            _pixelTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            //Initialize collision rectangle
            _collisionRect = new Rectangle(0, 0, (int)_tipSize, (int)_tipSize);
            Bounds = _collisionRect;
        }

        public override void Update()
        {
            if (State == HarpoonState.Cooldown)
            {
                _player.IsHarpoonFiring = false;
                CooldownTimer += Globals.DeltaTime;

                if (CooldownTimer >= CooldownDuration)
                {
                    State = HarpoonState.Ready;
                    CooldownTimer = 0f;
                }
                return; //Skip the rest of Update if in cooldown
            }

            _origin = new Vector2(_player.Bounds.X + _player.Bounds.Width / 2f, _player.Bounds.Y + _player.Bounds.Height / 1.3f); //Update origin position based on player position

            if (State == HarpoonState.Ready)
            {
                UpdateAiming();

                //Fire harpoon on click
                if (IM.MouseClicked)
                {
                    Fire();
                }
            }
            else if (State == HarpoonState.Firing)
            {
                _length += Speed * Globals.DeltaTime;
                UpdateTipPosition();

                //Check if reached max length then retract
                if (_length >= _maxLength)
                {
                    State = HarpoonState.Retracting;
                }
            }
            else if (State == HarpoonState.Retracting)
            {
                _length -= Speed * 1.8f * Globals.DeltaTime; //Retract faster then firing
                UpdateTipPosition();

                //Update caught fish position
                if (_caughtFish != null)
                {
                    _caughtFish.Position = _tip;
                }

                //Check if fully retracted
                if (_length <= 0)
                {
                    _length = 0;
                    UnregisterFromCollisionManager();
                    State = HarpoonState.Cooldown;

                    //Process caught fish
                    if (_caughtFish != null)
                    {
                        ProcessCaughtFish();
                    }
                }
            }
        }

        private void UpdateAiming()
        {
            Vector2 mousePos = IM.MousePosition;

            Vector2 dirToMouse = mousePos - _origin; //Calculate direction from player to mouse

            //Avoid division by zero
            if (dirToMouse.Length() > 0)
            {
                dirToMouse.Normalize();
            }
            else
            {
                dirToMouse = new Vector2(1, 0); //Default direction
                return;
            }

            //Restrict to the bottom of the player in half-circle
            if (dirToMouse.Y < 0)
            {
                //If pointing above horizon, clamp to horizon
                if (dirToMouse.X > 0)
                    dirToMouse = new Vector2(1, 0);
                else if (dirToMouse.X < 0)
                    dirToMouse = new Vector2(-1, 0);
                else
                    dirToMouse = new Vector2(1, 0); //Default if directly above
            }

            Direction = dirToMouse;
        }

        private void Fire()
        {
            if (State == HarpoonState.Ready)
            {
                State = HarpoonState.Firing;
                _player.IsHarpoonFiring = true;
                _length = 0f;  //Start with zero length
                _caughtFish = null;
                UpdateTipPosition(); //Update tip position
             
                RegisterWithCollisionManager(); //Register with collision manager when firing
            }
        }

        private void UpdateTipPosition()
        {
            _tip = _origin + Direction * _length;
            Position = _tip; //Update the sprite position to the tip
            UpdateCollisionRect();
        }

        private void UpdateCollisionRect()
        {
            _collisionRect = new Rectangle((int)(_tip.X - _tipSize / 2), (int)(_tip.Y - _tipSize / 2), (int)_tipSize, (int)_tipSize);
            Bounds = _collisionRect;
        }

        private void RegisterWithCollisionManager()
        {
            CollisionManager.Instance.Register(this);
        }

        private void UnregisterFromCollisionManager()
        {
            CollisionManager.Instance.Unregister(this);
        }

        public void OnCollision(ICollidable other)
        {
            if (State != HarpoonState.Firing) return; // Only process collision if Firing 

            //Collision with fish
            if (other is Fish fish && _caughtFish == null)
            {
                _caughtFish = fish;
                State = HarpoonState.Retracting;
            }

            //Retract when hitting a rock
            if (other is Rock)
            {
                State = HarpoonState.Retracting;
            }
        }
        private void ProcessCaughtFish()
        {
            _player.IsHarpoonFiring = false;

            if (_caughtFish != null)
            {
                bool hasAnomalies = _caughtFish.HasAnomalies; //Check if the fish has anomalies

                //Add a point if caught fish has anomalies
                if (hasAnomalies)
                {
                    _scoreManager.AddPoints(1);
                }

                _caughtFish.Deactivate(); //After caught deactivate the fish

                _fishes.Remove(_caughtFish);
                _caughtFish = null;
            }
        }

        public override void UpdateLayerDepth()
        {
            base.UpdateLayerDepth();

            LayerDepth = MathHelper.Clamp(LayerDepth, 0.0f, 1.0f);
        }

        public override void Draw()
        {
            //Don't draw if in Cooldown state
            if (State == HarpoonState.Cooldown)
                return;

            //Don't draw if in Ready state with no aim
            if (State == HarpoonState.Ready && Direction == Vector2.Zero)
                return;

            //Start and end of harpoon line
            Vector2 start = _origin;
            //End of harpoon if ready should change with direction and should be at the tip in any other state
            Vector2 end = State == HarpoonState.Ready ? _origin + Direction * 50 : _tip;

            //Calculate the line length and angle
            float lineLength = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(Direction.Y, Direction.X);

            //Draw the line
            Globals.SpriteBatch.Draw(
                _pixelTexture,
                start,
                null,
                _harpoonColor,
                angle,
                Vector2.Zero,
                new Vector2(lineLength, _harpoonThickness),
                SpriteEffects.None,
                LayerDepth
            );

            //Slightly larger tip at the end
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
                    LayerDepth
                );

                //Collision rectangle for debugging
                if (_showCollisionRect)
                {
                    DebugRenderer.DrawRectangle(_collisionRect, Color.Yellow, LayerDepth + 0.01f);
                }
            }
        }
    }
}