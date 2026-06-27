using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MorphoriaReferenceImporter
{
    private const string UnityReferenceRoot = "Assets/Morphoria/Art/References";

    [MenuItem("Morphoria/Sync Visual References")]
    public static void SyncReferences()
    {
        string repositoryRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
        DeleteRootReferenceImages();
        CopyFolder(Path.Combine(repositoryRoot, "references", "visual_cards"), UnityReferenceRoot + "/visual_cards");
        CopyFolder(Path.Combine(repositoryRoot, "references", "concept_cards"), UnityReferenceRoot + "/concept_cards");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void DeleteRootReferenceImages()
    {
        if (!Directory.Exists(UnityReferenceRoot))
        {
            return;
        }

        string[] files = Directory.GetFiles(UnityReferenceRoot, "*.png", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < files.Length; i++)
        {
            string assetPath = files[i].Replace("\\", "/");
            AssetDatabase.DeleteAsset(assetPath);
        }
    }

    private static void CopyFolder(string sourceFolder, string targetAssetFolder)
    {
        if (!Directory.Exists(sourceFolder))
        {
            Debug.LogWarning("Morphoria reference folder not found: " + sourceFolder);
            return;
        }

        Directory.CreateDirectory(targetAssetFolder);
        string[] files = Directory.GetFiles(sourceFolder, "*.png", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < files.Length; i++)
        {
            string targetAssetPath = targetAssetFolder + "/" + Path.GetFileName(files[i]);
            string targetAbsolutePath = Path.GetFullPath(targetAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetAbsolutePath));
            File.Copy(files[i], targetAbsolutePath, true);
            AssetDatabase.ImportAsset(targetAssetPath, ImportAssetOptions.ForceUpdate);
        }
    }
}
