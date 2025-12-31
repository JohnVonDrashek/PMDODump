using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;
using RogueEssence;
using RogueEssence.Data;
using PMDC;
using PMDC.Data;
using Microsoft.Xna.Framework;

namespace DataGenerator.Data
{
    /// <summary>
    /// Provides methods for generating Pokemon skin/coloration data.
    /// Handles normal and shiny variants with their visual effects.
    /// </summary>
    public static class SkinInfo
    {
        /// <summary>
        /// Defines the available Pokemon coloration variants.
        /// Determines the visual appearance and special effects of a Pokemon.
        /// </summary>
        public enum Coloration
        {
            /// <summary>
            /// Special value indicating an unknown or invalid coloration.
            /// </summary>
            Unknown = -1,
            /// <summary>
            /// Standard Pokemon coloration with no special effects. Displays red on minimap.
            /// </summary>
            Normal = 0,
            /// <summary>
            /// Rare alternate coloration with sparkle visual effects. Displays magenta on minimap.
            /// </summary>
            Shiny = 1,
            /// <summary>
            /// Ultra-rare square sparkle variant. Displays magenta on minimap and is marked as a challenge encounter.
            /// </summary>
            SquareShiny = 2,
        }

        /// <summary>
        /// Maximum number of skin variants available.
        /// </summary>
        public const int MAX_GROUPS = 3;

        /// <summary>
        /// Generates and saves all skin data entries including visual effects for shiny Pokemon.
        /// </summary>
        public static void AddSkinData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.Skin.ToString());

            for (int ii = 0; ii < MAX_GROUPS; ii++)
            {
                SkinData data = new SkinData(new LocalText(ii > 0 ? "Shiny" : "Normal"), ii > 0 ? '\uE10C' : '\0');
                data.IndexNum = ii;
                string fileName = "";
                switch (ii)
                {
                    case 0:
                        {
                            fileName = "normal";
                            data.MinimapColor = Color.Red;
                        }
                        break;
                    case 1:
                        {
                            fileName = "shiny";
                            FiniteAreaEmitter emitter = new FiniteAreaEmitter(new AnimData("Screen_Sparkle_RSE", 5));
                            emitter.Range = GraphicsManager.TileSize;
                            emitter.Speed = GraphicsManager.TileSize * 2;
                            emitter.TotalParticles = 12;
                            emitter.Layer = DrawLayer.Front;
                            data.LeaderFX.Emitter = emitter;
                            data.LeaderFX.Sound = "EVT_CH14_Eye_Glint";
                            data.LeaderFX.Delay = 20;
                            data.Display = true;
                            data.MinimapColor = new Color(255, 0, 255);
                        }
                        break;
                    case 2:
                        {
                            fileName = "shiny_square";
                            FiniteAreaEmitter emitter = new FiniteAreaEmitter(new AnimData("Captivate_Sparkle", 2));
                            emitter.Range = GraphicsManager.TileSize;
                            emitter.Speed = GraphicsManager.TileSize * 2;
                            emitter.TotalParticles = 10;
                            emitter.Layer = DrawLayer.Front;
                            data.Comment = "Square";
                            data.LeaderFX.Emitter = emitter;
                            data.LeaderFX.Sound = "EVT_CH14_Eye_Glint";
                            data.LeaderFX.Delay = 20;
                            data.Challenge = true;
                            data.Display = true;
                            data.MinimapColor = new Color(255, 0, 255);
                        }
                        break;
                }
                DataManager.SaveEntryData(fileName, DataManager.DataType.Skin.ToString(), data);
            }
        }

        /// <summary>
        /// Generates minimal skin data for testing with only basic variants.
        /// </summary>
        public static void AddMinSkinData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.Skin.ToString());

            for (int ii = 0; ii < MAX_GROUPS; ii++)
            {
                SkinData data = new SkinData(new LocalText("Normal"), '\0');
                data.IndexNum = ii;
                string fileName = "normal";
                data.MinimapColor = Color.Red;
                DataManager.SaveEntryData(fileName, DataManager.DataType.Skin.ToString(), data);
            }
        }
    }
}

