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
    /// Provides methods for generating skill group (egg group) data.
    /// Egg groups determine which Pokemon can breed together.
    /// </summary>
    public static class SkillGroupInfo
    {
        /// <summary>
        /// Defines all available Pokemon egg groups for breeding compatibility.
        /// Pokemon in the same egg group can breed together to produce offspring.
        /// </summary>
        public enum EggGroup
        {
            /// <summary>
            /// Cannot breed with any Pokemon. Includes most legendaries, baby Pokemon, and certain others.
            /// </summary>
            Undiscovered = 0,
            /// <summary>
            /// Special group for Ditto. Can breed with any Pokemon except Undiscovered group.
            /// </summary>
            Ditto = 1,
            /// <summary>
            /// Large, reptilian, or dinosaur-like Pokemon. Examples: Charizard, Tyranitar, Nidoking.
            /// </summary>
            Monster = 2,
            /// <summary>
            /// Amphibious Pokemon that can live on land and water. Examples: Blastoise, Lapras, Slowbro.
            /// </summary>
            Water1 = 3,
            /// <summary>
            /// Insect and arachnid Pokemon. Examples: Butterfree, Beedrill, Scizor.
            /// </summary>
            Bug = 4,
            /// <summary>
            /// Avian and winged Pokemon. Examples: Pidgeot, Crobat, Togekiss.
            /// </summary>
            Flying = 5,
            /// <summary>
            /// Mammalian and terrestrial Pokemon. Examples: Arcanine, Eevee, Lucario.
            /// </summary>
            Field = 6,
            /// <summary>
            /// Cute, small, fairy-like Pokemon. Examples: Clefairy, Pikachu, Marill.
            /// </summary>
            Fairy = 7,
            /// <summary>
            /// Plant-based Pokemon. Examples: Venusaur, Vileplume, Roserade.
            /// </summary>
            Grass = 8,
            /// <summary>
            /// Humanoid bipedal Pokemon. Examples: Alakazam, Machamp, Gardevoir.
            /// </summary>
            Humanlike = 9,
            /// <summary>
            /// Crustacean, cephalopod, and shellfish Pokemon. Examples: Omastar, Kingler, Cloyster.
            /// </summary>
            Water3 = 10,
            /// <summary>
            /// Inorganic, rock, or mineral-based Pokemon. Examples: Geodude, Steelix, Aggron.
            /// </summary>
            Mineral = 11,
            /// <summary>
            /// Amorphous or shapeless Pokemon. Examples: Muk, Gengar, Ditto.
            /// </summary>
            Amorphous = 12,
            /// <summary>
            /// Fish-like Pokemon. Examples: Goldeen, Magikarp, Lanturn.
            /// </summary>
            Water2 = 13,
            /// <summary>
            /// Draconic and serpentine Pokemon. Examples: Dragonite, Salamence, Garchomp.
            /// </summary>
            Dragon = 14
        }

        /// <summary>
        /// Maximum number of egg groups available.
        /// </summary>
        public const int MAX_GROUPS = 15;

        /// <summary>
        /// Generates and saves all skill group (egg group) data entries.
        /// </summary>
        public static void AddSkillGroupData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.SkillGroup.ToString());
            for (int ii = 0; ii < MAX_GROUPS; ii++)
            {
                SkillGroupData skillGroup = new SkillGroupData(new LocalText(Text.GetMemberTitle(((EggGroup)ii).ToString())));
                DataManager.SaveEntryData(Text.Sanitize(skillGroup.Name.DefaultText).ToLower(), DataManager.DataType.SkillGroup.ToString(), skillGroup);
            }
        }

        /// <summary>
        /// Generates minimal skill group data for testing with only the first group.
        /// </summary>
        public static void AddMinSkillGroupData()
        {
            DataInfo.DeleteIndexedData(DataManager.DataType.SkillGroup.ToString());
            for (int ii = 0; ii < 1; ii++)
            {
                SkillGroupData skillGroup = new SkillGroupData(new LocalText(Text.GetMemberTitle(((EggGroup)ii).ToString())));
                DataManager.SaveEntryData(Text.Sanitize(skillGroup.Name.DefaultText).ToLower(), DataManager.DataType.SkillGroup.ToString(), skillGroup);
            }
        }

    }
}

