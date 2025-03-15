using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dredge_lung_test
{

    //READ ONLY AND ENCAPSULATION
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameManager _gm; //Initialize the game manager

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            //Initialize the Globals attributes 
            Globals.GraphicsDevice = GraphicsDevice;
            Globals.ScreenWidth = 1920;
            Globals.ScreenHeight = 1080;
            Globals.Content = Content;

            PlayableArea.Initialize(1080, 1920); //Initialize the playable area the size of the background

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.SpriteBatch = _spriteBatch;

            // TODO: use this.Content to load your game content here
            _gm = new(); //Loading my game manager
            DebugRenderer.Initialize(GraphicsDevice); //Loading my debug class
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //Updating only Globals and the game manager. the rest is in the game manager
            Globals.Update(gameTime);
            _gm.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);
            _gm.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
