# .github/workflows

GitHub Actions CI/CD automation.

## Contents

| Workflow | Trigger | Description |
|----------|---------|-------------|
| `deploy.yml` | Tag push | Build and release PMDOSetup for all platforms |

## deploy.yml

Automated release workflow that:

1. **Builds PMDOSetup** for 4 platforms:
   - Windows x86 (`win-x86`)
   - Windows x64 (`win-x64`)
   - Linux x64 (`linux-x64`)
   - macOS x64 (`osx-x64`)

2. **Creates GitHub Release** with:
   - Version from `DataGenerator.csproj`
   - Release notes from `CHANGELOG.md`
   - Zipped installers for each platform

## Triggering a Release

```bash
git tag v1.0.0
git push origin v1.0.0
```

The workflow runs on any tag push (`tags: '*'`).
