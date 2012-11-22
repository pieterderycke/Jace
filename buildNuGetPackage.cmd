SET version="0.6"

Tools\patch.exe Jace\Properties\AssemblyInfo.cs nuget\AssemblyInfo.Patch
msbuild /p:Configuration=Release Jace\Jace.csproj
MKDIR nuget\lib\net40
COPY Jace\bin\Release\*.dll nuget\lib\net40\

msbuild /p:Configuration=Release Jace.WinRT\Jace.WinRT.csproj
MKDIR nuget\lib\windows8
COPY Jace.WinRT\bin\Release\*.dll nuget\lib\windows8\

COPY Jace.nuspec nuget\

Tools\NuGet\nuget.exe pack nuget\Jace.nuspec -Version %version%

RMDIR nuget /S /Q