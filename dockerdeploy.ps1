$solutionRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$testProjectPath = Join-Path $solutionRoot "GAC.WMS.UnitTests"

Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore

Write-Host "Building the solution..." -ForegroundColor Cyan
dotnet build --no-restore

Write-Host "Running test cases..." -ForegroundColor Cyan
dotnet test "$testProjectPath\GAC.WMS.UnitTests.csproj" --no-restore --no-build --logger "trx;LogFileName=test-results.trx"
if ($LASTEXITCODE -ne 0) {
  Write-Host "Tests failed. Aborting deployment." -ForegroundColor Red
  exit 1
}

$exists_db = docker ps -a --format '{{.Names}}' | Where-Object { $_ -eq "gacwms-db-1" }
if($exists_db)
{
   Write-Host "Container gacwms-db-1 exists. Stopping and removing..."
   docker stop "gacwms-db-1" | Out-Null
   docker rm "gacwms-db-1" | Out-Null
   Write-Host "Container gacwms-db-1 removed."
}
$exists_server = docker ps -a --format '{{.Names}}' | Where-Object { $_ -eq "gacwms-server-1" }

if($exists_server)
{
   Write-Host "Container gacwms-server-1 exists. Stopping and removing..."
   docker stop "gacwms-server-1" | Out-Null
   docker rm "gacwms-server-1" | Out-Null
   Write-Host "Container gacwms-server-1 removed."
}

Write-Host "Building Docker image..." -ForegroundColor Cyan
docker compose up --build -d