---
name: pmdo-sync-check
description: Use when validating data consistency across monster, item, skill, and zone definitions in PMD: Origins to catch missing references, orphaned data, or localization gaps.
---

# PMD: Origins Data Sync Check

Validates cross-references between game data files to detect inconsistencies.

## Data Locations

| Data Type | Source Definition | Compiled Data |
|-----------|-------------------|---------------|
| Monsters | `DataGenerator/Data/MonsterInfo.cs` | `DumpAsset/Data/Monster/*.json` |
| Items | `DataGenerator/Data/ItemInfo.cs` | `DumpAsset/Data/Item/*.json` |
| Skills | `DataGenerator/Data/Skills/SkillInfo.cs` | `DumpAsset/Data/Skill/*.json` |
| Zones | `DataGenerator/Data/Zones/*.cs` | `DumpAsset/Data/Zone/*.json` |
| Localization | `DataGenerator/Dev/Localization.cs` | - |

## Validation Checks

### 1. Monster References in Zones

Find monster IDs in spawn tables and verify they exist:

```bash
# Extract monster refs from zone files
rg 'GetTeamMob\("(\w+)"' -o -r '$1' DataGenerator/Data/Zones/ | sort -u > /tmp/zone_monsters.txt

# List compiled monsters
ls DumpAsset/Data/Monster/ | sed 's/.json//' | sort > /tmp/defined_monsters.txt

# Find missing monsters
comm -23 /tmp/zone_monsters.txt /tmp/defined_monsters.txt
```

### 2. Item References in Zones

Find item IDs in drop tables and verify they exist:

```bash
# Extract item refs from zone files
rg 'new InvItem\("([^"]+)"' -o -r '$1' DataGenerator/Data/Zones/ | sort -u > /tmp/zone_items.txt

# List compiled items
ls DumpAsset/Data/Item/ | sed 's/.json//' | sort > /tmp/defined_items.txt

# Find missing items
comm -23 /tmp/zone_items.txt /tmp/defined_items.txt
```

### 3. Skill References

Find skill IDs used in monster movesets and spawns:

```bash
# Extract skill refs from zone spawns (moves in GetTeamMob calls)
rg 'GetTeamMob\([^)]+, "(\w+)"' -o DataGenerator/Data/Zones/ | grep -oE '"[a-z_]+"' | tr -d '"' | sort -u > /tmp/zone_skills.txt

# List compiled skills
ls DumpAsset/Data/Skill/ | sed 's/.json//' | sort > /tmp/defined_skills.txt

# Find missing skills
comm -23 /tmp/zone_skills.txt /tmp/defined_skills.txt
```

### 4. Orphaned Data Detection

Find compiled data with no source references:

```bash
# Monsters defined but never spawned
comm -13 /tmp/zone_monsters.txt /tmp/defined_monsters.txt | head -20

# Items defined but never placed
comm -13 /tmp/zone_items.txt /tmp/defined_items.txt | head -20
```

### 5. Localization Key Audit

Check for missing LocalText entries:

```bash
# Find LocalText usages without translations
rg 'new LocalText\("([^"]+)"\)' -o -r '$1' DataGenerator/Data/ | sort -u | wc -l

# Check localization export methods exist for data type
rg 'PrintStringTable|WriteStringTable' DataGenerator/Dev/Localization.cs
```

## Quick Validation Commands

```bash
# Count mismatches summary
echo "=== Data Sync Summary ==="
echo "Monsters in zones: $(rg -c 'GetTeamMob' DataGenerator/Data/Zones/ | awk -F: '{s+=$2}END{print s}')"
echo "Items in zones: $(rg -c 'new InvItem' DataGenerator/Data/Zones/ | awk -F: '{s+=$2}END{print s}')"
echo "Defined monsters: $(ls DumpAsset/Data/Monster/*.json | wc -l)"
echo "Defined items: $(ls DumpAsset/Data/Item/*.json | wc -l)"
echo "Defined skills: $(ls DumpAsset/Data/Skill/*.json | wc -l)"
```

## Common Issues

- **Typo in monster/item ID**: String IDs are lowercase with underscores (e.g., `pikachu`, `berry_oran`, `thunder_wave`)
- **Missing form specification**: `MonsterID` uses format `("name", formNum, "skin", gender)`
- **Skill name mismatch**: Skills use snake_case (e.g., `hidden_power` not `hiddenpower`)
