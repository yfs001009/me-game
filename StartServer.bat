@echo off
cd /d "%~dp0"
dotnet run --project Server.Fantasy\Main\Main.csproj --framework net9.0
pause
