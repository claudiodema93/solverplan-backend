#!/usr/bin/env bash
set -euo pipefail

# Default parameter
SPEC_URL="${1:-https://localhost:7030/openapi/v1.json}"

# Get repository root (2 levels up from script directory)
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPO_ROOT="$( cd "$SCRIPT_DIR/../.." && pwd )"
cd "$REPO_ROOT"

echo "Running NSwag generation against $SPEC_URL..."
"$SCRIPT_DIR/generate-api-clients.sh" "$SPEC_URL"

TARGET_FILE="src/Playground/Playground.Blazor/ApiClient/Generated.cs"

echo "Checking for drift in $TARGET_FILE..."
if git diff --exit-code -- "$TARGET_FILE"; then
    echo "No drift detected."
else
    echo "Drift detected! The generated client differs from the committed version."
    exit 1
fi
