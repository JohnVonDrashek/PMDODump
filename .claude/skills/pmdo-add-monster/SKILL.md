---
name: pmdo-add-monster
description: Use when adding a new monster/Pokemon species to PMD Origins, including its data definition and zone spawn entries.
---

# Adding a Monster to PMD: Origins

## Monster Data Definition

The primary monster data lives in `DataGenerator/Data/MonsterInfo.cs`. Each monster entry requires:

- **MonsterData fields**: `IndexNum`, `Name`, `Title`, `JoinRate`, `EXPTable`, `SkillGroup1`, `SkillGroup2`
- **MonsterFormData** for each form: stats, types, abilities, level-up moves, height, weight, etc.

Monster data is generated from `DataGenerator/Data/Monster/pokedex.9.sqlite` database. For custom monsters, add entries in `CreateDex()` or create a new method following the `AddMinMonsterData()` pattern.

## Zone Spawn Tables

To make a monster appear in dungeons, add spawn entries in zone files under `DataGenerator/Data/Zones/`:

```csharp
// Pattern: GetTeamMob(species, ability, move1-4, level, tactic)
poolSpawn.Spawns.Add(
    GetTeamMob("species_name", "ability_id", "move1", "move2", "", "",
        new RandRange(level), "wander_dumb"),
    new IntRange(startFloor, endFloor),
    spawnWeight
);
```

Zone files are split by category:
- `ZoneInfo.cs` - Story dungeons
- `ZoneInfoPostgame.cs` - Post-credits content
- `ZoneInfoOptional.cs` - Side dungeons
- `ZoneInfoChallenge.cs` - Challenge modes

## Checklist

- [ ] **MonsterInfo.cs**: Define `MonsterData` with forms, stats, types, abilities, moves
- [ ] **releases.out.txt**: Set `Released = true` for available forms (`DataGenerator/Data/Monster/`)
- [ ] **personality_in.txt**: Assign personality traits (`DataGenerator/Data/Monster/`)
- [ ] **Zone files**: Add `GetTeamMob()` spawn entries in appropriate zones
- [ ] **DumpAsset/**: Compile assets after changes (submodule)

## Key References

- Species ID: lowercase name (e.g., `"pikachu"`, `"mr_mime"`)
- Ability ID: lowercase with underscores (e.g., `"natural_cure"`, `"compound_eyes"`)
- Move ID: lowercase with underscores (e.g., `"thunder_shock"`, `"quick_attack"`)
- Tactic options: `"wander_dumb"`, `"wander_normal"`, `"slow_patrol"`, `"weird_tree"`
