using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
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
}
