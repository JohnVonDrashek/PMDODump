# Zones

Dungeon and area definitions for PMD: Origins.

## Overview

This directory contains the procedural generation rules and content definitions for all explorable areas in the game. Each zone defines floor layouts, item spawns, enemy encounters, traps, weather conditions, and special events. The code uses RogueEssence's layered generation system to create reproducible randomized dungeons.

## Contents

| File | Size | Description |
|------|------|-------------|
| `ZoneInfo.cs` | 391 KB | Main zone entry point, story dungeons (Tropical Path, etc.) |
| `ZoneInfoBase.cs` | 270 KB | Debug zone and hub area definitions |
| `ZoneInfoOptional.cs` | 748 KB | Optional/side dungeons (Bramble Woods, etc.) |
| `ZoneInfoPostgame.cs` | 128 KB | Post-game dungeon content |
| `ZoneInfoChallenge.cs` | 206 KB | Challenge mode dungeons |
| `ZoneInfoRogue.cs` | 299 KB | Roguelike mode dungeons |
| `ZoneInfoTables.cs` | 162 KB | Shared spawn tables and probability weights |
| `ZoneInfoLists.cs` | 40 KB | Item and monster list definitions |
| `ZoneInfoHelpers.cs` | 59 KB | Utility methods and generation priorities |
| `MapInfo.cs` | 204 KB | Static map definitions (towns, cutscene areas) |

## Architecture

### Partial Class Structure

Zone definitions use C# partial classes to split the large `ZoneInfo` class across multiple files:

```csharp
// ZoneInfo.cs
public partial class ZoneInfo
{
    public static void AddZoneData(bool translate, params int[] zonesToAdd) { ... }
    public static ZoneData GetZoneData(int index, bool translate) { ... }
}

// ZoneInfoOptional.cs
public partial class ZoneInfo
{
    static void FillBrambleWoods(ZoneData zone, bool translate) { ... }
}
```

### Generation Priority System

Floor generation uses a priority queue to order generation steps:

| Priority | Constant | Description |
|----------|----------|-------------|
| -7 | `PR_FILE_LOAD` | Load prebuilt map files |
| -6 | `PR_FLOOR_DATA` | Set floor name, ID, metadata |
| -5 | `PR_GRID_INIT` | Initialize room grid |
| -4 | `PR_GRID_GEN` | Generate room connections |
| -3 | `PR_ROOMS_INIT` | Convert grid to freeform layout |
| -2 | `PR_ROOMS_GEN` | Generate room shapes |
| -1 | `PR_TILES_INIT` | Initialize tile array |
| 0 | `PR_TILES_GEN` | Draw floors and walls |
| 1 | `PR_RESPAWN_*` | Configure respawn rules |
| 2 | `PR_EXITS` | Place stairs and exits |
| 3 | `PR_WATER` | Add water/lava terrain |
| 4 | `PR_TEXTURES` | Apply visual textures |
| 5 | `PR_SPAWN_TRAPS` | Place traps |
| 6 | `PR_SPAWN_*` | Spawn items, money, enemies |
| 7 | `PR_DBG_CHECK` | Debug validation |

## Creating a New Dungeon

### Basic Structure

```csharp
static void FillMyDungeon(ZoneData zone)
{
    zone.Name = new LocalText("My Dungeon");
    zone.Rescues = 2;           // Max rescue attempts
    zone.Level = 20;            // Recommended level
    zone.Rogue = RogueStatus.NoTransfer;  // Item transfer rules

    int max_floors = 10;

    LayeredSegment floorSegment = new LayeredSegment();
    floorSegment.IsRelevant = true;
    floorSegment.ZoneSteps.Add(new SaveVarsZoneStep(PR_EXITS_RESCUE));
    floorSegment.ZoneSteps.Add(new FloorNameDropZoneStep(
        PR_FLOOR_DATA,
        new LocalText("My Dungeon\nB{0}F"),
        new Priority(-15)
    ));

    // Add money spawning
    MoneySpawnZoneStep moneySpawnZoneStep = GetMoneySpawn(zone.Level, 0);
    floorSegment.ZoneSteps.Add(moneySpawnZoneStep);

    // Add item spawning
    ItemSpawnZoneStep itemSpawnZoneStep = new ItemSpawnZoneStep();
    itemSpawnZoneStep.Priority = PR_RESPAWN_ITEM;
    // ... configure item pools

    // Add enemy spawning
    // ... configure monster pools

    // Add floor layouts
    for (int ii = 0; ii < max_floors; ii++)
    {
        // Configure each floor
    }

    zone.Segments.Add(floorSegment);
}
```

### Adding Items to a Floor

```csharp
CategorySpawn<InvItem> necessities = new CategorySpawn<InvItem>();
necessities.SpawnRates.SetRange(14, new IntRange(0, max_floors));
itemSpawnZoneStep.Spawns.Add("necessities", necessities);

necessities.Spawns.Add(new InvItem("berry_oran"), new IntRange(0, max_floors), 12);
necessities.Spawns.Add(new InvItem("food_apple"), new IntRange(0, max_floors), 10);
necessities.Spawns.Add(new InvItem("seed_reviver"), new IntRange(0, max_floors), 5);
```

### Adding Enemies to a Floor

```csharp
PoolTeamSpawner poolSpawn = new PoolTeamSpawner();
poolSpawn.Spawns.Add(GetTeamMob("rattata", "", "tackle", "tail_whip", "", "", new RandRange(5)), 10);
poolSpawn.Spawns.Add(GetTeamMob("pidgey", "", "tackle", "sand_attack", "", "", new RandRange(5)), 10);
```

### Floor Layout Types

- **GridFloorGen** - Room-based grid layout
- **StairsFloorGen** - Linear staircase progression
- **LoadGen** - Load premade map files
- **RoomFloorGen** - Freeform room placement

## Key Helper Methods

From `ZoneInfoHelpers.cs`:

| Method | Purpose |
|--------|---------|
| `GetMoneySpawn(level, tier)` | Configure Poke spawning |
| `GetTeamMob(species, ability, moves...)` | Create enemy spawn entry |
| `AddFloorData(layout, music, darkness...)` | Set floor properties |
| `AddTitleDrop(layout)` | Add floor title display |
| `GetGenericMobSpawn(floor, difficulty)` | Generate level-appropriate enemies |

## Spawn Rate Tuning

Spawn rates use weighted probability. Higher numbers = more common:

```csharp
// Oran Berry: weight 12 (common)
necessities.Spawns.Add(new InvItem("berry_oran"), new IntRange(0, max_floors), 12);

// Reviver Seed: weight 5 (rare)
necessities.Spawns.Add(new InvItem("seed_reviver"), new IntRange(0, max_floors), 5);
```

## Notes for Modders

- Zone indices 0-54 are used by the base game
- Use high indices (100+) for custom dungeons to avoid conflicts
- Test floor generation with debug mode to verify layouts
- Balance item spawns based on dungeon difficulty
- Remember to register new zones in `GetZoneData()` switch statement
- Use `translate` parameter to control localization string loading
