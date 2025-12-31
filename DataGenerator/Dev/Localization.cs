using System;
using System.Collections.Generic;
using System.IO;
using RogueElements;
using RogueEssence.Data;
using RogueEssence.Content;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using RogueEssence.LevelGen;
using System.Text.RegularExpressions;
using System.Linq;
using RogueEssence;
using RogueEssence.Dev;
using PMDC.Data;
using PMDC.Dungeon;
using DataGenerator.Data;


namespace DataGenerator.Dev
{
    /// <summary>
    /// Provides methods for exporting and importing localization string tables.
    /// Handles translation workflows for game data including items, skills, zones, and other content.
    /// </summary>
    /// <remarks>
    /// The localization workflow involves:
    /// <list type="number">
    /// <item><description>Export: Print* methods generate tab-separated files with English text and existing translations</description></item>
    /// <item><description>Translate: External tools process the exported files and produce *.out.txt files with translations</description></item>
    /// <item><description>Import: Write* methods read the *.out.txt files and update the game data with new translations</description></item>
    /// </list>
    /// </remarks>
    public static class Localization
    {
        /// <summary>
        /// Exports the exclusive item type names to a localization file.
        /// </summary>
        /// <remarks>
        /// Iterates through all <see cref="ExclusiveItemType"/> enum values and outputs their
        /// localized name expressions for translation.
        /// </remarks>
        public static void PrintExclusiveNameStringTable()
        {
            string path = GenPath.TL_PATH + "ExclusiveItemType.txt";


            Dictionary<string, (string, LocalText)> rows = new Dictionary<string, (string, LocalText)>();
            List<string> orderedKeys = new List<string>();
            HashSet<string> languages = new HashSet<string>();

            //names
            foreach (ExclusiveItemType nameType in Enum.GetValues(typeof(ExclusiveItemType)).Cast<ExclusiveItemType>())
            {
                LocalText nameText = AutoItemInfo.GetLocalExpression(nameType, false);
                updateWorkingLists(rows, orderedKeys, languages, typeof(ExclusiveItemType).Name+ "."+nameType, "", nameText);
            }

            printLocalizationRows(path, languages, orderedKeys, rows);
        }

        /// <summary>
        /// Exports the exclusive item effect descriptions to a localization file.
        /// </summary>
        /// <remarks>
        /// Iterates through all <see cref="ExclusiveItemEffect"/> enum values and outputs their
        /// generated descriptions for translation.
        /// </remarks>
        public static void PrintExclusiveDescStringTable()
        {
            string path = GenPath.TL_PATH + "ExclusiveItemEffect.txt";


            Dictionary<string, (string, LocalText)> rows = new Dictionary<string, (string, LocalText)>();
            List<string> orderedKeys = new List<string>();
            HashSet<string> languages = new HashSet<string>();

            foreach (ExclusiveItemEffect descType in Enum.GetValues(typeof(ExclusiveItemEffect)).Cast<ExclusiveItemEffect>())
            {
                ItemData item = new ItemData();
                item.UseEvent.Element = "none";
                AutoItemInfo.FillExclusiveEffects("", item, new List<LocalText>(), false, descType, new object[0], false);
                updateWorkingLists(rows, orderedKeys, languages, typeof(ExclusiveItemEffect).Name + "." + descType, item.Comment, item.Desc);
            }

            printLocalizationRows(path, languages, orderedKeys, rows);
        }

        //these methods are meant to add to existing tables, which have been preconverted.
        //the asterisk safeguards should be removed when processing these entries
        //initially, there will be an official-word english column.
        //start with highlighting all cells in red
        //you must go through the full spreadsheet and find all cases where the english column does match with the actual description, or is better than what you have.
        //in those cases, un-highlight those rows to denote that they have been approved.
        //for entries with base-blank descriptions, if nothing can be thought up, delete the row
        //*for items, existing names and descriptions must be entered manually*
        //finally, delete the english descriptions
        //when importing, must first run a macro on the spreadsheet that erases all highlighted translations.

        /// <summary>
        /// Exports a string table for data types that have only a name (no description).
        /// </summary>
        /// <param name="dataType">The type of data to export (e.g., Element, Rank, AI).</param>
        /// <param name="getData">A delegate function to retrieve data entries by key.</param>
        public static void PrintNamedStringTable(DataManager.DataType dataType, GetNamedData getData)
        {
            printNamedDataTable(GenPath.TL_PATH + dataType.ToString() + ".txt", DataManager.Instance.DataIndices[dataType], getData);
        }

        /// <summary>
        /// Exports a string table for data types that have both a name and description.
        /// </summary>
        /// <param name="dataType">The type of data to export (e.g., Skill, Intrinsic, Status).</param>
        /// <param name="getData">A delegate function to retrieve described data entries by key.</param>
        public static void PrintDescribedStringTable(DataManager.DataType dataType, GetDescribedData getData)
        {
            printDescribedDataTable(GenPath.TL_PATH + dataType.ToString() + ".txt", DataManager.Instance.DataIndices[dataType], getData);
        }

        /// <summary>
        /// Exports the item string table with special handling for various item types.
        /// </summary>
        /// <remarks>
        /// Handles special cases:
        /// <list type="bullet">
        /// <item><description>Skips blank entries and TM items (auto-generated names)</description></item>
        /// <item><description>Skips auto-calculated exclusive item names</description></item>
        /// <item><description>Skips all exclusive item descriptions (always auto-calculated)</description></item>
        /// <item><description>Detects and warns about duplicate item names</description></item>
        /// </list>
        /// </remarks>
        public static void PrintItemStringTable()
        {
            DataManager.DataType dataType = DataManager.DataType.Item;
            string path = GenPath.TL_PATH + dataType.ToString() + ".txt";

            Dictionary<string, string> repeatCheck = new Dictionary<string, string>();

            Dictionary<string, (string, LocalText)> rows = new Dictionary<string, (string, LocalText)>();
            List<string> orderedKeys = new List<string>();
            HashSet<string> languages = new HashSet<string>();

            EntryDataIndex index = DataManager.Instance.DataIndices[dataType];
            List<string> dataKeys = index.GetOrderedKeys(true);
            foreach (string key in dataKeys)
            {
                ItemData data = DataManager.Instance.GetItem(key);

                //skip blank entries
                if (data.Name.DefaultText == "")
                    continue;
                if (repeatCheck.ContainsKey(data.Name.DefaultText))
                    Console.WriteLine("Item name \"{0}\" repeated between {1} and {2}", data.Name.DefaultText, repeatCheck[data.Name.DefaultText], key);
                repeatCheck[data.Name.DefaultText] = key;

                //skip TMs
                if (data.UsageType == ItemData.UseType.Learn)
                    continue;
                //skip autocalculated exclusive item NAMES
                if (data.ItemStates.Contains<MaterialState>() && data.ItemStates.GetWithDefault<ExclusiveState>().ItemType != ExclusiveItemType.None)
                    continue;
                //TODO: get these type names via reflection
                updateWorkingLists(rows, orderedKeys, languages, index.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + 0.ToString("D4") + "|data.Name", data.Comment, data.Name);

                //skip ALL exclusive item DESCRIPTIONS because they are guaranteed autocalculated
                if (data.ItemStates.Contains<MaterialState>())
                    continue;
                updateWorkingLists(rows, orderedKeys, languages, index.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + 1.ToString("D4") + "|data.Desc", "", data.Desc);
            }

            printLocalizationRows(path, languages, orderedKeys, rows);
        }

        /// <summary>
        /// Delegate for retrieving entry data by string key.
        /// </summary>
        /// <param name="index">The string key identifying the data entry.</param>
        /// <returns>The entry data for the specified key.</returns>
        public delegate IEntryData GetNamedData(string index);

        /// <summary>
        /// Delegate for retrieving described data (with name and description) by string key.
        /// </summary>
        /// <param name="index">The string key identifying the data entry.</param>
        /// <returns>The described data for the specified key.</returns>
        public delegate IDescribedData GetDescribedData(string index);

        /// <summary>
        /// Writes a localization table for named data entries to the specified path.
        /// </summary>
        /// <param name="path">The output file path.</param>
        /// <param name="index">The data index containing entry metadata.</param>
        /// <param name="method">The delegate to retrieve individual data entries.</param>
        private static void printNamedDataTable(string path, EntryDataIndex index, GetNamedData method)
        {
            Dictionary<string, (string, LocalText)> rows = new Dictionary<string, (string, LocalText)>();
            List<string> orderedKeys = new List<string>();
            HashSet<string> languages = new HashSet<string>();

            List<string> dataKeys = index.GetOrderedKeys(true);
            foreach (string key in dataKeys)
            {
                IEntryData data = method(key);

                //skip blank entries
                if (data.Name.DefaultText == "")
                    continue;

                //TODO: get these type names via reflection
                updateWorkingLists(rows, orderedKeys, languages, index.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + 0.ToString("D4") + "|data.Name", data.Comment, data.Name);
            }

            printLocalizationRows(path, languages, orderedKeys, rows);
        }

        /// <summary>
        /// Most translatable content here are just types with name and desc, so a common function is used to handle them.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="total"></param>
        /// <param name="method"></param>
        private static void printDescribedDataTable(string path, EntryDataIndex index, GetDescribedData method)
        {
            Dictionary<string, (string, LocalText)> rows = new Dictionary<string, (string, LocalText)>();
            List<string> orderedKeys = new List<string>();
            HashSet<string> languages = new HashSet<string>();

            List<string> dataKeys = index.GetOrderedKeys(true);
            foreach(string key in dataKeys)
            {
                IDescribedData data = method(key);

                //skip blank entries
                if (data.Name.DefaultText == "")
                    continue;

                //TODO: get these type names via reflection
                updateWorkingLists(rows, orderedKeys, languages, index.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + 0.ToString("D4") + "|data.Name", data.Comment, data.Name);
                updateWorkingLists(rows, orderedKeys, languages, index.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + 1.ToString("D4") + "|data.Desc", "", data.Desc);
            }

            printLocalizationRows(path, languages, orderedKeys, rows);
        }


        /// <summary>
        /// Delegate for retrieving titled data (maps, ground maps) by filename.
        /// </summary>
        /// <param name="filename">The filename (without extension) identifying the data entry.</param>
        /// <returns>The entry data for the specified filename.</returns>
        public delegate IEntryData GetTitledData(string filename);

        /// <summary>
        /// Exports a string table for titled data types like maps and ground maps.
        /// </summary>
        /// <param name="dir">The directory containing the data files.</param>
        /// <param name="ext">The file extension to filter by.</param>
        /// <param name="method">The delegate to retrieve individual data entries.</param>
        /// <remarks>
        /// Titled data types have a name and title card. The output is sorted alphabetically by key.
        /// </remarks>
        public static void PrintTitledDataTable(string dir, string ext, GetTitledData method)
        {
            string path = GenPath.TL_PATH + (new DirectoryInfo(dir)).Name + ".txt";
            List<string> entryNames = new List<string>();
            foreach (string name in PathMod.GetModFiles(dir, "*"+ ext))
                entryNames.Add(Path.GetFileNameWithoutExtension(name));

            Dictionary<string, (string, LocalText)> rows = new Dictionary<string, (string, LocalText)>();
            List<string> orderedKeys = new List<string>();
            HashSet<string> languages = new HashSet<string>();
            for (int ii = 0; ii < entryNames.Count; ii++)
            {
                IEntryData data = method(entryNames[ii]);

                //TODO: get these type names via reflection
                updateWorkingLists(rows, orderedKeys, languages, entryNames[ii] + "-" + 0.ToString("D4") + "|data.Name", data.Comment, data.Name);
            }

            orderedKeys.Sort(StringComparer.Ordinal);
            printLocalizationRows(path, languages, orderedKeys, rows);
        }

        /// <summary>
        /// Exports the zone string table including zone names and floor naming patterns.
        /// </summary>
        /// <remarks>
        /// Zone data is an exceptional case that includes both zone names and floor naming patterns
        /// from <see cref="FloorNameIDZoneStep"/> steps within each zone segment.
        /// </remarks>
        public static void PrintZoneStringTable()
        {
            string path = GenPath.TL_PATH + "Zone.txt";
            Dictionary<string, (string, LocalText)> rows = new Dictionary<string, (string, LocalText)>();
            List<string> orderedKeys = new List<string>();
            HashSet<string> languages = new HashSet<string>();

            EntryDataIndex index = DataManager.Instance.DataIndices[DataManager.DataType.Zone];
            List<string> dataKeys = index.GetOrderedKeys(true);
            foreach (string key in dataKeys)
            {
                ZoneData data = DataManager.Instance.GetZone(key);

                int nn = 0;
                updateWorkingLists(rows, orderedKeys, languages, index.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + nn.ToString("D4") + "|data.Name", data.Comment, data.Name);
                for (int jj = 0; jj < data.Segments.Count; jj++)
                {
                    ZoneSegmentBase structure = data.Segments[jj];

                    for (int kk = 0; kk < structure.ZoneSteps.Count; kk++)
                    {
                        FloorNameIDZoneStep postProc = structure.ZoneSteps[kk] as FloorNameIDZoneStep;
                        if (postProc != null)
                        {
                            //TODO: get these type names via reflection
                            nn++;
                            updateWorkingLists(rows, orderedKeys, languages,
                                index.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + nn.ToString("D4") + "|((FloorNameIDZoneStep)data.Segments[" + jj.ToString("D4") + "].ZoneSteps[" + kk.ToString("D4") + "]).Name", "", postProc.Name);
                        }
                    }
                }
            }

            printLocalizationRows(path, languages, orderedKeys, rows);
        }

        /// <summary>
        /// Updates the working lists used during localization export.
        /// </summary>
        /// <param name="rows">Dictionary mapping keys to comment and LocalText pairs.</param>
        /// <param name="orderedKeys">List maintaining insertion order of keys.</param>
        /// <param name="languages">Set of all encountered language codes.</param>
        /// <param name="key">The unique key for this localization entry.</param>
        /// <param name="comment">An optional comment for translators.</param>
        /// <param name="val">The LocalText containing the default text and existing translations.</param>
        private static void updateWorkingLists(Dictionary<string, (string, LocalText)> rows, List<string> orderedKeys, HashSet<string> languages, string key, string comment, LocalText val)
        {
            foreach (string language in val.LocalTexts.Keys)
                languages.Add(language);
            rows.Add(key, (/*comment*/"", val));
            orderedKeys.Add(key);
        }

        /// <summary>
        /// Writes the collected localization rows to a tab-separated file.
        /// </summary>
        /// <param name="path">The output file path.</param>
        /// <param name="languages">Set of language codes to include as columns.</param>
        /// <param name="orderedKeys">Keys in the order they should appear in the file.</param>
        /// <param name="rows">Dictionary mapping keys to comment and LocalText pairs.</param>
        /// <remarks>
        /// Output format: Key, Comment, EN (default text), then one column per additional language.
        /// Newlines in text are escaped as \n.
        /// </remarks>
        private static void printLocalizationRows(string path, HashSet<string> languages, List<string> orderedKeys, Dictionary<string, (string, LocalText)> rows)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write("Key\t\tEN");
                foreach (string language in languages)
                    file.Write("\t"+language.ToUpper());
                file.WriteLine();
                foreach(string key in orderedKeys)
                {
                    LocalText text = rows[key].Item2;
                    file.Write(key+"\t"+ rows[key] .Item1+ "\t"+text.DefaultText.Replace("\n", "\\n"));
                    foreach (string language in languages)
                    {
                        file.Write("\t");
                        if (text.LocalTexts.ContainsKey(language))
                            file.Write(text.LocalTexts[language].Replace("\n", "\\n"));
                    }
                    file.WriteLine();
                }
            }
        }
















        //write backs

        /// <summary>
        /// Imports translations for titled data (maps, ground maps) from a localization file.
        /// </summary>
        /// <param name="path">The directory containing the data files.</param>
        /// <param name="ext">The file extension to filter by.</param>
        /// <param name="method">The delegate to retrieve individual data entries.</param>
        /// <remarks>
        /// Reads from [DirectoryName].out.txt and updates the corresponding data files.
        /// </remarks>
        public static void WriteTitledDataTable(string path, string ext, GetTitledData method)
        {
            string filename = (new DirectoryInfo(path)).Name;
            List<string> entryNames = new List<string>();
            foreach (string name in PathMod.GetModFiles(path, "*" + ext))
                entryNames.Add(Path.GetFileNameWithoutExtension(name));

            Dictionary<string, LocalText> rows = readLocalizationRows(GenPath.TL_PATH + filename + ".out.txt");
            for (int ii = 0; ii < entryNames.Count; ii++)
            {
                IEntryData data = method(entryNames[ii]);

                data.Name = rows[entryNames[ii] + "-" + 0.ToString("D4") + "|data.Name"];

                DataManager.SaveData(data, path, entryNames[ii], ext);
            }
        }

        /// <summary>
        /// Imports translations for named data types from a localization file.
        /// </summary>
        /// <param name="dataType">The type of data to import translations for.</param>
        /// <remarks>
        /// Reads from [DataType].out.txt and updates the corresponding data files.
        /// Only updates entries that have non-empty default names.
        /// </remarks>
        public static void WriteNamedDataTable(DataManager.DataType dataType)
        {

            Dictionary<string, LocalText> rows = readLocalizationRows(GenPath.TL_PATH + dataType.ToString() + ".out.txt");
            foreach (string key in DataManager.Instance.DataIndices[dataType].GetOrderedKeys(true))
            {
                string tlKey = dataType.ToString() + "-" + key + "|data.Name";
                if (rows.ContainsKey(tlKey))
                {
                    string dir = PathMod.NoMod(DataManager.DATA_PATH + dataType.ToString() + "/" + key + DataManager.DATA_EXT);

                    IEntryData describedData = DataManager.LoadObject<IEntryData>(dir);

                    if (describedData.Name.DefaultText != "")
                    {
                        describedData.Name = rows[tlKey];
                        DataManager.SaveObject(describedData, dir);
                    }
                }
            }
        }

        /// <summary>
        /// Copies the name from one data entry to another within the same data type.
        /// </summary>
        /// <param name="dataType">The type of data being copied.</param>
        /// <param name="from">The source entry key to copy from.</param>
        /// <param name="to">The destination entry key to copy to.</param>
        /// <remarks>
        /// Used for cases like copying "shiny" skin name to "shiny_square".
        /// </remarks>
        public static void CopyNamedData(DataManager.DataType dataType, string from, string to)
        {
            string fromDir = PathMod.NoMod(DataManager.DATA_PATH + dataType.ToString() + "/" + from + DataManager.DATA_EXT);
            string toDir = PathMod.NoMod(DataManager.DATA_PATH + dataType.ToString() + "/" + to + DataManager.DATA_EXT);

            IEntryData fromData = DataManager.LoadObject<IEntryData>(fromDir);
            IEntryData toData = DataManager.LoadObject<IEntryData>(toDir);

            toData.Name = fromData.Name;
            DataManager.SaveObject(toData, toDir);
        }

        /// <summary>
        /// Imports translations for described data types (with name and description) from a localization file.
        /// </summary>
        /// <param name="dataType">The type of data to import translations for.</param>
        /// <remarks>
        /// Reads from [DataType].out.txt and updates the corresponding data files.
        /// Only updates entries that have non-empty default names.
        /// </remarks>
        public static void WriteDescribedDataTable(DataManager.DataType dataType)
        {

            Dictionary<string, LocalText> rows = readLocalizationRows(GenPath.TL_PATH + dataType.ToString() + ".out.txt");
            foreach(string key in DataManager.Instance.DataIndices[dataType].GetOrderedKeys(true))
            {
                string dir = PathMod.NoMod(DataManager.DATA_PATH + dataType.ToString() + "/" + key + DataManager.DATA_EXT);

                IDescribedData describedData = DataManager.LoadObject<IDescribedData>(dir);

                if (describedData.Name.DefaultText != "")
                {
                    int sort = DataManager.Instance.DataIndices[dataType].Get(key).SortOrder;
                    describedData.Name = rows[sort.ToString("D4") + "-" + key + "-" + 0.ToString("D4") + "|data.Name"];
                    describedData.Desc = rows[sort.ToString("D4") + "-" + key + "-" + 1.ToString("D4") + "|data.Desc"];

                    DataManager.SaveObject(describedData, dir);
                }
            }
        }

        /// <summary>
        /// Imports translations for items with special handling for auto-generated content.
        /// </summary>
        /// <remarks>
        /// Handles special cases:
        /// <list type="bullet">
        /// <item><description>TM items: Auto-generates name and description from skill translations</description></item>
        /// <item><description>Exclusive items: Skipped (handled by AddCalculatedItemData)</description></item>
        /// <item><description>Regular items: Imports name and description directly</description></item>
        /// </list>
        /// </remarks>
        public static void WriteItemStringTable()
        {
            DataManager.DataType dataType = DataManager.DataType.Item;
            Dictionary<string, LocalText> rows = readLocalizationRows(GenPath.TL_PATH + dataType.ToString() + ".out.txt");
            Dictionary<string, LocalText> skillRows = readLocalizationRows(GenPath.TL_PATH + DataManager.DataType.Skill.ToString() + ".out.txt");
            Dictionary<string, LocalText> specialRows = readLocalizationRows(GenPath.TL_PATH + "Special.out.txt");

            EntryDataIndex itemIndex = DataManager.Instance.DataIndices[dataType];
            EntryDataIndex skillIndex = DataManager.Instance.DataIndices[DataManager.DataType.Skill];
            foreach (string key in itemIndex.GetOrderedKeys(true))
            {
                string dir = PathMod.NoMod(DataManager.DATA_PATH + dataType.ToString() + "/" + key + DataManager.DATA_EXT);

                ItemData data = DataManager.LoadObject<ItemData>(dir);

                //skip blank entries
                if (data.Name.DefaultText == "")
                    continue;

                if (data.UsageType == ItemData.UseType.Learn)
                {
                    //autocalculate TM name and descriptions
                    LocalText tmFormatName = specialRows["tmFormatName"];
                    LocalText tmFormatDesc = specialRows["tmFormatDesc"];
                    string moveIndex = data.ItemStates.GetWithDefault<ItemIDState>().ID;
                    LocalText moveName = skillRows[skillIndex.Get(moveIndex).SortOrder.ToString("D4") + "-" + moveIndex + "-" + 0.ToString("D4") + "|data.Name"];
                    data.Name = LocalText.FormatLocalText(tmFormatName, moveName);
                    data.Desc = LocalText.FormatLocalText(tmFormatDesc, moveName);
                }
                else if (data.ItemStates.Contains<MaterialState>())
                {
                    //skip this; exclusive items will be added in AddCalculatedItemData
                    //if (data.ItemStates.Get<ExclusiveState>().ItemType != ExclusiveItemType.None)
                    //{
                    //    //autocalculate exclusive item NAMES
                    //    LocalText exclFormatName = itemTypeRows[typeof(ExclusiveItemType).Name + "." + data.ItemStates.Get<ExclusiveState>().ItemType];
                    //    MonsterData monsterData = DataManager.Instance.GetMonster();
                    //    data.Name = LocalText.FormatLocalText(exclFormatName, monsterData.Name);
                    //}
                    //else
                    //{
                    //    data.Name = rows[ii.ToString("D4") + "-" + 0.ToString("D4") + "|data.Name"];
                    //}
                    ////generate autocalculated item descriptions
                    //LocalText qualityText;
                    //if (/*Species-based*/)
                    //{
                    //    if (/*Allow And-Family*/)
                    //    {

                    //    }
                    //    else//Just list out the names
                    //    {

                    //    }
                    //}
                    //else if (/*Type-based*/)
                    //{

                    //}
                    //data.Desc = rows[ii.ToString("D4") + "-" + 1.ToString("D4") + "|data.Desc"];
                }
                else
                {
                    //TODO: get these type names via reflection
                    data.Name = rows[itemIndex.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + 0.ToString("D4") + "|data.Name"];
                    data.Desc = rows[itemIndex.Get(key).SortOrder.ToString("D4") + "-" + key + "-" + 1.ToString("D4") + "|data.Desc"];
                }

                DataManager.SaveObject(data, dir);
            }
        }

        /// <summary>
        /// Imports translations for zones including zone names and floor naming patterns.
        /// </summary>
        /// <remarks>
        /// Updates both the zone name and any FloorNameIDZoneStep names found in zone segments.
        /// </remarks>
        public static void WriteZoneStringTable()
        {
            DataManager.DataType dataType = DataManager.DataType.Zone;
            Dictionary<string, LocalText> rows = readLocalizationRows(GenPath.TL_PATH + dataType.ToString() + ".out.txt");
            foreach (string key in DataManager.Instance.DataIndices[dataType].GetOrderedKeys(false))
            {
                string dir = PathMod.NoMod(DataManager.DATA_PATH + dataType.ToString() + "/" + key + DataManager.DATA_EXT);

                ZoneData data = DataManager.LoadObject<ZoneData>(dir);

                int nn = 0;
                int sort = DataManager.Instance.DataIndices[dataType].Get(key).SortOrder;
                data.Name = rows[sort.ToString("D4") + "-" + key + "-" + 0.ToString("D4") + "|data.Name"];
                for (int jj = 0; jj < data.Segments.Count; jj++)
                {
                    ZoneSegmentBase structure = data.Segments[jj];
                    for (int kk = 0; kk < structure.ZoneSteps.Count; kk++)
                    {
                        FloorNameIDZoneStep postProc = structure.ZoneSteps[kk] as FloorNameIDZoneStep;
                        if (postProc != null)
                        {
                            //TODO: get these type names via reflection
                            nn++;
                            postProc.Name = rows[sort.ToString("D4") + "-" + key + "-" + nn.ToString("D4") + "|((FloorNameIDZoneStep)data.Segments[" + jj.ToString("D4") + "].ZoneSteps[" + kk.ToString("D4") + "]).Name"];
                        }
                    }
                }

                DataManager.SaveObject(data, dir);
            }
        }

        /// <summary>
        /// Reads a localization file and returns a dictionary of LocalText entries.
        /// </summary>
        /// <param name="path">The path to the localization file (typically *.out.txt).</param>
        /// <returns>A dictionary mapping keys to LocalText objects containing all translations.</returns>
        /// <remarks>
        /// Expected file format: Tab-separated with columns: Key, Comment, EN, [other languages].
        /// The first row is a header containing language codes.
        /// </remarks>
        public static Dictionary<string, LocalText> readLocalizationRows(string path)
        {
            Dictionary<string, LocalText> rows = new Dictionary<string, LocalText>();
            using (StreamReader inStream = new StreamReader(path))
            {
                string[] langs = inStream.ReadLine().Split('\t');
                while (!inStream.EndOfStream)
                {
                    string[] cols = inStream.ReadLine().Split('\t');
                    string key = cols[0];
                    LocalText text = new LocalText(cols[2]);
                    for (int ii = 3; ii < langs.Length; ii++)
                    {
                        if (ii < cols.Length && !String.IsNullOrWhiteSpace(cols[ii]))
                            text.LocalTexts.Add(langs[ii].ToLower(), cols[ii]);
                    }
                    rows[key] = text;
                }
            }
            return rows;
        }

    }
}
