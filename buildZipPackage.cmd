SET version=0.9.1

SET msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"

Call %msbuild% /p:Configuration=Release Jace\Jace.csproj
MKDIR zip\netstandard1.6
COPY Jace\bin\Release\netstandard1.6\*.dll zip\netstandard1.6\

COPY LICENSE.md zip\
COPY README.md zip\

RM Jace.%version%.zip
Tools\7-Zip\7za.exe a -tzip Jace.%version%.zip .\zip\*

RMDIR zip /S /Q