SET version="0.9"

msbuild /p:Configuration=Release Jace\Jace.csproj
MKDIR nuget\lib\net40
COPY Jace\bin\Release\*.dll nuget\lib\net40\

msbuild /p:Configuration=Release Jace.Portable\Jace.Portable.csproj
MKDIR nuget\lib\portable-net451+win8+wpa81+wp8
COPY Jace.Portable\bin\Release\*.dll "nuget\lib\portable-net451+win8+wpa81+wp8\"
MKDIR nuget\lib\monoandroid
COPY Jace.Portable\bin\Release\*.dll nuget\lib\monoandroid\

COPY Jace.nuspec nuget\

Tools\NuGet\nuget.exe pack nuget\Jace.nuspec -Version %version%

RMDIR nuget /S /Q