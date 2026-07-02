@echo off
echo ========================================
echo    ZrAdmin.NET One-Key Restart Script
echo ========================================
echo.

echo [1/3] Stopping all services...
taskkill /FI "WINDOWTITLE eq ZrAdmin-Backend*" /F >nul 2>&1
taskkill /FI "WINDOWTITLE eq ZrAdmin-Frontend*" /F >nul 2>&1
timeout /t 2 /nobreak >nul

echo [2/3] Starting Backend (port: 8888)...
start "ZrAdmin-Backend" cmd /k "dotnet watch --project ZR.Admin.WebApi run"
timeout /t 3 /nobreak >nul

echo [3/3] Starting Frontend (port: 8887)...
start "ZrAdmin-Frontend" cmd /k "cd /d ZR.Vue && npm run dev"

echo.
echo ========================================
echo  Restart Complete!
echo  Backend:  http://localhost:8888
echo  Frontend: http://localhost:8887
echo ========================================
echo.
pause
