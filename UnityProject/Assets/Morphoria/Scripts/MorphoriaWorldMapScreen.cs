using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
    public sealed class MorphoriaWorldMapScreen : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private string selectedLevelId;
        private GUIStyle titleStyle;
        private GUIStyle levelStyle;
        private GUIStyle labelStyle;
        private GUIStyle smallStyle;
        private GUIStyle panelStyle;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
            selectedLevelId = session.SaveData.currentLevelId;
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();

            bool wide = Screen.width >= 1120f;
            float margin = wide ? 24f : 32f;
            float detailWidth = wide ? 360f : 0f;
            float listWidth = wide ? 360f : Screen.width - margin * 2f;
            Rect panel = wide
                ? new Rect(margin, 28f, listWidth, Screen.height - 56f)
                : new Rect(margin, 28f, Screen.width - margin * 2f, Screen.height * 0.62f);
            Rect detail = wide
                ? new Rect(Screen.width - detailWidth - margin, 28f, detailWidth, Screen.height - 56f)
                : new Rect(margin, panel.yMax + 12f, panel.width, Screen.height - panel.yMax - 40f);

            DrawLevelList(panel);
            DrawLevelDetails(detail);
        }

        private void DrawLevelList(Rect panel)
        {
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 20f, panel.width - 56f, 46f), "Carte du monde", titleStyle);
            GUI.Label(new Rect(panel.x + 28f, panel.y + 58f, panel.width - 56f, 24f), "Etoiles " + session.SaveData.totalGoldenStars + "    Prismes " + session.SaveData.totalPrismStars + "    Villageois " + session.SaveData.totalVillagersSaved, labelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 34f, panel.y + 96f, panel.width - 68f, panel.height - 144f));
            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                MorphoriaLevelInfo level = MorphoriaGameContent.Levels[i];
                MorphoriaWorldInfo world = MorphoriaGameContent.GetWorld(level.worldId);
                MorphoriaLevelProgress progress = session.ProgressFor(level.id);
                string state = progress.completed ? "Termine" : progress.unlocked ? "Disponible" : "Verrouille";
                string stats = progress.completed
                    ? progress.bestGoldenStars + "/" + level.targetGoldenStars + "  " + progress.bestPrismStars + "/" + level.targetPrismStars + "  " + progress.bestVillagers + "/" + level.targetVillagers
                    : "--";

                GUI.color = level.id == selectedLevelId ? Color.Lerp(world.color, Color.white, 0.45f) : Color.Lerp(world.color, Color.white, 0.18f);
                if (GUILayout.Button((i + 1).ToString("00") + "  " + level.displayName + "    " + state + "    " + stats, levelStyle, GUILayout.Height(44f)))
                {
                    selectedLevelId = level.id;
                }
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

        private void DrawLevelDetails(Rect panel)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevel(selectedLevelId);
            MorphoriaLevelProgress progress = session.ProgressFor(level.id);
            MorphoriaWorldInfo world = MorphoriaGameContent.GetWorld(level.worldId);

            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.color = Color.Lerp(world.color, Color.white, 0.3f);
            GUI.DrawTexture(new Rect(panel.x, panel.y, panel.width, 4f), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(panel.x + 24f, panel.y + 22f, panel.width - 48f, 32f), level.displayName, titleStyle);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 60f, panel.width - 48f, 24f), MorphoriaGameContent.GetWorld(level.worldId).displayName, labelStyle);

            string status = progress.completed ? "Termine" : progress.unlocked ? "Disponible" : "Verrouille";
            GUI.Label(new Rect(panel.x + 24f, panel.y + 106f, panel.width - 48f, 24f), status, labelStyle);
            DrawStat(panel, 146f, "Etoiles", progress.bestGoldenStars, level.targetGoldenStars);
            DrawStat(panel, 184f, "Prismes", progress.bestPrismStars, level.targetPrismStars);
            DrawStat(panel, 222f, "Villageois", progress.bestVillagers, level.targetVillagers);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 264f, panel.width - 48f, 24f), "Clears  " + progress.clears, smallStyle);

            GUILayout.BeginArea(new Rect(panel.x + 24f, panel.yMax - 70f, panel.width - 48f, 46f));
            GUI.enabled = progress.unlocked;
            if (GUILayout.Button("Jouer", GUILayout.Height(42f)))
            {
                session.LoadLevel(level.id);
            }
            GUI.enabled = true;
            GUILayout.EndArea();
        }

        private void DrawStat(Rect panel, float y, string label, int value, int target)
        {
            GUI.Label(new Rect(panel.x + 24f, panel.y + y, 110f, 24f), label, labelStyle);
            GUI.Label(new Rect(panel.x + 134f, panel.y + y, 90f, 24f), value + " / " + target, labelStyle);

            Rect bar = new Rect(panel.x + 220f, panel.y + y + 7f, panel.width - 250f, 10f);
            Color old = GUI.color;
            GUI.color = new Color(0.12f, 0.16f, 0.22f, 1f);
            GUI.DrawTexture(bar, Texture2D.whiteTexture);
            GUI.color = new Color(0.9f, 0.72f, 0.22f, 1f);
            float fill = target <= 0 ? 1f : Mathf.Clamp01(value / (float)target);
            GUI.DrawTexture(new Rect(bar.x, bar.y, bar.width * fill, bar.height), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void DrawBackdrop()
        {
            Color old = GUI.color;
            GUI.color = new Color(0.05f, 0.08f, 0.12f, 0.42f);
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
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft
            };
            levelStyle.padding = new RectOffset(18, 18, 8, 8);
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = new Color(0.92f, 0.96f, 1f) },
                alignment = TextAnchor.MiddleLeft
            };
            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.82f, 0.88f, 0.94f) },
                alignment = TextAnchor.MiddleLeft
            };
            panelStyle = new GUIStyle(GUI.skin.box);
        }
    }
}
