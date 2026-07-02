@echo off
chcp 65001 >nul
echo ================================
echo    ZrAdmin.NET 停止服务
echo ================================

echo 正在停止后端和前端服务...

taskkill /FI "WINDOWTITLE eq ZrAdmin Backend*" /F >nul 2>&1
taskkill /FI "WINDOWTITLE eq ZrAdmin Frontend*" /F >nul 2>&1

echo 服务已停止！
echo.
echo 按任意键关闭此窗口...
pause >nul
