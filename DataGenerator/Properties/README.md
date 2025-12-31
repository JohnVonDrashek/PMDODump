# Properties

.NET assembly configuration for DataGenerator.

## Overview

Standard .NET project properties folder containing launch profiles and assembly metadata. This directory is auto-managed by Visual Studio and the .NET SDK.

## Contents

| File | Description |
|------|-------------|
| `launchSettings.json` | Debug launch configuration with default arguments |

## Launch Configuration

The default launch profile configures paths for local development:

```json
{
  "profiles": {
    "DataGenerator": {
      "commandName": "Project",
      "commandLineArgs": "-asset ../../../../DumpAsset/ -raw ../../../../RawAsset/ -gen ../../../../DataAsset/ -dump monster"
    }
  }
}
```

### Path Layout

The default configuration assumes this directory structure:

```
project-root/
  DumpAsset/        # Compiled game assets
  RawAsset/         # Development assets
  DataAsset/        # Generated data output
  DataGenerator/    # This project
```

Adjust paths in `launchSettings.json` to match your local setup.

## Customizing Launch Profiles

Add additional profiles for different workflows:

```json
{
  "profiles": {
    "DataGenerator": {
      "commandName": "Project",
      "commandLineArgs": "-asset ../../../../DumpAsset/ -dump monster"
    },
    "ExportStrings": {
      "commandName": "Project",
      "commandLineArgs": "-asset ../../../../DumpAsset/ -strings out"
    },
    "DumpAll": {
      "commandName": "Project",
      "commandLineArgs": "-asset ../../../../DumpAsset/ -dump all"
    }
  }
}
```
