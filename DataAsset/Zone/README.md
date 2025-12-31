# Zone

Dungeon zone configuration files defining floor layouts, item spawns, and monster encounters.

## Overview

This directory contains C#-style configuration snippets for each dungeon zone in PMD: Origins. These files define item spawn tables, monster encounters, floor generation parameters, and other zone-specific data. Files are synced using `Scripts/zone_sync.py`.

**Note**: All files in this directory ending in `.out.txt` are generated output. The source file `PreZone.txt` provides reference data for zone generation.

## Contents

### Source File

| File | Description |
|------|-------------|
| `PreZone.txt` | Zone generation reference data and templates |

### Generated Zone Files (`.out.txt`)

Each dungeon has a corresponding output file:

| File | Dungeon |
|------|---------|
| `Copper_Quarry.out.txt` | Copper Quarry (starter dungeon) |
| `Bramble_Woods.out.txt` | Bramble Woods |
| `Fertile_Valley.out.txt` | Fertile Valley |
| `Guildmaster_Trail.out.txt` | Guildmaster Trail |
| `Sleeping_Caldera.out.txt` | Sleeping Caldera |
| ... | (50+ zones) |

## Zone File Format

Zone files use C# code syntax defining spawn tables and floor configurations:

### Item Spawn Section

```csharp
//items
ItemSpawnZoneStep itemSpawnZoneStep = new ItemSpawnZoneStep();
itemSpawnZoneStep.Priority = PR_RESPAWN_ITEM;

//necessities
CategorySpawn<InvItem> necessities = new CategorySpawn<InvItem>();
necessities.SpawnRates.SetRange(14, new IntRange(0, max_floors));
itemSpawnZoneStep.Spawns.Add("necessities", necessities);

necessities.Spawns.Add(new InvItem("berry_leppa"), new IntRange(0, max_floors), 9);
necessities.Spawns.Add(new InvItem("berry_oran"), new IntRange(0, max_floors), 6);
```

### Monster Spawn Section

```csharp
//mobs
TeamSpawnZoneStep poolSpawn = new TeamSpawnZoneStep();
poolSpawn.Priority = PR_RESPAWN_MOB;

poolSpawn.Spawns.Add(GetTeamMob("mawile", "", "iron_head", "taunt", "", "",
    new RandRange(24), "wander_dumb"), new IntRange(3, max_floors), 10);
poolSpawn.Spawns.Add(GetTeamMob("aron", "", "metal_claw", "harden", "", "",
    new RandRange(24), "wander_dumb"), new IntRange(0, max_floors), 10);
```

## Item Categories

| Category | Description | Example Items |
|----------|-------------|---------------|
| `necessities` | Essential consumables | Oran Berry, Leppa Berry, Apple |
| `snacks` | Seeds and type berries | Ban Seed, Blast Seed, Shuca Berry |
| `boosters` | Stat-boosting items | Gummis |
| `special` | Utility items | Apricorns, Assembly Box |
| `throwable` | Projectile items | Geo Pebble, Iron Thorn, Wands |
| `orbs` | Room/floor effect orbs | Trawl Orb, Weather Orb |
| `held` | Equipment items | Expert Belt, Metronome |
| `tms` | Technical Machines | TM Round, TM Dig |

## Spawn Configuration

### Spawn Rates

```csharp
// Syntax: category.SpawnRates.SetRange(rate, new IntRange(startFloor, endFloor));
necessities.SpawnRates.SetRange(14, new IntRange(0, max_floors));
```

### Item Spawns

```csharp
// Syntax: Spawns.Add(item, floor_range, weight);
necessities.Spawns.Add(new InvItem("berry_leppa"), new IntRange(0, max_floors), 9);

// With quantity:
throwable.Spawns.Add(new InvItem("ammo_geo_pebble", false, 3), new IntRange(0, max_floors), 10);
```

### Monster Spawns

```csharp
// Syntax: GetTeamMob(species, ability, move1, move2, move3, move4, level, ai_type)
poolSpawn.Spawns.Add(GetTeamMob("mawile", "", "iron_head", "taunt", "", "",
    new RandRange(24), "wander_dumb"), new IntRange(3, max_floors), 10);

// With form specification:
poolSpawn.Spawns.Add(GetTeamMob(new MonsterID("grimer", 1, "", Gender.Unknown),
    "", "bite", "poison_fang", "", "", new RandRange(26), "wander_dumb"),
    new IntRange(7, max_floors), 10);
```

## Workflow

### Modifying Zone Configuration

1. Edit the desired `***.out.txt` file or work with zone generation scripts
2. Adjust spawn rates, floor ranges, and weights
3. Test changes in-game

### Adding New Items to a Zone

1. Add spawn entry to appropriate category section
2. Set floor range with `new IntRange(startFloor, endFloor)`
3. Assign spawn weight (higher = more common)

### Balancing Monster Encounters

1. Adjust level ranges: `new RandRange(minLevel)` or `new RandRange(minLevel, maxLevel)`
2. Modify floor ranges for encounter availability
3. Tune spawn weights for frequency

## Common Dungeon Zones

| Zone | Description | Floors |
|------|-------------|--------|
| `Copper_Quarry` | Early-game mining dungeon | ~10 |
| `Guildmaster_Trail` | Tutorial/story dungeon | Variable |
| `Sleeping_Caldera` | Volcanic area | ~15 |
| `Snowbound_Path` | Ice-themed dungeon | ~12 |
| `Secret_Garden` | Hidden area with rare Pokemon | ~20 |

## Related Files

- `DataAsset/Monster/releases.txt` - Monster spawn locations reference
- `DataAsset/String/Zone.txt` - Zone name localizations
- `Scripts/zone_sync.py` - Sync script
- `Scripts/zoneGen.py` - Zone generation utilities
