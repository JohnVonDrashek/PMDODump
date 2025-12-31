# PMD: Origins (PMDO)

Open source Pokémon Mystery Dungeon roguelike built on the RogueEssence engine.

## Key Entry Points

| Entry Point | Purpose |
|-------------|---------|
| `PMDOData.sln` | Main Visual Studio solution |
| `DataGenerator/Program.cs` | Data file generation CLI |
| `PMDC/MapGenTest/Program.cs` | Dungeon map testing/debugging |
| `PMDC/PMDC/Program.cs` | Main game executable |
| `PMDOSetup/` | Installer/updater application |
| `deploy` | Build & package script for all platforms |

## Directory Guide

| Directory | Purpose |
|-----------|---------|
| `PMDC/` | Submodule: Battle system & game logic |
| `PMDC/RogueEssence/` | Core roguelike engine (nested submodule) |
| `PMDC/RogueEssence/RogueElements/` | Procedural generation library |
| `PMDC/MapGenTest/` | Dungeon map testing tool |
| `DumpAsset/` | Submodule: Compiled game assets (sprites, sounds, data) |
| `DumpAsset/Data/` | Serialized game data (monsters, items, skills, zones) |
| `DumpAsset/Content/` | Graphics, audio, fonts |
| `DumpAsset/Strings/` | Localization files |
| `RawAsset/` | Submodule: Unconverted source graphics |
| `DataGenerator/` | Tool to generate game data files |
| `DataGenerator/Data/` | C# classes defining all game content |
| `DataGenerator/Data/Zones/` | Dungeon zone definitions |
| `DataAsset/` | Spreadsheets & CSVs for data sync |
| `Scripts/` | Python utilities for asset/data sync |

## Build Commands

```bash
# Initialize submodules
git submodule update --init --recursive

# Build game (pick platform)
dotnet publish -c Release -r win-x64 PMDC/PMDC/PMDC.csproj
dotnet publish -c Release -r linux-x64 PMDC/PMDC/PMDC.csproj
dotnet publish -c Release -r osx-x64 PMDC/PMDC/PMDC.csproj

# Build server
dotnet publish -c Release -r linux-x64 PMDC/RogueEssence/WaypointServer/WaypointServer.csproj

# Build installer
dotnet publish -c Release -r win-x64 PMDOSetup/PMDOSetup.csproj

# Full deploy (all platforms)
./deploy
```

## DataGenerator Commands

```bash
# Run from DataGenerator/bin/ or with proper paths

# One-time item prep (generates tables for items)
-itemprep

# Dump all data types
-dump

# Reserialize specific data types
-reserialize Skill
-reserialize Monster

# Export strings for translation
-strings out

# Import translated strings
-strings in

# Set custom paths
-asset <path>  # Asset path
-raw <path>    # Raw asset path
-gen <path>    # Generated data path
```

## MapGenTest Commands

```bash
# Test dungeon generation
-asset <path>    # Set asset path
-raw <path>      # Set raw asset path
-lua             # Enable Lua debugging
-quest <name>    # Load specific quest/mod
-mod <name>      # Load mod(s)
-exp             # Run experience testing
-expdir <path>   # Set experience test output dir
```

## Data Types

DataManager.DataType enum (used with `-dump`, `-reserialize`, `-index`):
- `Monster` - Pokémon species data
- `Skill` - Move definitions
- `Item` - Item definitions
- `Intrinsic` - Abilities
- `Status` - Status conditions
- `MapStatus` - Map-wide effects
- `Zone` - Dungeon definitions
- `AI` - AI behavior patterns
- `Element` - Type chart
- `Tile` - Tile definitions
- `Terrain` - Terrain types
- `Emote` - Emote animations
- `GrowthGroup` - Experience curves
- `SkillGroup` - Egg groups
- `Rank` - Exploration ranks

## Scripts (Python)

Requires: `uv pip install -r Scripts/requirements.txt`

| Script | Purpose |
|--------|---------|
| `item_sync.py` | Sync exclusive items from spreadsheet |
| `string_sync.py` | Sync translation strings |
| `monster_sync.py` | Sync monster data |
| `zone_sync.py` | Sync zone/dungeon data |
| `sprite_sync.py` | Sync sprite assets |
| `localization.py` | Full localization workflow |
| `sheetMerge.py` | Merge sprite sheets |
| `font_maker.py` | Generate bitmap fonts |
| `TrackerUtils.py` | Sprite tracker utilities |

## Key Patterns

- **Serialization**: Uses custom `Serializer` with `SerializerContractResolver` and `UpgradeBinder`
- **Path Management**: `PathMod` class handles all path resolution
- **Data Files**: JSON/XML serialized in `DumpAsset/Data/`
- **Scripting**: Lua via NLua for game scripts
- **Graphics**: FNA (XNA reimplementation) for rendering
- **Procedural Gen**: RogueElements library for dungeon generation

## Submodule Repositories

| Submodule | Repository |
|-----------|------------|
| PMDC | https://github.com/PMDCollab/PMDC.git |
| DumpAsset | https://github.com/audinowho/DumpAsset.git |
| RawAsset | https://github.com/PMDCollab/RawAsset.git |

## macOS Development Setup

### Quick Start (Pre-built Release)

The easiest way to run the game on macOS is using the official pre-built release:

```bash
# Download latest release
gh release download v0.8.11 --repo PMDCollab/PMDC --pattern "pmdc-osx-x64.zip"
unzip pmdc-osx-x64.zip -d PMDO-Game

# Initialize asset submodules (if not done)
git submodule update --init DumpAsset RawAsset

# Run the game
cd PMDO-Game/PMDO
chmod +x PMDO
./PMDO -asset /path/to/PMDODump/DumpAsset/ -raw /path/to/PMDODump/RawAsset/
```

**Note:** The x64 build runs via Rosetta 2 on Apple Silicon (M1/M2/M3).

### Building from Source (Advanced)

Building from source on macOS requires native FNA libraries which need SDL3.

#### Prerequisites

```bash
# Install dependencies
brew install dotnet@8 sdl2 cmake

# Clone PMDC (game engine)
git clone https://github.com/PMDCollab/PMDC.git ~/code/PMDC
cd ~/code/PMDC
git submodule update --init --recursive
```

#### Build Native Libraries (FNA3D, FAudio)

FNA requires native libraries that must be built from source on macOS (fnalibs no longer provides macOS binaries as of April 2025):

```bash
# Clone the Apple builder
git clone https://github.com/TheSpydog/fnalibs-apple-builder.git
cd fnalibs-apple-builder

# Download sources
./updatelibs.sh

# Build for macOS (requires Xcode CLI tools)
./buildlibs.sh macos

# Copy libraries to game directory
cp bin/macos/*.dylib ~/code/PMDC/PMDC/bin/Release/net8.0/
```

**Current Issue:** FNA3D now requires SDL3, not SDL2. The build process is evolving.

#### Build the Game

```bash
cd ~/code/PMDC
dotnet restore PMDC.sln
dotnet build PMDC/PMDC.csproj -c Release

# Run with assets from PMDODump
cd PMDC/bin/Release/net8.0
dotnet PMDC.dll -asset ~/code/PMDODump/DumpAsset/ -raw ~/code/PMDODump/RawAsset/
```

### Troubleshooting

| Error | Solution |
|-------|----------|
| `.NET 8.0 not found` | `brew install dotnet@8` and use `/opt/homebrew/opt/dotnet@8/bin/dotnet` |
| `libSDL2-2.0.0.dylib not found` | `ln -s /opt/homebrew/lib/libSDL2-2.0.0.dylib ./` in the bin directory |
| `libFNA3D.0.dylib not found` | Build FNA3D from source (see above) or use pre-built release |

## External Resources

- [Wiki](https://wiki.pmdo.pmdcollab.org/Main_Page)
- [Pokécommunity Thread](https://www.pokecommunity.com/showthread.php?p=10325347)
- [Releases](https://github.com/PMDCollab/PMDC/releases)
- [FNA3D GitHub](https://github.com/FNA-XNA/FNA3D)
- [fnalibs-apple-builder](https://github.com/TheSpydog/fnalibs-apple-builder)
