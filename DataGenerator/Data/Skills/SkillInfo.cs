using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;
using RogueEssence;
using RogueEssence.Data;
using PMDC.Dungeon;
using PMDC;
using PMDC.Data;

namespace DataGenerator.Data
{
    /// <summary>
    /// Provides skill/move data generation and management for Pokemon Mystery Dungeon.
    /// This partial class handles loading, saving, and generating skill definitions.
    /// </summary>
    /// <remarks>
    /// Skill data is split across partial class files:
    /// <list type="bullet">
    /// <item><description>SkillInfo.cs - Core methods for skill data management</description></item>
    /// <item><description>SkillsPMD.cs - Pokemon moves from generations 1-4 (indices 0-467)</description></item>
    /// <item><description>SkillsGen5Plus.cs - Pokemon moves from generation 5 onwards (indices 468+)</description></item>
    /// </list>
    /// </remarks>
    public partial class SkillInfo
    {
        /// <summary>
        /// The maximum number of skills/moves supported by the data generator.
        /// </summary>
        public const int MAX_SKILLS = 901;

        /// <summary>
        /// Saves all unreleased move data to the data store.
        /// </summary>
        /// <remarks>
        /// Iterates through all skills and only saves those marked as unreleased.
        /// Useful for updating placeholder or work-in-progress move definitions.
        /// </remarks>
        public static void AddUnreleasedMoveData()
        {
            for (int ii = 0; ii < MAX_SKILLS; ii++)
            {

                (string, SkillData) move = GetSkillData(ii);
                if (!move.Item2.Released)
                    DataManager.SaveEntryData(move.Item1, DataManager.DataType.Skill.ToString(), move.Item2);
            }
        }

        /// <summary>
        /// Updates existing move entries with animation/effect data from generated move definitions.
        /// </summary>
        /// <param name="movesToAdd">
        /// Skill indices to update. If empty, updates all skills from 0 to <see cref="MAX_SKILLS"/>.
        /// </param>
        /// <remarks>
        /// Transfers only the AfterActions event data from generated moves to existing saved moves.
        /// Preserves all other existing move data while updating animation-related events.
        /// </remarks>
        public static void AddMoveDataToAnims(params int[] movesToAdd)
        {
            if (movesToAdd.Length > 0)
            {
                for (int ii = 0; ii < movesToAdd.Length; ii++)
                {

                    (string, SkillData) move = GetSkillData(movesToAdd[ii]);
                    SkillData oldMove = DataManager.LoadEntryData<SkillData>(move.Item1, DataManager.DataType.Skill.ToString());
                    if (oldMove != null)
                    {
                        oldMove.Data.AfterActions = move.Item2.Data.AfterActions;
                        DataManager.SaveEntryData(move.Item1, DataManager.DataType.Skill.ToString(), oldMove);
                    }
                }
            }
            else
            {
                for (int ii = 0; ii < MAX_SKILLS; ii++)
                {
                    (string, SkillData) move = GetSkillData(ii);
                    SkillData oldMove = DataManager.LoadEntryData<SkillData>(move.Item1, DataManager.DataType.Skill.ToString());
                    if (oldMove != null)
                    {
                        oldMove.Data.AfterActions = move.Item2.Data.AfterActions;
                        DataManager.SaveEntryData(move.Item1, DataManager.DataType.Skill.ToString(), oldMove);
                    }
                }
            }
        }

        /// <summary>
        /// Generates and saves move data to the data store.
        /// </summary>
        /// <param name="movesToAdd">
        /// Skill indices to generate and save. If empty, generates all skills from 0 to <see cref="MAX_SKILLS"/>.
        /// </param>
        /// <remarks>
        /// This is the primary method for populating the skill database.
        /// Overwrites any existing data for the specified skills.
        /// </remarks>
        public static void AddMoveData(params int[] movesToAdd)
        {
            if (movesToAdd.Length > 0)
            {
                for (int ii = 0; ii < movesToAdd.Length; ii++)
                {

                    (string, SkillData) move = GetSkillData(movesToAdd[ii]);
                    DataManager.SaveEntryData(move.Item1, DataManager.DataType.Skill.ToString(), move.Item2);
                }
            }
            else
            {
                for (int ii = 0; ii < MAX_SKILLS; ii++)
                {
                    (string, SkillData) move = GetSkillData(ii);
                    //System.Diagnostics.Debug.WriteLine(String.Format("{0}\t{1}", ii, move.Item1));
                    DataManager.SaveEntryData(move.Item1, DataManager.DataType.Skill.ToString(), move.Item2);
                }
            }
        }

        /// <summary>
        /// Generates a skill definition for the specified skill index.
        /// </summary>
        /// <param name="ii">The skill index (0 to <see cref="MAX_SKILLS"/> - 1).</param>
        /// <returns>
        /// A tuple containing the sanitized filename and the populated <see cref="SkillData"/> object.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Delegates to FillSkillsPMD for indices 0-467 or FillSkillsGen5Plus for 468+.
        /// </para>
        /// <para>
        /// Name prefixes have special meanings:
        /// <list type="bullet">
        /// <item><description>"**" - Unreleased skill (asterisks removed from final name)</description></item>
        /// <item><description>"-" - Released but no animation</description></item>
        /// <item><description>"=" - Released but no sound</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static (string, SkillData) GetSkillData(int ii)
        {
            SkillData skill = new SkillData();
            string fileName = "";
            skill.IndexNum = ii;

            if (ii < 468)
                FillSkillsPMD(skill, ii, ref fileName);
            else
                FillSkillsGen5Plus(skill, ii, ref fileName);

            if (skill.Name.DefaultText.StartsWith("**"))
                skill.Name.DefaultText = skill.Name.DefaultText.Replace("*", "");
            else if (skill.Name.DefaultText != "")
                skill.Released = true;

            if (skill.Name.DefaultText.StartsWith("-"))
            {
                skill.Name.DefaultText = skill.Name.DefaultText.Substring(1);
                skill.Comment = "No Anim";
                skill.Released = true;
            }
            else if (skill.Name.DefaultText.StartsWith("="))
            {
                skill.Name.DefaultText = skill.Name.DefaultText.Substring(1);
                skill.Comment = "No Sound";
                skill.Released = true;
            }

            if (fileName == "")
                fileName = Text.Sanitize(skill.Name.DefaultText).ToLower();

            return (fileName, skill);
        }

        /// <summary>
        /// Deletes all existing skill data and saves only the first skill (Attack).
        /// </summary>
        /// <remarks>
        /// Used for minimal testing scenarios where only the base "Attack" move is needed.
        /// Clears the entire skill data store before saving.
        /// </remarks>
        public static void AddMinMoveData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.Skill.ToString());
            for (int ii = 0; ii < 1; ii++)
            {
                (string, SkillData) move = GetSkillData(ii);
                DataManager.SaveEntryData(move.Item1, DataManager.DataType.Skill.ToString(), move.Item2);
            }
        }
    }
}