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
    /// Provides methods and data for generating team rank data in the game.
    /// Team ranks determine progression milestones and bag size upgrades.
    /// </summary>
    public static class RankInfo
    {
        /// <summary>
        /// Defines the available team rank tiers from None to Grandmaster.
        /// Higher ranks unlock larger bag sizes and indicate team progression.
        /// </summary>
        public enum TeamRank
        {
            /// <summary>
            /// Special value indicating an unknown or invalid rank.
            /// </summary>
            Unknown = -1,
            /// <summary>
            /// Starting rank with no progression. Bag size: 24.
            /// </summary>
            None,
            /// <summary>
            /// Basic rank achieved with minimal experience. Bag size: 24.
            /// </summary>
            Normal,
            /// <summary>
            /// Third tier rank. Bag size increases to 32.
            /// </summary>
            Bronze,
            /// <summary>
            /// Fourth tier rank. Bag size increases to 40.
            /// </summary>
            Silver,
            /// <summary>
            /// Fifth tier rank. Bag size reaches maximum of 48.
            /// </summary>
            Gold,
            /// <summary>
            /// Sixth tier rank. Bag size: 48.
            /// </summary>
            Platinum,
            /// <summary>
            /// Seventh tier rank. Bag size: 48.
            /// </summary>
            Diamond,
            /// <summary>
            /// Eighth tier rank. Bag size: 48.
            /// </summary>
            Super,
            /// <summary>
            /// Ninth tier rank. Bag size: 48.
            /// </summary>
            Ultra,
            /// <summary>
            /// Tenth tier rank. Bag size: 48.
            /// </summary>
            Hyper,
            /// <summary>
            /// Eleventh tier rank. Bag size: 48.
            /// </summary>
            Master,
            /// <summary>
            /// Highest achievable rank. Bag size: 48.
            /// </summary>
            Grandmaster
        }

        /// <summary>
        /// Experience point thresholds required to advance to the next rank.
        /// Each index corresponds to a TeamRank value. A value of 0 indicates no further progression.
        /// </summary>
        public static int[] RANK_NEXT = new int[] { 1,
                                                    100,
                                                    300,
                                                    1200,
                                                    1600,
                                                    3200,
                                                    6400,
                                                    12000,
                                                    20000,
                                                    50000,
                                                    100000,
                                                    0};

        /// <summary>
        /// Maximum number of rank groups available.
        /// </summary>
        public const int MAX_GROUPS = 12;

        /// <summary>
        /// Generates and saves all team rank data entries.
        /// Configures rank names, experience requirements, and bag sizes for each rank tier.
        /// </summary>
        public static void AddRankData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.Rank.ToString());
            for (int ii = 0; ii < MAX_GROUPS; ii++)
            {
                string next = "";
                if (ii < MAX_GROUPS - 1)
                    next = Text.Sanitize(Text.GetMemberTitle(((TeamRank)ii + 1).ToString())).ToLower();
                RankData data = new RankData(new LocalText(Text.GetMemberTitle(((TeamRank)ii).ToString())), 24, RANK_NEXT[ii], next);
                if (ii == (int)TeamRank.None)
                    data.BagSize = 24;
                else if (ii == (int)TeamRank.Normal)
                    data.BagSize = 24;
                else if (ii == (int)TeamRank.Bronze)
                    data.BagSize = 32;
                else if (ii == (int)TeamRank.Silver)
                    data.BagSize = 40;
                else
                    data.BagSize = 48;
                DataManager.SaveEntryData(Text.Sanitize(data.Name.DefaultText).ToLower(), DataManager.DataType.Rank.ToString(), data);
            }
        }

        /// <summary>
        /// Generates minimal rank data with only a single rank entry.
        /// Used for testing or minimal game configurations.
        /// </summary>
        public static void AddMinRankData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.Rank.ToString());
            for (int ii = 0; ii < 1; ii++)
            {
                string next = "";
                RankData data = new RankData(new LocalText(Text.GetMemberTitle(((TeamRank)ii).ToString())), 24, RANK_NEXT[ii], next);
                data.BagSize = 24;
                DataManager.SaveEntryData(Text.Sanitize(data.Name.DefaultText).ToLower(), DataManager.DataType.Rank.ToString(), data);
            }
        }
    }
}

