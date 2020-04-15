@echo off

".\.nuget\nuget.exe" "install" "FAKE.IIS" "-OutputDirectory" "tools" "-ExcludeVersion" "-version" "2.13.3"
".\.nuget\nuget.exe" "install" "FAKE.Core" "-OutputDirectory" "tools" "-ExcludeVersion" "-version" "2.13.3"

:Build
cls

SET TARGET="All"

IF NOT [%1]==[] (set TARGET="%1")

SET BUILDMODE="Release"
IF NOT [%2]==[] (set BUILDMODE="%2")

:: because we want to run specific steps inline on qed
:: we need to break the dependency chain
:: this ensures we do a build before running any tests

if TARGET=="All" (SET RunBuild=1)
if TARGET=="RunUnitTests" (SET RunBuild=1)
if TARGET=="RunIntegrationTests" (SET RunBuild=1)
if TARGET=="CreatePackages" (SET RunBuild=1)

if "%BUILD_ID%"=="" (
"tools\FAKE.Core\tools\Fake.exe" "build.fsx" "target=BuildApp" "buildMode=%BUILDMODE%"
)

if NOT "%BUILD_ID%"=="" (
"tools\FAKE.Core\tools\Fake.exe" "build.fsx" "target=%TARGET%" "buildMode=%BUILDMODE%" "PackageVersion=%BUILD_ID%"
)

rem Bail if we're running a TeamCity build.
if defined TEAMCITY_PROJECT_NAME goto Quit

rem Bail if we're running a MyGet build.
if /i "%BuildRunner%"=="MyGet" goto Quit

:Quit
exit /b %errorlevel%