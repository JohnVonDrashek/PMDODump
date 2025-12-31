# Monster

Monster/Pokemon data including species releases, personality quiz mappings, and Pokedex information.

## Overview

This directory contains source data for Pokemon species management, including which monsters are available, where they spawn, personality quiz configurations, and Pokedex reference data. Files are synced using `Scripts/monster_sync.py`.

## Contents

| File | Type | Description |
|------|------|-------------|
| `personality_in.txt` | Source | Personality quiz trait mapping (matrix format) |
| `personality_out.txt` | Generated | Compiled personality data (do not edit) |
| `pokedex.9.sqlite` | Reference | SQLite database with Pokedex data |
| `releases.txt` | Source | Pokemon release status and dungeon spawn locations |
| `releases.out.txt` | Generated | Compiled release data (do not edit) |

## File Formats

### releases.txt

Tab-separated data tracking Pokemon availability and spawn locations:

| Column | Description |
|--------|-------------|
| F### | Family number |
| Family | Evolution family name (e.g., `bulbasaur`) |
| ### | National Pokedex number |
| Species | Species ID (lowercase, underscored) |
| Form | Form number (0 = base form) |
| Name | Display name including form (e.g., `Alolan Vulpix`) |
| Unimplemented | Missing mechanics (e.g., `stomping_tantrum`) |
| Encounter | Dungeon spawn locations (space-separated zone names) |
| Sprited | Whether sprites are complete (`TRUE`/`FALSE`) |
| Diff | Differential flag |

Example:
```
4  charmander  5  charmeleon  0  Charmeleon    guildmaster_trail  TRUE  TRUE
```

Special Encounter values:
- `EVOLVE` - Only obtainable through evolution
- `TEMP` - Temporarily unavailable
- Dungeon names - Space-separated list of spawn zones

### personality_in.txt

Personality compatibility matrix:
- Row: Pokedex number + form
- Columns: Personality trait flags (`x` = compatible)

Determines which starter Pokemon can result from different personality quiz answers.

### pokedex.9.sqlite

SQLite database containing:
- Base species data
- Type information
- Evolution chains
- Base stats
- Learnsets

Used as reference data for generation scripts.

## Workflow

### Adding New Pokemon

1. Add entry to `releases.txt` with appropriate columns
2. Set `Encounter` to dungeon zone names or special values
3. Set `Sprited` to `FALSE` until sprites are complete
4. Run sync script:
   ```bash
   uv run Scripts/monster_sync.py
   ```

### Modifying Spawn Locations

1. Edit the `Encounter` column in `releases.txt`
2. Use space-separated zone names (e.g., `guildmaster_trail trickster_woods`)
3. Sync to update game data

### Personality Quiz Adjustments

1. Edit `personality_in.txt` to modify trait compatibility
2. Matrix format: each row is a Pokemon, columns are personality traits
3. Mark compatible traits with `x`
4. Sync generates `personality_out.txt`

## Form Naming Conventions

| Form Number | Convention | Example |
|-------------|------------|---------|
| 0 | Base form | `Vulpix` |
| 1+ | Regional/Special | `Alolan Vulpix`, `Mega Charizard X` |

Special forms include:
- Regional variants (Alolan, Galarian, Hisuian, Paldean)
- Mega Evolutions
- Gigantamax forms
- Gender differences

## Related Files

- `DataAsset/Item/EvoTreeRef.txt` - Evolution tree data
- `Scripts/monster_sync.py` - Sync script
- `Scripts/monsterGen.py` - Monster generation utilities
