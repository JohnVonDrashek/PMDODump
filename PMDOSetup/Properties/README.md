# Properties

![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet)

## Overview

This directory contains .NET assembly configuration files for the PMDOSetup project. These files configure how the application is built, debugged, and launched during development.

## Contents

| File | Description |
|------|-------------|
| `launchSettings.json` | Visual Studio / .NET CLI launch profile configuration |

## launchSettings.json

Defines launch profiles for debugging and running the application:

```json
{
  "profiles": {
    "PMDOSetup": {
      "commandName": "Project"
    }
  }
}
```

### Profile Details

| Property | Value | Description |
|----------|-------|-------------|
| `commandName` | `Project` | Launches the project directly (standard .NET project execution) |

## Auto-Generated Files

During build, additional files are created in `obj/Debug/net6.0/`:

| File | Description |
|------|-------------|
| `PMDOSetup.AssemblyInfo.cs` | Auto-generated assembly metadata |
| `PMDOSetup.AssemblyInfoInputs.cache` | Build cache for assembly info |
| `PMDOSetup.GeneratedMSBuildEditorConfig.editorconfig` | Generated editor configuration |
| `.NETCoreApp,Version=v6.0.AssemblyAttributes.cs` | .NET Core assembly attributes |

These files are regenerated during each build and should not be modified manually.
