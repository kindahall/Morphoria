using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
    public sealed class MorphoriaHubState : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle panelStyle;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void OnGUI()
        {
            EnsureStyles();
            MorphoriaSaveData data = session.SaveData;
            Rect panel = new Rect(18f, Screen.height - 158f, 390f, 136f);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 12f, panel.width - 36f, 28f), "Village d'Ecloria", titleStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 46f, panel.width - 36f, 24f), "Villageois sauves  " + data.totalVillagersSaved, labelStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 72f, panel.width - 36f, 24f), "Etoiles  " + data.totalGoldenStars + "    Prismes  " + data.totalPrismStars, labelStyle);

            Rect buttons = new Rect(panel.x + 18f, panel.y + 98f, panel.width - 36f, 28f);
            GUILayout.BeginArea(buttons);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Carte"))
            {
                session.LoadWorldMap();
            }
            if (GUILayout.Button("Menu"))
            {
                session.LoadMainMenu();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = new Color(0.92f, 0.96f, 1f) }
            };
            panelStyle = new GUIStyle(GUI.skin.box);
        }
    }
}
