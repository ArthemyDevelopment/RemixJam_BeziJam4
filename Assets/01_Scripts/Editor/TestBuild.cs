using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class TestBuild
{
    [MenuItem("Build/Test Build Windows")]
    public static void BuildWindows()
    {
        Debug.Log("Starting test build...");
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/00_Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = "Builds/TestBuild/TestGame.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.Development;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed");
        }
    }
}