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
        private GUIStyle summaryStyle;
        private GUIStyle statStyle;
        private GUIStyle warningStyle;
        private bool showSettings;
        private bool confirmNewGame;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();

            float panelWidth = Mathf.Min(540f, Screen.width - 48f);
            float panelHeight = Mathf.Min(showSettings ? 590f : 510f, Screen.height - 48f);
            Rect panel = new Rect(Screen.width * 0.5f - panelWidth * 0.5f, Mathf.Max(24f, Screen.height * 0.5f - panelHeight * 0.5f), panelWidth, panelHeight);
            GUI.Box(panel, GUIContent.none, panelStyle);
            GUI.Label(new Rect(panel.x + 38f, panel.y + 24f, panel.width - 76f, 58f), "Morphoria", titleStyle);

            GUILayout.BeginArea(new Rect(panel.x + 54f, panel.y + 92f, panel.width - 108f, panel.height - 118f));
            DrawSaveSummary();
            GUILayout.Space(12f);

            if (confirmNewGame)
            {
                GUILayout.Label("Nouvelle partie remplacera la sauvegarde actuelle.", warningStyle);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Confirmer", buttonStyle, GUILayout.Height(42f)))
                {
                    confirmNewGame = false;
                    session.NewGame();
                }

                if (GUILayout.Button("Annuler", buttonStyle, GUILayout.Height(42f)))
                {
                    confirmNewGame = false;
                }
                GUILayout.EndHorizontal();
            }
            else if (GUILayout.Button("Nouvelle partie", buttonStyle, GUILayout.Height(46f)))
            {
                if (session.CanContinue)
                {
                    confirmNewGame = true;
                }
                else
                {
                    session.NewGame();
                }
            }

            GUI.enabled = session.CanContinue;
            if (GUILayout.Button(ContinueButtonText(), buttonStyle, GUILayout.Height(46f)))
            {
                confirmNewGame = false;
                session.ContinueGame();
            }

            if (GUILayout.Button("Carte du monde", buttonStyle, GUILayout.Height(46f)))
            {
                confirmNewGame = false;
                session.LoadWorldMap();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Reglages", buttonStyle, GUILayout.Height(46f)))
            {
                confirmNewGame = false;
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
            GUI.color = new Color(0.58f, 0.28f, 1f, 0.18f);
            GUI.DrawTexture(new Rect(0f, Screen.height * 0.18f, Screen.width, 4f), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.78f, 0.13f, 0.16f);
            GUI.DrawTexture(new Rect(0f, Screen.height * 0.82f, Screen.width, 3f), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void DrawSaveSummary()
        {
            bool hasSave = session.CanContinue;
            MorphoriaSaveData data = session.SaveData;
            if (!hasSave || data == null)
            {
                GUILayout.Label("Aucune sauvegarde", summaryStyle);
                GUILayout.Label("Pret pour une nouvelle aventure", statStyle);
                return;
            }

            MorphoriaSaveSystem.Normalize(data);
            GUILayout.Label("Sauvegarde: " + CurrentLevelName(data), summaryStyle);
            GUILayout.Label(CompletedLevels(data) + " / " + MorphoriaGameContent.Levels.Length + " niveaux   " +
                data.totalGoldenStars + " / " + TotalGoldenTargets() + " etoiles   " +
                data.totalPrismStars + " / " + TotalPrismTargets() + " prismes   " +
                data.totalVillagersSaved + " / " + TotalVillagerTargets() + " villageois", statStyle);
        }

        private string ContinueButtonText()
        {
            if (!session.CanContinue || session.SaveData == null)
            {
                return "Continuer";
            }

            return "Continuer - " + CurrentLevelName(session.SaveData);
        }

        private static string CurrentLevelName(MorphoriaSaveData data)
        {
            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                if (MorphoriaGameContent.Levels[i].id == data.currentLevelId)
                {
                    return MorphoriaGameContent.Levels[i].displayName;
                }
            }

            return "Village d'Ecloria";
        }

        private static int CompletedLevels(MorphoriaSaveData data)
        {
            int completed = 0;
            for (int i = 0; i < data.levels.Count; i++)
            {
                if (data.levels[i].completed)
                {
                    completed++;
                }
            }

            return completed;
        }

        private static int TotalGoldenTargets()
        {
            int total = 0;
            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                total += MorphoriaGameContent.Levels[i].targetGoldenStars;
            }

            return total;
        }

        private static int TotalPrismTargets()
        {
            int total = 0;
            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                total += MorphoriaGameContent.Levels[i].targetPrismStars;
            }

            return total;
        }

        private static int TotalVillagerTargets()
        {
            int total = 0;
            for (int i = 0; i < MorphoriaGameContent.Levels.Length; i++)
            {
                total += MorphoriaGameContent.Levels[i].targetVillagers;
            }

            return total;
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
            summaryStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(0.86f, 0.94f, 1f) }
            };
            statStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(0.78f, 0.86f, 0.94f) }
            };
            warningStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(1f, 0.78f, 0.36f) }
            };
        }
    }
}
