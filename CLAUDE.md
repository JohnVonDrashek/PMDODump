# PMD: Origins (PMDO)

Open source Pokémon Mystery Dungeon roguelike built on the RogueEssence engine.

## Rules

- **Never commit or push without explicit user consent.** Always ask before running `git commit` or `git push`.

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

## macOS Development Setup (Apple Silicon)

### Prerequisites

```bash
brew install dotnet@8 sdl2 lua@5.4 cmake
```

### Clone and Build PMDC

```bash
# Clone PMDC engine
git clone https://github.com/PMDCollab/PMDC.git ~/code/PMDC
cd ~/code/PMDC
git submodule update --init --recursive

# Build game
dotnet restore PMDC.sln
dotnet build PMDC/PMDC.csproj -c Release
```

### Build Native Libraries (FNA3D, FAudio)

FNA requires native libraries built from source on macOS:

```bash
# Clone fnalibs-apple-builder
git clone https://github.com/TheSpydog/fnalibs-apple-builder.git ~/code/fnalibs-apple-builder
cd ~/code/fnalibs-apple-builder

# Checkout matching versions (24.08)
rm -rf FNA3D FAudio
git clone https://github.com/FNA-XNA/FNA3D.git && cd FNA3D && git checkout 24.08 && git submodule update --init && cd ..
git clone https://github.com/FNA-XNA/FAudio.git && cd FAudio && git checkout 24.08 && cd ..

# Build FNA3D
cd FNA3D && mkdir build && cd build
cmake .. -DCMAKE_OSX_ARCHITECTURES="arm64" \
  -DSDL2_INCLUDE_DIRS=/opt/homebrew/include/SDL2 \
  -DSDL2_LIBRARIES=/opt/homebrew/lib/libSDL2.dylib \
  -DCMAKE_POLICY_VERSION_MINIMUM=3.5
make -j8
cd ../..

# Build FAudio
cd FAudio && mkdir build && cd build
cmake .. -DCMAKE_OSX_ARCHITECTURES="arm64" \
  -DSDL2_INCLUDE_DIRS=/opt/homebrew/include/SDL2 \
  -DSDL2_LIBRARIES=/opt/homebrew/lib/libSDL2.dylib \
  -DCMAKE_POLICY_VERSION_MINIMUM=3.5
make -j8
cd ../..
```

### Link Libraries and Run

```bash
# Copy/link native libs to build directory
cd ~/code/PMDC/PMDC/bin/Release/net8.0
ln -s /opt/homebrew/lib/libSDL2-2.0.0.dylib .
cp ~/code/fnalibs-apple-builder/FNA3D/build/libFNA3D.0.dylib .
cp ~/code/fnalibs-apple-builder/FAudio/build/libFAudio.0.dylib .
ln -s /opt/homebrew/opt/lua@5.4/lib/liblua.dylib lua54.dylib

# Run the game
/opt/homebrew/opt/dotnet@8/bin/dotnet PMDC.dll \
  -asset ~/code/PMDODump/DumpAsset/ \
  -raw ~/code/PMDODump/RawAsset/
```

### Native Library Summary

| Library | Version | Source |
|---------|---------|--------|
| SDL2 | 2.x | `brew install sdl2` |
| FNA3D | 24.08 | Build from source |
| FAudio | 24.08 | Build from source |
| Lua | 5.4 | `brew install lua@5.4` |

### Troubleshooting

| Error | Solution |
|-------|----------|
| `.NET 8.0 not found` | Use `/opt/homebrew/opt/dotnet@8/bin/dotnet` |
| `libSDL2 not found` | `ln -s /opt/homebrew/lib/libSDL2-2.0.0.dylib .` |
| `libFNA3D not found` | Build FNA3D 24.08 from source (see above) |
| `lua54 not found` | `ln -s /opt/homebrew/opt/lua@5.4/lib/liblua.dylib lua54.dylib` |

## External Resources

- [Wiki](https://wiki.pmdo.pmdcollab.org/Main_Page)
- [Pokécommunity Thread](https://www.pokecommunity.com/showthread.php?p=10325347)
- [Releases](https://github.com/PMDCollab/PMDC/releases)
- [FNA3D GitHub](https://github.com/FNA-XNA/FNA3D)
- [fnalibs-apple-builder](https://github.com/TheSpydog/fnalibs-apple-builder)
