#r @"packages\FAKE\tools\FakeLib.dll"
open Fake

Target "Build" (fun _ ->
    MSBuildDebug "" "Rebuild" ["WebSharper.Playground.sln"] |> Log "AppBuild-Output"
)

RunTargetOrDefault "Build"