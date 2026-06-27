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
        ValidateCharacterPrefabs(issues);
        ValidateCampaignProgression(issues);
        ValidateHubRestorationProgression(issues);

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

    private static void ValidateCampaignProgression(List<string> issues)
    {
        MorphoriaSaveData data = MorphoriaSaveSystem.CreateNew();
        if (data.levels.Count != MorphoriaGameContent.Levels.Length)
        {
            issues.Add("Campaign save should track " + MorphoriaGameContent.Levels.Length + " levels, found " + data.levels.Count + ".");
            return;
        }

        for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.Levels[i];
            MorphoriaLevelProgress progress = MorphoriaSaveSystem.GetProgress(data, level.id);
            bool shouldBeUnlocked = i == 0;
            if (progress.unlocked != shouldBeUnlocked)
            {
                issues.Add("New campaign unlock state is wrong for " + level.id + ".");
            }
        }

        int expectedGolden = 0;
        int expectedPrism = 0;
        int expectedVillagers = 0;
        for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.Levels[i];
            MorphoriaLevelClearResult result = MorphoriaCampaignProgression.MarkLevelComplete(data, level.id, level.targetGoldenStars, level.targetPrismStars, level.targetVillagers);
            expectedGolden += level.targetGoldenStars;
            expectedPrism += level.targetPrismStars;
            expectedVillagers += level.targetVillagers;

            if (!result.firstClear || !result.newBest || result.rank != "Prisme")
            {
                issues.Add("Perfect first clear result is wrong for " + level.id + ".");
            }

            MorphoriaLevelProgress progress = MorphoriaSaveSystem.GetProgress(data, level.id);
            if (!progress.completed || progress.clears != 1)
            {
                issues.Add("Campaign clear did not complete " + level.id + ".");
            }

            MorphoriaLevelInfo next = MorphoriaGameContent.GetNextLevel(level.id);
            if (next != null)
            {
                MorphoriaLevelProgress nextProgress = MorphoriaSaveSystem.GetProgress(data, next.id);
                if (!result.unlockedNextLevel || result.campaignComplete || !nextProgress.unlocked || result.nextLevelId != next.id || result.nextLevelName != next.displayName)
                {
                    issues.Add("Campaign clear did not unlock next level after " + level.id + ".");
                }
            }
            else if (!data.finalBossDefeated || !result.campaignComplete || result.unlockedNextLevel || result.nextLevelId != string.Empty || result.nextLevelName != string.Empty)
            {
                issues.Add("Final campaign clear did not mark Noctar defeated correctly.");
            }
        }

        if (data.totalGoldenStars != expectedGolden || data.totalPrismStars != expectedPrism || data.totalVillagersSaved != expectedVillagers)
        {
            issues.Add("Campaign totals are wrong after full clear.");
        }

        MorphoriaLevelInfo firstLevel = MorphoriaGameContent.Levels[0];
        string goldenId = firstLevel.id + "_golden_validation";
        string prismId = firstLevel.id + "_prism_validation";
        string villagerId = firstLevel.id + "_villager_validation";
        if (!MorphoriaSaveSystem.RecordCollected(data, firstLevel.id, goldenId, CollectibleKind.GoldenStar) || !MorphoriaSaveSystem.HasCollected(data, firstLevel.id, goldenId, CollectibleKind.GoldenStar))
        {
            issues.Add("Campaign save should persist collected golden object ids.");
        }

        if (!MorphoriaSaveSystem.RecordCollected(data, firstLevel.id, prismId, CollectibleKind.ChoiceStar) || !MorphoriaSaveSystem.HasCollected(data, firstLevel.id, prismId, CollectibleKind.ChoiceStar))
        {
            issues.Add("Campaign save should persist collected prism object ids.");
        }

        if (!MorphoriaSaveSystem.RecordRescuedVillager(data, firstLevel.id, villagerId) || !MorphoriaSaveSystem.HasRescuedVillager(data, firstLevel.id, villagerId))
        {
            issues.Add("Campaign save should persist rescued villager ids.");
        }

        if (MorphoriaSaveSystem.RecordCollected(data, firstLevel.id, goldenId, CollectibleKind.GoldenStar) || MorphoriaSaveSystem.RecordRescuedVillager(data, firstLevel.id, villagerId))
        {
            issues.Add("Campaign save should ignore duplicate persistent object ids.");
        }

        MorphoriaLevelClearResult replay = MorphoriaCampaignProgression.MarkLevelComplete(data, firstLevel.id, 1, 0, 0);
        MorphoriaLevelProgress firstProgress = MorphoriaSaveSystem.GetProgress(data, firstLevel.id);
        if (replay.firstClear || replay.newBest || firstProgress.clears != 2 || firstProgress.bestGoldenStars != firstLevel.targetGoldenStars)
        {
            issues.Add("Replay clear should preserve best results while incrementing clears.");
        }
    }

    private static List<string> ExpectedScenePaths()
    {
        List<string> paths = new List<string>
        {
            ScenePath(MorphoriaGameContent.MainMenuScene),
            ScenePath(MorphoriaGameContent.HubScene),
            ScenePath(MorphoriaGameContent.WorldMapScene),
            ScenePath(MorphoriaGameContent.FinaleScene)
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

        RequireOne<MorphoriaFeedbackSystem>(sceneName, issues);
        ValidateFeedbackSystem(sceneName, issues);

        if (sceneName == MorphoriaGameContent.MainMenuScene)
        {
            RequireOne<MorphoriaMenuScreen>(sceneName, issues);
            ValidateMainMenuCharacters(sceneName, issues);
        }
        else if (sceneName == MorphoriaGameContent.WorldMapScene)
        {
            RequireOne<MorphoriaWorldMapScreen>(sceneName, issues);
            ValidateWorldMapScene(sceneName, issues);
        }
        else if (sceneName == MorphoriaGameContent.FinaleScene)
        {
            RequireOne<MorphoriaFinaleScreen>(sceneName, issues);
            ValidateFinaleCharacters(sceneName, issues);
        }
        else if (sceneName == MorphoriaGameContent.HubScene)
        {
            RequireOne<MorphoriaPlayer>(sceneName, issues);
            RequireOne<PlayerInventory>(sceneName, issues);
            RequireOne<MorphoriaProceduralAnimator>(sceneName, issues);
            RequireOne<ThirdPersonCamera>(sceneName, issues);
            ValidateThirdPersonCamera(sceneName, issues);
            RequireOne<MorphoriaHud>(sceneName, issues);
            RequireOne<MorphoriaGameOverScreen>(sceneName, issues);
            RequireOne<MorphoriaHubState>(sceneName, issues);
            RequireOne<MorphoriaHubRestoration>(sceneName, issues);
            RequireAtLeast<MorphoriaScenePortal>(sceneName, 2, issues);
            ValidatePlayerControllerFeel(sceneName, issues);
            ValidatePlayerInventory(sceneName, issues);
            ValidateHubRestorationScene(sceneName, issues);
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

        ValidatePlayerAvatarPrefabs(sceneName, issues);
        ValidateScenePortals(sceneName, issues);
    }

    private static void ValidateHubRestorationProgression(List<string> issues)
    {
        MorphoriaSaveData data = MorphoriaSaveSystem.CreateNew();
        if (MorphoriaHubRestoration.CalculateStage(data) != 0)
        {
            issues.Add("Fresh save should show damaged village restoration stage.");
        }

        MorphoriaLevelInfo firstLevel = MorphoriaGameContent.Levels[0];
        MorphoriaCampaignProgression.MarkLevelComplete(data, firstLevel.id, firstLevel.targetGoldenStars, firstLevel.targetPrismStars, firstLevel.targetVillagers);
        if (MorphoriaHubRestoration.CalculateStage(data) < 1)
        {
            issues.Add("First level clear should advance the village restoration stage.");
        }

        for (int i = 1; i < MorphoriaGameContent.Levels.Length; i++)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.Levels[i];
            MorphoriaCampaignProgression.MarkLevelComplete(data, level.id, level.targetGoldenStars, level.targetPrismStars, level.targetVillagers);
        }

        if (MorphoriaHubRestoration.CalculateStage(data) != 3)
        {
            issues.Add("Full campaign clear should restore Ecloria completely.");
        }
    }

    private static void ValidateHubRestorationScene(string sceneName, List<string> issues)
    {
        MorphoriaHubRestoration[] restorations = UnityEngine.Object.FindObjectsByType<MorphoriaHubRestoration>(FindObjectsInactive.Include);
        if (restorations.Length != 1)
        {
            return;
        }

        MorphoriaHubRestoration restoration = restorations[0];
        if (restoration.damagedStage == null || restoration.damagedStage.Length == 0)
        {
            issues.Add(sceneName + ": hub restoration needs a damaged stage.");
        }

        if (restoration.repairedStage == null || restoration.repairedStage.Length == 0)
        {
            issues.Add(sceneName + ": hub restoration needs a repaired stage.");
        }

        if (restoration.gardenStage == null || restoration.gardenStage.Length == 0)
        {
            issues.Add(sceneName + ": hub restoration needs a garden stage.");
        }

        if (restoration.finaleStage == null || restoration.finaleStage.Length == 0)
        {
            issues.Add(sceneName + ": hub restoration needs a finale stage.");
        }

        if (restoration.heartLight == null)
        {
            issues.Add(sceneName + ": hub restoration should control the prism heart light.");
        }
    }

    private static void ValidateWorldMapScene(string sceneName, List<string> issues)
    {
        MorphoriaWorldMapNode[] nodes = UnityEngine.Object.FindObjectsByType<MorphoriaWorldMapNode>(FindObjectsInactive.Include);
        MorphoriaWorldMapRoute[] routes = UnityEngine.Object.FindObjectsByType<MorphoriaWorldMapRoute>(FindObjectsInactive.Include);

        if (nodes.Length != MorphoriaGameContent.Levels.Length)
        {
            issues.Add(sceneName + ": expected " + MorphoriaGameContent.Levels.Length + " dynamic map nodes, found " + nodes.Length + ".");
        }

        if (routes.Length != MorphoriaGameContent.Levels.Length - 1)
        {
            issues.Add(sceneName + ": expected " + (MorphoriaGameContent.Levels.Length - 1) + " dynamic map routes, found " + routes.Length + ".");
        }

        HashSet<string> nodeIds = new HashSet<string>();
        for (int i = 0; i < nodes.Length; i++)
        {
            MorphoriaWorldMapNode node = nodes[i];
            if (string.IsNullOrEmpty(node.levelId))
            {
                issues.Add(sceneName + ": map node " + node.name + " needs a level id.");
                continue;
            }

            if (!nodeIds.Add(node.levelId))
            {
                issues.Add(sceneName + ": duplicate map node for " + node.levelId + ".");
            }

            if (node.lockedVisual == null || node.unlockedVisual == null || node.completedVisual == null || node.glowLight == null || node.stateLabel == null)
            {
                issues.Add(sceneName + ": map node " + node.levelId + " is missing visual state references.");
            }
        }

        for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
        {
            if (!nodeIds.Contains(MorphoriaGameContent.Levels[i].id))
            {
                issues.Add(sceneName + ": missing map node for " + MorphoriaGameContent.Levels[i].id + ".");
            }
        }

        for (int i = 0; i < routes.Length; i++)
        {
            MorphoriaWorldMapRoute route = routes[i];
            if (string.IsNullOrEmpty(route.fromLevelId) || string.IsNullOrEmpty(route.toLevelId))
            {
                issues.Add(sceneName + ": map route " + route.name + " needs level ids.");
            }

            if (route.lockedVisual == null || route.unlockedVisual == null || route.routeLight == null)
            {
                issues.Add(sceneName + ": map route " + route.name + " is missing visual state references.");
            }
        }
    }

    private static void ValidateMainMenuCharacters(string sceneName, List<string> issues)
    {
        string[,] expected =
        {
            { "Menu_Rokko_Character", "Assets/Morphoria/Prefabs/Characters/PF_Rokko.prefab" },
            { "Menu_Luma_Character", "Assets/Morphoria/Prefabs/Characters/PF_Luma.prefab" },
            { "Menu_Papyra_Character", "Assets/Morphoria/Prefabs/Characters/PF_Papyra.prefab" },
            { "Menu_Cizo_Character", "Assets/Morphoria/Prefabs/Characters/PF_Cizo.prefab" }
        };

        for (int i = 0; i < expected.GetLength(0); i++)
        {
            string objectName = expected[i, 0];
            string prefabPath = expected[i, 1];
            GameObject visual = GameObject.Find(objectName);
            if (visual == null)
            {
                issues.Add(sceneName + ": missing menu character " + objectName + ".");
                continue;
            }

            string actualPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(visual);
            if (actualPath != prefabPath)
            {
                issues.Add(sceneName + ": menu character " + objectName + " is not linked to " + prefabPath + ".");
            }
        }
    }

    private static void ValidateFinaleCharacters(string sceneName, List<string> issues)
    {
        string[,] expected =
        {
            { "Finale_Rokko_Character", "Assets/Morphoria/Prefabs/Characters/PF_Rokko.prefab" },
            { "Finale_Luma_Character", "Assets/Morphoria/Prefabs/Characters/PF_Luma.prefab" },
            { "Finale_Papyra_Character", "Assets/Morphoria/Prefabs/Characters/PF_Papyra.prefab" },
            { "Finale_Cizo_Character", "Assets/Morphoria/Prefabs/Characters/PF_Cizo.prefab" },
            { "Finale_Noctar_Redeemed", "Assets/Morphoria/Prefabs/Characters/PF_Noctar.prefab" }
        };

        for (int i = 0; i < expected.GetLength(0); i++)
        {
            string objectName = expected[i, 0];
            string prefabPath = expected[i, 1];
            GameObject visual = GameObject.Find(objectName);
            if (visual == null)
            {
                issues.Add(sceneName + ": missing finale character " + objectName + ".");
                continue;
            }

            string actualPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(visual);
            if (actualPath != prefabPath)
            {
                issues.Add(sceneName + ": finale character " + objectName + " is not linked to " + prefabPath + ".");
            }
        }
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
        RequireOne<PlayerInventory>(sceneName, issues);
        RequireOne<MorphoriaProceduralAnimator>(sceneName, issues);
        RequireOne<ThirdPersonCamera>(sceneName, issues);
        ValidateThirdPersonCamera(sceneName, issues);
        RequireOne<MorphoriaHud>(sceneName, issues);
        RequireOne<MorphoriaPauseMenu>(sceneName, issues);
        RequireOne<MorphoriaGameOverScreen>(sceneName, issues);
        RequireOne<MorphoriaLevelResultScreen>(sceneName, issues);
        RequireOne<LevelExit>(sceneName, issues);
        RequireAtLeast<Checkpoint>(sceneName, 1, issues);
        RequireAtLeast<MorphoriaEnemy>(sceneName, 2, issues);
        RequireAtLeast<MorphoriaCollectible>(sceneName, 8, issues);
        ValidatePlayerControllerFeel(sceneName, issues);
        ValidatePlayerInventory(sceneName, issues);
        ValidateRouteLanguage(sceneName, issues);
        ValidateWorldLandmarks(sceneName, issues);
        if (level.targetVillagers > 0)
        {
            RequireOne<MiniBoss>(sceneName, issues);
            ValidateMiniBossVisual(sceneName, issues);
        }

        LevelExit[] exits = UnityEngine.Object.FindObjectsByType<LevelExit>(FindObjectsInactive.Include);
        VillagerCage[] cages = UnityEngine.Object.FindObjectsByType<VillagerCage>(FindObjectsInactive.Include);
        MorphoriaCollectible[] collectibles = UnityEngine.Object.FindObjectsByType<MorphoriaCollectible>(FindObjectsInactive.Include);
        MorphoriaHud[] huds = UnityEngine.Object.FindObjectsByType<MorphoriaHud>(FindObjectsInactive.Include);

        int goldenStars = 0;
        int prismStars = 0;
        HashSet<string> collectibleIds = new HashSet<string>();
        for (int i = 0; i < collectibles.Length; i++)
        {
            if (string.IsNullOrEmpty(collectibles[i].persistentId))
            {
                issues.Add(sceneName + ": collectible " + collectibles[i].name + " needs a persistent id.");
            }
            else if (!collectibleIds.Add(collectibles[i].persistentId))
            {
                issues.Add(sceneName + ": duplicate collectible persistent id " + collectibles[i].persistentId + ".");
            }

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

        HashSet<string> cageIds = new HashSet<string>();
        for (int i = 0; i < cages.Length; i++)
        {
            if (string.IsNullOrEmpty(cages[i].persistentId))
            {
                issues.Add(sceneName + ": villager cage " + cages[i].name + " needs a persistent id.");
            }
            else if (!cageIds.Add(cages[i].persistentId))
            {
                issues.Add(sceneName + ": duplicate villager cage persistent id " + cages[i].persistentId + ".");
            }
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

            if (level.targetVillagers > 0 && hud.miniBoss == null)
            {
                issues.Add(sceneName + ": HUD should reference the level mini-boss for objective and boss feedback.");
            }
        }

        for (int i = 0; i < exits.Length; i++)
        {
            if (exits[i].requiredVillagers != level.targetVillagers)
            {
                issues.Add(sceneName + ": exit villager requirement should match MorphoriaLevelInfo.");
            }

            if (exits[i].requiredVillagers > cages.Length)
            {
                issues.Add(sceneName + ": exit requires " + exits[i].requiredVillagers + " villagers but scene has " + cages.Length + " cage(s).");
            }
        }
    }

    private static void ValidateFeedbackSystem(string sceneName, List<string> issues)
    {
        MorphoriaFeedbackSystem[] systems = UnityEngine.Object.FindObjectsByType<MorphoriaFeedbackSystem>(FindObjectsInactive.Include);
        if (systems.Length != 1)
        {
            return;
        }

        AudioSource[] sources = systems[0].GetComponents<AudioSource>();
        if (sources.Length < 2)
        {
            issues.Add(sceneName + ": feedback system needs separate AudioSources for cues and runtime ambience.");
        }

        if (!systems[0].emitPulseRings || !systems[0].emitFlashLights)
        {
            issues.Add(sceneName + ": feedback system visual rings and flashes should be enabled.");
        }
    }

    private static void ValidatePlayerInventory(string sceneName, List<string> issues)
    {
        PlayerInventory[] inventories = UnityEngine.Object.FindObjectsByType<PlayerInventory>(FindObjectsInactive.Include);
        if (inventories.Length != 1)
        {
            return;
        }

        PlayerInventory inventory = inventories[0];
        if (inventory.startingHearts < 3)
        {
            issues.Add(sceneName + ": player should start with at least three hearts.");
        }

        if (inventory.startingChoiceStars < 1)
        {
            issues.Add(sceneName + ": player should start with choice stars for form switching.");
        }
    }

    private static void ValidatePlayerControllerFeel(string sceneName, List<string> issues)
    {
        MorphoriaPlayer[] players = UnityEngine.Object.FindObjectsByType<MorphoriaPlayer>(FindObjectsInactive.Include);
        if (players.Length != 1)
        {
            return;
        }

        MorphoriaPlayer player = players[0];
        if (player.coyoteTime <= 0f || player.jumpBufferTime <= 0f)
        {
            issues.Add(sceneName + ": player needs coyote time and jump buffering for platforming feel.");
        }

        if (player.airControlMultiplier <= 0f || player.airControlMultiplier > 1f)
        {
            issues.Add(sceneName + ": player air control multiplier should be between 0 and 1.");
        }

        if (player.fallGravityMultiplier < 1f || player.earlyJumpReleaseMultiplier < 1f)
        {
            issues.Add(sceneName + ": player gravity multipliers must not soften falls below normal gravity.");
        }

        if (player.maxFallSpeed < 12f)
        {
            issues.Add(sceneName + ": player max fall speed is too low for readable platforming.");
        }

        if (player.landingCameraImpulse <= 0f)
        {
            issues.Add(sceneName + ": player should send a subtle camera impulse on hard landings.");
        }
    }

    private static void ValidateRouteLanguage(string sceneName, List<string> issues)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
        int routeGroups = 0;
        int totemBases = 0;
        int totemLights = 0;
        int routeRails = 0;

        for (int i = 0; i < transforms.Length; i++)
        {
            string name = transforms[i].name;
            if (name.EndsWith("_Route_Language", StringComparison.Ordinal))
            {
                routeGroups++;
            }

            if (name.Contains("_Totem_") && name.EndsWith("_Base", StringComparison.Ordinal))
            {
                totemBases++;
            }

            if (name.Contains("_Totem_") && name.EndsWith("_Light", StringComparison.Ordinal))
            {
                totemLights++;
            }

            if (name.Contains("_Rail_") && (name.EndsWith("_Left", StringComparison.Ordinal) || name.EndsWith("_Right", StringComparison.Ordinal)))
            {
                routeRails++;
            }
        }

        if (routeGroups < 1)
        {
            issues.Add(sceneName + ": playable level needs a Route_Language group for readable path landmarks.");
        }

        if (totemBases < 6 || totemLights < 6)
        {
            issues.Add(sceneName + ": playable level needs at least six route totems with lights.");
        }

        if (routeRails < 10)
        {
            issues.Add(sceneName + ": playable level needs at least ten route rail strips.");
        }
    }

    private static void ValidateWorldLandmarks(string sceneName, List<string> issues)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
        int landmarkGroups = 0;
        int landmarkPieces = 0;
        int landmarkLights = 0;

        for (int i = 0; i < transforms.Length; i++)
        {
            string name = transforms[i].name;
            if (name.StartsWith("WorldLandmarks_", StringComparison.Ordinal))
            {
                landmarkGroups++;
            }

            if (name.Contains("_Landmark_"))
            {
                landmarkPieces++;
            }

            if (name.Contains("_Landmark_") && name.EndsWith("_Light", StringComparison.Ordinal))
            {
                landmarkLights++;
            }
        }

        if (landmarkGroups < 1)
        {
            issues.Add(sceneName + ": playable level needs a WorldLandmarks group to preserve visual-card identity.");
        }

        if (landmarkPieces < 18)
        {
            issues.Add(sceneName + ": playable level needs at least eighteen landmark pieces for readable world silhouettes.");
        }

        if (landmarkLights < 2)
        {
            issues.Add(sceneName + ": playable level needs at least two lit landmarks for premium magical readability.");
        }
    }

    private static void ValidateMiniBossVisual(string sceneName, List<string> issues)
    {
        MiniBoss[] bosses = UnityEngine.Object.FindObjectsByType<MiniBoss>(FindObjectsInactive.Include);
        for (int i = 0; i < bosses.Length; i++)
        {
            MiniBoss boss = bosses[i];
            CapsuleCollider collider = boss.GetComponent<CapsuleCollider>();
            if (collider == null || !collider.isTrigger)
            {
                issues.Add(sceneName + ": mini-boss needs a trigger CapsuleCollider.");
            }

            if (boss.renderers == null || boss.renderers.Length < 5)
            {
                issues.Add(sceneName + ": mini-boss renderer list does not include the Noctar visual.");
            }

            Transform visual = boss.transform.Find("Noctar_Boss_Visual");
            if (visual == null)
            {
                issues.Add(sceneName + ": mini-boss is missing Noctar_Boss_Visual.");
                continue;
            }

            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(visual.gameObject);
            if (prefabPath != "Assets/Morphoria/Prefabs/Characters/PF_Noctar.prefab")
            {
                issues.Add(sceneName + ": mini-boss visual is not linked to PF_Noctar.prefab.");
            }

            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevelByScene(sceneName);
            if (level != null && level.worldId == "fortress")
            {
                ValidateNoctarFinalSequence(sceneName, boss, issues);
            }
            else if (boss.weaknessSequence != null && boss.weaknessSequence.Length > 0)
            {
                issues.Add(sceneName + ": only the Noctar fortress boss should use a weakness sequence.");
            }
        }
    }

    private static void ValidateNoctarFinalSequence(string sceneName, MiniBoss boss, List<string> issues)
    {
        MorphoriaAbility[] sequence = boss.weaknessSequence;
        if (sequence == null || sequence.Length < 4)
        {
            issues.Add(sceneName + ": Noctar final boss needs a four-form weakness sequence.");
            return;
        }

        bool hasStone = ContainsAbility(sequence, MorphoriaAbility.Break);
        bool hasLeaf = ContainsAbility(sequence, MorphoriaAbility.Glide);
        bool hasPaper = ContainsAbility(sequence, MorphoriaAbility.Fold);
        bool hasScissors = ContainsAbility(sequence, MorphoriaAbility.Cut);
        if (!hasStone || !hasLeaf || !hasPaper || !hasScissors)
        {
            issues.Add(sceneName + ": Noctar final boss sequence must use Stone, Leaf, Paper, and Scissors.");
        }

        if (boss.maxHealth != sequence.Length)
        {
            issues.Add(sceneName + ": Noctar final boss health must match its weakness sequence length.");
        }

        string[] runeNames = { "Noctar_Rune_Rokko", "Noctar_Rune_Luma", "Noctar_Rune_Papyra", "Noctar_Rune_Cizo" };
        for (int i = 0; i < runeNames.Length; i++)
        {
            if (GameObject.Find(runeNames[i]) == null)
            {
                issues.Add(sceneName + ": missing final boss rune " + runeNames[i] + ".");
            }
        }
    }

    private static bool ContainsAbility(MorphoriaAbility[] abilities, MorphoriaAbility ability)
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i] == ability)
            {
                return true;
            }
        }

        return false;
    }

    private static void ValidateThirdPersonCamera(string sceneName, List<string> issues)
    {
        ThirdPersonCamera[] cameras = UnityEngine.Object.FindObjectsByType<ThirdPersonCamera>(FindObjectsInactive.Include);
        if (cameras.Length != 1)
        {
            return;
        }

        ThirdPersonCamera camera = cameras[0];
        if (camera.target == null)
        {
            issues.Add(sceneName + ": ThirdPersonCamera has no target.");
        }

        if (camera.minDistance <= 0f || camera.maxDistance <= camera.minDistance || camera.distance < camera.minDistance || camera.distance > camera.maxDistance)
        {
            issues.Add(sceneName + ": ThirdPersonCamera has invalid zoom distances.");
        }

        if (camera.collisionRadius <= 0.05f || camera.collisionPadding <= 0.01f || camera.collisionRetreatSharpness <= 0f)
        {
            issues.Add(sceneName + ": ThirdPersonCamera collision tuning is incomplete.");
        }

        if (camera.recenterDelay <= 0f || camera.recenterSharpness <= 0f || camera.pitchRecenterSharpness <= 0f)
        {
            issues.Add(sceneName + ": ThirdPersonCamera recenter tuning is incomplete.");
        }

        if (camera.defaultPitch < camera.minPitch || camera.defaultPitch > camera.maxPitch)
        {
            issues.Add(sceneName + ": ThirdPersonCamera default pitch is outside pitch limits.");
        }

        if (camera.lookAheadDistance <= 0f || camera.lookAheadSharpness <= 0f)
        {
            issues.Add(sceneName + ": ThirdPersonCamera look-ahead tuning is incomplete.");
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

    private static void ValidateCharacterPrefabs(List<string> issues)
    {
        string[] prefabPaths =
        {
            "Assets/Morphoria/Prefabs/Characters/PF_Rokko.prefab",
            "Assets/Morphoria/Prefabs/Characters/PF_Luma.prefab",
            "Assets/Morphoria/Prefabs/Characters/PF_Papyra.prefab",
            "Assets/Morphoria/Prefabs/Characters/PF_Cizo.prefab",
            "Assets/Morphoria/Prefabs/Characters/PF_Noctar.prefab"
        };

        string[][] requiredAnimatedParts =
        {
            new[] { "left_eye", "right_eye", "left_fist", "right_fist", "amber_crack", "scarf" },
            new[] { "left_eye", "right_eye", "left_wing", "right_wing", "leaf_crown", "orange_scarf" },
            new[] { "left_eye", "right_eye", "fold_left", "fold_right", "paper_rune", "paper_hat" },
            new[] { "left_eye", "right_eye", "left_blade", "right_blade", "left_handle", "right_handle", "blue_scarf" },
            new[] { "left_eye", "right_eye", "left_shoulder", "right_shoulder", "crown_mid" }
        };

        for (int i = 0; i < prefabPaths.Length; i++)
        {
            string path = prefabPaths[i];
            if (!File.Exists(path))
            {
                issues.Add("Missing character prefab: " + path);
                continue;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                issues.Add("Character prefab cannot be loaded: " + path);
                continue;
            }

            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length < 5)
            {
                issues.Add("Character prefab has too few renderers: " + path + " (" + renderers.Length + ").");
            }

            for (int j = 0; j < requiredAnimatedParts[i].Length; j++)
            {
                string partName = requiredAnimatedParts[i][j];
                if (prefab.transform.Find(partName) == null)
                {
                    issues.Add("Character prefab missing animated part " + partName + ": " + path + ".");
                }
            }
        }
    }

    private static void ValidatePlayerAvatarPrefabs(string sceneName, List<string> issues)
    {
        MorphoriaPlayer[] players = UnityEngine.Object.FindObjectsByType<MorphoriaPlayer>(FindObjectsInactive.Include);
        if (players.Length == 0)
        {
            return;
        }

        MorphoriaAvatar[] avatars = UnityEngine.Object.FindObjectsByType<MorphoriaAvatar>(FindObjectsInactive.Include);
        for (int i = 0; i < avatars.Length; i++)
        {
            if (!avatars[i].HasAllFormPrefabs)
            {
                issues.Add(sceneName + ": player avatar does not reference all four form prefabs.");
            }
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

            if (!((object)portal is IFormInteractable))
            {
                issues.Add(sceneName + ": portal " + GetHierarchyPath(portal.gameObject) + " is not interactable.");
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
