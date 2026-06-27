using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
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
}
