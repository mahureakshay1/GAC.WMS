$solutionRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$testProjectPath = Join-Path $solutionRoot "GAC.WMS.UnitTests"
$containerName = "gacwms"
$hostFolder = "C:\SharedData"
$containerFolder = "/app/shared"


Write-Host "Building and running test cases..." -ForegroundColor Cyan
dotnet test "$testProjectPath\GAC.WMS.UnitTests.csproj" --no-restore --logger "trx;LogFileName=test-results.trx"
if ($LASTEXITCODE -ne 0) {
   Write-Host "Tests failed. Aborting deployment." -ForegroundColor Red
   exit 1
}

Write-Host "Building Docker image..." -ForegroundColor Cyan
docker rm -f gacwms-db-1
docker rm -f gacwms-server-1
docker compose up --build
docker exec -i gacwms-db-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Akshay@123!" -i /docker-entrypoint-initdb.d/init.sql -C
