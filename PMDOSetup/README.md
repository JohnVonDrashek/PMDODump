# PMDOSetup

![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20macOS%20%7C%20Linux-blue)
![License](https://img.shields.io/badge/License-See%20Main%20Repo-lightgrey)

## Overview

PMDOSetup is the cross-platform installer and updater application for **PMD: Origins** (Pokemon Mystery Dungeon: Origins), a Pokemon Mystery Dungeon roguelike fangame. It handles downloading, installing, and updating the game from GitHub releases automatically.

### Features

- **Automatic Updates**: Checks GitHub releases for new versions and downloads updates
- **Cross-Platform Support**: Works on Windows (x64/x86), macOS, Linux, FreeBSD, NetBSD, and OpenBSD
- **Self-Contained Executable**: Publishes as a single self-extracting executable
- **Save Data Protection**: Automatically backs up save data before updates
- **Version Rollback**: Allows reverting to previous game versions
- **Self-Update**: Can update itself from GitHub releases
- **Uninstall Support**: Clean uninstallation while retaining save data

### Menu Options

When run with an existing installation, the updater presents:

1. **Force Update** - Re-download and install the current latest version
2. **Update the Updater** - Download the latest PMDOSetup executable
3. **Reset Updater XML** - Reset configuration to defaults
4. **Uninstall (Retain Save Data)** - Remove game files but keep saves
5. **Revert to an Older Version** - Browse and install previous releases

## Building

The application is built using .NET 6.0 and publishes as a self-contained single-file executable.

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later

### Publish Commands

Build for your target platform using one of the following commands:

```bash
# Windows x64
dotnet publish -c Release -r win-x64 PMDOSetup/PMDOSetup.csproj

# Windows x86
dotnet publish -c Release -r win-x86 PMDOSetup/PMDOSetup.csproj

# macOS x64
dotnet publish -c Release -r osx-x64 PMDOSetup/PMDOSetup.csproj

# macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 PMDOSetup/PMDOSetup.csproj

# Linux x64
dotnet publish -c Release -r linux-x64 PMDOSetup/PMDOSetup.csproj

# Linux x86
dotnet publish -c Release -r linux-x86 PMDOSetup/PMDOSetup.csproj
```

Output is written to `../publish/{RuntimeIdentifier}/PMDOSetup/`.

### Debug Build

```bash
dotnet build PMDOSetup/PMDOSetup.csproj
```

## Contents

| File | Description |
|------|-------------|
| `Program.cs` | Main application logic - handles update checking, downloading, extraction, and UI |
| `PMDOSetup.csproj` | .NET 6.0 project file with self-contained publish configuration |
| `Properties/` | .NET assembly configuration (launch settings) |

## Configuration

The updater stores its configuration in `Updater.xml` alongside the executable:

- **ExeRepo**: GitHub repository for releases (default: `audinowho/PMDODump`)
- **Asset**: Asset submodule name (default: `DumpAsset`)
- **UpdaterVersion**: Current updater version
- **LastVersion**: Last installed game version
- **ToDelete**: List of paths to clean during updates
- **Executables**: List of files requiring execute permissions (Unix)

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| [Mono.Posix.NETStandard](https://www.nuget.org/packages/Mono.Posix.NETStandard) | 1.0.0 | Unix file permission handling |

## How It Works

1. **Platform Detection**: Identifies OS and architecture at runtime
2. **Version Check**: Queries GitHub API for latest release
3. **Submodule Resolution**: Resolves PMDC (executable) and DumpAsset (game data) submodules
4. **Download**: Downloads platform-specific game executable and asset archives
5. **Backup**: Saves existing save data to `SAVE.bak/`
6. **Clean Install**: Deletes old game files (preserves save data)
7. **Extract**: Unzips downloaded archives, sets Unix execute permissions
8. **Update Config**: Records new version in `Updater.xml`

## Command Line Arguments

The updater accepts an optional numeric argument to select a menu option without user input:

```bash
# Check for updates (default)
./PMDOSetup

# Force update
./PMDOSetup 1

# Update the updater
./PMDOSetup 2

# Reset XML
./PMDOSetup 3

# Uninstall
./PMDOSetup 4

# Revert version (requires interactive selection)
./PMDOSetup 5
```

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
