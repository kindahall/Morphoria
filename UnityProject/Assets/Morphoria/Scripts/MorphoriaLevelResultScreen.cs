using System;
using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaLevelResultScreen : MonoBehaviour
    {
        private MorphoriaLevelClearResult result;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private bool showing;

        public bool IsShowing => showing;

        public void Show(MorphoriaLevelClearResult clearResult)
        {
            result = clearResult;
            showing = clearResult != null;

            if (showing)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void OnGUI()
        {
            if (!showing || result == null)
            {
                return;
            }

            EnsureStyles();

            Color old = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.62f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = old;

            float width = Mathf.Min(560f, Screen.width - 48f);
            float height = 430f;
            Rect panel = new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height * 0.5f - height * 0.5f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUI.Label(new Rect(panel.x + 34f, panel.y + 24f, panel.width - 68f, 34f), "Niveau termine", titleStyle);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 62f, panel.width - 68f, 28f), result.levelName, labelStyle);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 104f, panel.width - 68f, 30f), "Rang  " + result.rank, titleStyle);

            DrawStat(panel.x + 34f, panel.y + 154f, "Etoiles", result.goldenStars, result.targetGoldenStars);
            DrawStat(panel.x + 34f, panel.y + 188f, "Prismes", result.prismStars, result.targetPrismStars);
            DrawStat(panel.x + 34f, panel.y + 222f, "Villageois", result.villagersSaved, result.targetVillagers);

            string status = result.firstClear ? "Niveau valide" : result.newBest ? "Nouveau record" : "Progression gardee";
            if (result.unlockedNextLevel && !string.IsNullOrEmpty(result.nextLevelName))
            {
                status += "    " + result.nextLevelName;
            }

            GUI.Label(new Rect(panel.x + 34f, panel.y + 270f, panel.width - 68f, 28f), status, labelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 34f, panel.yMax - 78f, panel.width - 68f, 46f));
            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(result.nextLevelId) && GUILayout.Button("Suite", buttonStyle, GUILayout.Height(42f)))
            {
                LoadNextLevel();
            }

            if (GUILayout.Button("Village", buttonStyle, GUILayout.Height(42f)))
            {
                LoadHub();
            }

            if (GUILayout.Button("Carte", buttonStyle, GUILayout.Height(42f)))
            {
                LoadWorldMap();
            }

            if (GUILayout.Button("Rejouer", buttonStyle, GUILayout.Height(42f)))
            {
                Replay();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawStat(float x, float y, string label, int value, int target)
        {
            GUI.Label(new Rect(x, y, 150f, 24f), label, labelStyle);
            GUI.Label(new Rect(x + 150f, y, 140f, 24f), value + " / " + target, labelStyle);

            Rect bar = new Rect(x + 300f, y + 5f, 180f, 12f);
            Color old = GUI.color;
            GUI.color = new Color(0.12f, 0.16f, 0.22f, 1f);
            GUI.DrawTexture(bar, Texture2D.whiteTexture);
            GUI.color = new Color(0.9f, 0.72f, 0.22f, 1f);
            float fill = target <= 0 ? 1f : Mathf.Clamp01(value / (float)target);
            GUI.DrawTexture(new Rect(bar.x, bar.y, bar.width * fill, bar.height), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void LoadHub()
        {
            showing = false;
            Time.timeScale = 1f;
            MorphoriaGameSession.GetOrCreate().LoadHub();
        }

        private void LoadWorldMap()
        {
            showing = false;
            Time.timeScale = 1f;
            MorphoriaGameSession.GetOrCreate().LoadWorldMap();
        }

        private void LoadNextLevel()
        {
            showing = false;
            Time.timeScale = 1f;
            MorphoriaGameSession.GetOrCreate().LoadLevel(result.nextLevelId);
        }

        private void Replay()
        {
            showing = false;
            Time.timeScale = 1f;
            MorphoriaGameSession.GetOrCreate().LoadLevel(result.levelId);
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            panelStyle = new GUIStyle(GUI.skin.box);
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.92f, 0.96f, 1f) }
            };
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
