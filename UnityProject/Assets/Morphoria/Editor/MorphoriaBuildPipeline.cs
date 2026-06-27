using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class MorphoriaBuildPipeline
{
    private const string CompanyName = "Artisaul";
    private const string ProductName = "Morphoria";
    private const string BundleVersion = "0.1.0";
    private const string MacBuildFolder = "../Builds/Morphoria-macOS";
    private const string MacAppName = "Morphoria.app";

    [MenuItem("Morphoria/Build/Mac Playtest App")]
    public static void BuildMacPlaytestApp()
    {
        BuildMac(BuildOptions.Development);
    }

    public static void BuildMacPlaytestAppBatch()
    {
        BuildMac(BuildOptions.Development);
    }

    private static void BuildMac(BuildOptions options)
    {
        ConfigurePlayerSettings();
        MorphoriaSceneBuilder.BuildGameShellScenes();
        MorphoriaProductionValidator.ValidateProductionScenes();

        string outputPath = Path.GetFullPath(Path.Combine(MacBuildFolder, MacAppName));
        string outputDirectory = Path.GetDirectoryName(outputPath);
        if (string.IsNullOrEmpty(outputDirectory))
        {
            throw new InvalidOperationException("Morphoria build output directory is invalid.");
        }

        Directory.CreateDirectory(outputDirectory);
        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = EnabledScenePaths(),
            locationPathName = outputPath,
            target = BuildTarget.StandaloneOSX,
            options = options
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
        {
            throw new Exception("Morphoria macOS build failed: " + summary.result + " (" + summary.totalErrors + " error(s)).");
        }

        Debug.Log("Morphoria macOS build succeeded: " + outputPath + " (" + (summary.totalSize / (1024f * 1024f)).ToString("0.0") + " MB)");
    }

    private static void ConfigurePlayerSettings()
    {
        PlayerSettings.companyName = CompanyName;
        PlayerSettings.productName = ProductName;
        PlayerSettings.bundleVersion = BundleVersion;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, "com.artisaul.morphoria");
    }

    private static string[] EnabledScenePaths()
    {
        List<string> scenes = new List<string>();
        EditorBuildSettingsScene[] configuredScenes = EditorBuildSettings.scenes;
        for (int i = 0; i < configuredScenes.Length; i++)
        {
            if (configuredScenes[i].enabled && File.Exists(configuredScenes[i].path))
            {
                scenes.Add(configuredScenes[i].path);
            }
        }

        if (scenes.Count == 0)
        {
            throw new Exception("No enabled scenes found in Build Settings.");
        }

        return scenes.ToArray();
    }
}
