using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dredge_lung_test
{
    public class AnomalyManager
    {
        private static AnomalyManager _instance;
        private readonly Random _random = new Random();

        // Textures for each anomaly type
        private Texture2D _extraLimbsTexture;
        private Texture2D _inflammationTexture;
        // Flag to track if textures loaded successfully
        public bool TexturesLoaded { get; private set; } = false;

        // Constructor is private for singleton pattern
        private AnomalyManager()
        {
            TexturesLoaded = LoadTextures();
            // Debug message to confirm initialization
            Console.WriteLine($"AnomalyManager initialized. Textures loaded: {TexturesLoaded}");
        }

        // Singleton instance access
        public static AnomalyManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AnomalyManager();
                }
                return _instance;
            }
        }

        private bool LoadTextures()
        {
            try
            {
                // Load ExtraLimbs texture
                _extraLimbsTexture = Globals.Content.Load<Texture2D>("ExtraLimbs");
                // Load Inflammation texture
                _inflammationTexture = Globals.Content.Load<Texture2D>("InflammationD");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR loading anomaly textures: {ex.Message}");
                return false;
            }
        }

        // Generate random anomalies for a fish
        public List<Anomaly> GenerateAnomaliesForFish(Fish fish)
        {
            List<Anomaly> anomalies = new List<Anomaly>();

            if (!TexturesLoaded)
            {
                // Debug warning when textures are not loaded
                Console.WriteLine("WARNING: Textures not loaded");
                return anomalies; // Return empty list if textures are not loaded
            }

            Rectangle sourceRect = fish.SourceRect;

            if (sourceRect.Width <= 0 || sourceRect.Height <= 0)
            {
                sourceRect = new Rectangle(0, 0, 100, 100);
            }

            if (_random.NextDouble() < 0.7)
            {
                int anomalyCount = _random.Next(1, 3);

                for (int i = 0; i < anomalyCount; i++)
                {
                    AnomalyType selectedType = (AnomalyType)_random.Next(Enum.GetValues(typeof(AnomalyType)).Length);
                    Texture2D texture = GetTextureForAnomalyType(selectedType);

                    anomalies.Add(new Anomaly(selectedType, texture, sourceRect));
                }
            }

            return anomalies;
        }

        private Texture2D GetTextureForAnomalyType(AnomalyType type)
        {
            return type switch
            {
                AnomalyType.ExtraLimbs => _extraLimbsTexture,
                AnomalyType.Inflammation => _inflammationTexture,
                _ => _extraLimbsTexture
            };
        }
    }
}
