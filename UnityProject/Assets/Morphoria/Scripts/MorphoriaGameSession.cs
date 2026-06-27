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
