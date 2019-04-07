SET jaceVersion="0.9.3"

SET msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"

Call %msbuild% /p:Configuration=Release Jace\Jace.csproj
MKDIR nuget\lib\netstandard1.6
COPY Jace\bin\Release\netstandard1.6\*.dll nuget\lib\netstandard1.6\

COPY Jace.nuspec nuget\
Tools\NuGet\nuget.exe pack nuget\Jace.nuspec -Version %jaceVersion%

RMDIR nuget /S /Q