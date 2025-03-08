using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace Dredge_lung_test
{
    public class GameManager
    {
        private readonly BGM _bgm = new();
        private readonly Player _player;
        private List<Fish> _fishes;
        private readonly UIManager _ui;
        private readonly FishSpawner _fishSpawner;

        // Anomaly decision tracking
        private int _correctDecisions = 0;
        private int _totalDecisions = 0;

        // Access to the anomaly inspection screen
        private AnomalyInspectionScreen _inspectionScreen;

        public GameManager()
        {
            // Initialize fish list
            _fishes = new List<Fish>();
            // Background layers
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("1"), 0.0f, 0.0f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("2"), 0.1f, 0.09f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("4"), 0.2f, 0.1f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("5"), 0.3f, 0.2f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("7"), 0.4f, 0.3f));
            _bgm.AddLayer(new Layer(LoadAndRotateTexture("9"), 0.5f, 0.4f));
            // Create player
            _player = new Player(Globals.Content.Load<Texture2D>("submarine"), new Vector2(300, 300));

            // Create inspection screen
            _inspectionScreen = new AnomalyInspectionScreen(this);

            // Set up UI with fish list
            _ui = new UIManager(_fishes, this);
            _ui.SetUpUI();
            _ui.AddElement(_inspectionScreen);

            // Create fish spawner with screen dimensions
            _fishSpawner = new FishSpawner(_fishes, 1080, 1920);
        }

        public void Update()
        {
            _ui.Update();
            IM.Update();
            _bgm.Update(-200);
            _player.Update();
            // Update fish spawner
            _fishSpawner.Update();
            // Update all active fish
            foreach (Fish fish in _fishes)
            {
                fish.Update();
            }
        }

        public void Draw()
        {
            _bgm.Draw();
            _player.Draw();
            // Draw all active fish
            foreach (Fish fish in _fishes)
            {
                fish.Draw();
            }
            _ui.Draw();
        }

        private Texture2D LoadAndRotateTexture(string assetName)
        {
            Texture2D original = Globals.Content.Load<Texture2D>("Background/" + assetName);
            RenderTarget2D rotated = new RenderTarget2D(Globals.SpriteBatch.GraphicsDevice, original.Height, original.Width);
            Globals.SpriteBatch.GraphicsDevice.SetRenderTarget(rotated);
            Globals.SpriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(original, new Vector2(original.Height, 0), null, Color.White, MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            Globals.SpriteBatch.End();
            Globals.SpriteBatch.GraphicsDevice.SetRenderTarget(null);
            return rotated;
        }

        // Method to open the inspection screen for a fish
        public void InspectFish(Fish fish)
        {
            _inspectionScreen.Open(fish);
        }

        // Method to record anomaly decision
        public void RecordAnomalyDecision(bool isCorrect)
        {
            _totalDecisions++;
            if (isCorrect)
            {
                _correctDecisions++;
            }

            // This can be expanded later to include scoring or other gameplay mechanics
            System.Diagnostics.Debug.WriteLine($"Anomaly decision: {(isCorrect ? "Correct" : "Incorrect")}");
            System.Diagnostics.Debug.WriteLine($"Score: {_correctDecisions}/{_totalDecisions}");
        }

        // Method to get current accuracy
        public float GetAccuracy()
        {
            if (_totalDecisions == 0)
                return 0;

            return (float)_correctDecisions / _totalDecisions;
        }
    }
}