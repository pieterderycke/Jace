CALL buildNuGetPackage.cmd

@ECHO OFF
FOR /F "delims=|" %%I IN ('DIR "Jace.*.nupkg" /B /O:D') DO SET NuGetPackage=%%I
@ECHO ON

Tools\NuGet\nuget.exe push %NuGetPackage% -Source https://api.nuget.org/v3/index.json