@echo off
set "scriptPath=%~1"
shift
"C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\Roslyn\csi.exe" "%scriptPath%" %*