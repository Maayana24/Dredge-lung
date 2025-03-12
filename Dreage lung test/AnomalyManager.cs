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
                // Debug the content paths
                Debug.WriteLine("Attempting to load anomaly textures...");

                // Load ExtraLimbs texture
                try
                {
                    _extraLimbsTexture = Globals.Content.Load<Texture2D>("ExtraLimbs");
                    Debug.WriteLine("Loaded ExtraLimbs texture from root directory");
                }
                catch
                {
                    try
                    {
                        // Try with Fish subdirectory
                        _extraLimbsTexture = Globals.Content.Load<Texture2D>("Fish/ExtraLimbs");
                        Debug.WriteLine("Loaded ExtraLimbs texture from Fish subdirectory");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to load ExtraLimbs texture: {ex.Message}");
                        // Create fallback texture
                        _extraLimbsTexture = CreateFallbackTexture(Color.Purple);
                    }
                }

                // Load Inflammation texture
                try
                {
                    _inflammationTexture = Globals.Content.Load<Texture2D>("InflammationD");
                    Debug.WriteLine("Loaded Inflammation texture from root directory");
                }
                catch
                {
                    try
                    {
                        // Try with Fish subdirectory
                        _inflammationTexture = Globals.Content.Load<Texture2D>("Fish/InflammationD");
                        Debug.WriteLine("Loaded Inflammation texture from Fish subdirectory");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to load Inflammation texture: {ex.Message}");
                        // Create fallback texture
                        _inflammationTexture = CreateFallbackTexture(Color.Red);
                    }
                }

                // Verify textures were loaded
                Debug.WriteLine($"ExtraLimbs texture dimensions: {_extraLimbsTexture.Width}x{_extraLimbsTexture.Height}");
                Debug.WriteLine($"Inflammation texture dimensions: {_inflammationTexture.Width}x{_inflammationTexture.Height}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR loading anomaly textures: {ex.Message}");

                // Create fallback textures
                _extraLimbsTexture = CreateFallbackTexture(Color.Purple);
                _inflammationTexture = CreateFallbackTexture(Color.Red);

                Debug.WriteLine("Created fallback textures");
                return false;
            }
        }

        private Texture2D CreateFallbackTexture(Color color)
        {
            Texture2D fallbackTexture = new Texture2D(Globals.GraphicsDevice, 50, 50);
            Color[] colors = new Color[50 * 50];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;
            fallbackTexture.SetData(colors);
            return fallbackTexture;
        }

        // Generate random anomalies for a fish
        public List<Anomaly> GenerateAnomaliesForFish(Fish fish)
        {
            List<Anomaly> anomalies = new List<Anomaly>();

            // Check if textures were loaded properly
            if (!TexturesLoaded)
            {
                Debug.WriteLine("WARNING: Textures not loaded, using fallback textures");
            }

            // Access the fish's source rectangle directly from the property
            Rectangle sourceRect = fish.SourceRect;

            // Debug output - make sure we're getting proper source rectangle
            Debug.WriteLine($"Creating anomaly for {fish.Name} with source rect: {sourceRect}");

            // Make sure the source rectangle is valid
            if (sourceRect.Width <= 0 || sourceRect.Height <= 0)
            {
                Debug.WriteLine($"WARNING: Invalid source rectangle for fish {fish.Name}. Using default rectangle.");
                sourceRect = new Rectangle(0, 0, 100, 100); // Fallback rectangle
            }

            // Determine if this fish has anomalies (70% chance)
            if (_random.NextDouble() < 0.7)
            {
                // Determine how many anomalies (1-2)
                int anomalyCount = _random.Next(1, 3);

                for (int i = 0; i < anomalyCount; i++)
                {
                    // Randomly select anomaly type
                    AnomalyType selectedType = (AnomalyType)_random.Next(Enum.GetValues(typeof(AnomalyType)).Length);

                    // Randomly determine if the anomaly is deadly (30% chance)
                    bool isDeadly = _random.NextDouble() < 0.3;

                    // Get the appropriate texture
                    Texture2D texture = GetTextureForAnomalyType(selectedType);

                    // Select a random fallback color based on anomaly type
                    Color fallbackColor = selectedType == AnomalyType.ExtraLimbs ?
                        Color.Purple : Color.Red;

                    // Create the anomaly
                    anomalies.Add(new Anomaly(selectedType, isDeadly, texture, sourceRect, fallbackColor));

                    Debug.WriteLine($"Added {selectedType} anomaly to {fish.Name}, deadly: {isDeadly}");
                }
            }
            else
            {
                Debug.WriteLine($"No anomalies generated for {fish.Name}");
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
                default:
                    return _extraLimbsTexture;
            }
        }
    }
}