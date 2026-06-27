using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
    public sealed class MorphoriaWorldInfo
    {
        public string id;
        public string displayName;
        public Color color;
    }

    public sealed class MorphoriaLevelInfo
    {
        public string id;
        public string worldId;
        public string displayName;
        public string sceneName;
        public int order;
        public int targetGoldenStars;
        public int targetPrismStars;
        public int targetVillagers;
    }

    public static class MorphoriaGameContent
    {
        public const string MainMenuScene = "MainMenu";
        public const string HubScene = "VillageEcloriaHub";
        public const string WorldMapScene = "WorldMap";
        public const string LevelOneScene = "LePontDesQuatreFormes";

        public static readonly MorphoriaWorldInfo[] Worlds =
        {
            new MorphoriaWorldInfo { id = "ecloria", displayName = "Ecloria", color = new Color(0.37f, 0.67f, 0.38f) },
            new MorphoriaWorldInfo { id = "canyon", displayName = "Canyon Fracture", color = new Color(0.72f, 0.43f, 0.24f) },
            new MorphoriaWorldInfo { id = "gardens", displayName = "Jardins Suspendus", color = new Color(0.28f, 0.74f, 0.37f) },
            new MorphoriaWorldInfo { id = "archives", displayName = "Archives Origami", color = new Color(0.72f, 0.62f, 0.95f) },
            new MorphoriaWorldInfo { id = "forge", displayName = "Forge des Lames", color = new Color(0.44f, 0.72f, 0.9f) },
            new MorphoriaWorldInfo { id = "fortress", displayName = "Forteresse-Cage", color = new Color(0.52f, 0.25f, 0.82f) }
        };

        public static readonly MorphoriaLevelInfo[] Levels =
        {
            new MorphoriaLevelInfo { id = "level_01_bridge_four_forms", worldId = "ecloria", displayName = "Le Pont des Quatre Formes", sceneName = LevelOneScene, order = 0, targetGoldenStars = 50, targetPrismStars = 5, targetVillagers = 4 },
            new MorphoriaLevelInfo { id = "level_02_fracture_pass", worldId = "canyon", displayName = "La Passe Fracturee", sceneName = "CanyonFracturePass", order = 1, targetGoldenStars = 35, targetPrismStars = 3, targetVillagers = 2 },
            new MorphoriaLevelInfo { id = "level_03_wind_gardens", worldId = "gardens", displayName = "Les Couronnes du Vent", sceneName = "JardinsSuspendusVent", order = 2, targetGoldenStars = 35, targetPrismStars = 3, targetVillagers = 2 },
            new MorphoriaLevelInfo { id = "level_04_origami_archives", worldId = "archives", displayName = "Les Archives Origami", sceneName = "ArchivesOrigami", order = 3, targetGoldenStars = 35, targetPrismStars = 3, targetVillagers = 2 },
            new MorphoriaLevelInfo { id = "level_05_blade_forge", worldId = "forge", displayName = "La Forge des Lames", sceneName = "ForgeDesLames", order = 4, targetGoldenStars = 35, targetPrismStars = 3, targetVillagers = 2 },
            new MorphoriaLevelInfo { id = "level_06_noctar_fortress", worldId = "fortress", displayName = "Forteresse-Cage de Noctar", sceneName = "ForteresseCageNoctar", order = 5, targetGoldenStars = 45, targetPrismStars = 4, targetVillagers = 4 }
        };

        public static MorphoriaWorldInfo GetWorld(string id)
        {
            for (int i = 0; i < Worlds.Length; i++)
            {
                if (Worlds[i].id == id)
                {
                    return Worlds[i];
                }
            }

            return Worlds[0];
        }

        public static MorphoriaLevelInfo GetLevel(string id)
        {
            for (int i = 0; i < Levels.Length; i++)
            {
                if (Levels[i].id == id)
                {
                    return Levels[i];
                }
            }

            return Levels[0];
        }

        public static MorphoriaLevelInfo GetLevelByScene(string sceneName)
        {
            for (int i = 0; i < Levels.Length; i++)
            {
                if (Levels[i].sceneName == sceneName)
                {
                    return Levels[i];
                }
            }

            return null;
        }

        public static MorphoriaLevelInfo GetNextLevel(string id)
        {
            MorphoriaLevelInfo level = GetLevel(id);
            int nextOrder = level.order + 1;
            for (int i = 0; i < Levels.Length; i++)
            {
                if (Levels[i].order == nextOrder)
                {
                    return Levels[i];
                }
            }

            return null;
        }
    }

    [Serializable]
    public sealed class MorphoriaLevelProgress
    {
        public string levelId;
        public bool unlocked;
        public bool completed;
        public int bestGoldenStars;
        public int bestPrismStars;
        public int bestVillagers;
        public int clears;
    }

    [Serializable]
    public sealed class MorphoriaSaveData
    {
        public int version = 1;
        public string currentLevelId = MorphoriaGameContent.Levels[0].id;
        public string lastScene = MorphoriaGameContent.HubScene;
        public int totalGoldenStars;
        public int totalPrismStars;
        public int totalVillagersSaved;
        public bool finalBossDefeated;
        public float masterVolume = 0.85f;
        public float cameraSensitivity = 1.0f;
        public bool colorAssist;
        public bool subtitlesEnabled = true;
        public bool reduceMotion;
        public List<MorphoriaLevelProgress> levels = new List<MorphoriaLevelProgress>();
    }

    public static class MorphoriaSaveSystem
    {
        private const string SaveFileName = "morphoria_save.json";

        public static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public static bool SaveExists()
        {
            return File.Exists(SavePath);
        }

        public static MorphoriaSaveData CreateNew()
        {
            MorphoriaSaveData data = new MorphoriaSaveData();
            data.levels = new List<MorphoriaLevelProgress>();

            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                MorphoriaLevelInfo level = MorphoriaGameContent.Levels[i];
                data.levels.Add(new MorphoriaLevelProgress
                {
                    levelId = level.id,
                    unlocked = i == 0,
                    completed = false
                });
            }

            Normalize(data);
            return data;
        }

        public static MorphoriaSaveData LoadOrCreate()
        {
            if (!SaveExists())
            {
                return CreateNew();
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                MorphoriaSaveData data = JsonUtility.FromJson<MorphoriaSaveData>(json);
                if (data == null)
                {
                    return CreateNew();
                }

                Normalize(data);
                return data;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Morphoria save could not be loaded: " + exception.Message);
                return CreateNew();
            }
        }

        public static void Save(MorphoriaSaveData data)
        {
            Normalize(data);
            Directory.CreateDirectory(Application.persistentDataPath);
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        }

        public static MorphoriaLevelProgress GetProgress(MorphoriaSaveData data, string levelId)
        {
            Normalize(data);
            for (int i = 0; i < data.levels.Count; i++)
            {
                if (data.levels[i].levelId == levelId)
                {
                    return data.levels[i];
                }
            }

            MorphoriaLevelProgress progress = new MorphoriaLevelProgress { levelId = levelId };
            data.levels.Add(progress);
            return progress;
        }

        public static void Normalize(MorphoriaSaveData data)
        {
            if (data.levels == null)
            {
                data.levels = new List<MorphoriaLevelProgress>();
            }

            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                string levelId = MorphoriaGameContent.Levels[i].id;
                bool found = false;
                for (int j = 0; j < data.levels.Count; j++)
                {
                    if (data.levels[j].levelId == levelId)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    data.levels.Add(new MorphoriaLevelProgress
                    {
                        levelId = levelId,
                        unlocked = i == 0
                    });
                }
            }

            MorphoriaLevelProgress first = GetProgressWithoutNormalize(data, MorphoriaGameContent.Levels[0].id);
            if (first != null)
            {
                first.unlocked = true;
            }

            if (string.IsNullOrEmpty(data.currentLevelId))
            {
                data.currentLevelId = MorphoriaGameContent.Levels[0].id;
            }

            if (string.IsNullOrEmpty(data.lastScene))
            {
                data.lastScene = MorphoriaGameContent.HubScene;
            }

            data.masterVolume = Mathf.Clamp01(data.masterVolume);
            data.cameraSensitivity = Mathf.Clamp(data.cameraSensitivity, 0.35f, 2.5f);
            RecalculateTotals(data);
        }

        private static MorphoriaLevelProgress GetProgressWithoutNormalize(MorphoriaSaveData data, string levelId)
        {
            if (data.levels == null)
            {
                return null;
            }

            for (int i = 0; i < data.levels.Count; i++)
            {
                if (data.levels[i].levelId == levelId)
                {
                    return data.levels[i];
                }
            }

            return null;
        }

        private static void RecalculateTotals(MorphoriaSaveData data)
        {
            int golden = 0;
            int prism = 0;
            int villagers = 0;

            for (int i = 0; i < data.levels.Count; i++)
            {
                golden += Mathf.Max(0, data.levels[i].bestGoldenStars);
                prism += Mathf.Max(0, data.levels[i].bestPrismStars);
                villagers += Mathf.Max(0, data.levels[i].bestVillagers);
            }

            data.totalGoldenStars = golden;
            data.totalPrismStars = prism;
            data.totalVillagersSaved = villagers;
        }
    }

    public sealed class MorphoriaGameSession : MonoBehaviour
    {
        public static MorphoriaGameSession Instance { get; private set; }

        public MorphoriaSaveData SaveData { get; private set; }
        public string StatusText { get; private set; }

        public bool CanContinue => MorphoriaSaveSystem.SaveExists();

        public event Action<MorphoriaSaveData> SaveChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeBootstrap()
        {
            GetOrCreate();
        }

        public static MorphoriaGameSession GetOrCreate()
        {
            if (Instance != null)
            {
                return Instance;
            }

            MorphoriaGameSession existing = FindAnyObjectByType<MorphoriaGameSession>();
            if (existing != null)
            {
                return existing;
            }

            GameObject gameObject = new GameObject("Morphoria_GameSession");
            return gameObject.AddComponent<MorphoriaGameSession>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SaveData = MorphoriaSaveSystem.LoadOrCreate();
            ApplySettingsToScene();
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= HandleSceneLoaded;
                Instance = null;
            }
        }

        public void NewGame()
        {
            SaveData = MorphoriaSaveSystem.CreateNew();
            StatusText = "Nouvelle partie";
            Save();
            LoadScene(MorphoriaGameContent.HubScene);
        }

        public void ContinueGame()
        {
            SaveData = MorphoriaSaveSystem.LoadOrCreate();
            string scene = string.IsNullOrEmpty(SaveData.lastScene) ? MorphoriaGameContent.HubScene : SaveData.lastScene;
            StatusText = "Sauvegarde chargee";
            LoadScene(scene);
        }

        public void LoadHub()
        {
            LoadScene(MorphoriaGameContent.HubScene);
        }

        public void LoadWorldMap()
        {
            LoadScene(MorphoriaGameContent.WorldMapScene);
        }

        public void LoadMainMenu()
        {
            LoadScene(MorphoriaGameContent.MainMenuScene);
        }

        public void LoadLevel(string levelId)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevel(levelId);
            MorphoriaLevelProgress progress = MorphoriaSaveSystem.GetProgress(SaveData, level.id);

            if (!progress.unlocked)
            {
                StatusText = "Niveau verrouille";
                return;
            }

            SaveData.currentLevelId = level.id;
            SaveData.lastScene = level.sceneName;
            Save();
            LoadScene(level.sceneName);
        }

        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = MorphoriaGameContent.HubScene;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAfterDelay(string sceneName, float delaySeconds)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, delaySeconds));
        }

        public void MarkCurrentLevelComplete(int goldenStars, int prismStars, int villagersSaved)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevel(SaveData.currentLevelId);
            MarkLevelComplete(level.id, goldenStars, prismStars, villagersSaved);
        }

        public void MarkLevelComplete(string levelId, int goldenStars, int prismStars, int villagersSaved)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevel(levelId);
            MorphoriaLevelProgress progress = MorphoriaSaveSystem.GetProgress(SaveData, level.id);

            progress.completed = true;
            progress.clears++;
            progress.bestGoldenStars = Mathf.Max(progress.bestGoldenStars, goldenStars);
            progress.bestPrismStars = Mathf.Max(progress.bestPrismStars, prismStars);
            progress.bestVillagers = Mathf.Max(progress.bestVillagers, villagersSaved);

            MorphoriaLevelInfo next = MorphoriaGameContent.GetNextLevel(level.id);
            if (next != null)
            {
                MorphoriaSaveSystem.GetProgress(SaveData, next.id).unlocked = true;
            }
            else
            {
                SaveData.finalBossDefeated = true;
            }

            SaveData.lastScene = MorphoriaGameContent.HubScene;
            StatusText = level.displayName + " termine";
            Save();
        }

        public void Save()
        {
            MorphoriaSaveSystem.Save(SaveData);
            ApplySettingsToScene();
            SaveChanged?.Invoke(SaveData);
        }

        public MorphoriaLevelProgress ProgressFor(string levelId)
        {
            return MorphoriaSaveSystem.GetProgress(SaveData, levelId);
        }

        private IEnumerator LoadSceneRoutine(string sceneName, float delaySeconds)
        {
            float end = Time.unscaledTime + Mathf.Max(0f, delaySeconds);
            while (Time.unscaledTime < end)
            {
                yield return null;
            }

            LoadScene(sceneName);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 1f;
            ApplySettingsToScene();

            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevelByScene(scene.name);
            if (level != null)
            {
                SaveData.currentLevelId = level.id;
                SaveData.lastScene = level.sceneName;
                Save();
            }
            else if (scene.name != MorphoriaGameContent.MainMenuScene && scene.name != MorphoriaGameContent.WorldMapScene)
            {
                SaveData.lastScene = scene.name;
                Save();
            }
        }

        private void ApplySettingsToScene()
        {
            if (SaveData == null)
            {
                return;
            }

            AudioListener.volume = SaveData.masterVolume;
            ThirdPersonCamera[] cameras = FindObjectsByType<ThirdPersonCamera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].mouseSensitivity = 2.3f * SaveData.cameraSensitivity;
            }
        }
    }

    public static class MorphoriaSettingsPanel
    {
        public static void Draw(MorphoriaGameSession session)
        {
            MorphoriaSaveData data = session.SaveData;

            GUILayout.Label("Reglages");
            float volume = GUILayout.HorizontalSlider(data.masterVolume, 0f, 1f, GUILayout.Width(260f));
            GUILayout.Label("Volume  " + Mathf.RoundToInt(volume * 100f) + "%");

            float sensitivity = GUILayout.HorizontalSlider(data.cameraSensitivity, 0.35f, 2.5f, GUILayout.Width(260f));
            GUILayout.Label("Camera  " + sensitivity.ToString("0.00") + "x");

            bool colorAssist = GUILayout.Toggle(data.colorAssist, "Aide couleur");
            bool subtitles = GUILayout.Toggle(data.subtitlesEnabled, "Sous-titres");
            bool reduceMotion = GUILayout.Toggle(data.reduceMotion, "Mouvements reduits");

            bool changed =
                Mathf.Abs(volume - data.masterVolume) > 0.001f ||
                Mathf.Abs(sensitivity - data.cameraSensitivity) > 0.001f ||
                colorAssist != data.colorAssist ||
                subtitles != data.subtitlesEnabled ||
                reduceMotion != data.reduceMotion;

            if (changed)
            {
                data.masterVolume = volume;
                data.cameraSensitivity = sensitivity;
                data.colorAssist = colorAssist;
                data.subtitlesEnabled = subtitles;
                data.reduceMotion = reduceMotion;
                session.Save();
            }
        }
    }

    public sealed class MorphoriaMenuScreen : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private GUIStyle titleStyle;
        private GUIStyle buttonStyle;
        private GUIStyle panelStyle;
        private bool showSettings;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();

            Rect panel = new Rect(Screen.width * 0.5f - 230f, Screen.height * 0.5f - 220f, 460f, 440f);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 38f, panel.y + 28f, panel.width - 76f, 64f), "Morphoria", titleStyle);

            GUILayout.BeginArea(new Rect(panel.x + 54f, panel.y + 112f, panel.width - 108f, panel.height - 142f));
            if (GUILayout.Button("Nouvelle partie", buttonStyle, GUILayout.Height(46f)))
            {
                session.NewGame();
            }

            GUI.enabled = session.CanContinue;
            if (GUILayout.Button("Continuer", buttonStyle, GUILayout.Height(46f)))
            {
                session.ContinueGame();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Carte du monde", buttonStyle, GUILayout.Height(46f)))
            {
                session.LoadWorldMap();
            }

            if (GUILayout.Button("Reglages", buttonStyle, GUILayout.Height(46f)))
            {
                showSettings = !showSettings;
            }

            if (showSettings)
            {
                MorphoriaSettingsPanel.Draw(session);
            }

            if (GUILayout.Button("Quitter", buttonStyle, GUILayout.Height(42f)))
            {
                Application.Quit();
            }
            GUILayout.EndArea();
        }

        private void DrawBackdrop()
        {
            Color old = GUI.color;
            GUI.color = new Color(0.07f, 0.1f, 0.16f, 1f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = new Color(0.1f, 0.42f, 0.55f, 0.55f);
            GUI.DrawTexture(new Rect(0f, Screen.height * 0.58f, Screen.width, Screen.height * 0.42f), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 42,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            panelStyle = new GUIStyle(GUI.skin.box);
        }
    }

    public sealed class MorphoriaWorldMapScreen : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private GUIStyle titleStyle;
        private GUIStyle levelStyle;
        private GUIStyle panelStyle;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();

            Rect panel = new Rect(32f, 28f, Mathf.Min(760f, Screen.width - 64f), Screen.height - 56f);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 20f, panel.width - 56f, 46f), "Carte du monde", titleStyle);

            GUILayout.BeginArea(new Rect(panel.x + 34f, panel.y + 84f, panel.width - 68f, panel.height - 132f));
            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                MorphoriaLevelInfo level = MorphoriaGameContent.Levels[i];
                MorphoriaWorldInfo world = MorphoriaGameContent.GetWorld(level.worldId);
                MorphoriaLevelProgress progress = session.ProgressFor(level.id);
                string state = progress.completed ? "Termine" : progress.unlocked ? "Disponible" : "Verrouille";
                GUI.color = Color.Lerp(world.color, Color.white, 0.22f);
                GUI.enabled = progress.unlocked;
                if (GUILayout.Button(level.displayName + "    " + state, levelStyle, GUILayout.Height(44f)))
                {
                    session.LoadLevel(level.id);
                }
                GUI.enabled = true;
                GUI.color = Color.white;
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(panel.x + 34f, panel.yMax - 48f, panel.width - 68f, 36f));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Village", GUILayout.Height(32f)))
            {
                session.LoadHub();
            }
            if (GUILayout.Button("Menu", GUILayout.Height(32f)))
            {
                session.LoadMainMenu();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawBackdrop()
        {
            Color old = GUI.color;
            GUI.color = new Color(0.08f, 0.12f, 0.18f, 1f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };
            levelStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleLeft
            };
            levelStyle.padding = new RectOffset(18, 18, 8, 8);
            panelStyle = new GUIStyle(GUI.skin.box);
        }
    }

    public sealed class MorphoriaHubState : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle panelStyle;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void OnGUI()
        {
            EnsureStyles();
            MorphoriaSaveData data = session.SaveData;
            Rect panel = new Rect(18f, Screen.height - 158f, 390f, 136f);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 12f, panel.width - 36f, 28f), "Village d'Ecloria", titleStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 46f, panel.width - 36f, 24f), "Villageois sauves  " + data.totalVillagersSaved, labelStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 72f, panel.width - 36f, 24f), "Etoiles  " + data.totalGoldenStars + "    Prismes  " + data.totalPrismStars, labelStyle);

            Rect buttons = new Rect(panel.x + 18f, panel.y + 98f, panel.width - 36f, 28f);
            GUILayout.BeginArea(buttons);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Carte"))
            {
                session.LoadWorldMap();
            }
            if (GUILayout.Button("Menu"))
            {
                session.LoadMainMenu();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = new Color(0.92f, 0.96f, 1f) }
            };
            panelStyle = new GUIStyle(GUI.skin.box);
        }
    }

    public sealed class MorphoriaPauseMenu : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private bool paused;
        private bool showSettings;
        private GUIStyle titleStyle;
        private GUIStyle buttonStyle;
        private GUIStyle panelStyle;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetPaused(!paused);
            }
        }

        private void OnDisable()
        {
            if (paused)
            {
                SetPaused(false);
            }
        }

        private void OnGUI()
        {
            if (!paused)
            {
                return;
            }

            EnsureStyles();
            Color old = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.52f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = old;

            Rect panel = new Rect(Screen.width * 0.5f - 210f, Screen.height * 0.5f - 195f, 420f, 390f);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 22f, panel.width - 68f, 36f), "Pause", titleStyle);

            GUILayout.BeginArea(new Rect(panel.x + 58f, panel.y + 76f, panel.width - 116f, panel.height - 100f));
            if (GUILayout.Button("Reprendre", buttonStyle, GUILayout.Height(40f)))
            {
                SetPaused(false);
            }

            if (GUILayout.Button("Reglages", buttonStyle, GUILayout.Height(40f)))
            {
                showSettings = !showSettings;
            }

            if (showSettings)
            {
                MorphoriaSettingsPanel.Draw(session);
            }

            if (GUILayout.Button("Carte du monde", buttonStyle, GUILayout.Height(40f)))
            {
                SetPaused(false);
                session.LoadWorldMap();
            }

            if (GUILayout.Button("Retour au village", buttonStyle, GUILayout.Height(40f)))
            {
                SetPaused(false);
                session.LoadHub();
            }

            if (GUILayout.Button("Menu principal", buttonStyle, GUILayout.Height(40f)))
            {
                SetPaused(false);
                session.LoadMainMenu();
            }
            GUILayout.EndArea();
        }

        private void SetPaused(bool value)
        {
            paused = value;
            Time.timeScale = paused ? 0f : 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            panelStyle = new GUIStyle(GUI.skin.box);
        }
    }

    public sealed class MorphoriaScenePortal : MonoBehaviour
    {
        public string label = "Portail";
        public string targetScene;
        public string targetLevelId;
        public bool requireInteraction = true;

        private void OnTriggerStay(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            player.ShowFeedback(requireInteraction ? label + "  F" : label);
            if (!requireInteraction || Input.GetKeyDown(KeyCode.F))
            {
                MorphoriaGameSession session = MorphoriaGameSession.GetOrCreate();
                if (!string.IsNullOrEmpty(targetLevelId))
                {
                    session.LoadLevel(targetLevelId);
                }
                else
                {
                    session.LoadScene(targetScene);
                }
            }
        }
    }
}
