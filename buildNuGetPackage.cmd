msbuild /p:Configuration=Release Jace\Jace.csproj
Tools\NuGet\nuget.exe pack Jace\Jace.csproj -Prop Configuration=Release