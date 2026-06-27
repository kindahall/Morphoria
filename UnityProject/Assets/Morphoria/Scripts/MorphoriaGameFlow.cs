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
    public sealed class MorphoriaLevelClearResult
    {
        public string levelId;
        public string levelName;
        public string rank;
        public string nextLevelName;
        public bool firstClear;
        public bool newBest;
        public bool unlockedNextLevel;
        public int goldenStars;
        public int targetGoldenStars;
        public int prismStars;
        public int targetPrismStars;
        public int villagersSaved;
        public int targetVillagers;
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





}
