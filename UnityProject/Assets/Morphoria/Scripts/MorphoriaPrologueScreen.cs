using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaPrologueScreen : MonoBehaviour
    {
        private struct Beat
        {
            public string speaker;
            public string line;

            public Beat(string speaker, string line)
            {
                this.speaker = speaker;
                this.line = line;
            }
        }

        private static readonly Beat[] Beats =
        {
            new Beat("Lina", "Le vent sent bizarre... comme si le ciel retenait son souffle."),
            new Beat("Taro", "Alors on reste ensemble. S'il arrive quelque chose, je bloque."),
            new Beat("Sia", "Tu bloques toujours. Moi, je coupe le probleme en deux."),
            new Beat("Milo", "Techniquement, certains problemes se plient mieux qu'ils ne se coupent."),
            new Beat("Nocterion", "La liberte vous rend fragiles. Mes cages vous garderont entiers."),
            new Beat("Taro", "Un village enferme n'est pas un village protege."),
            new Beat("Nocterion", "Alors venez apprendre la difference entre courage et imprudence.")
        };

        private MorphoriaGameSession session;
        private GUIStyle titleStyle;
        private GUIStyle speakerStyle;
        private GUIStyle lineStyle;
        private GUIStyle buttonStyle;
        private GUIStyle panelStyle;
        private int index;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Advance();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Finish();
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();

            float width = Mathf.Min(720f, Screen.width - 48f);
            float height = Mathf.Min(360f, Screen.height - 48f);
            Rect panel = new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height - height - 36f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUI.Label(new Rect(panel.x + 34f, panel.y + 24f, panel.width - 68f, 42f), "Le Village Vole", titleStyle);

            Beat beat = Beats[Mathf.Clamp(index, 0, Beats.Length - 1)];
            GUI.Label(new Rect(panel.x + 42f, panel.y + 86f, panel.width - 84f, 30f), beat.speaker, speakerStyle);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 124f, panel.width - 84f, 92f), beat.line, lineStyle);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 222f, panel.width - 84f, 24f), (index + 1) + " / " + Beats.Length, lineStyle);

            GUILayout.BeginArea(new Rect(panel.x + 42f, panel.yMax - 76f, panel.width - 84f, 46f));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(index >= Beats.Length - 1 ? "Vers Ecloria" : "Continuer", buttonStyle, GUILayout.Height(42f)))
            {
                Advance();
            }

            if (GUILayout.Button("Passer", buttonStyle, GUILayout.Height(42f), GUILayout.Width(120f)))
            {
                Finish();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void Advance()
        {
            if (index >= Beats.Length - 1)
            {
                Finish();
                return;
            }

            index++;
        }

        private void Finish()
        {
            if (session != null && session.SaveData != null)
            {
                session.SaveData.prologueSeen = true;
                session.SaveData.lastScene = MorphoriaGameContent.HubScene;
                session.Save();
                session.LoadHub();
            }
        }

        private void DrawBackdrop()
        {
            Color old = GUI.color;
            GUI.color = new Color(0.04f, 0.06f, 0.1f, 0.46f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = new Color(0.58f, 0.28f, 1f, 0.18f);
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
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            speakerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.82f, 0.28f) }
            };
            lineStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                wordWrap = true,
                normal = { textColor = new Color(0.9f, 0.95f, 1f) }
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
