@echo off
REM =========================================
REM Quick Rename: SGOFAPI to PHCAPI
REM =========================================
echo.
echo ========================================
echo   Renomeacao Rapida: SGOFAPI - PHCAPI
echo ========================================
echo.
echo Este script ira renomear todo o projeto.
echo Um backup sera criado automaticamente.
echo.
pause

powershell -ExecutionPolicy Bypass -File "Rename-Project.ps1"

echo.
echo ========================================
echo   Concluido!
echo ========================================
echo.
echo Proximos passos:
echo 1. Feche o Visual Studio
echo 2. Limpe bin/obj
echo 3. Reabra o Visual Studio
echo 4. Rebuild Solution
echo.
pause
