# Docs

Lua scripting API documentation for PMD: Origins.

## Overview

This directory contains reference documentation for the Lua scripting API used in PMD: Origins. Each file documents a specific class or module's available functions, properties, and usage patterns.

These files are generated from source code documentation and synchronized using `Scripts/doc_sync.py`.

## Contents

| File | Description |
|------|-------------|
| `BattleContext.txt` | Battle context state and properties (strikes, hitboxes, damage tracking) |
| `BattleData.txt` | Combat data structures and skill/item effects |
| `Character.txt` | Dungeon character (party member/enemy) properties and methods |
| `DataManager.txt` | Game data access and management functions |
| `GroundChar.txt` | Ground map character entities and AI control |
| `IntrinsicData.txt` | Ability/intrinsic data structures |
| `ItemData.txt` | Item properties and usage data |
| `MapStatusData.txt` | Map-wide status effects (weather, terrain) |
| `MonsterData.txt` | Pokemon species data and form information |
| `Script.txt` | Core scripting API (AI, DUNGEON, GAME, GROUND, SOUND, STRINGS, TASK, UI modules) |
| `SkillData.txt` | Move/skill data structures |
| `StatusData.txt` | Status condition data and effects |
| `TileData.txt` | Terrain tile properties |

## Documentation Format

Each file uses a wiki-style markup:

```
== Module:FunctionName(Type1,Type2) ==

Description of what the function does.

=== Arguments ===

* <code>param1</code>: Description of parameter
* <code>param2</code>: Description of parameter

=== Returns ===

Description of return value

=== Example ===

<pre>
-- Lua code example
GAME:FadeOut(false, 60)
</pre>
```

## Key Modules (from Script.txt)

| Module | Purpose |
|--------|---------|
| `AI` | Character AI management (enable/disable, state changes) |
| `DUNGEON` | Dungeon-specific operations (floor info, character actions, VFX) |
| `GAME` | Core game operations (saving, zone transitions, party management, items) |
| `GROUND` | Ground map operations (character movement, spawning, animations) |
| `SOUND` | Audio playback (SE, BGM, fanfares) |
| `STRINGS` | Localization and string formatting |
| `TASK` | Coroutine and task management |
| `UI` | User interface (dialogues, menus, choices) |

## Workflow

### For Script Authors

1. Reference these docs when writing Lua scripts
2. Check `Script.txt` for the main API modules
3. Type-specific files provide detailed property information

### Updating Documentation

Documentation is auto-generated from source:

```bash
uv run Scripts/doc_sync.py
```

Do not manually edit these files - changes will be overwritten.
