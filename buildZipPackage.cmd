SET version=1.0

FOR /F "tokens=*" %%i IN ('"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe') do (SET msbuild=%%i)

Call "%msbuild%" /p:Configuration=Release Jace\Jace.csproj
MKDIR zip\netstandard1.6
COPY Jace\bin\Release\netstandard1.6\*.dll zip\netstandard1.6\

COPY LICENSE.md zip\
COPY README.md zip\

RM Jace.%version%.zip
Tools\7-Zip\7za.exe a -tzip Jace.%version%.zip .\zip\*

RMDIR zip /S /Q