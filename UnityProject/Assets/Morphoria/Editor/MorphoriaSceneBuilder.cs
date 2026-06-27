using System;
using System.Collections.Generic;
using System.IO;
using Morphoria;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class MorphoriaSceneBuilder
{
    private const string MainMenuPath = "Assets/Morphoria/Scenes/MainMenu.unity";
    private const string HubPath = "Assets/Morphoria/Scenes/VillageEcloriaHub.unity";
    private const string WorldMapPath = "Assets/Morphoria/Scenes/WorldMap.unity";
    private const string ScenePath = "Assets/Morphoria/Scenes/LePontDesQuatreFormes.unity";
    private const string MaterialFolder = "Assets/Morphoria/Materials";

    private static Material stoneMat;
    private static Material leafMat;
    private static Material paperMat;
    private static Material scissorsMat;
    private static Material neutralMat;
    private static Material darkRockMat;
    private static Material goldMat;
    private static Material prismMat;
    private static Material crystalMat;
    private static Material windMat;
    private static Material dangerMat;

    [InitializeOnLoadMethod]
    private static void AutoBuildOnFirstOpen()
    {
        EditorApplication.delayCall += () =>
        {
            string absoluteScenePath = Path.Combine(Directory.GetCurrentDirectory(), ScenePath);
            string absoluteMenuPath = Path.Combine(Directory.GetCurrentDirectory(), MainMenuPath);
            if (!Application.isBatchMode && (!File.Exists(absoluteScenePath) || !File.Exists(absoluteMenuPath)))
            {
                BuildGameShellScenes();
            }
        };
    }

    [MenuItem("Morphoria/Build Vertical Slice Scene")]
    public static void BuildVerticalSliceScene()
    {
        BuildVerticalSliceScene(true);
    }

    private static void BuildVerticalSliceScene(bool updateBuildSettings)
    {
        EnsureFolders();
        CreateMaterials();
        MorphoriaPrefabBuilder.BuildCharacterPrefabs();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = MorphoriaGameContent.LevelOneScene;

        RenderSettings.ambientLight = new Color(0.48f, 0.55f, 0.68f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.55f, 0.72f, 0.92f);
        RenderSettings.fogDensity = 0.0085f;

        Transform root = new GameObject("Morphoria_VerticalSlice").transform;
        Transform environment = new GameObject("Floating_Islands_And_Path").transform;
        Transform obstacles = new GameObject("Form_Obstacles").transform;
        Transform enemies = new GameObject("Standard_Enemies").transform;
        Transform collectibles = new GameObject("Collectibles_50_Golden_5_Prism").transform;
        Transform cages = new GameObject("Villager_Cages").transform;
        environment.SetParent(root);
        obstacles.SetParent(root);
        enemies.SetParent(root);
        collectibles.SetParent(root);
        cages.SetParent(root);

        CreateLighting();
        CreateFeedbackSystem();
        MorphoriaPlayer player = CreatePlayer();
        Camera camera = CreateCamera(player.transform);
        player.mainCamera = camera;
        MorphoriaHud hud = CreateHud(player);
        ConfigureHudForLevel(hud, MorphoriaGameContent.Levels[0]);
        CreatePauseMenu();
        CreateLevelResultScreen();

        Vector3[] path =
        {
            new Vector3(-36f, 1.1f, 0f),
            new Vector3(-24f, 1.1f, 0f),
            new Vector3(-11f, 3.15f, 4f),
            new Vector3(3f, 1.1f, 0f),
            new Vector3(17f, 1.1f, -3f),
            new Vector3(31f, 1.1f, 0f),
            new Vector3(45f, 1.1f, 0f),
            new Vector3(58f, 1.1f, 2f)
        };

        CreateIsland("Depart_Ecloria", new Vector3(-36f, 0f, 0f), new Vector3(8f, 1f, 8f), neutralMat, environment);
        CreateIsland("Zone_Pierre_Rokko", new Vector3(-24f, 0f, 0f), new Vector3(12f, 1f, 8f), stoneMat, environment);
        CreateIsland("Zone_Feuille_Luma", new Vector3(-11f, 2f, 4f), new Vector3(13f, 1f, 8f), leafMat, environment);
        CreateIsland("Zone_Papier_Papyra", new Vector3(3f, 0f, 0f), new Vector3(12f, 1f, 8f), paperMat, environment);
        CreateIsland("Zone_Ciseaux_Cizo", new Vector3(17f, 0f, -3f), new Vector3(12f, 1f, 8f), scissorsMat, environment);
        CreateIsland("Puzzle_Combine", new Vector3(31f, 0f, 0f), new Vector3(14f, 1f, 10f), neutralMat, environment);
        CreateIsland("Arene_Garde_Cage", new Vector3(45f, 0f, 0f), new Vector3(15f, 1f, 12f), darkRockMat, environment);
        CreateIsland("Sortie_Portail", new Vector3(58f, 0f, 2f), new Vector3(10f, 1f, 8f), neutralMat, environment);

        CreateBridge("Pont_Depart_Pierre", new Vector3(-30f, 0.1f, 0f), new Vector3(6f, 0.3f, 2.4f), stoneMat, environment);
        CreateBridge("Pont_Papier_Ciseaux", new Vector3(10f, 0.1f, -1.5f), new Vector3(7f, 0.3f, 2f), paperMat, environment);
        CreateBridge("Pont_Ciseaux_Puzzle", new Vector3(24f, 0.1f, -1.5f), new Vector3(7f, 0.3f, 2f), scissorsMat, environment);
        CreateBridge("Pont_Puzzle_Arene", new Vector3(38f, 0.1f, 0f), new Vector3(7f, 0.3f, 2.4f), neutralMat, environment);
        CreateBridge("Pont_Arene_Sortie", new Vector3(51.5f, 0.1f, 1f), new Vector3(6f, 0.3f, 2.2f), crystalMat, environment);

        GameObject fragileBridge = CreateBridge("Pont_Fragile_Evite_Rokko", new Vector3(-17.4f, 1.15f, 2.2f), new Vector3(5.8f, 0.26f, 2.1f), leafMat, environment);
        fragileBridge.AddComponent<FragilePlatform>();
        BoxCollider fragileTrigger = fragileBridge.AddComponent<BoxCollider>();
        fragileTrigger.isTrigger = true;
        fragileTrigger.size = new Vector3(1.02f, 2.2f, 1.02f);

        GameObject origamiBridge = CreateBridge("Pont_Origami_Active_Par_Rune", new Vector3(6.5f, 0.1f, 2.9f), new Vector3(5f, 0.25f, 1.5f), paperMat, environment);
        origamiBridge.SetActive(false);

        GameObject puzzleGate = CreateGate("Porte_Puzzle_Pierre", new Vector3(35.7f, 1.35f, 0f), new Vector3(0.8f, 2.7f, 5.2f), stoneMat, obstacles);
        GameObject cableGate = CreateGate("Barriere_Cable_Cizo", new Vector3(22f, 1.35f, -3f), new Vector3(0.8f, 2.7f, 4.2f), dangerMat, obstacles);

        CreateAbilityObstacle("Mur_Fissure_Rokko", new Vector3(-25.6f, 1.4f, 0f), new Vector3(1.1f, 2.8f, 5f), stoneMat, MorphoriaAbility.Break, "Mur brise", obstacles);
        CreateAbilityObstacle("Bloc_Lourd_Rokko", new Vector3(-21.2f, 1.0f, 2.25f), new Vector3(2.3f, 2f, 2.3f), stoneMat, MorphoriaAbility.PushHeavy, "Bloc deplace", obstacles);

        CreateWindZone(new Vector3(-11f, 3.25f, 4.2f), new Vector3(5.4f, 4.8f, 5.4f), obstacles);
        CreateBouncePad(new Vector3(-6.7f, 2.75f, 5.4f), obstacles);

        CreateAbilityObstacle("Passage_Mince_Papyra", new Vector3(1.1f, 1.25f, 0f), new Vector3(0.75f, 2.5f, 5f), paperMat, MorphoriaAbility.Fold, "Passage ouvert", obstacles);
        AbilityGate rune = CreateAbilityObstacle("Rune_Papier_Pont_Origami", new Vector3(5.3f, 0.68f, -1.75f), new Vector3(1.4f, 0.18f, 1.4f), prismMat, MorphoriaAbility.Fold, "Rune couverte", obstacles);
        rune.destroyOnSuccess = false;
        rune.activateOnSuccess = new[] { origamiBridge };

        CreateAbilityObstacle("Liane_Cizo", new Vector3(15.2f, 1.75f, -3.1f), new Vector3(0.34f, 3.9f, 0.34f), leafMat, MorphoriaAbility.Cut, "Liane coupee", obstacles, true);
        AbilityGate cable = CreateAbilityObstacle("Cable_Mecanique_Cizo", new Vector3(20.8f, 1.75f, -3.1f), new Vector3(0.28f, 4.8f, 0.28f), scissorsMat, MorphoriaAbility.Cut, "Cable coupe", obstacles, true);
        cable.deactivateOnSuccess = new[] { cableGate };

        CreateHeavyPlate(new Vector3(29.2f, 0.65f, 2.9f), puzzleGate, obstacles);
        CreateWindZone(new Vector3(31.2f, 1.8f, -3.1f), new Vector3(4f, 3.3f, 3.2f), obstacles);
        CreateAbilityObstacle("Rune_Puzzle_Papyra", new Vector3(32.8f, 0.68f, -2.6f), new Vector3(1.25f, 0.18f, 1.25f), prismMat, MorphoriaAbility.Fold, "Rune scellee", obstacles);
        CreateAbilityObstacle("Filet_Final_Cizo", new Vector3(41.5f, 1.35f, 0f), new Vector3(0.8f, 2.7f, 4.5f), scissorsMat, MorphoriaAbility.Cut, "Filet ouvert", obstacles);

        CreateCheckpoint(new Vector3(38.4f, 1.1f, 3.2f), root);
        MiniBoss boss = CreateMiniBoss(new Vector3(45f, 1.2f, 0f), root);
        hud.miniBoss = boss;

        CreateVillagerCage("Cage_Rokko", new Vector3(43.2f, 1.25f, 4f), MorphoriaAbility.Break, boss, cages);
        CreateVillagerCage("Cage_Luma", new Vector3(47f, 1.25f, 4f), MorphoriaAbility.Glide, boss, cages);
        CreateVillagerCage("Cage_Papyra", new Vector3(43.2f, 1.25f, -4f), MorphoriaAbility.Fold, boss, cages);
        CreateVillagerCage("Cage_Cizo", new Vector3(47f, 1.25f, -4f), MorphoriaAbility.Cut, boss, cages);

        CreateVerticalSliceEnemies(enemies);
        CreateExitPortal(new Vector3(58.5f, 1.1f, 2f), root);
        CreateSectionLabels(root);
        CreateDecor(root);
        CreateStars(path, collectibles);
        CreateChoiceStars(collectibles);

        Selection.activeGameObject = player.gameObject;
        EditorSceneManager.SaveScene(scene, ScenePath);
        if (updateBuildSettings)
        {
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Morphoria/Build Game Shell Scenes")]
    public static void BuildGameShellScenes()
    {
        EnsureFolders();
        CreateMaterials();
        MorphoriaReferenceImporter.SyncReferences();
        MorphoriaPrefabBuilder.BuildCharacterPrefabs();

        BuildMainMenuScene();
        BuildHubScene();
        BuildWorldMapScene();
        BuildVerticalSliceScene(false);

        for (int i = 1; i < MorphoriaGameContent.Levels.Length; i++)
        {
            BuildAdventureLevelScene(MorphoriaGameContent.Levels[i]);
        }

        ApplyBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void BuildMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = MorphoriaGameContent.MainMenuScene;
        ConfigureRenderSettings(new Color(0.46f, 0.67f, 0.86f), new Color(0.38f, 0.62f, 0.82f), 0.011f);
        CreateLighting();
        CreateFeedbackSystem();
        CreateShellCamera("MainMenuCamera", new Vector3(0f, 5.2f, -12f), new Vector3(0f, 1.4f, 0f), new Color(0.09f, 0.13f, 0.19f), 48f);

        Transform root = new GameObject("Morphoria_MainMenu_Set").transform;
        CreateIsland("Menu_Island_Ecloria", new Vector3(0f, -0.4f, 0f), new Vector3(11f, 1f, 7f), neutralMat, root);
        CreateCube("Menu_Prism_Core", new Vector3(0f, 1.2f, 0.3f), new Vector3(1.1f, 2.1f, 1.1f), prismMat, root).transform.rotation = Quaternion.Euler(0f, 26f, 45f);
        CreateCylinder("Menu_Portal_Ring", new Vector3(0f, 1.8f, 0.2f), new Vector3(2.6f, 0.08f, 2.6f), crystalMat, root, Quaternion.Euler(90f, 0f, 0f));
        CreateTitleText("Morphoria", new Vector3(0f, 3.5f, 0.4f), 0.78f, crystalMat.color, root);

        CreateFormStatue("Menu_Rokko", new Vector3(-4.1f, 0.45f, -0.2f), stoneMat, root);
        CreateFormStatue("Menu_Luma", new Vector3(-1.35f, 0.45f, 1.9f), leafMat, root);
        CreateFormStatue("Menu_Papyra", new Vector3(1.35f, 0.45f, 1.9f), paperMat, root);
        CreateFormStatue("Menu_Cizo", new Vector3(4.1f, 0.45f, -0.2f), scissorsMat, root);

        GameObject controller = new GameObject("MainMenu_Controller");
        controller.AddComponent<MorphoriaMenuScreen>();

        EditorSceneManager.SaveScene(scene, MainMenuPath);
    }

    private static void BuildHubScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = MorphoriaGameContent.HubScene;
        ConfigureRenderSettings(new Color(0.52f, 0.72f, 0.9f), new Color(0.58f, 0.74f, 0.9f), 0.009f);
        CreateLighting();
        CreateFeedbackSystem();

        Transform root = new GameObject("Village_Ecloria_Hub").transform;
        Transform village = new GameObject("Village_Plaza").transform;
        village.SetParent(root);

        MorphoriaPlayer player = CreatePlayer();
        player.transform.position = new Vector3(0f, 2.1f, -1.5f);
        Camera camera = CreateCamera(player.transform);
        player.mainCamera = camera;
        MorphoriaHud hud = CreateHud(player);
        hud.objective = "Choisissez une destination";
        hud.showLevelGoals = false;
        CreatePauseMenu();
        new GameObject("Hub_State").AddComponent<MorphoriaHubState>();

        CreateIsland("Hub_Place_Centrale", new Vector3(0f, 0f, 0f), new Vector3(15f, 1f, 12f), neutralMat, village);
        CreateIsland("Hub_Atelier_Assets", new Vector3(-10f, 0f, 3.2f), new Vector3(6f, 1f, 5f), paperMat, village);
        CreateIsland("Hub_Jardin_Luma", new Vector3(10f, 0f, 3.2f), new Vector3(6f, 1f, 5f), leafMat, village);
        CreateBridge("Hub_Pont_Atelier", new Vector3(-5f, 0.1f, 1.8f), new Vector3(5f, 0.28f, 1.7f), paperMat, village);
        CreateBridge("Hub_Pont_Jardin", new Vector3(5f, 0.1f, 1.8f), new Vector3(5f, 0.28f, 1.7f), leafMat, village);

        CreateCube("Hub_Coeur_Prismatique", new Vector3(0f, 1.55f, 2.4f), new Vector3(1.25f, 2.2f, 1.25f), prismMat, village).transform.rotation = Quaternion.Euler(0f, 38f, 45f);
        CreateVillageHouse("Maison_Rokko", new Vector3(-5.1f, 0.75f, -3.4f), stoneMat, village);
        CreateVillageHouse("Maison_Luma", new Vector3(-1.8f, 0.75f, -4.6f), leafMat, village);
        CreateVillageHouse("Maison_Papyra", new Vector3(1.8f, 0.75f, -4.6f), paperMat, village);
        CreateVillageHouse("Maison_Cizo", new Vector3(5.1f, 0.75f, -3.4f), scissorsMat, village);

        CreateScenePortal("Portail_CarteDuMonde", new Vector3(0f, 1.1f, 5.2f), "Carte du monde", MorphoriaGameContent.WorldMapScene, string.Empty, root);
        CreateScenePortal("Portail_PontQuatreFormes", new Vector3(-6.4f, 1.1f, 1.1f), "Pont des Quatre Formes", string.Empty, MorphoriaGameContent.Levels[0].id, root);
        CreateScenePortal("Portail_DernierNiveauDebloque", new Vector3(6.4f, 1.1f, 1.1f), "Carte des niveaux", MorphoriaGameContent.WorldMapScene, string.Empty, root);

        CreateLabel("VILLAGE", new Vector3(0f, 1.1f, -5.2f), goldMat.color, root);
        CreateDecor(root);

        Selection.activeGameObject = player.gameObject;
        EditorSceneManager.SaveScene(scene, HubPath);
    }

    private static void BuildWorldMapScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = MorphoriaGameContent.WorldMapScene;
        ConfigureRenderSettings(new Color(0.38f, 0.54f, 0.76f), new Color(0.34f, 0.5f, 0.72f), 0.006f);
        CreateLighting();
        CreateFeedbackSystem();
        CreateShellCamera("WorldMapCamera", new Vector3(0f, 15f, -14f), new Vector3(0f, 0f, 0f), new Color(0.08f, 0.12f, 0.18f), 42f);

        Transform root = new GameObject("WorldMap_Table").transform;
        CreateIsland("Carte_Table_Cristal", new Vector3(0f, -0.4f, 0f), new Vector3(20f, 0.6f, 10f), darkRockMat, root);

        Vector3[] nodes =
        {
            new Vector3(-8f, 0.35f, 0f),
            new Vector3(-4.8f, 0.35f, 2.4f),
            new Vector3(-1.6f, 0.35f, -1.8f),
            new Vector3(1.8f, 0.35f, 1.9f),
            new Vector3(5.1f, 0.35f, -1.4f),
            new Vector3(8f, 0.35f, 0.9f)
        };

        for (int i = 0; i < nodes.Length; i++)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.Levels[i];
            MorphoriaWorldInfo world = MorphoriaGameContent.GetWorld(level.worldId);
            Material material = GetPrimaryMaterial(level);
            CreateCylinder("Map_Node_" + level.sceneName, nodes[i], new Vector3(0.72f, 0.16f, 0.72f), material, root, Quaternion.identity);
            CreateLabel((i + 1).ToString(), nodes[i] + new Vector3(0f, 0.26f, -0.38f), world.color, root);

            if (i > 0)
            {
                Vector3 midpoint = (nodes[i - 1] + nodes[i]) * 0.5f;
                float length = Vector3.Distance(nodes[i - 1], nodes[i]);
                GameObject route = CreateCube("Map_Route_" + i, midpoint, new Vector3(0.18f, 0.08f, length), crystalMat, root);
                route.transform.rotation = Quaternion.LookRotation(nodes[i] - nodes[i - 1], Vector3.up);
            }
        }

        GameObject controller = new GameObject("WorldMap_Controller");
        controller.AddComponent<MorphoriaWorldMapScreen>();
        EditorSceneManager.SaveScene(scene, WorldMapPath);
    }

    private static void BuildAdventureLevelScene(MorphoriaLevelInfo level)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = level.sceneName;
        Color fog = Color.Lerp(GetPrimaryMaterial(level).color, new Color(0.55f, 0.72f, 0.92f), 0.58f);
        ConfigureRenderSettings(fog, fog, 0.0085f);
        CreateLighting();
        CreateFeedbackSystem();

        Transform root = new GameObject(level.sceneName + "_Adventure").transform;
        Transform environment = new GameObject("Floating_Islands").transform;
        Transform obstacles = new GameObject("Form_Obstacles").transform;
        Transform enemies = new GameObject("Standard_Enemies").transform;
        Transform collectibles = new GameObject("Collectibles").transform;
        Transform cages = new GameObject("Villager_Cages").transform;
        environment.SetParent(root);
        obstacles.SetParent(root);
        enemies.SetParent(root);
        collectibles.SetParent(root);
        cages.SetParent(root);

        MorphoriaPlayer player = CreatePlayer();
        player.transform.position = new Vector3(-24f, 2.1f, 0f);
        Camera camera = CreateCamera(player.transform);
        player.mainCamera = camera;
        MorphoriaHud hud = CreateHud(player);
        hud.objective = level.targetVillagers > 0 ? "Liberez les villageois" : "Atteignez le portail";
        ConfigureHudForLevel(hud, level);
        CreatePauseMenu();
        CreateLevelResultScreen();

        Material primary = GetPrimaryMaterial(level);
        Material secondary = GetSecondaryMaterial(level);
        Vector3[] path =
        {
            new Vector3(-24f, 1.1f, 0f),
            new Vector3(-13f, 1.1f, 1.8f),
            new Vector3(-2f, 2.3f, -1.6f),
            new Vector3(10f, 1.1f, 0f),
            new Vector3(22f, 1.1f, 1.4f),
            new Vector3(34f, 1.1f, 0f)
        };

        for (int i = 0; i < path.Length; i++)
        {
            Material material = i % 2 == 0 ? primary : secondary;
            CreateIsland("Ile_" + level.sceneName + "_" + i, path[i] - Vector3.up * 1.1f, new Vector3(i == path.Length - 1 ? 10f : 8f, 1f, 7f), material, environment);
            if (i > 0)
            {
                Vector3 midpoint = (path[i - 1] + path[i]) * 0.5f - Vector3.up;
                float length = Vector3.Distance(path[i - 1], path[i]) - 2f;
                GameObject bridge = CreateBridge("Pont_" + level.sceneName + "_" + i, midpoint, new Vector3(length, 0.28f, 1.8f), material, environment);
                Vector3 direction = path[i] - path[i - 1];
                direction.y = 0f;
                bridge.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0f, 90f, 0f);
            }
        }

        CreateAdventureMechanics(level, obstacles, environment);
        CreateAdventureEnemies(level, path, enemies);
        CreateCheckpoint(path[3], root);
        CreateStars(path, collectibles, level.targetGoldenStars);
        CreateChoiceStarsForPath(level.sceneName, path, collectibles, level.targetPrismStars);

        if (level.targetVillagers > 0)
        {
            MiniBoss boss = CreateMiniBoss(new Vector3(22f, 1.2f, 1.4f), root);
            boss.maxHealth = level.worldId == "fortress" ? 6 : 3;
            hud.miniBoss = boss;
            CreateAdventureCages(level, boss, cages);
        }

        LevelExit exit = CreateExitPortal(path[path.Length - 1], root);
        exit.requiredVillagers = level.targetVillagers;
        CreateTitleText(level.displayName, new Vector3(-24f, 2.8f, -3.1f), 0.28f, primary.color, root);
        CreateDecor(root);

        Selection.activeGameObject = player.gameObject;
        EditorSceneManager.SaveScene(scene, ScenePathFor(level));
    }

    private static void ConfigureRenderSettings(Color ambient, Color fogColor, float fogDensity)
    {
        RenderSettings.ambientLight = ambient;
        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    private static void ApplyBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene(MainMenuPath, true),
            new EditorBuildSettingsScene(HubPath, true),
            new EditorBuildSettingsScene(WorldMapPath, true),
            new EditorBuildSettingsScene(ScenePath, true)
        };

        for (int i = 1; i < MorphoriaGameContent.Levels.Length; i++)
        {
            scenes.Add(new EditorBuildSettingsScene(ScenePathFor(MorphoriaGameContent.Levels[i]), true));
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static string ScenePathFor(MorphoriaLevelInfo level)
    {
        return "Assets/Morphoria/Scenes/" + level.sceneName + ".unity";
    }

    private static Material GetPrimaryMaterial(MorphoriaLevelInfo level)
    {
        switch (level.worldId)
        {
            case "canyon":
                return stoneMat;
            case "gardens":
                return leafMat;
            case "archives":
                return paperMat;
            case "forge":
                return scissorsMat;
            case "fortress":
                return darkRockMat;
            default:
                return neutralMat;
        }
    }

    private static Material GetSecondaryMaterial(MorphoriaLevelInfo level)
    {
        switch (level.worldId)
        {
            case "canyon":
                return neutralMat;
            case "gardens":
                return crystalMat;
            case "archives":
                return prismMat;
            case "forge":
                return dangerMat;
            case "fortress":
                return prismMat;
            default:
                return crystalMat;
        }
    }

    private static MorphoriaAbility MainAbilityFor(MorphoriaLevelInfo level)
    {
        switch (level.worldId)
        {
            case "canyon":
                return MorphoriaAbility.Break;
            case "gardens":
                return MorphoriaAbility.Glide;
            case "archives":
                return MorphoriaAbility.Fold;
            case "forge":
                return MorphoriaAbility.Cut;
            case "fortress":
                return MorphoriaAbility.Break;
            default:
                return MorphoriaAbility.Any;
        }
    }

    private static void CreateAdventureMechanics(MorphoriaLevelInfo level, Transform obstacles, Transform environment)
    {
        if (level.worldId == "canyon")
        {
            CreateAbilityObstacle("Canyon_Mur_Fracture", new Vector3(-14.7f, 1.45f, 1.8f), new Vector3(1f, 2.8f, 4.7f), stoneMat, MorphoriaAbility.Break, "Passage ouvert", obstacles);
            GameObject gate = CreateGate("Canyon_Porte_Lourde", new Vector3(8.3f, 1.35f, 0f), new Vector3(0.8f, 2.7f, 4.2f), stoneMat, obstacles);
            CreateHeavyPlate(new Vector3(3.6f, 1.85f, -2.7f), gate, obstacles);
        }
        else if (level.worldId == "gardens")
        {
            CreateWindZone(new Vector3(-2f, 2.7f, -1.6f), new Vector3(5f, 4.8f, 4.4f), obstacles);
            CreateBouncePad(new Vector3(7.6f, 0.75f, -2.2f), obstacles);
            GameObject fragile = CreateBridge("Jardins_Pont_Fragile", new Vector3(15.8f, 0.15f, 0.7f), new Vector3(5.2f, 0.25f, 1.7f), leafMat, environment);
            fragile.AddComponent<FragilePlatform>();
            BoxCollider trigger = fragile.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1f, 2.2f, 1f);
        }
        else if (level.worldId == "archives")
        {
            GameObject bridge = CreateBridge("Archives_Pont_Origami_Replie", new Vector3(4.2f, 0.15f, 1.5f), new Vector3(6f, 0.25f, 1.5f), paperMat, environment);
            bridge.SetActive(false);
            AbilityGate rune = CreateAbilityObstacle("Archives_Rune_Papyra", new Vector3(-2f, 1.88f, -1.6f), new Vector3(1.25f, 0.18f, 1.25f), prismMat, MorphoriaAbility.Fold, "Pont deploye", obstacles);
            rune.destroyOnSuccess = false;
            rune.activateOnSuccess = new[] { bridge };
            CreateAbilityObstacle("Archives_Fente_Papier", new Vector3(12f, 1.3f, 0f), new Vector3(0.7f, 2.6f, 4.3f), paperMat, MorphoriaAbility.Fold, "Fente traversee", obstacles);
        }
        else if (level.worldId == "forge")
        {
            GameObject gate = CreateGate("Forge_Grille_Mecanique", new Vector3(9.2f, 1.35f, 0f), new Vector3(0.8f, 2.7f, 4.2f), dangerMat, obstacles);
            AbilityGate cable = CreateAbilityObstacle("Forge_Cable_Cizo", new Vector3(4.6f, 1.9f, -2.8f), new Vector3(0.28f, 4.8f, 0.28f), scissorsMat, MorphoriaAbility.Cut, "Grille ouverte", obstacles, true);
            cable.deactivateOnSuccess = new[] { gate };
            CreateAbilityObstacle("Forge_Filet_Lames", new Vector3(19.2f, 1.35f, 1.4f), new Vector3(0.8f, 2.7f, 4.5f), scissorsMat, MorphoriaAbility.Cut, "Filet coupe", obstacles);
        }
        else if (level.worldId == "fortress")
        {
            CreateAbilityObstacle("Forteresse_Mur_Rokko", new Vector3(-14.7f, 1.45f, 1.8f), new Vector3(1f, 2.8f, 4.7f), stoneMat, MorphoriaAbility.Break, "Mur brise", obstacles);
            CreateWindZone(new Vector3(-2f, 2.7f, -1.6f), new Vector3(5f, 4.8f, 4.4f), obstacles);
            CreateAbilityObstacle("Forteresse_Rune_Papyra", new Vector3(10f, 0.68f, 2.7f), new Vector3(1.25f, 0.18f, 1.25f), prismMat, MorphoriaAbility.Fold, "Rune activee", obstacles);
            CreateAbilityObstacle("Forteresse_Chaines_Cizo", new Vector3(18.6f, 1.55f, 1.4f), new Vector3(0.36f, 3.4f, 0.36f), scissorsMat, MorphoriaAbility.Cut, "Chaines coupees", obstacles, true);
        }
        else
        {
            CreateAbilityObstacle("Obstacle_Equipe", new Vector3(-13f, 1.35f, 1.8f), new Vector3(0.8f, 2.7f, 4.2f), neutralMat, MorphoriaAbility.Any, "Passage ouvert", obstacles);
        }
    }

    private static void CreateVerticalSliceEnemies(Transform parent)
    {
        CreateEnemy("Picboule_Pierre", "Picboule", new Vector3(-21.6f, 1.35f, -2.2f), MorphoriaAbility.Break, stoneMat, parent, Vector3.forward);
        CreateEnemy("Flottevent_Feuille", "Flottevent", new Vector3(-8.6f, 3.65f, 5.2f), MorphoriaAbility.Glide, leafMat, parent, Vector3.right, 1.9f, 1.35f, 0.28f);
        CreateEnemy("Tache_Encre_Papier", "Tache-Encre", new Vector3(5.7f, 1.25f, 2.45f), MorphoriaAbility.Fold, paperMat, parent, Vector3.forward);
        CreateEnemy("Roncivore_Ciseaux", "Roncivore", new Vector3(18.5f, 1.25f, -5.05f), MorphoriaAbility.Cut, scissorsMat, parent, Vector3.right);
    }

    private static void CreateAdventureEnemies(MorphoriaLevelInfo level, Vector3[] path, Transform parent)
    {
        if (level.worldId == "canyon")
        {
            CreateEnemy("Canyon_Picboule", "Picboule", path[1] + new Vector3(0f, 0.35f, -2.2f), MorphoriaAbility.Break, stoneMat, parent, Vector3.forward);
            CreateEnemy("Canyon_Roule_Roc", "Roule-Roc", path[3] + new Vector3(0f, 0.35f, 2.2f), MorphoriaAbility.Fold, paperMat, parent, Vector3.right);
        }
        else if (level.worldId == "gardens")
        {
            CreateEnemy("Jardins_Flottevent", "Flottevent", path[1] + new Vector3(0f, 1.35f, 2.25f), MorphoriaAbility.Glide, leafMat, parent, Vector3.right, 1.9f, 1.3f, 0.3f);
            CreateEnemy("Jardins_Roncivore", "Roncivore", path[3] + new Vector3(0f, 0.35f, -2.25f), MorphoriaAbility.Cut, scissorsMat, parent, Vector3.forward);
        }
        else if (level.worldId == "archives")
        {
            CreateEnemy("Archives_Tache_Encre", "Tache-Encre", path[1] + new Vector3(0f, 0.35f, -2.2f), MorphoriaAbility.Fold, paperMat, parent, Vector3.forward);
            CreateEnemy("Archives_Papier_Masque", "Papier-Masque", path[3] + new Vector3(0f, 0.35f, 2.2f), MorphoriaAbility.Cut, scissorsMat, parent, Vector3.right);
        }
        else if (level.worldId == "forge")
        {
            CreateEnemy("Forge_Scie_Folle", "Scie-Folle", path[1] + new Vector3(0f, 0.35f, 2.2f), MorphoriaAbility.Break, stoneMat, parent, Vector3.right);
            CreateEnemy("Forge_Aimant_Lame", "Aimant-Lame", path[3] + new Vector3(0f, 0.35f, -2.2f), MorphoriaAbility.Fold, paperMat, parent, Vector3.forward);
        }
        else if (level.worldId == "fortress")
        {
            CreateEnemy("Forteresse_Picboule", "Picboule", path[1] + new Vector3(0f, 0.35f, -2.3f), MorphoriaAbility.Break, stoneMat, parent, Vector3.forward);
            CreateEnemy("Forteresse_Flottevent", "Flottevent", path[2] + new Vector3(0f, 1.35f, 2.4f), MorphoriaAbility.Glide, leafMat, parent, Vector3.right, 2.0f, 1.3f, 0.32f);
            CreateEnemy("Forteresse_Tache_Encre", "Tache-Encre", path[3] + new Vector3(0f, 0.35f, -2.35f), MorphoriaAbility.Fold, paperMat, parent, Vector3.forward);
            CreateEnemy("Forteresse_Roncivore", "Roncivore", path[4] + new Vector3(0f, 0.35f, 2.35f), MorphoriaAbility.Cut, scissorsMat, parent, Vector3.right);
        }
        else
        {
            CreateEnemy("Ecloria_Picboule", "Picboule", path[1] + new Vector3(0f, 0.35f, -2.2f), MorphoriaAbility.Break, stoneMat, parent, Vector3.forward);
            CreateEnemy("Ecloria_Roncivore", "Roncivore", path[3] + new Vector3(0f, 0.35f, 2.2f), MorphoriaAbility.Cut, scissorsMat, parent, Vector3.right);
        }
    }

    private static MorphoriaEnemy CreateEnemy(string objectName, string displayName, Vector3 position, MorphoriaAbility weakness, Material themeMaterial, Transform parent, Vector3 patrolAxis, float speed = 1.55f, float patrolDistance = 1.7f, float hover = 0.08f)
    {
        GameObject root = new GameObject(objectName);
        root.transform.SetParent(parent, false);
        root.transform.position = position;

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = weakness == MorphoriaAbility.Glide ? 1.25f : 1.05f;
        trigger.center = new Vector3(0f, 0.75f, 0f);

        MorphoriaEnemy enemy = root.AddComponent<MorphoriaEnemy>();
        enemy.displayName = displayName;
        enemy.weakness = weakness;
        enemy.moveSpeed = speed;
        enemy.patrolDistance = patrolDistance;
        enemy.hoverAmplitude = hover;
        enemy.patrolAxis = patrolAxis;

        BuildEnemyVisual(displayName, weakness, themeMaterial, root.transform);
        enemy.renderers = root.GetComponentsInChildren<Renderer>();
        return enemy;
    }

    private static void BuildEnemyVisual(string displayName, MorphoriaAbility weakness, Material themeMaterial, Transform parent)
    {
        Material core = weakness == MorphoriaAbility.Break ? darkRockMat : themeMaterial;
        Material accent = WeaknessMaterial(weakness);

        if (displayName == "Flottevent")
        {
            CreateEnemyPart("body", PrimitiveType.Sphere, new Vector3(0f, 0.9f, 0f), new Vector3(0.62f, 0.62f, 0.62f), core, parent);
            CreateEnemyPart("left_wing", PrimitiveType.Cube, new Vector3(-0.72f, 0.9f, 0f), new Vector3(0.16f, 0.72f, 0.9f), accent, parent).transform.localRotation = Quaternion.Euler(0f, -18f, 0f);
            CreateEnemyPart("right_wing", PrimitiveType.Cube, new Vector3(0.72f, 0.9f, 0f), new Vector3(0.16f, 0.72f, 0.9f), accent, parent).transform.localRotation = Quaternion.Euler(0f, 18f, 0f);
            CreateEnemyPart("wind_eye", PrimitiveType.Sphere, new Vector3(0f, 1f, -0.42f), new Vector3(0.16f, 0.16f, 0.08f), prismMat, parent);
        }
        else if (displayName == "Roncivore")
        {
            CreateEnemyPart("stem", PrimitiveType.Capsule, new Vector3(0f, 0.7f, 0f), new Vector3(0.42f, 0.78f, 0.42f), leafMat, parent);
            CreateEnemyPart("jaw_left", PrimitiveType.Cube, new Vector3(-0.32f, 1.2f, -0.1f), new Vector3(0.48f, 0.18f, 0.55f), accent, parent).transform.localRotation = Quaternion.Euler(0f, 0f, 22f);
            CreateEnemyPart("jaw_right", PrimitiveType.Cube, new Vector3(0.32f, 1.2f, -0.1f), new Vector3(0.48f, 0.18f, 0.55f), accent, parent).transform.localRotation = Quaternion.Euler(0f, 0f, -22f);
            CreateEnemyPart("thorn", PrimitiveType.Cube, new Vector3(0f, 1.5f, -0.1f), new Vector3(0.18f, 0.42f, 0.18f), dangerMat, parent).transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        }
        else if (displayName == "Tache-Encre" || displayName == "Aimant-Lame")
        {
            CreateEnemyPart("ink_core", PrimitiveType.Sphere, new Vector3(0f, 0.62f, 0f), new Vector3(0.74f, 0.48f, 0.74f), darkRockMat, parent);
            CreateEnemyPart("rune_bar", PrimitiveType.Cube, new Vector3(0f, 0.95f, -0.5f), new Vector3(0.58f, 0.08f, 0.06f), accent, parent);
            CreateEnemyPart("rune_dot", PrimitiveType.Sphere, new Vector3(0f, 1.13f, -0.5f), new Vector3(0.16f, 0.16f, 0.07f), accent, parent);
        }
        else if (displayName == "Papier-Masque")
        {
            CreateEnemyPart("mask_body", PrimitiveType.Cube, new Vector3(0f, 0.85f, 0f), new Vector3(0.82f, 0.92f, 0.18f), paperMat, parent);
            CreateEnemyPart("mask_blade_left", PrimitiveType.Cube, new Vector3(-0.5f, 0.9f, -0.03f), new Vector3(0.18f, 0.72f, 0.16f), accent, parent).transform.localRotation = Quaternion.Euler(0f, 0f, -28f);
            CreateEnemyPart("mask_blade_right", PrimitiveType.Cube, new Vector3(0.5f, 0.9f, -0.03f), new Vector3(0.18f, 0.72f, 0.16f), accent, parent).transform.localRotation = Quaternion.Euler(0f, 0f, 28f);
        }
        else
        {
            CreateEnemyPart("body", PrimitiveType.Sphere, new Vector3(0f, 0.78f, 0f), new Vector3(0.72f, 0.72f, 0.72f), core, parent);
            CreateEnemyPart("weakness_core", PrimitiveType.Cube, new Vector3(0f, 1.18f, -0.5f), new Vector3(0.28f, 0.28f, 0.08f), accent, parent).transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            CreateEnemyPart("spike_left", PrimitiveType.Cube, new Vector3(-0.56f, 0.82f, 0f), new Vector3(0.24f, 0.24f, 0.24f), dangerMat, parent).transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            CreateEnemyPart("spike_right", PrimitiveType.Cube, new Vector3(0.56f, 0.82f, 0f), new Vector3(0.24f, 0.24f, 0.24f), dangerMat, parent).transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        }

        Light light = parent.gameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = WeaknessColor(weakness);
        light.range = 3.8f;
        light.intensity = 0.9f;
    }

    private static GameObject CreateEnemyPart(string name, PrimitiveType primitive, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        Renderer renderer = part.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        DestroyColliderImmediate(part);
        return part;
    }

    private static Material WeaknessMaterial(MorphoriaAbility weakness)
    {
        switch (weakness)
        {
            case MorphoriaAbility.Break:
            case MorphoriaAbility.PushHeavy:
            case MorphoriaAbility.ResistWind:
                return stoneMat;
            case MorphoriaAbility.Glide:
                return leafMat;
            case MorphoriaAbility.Fold:
                return paperMat;
            case MorphoriaAbility.Cut:
                return scissorsMat;
            default:
                return prismMat;
        }
    }

    private static Color WeaknessColor(MorphoriaAbility weakness)
    {
        return WeaknessMaterial(weakness).color;
    }

    private static void CreateAdventureCages(MorphoriaLevelInfo level, MiniBoss boss, Transform parent)
    {
        MorphoriaAbility primary = MainAbilityFor(level);
        CreateVillagerCage(level.sceneName + "_Cage_A", new Vector3(19.8f, 1.25f, 4.2f), primary, boss, parent);
        CreateVillagerCage(level.sceneName + "_Cage_B", new Vector3(24.3f, 1.25f, 4.2f), MorphoriaAbility.Cut, boss, parent);

        if (level.targetVillagers > 2)
        {
            CreateVillagerCage(level.sceneName + "_Cage_C", new Vector3(19.8f, 1.25f, -3.7f), MorphoriaAbility.Fold, boss, parent);
            CreateVillagerCage(level.sceneName + "_Cage_D", new Vector3(24.3f, 1.25f, -3.7f), MorphoriaAbility.Break, boss, parent);
        }
    }

    private static void CreateChoiceStarsForPath(string prefix, Vector3[] path, Transform parent, int targetCount)
    {
        for (int i = 0; i < targetCount; i++)
        {
            Vector3 position = SamplePath(path, (i + 1f) / (targetCount + 1f));
            position += new Vector3(0f, 0.75f, i % 2 == 0 ? -2.1f : 2.1f);
            CreateStar("ChoiceStar_" + prefix + "_" + (i + 1).ToString("00"), position, CollectibleKind.ChoiceStar, prismMat, parent);
        }
    }

    private static Vector3 SamplePath(Vector3[] path, float t)
    {
        if (path == null || path.Length == 0)
        {
            return Vector3.zero;
        }

        if (path.Length == 1)
        {
            return path[0];
        }

        float scaled = Mathf.Clamp01(t) * (path.Length - 1);
        int index = Mathf.Min(path.Length - 2, Mathf.FloorToInt(scaled));
        float localT = scaled - index;
        return Vector3.Lerp(path[index], path[index + 1], localT);
    }

    private static Camera CreateShellCamera(string name, Vector3 position, Vector3 lookAt, Color background, float fieldOfView)
    {
        GameObject cameraObject = new GameObject(name);
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = background;
        camera.fieldOfView = fieldOfView;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = position;
        cameraObject.transform.LookAt(lookAt);
        return camera;
    }

    private static void CreatePauseMenu()
    {
        new GameObject("PauseMenu").AddComponent<MorphoriaPauseMenu>();
    }

    private static void CreateFeedbackSystem()
    {
        GameObject feedback = new GameObject("Morphoria_FeedbackSystem");
        feedback.AddComponent<AudioSource>();
        feedback.AddComponent<MorphoriaFeedbackSystem>();
    }

    private static void CreateLevelResultScreen()
    {
        new GameObject("Level_Result_Screen").AddComponent<MorphoriaLevelResultScreen>();
    }

    private static void CreateScenePortal(string name, Vector3 position, string label, string targetScene, string targetLevelId, Transform parent)
    {
        GameObject trigger = new GameObject(name);
        trigger.transform.SetParent(parent, false);
        trigger.transform.position = position;
        SphereCollider collider = trigger.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 1.35f;

        MorphoriaScenePortal portal = trigger.AddComponent<MorphoriaScenePortal>();
        portal.label = label;
        portal.targetScene = targetScene;
        portal.targetLevelId = targetLevelId;

        CreateCylinder(name + "_Ring", position + Vector3.up * 1.15f, new Vector3(1.2f, 0.06f, 1.2f), crystalMat, parent, Quaternion.Euler(90f, 0f, 0f));
        Light light = trigger.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.22f, 0.72f, 1f);
        light.range = 6f;
        light.intensity = 1.8f;
    }

    private static void CreateVillageHouse(string name, Vector3 position, Material material, Transform parent)
    {
        CreateCube(name + "_Base", position, new Vector3(1.5f, 1.5f, 1.5f), material, parent);
        GameObject roof = CreateCube(name + "_Toit", position + Vector3.up * 0.95f, new Vector3(1.85f, 0.5f, 1.85f), darkRockMat, parent);
        roof.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
    }

    private static void CreateFormStatue(string name, Vector3 position, Material material, Transform parent)
    {
        CreateCylinder(name + "_Base", position, new Vector3(0.72f, 0.22f, 0.72f), darkRockMat, parent, Quaternion.identity);
        GameObject body = CreateCube(name + "_Body", position + Vector3.up * 0.8f, new Vector3(0.72f, 1f, 0.72f), material, parent);
        body.transform.rotation = Quaternion.Euler(0f, 25f, 0f);
        CreateCube(name + "_Halo", position + Vector3.up * 1.55f, new Vector3(0.92f, 0.08f, 0.92f), crystalMat, parent).transform.rotation = Quaternion.Euler(0f, 45f, 45f);
    }

    private static void CreateTitleText(string text, Vector3 position, float size, Color color, Transform parent)
    {
        GameObject title = new GameObject("Title_" + text.Replace(" ", "_"));
        title.transform.SetParent(parent, false);
        title.transform.position = position;
        title.transform.rotation = Quaternion.Euler(18f, 0f, 0f);
        TextMesh mesh = title.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.characterSize = size;
        mesh.fontSize = 72;
        mesh.color = Color.Lerp(color, Color.white, 0.25f);
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory("Assets/Morphoria/Art/References");
        Directory.CreateDirectory("Assets/Morphoria/Data");
        Directory.CreateDirectory("Assets/Morphoria/Editor");
        Directory.CreateDirectory("Assets/Morphoria/Materials");
        Directory.CreateDirectory("Assets/Morphoria/Prefabs");
        Directory.CreateDirectory("Assets/Morphoria/Scenes");
        Directory.CreateDirectory("Assets/Morphoria/Scripts");
        Directory.CreateDirectory("Assets/Morphoria/Textures");
    }

    private static void CreateMaterials()
    {
        stoneMat = CreateMaterial("M_Stone_Rokko_Ocher", new Color(0.56f, 0.38f, 0.23f), new Color(1f, 0.52f, 0.1f));
        leafMat = CreateMaterial("M_Leaf_Luma_Green", new Color(0.18f, 0.62f, 0.24f), new Color(0.86f, 1f, 0.28f));
        paperMat = CreateMaterial("M_Paper_Papyra_Ivory", new Color(0.82f, 0.76f, 0.96f), new Color(0.55f, 0.34f, 0.9f));
        scissorsMat = CreateMaterial("M_Scissors_Cizo_Steel", new Color(0.54f, 0.72f, 0.88f), new Color(0.08f, 0.65f, 1f));
        neutralMat = CreateMaterial("M_Ecloria_Warm_Grass", new Color(0.34f, 0.58f, 0.34f), new Color(0.94f, 0.78f, 0.3f));
        darkRockMat = CreateMaterial("M_Noctar_Arena_Dark", new Color(0.13f, 0.11f, 0.18f), new Color(0.62f, 0.22f, 1f));
        goldMat = CreateMaterial("M_Golden_Star_Crystal", new Color(1f, 0.78f, 0.13f), new Color(1f, 0.88f, 0.28f));
        prismMat = CreateMaterial("M_Prism_Star_Violet", new Color(0.58f, 0.28f, 1f), new Color(0.85f, 0.55f, 1f));
        crystalMat = CreateMaterial("M_Crystal_Cage_Blue", new Color(0.25f, 0.78f, 1f, 0.62f), new Color(0.1f, 0.7f, 1f), true);
        windMat = CreateMaterial("M_Wind_Cyan_Transparent", new Color(0.3f, 0.88f, 1f, 0.26f), new Color(0.35f, 0.95f, 1f), true);
        dangerMat = CreateMaterial("M_Danger_Orange_Red", new Color(0.86f, 0.25f, 0.12f), new Color(1f, 0.45f, 0.08f));
    }

    private static Material CreateMaterial(string name, Color color, Color emission, bool transparent = false)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Diffuse");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", transparent ? 3f : 0f);
        }

        if (transparent)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
        else
        {
            material.SetOverrideTag("RenderType", string.Empty);
            material.SetInt("_SrcBlend", (int)BlendMode.One);
            material.SetInt("_DstBlend", (int)BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }

        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", emission * 0.45f);
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    private static void CreateLighting()
    {
        GameObject sun = new GameObject("Sun_Key_Light");
        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.color = new Color(1f, 0.88f, 0.68f);
        sunLight.intensity = 1.35f;
        sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

        GameObject fill = new GameObject("Blue_Crystal_Fill");
        Light fillLight = fill.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.color = new Color(0.48f, 0.76f, 1f);
        fillLight.intensity = 0.45f;
        fill.transform.rotation = Quaternion.Euler(20f, 130f, 0f);
    }

    private static MorphoriaPlayer CreatePlayer()
    {
        GameObject player = new GameObject("Player_Rokko_Luma_Papyra_Cizo");
        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        if (ignoreRaycastLayer >= 0)
        {
            player.layer = ignoreRaycastLayer;
        }

        player.transform.position = new Vector3(-36f, 2.1f, 0f);
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2.3f;
        controller.radius = 0.45f;
        controller.center = new Vector3(0f, 1.08f, 0f);
        player.AddComponent<PlayerInventory>();
        MorphoriaAvatar avatar = player.AddComponent<MorphoriaAvatar>();
        AssignCharacterPrefabs(avatar);
        MorphoriaPlayer morphoriaPlayer = player.AddComponent<MorphoriaPlayer>();
        player.AddComponent<MorphoriaProceduralAnimator>();
        return morphoriaPlayer;
    }

    private static void AssignCharacterPrefabs(MorphoriaAvatar avatar)
    {
        avatar.stonePrefab = LoadCharacterPrefab("PF_Rokko");
        avatar.leafPrefab = LoadCharacterPrefab("PF_Luma");
        avatar.paperPrefab = LoadCharacterPrefab("PF_Papyra");
        avatar.scissorsPrefab = LoadCharacterPrefab("PF_Cizo");
        EditorUtility.SetDirty(avatar);
    }

    private static GameObject LoadCharacterPrefab(string name)
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Morphoria/Prefabs/Characters/" + name + ".prefab");
    }

    private static Camera CreateCamera(Transform target)
    {
        GameObject cameraObject = new GameObject("ThirdPersonCamera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.54f, 0.73f, 0.92f);
        camera.fieldOfView = 58f;
        cameraObject.AddComponent<AudioListener>();
        ThirdPersonCamera controller = cameraObject.AddComponent<ThirdPersonCamera>();
        controller.target = target;
        controller.distance = 7.1f;
        controller.minDistance = 2.05f;
        controller.maxDistance = 8.8f;
        controller.defaultPitch = 24f;
        controller.lookAheadDistance = 1.2f;
        controller.collisionRadius = 0.36f;
        controller.collisionPadding = 0.22f;
        controller.recenterDelay = 1.05f;
        cameraObject.transform.position = target.position + new Vector3(0f, 4f, -8f);
        cameraObject.transform.LookAt(target.position + Vector3.up);
        return camera;
    }

    private static MorphoriaHud CreateHud(MorphoriaPlayer player)
    {
        GameObject hud = new GameObject("HUD_Morphoria");
        MorphoriaHud controller = hud.AddComponent<MorphoriaHud>();
        controller.player = player;
        controller.objective = "Liberez les villageois";
        return controller;
    }

    private static void ConfigureHudForLevel(MorphoriaHud hud, MorphoriaLevelInfo level)
    {
        hud.showLevelGoals = true;
        hud.targetGoldenStars = level.targetGoldenStars;
        hud.targetPrismStars = level.targetPrismStars;
        hud.targetVillagers = level.targetVillagers;
    }

    private static GameObject CreateIsland(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject top = CreateCube(name, position, scale, material, parent);
        GameObject underside = CreateCube(name + "_floating_rock", position + new Vector3(0f, -0.78f, 0f), new Vector3(scale.x * 0.72f, 0.75f, scale.z * 0.72f), darkRockMat, parent);
        underside.transform.rotation = Quaternion.Euler(0f, 14f, 0f);
        return top;
    }

    private static GameObject CreateBridge(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject bridge = CreateCube(name, position, scale, material, parent);
        CreateCube(name + "_left_edge", position + new Vector3(0f, 0.23f, scale.z * 0.5f), new Vector3(scale.x, 0.18f, 0.12f), darkRockMat, parent);
        CreateCube(name + "_right_edge", position + new Vector3(0f, 0.23f, -scale.z * 0.5f), new Vector3(scale.x, 0.18f, 0.12f), darkRockMat, parent);
        return bridge;
    }

    private static GameObject CreateGate(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject gate = CreateCube(name, position, scale, material, parent);
        return gate;
    }

    private static AbilityGate CreateAbilityObstacle(string name, Vector3 position, Vector3 scale, Material material, MorphoriaAbility ability, string success, Transform parent, bool cylinder = false)
    {
        GameObject obstacle = cylinder
            ? CreateCylinder(name, position, scale, material, parent, Quaternion.Euler(0f, 0f, 90f))
            : CreateCube(name, position, scale, material, parent);

        AbilityGate gate = obstacle.AddComponent<AbilityGate>();
        gate.requiredAbility = ability;
        gate.successMessage = success;
        return gate;
    }

    private static void CreateWindZone(Vector3 position, Vector3 scale, Transform parent)
    {
        GameObject wind = CreateCube("Courant_Air_Luma", position, scale, windMat, parent);
        Collider collider = wind.GetComponent<Collider>();
        collider.isTrigger = true;
        Morphoria.WindZone zone = wind.AddComponent<Morphoria.WindZone>();
        zone.windVelocity = new Vector3(0f, 18f, 2.5f);
    }

    private static void CreateBouncePad(Vector3 position, Transform parent)
    {
        GameObject pad = CreateCylinder("Fleur_Rebondissante_Luma", position, new Vector3(1.2f, 0.16f, 1.2f), leafMat, parent, Quaternion.identity);
        Collider collider = pad.GetComponent<Collider>();
        collider.isTrigger = true;
        pad.AddComponent<BouncePad>();
        CreateCube("Fleur_Petales_Luma", position + Vector3.up * 0.18f, new Vector3(2.1f, 0.08f, 0.45f), goldMat, parent).transform.rotation = Quaternion.Euler(0f, 45f, 0f);
        CreateCube("Fleur_Petales_Luma_B", position + Vector3.up * 0.2f, new Vector3(0.45f, 0.08f, 2.1f), goldMat, parent).transform.rotation = Quaternion.Euler(0f, 45f, 0f);
    }

    private static void CreateHeavyPlate(Vector3 position, GameObject targetGate, Transform parent)
    {
        GameObject plate = CreateCylinder("Bouton_Pression_Rokko", position, new Vector3(1.4f, 0.08f, 1.4f), stoneMat, parent, Quaternion.identity);
        Collider collider = plate.GetComponent<Collider>();
        collider.isTrigger = true;
        HeavyPressurePlate pressurePlate = plate.AddComponent<HeavyPressurePlate>();
        pressurePlate.deactivateOnPress = new[] { targetGate };
        pressurePlate.message = "Porte ouverte";
    }

    private static void CreateCheckpoint(Vector3 position, Transform parent)
    {
        GameObject checkpoint = CreateCylinder("Checkpoint_Cristal", position, new Vector3(0.9f, 0.7f, 0.9f), crystalMat, parent, Quaternion.identity);
        Collider collider = checkpoint.GetComponent<Collider>();
        collider.isTrigger = true;
        checkpoint.AddComponent<Checkpoint>();
        Light light = checkpoint.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.2f, 0.75f, 1f);
        light.range = 7f;
        light.intensity = 2.2f;
    }

    private static MiniBoss CreateMiniBoss(Vector3 position, Transform parent)
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        boss.name = "MiniBoss_Garde_Cage";
        boss.transform.SetParent(parent, false);
        boss.transform.position = position;
        boss.transform.localScale = new Vector3(1.4f, 1.55f, 1.4f);
        boss.GetComponent<Renderer>().sharedMaterial = darkRockMat;
        Collider collider = boss.GetComponent<Collider>();
        collider.isTrigger = true;

        MiniBoss miniBoss = boss.AddComponent<MiniBoss>();
        miniBoss.maxHealth = 4;
        miniBoss.renderers = boss.GetComponentsInChildren<Renderer>();
        miniBoss.patrolPoints = new[]
        {
            CreateMarker("Boss_Patrol_A", new Vector3(41.5f, 1.05f, -3.5f), parent),
            CreateMarker("Boss_Patrol_B", new Vector3(48.5f, 1.05f, 3.5f), parent)
        };

        CreateCube("Garde_Cage_Cadenas", new Vector3(0f, 1.2f, -0.75f), new Vector3(0.75f, 0.55f, 0.22f), prismMat, boss.transform);
        return miniBoss;
    }

    private static void CreateVillagerCage(string name, Vector3 position, MorphoriaAbility ability, MiniBoss boss, Transform parent)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = position;
        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.5f;

        GameObject cageVisual = CreateCube(name + "_crystal_cage", Vector3.zero, new Vector3(1.45f, 1.8f, 1.45f), crystalMat, root.transform);
        cageVisual.transform.localPosition = Vector3.up * 0.95f;
        CreateCube(name + "_lock", new Vector3(0f, 0f, -0.78f), new Vector3(0.42f, 0.42f, 0.18f), prismMat, cageVisual.transform);

        GameObject villager = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        villager.name = name + "_villager";
        villager.transform.SetParent(root.transform, false);
        villager.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        villager.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
        villager.GetComponent<Renderer>().sharedMaterial = goldMat;
        UnityEngine.Object.DestroyImmediate(villager.GetComponent<Collider>());
        villager.SetActive(false);

        VillagerCage cage = root.AddComponent<VillagerCage>();
        cage.requiredAbility = ability;
        cage.boss = boss;
        cage.cageVisual = cageVisual;
        cage.villagerVisual = villager;
    }

    private static LevelExit CreateExitPortal(Vector3 position, Transform parent)
    {
        GameObject trigger = new GameObject("Portail_Sortie");
        trigger.transform.SetParent(parent, false);
        trigger.transform.position = position;
        BoxCollider collider = trigger.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(3.2f, 4f, 1.2f);
        LevelExit exit = trigger.AddComponent<LevelExit>();

        CreateCylinder("Portail_Sortie_Anneau_A", position + Vector3.up * 1.6f, new Vector3(1.6f, 0.08f, 1.6f), crystalMat, parent, Quaternion.Euler(90f, 0f, 0f));
        CreateCylinder("Portail_Sortie_Anneau_B", position + Vector3.up * 1.6f, new Vector3(2.05f, 0.05f, 2.05f), prismMat, parent, Quaternion.Euler(90f, 0f, 0f));
        Light light = trigger.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.18f, 0.68f, 1f);
        light.range = 9f;
        light.intensity = 3f;
        return exit;
    }

    private static void CreateSectionLabels(Transform parent)
    {
        CreateLabel("DEPART", new Vector3(-36f, 1.1f, -3.6f), neutralMat.color, parent);
        CreateLabel("PIERRE", new Vector3(-24f, 1.1f, -3.6f), stoneMat.color, parent);
        CreateLabel("FEUILLE", new Vector3(-11f, 3.1f, 0.6f), leafMat.color, parent);
        CreateLabel("PAPIER", new Vector3(3f, 1.1f, -3.6f), paperMat.color, parent);
        CreateLabel("CISEAUX", new Vector3(17f, 1.1f, -6.6f), scissorsMat.color, parent);
        CreateLabel("PUZZLE", new Vector3(31f, 1.1f, -4.6f), goldMat.color, parent);
        CreateLabel("CAGES", new Vector3(45f, 1.1f, -5.3f), prismMat.color, parent);
        CreateLabel("SORTIE", new Vector3(58f, 1.1f, -1.7f), crystalMat.color, parent);
    }

    private static void CreateLabel(string text, Vector3 position, Color color, Transform parent)
    {
        GameObject label = new GameObject("Label_" + text);
        label.transform.SetParent(parent, false);
        label.transform.position = position;
        label.transform.rotation = Quaternion.Euler(68f, 0f, 0f);
        TextMesh mesh = label.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.characterSize = 0.34f;
        mesh.fontSize = 54;
        mesh.color = Color.Lerp(color, Color.white, 0.25f);
    }

    private static void CreateDecor(Transform parent)
    {
        for (int i = 0; i < 18; i++)
        {
            float t = i / 17f;
            Vector3 position = Vector3.Lerp(new Vector3(-35f, 0.6f, 4f), new Vector3(58f, 0.6f, 5f), t);
            position.z += Mathf.Sin(i * 1.7f) * 2.2f;
            Material material = i % 3 == 0 ? prismMat : crystalMat;
            GameObject crystal = CreateCube("Decor_Crystal_" + i, position + Vector3.up * UnityEngine.Random.Range(0.05f, 0.5f), new Vector3(0.34f, UnityEngine.Random.Range(0.9f, 1.8f), 0.34f), material, parent);
            crystal.transform.rotation = Quaternion.Euler(0f, i * 29f, 45f);
        }

        CreateCube("Village_Coeur_Prismatique_Rappel", new Vector3(-38.5f, 1.2f, 3.1f), new Vector3(0.8f, 1.25f, 0.8f), prismMat, parent).transform.rotation = Quaternion.Euler(0f, 22f, 45f);
    }

    private static void CreateStars(Vector3[] path, Transform parent, int targetCount = 50)
    {
        int created = 0;
        int segmentCount = Mathf.Max(1, path.Length - 1);
        for (int i = 0; i < segmentCount && created < targetCount; i++)
        {
            int remainingSegments = segmentCount - i;
            int count = Mathf.CeilToInt((targetCount - created) / (float)remainingSegments);
            for (int j = 0; j < count && created < targetCount; j++)
            {
                float t = (j + 1f) / (count + 1f);
                Vector3 position = Vector3.Lerp(path[i], path[i + 1], t);
                position.y += 0.55f + Mathf.Sin((created + 1) * 0.9f) * 0.18f;
                position.z += Mathf.Sin(created * 1.3f) * 0.8f;
                CreateStar("GoldenStar_" + (created + 1).ToString("00"), position, CollectibleKind.GoldenStar, goldMat, parent);
                created++;
            }
        }
    }

    private static void CreateChoiceStars(Transform parent)
    {
        CreateStar("ChoiceStar_Pierre", new Vector3(-20f, 1.7f, -2.7f), CollectibleKind.ChoiceStar, prismMat, parent);
        CreateStar("ChoiceStar_Feuille", new Vector3(-8f, 4.1f, 6.4f), CollectibleKind.ChoiceStar, prismMat, parent);
        CreateStar("ChoiceStar_Papier", new Vector3(7.7f, 1.7f, 2.7f), CollectibleKind.ChoiceStar, prismMat, parent);
        CreateStar("ChoiceStar_Ciseaux", new Vector3(21f, 1.7f, -5.3f), CollectibleKind.ChoiceStar, prismMat, parent);
        CreateStar("ChoiceStar_Arene", new Vector3(45f, 2.2f, 0f), CollectibleKind.ChoiceStar, prismMat, parent);
    }

    private static void CreateStar(string name, Vector3 position, CollectibleKind kind, Material material, Transform parent)
    {
        GameObject star = new GameObject(name);
        star.transform.SetParent(parent, false);
        star.transform.position = position;
        star.transform.rotation = Quaternion.Euler(0f, 25f, 45f);

        SphereCollider collider = star.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 0.55f;

        MorphoriaCollectible collectible = star.AddComponent<MorphoriaCollectible>();
        collectible.kind = kind;

        GameObject diamondA = CreateCube(name + "_Facet_A", Vector3.zero, new Vector3(0.24f, 0.74f, 0.24f), material, star.transform);
        diamondA.transform.localRotation = Quaternion.Euler(45f, 0f, 45f);
        GameObject diamondB = CreateCube(name + "_Facet_B", Vector3.zero, new Vector3(0.18f, 0.5f, 0.18f), material, star.transform);
        diamondB.transform.localRotation = Quaternion.Euler(-35f, 62f, 25f);
        DestroyColliderImmediate(diamondA);
        DestroyColliderImmediate(diamondB);
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = position;
        cube.transform.localScale = scale;
        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        return cube;
    }

    private static GameObject CreateCylinder(string name, Vector3 position, Vector3 scale, Material material, Transform parent, Quaternion rotation)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = name;
        cylinder.transform.SetParent(parent, false);
        cylinder.transform.localPosition = position;
        cylinder.transform.localRotation = rotation;
        cylinder.transform.localScale = scale;
        Renderer renderer = cylinder.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        return cylinder;
    }

    private static Transform CreateMarker(string name, Vector3 position, Transform parent)
    {
        GameObject marker = new GameObject(name);
        marker.transform.SetParent(parent, false);
        marker.transform.position = position;
        return marker.transform;
    }

    private static void DestroyColliderImmediate(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.DestroyImmediate(collider);
        }
    }
}
