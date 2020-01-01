SET jaceVersion="1.0"

FOR /F "tokens=*" %%i IN ('"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe') do (SET msbuild=%%i)

Call "%msbuild%" /p:Configuration=Release Jace\Jace.csproj
MKDIR nuget\lib\netstandard1.6
COPY Jace\bin\Release\netstandard1.6\*.dll nuget\lib\netstandard1.6\

COPY Jace.nuspec nuget\
Tools\NuGet\nuget.exe pack nuget\Jace.nuspec -Version %jaceVersion%

RMDIR nuget /S /Q