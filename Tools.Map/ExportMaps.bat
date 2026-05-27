@echo off
setlocal
cd /d %~dp0

set TILED=%~dp0Tiled\tiled.exe
set MAP_SRC=%~dp0TiledProject\Maps
set MAP_OUT=%~dp0..\Client.Unity\Assets\AssetRaw\Maps

if not exist "%MAP_OUT%" mkdir "%MAP_OUT%"

for %%f in ("%MAP_SRC%\*.tmx") do (
    echo Exporting %%~nxf
    "%TILED%" --export-map json "%%~f" "%MAP_OUT%\%%~nf.json"
    if errorlevel 1 exit /b %errorlevel%
)

echo Done.
endlocal
