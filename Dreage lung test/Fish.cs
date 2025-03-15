using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace Dredge_lung_test
{
    public class Fish : Sprite, ICollidable, ILayerable
    {
        public Rectangle SourceRect { get; private set; } //WHATS THE DIFFIRENCE
        private Rectangle CollisionRect { get; set; }
        public string Name { get; private set; } //So I could easily differentiate the fish 
        public List<Anomaly> Anomalies { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool HasAnomalies => Anomalies.Count > 0; //To check if the fish has anomalies

        public static bool ShowCollisionRects = true; //Debug


        public Fish(string name, Vector2 position, float speed = 150, Rectangle sourceRect = default, Vector2? scale = null, Vector2? direction = null)
        : base(Globals.Content.Load<Texture2D>("Fish/FishTemplate"), position) //All fish textures are in one sprite sheet
        {
            Name = name;
            Speed = speed;
            Direction = direction ?? Direction; //Not all fish needs a special direction
            SourceRect = sourceRect.Width > 0 && sourceRect.Height > 0 ? sourceRect : new Rectangle(0, 0, 100, 100); //Validity check for source rectangle
            Scale = scale ?? Scale; //If fish needs to change scale

            Anomalies = AnomalyManager.Instance.GenerateAnomaliesForFish(this); //Generating the anomalies

            ZIndex = 30;
            LayerDepth = 0.8f;
            UpdateLayerDepth();

            // Initial collision rect setup????
            UpdateCollisionRect();

            // Register with collision manager????
            CollisionManager.Instance.Register(this);
        }

        public override void Update()
        {
            if (!IsActive) return;

            Movement();
            SpriteEffect = Direction.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None; //Flipping fish sprite based on direction
            UpdateCollisionRect();
        }

        public override void UpdateLayerDepth()
        {
            base.UpdateLayerDepth();

            //Updating anomalies layer to be slightly above the fish layer
            foreach (var anomaly in Anomalies)
            {
                anomaly.LayerDepth = LayerDepth + 0.01f;
            }
        }

        public virtual void Movement()
        {
            Position += Speed * Direction * Globals.DeltaTime;
        }

        protected virtual void UpdateCollisionRect()
        {
            int width = (int)(SourceRect.Width * Scale.X);
            int height = (int)(SourceRect.Height * Scale.Y);
            CollisionRect = new Rectangle((int)(Position.X - width / 2), (int)(Position.Y - height / 2), width, height); //COLLISION SOURCERECT AND BOUNDS????
            Bounds = CollisionRect;
        }

        public Rectangle GetSourceRect() //????
        {
            return SourceRect;
        }

        public override void Draw()
        {
            if (!IsActive) return;

            //Drawing the fish
            Globals.SpriteBatch.Draw(Texture, Position, SourceRect, Color.White, 0, new Vector2(SourceRect.Width / 2, SourceRect.Height / 2), Scale, SpriteEffect, LayerDepth);

            //Drawing anomalies on top of the fish
            foreach (var anomaly in Anomalies)
            {
                anomaly.Draw(Position, Scale, SpriteEffect, anomaly.LayerDepth);
            }

            // Debug: Draw collision rectangle
            if (ShowCollisionRects)
            {
                DebugRenderer.DrawRectangle(CollisionRect, Color.Green, LayerDepth + 0.02f);
            }
        }

        public void OnCollision(ICollidable other)
        {
            // Handle collision with harpoon
            if (other is Harpoon && IsActive)
            {
                // Collision handling is primarily done in the Harpoon class
                // Fish could react here if needed
            }
        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            // Unregister from collision manager
            CollisionManager.Instance.Unregister(this);
        }
    }
}