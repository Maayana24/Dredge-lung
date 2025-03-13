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

        public bool TexturesLoaded { get; private set; } = false;

        // Constructor is private for singleton pattern
        private AnomalyManager()
        {
            LoadTextures();
        }

        // Singleton instance access
        public static AnomalyManager Instance => _instance ??= new AnomalyManager();

        private void LoadTextures()
        {
            _extraLimbsTexture = Globals.Content.Load<Texture2D>("Fish/ExtraLimbs");
            _inflammationTexture = Globals.Content.Load<Texture2D>("Fish/InflammationD");
        }

        public List<Anomaly> GenerateAnomaliesForFish(Fish fish)
        {
            List<Anomaly> anomalies = new List<Anomaly>();

            Rectangle sourceRect = fish.SourceRect;

            if (sourceRect.Width <= 0 || sourceRect.Height <= 0)
            {
                sourceRect = new Rectangle(0, 0, 100, 100);
            }

            if (_random.NextDouble() < 0.4)
            {
                int anomalyCount = _random.Next(1, 3);

                for (int i = 0; i < anomalyCount; i++)
                {
                    AnomalyType selectedType = (AnomalyType)_random.Next(Enum.GetValues(typeof(AnomalyType)).Length);
                    Texture2D texture = GetTextureForAnomalyType(selectedType);
                    Color fallbackColor = selectedType == AnomalyType.ExtraLimbs ? Color.Purple : Color.Red;

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
                _ => _extraLimbsTexture,
            };
        }
    }
}
