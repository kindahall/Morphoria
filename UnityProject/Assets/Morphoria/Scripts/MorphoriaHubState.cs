using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
    public sealed class MorphoriaHubState : MonoBehaviour
    {
        private MorphoriaGameSession session;
        private MorphoriaHubRestoration restoration;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle panelStyle;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
            restoration = FindAnyObjectByType<MorphoriaHubRestoration>();
        }

        private void OnGUI()
        {
            EnsureStyles();
            MorphoriaSaveData data = session.SaveData;
            int stage = restoration != null ? restoration.Stage : MorphoriaHubRestoration.CalculateStage(data);
            Rect panel = new Rect(18f, Screen.height - 184f, 390f, 162f);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 12f, panel.width - 36f, 28f), "Village d'Ecloria", titleStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 42f, panel.width - 36f, 24f), MorphoriaHubRestoration.StageLabel(stage), labelStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 68f, panel.width - 36f, 24f), "Villageois sauves  " + data.totalVillagersSaved, labelStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 94f, panel.width - 36f, 24f), "Etoiles  " + data.totalGoldenStars + "    Prismes  " + data.totalPrismStars, labelStyle);

            Rect buttons = new Rect(panel.x + 18f, panel.y + 124f, panel.width - 36f, 28f);
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
