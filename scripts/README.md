# Scripts

Cross-platform scripts for development and testing.

## CLI Testing

Test the FSH CLI tool locally without publishing to NuGet.

### Windows (PowerShell)
```powershell
# Install locally
./scripts/test-cli.ps1 -Version "10.0.0-local"

# Skip build (use existing package)
./scripts/test-cli.ps1 -SkipBuild

# Uninstall
./scripts/test-cli.ps1 -Uninstall
```

### macOS/Linux (Bash)
```bash
# Make script executable (first time only)
chmod +x ./scripts/test-cli.sh

# Install locally
./scripts/test-cli.sh --version "10.0.0-local"

# Skip build (use existing package)
./scripts/test-cli.sh --skip-build

# Uninstall
./scripts/test-cli.sh --uninstall

# Show help
./scripts/test-cli.sh --help
```

### Alternative (PowerShell Core - cross-platform)
If you have PowerShell Core installed on macOS/Linux:
```bash
pwsh ./scripts/test-cli.ps1 -Version "10.0.0-local"
```

## OpenAPI Client Generation

See [scripts/openapi/README.md](./openapi/README.md) for details on generating API clients.

## Platform Notes

- **Windows**: Use PowerShell scripts (`.ps1`)
- **macOS/Linux**: Use Bash scripts (`.sh`) or install [PowerShell Core](https://github.com/PowerShell/PowerShell) to use `.ps1` scripts
- All scripts provide equivalent functionality across platforms
- Bash scripts on Unix systems need execute permissions: `chmod +x script.sh`
