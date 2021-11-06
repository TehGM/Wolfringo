:: ad-hoc script for packing all packages
:: designed to produce local-only pre-release versions for testing

@echo off
set outputdir=bin\output

:: clean output directories
if exist %outputdir% rmdir /s /q %outputdir%
mkdir %outputdir%

:: restore dependencies
dotnet restore

:: pack each project
dotnet pack Wolfringo.Core -c Debug -o %outputdir% --no-restore
dotnet pack Wolfringo.Commands -c Debug -o %outputdir% --no-restore
dotnet pack Wolfringo.Utilities -c Debug -o %outputdir% --no-restore
dotnet pack Wolfringo.Utilities.Interactive -c Debug -o %outputdir% --no-restore
dotnet pack Wolfringo.Hosting -c Debug -o %outputdir% --no-restore

:: pack metapackage
nuget pack "Wolfringo\Wolfringo.nuspec" -Exclude "*.*" -BasePath "Wolfringo" -NonInteractive -OutputDirectory %outputdir%