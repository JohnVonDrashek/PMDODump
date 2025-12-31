# Data

C# source definitions for all game content in PMD: Origins.

## Overview

This directory contains the authoritative source code for defining Pokemon, moves, abilities, items, status effects, and other game mechanics. Each file exports a static class with `Add*Data()` methods that generate serialized game data. The definitions use RogueEssence and PMDC APIs to construct complex game objects with behaviors, animations, and effects.

## Contents

| File | Description | Entry Count |
|------|-------------|-------------|
| `MonsterInfo.cs` | Pokemon species with stats, types, evolutions, learnsets | ~1011 species |
| `ItemInfo.cs` | Consumables, equipment, and held items | ~2500 items |
| `IntrinsicInfo.cs` | Pokemon abilities (e.g., Intimidate, Levitate) | ~299 abilities |
| `StatusInfo.cs` | Status conditions (Sleep, Poison, stat changes) | ~150 statuses |
| `MapStatusInfo.cs` | Map-wide effects (weather, terrain, traps) | - |
| `TileInfo.cs` | Dungeon tile types and terrain properties | - |
| `AIInfo.cs` | NPC behavior patterns and combat AI | - |
| `AutoItemInfo.cs` | Procedurally generated exclusive items | - |
| `DataInfo.cs` | Universal game settings, type chart, global events | - |
| `ElementInfo.cs` | Type definitions (Fire, Water, etc.) | 18 types |
| `EmoteInfo.cs` | Character emotion animations | - |
| `GrowthInfo.cs` | Experience curve definitions | - |
| `RankInfo.cs` | Rescue team rank progression | - |
| `SkillGroupInfo.cs` | Move learning group categories | - |
| `SkinInfo.cs` | Shiny and alternate form variants | - |
| `ZoneInfoLists.cs` | Shared spawn tables and item pools for zones | - |
| `Skills/` | Move and attack definitions (split by generation) | ~901 moves |
| `Zones/` | Dungeon and area definitions | ~55 zones |

## Adding New Content

### General Pattern

Each content type follows this structure:

```csharp
public static class ExampleInfo
{
    public const int MAX_EXAMPLES = 100;

    public static void AddExampleData()
    {
        DataInfo.DeleteIndexedData(DataManager.DataType.Example.ToString());
        for (int ii = 0; ii < MAX_EXAMPLES; ii++)
        {
            (string, ExampleData) example = GetExampleData(ii);
            if (example.Item1 != "")
                DataManager.SaveEntryData(example.Item1, DataManager.DataType.Example.ToString(), example.Item2);
        }
    }

    public static (string, ExampleData) GetExampleData(int ii)
    {
        string fileName = "";
        ExampleData data = new ExampleData();

        if (ii == 0)
        {
            data.Name = new LocalText("Example Name");
            data.Desc = new LocalText("Description text.");
            // ... additional properties
        }

        if (fileName == "")
            fileName = Text.Sanitize(data.Name.DefaultText).ToLower();
        return (fileName, data);
    }
}
```

### Adding a New Item

1. Open `ItemInfo.cs`
2. Find an unused index in `GetItemData()`
3. Add a new `else if` block:

```csharp
else if (ii == 123)
{
    item.Name = new LocalText("My Custom Item");
    item.Desc = new LocalText("Does something cool.");
    item.Sprite = "Item_Sprite_Name";
    item.Price = 100;
    item.UseEvent.OnHits.Add(0, new SomeEffect());
}
```

4. Run `DataGenerator -dump item`

### Adding a New Status Effect

1. Open `StatusInfo.cs`
2. Add to `GetStatusData()`:

```csharp
else if (ii == 75)
{
    status.Name = new LocalText("Dazed");
    fileName = "dazed";
    status.MenuName = true;
    status.Desc = new LocalText("The Pokemon is confused and may hurt itself.");
    status.Emoticon = "Confused";
    status.StatusStates.Set(new BadStatusState());
    // Add behavior events...
}
```

### Adding a New Ability

1. Open `IntrinsicInfo.cs`
2. Add to `GetIntrinsicData()`:

```csharp
else if (ii == 200)
{
    ability.Name = new LocalText("Custom Ability");
    ability.Desc = new LocalText("Boosts power in certain conditions.");
    ability.OnActions.Add(0, new MultiplyDamageEvent(1.5f));
}
```

## Key Concepts

### LocalText

All user-facing strings use `LocalText` for localization support:

```csharp
new LocalText("English Text")
```

### Events and Behaviors

Game mechanics are implemented through event handlers:

- `OnHits` - Triggered when damage is dealt
- `OnActions` - Triggered when using a move
- `BeforeHits` - Triggered before damage calculation
- `OnRefresh` - Triggered on stat recalculation
- `OnDeaths` - Triggered when a Pokemon faints

### Element Types

Reference types by string ID:

```csharp
skill.Data.Element = "fire";
item.UseEvent.Element = "none";
```

## Dependencies

The data definitions rely heavily on:

- **RogueEssence.Data** - Base data types
- **RogueEssence.Dungeon** - Combat and dungeon mechanics
- **PMDC.Dungeon** - PMD-specific events and effects
- **PMDC.Data** - PMD data structures

## Notes for Modders

- Indices are arbitrary but must be unique within each data type
- Use the `**` prefix in names to mark unreleased content
- Use the `-` prefix in names to mark content missing animations
- Run `-dump <type>` after any changes to regenerate data
- Test changes in-game before committing
