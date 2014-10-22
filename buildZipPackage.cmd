SET version=0.8.6

msbuild /p:Configuration=Release Jace\Jace.csproj
MKDIR zip\net40
COPY Jace\bin\Release\*.dll zip\net40\

msbuild /p:Configuration=Release Jace.WinRT\Jace.WinRT.csproj
MKDIR zip\windows8
COPY Jace.WinRT\bin\Release\*.dll zip\windows8\

msbuild /p:Configuration=Release Jace.WP8\Jace.WP8.csproj
MKDIR zip\wp8
COPY Jace.WP8\bin\Release\*.dll zip\wp8\
MKDIR zip\wpa81
COPY Jace.WinRT\bin\Release\*.dll zip\wpa81\

msbuild /p:Configuration=Release Jace.WP7\Jace.WP7.csproj
MKDIR zip\wp7
COPY Jace.WP7\bin\Release\*.dll zip\wp7\

REM msbuild /p:Configuration=Release Jace.Android\Jace.Android.csproj
MKDIR zip\xamarin.android
COPY Jace.Android\bin\Release\*.dll zip\xamarin.android\

COPY LICENSE.md zip\
COPY README.md zip\

RM Jace.%version%.zip
Tools\7-Zip\7za.exe a -tzip Jace.%version%.zip .\zip\*

RMDIR zip /S /Q