// include Fake libs
#r "tools/Fake.Core/tools/FakeLib.dll"
#r "tools/Fake.IIS/tools/Fake.IIS.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.IISExpress

RestorePackages()

let authors = ["AutoScout24"]

// project name and description
let projectName = "FeatureBee"
let projectDescription = "Release your features without a release."
let projectSummary = projectDescription


// Directories
let buildDir        = "./build/"
let testResultsDir  = "./testresults/"
let deployDir       = "./deploy/"
let packagingDir    = "./packaging/"
let websiteDir      = "./build/_PublishedWebsites/"

let project = "FeatureBee.Server"
let hostName = "localhost"
let port = 51100

let releaseNotes =
    ReadFile "ReleaseNotes.md"
    |> ReleaseNotesHelper.parseReleaseNotes

let version = getBuildParamOrDefault "PackageVersion" releaseNotes.AssemblyVersion

let buildMode = getBuildParamOrDefault "buildMode" "Release"

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir; testResultsDir; deployDir; packagingDir]
)

Target "SetVersion" (fun _ ->
    CreateCSharpAssemblyInfo "./SolutionInfo.cs"
      [ Attribute.Product projectName
        Attribute.Version version
        Attribute.FileVersion version
        Attribute.ComVisible false]
)

Target "BuildApp" (fun _ ->
    // compile all projects below src/app/
     MSBuild buildDir "Build" ["Configuration", buildMode] ["./FeatureBee.sln"]
        |> Log "AppBuild-Output: "
)

Target "NUnitTest" (fun _ ->  
    !! ("./build/FeatureBee*.dll") 
        |> NUnit (fun p -> 
            {p with
                DisableShadowCopy = true; 
                OutputFile = testResultsDir + "TestResults.xml"})
)

Target "MSpecTest" (fun _ ->
    !! ("./build/FeatureBee*.dll") 
        |> MSpec (fun p -> 
            {p with 
                ExcludeTags = ["LongRunning"]
                HtmlOutputDir = testResultsDir})
)

Target "CreatePackage" (fun _ ->
    let net45Dir = packagingDir @@ "lib/net45/"
    CleanDirs [net45Dir]

    CopyFile net45Dir (buildDir @@ "FeatureBee.Client.dll")
    CopyFile net45Dir (buildDir @@ "FeatureBee.Client.pdb")
    CopyFiles packagingDir ["LICENSE.txt"; "README.md"; "ReleaseNotes.md"]

    NuGet (fun p ->
        {p with
            Authors = authors
            Project = projectName
            Description = projectDescription
            OutputPath = packagingDir
            Summary = projectSummary
            WorkingDir = packagingDir
            Version = version
            ReleaseNotes = toLines releaseNotes.Notes
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey"
            Dependencies = ["Microsoft.AspNet.SignalR.Client", GetPackageVersion "./packages/" "Microsoft.AspNet.SignalR.Client"]
             }) "featurebee.nuspec"
)

Target "AcceptanceTest" (fun _ ->

    let config = createConfigFile(project, 1, "iisexpress-template.config", websiteDir + "/" + project, hostName, port)
    let webSiteProcess = HostWebsite id config 1

    System.Threading.Thread.Sleep 5000

    let result =
        ExecProcess (fun info ->
            info.FileName <- (buildDir @@ "FeatureBee.Server.Acceptance.exe")
            info.WorkingDirectory <- buildDir
        ) (System.TimeSpan.FromMinutes 5.)

    ProcessHelper.killProcessById webSiteProcess.Id
 
    if result <> 0 then failwith "Failed result from acceptance tests"
)

Target "Zip" (fun _ ->
    !! (buildDir + "/_PublishedWebsites/FeatureBee.Server/**/*.*") 
        -- "*.zip"
        -- "**/*.mdf"
        -- "**/*.ldf"
        |> Zip (buildDir + "/_PublishedWebsites/FeatureBee.Server/") (deployDir + "FeatureBee.Server." + version + ".zip")
)

Target "All" DoNothing

// Build order
"Clean"
  ==> "SetVersion"
  ==> "BuildApp"
  ==> "NUnitTest"
  ==> "MSpecTest"
  ==> "AcceptanceTest"
  ==> "CreatePackage"
  ==> "Zip"
  ==> "All"

// start build
RunTargetOrDefault "All"
