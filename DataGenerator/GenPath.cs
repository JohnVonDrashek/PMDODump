using System;
using System.Collections.Generic;

namespace DataGenerator
{
    /// <summary>
    /// Provides static path constants and properties for data generation asset directories.
    /// Centralizes file path configuration for the DataGenerator tool.
    /// </summary>
    public static class GenPath
    {
        /// <summary>
        /// The root path for generated data assets. Can be customized via the -gen command-line argument.
        /// </summary>
        public static string DATA_GEN_PATH = "DataAsset/";

        /// <summary>
        /// Gets the path for localization/translation string files.
        /// </summary>
        public static string TL_PATH { get => DATA_GEN_PATH + "String/"; }

        /// <summary>
        /// Gets the path for item data generation files.
        /// </summary>
        public static string ITEM_PATH { get => DATA_GEN_PATH + "Item/"; }

        /// <summary>
        /// Gets the path for monster data generation files.
        /// </summary>
        public static string MONSTER_PATH { get => DATA_GEN_PATH + "Monster/"; }

        /// <summary>
        /// Gets the path for zone/dungeon data generation files.
        /// </summary>
        public static string ZONE_PATH { get => DATA_GEN_PATH + "Zone/"; }
    }
}
