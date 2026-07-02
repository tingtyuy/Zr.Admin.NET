@echo off
echo ========================================
echo    ZrAdmin.NET One-Key Start Script
echo ========================================
echo.

echo [1/2] Starting Backend (port: 8888)...
start "ZrAdmin-Backend" cmd /k "dotnet watch  --no-hot-reload --project ZR.Admin.WebApi run"

timeout /t 3 /nobreak >nul

echo [2/2] Starting Frontend (port: 8887)...
start "ZrAdmin-Frontend" cmd /k "cd /d ZR.Vue && npm run dev"

echo.
echo ========================================
echo  All services started!
echo  Backend:  http://localhost:8888
echo  Frontend: http://localhost:8887
echo ========================================
echo.
pause
