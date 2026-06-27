using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
    public sealed class MorphoriaGameSession : MonoBehaviour
    {
        public static MorphoriaGameSession Instance { get; private set; }

        public MorphoriaSaveData SaveData { get; private set; }
        public string StatusText { get; private set; }
        public MorphoriaLevelClearResult LastClearResult { get; private set; }

        public bool CanContinue => MorphoriaSaveSystem.SaveExists();

        public event Action<MorphoriaSaveData> SaveChanged;

        private Coroutine sceneLoadRoutine;

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
            MorphoriaScreenFader.GetOrCreate();
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

        public void LoadFinale()
        {
            if (SaveData != null)
            {
                SaveData.lastScene = MorphoriaGameContent.FinaleScene;
                Save();
            }

            LoadScene(MorphoriaGameContent.FinaleScene);
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
            if (sceneLoadRoutine != null)
            {
                StopCoroutine(sceneLoadRoutine);
            }

            sceneLoadRoutine = StartCoroutine(LoadSceneWithFadeRoutine(sceneName));
        }

        public void LoadSceneAfterDelay(string sceneName, float delaySeconds)
        {
            StartCoroutine(DelayedLoadSceneRoutine(sceneName, delaySeconds));
        }

        public MorphoriaLevelClearResult MarkCurrentLevelComplete(int goldenStars, int prismStars, int villagersSaved)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevel(SaveData.currentLevelId);
            return MarkLevelComplete(level.id, goldenStars, prismStars, villagersSaved);
        }

        public MorphoriaLevelClearResult MarkLevelComplete(string levelId, int goldenStars, int prismStars, int villagersSaved)
        {
            LastClearResult = MorphoriaCampaignProgression.MarkLevelComplete(SaveData, levelId, goldenStars, prismStars, villagersSaved);
            StatusText = LastClearResult.levelName + " termine";
            Save();
            return LastClearResult;
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

        public bool TryGetActiveLevel(out MorphoriaLevelInfo level)
        {
            Scene scene = SceneManager.GetActiveScene();
            level = MorphoriaGameContent.GetLevelByScene(scene.name);
            if (level == null && SaveData != null && !string.IsNullOrEmpty(SaveData.currentLevelId))
            {
                MorphoriaLevelInfo savedLevel = MorphoriaGameContent.GetLevel(SaveData.currentLevelId);
                if (savedLevel != null && savedLevel.sceneName == scene.name)
                {
                    level = savedLevel;
                }
            }

            return level != null;
        }

        public bool HasCollectedInActiveLevel(string objectId, CollectibleKind kind)
        {
            if (SaveData == null || !TryGetActiveLevel(out MorphoriaLevelInfo level))
            {
                return false;
            }

            return MorphoriaSaveSystem.HasCollected(SaveData, level.id, objectId, kind);
        }

        public void RecordCollectedInActiveLevel(string objectId, CollectibleKind kind)
        {
            if (SaveData == null || !TryGetActiveLevel(out MorphoriaLevelInfo level))
            {
                return;
            }

            if (MorphoriaSaveSystem.RecordCollected(SaveData, level.id, objectId, kind))
            {
                Save();
            }
        }

        public bool HasRescuedVillagerInActiveLevel(string villagerId)
        {
            if (SaveData == null || !TryGetActiveLevel(out MorphoriaLevelInfo level))
            {
                return false;
            }

            return MorphoriaSaveSystem.HasRescuedVillager(SaveData, level.id, villagerId);
        }

        public void RecordRescuedVillagerInActiveLevel(string villagerId)
        {
            if (SaveData == null || !TryGetActiveLevel(out MorphoriaLevelInfo level))
            {
                return;
            }

            if (MorphoriaSaveSystem.RecordRescuedVillager(SaveData, level.id, villagerId))
            {
                Save();
            }
        }

        private IEnumerator DelayedLoadSceneRoutine(string sceneName, float delaySeconds)
        {
            float end = Time.unscaledTime + Mathf.Max(0f, delaySeconds);
            while (Time.unscaledTime < end)
            {
                yield return null;
            }

            LoadScene(sceneName);
        }

        private IEnumerator LoadSceneWithFadeRoutine(string sceneName)
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = MorphoriaGameContent.HubScene;
            }

            MorphoriaScreenFader fader = MorphoriaScreenFader.GetOrCreate();
            bool reduceMotion = SaveData != null && SaveData.reduceMotion;
            yield return fader.FadeTo(1f, reduceMotion ? 0.05f : 0.18f);
            SceneManager.LoadScene(sceneName);
            yield return null;
            ApplySettingsToScene();
            yield return fader.FadeTo(0f, reduceMotion ? 0.05f : 0.24f);
            sceneLoadRoutine = null;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 1f;
            MorphoriaScreenFader.GetOrCreate();
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
                cameras[i].reduceMotion = SaveData.reduceMotion;
            }
        }
    }
}
