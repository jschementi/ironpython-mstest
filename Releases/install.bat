@echo off

regedit /S %~dp0install.reg

set EXTDIR=%LOCALAPPDATA%\Microsoft\VisualStudio\10.0\Extensions\Jimmy Schementi\IronPython unit testing\1.0\
set TLBEXP_CMD=C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\TlbExp.exe
set REGASM_CMD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe
set ASM=IronPythonTools.Testing.dll

pushd %EXTDIR%

"%TLBEXP_CMD%" /nologo %ASM%
"%REGASM_CMD%" /nologo /codebase %ASM%

popd

set TEMPLATES=%userprofile%\Documents\Visual Studio 2010\Templates\ProjectTemplates

copy /Y "%~dp0IronPython Test Project.zip" "%TEMPLATES%"