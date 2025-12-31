# String

Localization strings for all in-game text across multiple languages.

## Overview

This directory contains the source files for all translatable text in PMD: Origins. Each category of text has a source file (`.txt`) that translators edit, and a generated output file (`.out.txt`) produced by `Scripts/string_sync.py`.

**Important**: Only edit the `.txt` source files. Files ending in `.out.txt` are generated and will be overwritten.

## Contents

| Source File | Output File | Description |
|-------------|-------------|-------------|
| `ExclusiveItemEffect.txt` | `ExclusiveItemEffect.out.txt` | Exclusive item effect names |
| `ExclusiveItemType.txt` | `ExclusiveItemType.out.txt` | Exclusive item type categories |
| `Ground.txt` | `Ground.out.txt` | Ground map names and descriptions |
| `Intrinsic.txt` | `Intrinsic.out.txt` | Ability names and descriptions |
| `Item.txt` | `Item.out.txt` | Item names and descriptions |
| `Map.txt` | `Map.out.txt` | Map/location names |
| `MapStatus.txt` | `MapStatus.out.txt` | Weather and terrain effect names |
| `Skill.txt` | `Skill.out.txt` | Move names and descriptions |
| `Status.txt` | `Status.out.txt` | Status condition names |
| `Tile.txt` | `Tile.out.txt` | Terrain tile names |
| `Zone.txt` | `Zone.out.txt` | Dungeon zone names |
| `changelog.txt` | - | Version changelog (special format) |

### Output-Only Files

These files are generated from other sources:

| File | Source |
|------|--------|
| `AI.out.txt` | AI behavior labels |
| `Element.out.txt` | Pokemon type names |
| `Rank.out.txt` | Rescue team rank names |
| `Skin.out.txt` | Pokemon skin/shiny variant names |
| `Special.out.txt` | Special event strings |

## File Format

Tab-separated values (TSV) with the following structure:

| Column | Description |
|--------|-------------|
| Key | Unique string identifier (e.g., `0001-held_assault_vest-0000\|data.Name`) |
| Comment | Optional translator notes |
| EN | English text |
| FR | French |
| DE | German |
| ES | Spanish |
| PT | Portuguese |
| IT | Italian |
| KO | Korean |
| JA | Japanese (Romaji) |
| JA-JP | Japanese (Native) |
| ZH-HANS | Simplified Chinese |
| ZH-HANT | Traditional Chinese |
| RU | Russian |

### Key Format

Keys follow the pattern: `{prefix}-{id}-{index}|{path}`

- `prefix`: Category number (`0000`, `0001`, etc.)
- `id`: Item/monster/skill ID (lowercase with underscores)
- `index`: Entry index for multi-line content
- `path`: Data path (e.g., `data.Name`, `data.Desc`)

Example:
```
0001-held_black_belt-0000|data.Name		Black Belt	Ceinture Noire	Schwarzgurt	...
0001-held_black_belt-0001|data.Desc		An item to be held by a Pokémon...
```

## Workflow

### For Translators

1. Open the appropriate `.txt` source file
2. Find your language column (EN, FR, DE, etc.)
3. Add or modify translations in empty cells
4. Leave cells empty if translation is pending
5. Run sync to generate output:
   ```bash
   uv run Scripts/string_sync.py
   ```

### Translation Guidelines

- Keep placeholder tags intact: `{0}`, `{1}`, `\n` (newline)
- Match formatting of English source when possible
- Use native script for CJK languages (Chinese, Japanese, Korean)
- Empty cells inherit from English in game

### Adding New Strings

1. Add a new row with appropriate Key format
2. Fill in EN (English) column first
3. Add translations to other language columns
4. Sync to generate output files

## Special Characters

| Character | Meaning |
|-----------|---------|
| `{0}`, `{1}` | String placeholders (e.g., Pokemon name, type) |
| `\n` | Newline |
| `\uE024` | Special currency symbol |
| Tab | Column separator |

## Supported Languages

| Code | Language | Script |
|------|----------|--------|
| EN | English | Latin |
| FR | French | Latin |
| DE | German | Latin |
| ES | Spanish | Latin |
| PT | Portuguese | Latin |
| IT | Italian | Latin |
| KO | Korean | Hangul |
| JA | Japanese (Romaji) | Latin |
| JA-JP | Japanese | Kanji/Kana |
| ZH-HANS | Simplified Chinese | Hanzi |
| ZH-HANT | Traditional Chinese | Hanzi |
| RU | Russian | Cyrillic |

## Related Files

- `Scripts/string_sync.py` - Sync script
- `Scripts/localization.py` - Localization utilities
