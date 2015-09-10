#r @"packages\FAKE\tools\FakeLib.dll"
open Fake

Target "Build" (fun _ ->
    MSBuildRelease "" "Rebuild" ["Application1/Application1.fsproj"] |> Log "AppBuild-Output"
)

RunTargetOrDefault "Build"