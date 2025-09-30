@echo off
echo Starting AgileBoard deployment...

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo Docker is not running. Please start Docker first.
    exit /b 1
)

REM Build and start services
echo Building and starting services...
docker-compose down
docker-compose build --no-cache
docker-compose up -d

REM Wait for services
echo Waiting for services to start...
timeout /t 30 /nobreak >nul

echo Deployment completed!
echo API Documentation: http://localhost:8080/swagger
echo Health Check: http://localhost:8080/health
pause