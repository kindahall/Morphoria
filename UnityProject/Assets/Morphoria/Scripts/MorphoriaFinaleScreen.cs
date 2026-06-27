using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaFinaleScreen : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();

            float width = Mathf.Min(640f, Screen.width - 48f);
            float height = 410f;
            Rect panel = new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height * 0.5f - height * 0.5f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            MorphoriaSaveData data = session.SaveData;
            GUI.Label(new Rect(panel.x + 38f, panel.y + 30f, panel.width - 76f, 42f), "Ecloria liberee", titleStyle);
            GUI.Label(new Rect(panel.x + 38f, panel.y + 82f, panel.width - 76f, 28f), "Noctar est vaincu. Les quatre formes veillent a nouveau sur Morphoria.", labelStyle);
            GUI.Label(new Rect(panel.x + 38f, panel.y + 140f, panel.width - 76f, 26f), "Etoiles  " + data.totalGoldenStars, labelStyle);
            GUI.Label(new Rect(panel.x + 38f, panel.y + 174f, panel.width - 76f, 26f), "Prismes  " + data.totalPrismStars, labelStyle);
            GUI.Label(new Rect(panel.x + 38f, panel.y + 208f, panel.width - 76f, 26f), "Villageois sauves  " + data.totalVillagersSaved, labelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 38f, panel.yMax - 78f, panel.width - 76f, 44f));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Village", buttonStyle, GUILayout.Height(40f)))
            {
                session.LoadHub();
            }

            if (GUILayout.Button("Carte", buttonStyle, GUILayout.Height(40f)))
            {
                session.LoadWorldMap();
            }

            if (GUILayout.Button("Menu", buttonStyle, GUILayout.Height(40f)))
            {
                session.LoadMainMenu();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawBackdrop()
        {
            Color old = GUI.color;
            GUI.color = new Color(0.04f, 0.06f, 0.1f, 1f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = new Color(0.34f, 0.86f, 1f, 0.18f);
            GUI.DrawTexture(new Rect(0f, Screen.height * 0.54f, Screen.width, Screen.height * 0.46f), Texture2D.whiteTexture);
            GUI.color = old;
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
                fontSize = 34,
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
