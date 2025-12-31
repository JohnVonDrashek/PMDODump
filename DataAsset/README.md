# DataAsset

Human-editable source files that sync to game data via Python scripts.

## Overview

This directory contains spreadsheets, text files, and configuration data that serve as the authoritative source for game content. These files are designed to be human-readable and editable, then synced to game data formats using scripts in `Scripts/`.

**Important**: Files ending in `.out.txt` are generated output. Do not edit them directly - your changes will be overwritten. Edit the source files (without `.out`) instead.

## Contents

| Subdirectory | Description | Primary Files |
|--------------|-------------|---------------|
| [Docs/](./Docs/) | Lua scripting API documentation | `.txt` files documenting script functions |
| [Item/](./Item/) | Item configuration and exclusive item data | Evolution trees, auto-item references, exclusive effects |
| [Monster/](./Monster/) | Monster/Pokemon data and spawn configuration | Personality quiz, release tracking, Pokedex database |
| [String/](./String/) | Localization strings for all game text | `.txt` source and `.out.txt` generated pairs |
| [Zone/](./Zone/) | Dungeon zone configurations | Floor layouts, item spawns, monster spawns |

## Workflow

### For Developers

1. Edit source files in this directory (never `.out.txt` files)
2. Run the appropriate sync script from `Scripts/`:
   ```bash
   # Examples:
   uv run Scripts/string_sync.py    # Sync localization strings
   uv run Scripts/zone_sync.py      # Sync zone configurations
   uv run Scripts/item_sync.py      # Sync item data
   uv run Scripts/monster_sync.py   # Sync monster data
   ```
3. Generated `.out.txt` files will be updated automatically
4. Commit both source and generated files together

### For Modders/Translators

1. **Localization**: Edit files in `String/` directory - add translations in the appropriate language column
2. **Zone Balancing**: Modify spawn rates and item tables in `Zone/` files
3. **Monster Data**: Adjust encounter data in `Monster/releases.txt`

### Sync Scripts Reference

| Script | Purpose | Input Directory |
|--------|---------|-----------------|
| `string_sync.py` | Compile localization strings | `String/` |
| `zone_sync.py` | Generate zone definitions | `Zone/` |
| `item_sync.py` | Sync item data | `Item/` |
| `monster_sync.py` | Sync monster data | `Monster/` |
| `doc_sync.py` | Generate API documentation | `Docs/` |

## File Format Notes

- Tab-separated values (TSV) are used for most spreadsheet data
- UTF-8 encoding with support for CJK characters
- Empty cells indicate missing translations or optional fields
- Comment lines typically start with `//`
