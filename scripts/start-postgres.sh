#!/usr/bin/env bash
# Start a local PostgreSQL container for development/migrations
# Usage: ./scripts/start-postgres.sh [--stop] [--reset]
#
# Connection string (matches appsettings.json):
#   Server=localhost;Database=fsh;User Id=postgres;Password=password

set -euo pipefail

CONTAINER_NAME="solverplan-postgres"
IMAGE="postgres:16-alpine"
PORT=5432
DB_NAME="fsh"
DB_USER="postgres"
DB_PASSWORD="password"

STOP=false
RESET=false

for arg in "$@"; do
    case $arg in
        --stop)  STOP=true  ;;
        --reset) RESET=true ;;
    esac
done

echo ""
echo "========================================"
echo "  SolverPlan — PostgreSQL Docker"
echo "========================================"
echo ""

# --- Stop / Remove ---
if $STOP || $RESET; then
    if [ "$(docker ps -q --filter "name=$CONTAINER_NAME")" ]; then
        echo "Stopping container '$CONTAINER_NAME'..."
        docker stop "$CONTAINER_NAME" > /dev/null
        echo "  Stopped."
    fi

    if [ "$(docker ps -aq --filter "name=$CONTAINER_NAME")" ]; then
        echo "Removing container '$CONTAINER_NAME'..."
        docker rm "$CONTAINER_NAME" > /dev/null
        echo "  Removed."
    fi

    if $STOP; then
        echo ""
        echo "Done."
        exit 0
    fi
fi

# --- Check if already running ---
if [ "$(docker ps -q --filter "name=$CONTAINER_NAME")" ]; then
    echo "Container '$CONTAINER_NAME' is already running."
    echo ""
    echo "  Host    : localhost:$PORT"
    echo "  Database: $DB_NAME"
    echo "  User    : $DB_USER"
    echo ""
    exit 0
fi

# --- Start (or re-create if stopped) ---
if [ "$(docker ps -aq --filter "name=$CONTAINER_NAME")" ]; then
    echo "Starting existing container '$CONTAINER_NAME'..."
    docker start "$CONTAINER_NAME" > /dev/null
else
    echo "Creating and starting container '$CONTAINER_NAME'..."
    docker run -d \
        --name "$CONTAINER_NAME" \
        -e POSTGRES_DB="$DB_NAME" \
        -e POSTGRES_USER="$DB_USER" \
        -e POSTGRES_PASSWORD="$DB_PASSWORD" \
        -p "${PORT}:5432" \
        --restart unless-stopped \
        "$IMAGE" > /dev/null
fi

# --- Wait for postgres to be ready ---
echo "Waiting for PostgreSQL to be ready..."
max_attempts=30
attempt=0
until docker exec "$CONTAINER_NAME" pg_isready -U "$DB_USER" -d "$DB_NAME" > /dev/null 2>&1; do
    attempt=$((attempt + 1))
    if [ $attempt -ge $max_attempts ]; then
        echo "PostgreSQL did not become ready in time."
        exit 1
    fi
    sleep 1
done

echo ""
echo "========================================"
echo "  PostgreSQL is ready!"
echo "========================================"
echo ""
echo "  Host    : localhost:$PORT"
echo "  Database: $DB_NAME"
echo "  User    : $DB_USER"
echo ""
echo "Run migrations:"
echo "  dotnet run --project src/Playground/Playground.Api"
echo ""
echo "Stop the container:"
echo "  ./scripts/start-postgres.sh --stop"
echo ""
echo "Reset (remove data):"
echo "  ./scripts/start-postgres.sh --reset"
echo ""
