#!/usr/bin/env bash
set -euo pipefail

# Parse arguments
VERSION="10.0.0-local"
UNINSTALL=false
SKIP_BUILD=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -v|--version)
            VERSION="$2"
            shift 2
            ;;
        -u|--uninstall)
            UNINSTALL=true
            shift
            ;;
        -s|--skip-build)
            SKIP_BUILD=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  -v, --version VERSION    Package version (default: 10.0.0-local)"
            echo "  -u, --uninstall          Uninstall the CLI tool"
            echo "  -s, --skip-build         Skip build and use existing package"
            echo "  -h, --help               Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use -h or --help for usage information"
            exit 1
            ;;
    esac
done

# Get script directory and repository root
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPO_ROOT="$( cd "$SCRIPT_DIR/.." && pwd )"
CLI_PROJECT="$REPO_ROOT/src/Tools/CLI/FSH.CLI.csproj"
NUPKGS_DIR="$REPO_ROOT/artifacts/nupkgs"

echo ""
echo -e "\033[0;36m========================================\033[0m"
echo -e "\033[0;36m  FSH CLI Local Test Script\033[0m"
echo -e "\033[0;36m========================================\033[0m"
echo ""
echo -e "\033[0;90mRepo Root: $REPO_ROOT\033[0m"
echo -e "\033[0;90mCLI Project: $CLI_PROJECT\033[0m"
echo ""

# Uninstall existing CLI
echo -e "\033[0;33m[1/4] Uninstalling existing fsh tool...\033[0m"
if dotnet tool uninstall -g FullStackHero.CLI 2>/dev/null; then
    echo -e "\033[0;32m      Uninstalled successfully\033[0m"
else
    echo -e "\033[0;90m      Not installed (skipping)\033[0m"
fi

if [ "$UNINSTALL" = true ]; then
    echo ""
    echo -e "\033[0;32mUninstall complete.\033[0m"
    exit 0
fi

# Build and pack
if [ "$SKIP_BUILD" = false ]; then
    echo ""
    echo -e "\033[0;33m[2/4] Building and packing CLI (Version: $VERSION)...\033[0m"

    # Clean artifacts
    if [ -d "$NUPKGS_DIR" ]; then
        rm -rf "$NUPKGS_DIR"
    fi
    mkdir -p "$NUPKGS_DIR"

    # Build with version
    if ! dotnet build "$CLI_PROJECT" -c Release -p:Version="$VERSION"; then
        echo -e "\033[0;31mBuild failed!\033[0m"
        exit 1
    fi

    # Pack with version
    if ! dotnet pack "$CLI_PROJECT" -c Release --no-build -o "$NUPKGS_DIR" -p:PackageVersion="$VERSION"; then
        echo -e "\033[0;31mPack failed!\033[0m"
        exit 1
    fi
    echo -e "\033[0;32m      Package created successfully\033[0m"
else
    echo ""
    echo -e "\033[0;90m[2/4] Skipping build (using existing package)...\033[0m"
fi

# Install from local package
echo ""
echo -e "\033[0;33m[3/4] Installing CLI from local package...\033[0m"
PACKAGE_PATH=$(find "$NUPKGS_DIR" -name "FullStackHero.CLI.*.nupkg" -type f | head -n 1)

if [ -z "$PACKAGE_PATH" ]; then
    echo -e "\033[0;31mNo package found in $NUPKGS_DIR\033[0m"
    exit 1
fi

PACKAGE_NAME=$(basename "$PACKAGE_PATH")
echo -e "\033[0;90m      Package: $PACKAGE_NAME\033[0m"

if ! dotnet tool install -g FullStackHero.CLI --add-source "$NUPKGS_DIR" --version "$VERSION"; then
    echo -e "\033[0;31mInstall failed!\033[0m"
    exit 1
fi
echo -e "\033[0;32m      Installed successfully\033[0m"

# Verify installation
echo ""
echo -e "\033[0;33m[4/4] Verifying installation...\033[0m"
echo ""

if FSH_PATH=$(command -v fsh 2>/dev/null); then
    echo -e "\033[0;90m      fsh location: $FSH_PATH\033[0m"
    echo ""
    echo -e "\033[0;36m----------------------------------------\033[0m"
    fsh --version
    echo -e "\033[0;36m----------------------------------------\033[0m"
else
    echo -e "\033[0;33m      Warning: 'fsh' command not found in PATH\033[0m"
    echo -e "\033[0;33m      You may need to restart your terminal\033[0m"
fi

echo ""
echo -e "\033[0;32m========================================\033[0m"
echo -e "\033[0;32m  CLI installed successfully!\033[0m"
echo -e "\033[0;32m========================================\033[0m"
echo ""
echo -e "\033[0;36mTest commands:\033[0m"
echo -e "\033[0;37m  fsh --help\033[0m"
echo -e "\033[0;37m  fsh new --help\033[0m"
echo -e "\033[0;37m  fsh new MyApp\033[0m"
echo ""
echo -e "\033[0;36mTo uninstall:\033[0m"
echo -e "\033[0;37m  $0 --uninstall\033[0m"
echo ""
