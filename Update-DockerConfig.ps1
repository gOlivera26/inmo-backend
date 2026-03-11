param(
    [Parameter(Mandatory=$true)]
    [string]$NewName,
    [switch]$SkipConfirmation
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Docker & Observability Config Updater" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($NewName -match '[^a-zA-Z0-9_.-]') {
    Write-Host "Error: El nombre solo puede contener letras, numeros, guiones y puntos." -ForegroundColor Red
    exit 1
}

$currentDir = $PSScriptRoot
if ([string]::IsNullOrEmpty($currentDir)) {
    $currentDir = Get-Location
}

Write-Host "Directorio: $currentDir" -ForegroundColor Yellow
Write-Host "Nuevo nombre: $NewName" -ForegroundColor Yellow
Write-Host ""

if (-not $SkipConfirmation) {
    Write-Host "Este script actualizara:" -ForegroundColor Yellow
    Write-Host "  - docker-compose.yml (servicios, contenedores, DB)" -ForegroundColor White
    Write-Host "  - observability/prometheus-config.yaml (job name)" -ForegroundColor White
    Write-Host "  - observability/tempo-config.yaml" -ForegroundColor White
    Write-Host "  - observability/loki-config.yaml" -ForegroundColor White
    Write-Host ""

    $response = Read-Host "Continuar? (S/N)"
    if ($response -ne 'S' -and $response -ne 's') {
        Write-Host "Operacion cancelada." -ForegroundColor Yellow
        exit 0
    }
}

Write-Host ""
Write-Host "[1/4] Actualizando docker-compose.yml..." -ForegroundColor Green

$dockerComposePath = Join-Path $currentDir "docker-compose.yml"
if (Test-Path $dockerComposePath) {
    $content = Get-Content $dockerComposePath -Raw -Encoding UTF8
    
    $content = $content -replace "Inmo24-api", "$NewName-api"
    $content = $content -replace "Inmo24", "$NewName"
    $content = $content -replace "Inmo24-API", "$NewName-API"
    $content = $content -replace "Inmo24-net", "$NewName-net"
    
    Set-Content -Path $dockerComposePath -Value $content -Encoding UTF8 -NoNewline
    Write-Host "  docker-compose.yml actualizado" -ForegroundColor White
}

Write-Host "[2/4] Actualizando prometheus-config.yaml..." -ForegroundColor Green

$prometheusPath = Join-Path $currentDir "observability\prometheus-config.yaml"
if (Test-Path $prometheusPath) {
    $content = Get-Content $prometheusPath -Raw -Encoding UTF8
    $content = $content -replace "Inmo24-api", "$NewName-api"
    
    Set-Content -Path $prometheusPath -Value $content -Encoding UTF8 -NoNewline
    Write-Host "  prometheus-config.yaml actualizado" -ForegroundColor White
}

Write-Host "[3/4] Actualizando tempo-config.yaml..." -ForegroundColor Green

$tempoPath = Join-Path $currentDir "observability\tempo-config.yaml"
if (Test-Path $tempoPath) {
    Write-Host "  tempo-config.yaml no requiere cambios" -ForegroundColor Gray
}

Write-Host "[4/4] Actualizando loki-config.yaml..." -ForegroundColor Green

$lokiPath = Join-Path $currentDir "observability\loki-config.yaml"
if (Test-Path $lokiPath) {
    Write-Host "  loki-config.yaml no requiere cambios" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Proceso completado!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Archivos actualizados:" -ForegroundColor Yellow
Write-Host "  - docker-compose.yml" -ForegroundColor White
Write-Host "  - observability/prometheus-config.yaml" -ForegroundColor White
Write-Host ""
Write-Host "Puedes ejecutar 'docker-compose up --build' para probar." -ForegroundColor Yellow
