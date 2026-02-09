#!/usr/bin/env bash
set -euo pipefail

# Default parameter
SPEC_URL="${1:-https://localhost:7030/openapi/v1.json}"

# Get script directory and repository root
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPO_ROOT="$( cd "$SCRIPT_DIR/../.." && pwd )"
CONFIG_PATH="$SCRIPT_DIR/nswag-playground.json"
OUTPUT_DIR="$REPO_ROOT/src/Playground/Playground.Blazor/ApiClient"

echo -e "\033[0;36mEnsuring dotnet local tools are restored...\033[0m"
dotnet tool restore

echo -e "\033[0;36mEnsuring output directory exists: $OUTPUT_DIR\033[0m"
mkdir -p "$OUTPUT_DIR"

echo -e "\033[0;36mGenerating API client from spec: $SPEC_URL\033[0m"
dotnet nswag run "$CONFIG_PATH" /variables:SpecUrl="$SPEC_URL"

echo -e "\033[0;32mDone. Generated clients are in $OUTPUT_DIR\033[0m"
