# Item

Item configuration spreadsheets for exclusive items, evolution data, and automatic item generation.

## Overview

This directory contains source data for item-related game mechanics including exclusive item effects, evolution tree references, and auto-generation parameters. Files are synced to game data using `Scripts/item_sync.py`.

## Contents

| File | Type | Description |
|------|------|-------------|
| `AutoItemRef.txt` | Source | Automatic exclusive item effect descriptions with placeholders |
| `EvoTreeRef.txt` | Source | Evolution tree data with personality compatibility flags |
| `EvoTreeRef_prev.txt` | Backup | Previous version of evolution tree data |
| `ExclusiveItem.out.txt` | Generated | Compiled exclusive item data (do not edit) |
| `PreAutoItem.txt` | Source | Reference tables for item generation (types, effects, stats, monsters, elements, statuses) |

## File Formats

### AutoItemRef.txt

Tab-separated columns defining exclusive item effects:

| Column | Description |
|--------|-------------|
| Rarity | Star rating (0-5*) |
| EffectType | Effect ID and name (e.g., `001: TypeStatBonus`) |
| Description | Effect text with placeholders (`{0}`, `{1}` for type/stat) |

Example:
```
1*  001: TypeStatBonus  When kept in the bag, it slightly boosts the {1} of {0}-type members.
```

### EvoTreeRef.txt

Evolution tree compatibility matrix with personality quiz traits:

- Column 1: Pokedex number
- Column 2: Form number
- Remaining columns: Personality trait flags (`x` = compatible)

Used to determine which starter Pokemon match quiz personality results.

### PreAutoItem.txt

Reference lookup tables in parallel columns:

| Column | Description |
|--------|-------------|
| ItemType | Item type categories (Beam, Branch, Claw, etc.) |
| EffectType | All possible exclusive item effects |
| Stat | Stats that can be modified |
| Monster | All monster IDs (for species-specific items) |
| Element | Pokemon types |
| Status | Status conditions |
| MapStatus | Map-wide effects |
| Category | Move categories (Physical, Magical, Status) |

## Workflow

### Editing Exclusive Item Effects

1. Modify descriptions in `AutoItemRef.txt`
2. Use `{0}`, `{1}` placeholders for dynamic content (type, stat, etc.)
3. Run sync script:
   ```bash
   uv run Scripts/item_sync.py
   ```

### Adding New Items

1. Add new effect types to `PreAutoItem.txt`
2. Create corresponding effect description in `AutoItemRef.txt`
3. Sync to generate `ExclusiveItem.out.txt`

### Evolution Tree Updates

1. Edit `EvoTreeRef.txt` to modify starter compatibility
2. Back up to `EvoTreeRef_prev.txt` if making major changes
3. Flags determine which personalities can select which starters

## Related Files

- `DataAsset/String/ExclusiveItemEffect.txt` - Localized effect names
- `DataAsset/String/ExclusiveItemType.txt` - Localized item type names
- `Scripts/item_sync.py` - Sync script
- `Scripts/itemGen.py` - Item generation utilities
