using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dredge_lung_test
{
    //Manager class to manage anomaly logic
    public class AnomalyManager
    {
        private static AnomalyManager _instance;
        public static AnomalyManager Instance => _instance ??= new AnomalyManager();

        private readonly Random _random = new Random();

        //Textures for anomalies type
        private Texture2D _extraLimbsTexture;
        private Texture2D _inflammationTexture;

        private AnomalyManager()
        {
            LoadTextures();
        }
        private void LoadTextures()
        {
            _extraLimbsTexture = Globals.Content.Load<Texture2D>("Fish/ExtraLimbs");
            _inflammationTexture = Globals.Content.Load<Texture2D>("Fish/InflammationD");
        }

        public List<Anomaly> GenerateAnomaliesForFish(Fish fish)
        {
            List<Anomaly> anomalies = new List<Anomaly>();

            Rectangle sourceRect = fish.SourceRect; //Using the fish source Rectangle

            if (sourceRect.Width <= 0 || sourceRect.Height <= 0) //If the sourceRect in not valid create new one
            {
                sourceRect = new Rectangle(0, 0, 100, 100);
            }

            if (_random.NextDouble() < 0.6) //Chances the fish will get anomalies
            {
                int anomalyCount = _random.Next(1, 2);

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
                _ => _extraLimbsTexture, //Default is extra limbs type
            };
        }
    }
}
