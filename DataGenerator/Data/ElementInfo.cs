using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;
using RogueEssence;
using RogueEssence.Data;
using PMDC;
using PMDC.Data;

namespace DataGenerator.Data
{
    /// <summary>
    /// Provides methods for generating Pokemon type (element) data.
    /// Defines all 18 types plus the None type used in the game.
    /// </summary>
    public static class ElementInfo
    {
        /// <summary>
        /// Defines all available Pokemon types/elements.
        /// Used for type matchups, STAB bonuses, and move categorization.
        /// </summary>
        public enum Element
        {
            /// <summary>
            /// Represents no type. Used for typeless moves or unassigned secondary types.
            /// </summary>
            None = 0,
            /// <summary>
            /// Bug type. Strong against Dark, Grass, Psychic. Weak to Fire, Flying, Rock.
            /// </summary>
            Bug = 1,
            /// <summary>
            /// Dark type. Strong against Ghost, Psychic. Weak to Bug, Fairy, Fighting.
            /// </summary>
            Dark = 2,
            /// <summary>
            /// Dragon type. Strong against Dragon. Weak to Dragon, Fairy, Ice.
            /// </summary>
            Dragon = 3,
            /// <summary>
            /// Electric type. Strong against Flying, Water. Weak to Ground.
            /// </summary>
            Electric = 4,
            /// <summary>
            /// Fairy type. Strong against Dark, Dragon, Fighting. Weak to Poison, Steel.
            /// </summary>
            Fairy = 5,
            /// <summary>
            /// Fighting type. Strong against Dark, Ice, Normal, Rock, Steel. Weak to Fairy, Flying, Psychic.
            /// </summary>
            Fighting = 6,
            /// <summary>
            /// Fire type. Strong against Bug, Grass, Ice, Steel. Weak to Ground, Rock, Water.
            /// </summary>
            Fire = 7,
            /// <summary>
            /// Flying type. Strong against Bug, Fighting, Grass. Weak to Electric, Ice, Rock.
            /// </summary>
            Flying = 8,
            /// <summary>
            /// Ghost type. Strong against Ghost, Psychic. Weak to Dark, Ghost.
            /// </summary>
            Ghost = 9,
            /// <summary>
            /// Grass type. Strong against Ground, Rock, Water. Weak to Bug, Fire, Flying, Ice, Poison.
            /// </summary>
            Grass = 10,
            /// <summary>
            /// Ground type. Strong against Electric, Fire, Poison, Rock, Steel. Weak to Grass, Ice, Water.
            /// </summary>
            Ground = 11,
            /// <summary>
            /// Ice type. Strong against Dragon, Flying, Grass, Ground. Weak to Fighting, Fire, Rock, Steel.
            /// </summary>
            Ice = 12,
            /// <summary>
            /// Normal type. No type advantages. Weak to Fighting.
            /// </summary>
            Normal = 13,
            /// <summary>
            /// Poison type. Strong against Fairy, Grass. Weak to Ground, Psychic.
            /// </summary>
            Poison = 14,
            /// <summary>
            /// Psychic type. Strong against Fighting, Poison. Weak to Bug, Dark, Ghost.
            /// </summary>
            Psychic = 15,
            /// <summary>
            /// Rock type. Strong against Bug, Fire, Flying, Ice. Weak to Fighting, Grass, Ground, Steel, Water.
            /// </summary>
            Rock = 16,
            /// <summary>
            /// Steel type. Strong against Fairy, Ice, Rock. Weak to Fighting, Fire, Ground.
            /// </summary>
            Steel = 17,
            /// <summary>
            /// Water type. Strong against Fire, Ground, Rock. Weak to Electric, Grass.
            /// </summary>
            Water = 18
        }

        /// <summary>
        /// Total number of elements including the None type.
        /// </summary>
        public const int MAX_ELEMENTS = 19;

        /// <summary>
        /// Generates and saves all element type data entries.
        /// </summary>
        public static void AddElementData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.Element.ToString());
            for (int ii = 0; ii < MAX_ELEMENTS; ii++)
            {
                ElementData element = new ElementData(new LocalText(((Element)ii).ToString()), (char)(ii + 0xE080));
                DataManager.SaveEntryData(Text.Sanitize(element.Name.DefaultText).ToLower(), DataManager.DataType.Element.ToString(), element);
            }
        }

        /// <summary>
        /// Generates minimal element data with only the None type for testing.
        /// </summary>
        public static void AddMinElementData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.Element.ToString());
            for (int ii = 0; ii < 1; ii++)
            {
                ElementData element = new ElementData(new LocalText(((Element)ii).ToString()), (char)(ii + 0xE080));
                DataManager.SaveEntryData(Text.Sanitize(element.Name.DefaultText).ToLower(), DataManager.DataType.Element.ToString(), element);
            }
        }
    }
}

