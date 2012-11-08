msbuild /p:Configuration=Release Jace\Jace.csproj
Tools\NuGet\nuget.exe pack Jace\Jace.csproj -Prop Configuration=Release

@ECHO OFF
FOR /F "delims=|" %%I IN ('DIR "Jace.*.nupkg" /B /O:D') DO SET NuGetPackage=%%I
@ECHO ON

REM ECHO %NuGetPackage%
Tools\NuGet\nuget.exe push %NuGetPackage%