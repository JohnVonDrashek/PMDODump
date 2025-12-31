# Scripts

[![Python](https://img.shields.io/badge/python-3.7+-blue.svg)](https://www.python.org/downloads/)
[![License](https://img.shields.io/badge/license-GPL--3.0-green.svg)](../LICENSE)

Python utilities for **PMD: Origins** (Pokemon Mystery Dungeon roguelike) data synchronization, asset processing, and code generation. These scripts bridge Google Sheets data with game files, process sprites and tilesets, and generate game content.

## Setup

### Prerequisites

- Python 3.7+
- Google Cloud Platform project with Sheets API enabled
- OAuth 2.0 credentials (`client_secret.json`)

### Installation

Using [uv](https://github.com/astral-sh/uv) (recommended):

```bash
# Create virtual environment
uv venv

# Activate the environment
source .venv/bin/activate  # Unix/macOS
# or
.venv\Scripts\activate     # Windows

# Install dependencies
uv pip install httplib2 google-api-python-client oauth2client Pillow requests
```

### Google Sheets Authentication

1. Create a project in [Google Cloud Console](https://console.cloud.google.com/)
2. Enable the Google Sheets API
3. Create OAuth 2.0 credentials (Desktop application)
4. Download `client_secret.json` to the `Scripts/` directory
5. Create sheet ID files for each sync script (e.g., `string_sheet_id.txt`, `item_sheet_id.txt`)
   - Each file contains just the Google Sheet ID on the first line

On first run, each sync script will open a browser for OAuth authentication. Credentials are cached in `.credentials/`.

## Contents

### Data Sync Scripts

These scripts synchronize game data between Google Sheets and local files.

| Script | Description | Sheet ID File |
|--------|-------------|---------------|
| `string_sync.py` | Syncs all localization strings (hardcode, scripts, data) | `string_sheet_id.txt` |
| `item_sync.py` | Syncs exclusive item data and evolution trees | `item_sheet_id.txt` |
| `monster_sync.py` | Syncs monster availability/release data | `monster_sheet_id.txt` |
| `zone_sync.py` | Syncs dungeon zone configurations | `zone_sheet_id.txt` |
| `talk_sync.py` | Syncs species-specific dialogue strings | `talk_sheet_id.txt` |
| `wiki_sync.py` | Syncs data with the PMDO wiki (experimental) | - |

### Generator Classes

Backend classes used by sync scripts to process specific data types.

| Script | Description |
|--------|-------------|
| `localization.py` | Handles translation string merging between .resx files and sheets |
| `itemGen.py` | Processes exclusive item data and evolution family trees |
| `monsterGen.py` | Processes monster form and availability data |
| `zoneGen.py` | Generates C# spawn code for dungeon items and monsters |
| `talkGen.py` | Merges species dialogue from sheets to .resx files |
| `sheetMerge.py` | Base class for Google Sheets operations (read/write/merge) |

### Asset Processing

Tools for converting and processing game assets.

| Script | Description |
|--------|-------------|
| `sprite_sync.py` | Imports sprites/portraits from SpriteCollab with transfer mapping |
| `font_maker.py` | Processes font sprite sheets (divide, trim, add shadows) |
| `tile_formatter.py` | Converts PMD tileset rips to PMDO/DTEF format |
| `doc_sync.py` | Generates wiki documentation from XML doc comments |
| `script_maker.py` | Generates boilerplate Lua init files for dungeon zones |

### Utilities

Shared utility modules.

| Script | Description |
|--------|-------------|
| `Constants.py` | Sprite animation constants (emotions, actions, directions) |
| `TrackerUtils.py` | Sprite tracker JSON handling, credit management, file operations |
| `utils.py` | Image processing utilities (bounds, palettes, offsets, comparisons) |

## Common Workflows

### Updating Translations

Sync all localization strings between the game and translation sheets:

```bash
uv run string_sync.py
```

This will:
1. Merge hardcoded UI strings (`Hardcode` sheet)
2. Merge content strings (`Content` sheet)
3. Merge script dialogue (`Script` sheet)
4. Merge all data type strings (abilities, items, moves, etc.)
5. Write updated `.resx` files and output files

### Updating Sprites from SpriteCollab

Import updated sprites and portraits:

```bash
uv run sprite_sync.py
```

This will:
1. Update `transfer.json` with newly added sprites
2. Pause for you to review `added_nodes.txt` and modify mappings
3. Transfer sprites/portraits to `RawAsset/` with diff tracking
4. Copy custom sprites from the Custom folder

### Syncing Item Data

Update exclusive item definitions and evolution trees:

```bash
uv run item_sync.py
```

### Converting Tilesets

Convert raw PMD tileset rips to PMDO format:

```python
from tile_formatter import ConvertAllTilesets, ConvertAllToDtef

# Convert raw rips to intermediate format
ConvertAllTilesets("input_dir", "output_dir")

# Convert to DTEF format for the engine
ConvertAllToDtef("intermediate_dir", "dtef_output_dir")
```

### Generating Zone Scripts

Create boilerplate Lua scripts for new dungeon zones:

```python
from script_maker import write_script

write_script("zone_042")  # Creates init.lua with standard callbacks
```

## For Modders

### Adding New Translations

1. Add your language column to the relevant Google Sheet
2. Run `string_sync.py` to generate the new `.resx` files
3. The script auto-detects new languages from sheet headers

### Modifying Sprite Transfers

The `transfer.json` file in `RawAsset/` controls which SpriteCollab sprites are imported:

```json
{
  "0001": {
    "name": "Bulbasaur",
    "portrait_dest": -1,  // -1 = default mapping
    "sprite_dest": -1,
    "idle": false,
    "subgroups": { ... }
  }
}
```

- `portrait_dest`/`sprite_dest`: `-2` = skip, `-1` = default, `>0` = remap to form index
- `idle`: Set `true` to add `<CutsceneIdle/>` tag to AnimData.xml

### Adding Custom Sprites

Place custom sprites (not in SpriteCollab) in the `Custom` folder structure:
```
Custom/
  sprite/
    0001/       # Species index
      0000/     # Form index
        *.png
        AnimData.xml
  portrait/
    0001/
      0000/
        *.png
```

Then add mappings to `custom_transfer.json`.

### Data File Formats

**Input files** (from game export):
- Tab-separated `.txt` files in `DataAsset/`

**Output files** (generated):
- `.out.txt` files with merged/updated data
- `.resx` XML files for localization
- `.csv` diff files for tracking changes

### Understanding Zone Generation

`zoneGen.py` converts spreadsheet data into C# spawn code:

```csharp
// Generated item spawns
ItemSpawnZoneStep itemSpawnZoneStep = new ItemSpawnZoneStep();
categorySpawn.Spawns.Add(new InvItem("berry_oran"), new IntRange(0, max_floors), 10);

// Generated monster spawns
poolSpawn.Spawns.Add(GetTeamMob("bulbasaur", "", "", "", "", new RandRange(5)), new IntRange(0, 5), 10);
```

### Extending Scripts

All sync scripts inherit from `SheetMerge`, which provides:
- `_query_sheet()` - Read sheet data
- `_write_sheet_table()` - Write data to sheet
- `_query_txt()` / `_write_txt()` - Read/write TSV files
- `_query_resx()` / `_write_resx()` - Read/write .resx localization files

To create a new sync script:
1. Create a generator class extending `SheetMerge`
2. Create a sync script with OAuth setup and sheet ID loading
3. Add your sheet ID file

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
