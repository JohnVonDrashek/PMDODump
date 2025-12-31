# Dev

Development utilities for DataGenerator.

## Overview

This directory contains tools for managing localization and translation workflows. The primary utility handles exporting translatable strings from game data to spreadsheet-friendly formats and importing translated strings back into the game's data files.

## Contents

| File | Description |
|------|-------------|
| `Localization.cs` | String table export/import utilities for multi-language support |

## Localization Workflow

### Export Strings

To extract all translatable strings for translation:

```bash
dotnet run -- -asset ../DumpAsset/ -strings out
```

This generates text files in `DataAsset/String/` with tab-separated values:
- One file per data type (Skill.txt, Item.txt, Zone.txt, etc.)
- Columns: Key, Comment, English, French, German, Spanish, Italian, Korean, Japanese, Chinese

### Import Translated Strings

After translating the spreadsheet files:

```bash
dotnet run -- -asset ../DumpAsset/ -strings in
```

This reads the translation files and updates all game data with localized strings.

## Supported Data Types

The localization system handles:

| Data Type | Export Method | Description |
|-----------|---------------|-------------|
| Skill | `PrintDescribedStringTable` | Move names and descriptions |
| Intrinsic | `PrintDescribedStringTable` | Ability names and descriptions |
| Status | `PrintDescribedStringTable` | Status effect text |
| MapStatus | `PrintDescribedStringTable` | Map effect text |
| Tile | `PrintDescribedStringTable` | Tile type names |
| Item | `PrintItemStringTable` | Item names and descriptions |
| Zone | `PrintZoneStringTable` | Dungeon names |
| Map | `PrintTitledDataTable` | Map/floor names |
| Ground | `PrintTitledDataTable` | Ground map names |
| ExclusiveItemType | `PrintExclusiveNameStringTable` | Exclusive item type names |
| ExclusiveItemEffect | `PrintExclusiveDescStringTable` | Exclusive item effect descriptions |

## Key Methods

### PrintDescribedStringTable

Exports data with Name and Description fields:

```csharp
Localization.PrintDescribedStringTable(
    DataManager.DataType.Skill,
    DataManager.Instance.GetSkill
);
```

### WriteDescribedDataTable

Imports translations back to data files:

```csharp
Localization.WriteDescribedDataTable(DataManager.DataType.Skill);
```

## Language Support

Currently supported languages (by column position):

| Column | Language Code | Language |
|--------|---------------|----------|
| 2 | en | English |
| 3 | fr | French |
| 4 | de | German |
| 5 | es | Spanish |
| 7 | it | Italian |
| 8 | ko | Korean |
| 9 | ja | Japanese (Hiragana/Katakana) |
| 10 | ja-jp | Japanese (Kanji) |
| 11 | zh-hans | Chinese (Simplified) |
| 12 | zh-hant | Chinese (Traditional) |

## Translation File Format

Export files use tab-separated values:

```
Key	Comment	EN	FR	DE	ES	PT	IT	KO	JA	JA-JP	ZH-HANS	ZH-HANT
skill.pound	-	Pound	Ecras'Face	Klaps	Destructor	-	Botta	막치기	はたく	はたく	拍击	拍击
```

- Empty cells or `-` indicate missing translations
- Highlighted cells (in spreadsheet) indicate untranslated/unverified entries

## Notes for Translators

- Always work from a fresh export to avoid overwriting game updates
- Preserve the Key column exactly - it's used for matching
- Comment column provides context for translators
- Test translations in-game before committing
- Some strings contain format placeholders like `{0}` - preserve these
