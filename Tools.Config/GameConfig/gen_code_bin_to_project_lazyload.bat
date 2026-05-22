Cd /d %~dp0
echo %CD%

set WORKSPACE=../..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.
set DATA_OUTPATH=%WORKSPACE%/Client.Unity/Assets/AssetRaw/Configs/bytes/
set CODE_OUTPATH=%WORKSPACE%/Client.Unity/Assets/GameScripts/HotFix/GameProto/GameConfig/

copy /y "%CONF_ROOT%\CustomTemplate\ConfigSystem.cs" "%WORKSPACE%\Client.Unity\Assets\GameScripts\HotFix\GameProto\ConfigSystem.cs"
copy /y "%CONF_ROOT%\CustomTemplate\ExternalTypeUtil.cs" "%WORKSPACE%\Client.Unity\Assets\GameScripts\HotFix\GameProto\ExternalTypeUtil.cs"

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin^
    --conf %CONF_ROOT%\luban.conf ^
    --customTemplateDir %CONF_ROOT%\CustomTemplate\CustomTemplate_Client_LazyLoad ^
    -x code.lineEnding=crlf ^
    -x outputCodeDir=%CODE_OUTPATH% ^
    -x outputDataDir=%DATA_OUTPATH% 
if errorlevel 1 exit /b %errorlevel%
if not defined AI_MODE pause
