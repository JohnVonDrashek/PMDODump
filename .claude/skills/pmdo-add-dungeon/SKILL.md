---
name: pmdo-add-dungeon
description: Use when adding a new dungeon zone to PMD: Origins, including floor configuration, monster spawns, items, and localization
---

# Adding Dungeons to PMD: Origins

## Zone Structure

Dungeons are defined in `DataGenerator/Data/Zones/ZoneInfo*.cs` files. Each zone requires:

1. **Zone metadata**: Name, level, rescues, exp percent, rogue status
2. **Floor segments**: LayeredSegment with floors, zone steps, and spawns
3. **Per-floor layouts**: GridFloorGen or ListFloorGen with generation steps

## Quick Start Template

```csharp
zone.Name = new LocalText("Dungeon Name");
zone.Rescues = 2;
zone.Level = 15;  // Base level for scaling
zone.ExpPercent = 80;
zone.Rogue = RogueStatus.NoTransfer;

int max_floors = 5;
LayeredSegment floorSegment = new LayeredSegment();
floorSegment.IsRelevant = true;
floorSegment.ZoneSteps.Add(new SaveVarsZoneStep(PR_EXITS_RESCUE));
floorSegment.ZoneSteps.Add(new FloorNameDropZoneStep(PR_FLOOR_DATA,
    new LocalText("Dungeon Name\n{0}F"), new Priority(-15)));

// Money spawning
MoneySpawnZoneStep moneySpawnZoneStep = GetMoneySpawn(zone.Level, 0);
floorSegment.ZoneSteps.Add(moneySpawnZoneStep);
```

## DungeonStage Enum

- `Beginner` - Early game (levels 1-15)
- `Intermediate` - Mid game (levels 15-35)
- `Advanced` - Late game (levels 35-60)
- `PostGame` - Post-credits content
- `Rogue` - Special roguelike mode

## Floor Configuration

```csharp
GridFloorGen layout = new GridFloorGen();
AddFloorData(layout, "Music.ogg", 1500, Map.SightRange.Clear, Map.SightRange.Clear);
AddTextureData(layout, "wall_tileset", "floor_tileset", "water_tileset", "element");
AddInitGridStep(layout, 3, 2, 10, 10);  // cols, rows, cellW, cellH
AddDrawGridSteps(layout);
AddStairStep(layout, false);  // false=down stairs
```

## Monster Spawns

```csharp
TeamSpawnZoneStep poolSpawn = new TeamSpawnZoneStep();
poolSpawn.Priority = PR_RESPAWN_MOB;
poolSpawn.Spawns.Add(GetTeamMob("species_id", "ability", "move1", "move2",
    "move3", "move4", new RandRange(level), "tactic"),
    new IntRange(startFloor, endFloor), weight);
poolSpawn.TeamSizes.Add(1, new IntRange(0, max_floors), 12);
floorSegment.ZoneSteps.Add(poolSpawn);

AddRespawnData(layout, maxFoes, respawnTime);
AddEnemySpawnData(layout, clumpFactor, new RandRange(min, max));
```

## Item Distribution

```csharp
ItemSpawnZoneStep itemSpawnZoneStep = new ItemSpawnZoneStep();
itemSpawnZoneStep.Priority = PR_RESPAWN_ITEM;

CategorySpawn<InvItem> category = new CategorySpawn<InvItem>();
category.SpawnRates.SetRange(10, new IntRange(0, max_floors));
category.Spawns.Add(new InvItem("item_id"), new IntRange(0, max_floors), weight);
itemSpawnZoneStep.Spawns.Add("category_name", category);
floorSegment.ZoneSteps.Add(itemSpawnZoneStep);

AddItemData(layout, new RandRange(2, 4), 25);  // amount, success%
```

## Key Helper Methods

| Method | Purpose |
|--------|---------|
| `AddFloorData` | Music, time limit, sight ranges |
| `AddTextureData` | Wall/floor/water tilesets |
| `AddInitGridStep` | Grid dimensions |
| `AddWaterSteps` | Add water terrain |
| `AddTrapsSteps` | Spawn traps |
| `AddEvoZoneStep` | Evolution altars |
| `AddHiddenStairStep` | Secret stairs |
| `GetTeamMob` | Create enemy spawn |
| `GetMoneySpawn` | Level-scaled money |

## Common Tactics

- `wander_dumb` - Random movement
- `wander_normal` - Standard AI
- `wander_smart` - Advanced AI
- `wait_only` - Stationary
- `boss` - Boss behavior

## Files Reference

- Zone definitions: `DataGenerator/Data/Zones/ZoneInfo*.cs`
- Helper methods: `DataGenerator/Data/Zones/ZoneInfoHelpers.cs`
- Item/trap tables: `DataGenerator/Data/Zones/ZoneInfoTables.cs`
- Localization: `DataGenerator/Dev/Localization.cs`
