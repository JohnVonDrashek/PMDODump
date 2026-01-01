using System;
using RogueEssence;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;
using PMDC.Dungeon;
using PMDC.Data;

namespace DataGenerator.Data
{
    /// <summary>
    /// Common character animation types used in skill definitions.
    /// </summary>
    public static class CharAnim
    {
        public const int Attack = 05;
        public const int Charge = 06;
        public const int Shoot = 07;
        public const int Strike = 08;
        public const int Chop = 09;
        public const int Scratch = 10;
        public const int Punch = 11;
        public const int Slap = 12;
        public const int Slam = 13;
        public const int Uppercut = 14;
        public const int Bite = 18;
        public const int Shake = 19;
        public const int Jab = 20;
        public const int Kick = 21;
        public const int Lick = 22;
        public const int Headbutt = 23;
        public const int Stomp = 24;
        public const int Hop = 25;
        public const int Dance = 26;
        public const int TailWhip = 27;
        public const int Sing = 29;
        public const int Sound = 30;
        public const int Rumble = 31;
        public const int FlapAround = 32;
        public const int Emit = 35;
        public const int Special = 36;
        public const int Withdraw = 37;
        public const int RearUp = 38;
        public const int Swell = 39;
        public const int Swing = 40;
    }

    /// <summary>
    /// Fluent builder for creating skill definitions with reduced boilerplate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Provides factory methods for common skill archetypes (Physical, Special, Status)
    /// and chainable configuration for hitboxes, effects, and animations.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// SkillBuilder.Physical("Pound")
    ///     .Desc("The target is physically pounded...")
    ///     .Charges(25).Element("normal").Power(45)
    ///     .Contact().Melee(CharAnim.Strike)
    ///     .PreSound("DUN_Pound")
    ///     .ApplyTo(skill);
    /// </code>
    /// </para>
    /// </remarks>
    public class SkillBuilder
    {
        private readonly SkillData _skill;

        private SkillBuilder(string name, BattleData.SkillCategory category)
        {
            _skill = new SkillData();
            _skill.Name = new LocalText(name);
            _skill.Data.Category = category;
            _skill.Data.HitRate = 100;
            _skill.Strikes = 1;
            _skill.HitboxAction = new AttackAction();
            _skill.HitboxAction.TargetAlignments = Alignment.Foe;
            _skill.Explosion.TargetAlignments = Alignment.Foe;
        }

        #region Factory Methods

        /// <summary>
        /// Creates a builder for a physical attack skill.
        /// </summary>
        /// <param name="name">The skill name (can include prefix like "-" or "=").</param>
        /// <returns>A new SkillBuilder configured for physical attacks.</returns>
        public static SkillBuilder Physical(string name)
        {
            var builder = new SkillBuilder(name, BattleData.SkillCategory.Physical);
            builder._skill.Data.OnHits.Add(-1, new DamageFormulaEvent());
            return builder;
        }

        /// <summary>
        /// Creates a builder for a special attack skill.
        /// </summary>
        /// <param name="name">The skill name.</param>
        /// <returns>A new SkillBuilder configured for special attacks.</returns>
        public static SkillBuilder Special(string name)
        {
            var builder = new SkillBuilder(name, BattleData.SkillCategory.Magical);
            builder._skill.Data.OnHits.Add(-1, new DamageFormulaEvent());
            return builder;
        }

        /// <summary>
        /// Creates a builder for a status skill (no damage).
        /// </summary>
        /// <param name="name">The skill name.</param>
        /// <returns>A new SkillBuilder configured for status moves.</returns>
        public static SkillBuilder Status(string name)
        {
            var builder = new SkillBuilder(name, BattleData.SkillCategory.Status);
            builder._skill.Data.HitRate = -1;
            return builder;
        }

        #endregion

        #region Basic Configuration

        /// <summary>
        /// Sets the skill description.
        /// </summary>
        public SkillBuilder Desc(string description)
        {
            _skill.Desc = new LocalText(description);
            return this;
        }

        /// <summary>
        /// Sets the base PP (charges) for the skill.
        /// </summary>
        public SkillBuilder Charges(int charges)
        {
            _skill.BaseCharges = charges;
            return this;
        }

        /// <summary>
        /// Sets the elemental type of the skill.
        /// </summary>
        public SkillBuilder Element(string element)
        {
            _skill.Data.Element = element;
            return this;
        }

        /// <summary>
        /// Sets the base power of the skill.
        /// </summary>
        public SkillBuilder Power(int power)
        {
            _skill.Data.SkillStates.Set(new BasePowerState(power));
            return this;
        }

        /// <summary>
        /// Sets the accuracy/hit rate of the skill.
        /// </summary>
        public SkillBuilder Accuracy(int hitRate)
        {
            _skill.Data.HitRate = hitRate;
            return this;
        }

        /// <summary>
        /// Sets the number of strikes for multi-hit moves.
        /// </summary>
        public SkillBuilder Strikes(int strikes)
        {
            _skill.Strikes = strikes;
            return this;
        }

        #endregion

        #region Skill States

        /// <summary>
        /// Marks this skill as making contact with the target.
        /// </summary>
        public SkillBuilder Contact()
        {
            _skill.Data.SkillStates.Set(new ContactState());
            return this;
        }

        /// <summary>
        /// Marks this skill as a punching move.
        /// </summary>
        public SkillBuilder Fist()
        {
            _skill.Data.SkillStates.Set(new FistState());
            return this;
        }

        /// <summary>
        /// Marks this skill as a biting move.
        /// </summary>
        public SkillBuilder Jaw()
        {
            _skill.Data.SkillStates.Set(new JawState());
            return this;
        }

        /// <summary>
        /// Marks this skill as a slashing/cutting move.
        /// </summary>
        public SkillBuilder Blade()
        {
            _skill.Data.SkillStates.Set(new BladeState());
            return this;
        }

        /// <summary>
        /// Marks this skill as a sound-based move.
        /// </summary>
        public SkillBuilder Sound()
        {
            _skill.Data.SkillStates.Set(new SoundState());
            return this;
        }

        #endregion

        #region Hitbox Actions

        /// <summary>
        /// Configures this skill as a melee attack with AttackAction.
        /// </summary>
        /// <param name="anim">The character animation to use.</param>
        /// <param name="hitTiles">Whether the attack hits tiles. Default true.</param>
        public SkillBuilder Melee(int anim, bool hitTiles = true)
        {
            var action = new AttackAction();
            action.CharAnimData = new CharAnimFrameType(anim);
            action.HitTiles = hitTiles;
            action.TargetAlignments = Alignment.Foe;
            _skill.HitboxAction = action;
            _skill.Explosion.TargetAlignments = Alignment.Foe;
            return this;
        }

        /// <summary>
        /// Configures melee attack with wide angle (hits sides too).
        /// </summary>
        public SkillBuilder MeleeWide(int anim, bool hitTiles = true)
        {
            Melee(anim, hitTiles);
            ((AttackAction)_skill.HitboxAction).WideAngle = AttackCoverage.Wide;
            return this;
        }

        /// <summary>
        /// Configures melee attack that hits all around the user.
        /// </summary>
        public SkillBuilder MeleeAround(int anim, bool hitTiles = true)
        {
            Melee(anim, hitTiles);
            ((AttackAction)_skill.HitboxAction).WideAngle = AttackCoverage.Around;
            return this;
        }

        /// <summary>
        /// Configures this skill as a projectile attack.
        /// </summary>
        /// <param name="anim">The character animation to use.</param>
        /// <param name="range">Projectile range in tiles.</param>
        /// <param name="speed">Projectile speed.</param>
        /// <param name="projectileAnim">Optional projectile sprite animation.</param>
        public SkillBuilder Projectile(int anim, int range, int speed = 10, string projectileAnim = null)
        {
            var action = new ProjectileAction();
            action.CharAnimData = new CharAnimFrameType(anim);
            action.Range = range;
            action.Speed = speed;
            action.StopAtWall = true;
            action.StopAtHit = true;
            action.HitTiles = true;
            if (projectileAnim != null)
                action.Anim = new AnimData(projectileAnim, 3);
            action.TargetAlignments = Alignment.Foe;
            _skill.HitboxAction = action;
            _skill.Explosion.TargetAlignments = Alignment.Foe;
            return this;
        }

        /// <summary>
        /// Configures this skill as an area effect.
        /// </summary>
        /// <param name="anim">The character animation to use.</param>
        /// <param name="range">Area range in tiles.</param>
        /// <param name="speed">Effect expansion speed.</param>
        public SkillBuilder Area(int anim, int range, int speed = 10)
        {
            var action = new AreaAction();
            action.CharAnimData = new CharAnimFrameType(anim);
            action.Range = range;
            action.Speed = speed;
            action.HitTiles = true;
            action.TargetAlignments = Alignment.Foe;
            _skill.HitboxAction = action;
            _skill.Explosion.TargetAlignments = Alignment.Foe;
            return this;
        }

        /// <summary>
        /// Configures this skill as a cone-shaped area effect.
        /// </summary>
        public SkillBuilder Cone(int anim, int range, int speed = 10)
        {
            Area(anim, range, speed);
            ((AreaAction)_skill.HitboxAction).HitArea = Hitbox.AreaLimit.Cone;
            return this;
        }

        /// <summary>
        /// Configures this skill as self-targeting.
        /// </summary>
        /// <param name="anim">The character animation to use.</param>
        public SkillBuilder Self(int anim)
        {
            var action = new SelfAction();
            action.CharAnimData = new CharAnimFrameType(anim);
            action.TargetAlignments = Alignment.Self;
            _skill.HitboxAction = action;
            _skill.Explosion.TargetAlignments = Alignment.Self;
            return this;
        }

        /// <summary>
        /// Configures this skill as a dash attack.
        /// </summary>
        /// <param name="range">Dash range in tiles.</param>
        /// <param name="stopAtHit">Whether to stop when hitting a target.</param>
        public SkillBuilder Dash(int range, bool stopAtHit = true)
        {
            var action = new DashAction();
            action.Range = range;
            action.StopAtWall = true;
            action.StopAtHit = stopAtHit;
            action.HitTiles = true;
            action.TargetAlignments = Alignment.Foe;
            _skill.HitboxAction = action;
            _skill.Explosion.TargetAlignments = Alignment.Foe;
            return this;
        }

        /// <summary>
        /// Configures this skill as a dash attack with specific animation.
        /// </summary>
        /// <param name="anim">The character animation to use.</param>
        /// <param name="range">Dash range in tiles.</param>
        /// <param name="stopAtHit">Whether to stop when hitting a target.</param>
        public SkillBuilder Dash(int anim, int range, bool stopAtHit = true)
        {
            var action = new DashAction();
            action.CharAnim = anim;
            action.Range = range;
            action.StopAtWall = true;
            action.StopAtHit = stopAtHit;
            action.HitTiles = true;
            action.TargetAlignments = Alignment.Foe;
            _skill.HitboxAction = action;
            _skill.Explosion.TargetAlignments = Alignment.Foe;
            return this;
        }

        /// <summary>
        /// Configures this skill to target self and allies.
        /// </summary>
        public SkillBuilder TargetAllies()
        {
            _skill.HitboxAction.TargetAlignments = Alignment.Self | Alignment.Friend;
            _skill.Explosion.TargetAlignments = Alignment.Self | Alignment.Friend;
            return this;
        }

        /// <summary>
        /// Configures this skill to target foes and allies (but not self).
        /// </summary>
        public SkillBuilder TargetAll()
        {
            _skill.HitboxAction.TargetAlignments = Alignment.Friend | Alignment.Foe;
            _skill.Explosion.TargetAlignments = Alignment.Friend | Alignment.Foe;
            return this;
        }

        #endregion

        #region Sound and Visual Effects

        /// <summary>
        /// Sets the sound effect played when the skill is used.
        /// </summary>
        public SkillBuilder UseSound(string sound)
        {
            _skill.HitboxAction.ActionFX.Sound = sound;
            return this;
        }

        /// <summary>
        /// Adds a pre-action sound effect (played before the attack).
        /// </summary>
        public SkillBuilder PreSound(string sound)
        {
            var preFX = new BattleFX();
            preFX.Sound = sound;
            _skill.HitboxAction.PreActions.Add(preFX);
            return this;
        }

        /// <summary>
        /// Sets the hit effect sound.
        /// </summary>
        public SkillBuilder HitSound(string sound)
        {
            _skill.Data.HitFX.Sound = sound;
            return this;
        }

        /// <summary>
        /// Adds an emitter animation to the attack action.
        /// </summary>
        public SkillBuilder Emitter(string animName, int frameTime = 3)
        {
            if (_skill.HitboxAction is AttackAction attack)
            {
                attack.Emitter = new SingleEmitter(new AnimData(animName, frameTime));
            }
            return this;
        }

        /// <summary>
        /// Sets the hit effect emitter.
        /// </summary>
        public SkillBuilder HitEmitter(string animName, int frameTime = 3)
        {
            _skill.Data.HitFX.Emitter = new SingleEmitter(new AnimData(animName, frameTime));
            return this;
        }

        /// <summary>
        /// Sets the action FX emitter (shown during action).
        /// </summary>
        public SkillBuilder ActionEmitter(string animName, int frameTime = 3)
        {
            _skill.HitboxAction.ActionFX.Emitter = new SingleEmitter(new AnimData(animName, frameTime));
            return this;
        }

        /// <summary>
        /// Sets lag time after the attack animation.
        /// </summary>
        public SkillBuilder Lag(int frames)
        {
            if (_skill.HitboxAction is AttackAction attack)
            {
                attack.LagBehindTime = frames;
            }
            return this;
        }

        #endregion

        #region Status Effects

        /// <summary>
        /// Adds a chance to inflict a status effect on hit.
        /// </summary>
        /// <param name="status">The status ID to inflict.</param>
        /// <param name="chance">Percent chance (100 = guaranteed).</param>
        public SkillBuilder InflictStatus(string status, int chance = 100)
        {
            if (chance < 100)
            {
                _skill.Data.SkillStates.Set(new AdditionalEffectState(chance));
                _skill.Data.OnHits.Add(0, new AdditionalEvent(new StatusBattleEvent(status, true, true)));
            }
            else
            {
                _skill.Data.OnHits.Add(0, new StatusBattleEvent(status, true, false));
            }
            return this;
        }

        /// <summary>
        /// Adds a stat stage change on hit.
        /// </summary>
        /// <param name="stat">The stat modifier ID (e.g., "mod_attack").</param>
        /// <param name="stages">Number of stages to change (negative for drops).</param>
        /// <param name="targetSelf">True to affect user, false to affect target.</param>
        public SkillBuilder StatChange(string stat, int stages, bool targetSelf = false)
        {
            _skill.Data.OnHits.Add(0, new StatusStackBattleEvent(stat, true, targetSelf, stages));
            return this;
        }

        /// <summary>
        /// Increases critical hit rate.
        /// </summary>
        /// <param name="stages">Number of crit stages to add.</param>
        public SkillBuilder HighCrit(int stages = 1)
        {
            _skill.Data.OnActions.Add(0, new BoostCriticalEvent(stages));
            return this;
        }

        /// <summary>
        /// Adds recoil damage after the attack.
        /// </summary>
        /// <param name="divisor">Fraction of damage dealt as recoil (3 = 1/3).</param>
        /// <param name="maxHP">If true, recoil is based on max HP; if false, based on damage dealt.</param>
        public SkillBuilder Recoil(int divisor, bool maxHP = false)
        {
            _skill.Data.AfterActions.Add(0, new HPRecoilEvent(divisor, maxHP));
            return this;
        }

        /// <summary>
        /// Applies a continuous damage effect (like Wrap, Bind).
        /// </summary>
        public SkillBuilder ContinuousDamage(string status)
        {
            _skill.Data.OnHits.Add(0, new OnHitEvent(true, false, 100, new GiveContinuousDamageEvent(status, true, true)));
            return this;
        }

        /// <summary>
        /// Knocks back the target.
        /// </summary>
        public SkillBuilder Knockback(int tiles)
        {
            _skill.Data.OnHits.Add(0, new KnockBackEvent(tiles));
            return this;
        }

        #endregion

        #region Build Methods

        /// <summary>
        /// Builds and returns the configured SkillData.
        /// </summary>
        /// <returns>The fully configured SkillData object.</returns>
        public SkillData Build() => _skill;

        /// <summary>
        /// Copies all properties from this builder to an existing SkillData object.
        /// Use this with the existing FillSkills methods that take a skill parameter.
        /// </summary>
        public void ApplyTo(SkillData target)
        {
            target.Name = _skill.Name;
            target.Desc = _skill.Desc;
            target.BaseCharges = _skill.BaseCharges;
            target.Strikes = _skill.Strikes;
            target.HitboxAction = _skill.HitboxAction;
            target.Explosion = _skill.Explosion;

            target.Data.Element = _skill.Data.Element;
            target.Data.Category = _skill.Data.Category;
            target.Data.HitRate = _skill.Data.HitRate;
            target.Data.HitFX = _skill.Data.HitFX;

            // Copy skill states
            foreach (var state in _skill.Data.SkillStates)
                target.Data.SkillStates.Set(state);

            // Copy event handlers
            foreach (var kvp in _skill.Data.OnActions)
                target.Data.OnActions.Add(kvp.Key, kvp.Value);
            foreach (var kvp in _skill.Data.BeforeHits)
                target.Data.BeforeHits.Add(kvp.Key, kvp.Value);
            foreach (var kvp in _skill.Data.OnHits)
                target.Data.OnHits.Add(kvp.Key, kvp.Value);
            foreach (var kvp in _skill.Data.AfterActions)
                target.Data.AfterActions.Add(kvp.Key, kvp.Value);
        }

        #endregion
    }
}
