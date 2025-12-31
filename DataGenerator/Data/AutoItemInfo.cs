using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using RogueEssence.LevelGen;
using RogueEssence;
using RogueEssence.Data;
using PMDC.Dungeon;
using PMDC;
using PMDC.Data;
using System.Text;
using RogueEssence.Content;
using DataGenerator.Dev;
using System.Linq;
using System.IO;
using RogueEssence.Script;

namespace DataGenerator.Data
{
    /// <summary>
    /// Defines special effects that can be applied to exclusive items.
    /// These effects provide unique bonuses based on Pokemon type, stats, or game mechanics.
    /// Star ratings (1*-5*) indicate effect rarity/power level.
    /// </summary>
    public enum ExclusiveItemEffect
    {
        /// <summary>
        /// No special effect applied to the item.
        /// </summary>
        None,
        /// <summary>
        /// Boosts a specific stat for Pokemon of a certain type. Rarity: 1*.
        /// </summary>
        TypeStatBonus,
        /// <summary>
        /// Increases movement speed for Pokemon of a certain type. Rarity: 3*.
        /// </summary>
        TypeSpeedBoost,
        /// <summary>
        /// Reduces damage from weaknesses for a type group. Rarity: 5*.
        /// </summary>
        TypeGroupWeaknessReduce,
        /// <summary>
        /// Allows certain type Pokemon to destroy terrain obstacles. Rarity: 4*.
        /// </summary>
        TypeBulldozer,
        /// <summary>
        /// Greatly increases critical hit ratio. Rarity: 3*.
        /// </summary>
        SuperCrit,
        /// <summary>
        /// Allows critical hits even on not-very-effective moves. Rarity: 2*.
        /// </summary>
        NVECrit,
        /// <summary>
        /// Prevents the Pokemon from being a traitor in missions. Rarity: 1*.
        /// </summary>
        Nontraitor,
        /// <summary>
        /// Allows movement across water terrain. Rarity: 2*.
        /// </summary>
        WaterTerrain,
        /// <summary>
        /// Allows movement across lava terrain. Rarity: 2*.
        /// </summary>
        LavaTerrain,
        /// <summary>
        /// Allows movement across all terrain types. Rarity: 4*.
        /// </summary>
        AllTerrain,
        /// <summary>
        /// Breaks walls when using moves. Rarity: 5*.
        /// </summary>
        Wallbreaker,
        /// <summary>
        /// Fills gaps in terrain when using moves. Rarity: 5*.
        /// </summary>
        GapFiller,
        /// <summary>
        /// Increases maximum PP for all moves. Rarity: 2*.
        /// </summary>
        PPBoost,
        /// <summary>
        /// Restores PP when the Pokemon takes a deep breath (waits). Rarity: 1*.
        /// </summary>
        DeepBreather,
        /// <summary>
        /// Increases accuracy when repeatedly using the same move. Rarity: 1*.
        /// </summary>
        PracticeSwinger,
        /// <summary>
        /// Reflects bad luck effects back to the source. Rarity: 1*.
        /// </summary>
        MisfortuneMirror,
        /// <summary>
        /// Reflects all normal attacks and projectiles back to attackers. Rarity: 3*.
        /// </summary>
        CounterBasher,
        /// <summary>
        /// Allows attacking twice in a single turn. Rarity: 2*.
        /// </summary>
        DoubleAttacker,
        /// <summary>
        /// Thrown items travel farther and pierce through targets. Rarity: 2*.
        /// </summary>
        MasterHurler,
        /// <summary>
        /// Revives fainted allies at the start of each floor. Rarity: 5*.
        /// </summary>
        AllyReviver,
        /// <summary>
        /// Revives fainted allies upon defeating a foe. Rarity: 5*.
        /// </summary>
        AllyReviverBattle,
        /// <summary>
        /// Enhanced Pressure ability effect, draining more PP from opponents. Rarity: 3*.
        /// </summary>
        PressurePlus,
        /// <summary>
        /// Normal attacks and projectiles can inflict a status condition. Rarity: 3*.
        /// </summary>
        StatusOnAttack,
        /// <summary>
        /// Normal attacks knock back the target. Rarity: 3*.
        /// </summary>
        KnockbackOnAttack,
        /// <summary>
        /// Normal attacks throw the target backward. Rarity: 3*.
        /// </summary>
        ThrowOnAttack,
        /// <summary>
        /// Normal attacks never miss. Rarity: 1*.
        /// </summary>
        SureHitAttacker,
        /// <summary>
        /// Normal attacks use Special Attack instead of Attack. Rarity: 1*.
        /// </summary>
        SpecialAttacker,
        /// <summary>
        /// Grants immunity to certain status conditions. Rarity: 3*.
        /// </summary>
        StatusImmune,
        /// <summary>
        /// Prevents stat reductions from opponent moves. Rarity: 2*.
        /// </summary>
        StatDropImmune,
        /// <summary>
        /// Allows movement and action while asleep. Rarity: 2*.
        /// </summary>
        SleepWalker,
        /// <summary>
        /// Allows movement while charging a move. Rarity: 3*.
        /// </summary>
        ChargeWalker,
        /// <summary>
        /// Reveals the location of stairs on the floor. Rarity: 4*.
        /// </summary>
        StairSensor,
        /// <summary>
        /// Reveals the location of items on the floor. Rarity: 2*.
        /// </summary>
        AcuteSniffer,
        /// <summary>
        /// Reveals floor layout within 20 tiles. Rarity: 5*.
        /// </summary>
        MapSurveyor,
        /// <summary>
        /// Reveals enemies and items through walls. Rarity: 5*.
        /// </summary>
        XRay,
        /// <summary>
        /// Inflicts a status condition when hit by a super-effective move. Rarity: 4*.
        /// </summary>
        WeaknessPayback,
        /// <summary>
        /// Warps the attacker away when hit by a super-effective move. Rarity: 4*.
        /// </summary>
        WarpPayback,
        /// <summary>
        /// Normal attacks move the user forward one tile. Rarity: 3*.
        /// </summary>
        LungeAttack,
        /// <summary>
        /// Normal attacks hit in a wider arc. Rarity: 2*.
        /// </summary>
        WideAttack,
        /// <summary>
        /// Normal attacks cause explosions. Rarity: 5*.
        /// </summary>
        ExplosiveAttack,
        /// <summary>
        /// Destroys traps when attacking them. Rarity: 4*.
        /// </summary>
        TrapBuster,
        /// <summary>
        /// Protects against explosion damage. Rarity: 3*.
        /// </summary>
        ExplosionGuard,
        /// <summary>
        /// Wand projectiles travel in an arc instead of straight. Rarity: 4*.
        /// </summary>
        WandMaster,
        /// <summary>
        /// Wand effects spread to adjacent tiles. Rarity: 5*.
        /// </summary>
        WandSpread,
        /// <summary>
        /// Linear projectiles become multi-ray shots but are consumed. Rarity: 5*.
        /// </summary>
        MultiRayShot,
        /// <summary>
        /// Restores nearby allies' HP when the user is at full HP. Rarity: 4*.
        /// </summary>
        RoyalVeil,
        /// <summary>
        /// Grants an extra turn after defeating an enemy. Rarity: 4*.
        /// </summary>
        Celebrate,
        /// <summary>
        /// Restores HP after defeating an enemy. Rarity: 3*.
        /// </summary>
        Absorption,
        /// <summary>
        /// Deals area-of-effect damage after defeating an enemy. Rarity: 3*.
        /// </summary>
        ExcessiveForce,
        /// <summary>
        /// Prevents forced movement and knockback effects. Rarity: 1*.
        /// </summary>
        Anchor,
        /// <summary>
        /// Reduces damage from multi-hit moves. Rarity: 2*.
        /// </summary>
        BarrageGuard,
        /// <summary>
        /// Guarantees hits and critical hits when all moves have equal PP. Rarity: 3*.
        /// </summary>
        BetterOdds,
        /// <summary>
        /// Dodges attacks when HP is critically low. Rarity: 4*.
        /// </summary>
        ClutchPerformer,
        /// <summary>
        /// Dodges attacks from distant enemies. Rarity: 4*.
        /// </summary>
        DistanceDodge,
        /// <summary>
        /// Dodges attacks from adjacent enemies. Rarity: 4*.
        /// </summary>
        CloseDodge,
        /// <summary>
        /// Increases natural HP recovery rate. Rarity: 5*.
        /// </summary>
        FastHealer,
        /// <summary>
        /// Automatically cures status conditions over time. Rarity: 3*.
        /// </summary>
        SelfCurer,
        /// <summary>
        /// Transfers curable status conditions to the target in front. Rarity: 3*.
        /// </summary>
        StatusMirror,
        /// <summary>
        /// Copies stat changes to the target in front. Rarity: 2*.
        /// </summary>
        StatMirror,
        /// <summary>
        /// Attack damage varies randomly. Rarity: 2*.
        /// </summary>
        ErraticAttacker,
        /// <summary>
        /// Incoming damage varies randomly. Rarity: 2*.
        /// </summary>
        ErraticDefender,
        /// <summary>
        /// Increases recruitment rate for wild Pokemon. Rarity: 2*. Meta-effect.
        /// </summary>
        FastFriend,
        /// <summary>
        /// Increases money found on the floor. Rarity: 3*. Meta-effect.
        /// </summary>
        CoinWatcher,
        /// <summary>
        /// Reveals hidden stairs on the floor. Rarity: 3*. Meta-effect.
        /// </summary>
        HiddenStairFinder,
        /// <summary>
        /// Reveals treasure chests on the floor. Rarity: 3*. Meta-effect.
        /// </summary>
        ChestFinder,
        /// <summary>
        /// Reveals shops on the floor. Rarity: 3*. Meta-effect.
        /// </summary>
        ShopFinder,
        /// <summary>
        /// Grants STAB bonus for a second type's moves. Rarity: 5*.
        /// </summary>
        SecondSTAB,
        /// <summary>
        /// Changes normal attack type to match the user's type. Rarity: 4*.
        /// </summary>
        TypedAttack,
        /// <summary>
        /// Normal attacks can hit through gaps in terrain. Rarity: 1*.
        /// </summary>
        GapProber,
        /// <summary>
        /// Normal attacks pass through targets to hit enemies behind. Rarity: 1*.
        /// </summary>
        PassThroughAttacker,
        /// <summary>
        /// Protects against weather-based damage.
        /// </summary>
        WeatherProtection,
        /// <summary>
        /// Normal attacks can remove the target's ability.
        /// </summary>
        RemoveAbilityAttack,
        /// <summary>
        /// Restores HP when eating berries, similar to the Cheek Pouch ability.
        /// </summary>
        CheekPouch,
        /// <summary>
        /// Gradually restores HP while standing in water.
        /// </summary>
        HealInWater,
        /// <summary>
        /// Gradually restores HP while standing in lava.
        /// </summary>
        HealInLava,
        /// <summary>
        /// Restores HP when entering a new floor.
        /// </summary>
        HealOnNewFloor,
        /// <summary>
        /// Survives a fatal hit from moves of a certain category with 1 HP.
        /// </summary>
        EndureCategory,
        /// <summary>
        /// Survives a fatal hit from moves of a certain type with 1 HP.
        /// </summary>
        EndureType,
        /// <summary>
        /// Drops spike traps when hit by attacks.
        /// </summary>
        SpikeDropper,
        /// <summary>
        /// Prevents status conditions during certain weather.
        /// </summary>
        NoStatusInWeather,
        /// <summary>
        /// Enhanced deep breathing that restores more PP. Rarity: 3*.
        /// </summary>
        DeepBreatherPlus,
        /// <summary>
        /// Reduces damage from super-effective moves. Rarity: 5*.
        /// </summary>
        WeaknessReduce,
        /// <summary>
        /// Boosts stats when healed by an ally. Rarity: 2*.
        /// </summary>
        Gratitude,
        /// <summary>
        /// Allows retreating after attacking without counterattack. Rarity: 2*.
        /// </summary>
        HitAndRun,
        /// <summary>
        /// Inflicts status when hitting with moves of a certain category. Rarity: 3*.
        /// </summary>
        StatusOnCategoryHit,
        /// <summary>
        /// Inflicts status when using moves of a certain category. Rarity: 5*.
        /// </summary>
        StatusOnCategoryUse,
        /// <summary>
        /// Applies a map-wide status when using moves of a certain category. Rarity: 5*.
        /// </summary>
        MapStatusOnCategoryUse,
        /// <summary>
        /// Can dash twice the normal distance. Rarity: 4*.
        /// </summary>
        DoubleDash,
        /// <summary>
        /// Status conditions splash to adjacent enemies. Rarity: 5*.
        /// </summary>
        StatusSplash,
        /// <summary>
        /// Normal attacks can devolve the target. Rarity: 3*.
        /// </summary>
        DevolveOnAttack,
        /// <summary>
        /// Normal attacks become projectiles. Rarity: 4*.
        /// </summary>
        ProjectileAttack,
        /// <summary>
        /// Enhances the Metronome move with better outcomes. Rarity: 5*.
        /// </summary>
        MetronomePlus,
        /// <summary>
        /// Increases attack range during certain weather. Rarity: 4*.
        /// </summary>
        AttackRangeInWeather,
        /// <summary>
        /// Reduces PP consumption for moves.
        /// </summary>
        PPSaver,
        /// <summary>
        /// Cures status conditions when defeating an enemy.
        /// </summary>
        CelebrateCure,
        /// <summary>
        /// Protects allies of a certain type from damage.
        /// </summary>
        TypeBodyguard,
        /// <summary>
        /// Boosts stats when using moves of a certain category.
        /// </summary>
        StatOnCategoryUse,
        /// <summary>
        /// Inflicts status when hit by moves of a certain type.
        /// </summary>
        StatusOnTypeHit,
        /// <summary>
        /// Restores HP and PP while sleeping.
        /// </summary>
        SweetDreams,
        /// <summary>
        /// Has a chance to inflict status when hit by moves of a certain type.
        /// </summary>
        ChanceStatusOnTypeHit,
        /// <summary>
        /// Has a chance to boost stats when hit by moves of a certain type.
        /// </summary>
        ChanceStatOnTypeHit,
        /// <summary>
        /// Has a chance to inflict status when hit by moves of a certain category.
        /// </summary>
        ChanceStatusOnCategoryHit,
        /// <summary>
        /// Changes moves of a certain type to a different damage category.
        /// </summary>
        TypeBecomesCategory,
        /// <summary>
        /// Has a chance to dodge super-effective moves.
        /// </summary>
        WeaknessDodge,
        /// <summary>
        /// Reveals heart-shaped items on the floor.
        /// </summary>
        HeartFinder,
        /// <summary>
        /// Inflicts a status condition when defeating an enemy.
        /// </summary>
        CelebrateStatus
    }

    /// <summary>
    /// Provides methods for auto-generating exclusive item data.
    /// Handles item naming, effects, and localization for Pokemon-specific equipment.
    /// </summary>
    public class AutoItemInfo
    {
        /// <summary>All loaded localization strings indexed by language code.</summary>
        private static Dictionary<string, Dictionary<string, string>> stringsAll;
        /// <summary>Special localization rows for unique items.</summary>
        private static Dictionary<string, LocalText> specialRows;
        /// <summary>Localization rows for specific named items.</summary>
        private static Dictionary<string, LocalText> specificItemRows;
        /// <summary>Localization rows for exclusive item type names.</summary>
        private static Dictionary<string, LocalText> itemTypeRows;
        /// <summary>Localization rows for exclusive item effect descriptions.</summary>
        private static Dictionary<string, LocalText> itemEffectRows;

        /// <summary>
        /// Initializes all localization string dictionaries from resource files.
        /// Must be called before generating translated item data.
        /// </summary>
        public static void InitStringsAll()
        {
            stringsAll = new Dictionary<string, Dictionary<string, string>>();
            stringsAll[""] = Text.LoadStringResx(PathMod.ModPath("Strings/strings.resx"));
            foreach (string code in Text.SupportedLangs)
            {
                if (code != "en")
                    stringsAll[code] = Text.LoadStringResx(PathMod.ModPath("Strings/strings." + code + ".resx"));
            }

            specialRows = Localization.readLocalizationRows(GenPath.TL_PATH  + "Special.out.txt");
            specificItemRows = Localization.readLocalizationRows(GenPath.TL_PATH + "Item.out.txt");
            itemTypeRows = Localization.readLocalizationRows(GenPath.TL_PATH + "ExclusiveItemType.out.txt");
            itemEffectRows = Localization.readLocalizationRows(GenPath.TL_PATH + "ExclusiveItemEffect.out.txt");
        }
        //this class will not be auto-generated
        //it will only contain english translations
        //it will ask datastringinfo for translations
        //when scraping translations, the algorithm will iterate the local expressions and effect list names separately
        //it will return with localtexts and then send them over to translation
        //when read back it will update datastringinfo for those enums
        //in this case, we can send arguments for string formatting too
        //since the callers of getLocalExpression for the purpose of string scraping can just use {0}, {1}, etc?


        //when it comes to custom specific item names,
        //it will go through all item names in the specific item range and print to list only if the exclusive item type is none

        /// <summary>
        /// Gets a localized name expression for an exclusive item type.
        /// </summary>
        /// <param name="type">The exclusive item type.</param>
        /// <param name="translate">If true, returns translated text; otherwise returns English with type name.</param>
        /// <returns>A LocalText containing the formatted item type name.</returns>
        public static LocalText GetLocalExpression(ExclusiveItemType type, bool translate)
        {
            if (!translate)
            {
                LocalText formatName = new LocalText("{0} " + type.ToString());
                return formatName;
            }
            else
            {
                LocalText formatName = itemTypeRows[typeof(ExclusiveItemType).Name + "." + type];
                return formatName;
            }
        }

        /// <summary>
        /// Creates content lists for item types, effects, and related game data.
        /// Used for generating documentation and validation of item configurations.
        /// </summary>
        public static void CreateContentLists()
        {
            List<string> itemTypes = new List<string>();
            List<string> effectTypes = new List<string>();
            Dictionary<string, string> effectRarities = new Dictionary<string, string>();
            Dictionary<string, string> effectDescriptions = new Dictionary<string, string>();
            List<string> stats = new List<string>();
            List<string> monsters = new List<string>();
            List<string> elements = new List<string>();
            List<string> statuses = new List<string>();
            List<string> mapStatuses = new List<string>();
            List<string> categories = new List<string>();


            foreach (ExclusiveItemType nameType in Enum.GetValues(typeof(ExclusiveItemType)).Cast<ExclusiveItemType>())
            {
                int rarity = 0;
                if (nameType >= ExclusiveItemType.Claw)
                    rarity++;
                if (nameType >= ExclusiveItemType.Tag)
                    rarity++;
                if (nameType >= ExclusiveItemType.Blade)
                    rarity++;
                itemTypes.Add(rarity + "* " + nameType.ToString());
            }
            itemTypes.Sort();

            foreach (ExclusiveItemEffect descType in Enum.GetValues(typeof(ExclusiveItemEffect)).Cast<ExclusiveItemEffect>())
            {


                ItemData item = new ItemData();
                item.UseEvent.Element = "none";
                item.SortCategory = 17;
                AutoItemInfo.FillExclusiveEffects("", item, new List<LocalText>(), false, descType, new object[0], false);


                effectTypes.Add(((int)descType).ToString("D3") + ": " + descType.ToString());
                effectRarities.Add(((int)descType).ToString("D3") + ": " + descType.ToString(), item.Rarity + "*");
                effectDescriptions.Add(((int)descType).ToString("D3") + ": " + descType.ToString(), item.Desc.DefaultText);
            }
            effectTypes.Sort();

            foreach (Stat statType in Enum.GetValues(typeof(Stat)))
            {
                if ((int)statType > -1)
                    stats.Add(((int)statType).ToString("D3") + ": " + statType.ToString());
            }


            foreach (string key in DataManager.Instance.DataIndices[DataManager.DataType.Monster].GetOrderedKeys(false))
                monsters.Add(key);

            foreach(string key in DataManager.Instance.DataIndices[DataManager.DataType.Element].GetOrderedKeys(false))
                elements.Add(key);

            foreach (string key in DataManager.Instance.DataIndices[DataManager.DataType.Status].GetOrderedKeys(false))
            {
                StatusData data = DataManager.Instance.GetStatus(key);
                if (data.MenuName && data.Name.DefaultText != "" && data.StatusStates.Contains<TransferStatusState>())
                    statuses.Add(key);
            }

            foreach (string key in DataManager.Instance.DataIndices[DataManager.DataType.MapStatus].GetOrderedKeys(false))
            {
                MapStatusData data = DataManager.Instance.GetMapStatus(key);
                if (!data.DefaultHidden && data.Name.DefaultText != "")
                    mapStatuses.Add(key);
            }

            foreach (BattleData.SkillCategory statType in Enum.GetValues(typeof(BattleData.SkillCategory)).Cast<BattleData.SkillCategory>())
                categories.Add(((int)statType).ToString("D3") + ": " + statType.ToString());


            string path = GenPath.ITEM_PATH  + "PreAutoItem.txt";
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("ItemType\tEffectType\tStat\tMonster\tElement\tStatus\tMapStatus\tCategory");
                int ii = 0;
                bool completed = false;
                while (!completed)
                {
                    completed = true;

                    List<string> row = new List<string>();

                    if (ii < itemTypes.Count)
                    {
                        row.Add(itemTypes[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    if (ii < effectTypes.Count){
                        row.Add(effectTypes[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    if (ii < stats.Count){
                        row.Add(stats[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    if (ii < monsters.Count){
                        row.Add(monsters[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    if (ii < elements.Count){
                        row.Add(elements[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    if (ii < statuses.Count){
                        row.Add(statuses[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    if (ii < mapStatuses.Count)
                    {
                        row.Add(mapStatuses[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    if (ii < categories.Count)
                    {
                        row.Add(categories[ii]);
                        completed = false;
                    }
                    else
                        row.Add("");
                    file.WriteLine(String.Join('\t', row.ToArray()));
                    ii++;
                }
            }
            path = GenPath.ITEM_PATH  + "AutoItemRef.txt";
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("Rarity\tEffectType\tDescription");
                for(int ii = 0; ii < effectTypes.Count; ii++)
                {
                    file.WriteLine(effectRarities[effectTypes[ii]] + '\t' + effectTypes[ii] + '\t' + effectDescriptions[effectTypes[ii]]);
                }
            }

            List<List<string>> evoTrees = new List<List<string>>();
            List<bool> branchedEvo = new List<bool>();
            HashSet<string> traversed = new HashSet<string>();

            List<string> monsterKeys = DataManager.Instance.DataIndices[DataManager.DataType.Monster].GetOrderedKeys(true);
            for (int ii = 0; ii < monsterKeys.Count; ii++)
            {
                if (traversed.Contains(monsterKeys[ii]))
                    continue;

                MonsterData data = DataManager.Instance.GetMonster(monsterKeys[ii]);
                if (!data.Released)
                    continue;

                List<string> dexNums = new List<string>();
                bool branched = FindFullFamily(dexNums, monsterKeys[ii]);
                evoTrees.Add(dexNums);
                branchedEvo.Add(branched);
                foreach (string dexNum in dexNums)
                    traversed.Add(dexNum);
            }

            path = GenPath.ITEM_PATH  + "EvoTreeRef.txt";
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("Family\tUser\tEvo\tIndex\tTrade\tRarity");
                for (int ii = 0; ii < evoTrees.Count; ii++)
                {
                    if (ii == 0)
                        continue;
                    List<string> evoTree = evoTrees[ii];

                    int familyIndex = int.MaxValue;
                    foreach (string species in evoTree)
                    {
                        MonsterEntrySummary familyEntrySummary = (MonsterEntrySummary)DataManager.Instance.DataIndices[DataManager.DataType.Monster].Get(species);
                        if (familyEntrySummary.SortOrder < familyIndex)
                            familyIndex = familyEntrySummary.SortOrder;
                    }

                    if (branchedEvo[ii])
                    {
                        for (int jj = 0; jj < 6; jj++)
                        {
                            if (jj < 2)
                                file.WriteLine(familyIndex.ToString() + "\t" + evoTree[Math.Max(0, evoTree.Count - 1 - jj)] + "\tFALSE\t" + (char)('A' + jj) + "\t@\t5*");
                            else if (jj < 4)
                                file.WriteLine(familyIndex.ToString() + "\t" + evoTree[Math.Max(0, evoTree.Count - 1 - jj)] + "\tFALSE\t" + (char)('A' + jj) + "\t@\t1-2*");
                            else
                                file.WriteLine(familyIndex.ToString() + "\t" + evoTree[Math.Max(0, evoTree.Count - 1 - jj)] + "\tFALSE\t" + (char)('A' + jj) + "\t@\t3-4*");
                        }
                    }
                    else if (evoTree.Count == 3)
                    {
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[2] + "\tTRUE\tA\t*\t5*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[1] + "\tTRUE\tB\t*\t4*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tTRUE\tC\t*\t1-2*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tTRUE\tD\t*\t1-2*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tTRUE\tE\t*\t2-3*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[1] + "\tTRUE\tF\t*\t2-3*");
                    }
                    else if (evoTree.Count == 2)
                    {
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[1] + "\tTRUE\tA\t*\t5*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[1] + "\tTRUE\tB\t*\t4*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tTRUE\tC\t*\t1-2*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tTRUE\tD\t*\t1-2*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tTRUE\tE\t*\t2-3*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[1] + "\tTRUE\tF\t*\t2-3*");
                    }
                    else if (evoTree.Count == 1)
                    {
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tFALSE\tA\t*\t5*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tFALSE\tB\t*\t4*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tFALSE\tC\t*\t1-2*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tFALSE\tD\t*\t1-2*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tFALSE\tE\t*\t2-3*");
                        file.WriteLine(familyIndex.ToString() + "\t" + evoTree[0] + "\tFALSE\tF\t*\t2-3*");
                    }
                }
            }
        }


        public static List<string[]> LoadItemRows(string path)
        {
            List<string[]> rows = new List<string[]>();
            using (StreamReader inStream = new StreamReader(path))
            {
                string[] header = inStream.ReadLine().Split('\t');
                while (!inStream.EndOfStream)
                {
                    string[] row = inStream.ReadLine().Split('\t');
                    rows.Add(row);
                }
            }
            return rows;
        }

        public static void WriteExclusiveItems(int init_idx, bool translate)
        {
            int incompleteLeft = 0;
            List<(string item, string[] reqItem)> specific_tradeables = new List<(string, string[])>();
            List<(string item, string dex, int reqCount)> random_tradeables = new List<(string, string, int)>();

            //load from generated csv
            if (File.Exists(GenPath.ITEM_PATH  + "ExclusiveItem.out.txt"))
            {
                Dictionary<int, bool> available_species = new Dictionary<int, bool>();
                HashSet<string> takenNames = new HashSet<string>();
                //read the Monster/releases.out.txt
                using (StreamReader file = new StreamReader(MonsterInfo.MONSTER_PATH + "releases.out.txt"))
                {
                    string[] header = file.ReadLine().Split('\t');
                    while (!file.EndOfStream)
                    {
                        string[] checks = file.ReadLine().Trim().Split('\t');

                        //get a list of all species that have at least one row that is certified + NOT NeedsPlace
                        int family_num = Int32.Parse(checks[0]);
                        bool certified = checks[10] == "True";
                        bool needsPlace = checks[12] == "True";
                        if (certified && !needsPlace)
                            available_species[family_num] = true;
                    }
                }

                List<string[]> rows = LoadItemRows(GenPath.ITEM_PATH  + "ExclusiveItem.out.txt");

                Dictionary<int, List<string[]>> families = new Dictionary<int, List<string[]>>();
                int prev_family = 0;
                for (int ii = 0; ii < rows.Count; ii++)
                {
                    string[] row = rows[ii];
                    int family_idx = Int32.Parse(row[3]);
                    if (family_idx < prev_family)
                        throw new Exception(String.Format("Out of order family on row {0}!", ii));
                    else if (family_idx == prev_family)
                    {
                        List<string[]> family_rows = families[family_idx];
                        char chr1 = family_rows[family_rows.Count - 1][10][0];
                        char chr2 = row[10][0];
                        if (chr1 + 1 != chr2)
                            throw new Exception(String.Format("Out of order labeling on row {0}!", ii));
                    }

                    if (!families.ContainsKey(family_idx))
                    {
                        if (row[10] != "A")
                            throw new Exception(String.Format("Out of order labeling on row {0}!", ii));
                        families[family_idx] = new List<string[]>();
                    }
                    families[family_idx].Add(row);
                    prev_family = family_idx;
                }

                List<int> family_list = new List<int>();
                foreach (int family_idx in families.Keys)
                    family_list.Add(family_idx);
                family_list.Sort();


                for (int hh = 0; hh < family_list.Count; hh++)
                {
                    int family_idx = family_list[hh];
                    List<string[]> family_rows = families[family_idx];

                    //decide on a family name
                    string familyName = family_rows[0][7];
                    if (familyName == "")
                    {
                        string earliestFamily = family_rows[0][2].Trim();
                        for (int nn = 1; nn < family_rows.Count; nn++)
                        {
                            MonsterEntrySummary earliestSummary = (MonsterEntrySummary)DataManager.Instance.DataIndices[DataManager.DataType.Monster].Get(earliestFamily);
                            string dexNum = family_rows[nn][2].Trim();

                            MonsterEntrySummary newSummary = (MonsterEntrySummary)DataManager.Instance.DataIndices[DataManager.DataType.Monster].Get(dexNum);
                            if (newSummary.SortOrder < earliestSummary.SortOrder)
                                earliestFamily = dexNum;
                        }
                        familyName = earliestFamily;
                    }

                    //if a excl item family has at least one member available, allow that item family to be written out
                    bool familyEnabled = available_species.ContainsKey(family_idx);

                    List<string[]> tradeables = new List<string[]>();

                    for (int ii = 0; ii < family_rows.Count; ii++)
                    {
                        string[] row = family_rows[ii];

                        string customName = row[1].Trim();
                        ExclusiveItemType exclType = (ExclusiveItemType)Enum.Parse(typeof(ExclusiveItemType), row[0].Substring(2));
                        ExclusiveItemEffect exclEffect = (ExclusiveItemEffect)Int32.Parse(row[13].Substring(0, 3));
                        string primaryDex = row[2].Trim();

                        string[] rarityStr = row[12].Substring(0, row[12].Length - 1).Split('-');
                        int minRarity = Int32.Parse(rarityStr[0]);
                        int maxRarity = minRarity;
                        if (rarityStr.Length > 1)
                            maxRarity = Int32.Parse(rarityStr[1]);


                        List<object> args = new List<object>();
                        if (row[14] != "")
                            args.Add(row[14].Trim());
                        if (row[15] != "")
                            args.Add(row[15].Trim());
                        if (row[16] != "")
                            args.Add(new Stat[] { (Stat)Int32.Parse(row[16].Substring(0, 3)) });
                        if (row[17] != "")
                            args.Add(row[17].Trim());
                        if (row[18] != "")
                            args.Add((BattleData.SkillCategory)Int32.Parse(row[18].Substring(0, 3)));
                        if (row[19] != "")
                            args.Add(Int32.Parse(row[19]));
                        if (row[20] != "")
                            args.Add(row[20].Trim());

                        List<string> familyStarts = new List<string>();
                        if (row[6] != "")
                        {
                            string[] startDexes = row[6].Split(',');
                            foreach (string startDex in startDexes)
                            {
                                string cutoff = startDex.Trim();
                                familyStarts.Add(cutoff);
                            }
                        }
                        else
                            familyStarts.Add(primaryDex);

                        bool includeFamily = row[5] == "True";
                        List<string> dexNums = new List<string>();
                        foreach (string dex in familyStarts)
                        {
                            if (includeFamily)
                            {
                                string firstStage = dex;
                                FindFullFamily(dexNums, dex);
                            }
                            else
                                dexNums.Add(dex);
                        }

                        string fileName = getAssetName(familyName, ii);

                        ItemData item = new ItemData();
                        item.UseEvent.Element = "none";
                        item.SortCategory = 17;

                        if (exclType != ExclusiveItemType.None && customName != "")
                            Console.WriteLine(String.Format("Item {0} found with both name \"{1}\" and type {2}.", fileName, customName, exclType));

                        AutoItemInfo.FillExclusiveData(fileName, item, "", customName, exclType, exclEffect, args.ToArray(), primaryDex, dexNums, translate, includeFamily);

                        item.Rarity = (Math.Clamp(item.Rarity, minRarity, maxRarity) - 1) / 2 + 1;
                        item.Sprite = "Box_Yellow";
                        item.Icon = 10;
                        item.Price = 800 * item.Rarity;
                        item.UsageType = ItemData.UseType.Treasure;
                        item.ItemStates.Set(new MaterialState());
                        item.BagEffect = true;
                        item.CannotDrop = true;


                        bool enabled = row[4] == "True";

                        if (item.Name.DefaultText.StartsWith("**"))
                            item.Name.DefaultText = item.Name.DefaultText.Replace("*", "");
                        else if (item.Name.DefaultText != "" && enabled && familyEnabled)
                            item.Released = true;

                        if (item.Name.DefaultText != "" && !item.Released)
                            incompleteLeft++;

                        if (item.Name.DefaultText != "")
                        {
                            if (takenNames.Contains(item.Name.DefaultText))
                                Console.WriteLine(String.Format("Name '{0}' is already taken!", item.Name.DefaultText));
                            takenNames.Add(item.Name.DefaultText);

                            DataManager.SaveEntryData(fileName, DataManager.DataType.Item.ToString(), item);
                        }

                        if (item.Released)
                        {
                            tradeables.Add(row);
                            if (tradeables.Count < ii + 1)
                                throw new Exception(String.Format("Out of order releasing on {0}!", fileName));
                        }

                    }


                    if (tradeables.Count == 0)
                    {

                    }
                    else if (tradeables[0][11] == "*")
                    {
                        //autocalculate normal case
                        if (tradeables.Count == 4)
                        {
                            //C+D+B=A
                            addSpecificTradeable(specific_tradeables, familyName, 'A', 'C', 'D', 'B');
                            //C+D=B
                            addSpecificTradeable(specific_tradeables, familyName, 'B', 'C', 'D');
                        }
                        else if (tradeables.Count == 5)
                        {
                            //C+D+B=A
                            addSpecificTradeable(specific_tradeables, familyName, 'A', 'C', 'D', 'B');
                            //C+D=B
                            addSpecificTradeable(specific_tradeables, familyName, 'B', 'C', 'D');
                            //C+D=E
                            addSpecificTradeable(specific_tradeables, familyName, 'E', 'C', 'D');

                        }
                        else if (tradeables.Count == 6)
                        {
                            //E+F+B=A
                            addSpecificTradeable(specific_tradeables, familyName, 'A', 'B', 'E');
                            //E+F=B
                            addSpecificTradeable(specific_tradeables, familyName, 'B', 'C', 'D');
                            //C+D=E
                            addSpecificTradeable(specific_tradeables, familyName, 'E', 'C', 'D');
                        }
                    }
                    else if (tradeables[0][11] == "@")
                    {
                        //autocalculate branch evo case
                        if (tradeables.Count == 5)
                        {
                            //C+D+E=A/B
                            addSpecificTradeable(specific_tradeables, familyName, 'A', 'C', 'D', 'E');
                            addSpecificTradeable(specific_tradeables, familyName, 'B', 'C', 'D', 'E');
                            //C+D=E
                            addSpecificTradeable(specific_tradeables, familyName, 'E', 'C', 'D');
                        }
                        else if (tradeables.Count == 6)
                        {
                            //C+D+E=A
                            addSpecificTradeable(specific_tradeables, familyName, 'A', 'C', 'D', 'E');
                            //C+D+F=B
                            addSpecificTradeable(specific_tradeables, familyName, 'B', 'C', 'D', 'F');
                            //C+D=E/F
                            addSpecificTradeable(specific_tradeables, familyName, 'E', 'C', 'D');
                            addSpecificTradeable(specific_tradeables, familyName, 'F', 'C', 'D');
                        }
                    }
                    else if (tradeables[0][11] == "/")
                    {
                        //autocalculate counterpart case
                        if (tradeables.Count == 6)
                        {
                            //C+D+E=A
                            addSpecificTradeable(specific_tradeables, familyName, 'A', 'C', 'D', 'E');
                            //C+D+F=B
                            addSpecificTradeable(specific_tradeables, familyName, 'B', 'C', 'D', 'F');
                            //C+D=E/F
                            addSpecificTradeable(specific_tradeables, familyName, 'E', 'C', 'D');
                            addSpecificTradeable(specific_tradeables, familyName, 'F', 'C', 'D');
                        }
                        else if (tradeables.Count == 7)
                        {
                            //G+E=A
                            addSpecificTradeable(specific_tradeables, familyName, 'A', 'G', 'E');
                            //G+F=B
                            addSpecificTradeable(specific_tradeables, familyName, 'B', 'G', 'F');
                            //C+D=E/F
                            addSpecificTradeable(specific_tradeables, familyName, 'E', 'C', 'D');
                            addSpecificTradeable(specific_tradeables, familyName, 'F', 'C', 'D');
                        }
                    }
                    else
                    {
                        for (int nn = 0; nn < tradeables.Count; nn++)
                        {
                            List<string> trade_ins = new List<string>();
                            for (int kk = 0; kk < tradeables[nn][11].Length; kk++)
                                trade_ins.Add(getAssetName(familyName, tradeables[nn][11][kk] - 'A'));

                            //only add if they have trade-ins
                            if (trade_ins.Count > 0)
                                specific_tradeables.Add((getAssetName(familyName, nn), trade_ins.ToArray()));
                        }
                    }

                    //add to random tradeable pool if...
                    for (int nn = 0; nn < tradeables.Count; nn++)
                    {
                        string item_id = getAssetName(familyName, nn);
                        bool has_tradeables = false;
                        for (int kk = 0; kk < specific_tradeables.Count; kk++)
                        {
                            if (specific_tradeables[kk].item == item_id)
                            {
                                has_tradeables = true;
                                break;
                            }
                        }
                        //they are at the bottom of their trade chain
                        if (!has_tradeables)
                        {
                            ItemData itemData = DataManager.LoadEntryData<ItemData>(item_id, DataManager.DataType.Item.ToString());
                            //has a rarity of 2 or lower
                            if (itemData.Rarity <= 2)
                            {
                                string species = tradeables[nn][2];

                                bool excluded = false;
                                foreach (string legend in ZoneInfo.IterateLegendaries())
                                {
                                    if (legend == species)
                                        excluded = true;

                                }
                                if (!excluded)
                                    random_tradeables.Add((item_id, species, itemData.Rarity));
                            }
                        }
                    }

                }

            }

            //output trade tables to common_gen.lua
            string path = PathMod.ModPath(Path.Join(LuaEngine.SCRIPT_PATH, PathMod.BaseNamespace, "common_gen.lua"));
            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write("--[[\n" +
                "    common_gen.lua\n" +
                "    A generated collection of values\n" +
                "]]--\n" +
                "COMMON_GEN = {}\n" +
                "\n" +
                "COMMON_GEN.TRADES = {\n");
                foreach ((string item, string[] reqItem) trade in specific_tradeables)
                {
                    List<string> wildcards = new List<string>();
                    foreach(string req in trade.reqItem)
                        wildcards.Add("\""+ req + "\"");
                    file.Write("{ Item=\"" + trade.item + "\", ReqItem={" + String.Join(',', wildcards) + "}},\n");
                }
                file.Write("}\n\n");
                file.Write("COMMON_GEN.TRADES_RANDOM = {\n");
                foreach ((string item, string dex, int reqCount) trade in random_tradeables)
                {
                    List<string> wildcards = new List<string>();
                    for (int nn = 0; nn < trade.reqCount * 2; nn++)
                        wildcards.Add("\"\"");
                    file.Write("{ Item=\"" + trade.item + "\", Dex=\"" + trade.dex + "\", ReqItem={" + String.Join(',', wildcards.ToArray()) + "}},\n");
                }
                file.Write("}\n\n");
            }

            Console.WriteLine(String.Format("Incomplete Left: {0}", incompleteLeft));
        }

        private static void addSpecificTradeable(List<(string, string[])> specific_tradeables, string family_name, char result, params char[] trades)
        {
            List<string> trade_items = new List<string>();
            foreach (char trade in trades)
            {
                trade_items.Add(getAssetName(family_name, trade - 'A'));
            }

            specific_tradeables.Add((getAssetName(family_name, result - 'A'), trade_items.ToArray()));
        }

        private static string getAssetName(string familyName, int idx)
        {
            return String.Format("xcl_family_{0}_{1}", familyName, idx.ToString("D2"));
        }


        /// <summary>
        /// The purpose of this function is:
        /// - To fill in the item effects based on the type and args.
        /// - To fill in the string description of the item effects, to be ready for local format.
        /// - To return the raw un-formatted string expressions to be ready for translation.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="localArgs"></param>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public static void FillExclusiveEffects(string itemID, ItemData item, List<LocalText> localArgs, bool includeEffects, ExclusiveItemEffect type, object[] args, bool translate)
        {
            if (type == ExclusiveItemEffect.TypeStatBonus)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, it slightly boosts the {1} of {0}-type members.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetElement((string)args[0]).Name);
                    Stat[] boostedStats = (Stat[])args[1];
                    List<LocalText> nameList = new List<LocalText>();
                    foreach (Stat stat in boostedStats)
                        nameList.Add(ToLocalText(stat, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));

                    localArgs.Add(BuildLocalTextList(nameList, translate));

                    foreach (Stat stat in boostedStats)
                    {
                        if (stat == Stat.Attack)
                            item.OnActions.Add(0, new TypeSpecificMultCategoryEvent((string)args[0], new DustState(), BattleData.SkillCategory.Physical, 10, 1));
                        else if (stat == Stat.MAtk)
                            item.OnActions.Add(0, new TypeSpecificMultCategoryEvent((string)args[0], new DustState(), BattleData.SkillCategory.Magical, 10, 1));
                        else if (stat == Stat.Defense)
                            item.BeforeBeingHits.Add(0, new TypeSpecificMultCategoryEvent((string)args[0], new SilkState(), BattleData.SkillCategory.Physical, 10, 1));
                        else if (stat == Stat.MDef)
                            item.BeforeBeingHits.Add(0, new TypeSpecificMultCategoryEvent((string)args[0], new SilkState(), BattleData.SkillCategory.Magical, 10, 1));
                    }
                }
            }
            else if (type == ExclusiveItemEffect.TypeSpeedBoost)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, it boosts the Movement Speed of {0}-type members that have no status conditions.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetElement((string)args[0]).Name);

                    item.OnRefresh.Add(0, new AddTypeSpeedEvent((string)args[0], 1, new GemBoostState()));
                }
            }
            else if (type == ExclusiveItemEffect.TypeGroupWeaknessReduce)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it reduces damage done to {0}-type members by {1}-type attacks, based on how many are on the team.");
                if (includeEffects)
                {
                    string defendType = (string)args[1];
                    string holderType = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(holderType).Name);
                    localArgs.Add(DataManager.Instance.GetElement(defendType).Name);

                    SingleEmitter emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1));
                    BattleEvent reduceEvent1 = new MultiplyElementEvent(defendType, 2, 3, false, new BattleAnimEvent(emitter, "DUN_Screen_Hit", true, 10));
                    emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1));
                    BattleEvent reduceEvent2 = new MultiplyElementEvent(defendType, 1, 3, false, new BattleAnimEvent(emitter, "DUN_Screen_Hit", true, 10));
                    emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1));
                    BattleEvent immuneEvent = new MultiplyElementEvent(defendType, -1, 1, true, new BattleAnimEvent(emitter, "DUN_Screen_Hit", true, 10));
                    item.BeforeBeingHits.Add(0, new TeamReduceEvent(holderType, reduceEvent1, reduceEvent2, immuneEvent, null));


                    //item.ProximityEvent.Radius = 0;
                    //item.ProximityEvent.TargetAlignments = (Alignment.Friend | Alignment.Foe);
                    //BattleEvent isolateEvent = new IsolateElementEvent(defendType);
                    BattleEvent absorbEvent = new AbsorbElementEvent(defendType, false, true,
                        new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1)), "DUN_Drink",
                        new RestoreHPEvent(1, 4, true));

                    //item.ProximityEvent.BeforeExplosions.Add(0, new TeamReduceEvent(holderType, null, null, null, isolateEvent));
                    item.BeforeBeingHits.Add(5, new TeamReduceEvent(holderType, null, null, null, absorbEvent));
                }
            }
            else if (type == ExclusiveItemEffect.TypeBulldozer)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it allows {0}-type moves to hit {1}-type Pokémon.");
                if (includeEffects)
                {
                    string attackType = (string)args[0];
                    string defendType = (string)args[1];
                    localArgs.Add(DataManager.Instance.GetElement(attackType).Name);
                    localArgs.Add(DataManager.Instance.GetElement(defendType).Name);

                    item.UserElementEffects.Add(0, new FamilyMatchupEvent(new LessImmunityEvent(attackType, defendType)));
                }
            }
            else if (type == ExclusiveItemEffect.SuperCrit)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, it raises the critical-hit ratio of super-effective hits.");
                if (includeEffects)
                {
                    item.BeforeHittings.Add(0, new FamilyBattleEvent(new CritEffectiveEvent(false, 1)));
                }
            }
            else if (type == ExclusiveItemEffect.NVECrit)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, it raises the critical-hit ratio of not-very-effective hits.");
                if (includeEffects)
                {
                    item.BeforeHittings.Add(0, new FamilyBattleEvent(new CritEffectiveEvent(true, 1)));
                }
            }
            else if (type == ExclusiveItemEffect.GapProber)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokemon's moves and items that hit in a straight line cannot damage allies.");
                if (includeEffects)
                {
                    item.OnActions.Add(1, new FamilyBattleEvent(new GapProberEvent()));
                }
            }
            else if (type == ExclusiveItemEffect.PassThroughAttacker)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokemon's moves and items that hit in a straight line will pass through walls.");
                if (includeEffects)
                {
                    item.OnActions.Add(1, new FamilyBattleEvent(new PierceEvent(true, true, false, true)));
                }
            }
            else if (type == ExclusiveItemEffect.Nontraitor)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon cannot damage allies with its moves.");
                if (includeEffects)
                {
                    item.OnActions.Add(1, new FamilyBattleEvent(new OnMoveUseEvent(new NontraitorEvent())));
                }
            }
            else if (type == ExclusiveItemEffect.WaterTerrain)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon can traverse water.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new AddMobilityEvent(TerrainData.Mobility.Water)));
                }
            }
            else if (type == ExclusiveItemEffect.LavaTerrain)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon can traverse lava without being burned.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new AddMobilityEvent(TerrainData.Mobility.Lava)));
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new LavaState())));
                }
            }
            else if (type == ExclusiveItemEffect.AllTerrain)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon can traverse water, lava, and pits.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new AddMobilityEvent(TerrainData.Mobility.Water)));
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new AddMobilityEvent(TerrainData.Mobility.Abyss)));
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new AddMobilityEvent(TerrainData.Mobility.Lava)));
                }
            }
            else if (type == ExclusiveItemEffect.Wallbreaker)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, some of the Pokémon's moves will break walls.");
                if (includeEffects)
                {
                    item.OnActions.Add(1, new FamilyBattleEvent(new MeleeHitTilesEvent(TileAlignment.Wall)));
                    SingleEmitter terrainEmitter = new SingleEmitter(new AnimData("Wall_Break", 2));
                    item.OnHitTiles.Add(0, new FamilyBattleEvent(new OnMoveUseEvent(new OnMeleeActionEvent(false, new RemoveTerrainStateEvent("DUN_Rollout", terrainEmitter, new FlagType(typeof(WallTerrainState)))))));
                }
            }
            else if (type == ExclusiveItemEffect.GapFiller)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, some of the Pokémon's moves will fill water, lava, and pits.");
                if (includeEffects)
                {
                    item.OnHitTiles.Add(0, new FamilyBattleEvent(new OnMoveUseEvent(new OnMeleeActionEvent(true, new RemoveTerrainStateEvent("", new EmptyFiniteEmitter(), new FlagType(typeof(WaterTerrainState)), new FlagType(typeof(LavaTerrainState)), new FlagType(typeof(AbyssTerrainState)))))));
                }
            }
            else if (type == ExclusiveItemEffect.PPBoost)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon gets an extra {0}PP to all of its moves.");
                if (includeEffects)
                {
                    localArgs.Add(new LocalText(((int)args[0]).ToString()));

                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new AddChargeEvent((int)args[0])));
                }
            }
            else if (type == ExclusiveItemEffect.PPSaver)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's PP cannot be lowered by the moves and abilities of opposing Pokémon.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new PPSaverEvent()));
                }
            }
            else if (type == ExclusiveItemEffect.DeepBreather)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, it restores the PP of a move when the Pokémon reaches a new floor.");
                if (includeEffects)
                {
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new DeepBreathEvent(false)));
                }
            }
            else if (type == ExclusiveItemEffect.PracticeSwinger)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will do increased damage if its previous move missed.");
                if (includeEffects)
                {
                    item.BeforeHittings.Add(0, new FamilyBattleEvent(new MultWhenMissEvent("missed_all_last_turn", 4, 3)));
                }
            }
            else if (type == ExclusiveItemEffect.MisfortuneMirror)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will more easily dodge attacks if its previous move missed.");
                if (includeEffects)
                {
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new EvasiveWhenMissEvent("missed_all_last_turn")));
                }
            }
            else if (type == ExclusiveItemEffect.CounterBasher)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will counter damage from regular attacks and thrown items.");
                if (includeEffects)
                {
                    SingleEmitter emitter = new SingleEmitter(new BeamAnimData("Column_Blue", 3, -1, -1, 192));
                    SingleEmitter targetEmitter = new SingleEmitter(new AnimData("Hit_Neutral", 3));
                    item.AfterBeingHits.Add(0, new FamilyBattleEvent(new CounterNonSkillEvent(1, 1, new BattleAnimEvent(emitter, "DUN_Light_Screen", true, 10), new BattleAnimEvent(targetEmitter, "DUN_Hit_Neutral", false))));
                }
            }
            else if (type == ExclusiveItemEffect.Gratitude)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon gives back recovered HP when it is healed by another Pokémon.");
                if (includeEffects)
                {
                    SingleEmitter emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_Out", 2));
                    item.AfterBeingHits.Add(0, new FamilyBattleEvent(new CounterHealEvent(1, 2, new BattleAnimEvent(emitter, "DUN_Growth", true, 10))));
                }
            }
            else if (type == ExclusiveItemEffect.DoubleAttacker)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, it allows the Pokémon's regular attack to strike twice.");
                if (includeEffects)
                {
                    item.BeforeActions.Add(-1, new FamilyBattleEvent(new RegularAttackNeededEvent(new MultiStrikeEvent(2, false))));
                }
            }
            else if (type == ExclusiveItemEffect.DoubleDash)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, it changes the Pokémon's lunging moves into two weaker strikes.");
                if (includeEffects)
                {
                    item.BeforeActions.Add(-1, new FamilyBattleEvent(new OnDashActionEvent(new MultiStrikeEvent(2, true))));
                }
            }
            else if (type == ExclusiveItemEffect.StatusSplash)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's stat-changing moves are spread to allies.");
                if (includeEffects)
                {
                    ExplosionData explosion = new ExplosionData();
                    explosion.Range = 1;
                    explosion.Speed = 10;
                    explosion.TargetAlignments = (Alignment.Self | Alignment.Friend);

                    explosion.ExplodeFX.Emitter = new SingleEmitter(new AnimData("Circle_White_Out", 3));

                    //CircleSquareReleaseEmitter emitter = new CircleSquareReleaseEmitter(new AnimData("Event_Gather_Sparkle", 5));
                    //emitter.BurstTime = 6;
                    //emitter.ParticlesPerBurst = 4;
                    //emitter.Bursts = 5;
                    //emitter.StartDistance = 4;
                    //explosion.Emitter = emitter;

                    string[] statuses = { "mod_speed", "mod_attack", "mod_defense", "mod_special_attack", "mod_special_defense", "mod_accuracy", "mod_evasion" };

                    item.BeforeActions.Add(-3, new FamilyBattleEvent(new OnSelfActionEvent(new GiveStatusNeededEvent(statuses, new ChangeExplosionEvent(explosion)))));
                }
            }
            else if (type == ExclusiveItemEffect.MasterHurler)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, items thrown by the Pokémon will pierce through enemies.");
                if (includeEffects)
                {
                    item.OnActions.Add(0, new FamilyBattleEvent(new PierceEvent(false, true, true, false)));
                }
            }
            else if (type == ExclusiveItemEffect.AllyReviver)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will revive a fallen ally when it reaches a new floor.");
                if (includeEffects)
                {
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new AllyReviverEvent()));
                }
            }
            else if (type == ExclusiveItemEffect.AllyReviverBattle)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will revive a fallen ally when it defeats an opponent.");
                if (includeEffects)
                {
                    item.AfterActions.Add(0, new FamilyBattleEvent(new KnockOutNeededEvent(new BattlelessEvent(false, new AllyReviverEvent()))));
                }
            }
            else if (type == ExclusiveItemEffect.PressurePlus)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will raise the PP usage of opponents that attack it.");
                if (includeEffects)
                {
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new AddContextStateEvent(new PressurePlus())));
                    item.AfterBeingHits.Add(0, new FamilyBattleEvent(new PressureEvent(0)));
                }
            }
            else if (type == ExclusiveItemEffect.StatusOnAttack)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attacks and thrown items have a chance to inflict the {0} status.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[0]).Name);

                    item.AfterHittings.Add(0, new FamilyBattleEvent(new RegularAttackNeededEvent(new OnHitEvent(true, false, 35, new StatusBattleEvent((string)args[0], true, true, false, false, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                    item.AfterHittings.Add(0, new FamilyBattleEvent(new ThrownItemNeededEvent(new OnHitEvent(true, false, 35, new StatusBattleEvent((string)args[0], true, true, false, false, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.DevolveOnAttack)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attacks and thrown items have a chance to devolve the target.");
                if (includeEffects)
                {
                    item.AfterHittings.Add(0, new FamilyBattleEvent(new RegularAttackNeededEvent(new OnHitEvent(true, false, 35, new DevolveEvent(true, "transformed", 5, new BattleAnimEvent(new SingleEmitter(new AnimData("Puff_Green", 3)), "DUN_Transform", true))))));
                    item.AfterHittings.Add(0, new FamilyBattleEvent(new ThrownItemNeededEvent(new OnHitEvent(true, false, 35, new DevolveEvent(true, "transformed", 5, new BattleAnimEvent(new SingleEmitter(new AnimData("Puff_Green", 3)), "DUN_Transform", true))))));
                }
            }
            else if (type == ExclusiveItemEffect.KnockbackOnAttack)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attack will knock opponents back.");
                if (includeEffects)
                {
                    item.AfterHittings.Add(0, new FamilyBattleEvent(new RegularAttackNeededEvent(new OnHitEvent(true, false, 100, new KnockBackEvent(1)))));
                }
            }
            else if (type == ExclusiveItemEffect.ThrowOnAttack)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attack will throw opponents back.");
                if (includeEffects)
                {
                    BattleData altData = new BattleData();
                    altData.Element = "none";
                    altData.Category = BattleData.SkillCategory.Physical;
                    altData.SkillStates.Set(new ContactState());
                    altData.HitRate = 90;
                    altData.SkillStates.Set(new BasePowerState(30));
                    altData.OnHits.Add(0, new ThrowBackEvent(2, new DamageFormulaEvent()));

                    item.OnActions.Add(-3, new FamilyBattleEvent(new RegularAttackNeededEvent(new ChangeDataEvent(altData))));
                }
            }
            else if (type == ExclusiveItemEffect.SureHitAttacker)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attack cannot miss.");
                if (includeEffects)
                {
                    item.OnActions.Add(0, new FamilyBattleEvent(new RegularAttackNeededEvent(new SetAccuracyEvent(-1))));
                }
            }
            else if (type == ExclusiveItemEffect.SpecialAttacker)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the category of the Pokémon's regular attack is changed from physical to special.");
                if (includeEffects)
                {
                    item.BeforeActions.Add(-5, new FamilyBattleEvent(new RegularAttackNeededEvent(new FlipCategoryEvent(false))));
                }
            }
            else if (type == ExclusiveItemEffect.TypeBecomesCategory)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {0}-type moves are changed to become {1}.");
                if (includeEffects)
                {
                    string element = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(element).Name);
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[1];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));

                    if (category == BattleData.SkillCategory.Physical)
                        category = BattleData.SkillCategory.Magical;
                    else if (category == BattleData.SkillCategory.Magical)
                        category = BattleData.SkillCategory.Physical;
                    item.BeforeActions.Add(-5, new FamilyBattleEvent(new ElementNeededEvent(element, new CategoryNeededEvent(category, new FlipCategoryEvent(false)))));
                }
            }
            else if (type == ExclusiveItemEffect.StatusOnCategoryHit)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {1} moves inflict the {0} status.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[0]).Name);
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[1];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));

                    item.AfterHittings.Add(0, new FamilyBattleEvent(new CategoryNeededEvent(category, new OnHitEvent(false, false, 100, new StatusBattleEvent((string)args[0], true, true, false, false, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.ChanceStatusOnCategoryHit)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {1} moves may inflict the {0} status.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[0]).Name);
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[1];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));
                    int chance = (int)args[2];

                    item.AfterHittings.Add(0, new FamilyBattleEvent(new CategoryNeededEvent(category, new OnHitEvent(false, false, chance, new StatusBattleEvent((string)args[0], true, true, false, false, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.StatusOnTypeHit)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {0}-type moves inflict the {1} status.");
                if (includeEffects)
                {
                    string element = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(element).Name);
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[1]).Name);

                    item.AfterHittings.Add(0, new FamilyBattleEvent(new ElementNeededEvent(element, new OnHitEvent(false, false, 100, new StatusBattleEvent((string)args[1], true, true, false, false, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.ChanceStatusOnTypeHit)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {0}-type moves have a chance to inflict the {1} status.");
                if (includeEffects)
                {
                    string element = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(element).Name);
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[1]).Name);
                    int chance = (int)args[2];

                    item.AfterHittings.Add(0, new FamilyBattleEvent(new ElementNeededEvent(element, new OnHitEvent(false, false, chance, new StatusBattleEvent((string)args[1], true, true, false, false, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.ChanceStatOnTypeHit)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {0}-type moves may lower the target's {1}.");
                if (includeEffects)
                {
                    string element = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(element).Name);
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[1]).Name);
                    int chance = (int)args[2];

                    item.AfterHittings.Add(0, new FamilyBattleEvent(new ElementNeededEvent(element, new OnHitEvent(false, false, chance, new StatusStackBattleEvent((string)args[1], true, true, false, false, -1, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.StatusOnCategoryUse)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon gains the {0} status after using a {1} move.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[0]).Name);
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[1];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));

                    item.AfterActions.Add(0, new FamilyBattleEvent(new CategoryNeededEvent(category, new StatusBattleEvent((string)args[0], false, true, true, false, new StringKey("MSG_EXCL_ITEM_TYPE")))));
                }
            }
            else if (type == ExclusiveItemEffect.StatOnCategoryUse)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {1} moves may increase the user's {0}.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[0]).Name);
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[1];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));
                    int chance = (int)args[2];

                    item.AfterActions.Add(0, new FamilyBattleEvent(new CategoryNeededEvent(category, new OnHitAnyEvent(false, chance, new StatusStackBattleEvent((string)args[0], false, true, true, false, 1, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.MapStatusOnCategoryUse)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon changes the floor to {1} after using a {0} move.");
                if (includeEffects)
                {
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[0];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));
                    MapStatusData mapStatus = DataManager.Instance.GetMapStatus((string)args[1]);
                    localArgs.Add(mapStatus.Name);

                    if (mapStatus.StatusStates.Contains<MapWeatherState>())
                        item.AfterActions.Add(0, new FamilyBattleEvent(new CategoryNeededEvent(category, new GiveMapStatusEvent((string)args[1], 10, new StringKey(), typeof(ExtendWeatherState)))));
                    else
                        item.AfterActions.Add(0, new FamilyBattleEvent(new CategoryNeededEvent(category, new GiveMapStatusEvent((string)args[1], 10))));
                }
            }
            else if (type == ExclusiveItemEffect.StatusImmune)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon becomes immune to the {0} status.");
                if (includeEffects)
                {
                    string status = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetStatus(status).Name);

                    item.BeforeStatusAdds.Add(0, new FamilyStatusEvent(new PreventStatusCheck(status, new StringKey("MSG_PROTECT_WITH"))));
                    if (status == "poison")//poison and bad poison
                        item.BeforeStatusAdds.Add(0, new FamilyStatusEvent(new PreventStatusCheck("poison_toxic", new StringKey("MSG_PROTECT_WITH"))));
                    if (status == "sleep")//asleep and yawn
                        item.BeforeStatusAdds.Add(0, new FamilyStatusEvent(new PreventStatusCheck("yawning", new StringKey("MSG_PROTECT_WITH"))));
                }
            }
            else if (type == ExclusiveItemEffect.StatDropImmune)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon cannot have its {0} lowered.");
                if (includeEffects)
                {
                    Stat[] boostedStats = (Stat[])args[0];
                    List<LocalText> nameList = new List<LocalText>();
                    foreach (Stat stat in boostedStats)
                        nameList.Add(ToLocalText(stat, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));

                    localArgs.Add(BuildLocalTextList(nameList, translate));

                    List<Stat> protectedStats = new List<Stat>();
                    protectedStats.AddRange(boostedStats);
                    item.BeforeStatusAdds.Add(0, new FamilyStatusEvent(new StatChangeCheck(protectedStats, new StringKey("MSG_STAT_DROP_PROTECT"), true, false, false)));
                }
            }
            else if (type == ExclusiveItemEffect.SleepWalker)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will be able to move when asleep.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(-1, new FamilyRefreshEvent(new MiscEvent(new SleepWalkerState())));
                }
            }
            else if (type == ExclusiveItemEffect.ChargeWalker)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will be able to move when charging attacks.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(-1, new FamilyRefreshEvent(new MiscEvent(new ChargeWalkerState())));
                }
            }
            else if (type == ExclusiveItemEffect.HitAndRun)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will not take damage from counter attacks.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(-1, new FamilyRefreshEvent(new MiscEvent(new HitAndRunState(itemID))));
                }
            }
            else if (type == ExclusiveItemEffect.StairSensor)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, it reveals the direction of the staircase whenever the Pokémon reaches a new floor.");
                if (includeEffects)
                {
                    SingleEmitter emitter = new SingleEmitter(new AnimData("Stair_Sensor_Arrow", 6), 6);
                    emitter.Layer = DrawLayer.Top;
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new StairSensorEvent("stairs_sensed", emitter)));
                }
            }
            else if (type == ExclusiveItemEffect.AcuteSniffer)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, it reveals the number of items laying on the ground whenever the Pokémon reaches a new floor.");
                if (includeEffects)
                {
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new AcuteSnifferEvent("items_sniffed", new AnimEvent(new SingleEmitter(new AnimData("Circle_Small_Blue_In", 2)), ""))));
                }
            }
            else if (type == ExclusiveItemEffect.MapSurveyor)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it partially reveals the floor's layout the Pokémon reaches a new floor.");
                if (includeEffects)
                {
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new MapSurveyorEvent("map_surveyed", 20)));
                }
            }
            else if (type == ExclusiveItemEffect.XRay)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it allows the Pokémon to see foes and items in heavy darkness.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new SetSightEvent(true, Map.SightRange.Clear)));
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new SeeItemsEvent(true)));
                }
            }
            else if (type == ExclusiveItemEffect.WeaknessPayback)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it inflicts the {1} status on the attacker when the Pokémon is hit with a {0}-type move.");
                if (includeEffects)
                {
                    string counterElement = (string)args[0];
                    string counterStatus = (string)args[1];
                    localArgs.Add(DataManager.Instance.GetElement(counterElement).Name);
                    localArgs.Add(DataManager.Instance.GetStatus(counterStatus).Name);

                    item.AfterBeingHits.Add(0, new FamilyBattleEvent(new ElementNeededEvent(counterElement, new HitCounterEvent(Alignment.Foe, false, false, false, 100, new StatusBattleEvent(counterStatus, false, true, false, false, new StringKey("MSG_EXCL_ITEM_TYPE"),
                        new BattleAnimEvent(new SingleEmitter(new AnimData("Circle_Small_Blue_Out", 2)), "DUN_Switcher", false, 0))))));
                }
            }
            else if (type == ExclusiveItemEffect.WeaknessReduce)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it reduces damage done by {0}-type attacks.");
                if (includeEffects)
                {
                    string counterElement = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(counterElement).Name);

                    SingleEmitter emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1));
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new MultiplyElementEvent(counterElement, 1, 2, false, new BattleAnimEvent(emitter, "DUN_Screen_Hit", true, 10))));
                }
            }
            else if (type == ExclusiveItemEffect.WarpPayback)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it warps the attackers away when the Pokémon is hit with a {0}-type move.");
                if (includeEffects)
                {
                    string counterElement = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(counterElement).Name);

                    item.AfterBeingHits.Add(0, new FamilyBattleEvent(new ElementNeededEvent(counterElement, new HitCounterEvent(Alignment.Foe, false, false, false, 100, new RandomWarpEvent(50, false, new StringKey("MSG_EXCL_ITEM_TYPE"))))));
                }
            }
            else if (type == ExclusiveItemEffect.LungeAttack)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it changes the Pokémon's regular attack into a dash.");
                if (includeEffects)
                {
                    DashAction altAction = new DashAction();
                    altAction.CharAnim = 05;//Attack
                    altAction.Range = 2;
                    altAction.StopAtWall = true;
                    altAction.StopAtHit = true;
                    altAction.HitTiles = true;
                    altAction.TargetAlignments = Alignment.Foe;

                    BattleFX preFX = new BattleFX();
                    preFX.Sound = "DUN_Attack";
                    altAction.PreActions.Add(preFX);

                    item.OnActions.Add(-3, new FamilyBattleEvent(new RegularAttackNeededEvent(new ChangeActionEvent(altAction))));
                    item.OnActions.Add(-3, new FamilyBattleEvent(new RegularAttackNeededEvent(new TraitorEvent())));
                }
            }
            else if (type == ExclusiveItemEffect.ProjectileAttack)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, it changes the Pokémon's regular attack into a short projectile.");
                if (includeEffects)
                {
                    ProjectileAction altAction = new ProjectileAction();
                    altAction.CharAnimData = new CharAnimFrameType(07);//shoot
                    altAction.Range = 2;
                    altAction.Speed = 10;
                    altAction.StopAtWall = true;
                    altAction.StopAtHit = true;
                    altAction.HitTiles = true;
                    altAction.Anim = new AnimData("Confuse_Ray", 2);
                    altAction.TargetAlignments = Alignment.Foe;

                    BattleFX preFX = new BattleFX();
                    preFX.Sound = "DUN_Attack";
                    altAction.PreActions.Add(preFX);

                    item.OnActions.Add(-3, new FamilyBattleEvent(new RegularAttackNeededEvent(new ChangeActionEvent(altAction))));
                    item.OnActions.Add(-3, new FamilyBattleEvent(new RegularAttackNeededEvent(new TraitorEvent())));
                }
            }
            else if (type == ExclusiveItemEffect.WideAttack)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attack will hit the front and sides.");
                if (includeEffects)
                {
                    AttackAction altAction = new AttackAction();
                    altAction.CharAnimData = new CharAnimFrameType(40);//Swing
                    altAction.HitTiles = true;
                    altAction.TargetAlignments = Alignment.Foe;

                    BattleFX preFX = new BattleFX();
                    preFX.Sound = "DUN_Attack";
                    altAction.PreActions.Add(preFX);

                    item.OnActions.Add(-3, new FamilyBattleEvent(new RegularAttackNeededEvent(new ChangeActionEvent(altAction))));
                }
            }
            else if (type == ExclusiveItemEffect.ExplosiveAttack)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attack explodes outwards with splash damage.");
                if (includeEffects)
                {
                    ExplosionData explosion = new ExplosionData();
                    explosion.Range = 1;
                    explosion.HitTiles = true;
                    explosion.Speed = 10;
                    explosion.ExplodeFX.Sound = "DUN_Blast_Seed";
                    explosion.TargetAlignments = (Alignment.Friend | Alignment.Foe);

                    CircleSquareAreaEmitter explosionEmitter = new CircleSquareAreaEmitter(new AnimData("Blast_Seed", 3));
                    explosionEmitter.ParticlesPerTile = 0.8;
                    explosion.Emitter = explosionEmitter;

                    item.OnActions.Add(-3, new FamilyBattleEvent(new RegularAttackNeededEvent(new ChangeExplosionEvent(explosion))));
                }
            }
            else if (type == ExclusiveItemEffect.TrapBuster)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's moves will destroy traps.");
                if (includeEffects)
                {
                    item.OnHitTiles.Add(0, new FamilyBattleEvent(new OnMoveUseEvent(new RemoveTrapEvent())));
                }
            }
            else if (type == ExclusiveItemEffect.ExplosionGuard)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will take reduced damage from explosions and splash damage.");
                if (includeEffects)
                {
                    SingleEmitter emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1));
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new BlastProofEvent(0, 1, 2, true, new BattleAnimEvent(emitter, "DUN_Screen_Hit", true, 10))));
                }
            }
            else if (type == ExclusiveItemEffect.WandMaster)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the effects of wands will fly in an arc when waved by the Pokémon.");
                if (includeEffects)
                {
                    ThrowAction altAction = new ThrowAction();
                    altAction.CharAnimData = new CharAnimFrameType(42);//Rotate
                    altAction.Coverage = ThrowAction.ArcCoverage.WideAngle;
                    altAction.Range = 10;
                    altAction.Speed = 12;
                    altAction.Anim = new AnimData("Confuse_Ray", 2);
                    altAction.TargetAlignments = Alignment.Foe;
                    BattleFX itemFX = new BattleFX();
                    itemFX.Sound = "DUN_Throw_Start";
                    altAction.PreActions.Add(itemFX);
                    altAction.ActionFX.Sound = "DUN_Blowback_Orb";

                    item.OnActions.Add(-3, new FamilyBattleEvent(new WandAttackNeededEvent(new List<string> { "wand_path", "wand_pounce" }, new ChangeActionEvent(altAction))));
                }
            }
            else if (type == ExclusiveItemEffect.WandSpread)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the effects of wands will spread out like a fan when waved by the Pokémon.");
                if (includeEffects)
                {
                    item.OnActions.Add(-3, new FamilyBattleEvent(new WandAttackNeededEvent(new List<string> { "wand_pounce" }, new SpreadProjectileEvent(ProjectileAction.RayCount.Three))));
                }
            }
            else if (type == ExclusiveItemEffect.MultiRayShot)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, items thrown by the Pokémon will spread out like a fan.");
                if (includeEffects)
                {
                    item.OnActions.Add(-2, new FamilyBattleEvent(new ThrowItemPreventDropEvent()));
                    item.OnActions.Add(-2, new FamilyBattleEvent(new ThrownItemNeededEvent(new SpreadProjectileEvent(ProjectileAction.RayCount.Three))));
                }
            }
            else if (type == ExclusiveItemEffect.RoyalVeil)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will gradually restore the HP of its allies when its own HP is full.");
                if (includeEffects)
                {
                    item.OnTurnEnds.Add(-3, new FamilySingleEvent(new RoyalVeilEvent(4, 16)));
                }
            }
            else if (type == ExclusiveItemEffect.Celebrate)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will be able to move again after defeating an enemy.");
                if (includeEffects)
                {
                    SingleEmitter emitter = new SingleEmitter(new AnimData("Circle_White_In", 1));
                    item.AfterActions.Add(1, new FamilyBattleEvent(new KnockOutNeededEvent(new PreserveTurnEvent(new StringKey("MSG_EXTRA_TURN"), new BattleAnimEvent(emitter, "DUN_Attack_Speed_Up", false, 20)))));
                }
            }
            else if (type == ExclusiveItemEffect.Absorption)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will regain HP after defeating an enemy.");
                if (includeEffects)
                {
                    item.AfterActions.Add(1, new FamilyBattleEvent(new KnockOutNeededEvent(new RestoreHPEvent(1, 8, false))));
                }
            }
            else if (type == ExclusiveItemEffect.ExcessiveForce)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will damage nearby enemies after defeating an enemy.");
                if (includeEffects)
                {
                    AreaAction altAction = new AreaAction();
                    altAction.HitTiles = true;
                    altAction.BurstTiles = TileAlignment.Any;
                    altAction.TargetAlignments = (Alignment.Self | Alignment.Friend | Alignment.Foe);
                    ExplosionData altExplosion = new ExplosionData();
                    altExplosion.TargetAlignments = Alignment.Friend;
                    altExplosion.Range = 1;
                    altExplosion.Speed = 10;
                    altExplosion.ExplodeFX.Sound = "DUN_Blast_Seed";
                    CircleSquareAreaEmitter emitter = new CircleSquareAreaEmitter(new AnimData("Blast_Seed", 3));
                    emitter.ParticlesPerTile = 0.8;
                    altExplosion.Emitter = emitter;
                    BattleData newData = new BattleData();
                    newData.Element = "none";
                    newData.HitRate = -1;
                    newData.OnHits.Add(-1, new MaxHPDamageEvent(6));

                    item.AfterHittings.Add(0, new FamilyBattleEvent(new TargetDeadNeededEvent(new InvokeCustomBattleEvent(altAction, altExplosion, newData, new StringKey("MSG_EXCESSIVE_FORCE"), true))));
                }
            }
            else if (type == ExclusiveItemEffect.CelebrateCure)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will recover from status effects after defeating an enemy.");
                if (includeEffects)
                {
                    BattleAnimEvent battleAnim = new BattleAnimEvent(new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1)), "DUN_Wonder_Tile", false, 10);
                    item.AfterActions.Add(1, new FamilyBattleEvent(new KnockOutNeededEvent(new RemoveStateStatusBattleEvent(typeof(BadStatusState), false, new StringKey("MSG_CURE_SELF"), battleAnim))));
                }
            }
            else if (type == ExclusiveItemEffect.CelebrateStatus)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon gains the {0} status after defeating an enemy.");
                if (includeEffects)
                {
                    localArgs.Add(DataManager.Instance.GetStatus((string)args[0]).Name);
                    item.AfterActions.Add(0, new FamilyBattleEvent(new KnockOutNeededEvent(new StatusBattleEvent((string)args[0], false, true, true, false, new StringKey("MSG_EXCL_ITEM_TYPE")))));
                }
            }
            else if (type == ExclusiveItemEffect.Anchor)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the Pokémon is prevented from being forced off its location.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new AnchorState())));
                }
            }
            else if (type == ExclusiveItemEffect.BarrageGuard)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon takes reduced damage from multiple attacks in a turn.");
                if (includeEffects)
                {
                    SingleEmitter emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1));
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new BarrageGuardEvent("was_hurt_last_turn", 2, 3, new BattleAnimEvent(emitter, "DUN_Screen_Hit", true, 10))));
                }
            }
            else if (type == ExclusiveItemEffect.BetterOdds)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's attacks never miss and always land a critical hit if all moves have the same PP.");
                if (includeEffects)
                {
                    item.BeforeHittings.Add(0, new FamilyBattleEvent(new BetterOddsEvent()));
                }
            }
            else if (type == ExclusiveItemEffect.ClutchPerformer)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon is more likely to evade attacks when at low HP.");
                if (includeEffects)
                {
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new EvasiveInPinchEvent()));
                }
            }
            else if (type == ExclusiveItemEffect.DistanceDodge)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon is more likely to evade attacks from far away.");
                if (includeEffects)
                {
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new EvasiveInDistanceEvent()));
                }
            }
            else if (type == ExclusiveItemEffect.CloseDodge)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon is more likely to evade attacks from close up.");
                if (includeEffects)
                {
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new EvasiveCloseUpEvent()));
                }
            }
            else if (type == ExclusiveItemEffect.WeaknessDodge)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon becomes extremely likely to avoid {0}-type attacks.");
                if (includeEffects)
                {
                    string counterElement = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(counterElement).Name);

                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new ElementNeededEvent(counterElement, new EvadeIfPossibleEvent())));
                }
            }
            else if (type == ExclusiveItemEffect.SweetDreams)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon gradually restores the HP of its teammates while they are asleep.");
                if (includeEffects)
                {
                    item.ProximityEvent.Radius = 3;
                    item.ProximityEvent.TargetAlignments = Alignment.Friend | Alignment.Self;
                    item.ProximityEvent.OnTurnEnds.Add(0, new FamilySingleEvent(new NightmareEvent("sleep", -4, new StringKey("MSG_HEAL_WITH_OTHER"), new AnimEvent(new SingleEmitter(new AnimData("Circle_Small_Blue_Out", 2)), "DUN_Healing_Wish_2", 0))));
                }
            }
            else if (type == ExclusiveItemEffect.FastHealer)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's natural HP-recovery speed is boosted.");
                if (includeEffects)
                {
                    item.ModifyHPs.Add(1, new FamilyHPEvent(new HealMultEvent(3, 2)));
                }
            }
            else if (type == ExclusiveItemEffect.SelfCurer)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon recovers faster from status problems.");
                if (includeEffects)
                {
                    item.BeforeStatusAdds.Add(0, new FamilyStatusEvent(new SelfCurerEvent(1, 2)));
                }
            }
            else if (type == ExclusiveItemEffect.StatusMirror)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, status problems inflicted on the Pokémon are passed to the Pokémon that caused it.");
                if (includeEffects)
                {
                    item.OnStatusAdds.Add(0, new FamilyStatusEvent(new StateStatusSyncEvent(typeof(BadStatusState), new StringKey("MSG_SYNCHRONIZE"), new StatusAnimEvent(new SingleEmitter(new AnimData("Circle_Small_Blue_Out", 2)), "DUN_Switcher", 30))));
                }
            }
            else if (type == ExclusiveItemEffect.StatMirror)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, stat changes inflicted on the Pokémon are passed to the Pokémon that caused it.");
                if (includeEffects)
                {
                    item.OnStatusAdds.Add(0, new FamilyStatusEvent(new StatDropSyncEvent(new StringKey("MSG_STAT_DROP_TRIGGER"), new StatusAnimEvent(new SingleEmitter(new AnimData("Circle_Small_Blue_Out", 2)), "DUN_Switcher", 30))));
                }
            }
            else if (type == ExclusiveItemEffect.ErraticAttacker)
            {
                item.Rarity = 2;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's moves are more intensely affected by type match-ups.");
                if (includeEffects)
                {
                    item.BeforeHittings.Add(0, new FamilyBattleEvent(new MultiplyEffectiveEvent(false, 6, 5)));
                    item.BeforeHittings.Add(0, new FamilyBattleEvent(new MultiplyEffectiveEvent(true, 4, 5)));
                }
            }
            else if (type == ExclusiveItemEffect.ErraticDefender)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, moves used on the Pokémon are more intensely affected by type match-ups.");
                if (includeEffects)
                {
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new MultiplyEffectiveEvent(false, 6, 5)));
                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new MultiplyEffectiveEvent(true, 4, 5)));
                }
            }
            else if (type == ExclusiveItemEffect.FastFriend)
            {
                item.Rarity = 1;
                item.Desc = new LocalText("When kept in the bag, the Pokémon has a higher chance of recruiting wild Pokémon.");
                if (includeEffects)
                {
                    item.OnActions.Add(0, new FamilyBattleEvent(new FlatRecruitmentEvent(10)));
                }
            }
            else if (type == ExclusiveItemEffect.CoinWatcher)
            {
                item.Rarity = 3;
                item.Name.DefaultText = item.Name.DefaultText;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will find more \\uE024 when exploring in dungeons.");
                item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new CoinModGenState(50))));
            }
            else if (type == ExclusiveItemEffect.HiddenStairFinder)
            {
                item.Rarity = 3;
                item.Name.DefaultText = "**" + item.Name.DefaultText;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will find more Hidden Stairs when exploring in dungeons.");
                item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new StairsModGenState(10))));
            }
            else if (type == ExclusiveItemEffect.ChestFinder)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will find more Treasure Chests when exploring in dungeons.");
                item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new ChestModGenState(10))));
            }
            else if (type == ExclusiveItemEffect.ShopFinder)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will find more shops when exploring in dungeons.");
                item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new ShopModGenState(10))));
            }
            else if (type == ExclusiveItemEffect.SecondSTAB)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's {0}-type moves are boosted.");
                if (includeEffects)
                {
                    string element = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(element).Name);

                    item.OnActions.Add(0, new FamilyBattleEvent(new MultiplyElementEvent(element, 4, 3, false)));
                }
            }
            else if (type == ExclusiveItemEffect.TypedAttack)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's regular attacks change to match its type.");
                if (includeEffects)
                {
                    item.OnActions.Add(-1, new FamilyBattleEvent(new RegularAttackNeededEvent(new MatchAttackToTypeEvent())));
                }
            }
            else if (type == ExclusiveItemEffect.WeatherProtection)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon is protected from weather damage.");
                if (includeEffects)
                {
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new SandState())));
                    item.OnRefresh.Add(0, new FamilyRefreshEvent(new MiscEvent(new HailState())));
                }
            }
            else if (type == ExclusiveItemEffect.RemoveAbilityAttack)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, it causes the Pokémon's regular attack to remove the target's ability.");
                if (includeEffects)
                {
                    item.AfterHittings.Add(0, new FamilyBattleEvent(new RegularAttackNeededEvent(new ChangeToAbilityEvent(DataManager.Instance.DefaultIntrinsic, true, true))));
                }
            }
            else if (type == ExclusiveItemEffect.CheekPouch)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's HP is restored when it eats a food item.");
                if (includeEffects)
                {
                    item.AfterBeingHits.Add(0, new FamilyBattleEvent(new FoodNeededEvent(new RestoreHPEvent(1, 3, true))));
                }
            }
            else if (type == ExclusiveItemEffect.HealInWater)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon recovers HP when in water.");
                if (includeEffects)
                {
                    item.OnTurnEnds.Add(0, new FamilySingleEvent(new TerrainNeededEvent("water", new FractionHealEvent(8, new StringKey("MSG_HEAL_WITH")))));
                }
            }
            else if (type == ExclusiveItemEffect.HealInLava)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon recovers HP when in lava.");
                if (includeEffects)
                {
                    item.OnTurnEnds.Add(0, new FamilySingleEvent(new TerrainNeededEvent("lava", new FractionHealEvent(8, new StringKey("MSG_HEAL_WITH")))));
                }
            }
            else if (type == ExclusiveItemEffect.HealOnNewFloor)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon is fully healed upon entering a new floor.");
                if (includeEffects)
                {
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new FractionHealEvent(1, new StringKey("MSG_HEAL_WITH"))));
                }
            }
            else if (type == ExclusiveItemEffect.EndureCategory)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it prevents the Pokémon from fainting to {0} moves, leaving it with 1 HP.");
                if (includeEffects)
                {
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[0];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));

                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new OnMoveUseEvent(new EndureCategoryEvent(category))));
                }
            }
            else if (type == ExclusiveItemEffect.EndureType)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it prevents the Pokémon from fainting to {0}-type moves, leaving it with 1 HP.");
                if (includeEffects)
                {
                    string element = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(element).Name);

                    item.BeforeBeingHits.Add(0, new FamilyBattleEvent(new OnMoveUseEvent(new EndureElementEvent(element))));
                }
            }
            else if (type == ExclusiveItemEffect.SpikeDropper)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, the Pokémon scatters Spikes when hit by a {0} attack.");
                if (includeEffects)
                {
                    BattleData.SkillCategory category = (BattleData.SkillCategory)args[0];
                    localArgs.Add(ToLocalText(category, translate ? itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type] : item.Desc, translate));

                    item.AfterBeingHits.Add(0, new FamilyBattleEvent(new CategoryNeededEvent(category, new CounterTrapEvent("trap_spikes", new SingleEmitter(new AnimData("Puff_Brown", 3)), "DUN_Substitute"))));
                }
            }
            else if (type == ExclusiveItemEffect.NoStatusInWeather)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon is protected from status conditions when the floor has the {0} status.");
                if (includeEffects)
                {
                    string mapStatus = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetMapStatus(mapStatus).Name);

                    SingleEmitter emitter = new SingleEmitter(new AnimData("Circle_Small_Blue_In", 1));
                    item.BeforeStatusAdds.Add(0, new FamilyStatusEvent(new WeatherNeededStatusEvent(mapStatus, new StateStatusCheck(typeof(BadStatusState), new StringKey("MSG_PROTECT_STATUS"), new StatusAnimEvent(emitter, "DUN_Screen_Hit", 10)))));
                }
            }
            else if (type == ExclusiveItemEffect.AttackRangeInWeather)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, the Pokémon's Attack Range is increased when the floor has the {0} status.");
                if (includeEffects)
                {
                    string mapStatus = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetMapStatus(mapStatus).Name);

                    item.OnActions.Add(-1, new FamilyBattleEvent(new WeatherAddRangeEvent(mapStatus, 1)));
                }
            }
            else if (type == ExclusiveItemEffect.DeepBreatherPlus)
            {
                item.Rarity = 4;
                item.Desc = new LocalText("When kept in the bag, it restores the PP of all moves when the Pokémon reaches a new floor.");
                if (includeEffects)
                {
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new DeepBreathEvent(true)));
                }
            }
            else if (type == ExclusiveItemEffect.MetronomePlus)
            {
                item.Rarity = 5;
                item.Desc = new LocalText("When kept in the bag, it increases the Pokémon's luck when using Metronome.");
                if (includeEffects)
                {
                    BattleData altData = new BattleData();
                    altData.Element = "normal";
                    altData.Category = BattleData.SkillCategory.Status;
                    altData.HitRate = -1;
                    altData.OnHits.Add(0, new NeededMoveEvent());

                    item.OnActions.Add(-3, new FamilyBattleEvent(new SpecificSkillNeededEvent(new ChangeDataEvent(altData), "metronome")));
                }
            }
            else if (type == ExclusiveItemEffect.TypeBodyguard)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon will step in to take {0}-type attacks for nearby allies.");
                if (includeEffects)
                {
                    string element = (string)args[0];
                    localArgs.Add(DataManager.Instance.GetElement(element).Name);

                    item.ProximityEvent.Radius = 1;
                    item.ProximityEvent.TargetAlignments = Alignment.Foe;
                    item.ProximityEvent.BeforeExplosions.Add(0, new DrawAttackEvent(Alignment.Friend, element, new StringKey("MSG_DRAW_ATTACK")));
                }
            }
            else if (type == ExclusiveItemEffect.HeartFinder)
            {
                item.Rarity = 3;
                item.Desc = new LocalText("When kept in the bag, the Pokémon may find a {0} when it enters a new floor.");
                if (includeEffects)
                {
                    
                    string element = "loot_heart_scale";
                    localArgs.Add(DataManager.Instance.GetItem(element).Name);
                    item.OnMapStarts.Add(0, new FamilySingleEvent(new GatherEvent(new List<string> { element }, 50, new AnimEvent(new SingleEmitter(new AnimData("Circle_Small_Blue_In", 2)), ""))));
                }
            }
            else
            {
                item.Name.DefaultText = "**" + item.Name.DefaultText;
            }

            if (translate)
            {
                item.Desc = itemEffectRows[typeof(ExclusiveItemEffect).Name + "." + type];
            }
        }


        public static string FillExclusiveTestData(ItemData item, string sprite, ExclusiveItemEffect effect, object[] effectArgs, bool translate)
        {
            string fileName = "xcl_test_" + Text.Sanitize(effect.ToString()).ToLower();

            AutoItemInfo.FillSpecificExclusiveData(fileName, item, "", effect.ToString() + " Test", ExclusiveItemType.None, effect, effectArgs, "bulbasaur", translate);
            item.Name.DefaultText = "**" + item.Name.DefaultText;

            return fileName;
        }

        public static void FillSpecificExclusiveData(string item_idx, ItemData item, string sprite, string name, ExclusiveItemType type, ExclusiveItemEffect effect, object[] effectArgs, string dexFamily, bool translate)
        {
            List<string> dexNums = new List<string>();
            FindFullFamily(dexNums, dexFamily);


            FillExclusiveData(item_idx, item, sprite, name, type, effect, effectArgs, dexFamily, dexNums, translate, true);
        }

        public static bool FindFullFamily(List<string> dexNums, string firstStage)
        {
            MonsterData data = DataManager.Instance.GetMonster(firstStage);
            string prevo = data.PromoteFrom;
            while (!String.IsNullOrEmpty(prevo))
            {
                firstStage = prevo;
                data = DataManager.Instance.GetMonster(firstStage);
                prevo = data.PromoteFrom;
            }
            if (data.Released)
            {
                dexNums.Add(firstStage.ToString());
                return FindEvos(dexNums, data);
            }
            return false;
        }

        public static bool FindEvos(List<string> dexNums, MonsterData data)
        {
            HashSet<string> uniquePromotions = new HashSet<string>();
            bool branched = false;
            foreach (PromoteBranch evo in data.Promotions)
            {
                uniquePromotions.Add(evo.Result);
                MonsterData evoData = DataManager.Instance.GetMonster(evo.Result);
                if (!evoData.Released)
                    continue;
                if (!dexNums.Contains(evo.Result))
                    dexNums.Add(evo.Result);
                branched |= FindEvos(dexNums, evoData);
            }
            return branched | (uniquePromotions.Count > 1);
        }

        public static void FillExclusiveData(string item_idx, ItemData item, string sprite, string name, ExclusiveItemType type, ExclusiveItemEffect effect, object[] effectArgs, string primaryDexNum, List<string> dexNums, bool translate, bool family)
        {
            item.Sprite = sprite;

            LocalText localName = null;
            if (type != ExclusiveItemType.None)
            {
                if (name != "")
                {
                    localName = GetLocalExpression(type, translate);
                    localName = LocalText.FormatLocalText(localName, new LocalText(name));
                }
                else
                {
                    MonsterData data = DataManager.Instance.GetMonster(primaryDexNum);
                    localName = GetLocalExpression(type, translate);
                    localName = LocalText.FormatLocalText(localName, data.Name);
                }
            }
            else
            {
                //custom name; leave as is.
                //when printing out to string table, the custom names are detected using exclusive item type checking

                //if translating, draw from translation list (should contain the original name as the default text)
                if (translate && name != "")
                    localName = specificItemRows[item.SortCategory.ToString("D4") + "-" + item_idx + "-" + 0.ToString("D4") + "|data.Name"];
                else
                    localName = new LocalText(name);
            }
            item.Name = localName;
            item.ItemStates.Set(new ExclusiveState(type));
            item.ItemStates.Set(new FamilyState(dexNums.ToArray()));

            LocalText monsterNames = null;
            if (family)
                monsterNames = DataManager.Instance.GetMonster(primaryDexNum).Name;
            else
            {
                List<LocalText> nameList = new List<LocalText>();
                foreach (string dexNum in dexNums)
                {
                    MonsterData data = DataManager.Instance.GetMonster(dexNum);
                    nameList.Add(data.Name);
                }
                monsterNames = BuildLocalTextList(nameList, translate);
            }

            LocalText reqStatement = GetExclusiveDescription(family, translate);

            List<LocalText> localArgs = new List<LocalText>();
            FillExclusiveEffects(item_idx, item, localArgs, true, effect, effectArgs, translate);
            item.Desc = LocalText.FormatLocalText(item.Desc, localArgs.ToArray());

            item.Desc = LocalText.FormatLocalText(reqStatement, monsterNames, item.Desc);
        }


        public static void FillExclusiveTypeData(string item_idx, ItemData item, string name, ExclusiveItemEffect effect, object[] effectArgs, string element, bool translate)
        {
            //set name
            if (translate)
                item.Name = specificItemRows[item.SortCategory.ToString("D4") + "-" + item_idx + "-" + 0.ToString("D4") + "|data.Name"];
            else
                item.Name = new LocalText(name);
            
            item.ItemStates.Set(new ExclusiveState());

            //desc: Item for a type
            //An exclusive item for {0}-type Pokémon.
            LocalText reqStatement = GetExclusiveTypeDescription(translate);
            ElementData elementData = DataManager.Instance.GetElement(element);

            List<object> newArgs = new List<object>();
            newArgs.Add(element);
            newArgs.AddRange(effectArgs);

            List<LocalText> localArgs = new List<LocalText>();
            FillExclusiveEffects(item_idx, item, localArgs, true, effect, newArgs.ToArray(), translate);
            item.Rarity = (item.Rarity - 1) / 2 + 1;
            item.Desc = LocalText.FormatLocalText(item.Desc, localArgs.ToArray());

            item.Desc = LocalText.FormatLocalText(reqStatement, elementData.Name, item.Desc);
        }


        public static LocalText GetExclusiveDescription(bool family, bool translate)
        {
            if (translate)
            {
                if (family)
                    return specialRows["exclFormatDescFamily"];
                else
                    return specialRows["exclFormatDesc"];
            }
            else
            {
                if (family)
                    return new LocalText("A rare treasure for the {0} family. {1}");
                else
                    return new LocalText("A rare treasure for {0}. {1}");
            }
        }

        public static LocalText GetExclusiveTypeDescription(bool translate)
        {
            if (translate)
                return specialRows["exclFormatDescType"];
            else
                return new LocalText("A rare treasure for {0}-type Pokémon. {1}");
        }



        public static LocalText BuildLocalTextList(List<LocalText> localTexts, bool translate)
        {
            List<string> defaultList = new List<string>();
            Dictionary<string, List<string>> langList = new Dictionary<string, List<string>>();

            foreach (LocalText name in localTexts)
            {
                defaultList.Add(name.DefaultText);
                foreach (string key in name.LocalTexts.Keys)
                {
                    if (!langList.ContainsKey(key))
                        langList[key] = new List<string>();
                    langList[key].Add(name.LocalTexts[key]);
                }
            }

            if (translate)
            {
                LocalText localList = new LocalText(BuildList(defaultList.ToArray(), ""));
                foreach (string key in langList.Keys)
                    localList.LocalTexts[key] = BuildList(langList[key].ToArray(), key);
                return localList;
            }
            else
                return new LocalText(Text.BuildList(defaultList.ToArray()));
        }

        public static LocalText ToLocalText<T>(T value, LocalText parent, bool translate) where T : Enum
        {
            if (translate)
            {
                LocalText typeNames = new LocalText(ToCulture(value, ""));
                foreach (string lang in parent.LocalTexts.Keys)
                    typeNames.LocalTexts[lang] = ToCulture(value, lang);
                return typeNames;
            }
            else
                return new LocalText(value.ToLocal());
        }


        private static string ToCulture<T>(T value, string code) where T : Enum
        {
            string key = "_ENUM_" + typeof(T).Name + "_" + value;
            string text;
            if (!stringsAll[code].TryGetValue(key, out text))
                text = stringsAll[""][key];
            if (!String.IsNullOrEmpty(text))
                return text;
            return value.ToString();
        }


        private static string BuildList(string[] input, string code)
        {
            StringBuilder totalString = new StringBuilder();
            for (int ii = 0; ii < input.Length; ii++)
            {
                if (ii > 0)
                {
                    string key = "ADD_SEPARATOR";
                    if (ii == input.Length - 1)
                        key = "ADD_END";
                    string text;
                    if (!stringsAll[code].TryGetValue(key, out text))
                        text = stringsAll[""][key];
                    totalString.Append(text);
                }
                totalString.Append(input[ii]);
            }
            return totalString.ToString();
        }
    }
}
