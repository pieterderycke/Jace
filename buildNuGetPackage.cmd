SET version="0.8.3"

msbuild /p:Configuration=Release Jace\Jace.csproj
MKDIR nuget\lib\net40
COPY Jace\bin\Release\*.dll nuget\lib\net40\

msbuild /p:Configuration=Release Jace.WinRT\Jace.WinRT.csproj
MKDIR nuget\lib\windows8
COPY Jace.WinRT\bin\Release\*.dll nuget\lib\windows8\

msbuild /p:Configuration=Release Jace.WP8\Jace.WP8.csproj
MKDIR nuget\lib\wp8
COPY Jace.WP8\bin\Release\*.dll nuget\lib\wp8\

msbuild /p:Configuration=Release Jace.WP7\Jace.WP7.csproj
MKDIR nuget\lib\wp7
COPY Jace.WP7\bin\Release\*.dll nuget\lib\wp7\

COPY Jace.nuspec nuget\

Tools\NuGet\nuget.exe pack nuget\Jace.nuspec -Version %version%

RMDIR nuget /S /Q