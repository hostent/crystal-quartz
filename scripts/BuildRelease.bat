FOR /F "TOKENS=3" %%A IN ('reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0" /v MSBuildToolsPath') DO SET MSBuildPath=%%Amsbuild

%MSBuildPath% /t:BuildAndPackArtifacts /p:Platform="Any CPU" /p:Configuration=Release Toolbox.proj

pause