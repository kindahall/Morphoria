using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
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
}
