msbuild /p:Configuration=Release Jace\Jace.csproj
Tools\NuGet\nuget.exe pack Jace\Jace.csproj -Version 0.5 -Prop Configuration=Release