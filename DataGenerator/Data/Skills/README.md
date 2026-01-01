# Skills

Move and attack definitions for PMD: Origins.

## Overview

This directory contains the complete moveset for all Pokemon attacks adapted from the main series games. Moves are split across files by generation, with `SkillInfo.cs` providing the routing logic and shared utilities. Each move definition includes damage formulas, hit mechanics, status effects, animations, and sound effects.

## Contents

| File | Size | Description |
|------|------|-------------|
| `SkillInfo.cs` | 5 KB | Entry point, routing logic, and export methods |
| `SkillsPMD.cs` | 771 KB | Moves 0-467 (Gen 1-4, PMD-era moves) |
| `SkillsGen5Plus.cs` | 495 KB | Moves 468-900 (Gen 5+ moves) |

**Total:** ~901 moves defined

## Move Definition Structure

Each move is defined in a large switch statement within `FillSkillsPMD()` or `FillSkillsGen5Plus()`:

```csharp
else if (ii == 1)
{
    skill.Name = new LocalText("Pound");
    skill.Desc = new LocalText("The target is physically pounded with a long tail, a foreleg, or the like.");
    skill.BaseCharges = 25;                    // PP equivalent
    skill.Data.Element = "normal";             // Type
    skill.Data.Category = BattleData.SkillCategory.Physical;
    skill.Data.SkillStates.Set(new ContactState());  // Makes contact
    skill.Data.HitRate = 100;                  // Accuracy
    skill.Data.SkillStates.Set(new BasePowerState(45));  // Base power

    // Damage calculation
    skill.Data.OnHits.Add(-1, new DamageFormulaEvent());

    skill.Strikes = 1;                         // Number of hits

    // Animation and hitbox
    skill.HitboxAction = new AttackAction();
    ((AttackAction)skill.HitboxAction).CharAnimData = new CharAnimFrameType(08); // Strike animation
    ((AttackAction)skill.HitboxAction).HitTiles = true;
    skill.HitboxAction.TargetAlignments = Alignment.Foe;
    skill.Explosion.TargetAlignments = Alignment.Foe;

    // Sound effect
    BattleFX preFX = new BattleFX();
    preFX.Sound = "DUN_Pound";
    skill.HitboxAction.PreActions.Add(preFX);
}
```

## Key Properties

### Basic Stats

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `LocalText` | Move name |
| `Desc` | `LocalText` | Move description |
| `BaseCharges` | `int` | PP (0 = unlimited like basic Attack) |
| `Data.Element` | `string` | Type (fire, water, etc.) |
| `Data.Category` | `SkillCategory` | Physical, Special, or Status |
| `Data.HitRate` | `int` | Accuracy percentage |
| `Strikes` | `int` | Number of hits |

### Skill States

```csharp
skill.Data.SkillStates.Set(new BasePowerState(80));    // Base power
skill.Data.SkillStates.Set(new ContactState());         // Makes contact
skill.Data.SkillStates.Set(new SoundState());          // Sound-based move
skill.Data.SkillStates.Set(new PunchState());          // Punching move
skill.Data.SkillStates.Set(new JawState());            // Biting move
```

### Event Handlers

| Event | Timing | Example Use |
|-------|--------|-------------|
| `OnActions` | Before move executes | Boost crit rate, charge effects |
| `BeforeHits` | Before damage calc | Type effectiveness mods |
| `OnHits` | When damage dealt | Apply status, secondary effects |
| `AfterActions` | After move completes | Recoil, stat drops |

## Common Move Patterns

### Status Move

```csharp
skill.Data.Category = BattleData.SkillCategory.Status;
skill.Data.HitRate = 100;
skill.Data.OnHits.Add(0, new StatusBattleEvent("sleep", true, false));
```

### Multi-Hit Move

```csharp
skill.Strikes = 5;  // Hits 5 times
skill.Data.OnActions.Add(0, new RandomGroupEvent(new RandRange(2, 6))); // 2-5 hits
```

### Boosted Crit Move

```csharp
skill.Data.OnActions.Add(0, new BoostCriticalEvent(1));  // +1 crit stage
```

### Recoil Move

```csharp
skill.Data.AfterActions.Add(0, new HPRecoilEvent(3));  // 1/3 recoil
```

### Stat-Boosting Move

```csharp
skill.Data.OnHits.Add(0, new StatusStackBattleEvent("mod_attack", true, false, 1));
```

### Weather-Setting Move

```csharp
skill.Data.OnHits.Add(0, new GiveMapStatusEvent("rain"));
```

## Hitbox Types

| Type | Usage |
|------|-------|
| `AttackAction` | Melee, single target |
| `ProjectileAction` | Ranged projectile |
| `AreaAction` | Area of effect |
| `ThrowAction` | Throw-based attacks |
| `SelfAction` | Self-targeting |
| `DashAction` | Charge/dash attacks |

## Adding a New Move

### Using SkillBuilder (Recommended)

The `SkillBuilder` fluent API reduces boilerplate from ~30 lines to ~5-8 lines:

```csharp
else if (ii == 902)  // New move
{
    SkillBuilder.Physical("Custom Move")
        .Desc("A powerful new attack.")
        .Charges(15).Element("psychic").Power(90).Accuracy(95)
        .Contact().Melee(CharAnim.Strike)
        .UseSound("DUN_Attack")
        .ApplyTo(skill);
}
```

**Factory Methods:**
| Method | Category | Defaults |
|--------|----------|----------|
| `Physical(name)` | Physical | HitRate=100, adds DamageFormulaEvent |
| `Special(name)` | Magical | HitRate=100, adds DamageFormulaEvent |
| `Status(name)` | Status | HitRate=-1 (always hits) |

**Hitbox Actions:**
| Method | Action Type | Description |
|--------|-------------|-------------|
| `Melee(anim)` | AttackAction | Single-target melee |
| `MeleeWide(anim)` | AttackAction | Hits sides too |
| `MeleeAround(anim)` | AttackAction | Hits all around |
| `Projectile(anim, range, speed)` | ProjectileAction | Ranged attack |
| `Area(anim, range)` | AreaAction | Area of effect |
| `Cone(anim, range)` | AreaAction | Cone-shaped area |
| `Dash(range)` | DashAction | Charge attack |
| `Self(anim)` | SelfAction | Self-targeting |

**Common Methods:**
- `.Charges(n)` - Base PP
- `.Element("type")` - Elemental type
- `.Power(n)` - Base power
- `.Accuracy(n)` - Hit rate (default 100 for attacks)
- `.Strikes(n)` - Multi-hit count
- `.Contact()` - Makes contact with target
- `.Fist()`, `.Jaw()`, `.Blade()`, `.Sound()` - Move categories
- `.HighCrit(stages)` - Increased crit rate
- `.InflictStatus("status", chance)` - Status effect
- `.StatChange("mod_x", stages, targetSelf)` - Stat modification
- `.Recoil(divisor)` - Recoil damage
- `.Knockback(tiles)` - Push target back
- `.UseSound("sound")`, `.PreSound("sound")` - Sound effects
- `.Emitter("anim", frameTime)` - Visual effects

### Traditional Method

For complex moves or those needing features not yet in SkillBuilder:

```csharp
else if (ii == 902)  // New move
{
    skill.Name = new LocalText("Custom Move");
    skill.Desc = new LocalText("A powerful new attack.");
    skill.BaseCharges = 15;
    skill.Data.Element = "psychic";
    skill.Data.Category = BattleData.SkillCategory.Special;
    skill.Data.HitRate = 95;
    skill.Data.SkillStates.Set(new BasePowerState(90));
    skill.Data.OnHits.Add(-1, new DamageFormulaEvent());
    skill.Strikes = 1;

    skill.HitboxAction = new ProjectileAction();
    // ... configure hitbox
}
```

4. Run `DataGenerator -dump skill`

## Name Prefixes

Special prefixes in move names indicate status:

| Prefix | Meaning |
|--------|---------|
| `**` | Unreleased (data exists but not available) |
| `-` | Missing animation |
| `=` | Missing sound |

These prefixes are stripped during export and set corresponding flags.

## Notes for Modders

- Move indices must be unique across both files
- The `Released` flag controls in-game availability
- Test moves thoroughly - complex effects may interact unexpectedly
- Animation IDs (CharAnimFrameType) must match sprite sheet frames
- Sound IDs must match audio files in the game assets
