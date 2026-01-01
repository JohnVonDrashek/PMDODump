# Skill Builder Pattern Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Create a fluent builder API to reduce skill definition boilerplate from ~30 lines to ~8 lines per skill.

**Architecture:** A `SkillBuilder` class with factory methods (`Physical()`, `Special()`, `Status()`) and chainable configuration methods. The builder populates a `SkillData` object with sensible defaults, then returns it via `Build()`. Existing if/else dispatch remains unchanged.

**Tech Stack:** C# 8.0+, targets `DataGenerator.Data` namespace, depends on `RogueEssence.*` and `PMDC.*` types.

---

## Task 1: Create SkillBuilder Core Class

**Files:**
- Create: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Create the SkillBuilder class with basic structure**

```csharp
using System;
using RogueEssence;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using PMDC.Dungeon;
using PMDC.Data;

namespace DataGenerator.Data
{
    /// <summary>
    /// Fluent builder for creating skill definitions with reduced boilerplate.
    /// </summary>
    /// <remarks>
    /// Provides factory methods for common skill archetypes (Physical, Special, Status)
    /// and chainable configuration for hitboxes, effects, and animations.
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
            builder._skill.Data.HitRate = -1; // Status moves typically always hit
            return builder;
        }

        /// <summary>
        /// Builds and returns the configured SkillData.
        /// </summary>
        /// <returns>The fully configured SkillData object.</returns>
        public SkillData Build() => _skill;
    }
}
```

**Step 2: Verify file compiles (no test runner, just syntax check)**

Run: `dotnet build DataGenerator/DataGenerator.csproj --no-restore 2>&1 | head -20`
Expected: Build errors due to missing submodule references (expected in worktree without submodules initialized)

---

## Task 2: Add Basic Configuration Methods

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Add description, charges, element, power, and accuracy methods**

Add these methods to `SkillBuilder` class:

```csharp
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
```

---

## Task 3: Add Skill State Methods

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Add contact, fist, jaw, blade, and sound state methods**

```csharp
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
```

---

## Task 4: Add Melee Action Builder

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Add CharAnim enum for common animation types**

Add before the SkillBuilder class:

```csharp
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
```

**Step 2: Add Melee method for AttackAction hitbox**

```csharp
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
        /// Configures melee attack with wide angle (hits around user).
        /// </summary>
        public SkillBuilder MeleeWide(int anim, bool hitTiles = true)
        {
            Melee(anim, hitTiles);
            ((AttackAction)_skill.HitboxAction).WideAngle = AttackCoverage.Around;
            return this;
        }
```

---

## Task 5: Add Projectile Action Builder

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Add Projectile method**

```csharp
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
```

---

## Task 6: Add Area and Self Action Builders

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Add Area and Self methods**

```csharp
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
        /// Configures this skill to target self and allies.
        /// </summary>
        public SkillBuilder TargetAllies()
        {
            _skill.HitboxAction.TargetAlignments = Alignment.Self | Alignment.Friend;
            _skill.Explosion.TargetAlignments = Alignment.Self | Alignment.Friend;
            return this;
        }
```

---

## Task 7: Add Sound and Effect Methods

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Add sound, emitter, and effect methods**

```csharp
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
        /// Adds an emitter animation to the attack.
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
```

---

## Task 8: Add Status Effect Methods

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillBuilder.cs`

**Step 1: Add methods for applying status effects**

```csharp
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
        public SkillBuilder Recoil(int divisor)
        {
            _skill.Data.AfterActions.Add(0, new HPRecoilEvent(divisor));
            return this;
        }
```

---

## Task 9: Migrate First 10 Simple Skills as Proof of Concept

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillsPMD.cs`

**Step 1: Migrate Pound (index 1)**

Replace the existing `else if (ii == 1)` block with:

```csharp
            else if (ii == 1)
            {
                return SkillBuilder.Physical("Pound")
                    .Desc("The target is physically pounded with a long tail, a foreleg, or the like.")
                    .Charges(25).Element("normal").Power(45)
                    .Contact().Melee(CharAnim.Strike)
                    .PreSound("DUN_Pound")
                    .Build();
            }
```

Wait - the current method signature is `void FillSkillsPMD(SkillData skill, ...)`. We need to copy properties from the built skill to the passed-in skill parameter.

**Revised Step 1: Add a helper method to copy from builder to existing skill**

Add to SkillBuilder:

```csharp
        /// <summary>
        /// Copies all properties from this builder to an existing SkillData object.
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
```

**Step 2: Migrate Pound (index 1) using ApplyTo pattern**

Replace in SkillsPMD.cs:

```csharp
            else if (ii == 1)
            {
                SkillBuilder.Physical("Pound")
                    .Desc("The target is physically pounded with a long tail, a foreleg, or the like.")
                    .Charges(25).Element("normal").Power(45)
                    .Contact().Melee(CharAnim.Strike)
                    .PreSound("DUN_Pound")
                    .ApplyTo(skill);
            }
```

**Step 3: Migrate additional simple melee skills**

Migrate these skills (indices 2-10) following the same pattern:

- 2: Karate Chop - Physical with HighCrit
- 3: Double Slap - Multi-strike
- 4: Comet Punch - Multi-strike with Fist
- 5: Mega Punch - High power Fist
- 7: Fire Punch - Fist with burn status
- 8: Ice Punch - Fist with freeze status
- 9: Thunder Punch - Fist with paralyze status
- 10: Scratch - Basic physical with Scratch anim

---

## Task 10: Migrate Skills 20-30

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillsPMD.cs`

Skills to migrate:
- 20: Bind - Contact damage + continuous damage status
- 21: Slam - Contact physical
- 22: Vine Whip - Ranged physical
- 23: Stomp - Contact with flinch
- 24: Double Kick - Multi-hit kick
- 25: Mega Kick - High power kick
- 26: Jump Kick - Contact with crash damage
- 27: Rolling Kick - Contact with flinch
- 28: Sand Attack - Status that lowers accuracy
- 29: Headbutt - Contact with flinch
- 30: Horn Attack - Contact physical

---

## Task 11: Migrate Skills 40-50 (Mixed Types)

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillsPMD.cs`

Skills to migrate (mix of physical, special, status):
- 40: Poison Sting - Projectile with poison
- 41: Twineedle - Multi-hit projectile
- 42: Pin Missile - Multi-hit projectile
- 43: Leer - Status projectile
- 44: Bite - Contact Jaw with flinch
- 45: Growl - Area status
- 46: Roar - Area knockback
- 47: Sing - Area sleep
- 48: Supersonic - Area confuse
- 49: Sonic Boom - Fixed damage projectile (complex - skip)
- 50: Disable - Status (complex - skip)

---

## Task 12: Add Using Statement to SkillsPMD

**Files:**
- Modify: `DataGenerator/Data/Skills/SkillsPMD.cs`

**Step 1: Add static using for CharAnim**

At the top of the file, add:

```csharp
using static DataGenerator.Data.CharAnim;
```

This allows using `Strike` instead of `CharAnim.Strike`.

---

## Task 13: Verify No Regression (Manual Comparison)

**Files:**
- No files modified

**Step 1: Document verification approach**

Since submodules aren't initialized in the worktree, full build/test isn't possible. Verification approach:

1. Visual inspection: Compare before/after code structure
2. Property coverage: Ensure all original properties are set
3. Future: When submodules are available, run DataGenerator and compare output JSON

**Manual Checklist per migrated skill:**
- [ ] Name matches original
- [ ] Desc matches original
- [ ] BaseCharges matches
- [ ] Element matches
- [ ] Category matches (Physical/Special/Status)
- [ ] HitRate matches
- [ ] BasePowerState value matches
- [ ] All SkillStates present (ContactState, FistState, etc.)
- [ ] All OnHits events present
- [ ] HitboxAction type matches
- [ ] CharAnimData matches
- [ ] HitTiles matches
- [ ] TargetAlignments matches
- [ ] Sound effects match

---

## Summary

This plan creates a `SkillBuilder` fluent API that reduces skill definitions from ~30 lines to ~5-8 lines. Key features:

1. **Factory methods**: `Physical()`, `Special()`, `Status()` set category-appropriate defaults
2. **Chainable configuration**: Element, power, charges, accuracy, strikes
3. **Skill states**: `Contact()`, `Fist()`, `Jaw()`, `Blade()`, `Sound()`
4. **Hitbox actions**: `Melee()`, `Projectile()`, `Area()`, `Self()`, `Cone()`
5. **Effects**: `UseSound()`, `PreSound()`, `Emitter()`, `HitEmitter()`
6. **Status effects**: `InflictStatus()`, `StatChange()`, `HighCrit()`, `Recoil()`

The 50 migrated skills serve as proof-of-concept. Full migration of 600+ skills would be done in subsequent tickets.
