#!/usr/bin/env pwsh
# Start a local PostgreSQL container for development/migrations
# Usage: ./scripts/start-postgres.ps1 [-Stop] [-Reset]
#
# Connection string (matches appsettings.json):
#   Server=localhost;Database=fsh;User Id=postgres;Password=password

param(
    [switch]$Stop,
    [switch]$Reset
)

$ErrorActionPreference = "Stop"

$ContainerName = "solverplan-postgres"
$Image         = "postgres:16-alpine"
$Port          = 5432
$DbName        = "fsh"
$DbUser        = "postgres"
$DbPassword    = "password"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SolverPlan — PostgreSQL Docker" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# --- Stop / Remove ---
if ($Stop -or $Reset) {
    $running = docker ps -q --filter "name=$ContainerName" 2>$null
    if ($running) {
        Write-Host "Stopping container '$ContainerName'..." -ForegroundColor Yellow
        docker stop $ContainerName | Out-Null
        Write-Host "  Stopped." -ForegroundColor Green
    }

    $exists = docker ps -aq --filter "name=$ContainerName" 2>$null
    if ($exists) {
        Write-Host "Removing container '$ContainerName'..." -ForegroundColor Yellow
        docker rm $ContainerName | Out-Null
        Write-Host "  Removed." -ForegroundColor Green
    }

    if ($Stop) {
        Write-Host ""
        Write-Host "Done." -ForegroundColor Green
        exit 0
    }
}

# --- Check if already running ---
$running = docker ps -q --filter "name=$ContainerName" 2>$null
if ($running) {
    Write-Host "Container '$ContainerName' is already running." -ForegroundColor Green
    Write-Host ""
    Write-Host "  Host    : localhost:$Port" -ForegroundColor White
    Write-Host "  Database: $DbName" -ForegroundColor White
    Write-Host "  User    : $DbUser" -ForegroundColor White
    Write-Host ""
    exit 0
}

# --- Start (or re-create if stopped) ---
$exists = docker ps -aq --filter "name=$ContainerName" 2>$null
if ($exists) {
    Write-Host "Starting existing container '$ContainerName'..." -ForegroundColor Yellow
    docker start $ContainerName | Out-Null
} else {
    Write-Host "Creating and starting container '$ContainerName'..." -ForegroundColor Yellow
    docker run -d `
        --name $ContainerName `
        -e POSTGRES_DB=$DbName `
        -e POSTGRES_USER=$DbUser `
        -e POSTGRES_PASSWORD=$DbPassword `
        -p "${Port}:5432" `
        --restart unless-stopped `
        $Image | Out-Null
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to start container!" -ForegroundColor Red
    exit 1
}

# --- Wait for postgres to be ready ---
Write-Host "Waiting for PostgreSQL to be ready..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0
do {
    Start-Sleep -Seconds 1
    $attempt++
    $ready = docker exec $ContainerName pg_isready -U $DbUser -d $DbName 2>$null
} while ($LASTEXITCODE -ne 0 -and $attempt -lt $maxAttempts)

if ($LASTEXITCODE -ne 0) {
    Write-Host "PostgreSQL did not become ready in time." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  PostgreSQL is ready!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  Host    : localhost:$Port" -ForegroundColor White
Write-Host "  Database: $DbName" -ForegroundColor White
Write-Host "  User    : $DbUser" -ForegroundColor White
Write-Host ""
Write-Host "Run migrations:" -ForegroundColor Cyan
Write-Host "  dotnet run --project src/Playground/Playground.Api" -ForegroundColor White
Write-Host ""
Write-Host "Stop the container:" -ForegroundColor Cyan
Write-Host "  ./scripts/start-postgres.ps1 -Stop" -ForegroundColor White
Write-Host ""
Write-Host "Reset (remove data):" -ForegroundColor Cyan
Write-Host "  ./scripts/start-postgres.ps1 -Reset" -ForegroundColor White
Write-Host ""
