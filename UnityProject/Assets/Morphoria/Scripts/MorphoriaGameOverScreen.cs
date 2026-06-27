using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaGameOverScreen : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private MorphoriaPlayer player;
        private bool showing;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;

        public bool IsShowing => showing;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        public void Show(MorphoriaPlayer defeatedPlayer)
        {
            player = defeatedPlayer;
            showing = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            if (showing)
            {
                showing = false;
                Time.timeScale = 1f;
            }
        }

        private void OnGUI()
        {
            if (!showing)
            {
                return;
            }

            EnsureStyles();

            Color old = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.66f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = old;

            float width = Mathf.Min(460f, Screen.width - 48f);
            float height = 276f;
            Rect panel = new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height * 0.5f - height * 0.5f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 28f, panel.width - 68f, 42f), "KO", titleStyle);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 80f, panel.width - 68f, 28f), "Plus de coeurs", labelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 42f, panel.y + 132f, panel.width - 84f, panel.height - 154f));
            if (GUILayout.Button("Checkpoint", buttonStyle, GUILayout.Height(42f)))
            {
                ContinueAtCheckpoint();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Village", buttonStyle, GUILayout.Height(40f)))
            {
                LoadHub();
            }

            if (GUILayout.Button("Carte", buttonStyle, GUILayout.Height(40f)))
            {
                LoadWorldMap();
            }

            if (GUILayout.Button("Menu", buttonStyle, GUILayout.Height(40f)))
            {
                LoadMainMenu();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void ContinueAtCheckpoint()
        {
            showing = false;
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (player == null)
            {
                player = FindAnyObjectByType<MorphoriaPlayer>();
            }

            if (player != null)
            {
                player.RecoverAtCheckpoint();
            }
            else
            {
                session.LoadHub();
            }
        }

        private void LoadHub()
        {
            showing = false;
            Time.timeScale = 1f;
            session.LoadHub();
        }

        private void LoadWorldMap()
        {
            showing = false;
            Time.timeScale = 1f;
            session.LoadWorldMap();
        }

        private void LoadMainMenu()
        {
            showing = false;
            Time.timeScale = 1f;
            session.LoadMainMenu();
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
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                alignment = TextAnchor.MiddleCenter,
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
