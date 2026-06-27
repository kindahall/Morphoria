using System;
using System.Collections.Generic;
using System.IO;
using Morphoria;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MorphoriaProductionValidator
{
    private const string SceneFolder = "Assets/Morphoria/Scenes";

    [MenuItem("Morphoria/Validate Production Scenes")]
    public static void ValidateProductionScenes()
    {
        List<string> issues = new List<string>();
        List<string> expectedScenePaths = ExpectedScenePaths();

        ValidateBuildSettings(expectedScenePaths, issues);
        ValidateVisualReferences(issues);

        for (int i = 0; i < expectedScenePaths.Count; i++)
        {
            ValidateScene(expectedScenePaths[i], issues);
        }

        if (issues.Count > 0)
        {
            for (int i = 0; i < issues.Count; i++)
            {
                Debug.LogError("Morphoria validation: " + issues[i]);
            }

            throw new Exception("Morphoria production validation failed with " + issues.Count + " issue(s).");
        }

        Debug.Log("Morphoria production validation passed for " + expectedScenePaths.Count + " scene(s).");
    }

    private static List<string> ExpectedScenePaths()
    {
        List<string> paths = new List<string>
        {
            ScenePath(MorphoriaGameContent.MainMenuScene),
            ScenePath(MorphoriaGameContent.HubScene),
            ScenePath(MorphoriaGameContent.WorldMapScene)
        };

        for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
        {
            paths.Add(ScenePath(MorphoriaGameContent.Levels[i].sceneName));
        }

        return paths;
    }

    private static string ScenePath(string sceneName)
    {
        return SceneFolder + "/" + sceneName + ".unity";
    }

    private static void ValidateBuildSettings(List<string> expectedScenePaths, List<string> issues)
    {
        Dictionary<string, bool> buildScenes = new Dictionary<string, bool>();
        EditorBuildSettingsScene[] configuredScenes = EditorBuildSettings.scenes;
        for (int i = 0; i < configuredScenes.Length; i++)
        {
            buildScenes[configuredScenes[i].path] = configuredScenes[i].enabled;
        }

        for (int i = 0; i < expectedScenePaths.Count; i++)
        {
            string path = expectedScenePaths[i];
            if (!File.Exists(path))
            {
                issues.Add("Missing scene file: " + path);
            }

            if (!buildScenes.TryGetValue(path, out bool enabled))
            {
                issues.Add("Scene is not in Build Settings: " + path);
            }
            else if (!enabled)
            {
                issues.Add("Scene is disabled in Build Settings: " + path);
            }
        }

        if (configuredScenes.Length > 0 && configuredScenes[0].path != ScenePath(MorphoriaGameContent.MainMenuScene))
        {
            issues.Add("MainMenu must be the first Build Settings scene.");
        }
    }

    private static void ValidateScene(string scenePath, List<string> issues)
    {
        if (!File.Exists(scenePath))
        {
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        GameObject[] roots = scene.GetRootGameObjects();

        if (roots.Length == 0)
        {
            issues.Add(sceneName + ": scene has no root objects.");
            return;
        }

        for (int i = 0; i < roots.Length; i++)
        {
            ValidateMissingScripts(sceneName, roots[i], issues);
        }

        Camera[] cameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include);
        AudioListener[] audioListeners = UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include);

        if (cameras.Length == 0)
        {
            issues.Add(sceneName + ": missing Camera.");
        }

        if (audioListeners.Length != 1)
        {
            issues.Add(sceneName + ": expected exactly one AudioListener, found " + audioListeners.Length + ".");
        }

        if (sceneName == MorphoriaGameContent.MainMenuScene)
        {
            RequireOne<MorphoriaMenuScreen>(sceneName, issues);
        }
        else if (sceneName == MorphoriaGameContent.WorldMapScene)
        {
            RequireOne<MorphoriaWorldMapScreen>(sceneName, issues);
        }
        else if (sceneName == MorphoriaGameContent.HubScene)
        {
            RequireOne<MorphoriaPlayer>(sceneName, issues);
            RequireOne<ThirdPersonCamera>(sceneName, issues);
            RequireOne<MorphoriaHud>(sceneName, issues);
            RequireOne<MorphoriaHubState>(sceneName, issues);
            RequireAtLeast<MorphoriaScenePortal>(sceneName, 2, issues);
            MorphoriaHud[] hubHuds = UnityEngine.Object.FindObjectsByType<MorphoriaHud>(FindObjectsInactive.Include);
            if (hubHuds.Length == 1 && hubHuds[0].showLevelGoals)
            {
                issues.Add(sceneName + ": hub HUD should not show level goal counters.");
            }
        }
        else
        {
            ValidatePlayableLevel(sceneName, issues);
        }

        ValidateScenePortals(sceneName, issues);
    }

    private static void ValidatePlayableLevel(string sceneName, List<string> issues)
    {
        MorphoriaLevelInfo level = MorphoriaGameContent.GetLevelByScene(sceneName);
        if (level == null)
        {
            issues.Add(sceneName + ": no MorphoriaLevelInfo found for scene.");
            return;
        }

        RequireOne<MorphoriaPlayer>(sceneName, issues);
        RequireOne<ThirdPersonCamera>(sceneName, issues);
        RequireOne<MorphoriaHud>(sceneName, issues);
        RequireOne<MorphoriaPauseMenu>(sceneName, issues);
        RequireOne<MorphoriaLevelResultScreen>(sceneName, issues);
        RequireOne<LevelExit>(sceneName, issues);
        RequireAtLeast<MorphoriaCollectible>(sceneName, 8, issues);

        LevelExit[] exits = UnityEngine.Object.FindObjectsByType<LevelExit>(FindObjectsInactive.Include);
        VillagerCage[] cages = UnityEngine.Object.FindObjectsByType<VillagerCage>(FindObjectsInactive.Include);
        MorphoriaCollectible[] collectibles = UnityEngine.Object.FindObjectsByType<MorphoriaCollectible>(FindObjectsInactive.Include);
        MorphoriaHud[] huds = UnityEngine.Object.FindObjectsByType<MorphoriaHud>(FindObjectsInactive.Include);

        int goldenStars = 0;
        int prismStars = 0;
        for (int i = 0; i < collectibles.Length; i++)
        {
            if (collectibles[i].kind == CollectibleKind.GoldenStar)
            {
                goldenStars += Mathf.Max(0, collectibles[i].amount);
            }
            else if (collectibles[i].kind == CollectibleKind.ChoiceStar || collectibles[i].kind == CollectibleKind.PrismStar)
            {
                prismStars += Mathf.Max(0, collectibles[i].amount);
            }
        }

        if (goldenStars != level.targetGoldenStars)
        {
            issues.Add(sceneName + ": expected " + level.targetGoldenStars + " golden stars, found " + goldenStars + ".");
        }

        if (prismStars != level.targetPrismStars)
        {
            issues.Add(sceneName + ": expected " + level.targetPrismStars + " prism stars, found " + prismStars + ".");
        }

        if (cages.Length != level.targetVillagers)
        {
            issues.Add(sceneName + ": expected " + level.targetVillagers + " villager cage(s), found " + cages.Length + ".");
        }

        if (huds.Length == 1)
        {
            MorphoriaHud hud = huds[0];
            if (!hud.showLevelGoals)
            {
                issues.Add(sceneName + ": level HUD must show level goal counters.");
            }

            if (hud.targetGoldenStars != level.targetGoldenStars || hud.targetPrismStars != level.targetPrismStars || hud.targetVillagers != level.targetVillagers)
            {
                issues.Add(sceneName + ": HUD targets do not match MorphoriaLevelInfo.");
            }
        }

        for (int i = 0; i < exits.Length; i++)
        {
            if (exits[i].requiredVillagers > cages.Length)
            {
                issues.Add(sceneName + ": exit requires " + exits[i].requiredVillagers + " villagers but scene has " + cages.Length + " cage(s).");
            }
        }
    }

    private static void ValidateVisualReferences(List<string> issues)
    {
        string[] visualCards =
        {
            "01_histoire_point_de_depart.png",
            "02_carte_du_monde_lumeria.png",
            "03_village_ecloria_evolution.png",
            "04_carte_premier_niveau_pont_quatre_formes.png",
            "05_obstacles_interactions_formes.png",
            "06_ennemis_faiblesses.png",
            "07_interface_hud_roue_des_formes.png"
        };

        for (int i = 0; i < visualCards.Length; i++)
        {
            string path = "Assets/Morphoria/Art/References/visual_cards/" + visualCards[i];
            if (!File.Exists(path))
            {
                issues.Add("Missing Unity visual reference: " + path);
            }
        }

        string conceptFolder = "Assets/Morphoria/Art/References/concept_cards";
        int conceptCount = Directory.Exists(conceptFolder) ? Directory.GetFiles(conceptFolder, "*.png", SearchOption.TopDirectoryOnly).Length : 0;
        if (conceptCount < 9)
        {
            issues.Add("Expected at least 9 Unity concept references, found " + conceptCount + ".");
        }
    }

    private static void ValidateMissingScripts(string sceneName, GameObject gameObject, List<string> issues)
    {
        Component[] components = gameObject.GetComponentsInChildren<Component>(true);
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                issues.Add(sceneName + ": missing script on " + GetHierarchyPath(gameObject) + ".");
            }
        }
    }

    private static void ValidateScenePortals(string sceneName, List<string> issues)
    {
        MorphoriaScenePortal[] portals = UnityEngine.Object.FindObjectsByType<MorphoriaScenePortal>(FindObjectsInactive.Include);
        for (int i = 0; i < portals.Length; i++)
        {
            MorphoriaScenePortal portal = portals[i];
            bool hasSceneTarget = !string.IsNullOrEmpty(portal.targetScene);
            bool hasLevelTarget = !string.IsNullOrEmpty(portal.targetLevelId);

            if (!hasSceneTarget && !hasLevelTarget)
            {
                issues.Add(sceneName + ": portal " + GetHierarchyPath(portal.gameObject) + " has no target.");
            }

            if (hasSceneTarget && !File.Exists(ScenePath(portal.targetScene)))
            {
                issues.Add(sceneName + ": portal " + GetHierarchyPath(portal.gameObject) + " targets missing scene " + portal.targetScene + ".");
            }

            if (hasLevelTarget && !KnownLevelId(portal.targetLevelId))
            {
                issues.Add(sceneName + ": portal " + GetHierarchyPath(portal.gameObject) + " targets unknown level " + portal.targetLevelId + ".");
            }
        }
    }

    private static bool KnownLevelId(string levelId)
    {
        for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
        {
            if (MorphoriaGameContent.Levels[i].id == levelId)
            {
                return true;
            }
        }

        return false;
    }

    private static void RequireOne<T>(string sceneName, List<string> issues) where T : UnityEngine.Object
    {
        T[] objects = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include);
        if (objects.Length != 1)
        {
            issues.Add(sceneName + ": expected exactly one " + typeof(T).Name + ", found " + objects.Length + ".");
        }
    }

    private static void RequireAtLeast<T>(string sceneName, int minimum, List<string> issues) where T : UnityEngine.Object
    {
        T[] objects = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include);
        if (objects.Length < minimum)
        {
            issues.Add(sceneName + ": expected at least " + minimum + " " + typeof(T).Name + " object(s), found " + objects.Length + ".");
        }
    }

    private static string GetHierarchyPath(GameObject gameObject)
    {
        string path = gameObject.name;
        Transform current = gameObject.transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}
