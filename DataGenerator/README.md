# DataGenerator

Command-line tool that compiles C# data definitions into serialized game data for PMD: Origins.

## Overview

DataGenerator transforms human-readable C# source code into the binary/JSON data files that the RogueEssence game engine consumes at runtime. This is the primary workflow for defining game content such as monsters, moves, items, abilities, status effects, and dungeon zones. The tool also handles localization string extraction and import for multi-language support.

## Contents

| File/Directory | Description |
|----------------|-------------|
| `Program.cs` | Main entry point with CLI argument parsing and data generation orchestration |
| `GenPath.cs` | Static path configuration for generated output directories |
| `DataGenerator.csproj` | .NET 8.0 project file with dependencies on RogueEssence, PMDC, and RogueElements |
| `App.config` | Legacy .NET Framework runtime configuration |
| `Data/` | C# source definitions for all game content types |
| `Dev/` | Development utilities including localization tools |
| `Properties/` | .NET assembly configuration and launch profiles |

## Usage

### Build

```bash
dotnet build DataGenerator.csproj
```

### Command-Line Arguments

| Argument | Description |
|----------|-------------|
| `-asset <path>` | Path to compiled game assets |
| `-raw <path>` | Path to raw/development assets |
| `-gen <path>` | Path for generated output data |
| `-dump <types>` | Generate data for specified types (see below) |
| `-dumpmin` | Generate minimal test data |
| `-index <types>` | Regenerate indices for specified types |
| `-reserialize <types>` | Reserialize existing data with updated format |
| `-strings out` | Export localization strings to translation files |
| `-strings in` | Import translated strings back into data |
| `-itemprep` | Prepare item content lists |
| `-zoneprep` | Prepare zone content lists |
| `-monsterprep` | Prepare monster content lists |
| `-preconvert` | Prepare assets for conversion |

### Data Types

The following data types can be specified with `-dump`, `-index`, or `-reserialize`:

- `Element` - Type chart and elemental types
- `GrowthGroup` - Experience curves
- `SkillGroup` - Move learning groups
- `Emote` - Character emote animations
- `AI` - NPC behavior patterns
- `Tile` / `Terrain` - Dungeon tile properties
- `Rank` - Rescue team ranks
- `Skin` - Shiny/alternate forms
- `Monster` - Pokemon species data
- `Skill` - Moves and attacks
- `Intrinsic` - Abilities
- `Status` - Status conditions
- `MapStatus` - Map-wide effects
- `Item` - Items and equipment
- `Zone` - Dungeons and areas

### Example Commands

Generate all monster data:
```bash
dotnet run -- -asset ../DumpAsset/ -raw ../RawAsset/ -gen ../DataAsset/ -dump monster
```

Export localization strings:
```bash
dotnet run -- -asset ../DumpAsset/ -strings out
```

Regenerate zone indices:
```bash
dotnet run -- -asset ../DumpAsset/ -index zone
```

## Architecture

The generator follows this workflow:

1. **Initialize** - Set up paths, load RogueEssence engine, initialize Lua scripting
2. **Generate** - Call `Add*Data()` methods from Data/*.cs files to create game objects
3. **Serialize** - Save objects to JSON/binary using RogueEssence serialization
4. **Index** - Build lookup indices for runtime data loading

## For Modders

When modifying game content:

1. Find the relevant file in `Data/` (e.g., `ItemInfo.cs` for items)
2. Locate the `Add*Data()` or `Get*Data()` method
3. Add or modify entries following the existing pattern
4. Run DataGenerator with the appropriate `-dump` flag
5. Copy generated files to your mod's data directory

## Dependencies

- **.NET 8.0** - Runtime
- **RogueEssence** - Core game engine
- **PMDC** - Pokemon Mystery Dungeon Content library
- **RogueElements** - Procedural generation library
- **System.Data.SQLite** - Pokemon database access
