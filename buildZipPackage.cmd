SET version=0.9

msbuild /p:Configuration=Release Jace\Jace.csproj
MKDIR zip\net40
COPY Jace\bin\Release\*.dll zip\net40\

msbuild /p:Configuration=Release Jace.Portable\Jace.Portable.csproj
MKDIR zip\windows8
COPY Jace.Portable\bin\Release\*.dll zip\windows8\
MKDIR zip\wp8
COPY Jace.Portable\bin\Release\*.dll zip\wp8\
MKDIR zip\wpa81
COPY Jace.Portable\bin\Release\*.dll zip\wpa81\
MKDIR zip\xamarin.android
COPY Jace.Portable\bin\Release\*.dll zip\xamarin.android\

COPY LICENSE.md zip\
COPY README.md zip\

RM Jace.%version%.zip
Tools\7-Zip\7za.exe a -tzip Jace.%version%.zip .\zip\*

RMDIR zip /S /Q