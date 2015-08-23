SET version="0.9"

msbuild /p:Configuration=Release Jace\Jace.csproj
MKDIR nuget\lib\net40
COPY Jace\bin\Release\*.dll nuget\lib\net40\

msbuild /p:Configuration=Release Jace.Portable\Jace.Portable.csproj
MKDIR nuget\lib\windows8
COPY Jace.Portable\bin\Release\*.dll nuget\lib\windows8\
MKDIR nuget\lib\wpa81
COPY Jace.Portable\bin\Release\*.dll nuget\lib\wpa81\
MKDIR nuget\portable-net451+win8+wpa81+wp8
COPY Jace.Portable\bin\Release\*.dll nuget\lib\portable-net451+win8+wpa81+wp8\
MKDIR nuget\lib\dotnet
COPY Jace.Portable\bin\Release\*.dll nuget\lib\dotnet\

msbuild /p:Configuration=Release Jace.WP8\Jace.WP8.csproj
MKDIR nuget\lib\wp8
COPY Jace.WP8\bin\Release\*.dll nuget\lib\wp8\

msbuild /p:Configuration=Release Jace.Android\Jace.Android.csproj
MKDIR nuget\lib\monoandroid
COPY Jace.Android\bin\Release\*.dll nuget\lib\monoandroid\

COPY Jace.nuspec nuget\

Tools\NuGet\nuget.exe pack nuget\Jace.nuspec -Version %version%

RMDIR nuget /S /Q