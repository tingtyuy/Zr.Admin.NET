@echo off
chcp 65001 >nul
echo ========================================
echo    ZrAdmin.NET One-Key Publish Script
echo ========================================
echo.

set PROJECT_DIR=%~dp0
set BACKEND_SRC=%PROJECT_DIR%ZR.Admin.WebApi
set FRONTEND_SRC=%PROJECT_DIR%ZR.Vue
set BACKEND_PUBLISH=%PROJECT_DIR%publish\backend
set FRONTEND_DIST=%FRONTEND_SRC%\dist
set IIS_BACKEND=D:\wwwroot\AI.API
set IIS_FRONTEND=D:\wwwroot\AI.Web

:: ==================== Build ====================
echo [1/5] Publishing Backend...
dotnet publish "%BACKEND_SRC%" -c Release -o "%BACKEND_PUBLISH%" --nologo
if %errorlevel% neq 0 (
    echo [ERROR] Backend publish failed!
    pause
    exit /b 1
)
echo      Backend publish succeeded.
echo.

echo [2/5] Building Frontend...
cd /d "%FRONTEND_SRC%"
call npm run build:prod
if %errorlevel% neq 0 (
    echo [ERROR] Frontend build failed!
    pause
    exit /b 1
)
cd /d "%PROJECT_DIR%"
echo      Frontend build succeeded.
echo.

:: ==================== Stop IIS Apps ====================
echo [3/5] Stopping IIS applications...
%windir%\system32\inetsrv\appcmd stop apppool /apppool.name:"AI.API" >nul 2>&1
%windir%\system32\inetsrv\appcmd stop apppool /apppool.name:"AI.Web" >nul 2>&1
timeout /t 2 /nobreak >nul
echo      IIS applications stopped.
echo.

:: ==================== Deploy ====================
echo [4/5] Deploying to IIS...

echo      Copying backend to %IIS_BACKEND%...
if not exist "%IIS_BACKEND%" mkdir "%IIS_BACKEND%"
xcopy "%BACKEND_PUBLISH%\*" "%IIS_BACKEND%\" /s /e /y /q

echo      Copying frontend to %IIS_FRONTEND%...
if not exist "%IIS_FRONTEND%" mkdir "%IIS_FRONTEND%"
xcopy "%FRONTEND_DIST%\*" "%IIS_FRONTEND%\" /s /e /y /q

echo      Deploy completed.
echo.

:: ==================== Restart IIS Apps ====================
echo [5/5] Starting IIS applications...
%windir%\system32\inetsrv\appcmd start apppool /apppool.name:"AI.API" >nul 2>&1
%windir%\system32\inetsrv\appcmd start apppool /apppool.name:"AI.Web" >nul 2>&1

echo.
echo ========================================
echo  Publish completed successfully!
echo  Backend:  http://192.168.3.8:6601
echo  Frontend: http://192.168.3.8:6602
echo ========================================
echo.
pause
