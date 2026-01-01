using DataGenerator.Data;
using RogueEssence;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using PMDC.Dungeon;
using PMDC.Data;
using Xunit;
using static DataGenerator.Data.CharAnim;

namespace DataGenerator.Tests;

/// <summary>
/// Tests for the SkillBuilder fluent API to ensure it produces
/// SkillData equivalent to the traditional imperative approach.
/// </summary>
public class SkillBuilderTests
{
    /// <summary>
    /// Static constructor to initialize DataManager before any tests run.
    /// DataManager.Instance is required for creating SkillData objects.
    /// </summary>
    static SkillBuilderTests()
    {
        // Initialize DataManager singleton if not already initialized
        if (DataManager.Instance == null)
        {
            DataManager.InitInstance();
            // Set default element to "none" - matches typical game setup
            DataManager.Instance.DefaultElement = "none";
        }
    }

    #region Factory Method Tests

    [Fact]
    public void Physical_SetsCorrectCategory()
    {
        var skill = SkillBuilder.Physical("Test Move").Build();

        Assert.Equal(BattleData.SkillCategory.Physical, skill.Data.Category);
    }

    [Fact]
    public void Physical_AddsDAMageFormulaEvent()
    {
        var skill = SkillBuilder.Physical("Test Move").Build();

        Assert.Contains(skill.Data.OnHits, kvp => kvp.Value is DamageFormulaEvent);
    }

    [Fact]
    public void Physical_SetsDefaultHitRate100()
    {
        var skill = SkillBuilder.Physical("Test Move").Build();

        Assert.Equal(100, skill.Data.HitRate);
    }

    [Fact]
    public void Special_SetsCorrectCategory()
    {
        var skill = SkillBuilder.Special("Test Move").Build();

        Assert.Equal(BattleData.SkillCategory.Magical, skill.Data.Category);
    }

    [Fact]
    public void Special_AddsDamageFormulaEvent()
    {
        var skill = SkillBuilder.Special("Test Move").Build();

        Assert.Contains(skill.Data.OnHits, kvp => kvp.Value is DamageFormulaEvent);
    }

    [Fact]
    public void Status_SetsCorrectCategory()
    {
        var skill = SkillBuilder.Status("Test Move").Build();

        Assert.Equal(BattleData.SkillCategory.Status, skill.Data.Category);
    }

    [Fact]
    public void Status_SetsHitRateToNegative1()
    {
        var skill = SkillBuilder.Status("Test Move").Build();

        Assert.Equal(-1, skill.Data.HitRate);
    }

    [Fact]
    public void Status_DoesNotAddDamageFormulaEvent()
    {
        var skill = SkillBuilder.Status("Test Move").Build();

        Assert.DoesNotContain(skill.Data.OnHits, kvp => kvp.Value is DamageFormulaEvent);
    }

    #endregion

    #region Basic Configuration Tests

    [Fact]
    public void Name_SetsCorrectly()
    {
        var skill = SkillBuilder.Physical("Pound").Build();

        Assert.Equal("Pound", skill.Name.DefaultText);
    }

    [Fact]
    public void Desc_SetsCorrectly()
    {
        var skill = SkillBuilder.Physical("Test")
            .Desc("A powerful attack.")
            .Build();

        Assert.Equal("A powerful attack.", skill.Desc.DefaultText);
    }

    [Fact]
    public void Charges_SetsBaseCharges()
    {
        var skill = SkillBuilder.Physical("Test")
            .Charges(25)
            .Build();

        Assert.Equal(25, skill.BaseCharges);
    }

    [Fact]
    public void Element_SetsDataElement()
    {
        var skill = SkillBuilder.Physical("Test")
            .Element("fire")
            .Build();

        Assert.Equal("fire", skill.Data.Element);
    }

    [Fact]
    public void Power_SetsBasePowerState()
    {
        var skill = SkillBuilder.Physical("Test")
            .Power(90)
            .Build();

        var basePowerState = skill.Data.SkillStates.GetWithDefault<BasePowerState>();
        Assert.NotNull(basePowerState);
        Assert.Equal(90, basePowerState.Power);
    }

    [Fact]
    public void Accuracy_OverridesDefaultHitRate()
    {
        var skill = SkillBuilder.Physical("Test")
            .Accuracy(75)
            .Build();

        Assert.Equal(75, skill.Data.HitRate);
    }

    [Fact]
    public void Strikes_SetsMultiHitCount()
    {
        var skill = SkillBuilder.Physical("Test")
            .Strikes(4)
            .Build();

        Assert.Equal(4, skill.Strikes);
    }

    #endregion

    #region Skill State Tests

    [Fact]
    public void Contact_AddsContactState()
    {
        var skill = SkillBuilder.Physical("Test")
            .Contact()
            .Build();

        Assert.True(skill.Data.SkillStates.Contains<ContactState>());
    }

    [Fact]
    public void Fist_AddsFistState()
    {
        var skill = SkillBuilder.Physical("Test")
            .Fist()
            .Build();

        Assert.True(skill.Data.SkillStates.Contains<FistState>());
    }

    [Fact]
    public void Jaw_AddsJawState()
    {
        var skill = SkillBuilder.Physical("Test")
            .Jaw()
            .Build();

        Assert.True(skill.Data.SkillStates.Contains<JawState>());
    }

    [Fact]
    public void Blade_AddsBladeState()
    {
        var skill = SkillBuilder.Physical("Test")
            .Blade()
            .Build();

        Assert.True(skill.Data.SkillStates.Contains<BladeState>());
    }

    [Fact]
    public void Sound_AddsSoundState()
    {
        var skill = SkillBuilder.Physical("Test")
            .Sound()
            .Build();

        Assert.True(skill.Data.SkillStates.Contains<SoundState>());
    }

    #endregion

    #region Hitbox Action Tests

    [Fact]
    public void Melee_CreatesAttackAction()
    {
        var skill = SkillBuilder.Physical("Test")
            .Melee(Strike)
            .Build();

        Assert.IsType<AttackAction>(skill.HitboxAction);
    }

    [Fact]
    public void Melee_SetsCharAnimData()
    {
        var skill = SkillBuilder.Physical("Test")
            .Melee(Strike)
            .Build();

        var action = (AttackAction)skill.HitboxAction;
        Assert.Equal(Strike, ((CharAnimFrameType)action.CharAnimData).ActionType);
    }

    [Fact]
    public void Melee_SetsHitTilesTrue()
    {
        var skill = SkillBuilder.Physical("Test")
            .Melee(Strike)
            .Build();

        var action = (AttackAction)skill.HitboxAction;
        Assert.True(action.HitTiles);
    }

    [Fact]
    public void Melee_SetsTargetAlignmentsFoe()
    {
        var skill = SkillBuilder.Physical("Test")
            .Melee(Strike)
            .Build();

        Assert.Equal(Alignment.Foe, skill.HitboxAction.TargetAlignments);
        Assert.Equal(Alignment.Foe, skill.Explosion.TargetAlignments);
    }

    [Fact]
    public void MeleeWide_SetsWideAngle()
    {
        var skill = SkillBuilder.Physical("Test")
            .MeleeWide(Kick)
            .Build();

        var action = (AttackAction)skill.HitboxAction;
        Assert.Equal(AttackCoverage.Wide, action.WideAngle);
    }

    [Fact]
    public void MeleeAround_SetsAroundCoverage()
    {
        var skill = SkillBuilder.Physical("Test")
            .MeleeAround(Swing)
            .Build();

        var action = (AttackAction)skill.HitboxAction;
        Assert.Equal(AttackCoverage.Around, action.WideAngle);
    }

    [Fact]
    public void Projectile_CreatesProjectileAction()
    {
        var skill = SkillBuilder.Special("Test")
            .Projectile(Shoot, 6)
            .Build();

        Assert.IsType<ProjectileAction>(skill.HitboxAction);
    }

    [Fact]
    public void Projectile_SetsRangeAndSpeed()
    {
        var skill = SkillBuilder.Special("Test")
            .Projectile(Shoot, 6, 12)
            .Build();

        var action = (ProjectileAction)skill.HitboxAction;
        Assert.Equal(6, action.Range);
        Assert.Equal(12, action.Speed);
    }

    [Fact]
    public void Area_CreatesAreaAction()
    {
        var skill = SkillBuilder.Special("Test")
            .Area(Special, 4)
            .Build();

        Assert.IsType<AreaAction>(skill.HitboxAction);
    }

    [Fact]
    public void Area_SetsRangeAndSpeed()
    {
        var skill = SkillBuilder.Special("Test")
            .Area(Special, 4, 8)
            .Build();

        var action = (AreaAction)skill.HitboxAction;
        Assert.Equal(4, action.Range);
        Assert.Equal(8, action.Speed);
    }

    [Fact]
    public void Cone_SetsHitAreaToCone()
    {
        var skill = SkillBuilder.Status("Test")
            .Cone(Sound, 3)
            .Build();

        var action = (AreaAction)skill.HitboxAction;
        Assert.Equal(Hitbox.AreaLimit.Cone, action.HitArea);
    }

    [Fact]
    public void Dash_CreatesDashAction()
    {
        var skill = SkillBuilder.Physical("Test")
            .Dash(3)
            .Build();

        Assert.IsType<DashAction>(skill.HitboxAction);
    }

    [Fact]
    public void Dash_SetsRange()
    {
        var skill = SkillBuilder.Physical("Test")
            .Dash(3)
            .Build();

        var action = (DashAction)skill.HitboxAction;
        Assert.Equal(3, action.Range);
    }

    [Fact]
    public void Self_CreatesSelfAction()
    {
        var skill = SkillBuilder.Status("Test")
            .Self(Charge)
            .Build();

        Assert.IsType<SelfAction>(skill.HitboxAction);
    }

    [Fact]
    public void Self_SetsTargetAlignmentsSelf()
    {
        var skill = SkillBuilder.Status("Test")
            .Self(Charge)
            .Build();

        Assert.Equal(Alignment.Self, skill.HitboxAction.TargetAlignments);
        Assert.Equal(Alignment.Self, skill.Explosion.TargetAlignments);
    }

    [Fact]
    public void TargetAllies_SetsCorrectAlignments()
    {
        var skill = SkillBuilder.Status("Test")
            .Area(Special, 2)
            .TargetAllies()
            .Build();

        var expected = Alignment.Self | Alignment.Friend;
        Assert.Equal(expected, skill.HitboxAction.TargetAlignments);
        Assert.Equal(expected, skill.Explosion.TargetAlignments);
    }

    #endregion

    #region Effect Tests

    [Fact]
    public void HighCrit_AddsBoostCriticalEvent()
    {
        var skill = SkillBuilder.Physical("Test")
            .HighCrit(2)
            .Build();

        Assert.Contains(skill.Data.OnActions, kvp => kvp.Value is BoostCriticalEvent);
    }

    [Fact]
    public void InflictStatus_WithChance_AddsAdditionalEffectState()
    {
        var skill = SkillBuilder.Physical("Test")
            .InflictStatus("burn", 30)
            .Build();

        Assert.True(skill.Data.SkillStates.Contains<AdditionalEffectState>());
    }

    [Fact]
    public void InflictStatus_WithFullChance_DoesNotAddAdditionalEffectState()
    {
        var skill = SkillBuilder.Physical("Test")
            .InflictStatus("burn", 100)
            .Build();

        Assert.False(skill.Data.SkillStates.Contains<AdditionalEffectState>());
    }

    [Fact]
    public void StatChange_AddsStatusStackBattleEvent()
    {
        var skill = SkillBuilder.Status("Test")
            .StatChange("mod_attack", 2, true)
            .Build();

        Assert.Contains(skill.Data.OnHits, kvp => kvp.Value is StatusStackBattleEvent);
    }

    [Fact]
    public void Recoil_AddsHPRecoilEvent()
    {
        var skill = SkillBuilder.Physical("Test")
            .Recoil(4)
            .Build();

        Assert.Contains(skill.Data.AfterActions, kvp => kvp.Value is HPRecoilEvent);
    }

    [Fact]
    public void Knockback_AddsKnockBackEvent()
    {
        var skill = SkillBuilder.Status("Test")
            .Knockback(5)
            .Build();

        Assert.Contains(skill.Data.OnHits, kvp => kvp.Value is KnockBackEvent);
    }

    #endregion

    #region Sound/Emitter Tests

    [Fact]
    public void UseSound_SetsActionFXSound()
    {
        var skill = SkillBuilder.Physical("Test")
            .Melee(Strike)
            .UseSound("DUN_Punch")
            .Build();

        Assert.Equal("DUN_Punch", skill.HitboxAction.ActionFX.Sound);
    }

    [Fact]
    public void PreSound_AddsPreAction()
    {
        var skill = SkillBuilder.Physical("Test")
            .Melee(Strike)
            .PreSound("DUN_Charge")
            .Build();

        Assert.NotEmpty(skill.HitboxAction.PreActions);
        Assert.Equal("DUN_Charge", skill.HitboxAction.PreActions[0].Sound);
    }

    #endregion

    #region ApplyTo Tests

    [Fact]
    public void ApplyTo_CopiesAllProperties()
    {
        var target = new SkillData();

        SkillBuilder.Physical("Test Move")
            .Desc("Test description")
            .Charges(15)
            .Element("fire")
            .Power(80)
            .Accuracy(90)
            .Contact()
            .Melee(Strike)
            .UseSound("DUN_Test")
            .ApplyTo(target);

        Assert.Equal("Test Move", target.Name.DefaultText);
        Assert.Equal("Test description", target.Desc.DefaultText);
        Assert.Equal(15, target.BaseCharges);
        Assert.Equal("fire", target.Data.Element);
        Assert.Equal(BattleData.SkillCategory.Physical, target.Data.Category);
        Assert.Equal(90, target.Data.HitRate);
        Assert.True(target.Data.SkillStates.Contains<ContactState>());
        Assert.True(target.Data.SkillStates.Contains<BasePowerState>());
        Assert.IsType<AttackAction>(target.HitboxAction);
    }

    #endregion

    #region Regression Tests - Compare Builder to Traditional

    [Fact]
    public void Pound_MatchesTraditionalDefinition()
    {
        // Build using builder
        var builderSkill = SkillBuilder.Physical("Pound")
            .Desc("The target is physically pounded with a long tail, a foreleg, or the like.")
            .Charges(25).Element("normal").Power(45)
            .Contact().Melee(Strike)
            .PreSound("DUN_Pound")
            .Build();

        // Build traditionally
        var traditionalSkill = new SkillData();
        traditionalSkill.Name = new LocalText("Pound");
        traditionalSkill.Desc = new LocalText("The target is physically pounded with a long tail, a foreleg, or the like.");
        traditionalSkill.BaseCharges = 25;
        traditionalSkill.Data.Element = "normal";
        traditionalSkill.Data.Category = BattleData.SkillCategory.Physical;
        traditionalSkill.Data.SkillStates.Set(new ContactState());
        traditionalSkill.Data.HitRate = 100;
        traditionalSkill.Data.SkillStates.Set(new BasePowerState(45));
        traditionalSkill.Data.OnHits.Add(-1, new DamageFormulaEvent());
        traditionalSkill.Strikes = 1;
        traditionalSkill.HitboxAction = new AttackAction();
        ((AttackAction)traditionalSkill.HitboxAction).CharAnimData = new CharAnimFrameType(08);
        ((AttackAction)traditionalSkill.HitboxAction).HitTiles = true;
        traditionalSkill.HitboxAction.TargetAlignments = Alignment.Foe;
        traditionalSkill.Explosion.TargetAlignments = Alignment.Foe;
        var preFX = new BattleFX();
        preFX.Sound = "DUN_Pound";
        traditionalSkill.HitboxAction.PreActions.Add(preFX);

        // Compare key properties
        Assert.Equal(traditionalSkill.Name.DefaultText, builderSkill.Name.DefaultText);
        Assert.Equal(traditionalSkill.Desc.DefaultText, builderSkill.Desc.DefaultText);
        Assert.Equal(traditionalSkill.BaseCharges, builderSkill.BaseCharges);
        Assert.Equal(traditionalSkill.Data.Element, builderSkill.Data.Element);
        Assert.Equal(traditionalSkill.Data.Category, builderSkill.Data.Category);
        Assert.Equal(traditionalSkill.Data.HitRate, builderSkill.Data.HitRate);
        Assert.Equal(traditionalSkill.Strikes, builderSkill.Strikes);

        // Compare skill states
        Assert.True(builderSkill.Data.SkillStates.Contains<ContactState>());
        Assert.True(builderSkill.Data.SkillStates.Contains<BasePowerState>());

        // Compare action type
        Assert.IsType<AttackAction>(builderSkill.HitboxAction);
        var builderAction = (AttackAction)builderSkill.HitboxAction;
        var traditionalAction = (AttackAction)traditionalSkill.HitboxAction;
        Assert.Equal(
            ((CharAnimFrameType)traditionalAction.CharAnimData).ActionType,
            ((CharAnimFrameType)builderAction.CharAnimData).ActionType);
        Assert.Equal(traditionalAction.HitTiles, builderAction.HitTiles);
        Assert.Equal(traditionalAction.TargetAlignments, builderAction.TargetAlignments);

        // Compare sound
        Assert.Equal(
            traditionalSkill.HitboxAction.PreActions[0].Sound,
            builderSkill.HitboxAction.PreActions[0].Sound);
    }

    [Fact]
    public void Tackle_MatchesTraditionalDefinition()
    {
        // Build using builder
        var builderSkill = SkillBuilder.Physical("Tackle")
            .Desc("A physical attack in which the user charges and slams into the target with its whole body.")
            .Charges(22).Element("normal").Power(45)
            .Contact().Dash(2).UseSound("DUN_Tackle")
            .Build();

        // Build traditionally
        var traditionalSkill = new SkillData();
        traditionalSkill.Name = new LocalText("Tackle");
        traditionalSkill.Desc = new LocalText("A physical attack in which the user charges and slams into the target with its whole body.");
        traditionalSkill.BaseCharges = 22;
        traditionalSkill.Data.Element = "normal";
        traditionalSkill.Data.Category = BattleData.SkillCategory.Physical;
        traditionalSkill.Data.SkillStates.Set(new ContactState());
        traditionalSkill.Data.HitRate = 100;
        traditionalSkill.Data.SkillStates.Set(new BasePowerState(45));
        traditionalSkill.Data.OnHits.Add(-1, new DamageFormulaEvent());
        traditionalSkill.Strikes = 1;
        traditionalSkill.HitboxAction = new DashAction();
        ((DashAction)traditionalSkill.HitboxAction).Range = 2;
        ((DashAction)traditionalSkill.HitboxAction).StopAtWall = true;
        ((DashAction)traditionalSkill.HitboxAction).StopAtHit = true;
        ((DashAction)traditionalSkill.HitboxAction).HitTiles = true;
        traditionalSkill.HitboxAction.TargetAlignments = Alignment.Foe;
        traditionalSkill.Explosion.TargetAlignments = Alignment.Foe;
        traditionalSkill.HitboxAction.ActionFX.Sound = "DUN_Tackle";

        // Compare key properties
        Assert.Equal(traditionalSkill.Name.DefaultText, builderSkill.Name.DefaultText);
        Assert.Equal(traditionalSkill.BaseCharges, builderSkill.BaseCharges);
        Assert.Equal(traditionalSkill.Data.Element, builderSkill.Data.Element);

        // Compare action
        Assert.IsType<DashAction>(builderSkill.HitboxAction);
        var builderAction = (DashAction)builderSkill.HitboxAction;
        var traditionalAction = (DashAction)traditionalSkill.HitboxAction;
        Assert.Equal(traditionalAction.Range, builderAction.Range);
        Assert.Equal(traditionalAction.StopAtWall, builderAction.StopAtWall);
        Assert.Equal(traditionalAction.StopAtHit, builderAction.StopAtHit);
        Assert.Equal(traditionalAction.HitTiles, builderAction.HitTiles);
        Assert.Equal(traditionalAction.ActionFX.Sound, builderAction.ActionFX.Sound);
    }

    [Fact]
    public void SwordsDance_MatchesTraditionalDefinition()
    {
        // Build using builder
        var builderSkill = SkillBuilder.Status("Swords Dance")
            .Desc("A frenetic dance to uplift the fighting spirit. This sharply raises the Attack stat.")
            .Charges(12).Element("normal")
            .StatChange("mod_attack", 2, true)
            .Self(TailWhip).TargetAllies()
            .ActionEmitter("Swords_Dance", 3)
            .PreSound("DUN_Icicle_Spear").UseSound("DUN_Swords_Dance_2")
            .Build();

        // Build traditionally
        var traditionalSkill = new SkillData();
        traditionalSkill.Name = new LocalText("Swords Dance");
        traditionalSkill.Desc = new LocalText("A frenetic dance to uplift the fighting spirit. This sharply raises the Attack stat.");
        traditionalSkill.BaseCharges = 12;
        traditionalSkill.Data.Element = "normal";
        traditionalSkill.Data.Category = BattleData.SkillCategory.Status;
        traditionalSkill.Data.HitRate = -1;
        traditionalSkill.Data.OnHits.Add(0, new StatusStackBattleEvent("mod_attack", true, false, 2));
        traditionalSkill.Strikes = 1;
        traditionalSkill.HitboxAction = new SelfAction();
        ((SelfAction)traditionalSkill.HitboxAction).CharAnimData = new CharAnimFrameType(27);
        traditionalSkill.HitboxAction.TargetAlignments = (Alignment.Self | Alignment.Friend);
        traditionalSkill.Explosion.TargetAlignments = (Alignment.Self | Alignment.Friend);

        // Compare key properties
        Assert.Equal(traditionalSkill.Name.DefaultText, builderSkill.Name.DefaultText);
        Assert.Equal(traditionalSkill.BaseCharges, builderSkill.BaseCharges);
        Assert.Equal(traditionalSkill.Data.Element, builderSkill.Data.Element);
        Assert.Equal(traditionalSkill.Data.Category, builderSkill.Data.Category);
        Assert.Equal(traditionalSkill.Data.HitRate, builderSkill.Data.HitRate);

        // Compare action type and targeting
        Assert.IsType<SelfAction>(builderSkill.HitboxAction);
        Assert.Equal(traditionalSkill.HitboxAction.TargetAlignments, builderSkill.HitboxAction.TargetAlignments);
        Assert.Equal(traditionalSkill.Explosion.TargetAlignments, builderSkill.Explosion.TargetAlignments);
    }

    #endregion

    #region CharAnim Constant Tests

    [Theory]
    [InlineData(Strike, 08)]
    [InlineData(Chop, 09)]
    [InlineData(Scratch, 10)]
    [InlineData(Punch, 11)]
    [InlineData(Slap, 12)]
    [InlineData(Bite, 18)]
    [InlineData(Jab, 20)]
    [InlineData(Kick, 21)]
    [InlineData(TailWhip, 27)]
    [InlineData(Shoot, 07)]
    [InlineData(Charge, 06)]
    public void CharAnim_HasCorrectValues(int actual, int expected)
    {
        Assert.Equal(expected, actual);
    }

    #endregion
}
