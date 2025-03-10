using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace Dredge_lung_test
{
    public class AnomalyManager
    {
        private static AnomalyManager _instance;
        private readonly Random _random = new Random();

        // Textures for each anomaly type
        private Texture2D _extraLimbsTexture;
        private Texture2D _inflammationTexture;
        // Future textures will be added here

        // Source rectangles mapping for different fish types
        private Dictionary<Type, Rectangle> _fishTypeToRectangle;

        // Constructor is private for singleton pattern
        private AnomalyManager()
        {
            LoadTextures();
            InitializeFishMapping();
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

        private void LoadTextures()
        {
            // Load anomaly textures
            _extraLimbsTexture = Globals.Content.Load<Texture2D>("Fish/InflammationD");
            _inflammationTexture = Globals.Content.Load<Texture2D>("Fish/InflammationD");
            // Future texture loading will be added here
        }

        private void InitializeFishMapping()
        {
            // Map fish types to their respective source rectangles
            _fishTypeToRectangle = new Dictionary<Type, Rectangle>
            {
                { typeof(Grouper), new Rectangle(100, 0, 300, 200) },
                { typeof(Angler), new Rectangle(0, 600, 1000, 600) },
                { typeof(Eel), new Rectangle(0, 1200, 1000, 600) },
                { typeof(Shark), new Rectangle(0, 550, 1000, 800) },
                { typeof(Jelly), new Rectangle(0, 1800, 800, 800) }
                // Additional fish types can be added here
            };
        }

        // Generate random anomalies for a fish
        public List<Anomaly> GenerateAnomaliesForFish(Fish fish)
        {
            List<Anomaly> anomalies = new List<Anomaly>();
            List<AnomalyType> availableTypes = new List<AnomalyType>
            {
                AnomalyType.ExtraLimbs,
                AnomalyType.Inflammation
                // Add future anomaly types here
            };

            // Determine how many anomalies (1 or 2)
            int anomalyCount = _random.Next(1, 3); // 1 or 2

            // Track if we've already added a deadly anomaly
            bool hasDeadlyAnomaly = false;

            for (int i = 0; i < anomalyCount; i++)
            {
                // If we've exhausted all anomaly types, break
                if (availableTypes.Count == 0)
                    break;

                // Select a random type from available types
                int typeIndex = _random.Next(availableTypes.Count);
                AnomalyType selectedType = availableTypes[typeIndex];

                // Remove selected type to avoid duplicates
                availableTypes.RemoveAt(typeIndex);

                // Determine if this anomaly should be deadly
                bool isDeadly = _random.Next(2) == 0; // 50% chance

                // If we already have a deadly anomaly, make this one non-deadly
                if (hasDeadlyAnomaly && isDeadly)
                {
                    isDeadly = false;
                }

                // If this is deadly, mark that we have a deadly anomaly
                if (isDeadly)
                {
                    hasDeadlyAnomaly = true;
                }

                // Get the right texture and source rectangle based on type
                Texture2D texture = GetTextureForAnomalyType(selectedType);
                Rectangle sourceRect = GetSourceRectForFish(fish.GetType());

                // Create and add the anomaly
                anomalies.Add(new Anomaly(selectedType, isDeadly, texture, sourceRect));
            }

            return anomalies;
        }

        private Texture2D GetTextureForAnomalyType(AnomalyType type)
        {
            switch (type)
            {
                case AnomalyType.ExtraLimbs:
                    return _extraLimbsTexture;
                case AnomalyType.Inflammation:
                    return _inflammationTexture;
                // Add cases for future anomaly types
                default:
                    return _extraLimbsTexture; // Default fallback
            }
        }

        private Rectangle GetSourceRectForFish(Type fishType)
        {
            if (_fishTypeToRectangle.TryGetValue(fishType, out Rectangle rect))
            {
                return rect;
            }
            // Default rectangle if type not found
            return new Rectangle(0, 0, 1000, 600);
        }
    }

}
