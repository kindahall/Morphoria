using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaHud : MonoBehaviour
    {
        public MorphoriaPlayer player;
        public MiniBoss miniBoss;
        public string objective = "Liberez les villageois";
        public bool showLevelGoals = true;
        public int targetGoldenStars = 50;
        public int targetPrismStars = 5;
        public int targetVillagers = 4;

        private MorphoriaGameSession session;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle smallStyle;
        private GUIStyle promptStyle;
        private GUIStyle assistStyle;
        private Transform guideTarget;
        private string guideLabel = string.Empty;
        private float nextGuideRefresh;
        private int lastVillagers = -1;
        private int lastBossHealth = -1;
        private bool lastBossDefeated;
        private string objectivePulse = string.Empty;
        private float objectivePulseUntil;

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void Update()
        {
            TrackProgress();
        }

        private void OnGUI()
        {
            if (player == null)
            {
                return;
            }

            EnsureStyles();
            MorphoriaSaveData settings = session != null ? session.SaveData : null;
            bool subtitlesEnabled = settings == null || settings.subtitlesEnabled;
            bool colorAssistEnabled = settings != null && settings.colorAssist;
            PlayerInventory inventory = player.Inventory;
            FormDefinition form = player.CurrentDefinition;

            float panelHeight = showLevelGoals ? 154f : 102f;
            DrawPanel(new Rect(18f, 18f, 270f, panelHeight), form.accent);
            GUI.Label(new Rect(36f, 28f, 210f, 24f), form.heroName + " / " + form.displayName, titleStyle);
            GUI.Label(new Rect(36f, 58f, 210f, 24f), "Coeurs  " + Hearts(inventory.Hearts), labelStyle);

            if (showLevelGoals)
            {
                GUI.Label(new Rect(36f, 84f, 230f, 24f), "Etoiles  " + inventory.GoldenStars + " / " + targetGoldenStars, labelStyle);
                GUI.Label(new Rect(36f, 110f, 230f, 24f), "Prismes  " + inventory.PrismObjectivesCollected + " / " + targetPrismStars + "    Stock  " + inventory.ChoiceStars, labelStyle);
                GUI.Label(new Rect(36f, 136f, 230f, 24f), "Villageois  " + inventory.VillagersSaved + " / " + targetVillagers, labelStyle);
            }
            else
            {
                GUI.Label(new Rect(36f, 84f, 230f, 24f), "Prismes  " + inventory.ChoiceStars, labelStyle);
            }

            if (player.ForcedFormTimer > 0f)
            {
                float timerY = 30f + panelHeight;
                DrawPanel(new Rect(18f, timerY, 270f, 46f), form.accent);
                GUI.Label(new Rect(36f, timerY + 10f, 230f, 24f), "Timer  " + player.ForcedFormTimer.ToString("0.0") + " s", labelStyle);
            }

            DrawPanel(new Rect(Screen.width - 318f, 18f, 300f, 86f), form.accent);
            GUI.Label(new Rect(Screen.width - 300f, 30f, 260f, 26f), "Objectif", titleStyle);
            GUI.Label(new Rect(Screen.width - 300f, 62f, 260f, 26f), CurrentObjective(), labelStyle);
            DrawGuideMarker(form);

            string feedback = player.FeedbackText;
            if (subtitlesEnabled && !string.IsNullOrEmpty(feedback))
            {
                Rect rect = new Rect(Screen.width * 0.5f - 180f, 28f, 360f, 48f);
                DrawPanel(rect, form.accent);
                GUI.Label(new Rect(rect.x + 18f, rect.y + 13f, rect.width - 36f, 24f), feedback, titleStyle);
            }

            if (subtitlesEnabled)
            {
                DrawObjectivePulse(form, Mathf.Max(128f, Screen.height - 190f));
            }

            string prompt = player.InteractionPrompt;
            if (!string.IsNullOrEmpty(prompt))
            {
                Color promptColor = player.InteractionPromptReady ? form.accent : new Color(0.92f, 0.35f, 0.32f);
                Rect rect = new Rect(Screen.width * 0.5f - 180f, Screen.height - 128f, 360f, 44f);
                DrawPanel(rect, promptColor);
                GUI.Label(new Rect(rect.x + 18f, rect.y + 10f, rect.width - 36f, 24f), prompt, promptStyle);
            }

            if (miniBoss != null && !miniBoss.IsDefeated)
            {
                DrawPanel(new Rect(Screen.width * 0.5f - 170f, Screen.height - 70f, 340f, 42f), new Color(0.63f, 0.23f, 0.95f));
                string bossLabel = miniBoss.UsesWeaknessSequence
                    ? "Noctar  " + FormCatalog.AbilityLabel(miniBoss.CurrentWeakness) + "  " + miniBoss.Health + " / " + miniBoss.maxHealth
                    : "Garde-Cage  " + miniBoss.Health + " / " + miniBoss.maxHealth;
                GUI.Label(new Rect(Screen.width * 0.5f - 150f, Screen.height - 60f, 300f, 24f), bossLabel, labelStyle);
            }

            DrawCompactWheel(form);

            if (player.IsWheelOpen)
            {
                DrawLargeWheel(player.WheelSelection);
            }

            if (colorAssistEnabled && !player.IsWheelOpen)
            {
                DrawColorAssistMarkers();
            }
        }

        private void DrawCompactWheel(FormDefinition activeForm)
        {
            float size = 94f;
            Rect rect = new Rect(Screen.width - size - 28f, Screen.height - size - 24f, size, size);
            DrawPanel(rect, activeForm.accent);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 10f, rect.width - 24f, 22f), "Roue", smallStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 36f, rect.width - 24f, 22f), "1  2  3  4", labelStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 62f, rect.width - 24f, 22f), "Tab", labelStyle);
        }

        private void DrawLargeWheel(MorphoriaForm selection)
        {
            float radius = 142f;
            Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            DrawPanel(new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f), new Color(0.8f, 0.9f, 1f));
            DrawWheelSlot(center + new Vector2(0f, -92f), MorphoriaForm.Leaf, selection);
            DrawWheelSlot(center + new Vector2(-92f, 0f), MorphoriaForm.Stone, selection);
            DrawWheelSlot(center + new Vector2(92f, 0f), MorphoriaForm.Scissors, selection);
            DrawWheelSlot(center + new Vector2(0f, 92f), MorphoriaForm.Paper, selection);
            GUI.Label(new Rect(center.x - 50f, center.y - 14f, 100f, 28f), "Morphoria", titleStyle);
        }

        private void DrawWheelSlot(Vector2 center, MorphoriaForm formId, MorphoriaForm selection)
        {
            FormDefinition form = FormCatalog.Get(formId);
            Rect rect = new Rect(center.x - 48f, center.y - 26f, 96f, 52f);
            DrawPanel(rect, formId == selection ? form.accent : form.color);
            GUI.Label(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, 18f), form.heroName, smallStyle);
            GUI.Label(new Rect(rect.x + 8f, rect.y + 28f, rect.width - 16f, 18f), form.displayName, smallStyle);
        }

        private void DrawPanel(Rect rect, Color accent)
        {
            Color old = GUI.color;
            GUI.color = new Color(0.04f, 0.07f, 0.11f, 0.88f);
            GUI.Box(rect, GUIContent.none, panelStyle);
            GUI.color = new Color(accent.r, accent.g, accent.b, 0.95f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 3f), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void DrawObjectivePulse(FormDefinition form, float y)
        {
            if (string.IsNullOrEmpty(objectivePulse) || Time.unscaledTime > objectivePulseUntil)
            {
                return;
            }

            Rect rect = new Rect(Screen.width * 0.5f - 190f, y, 380f, 42f);
            DrawPanel(rect, form.accent);
            GUI.Label(new Rect(rect.x + 18f, rect.y + 9f, rect.width - 36f, 24f), objectivePulse, promptStyle);
        }

        private string CurrentObjective()
        {
            if (!showLevelGoals || player == null)
            {
                return objective;
            }

            if (targetVillagers > 0)
            {
                if (miniBoss != null && !miniBoss.IsDefeated)
                {
                    return miniBoss.UsesWeaknessSequence ? "Affrontez Noctar" : "Battez le Garde-Cage";
                }

                if (player.Inventory.VillagersSaved < targetVillagers)
                {
                    return "Liberez les villageois";
                }
            }

            return "Rejoignez le portail";
        }

        private void TrackProgress()
        {
            if (player == null || !showLevelGoals)
            {
                return;
            }

            PlayerInventory inventory = player.Inventory;
            if (lastVillagers < 0)
            {
                lastVillagers = inventory.VillagersSaved;
                lastBossHealth = miniBoss != null ? miniBoss.Health : -1;
                lastBossDefeated = miniBoss == null || miniBoss.IsDefeated;
                return;
            }

            if (miniBoss != null)
            {
                if (!lastBossDefeated && miniBoss.IsDefeated)
                {
                    AnnounceObjective(miniBoss.UsesWeaknessSequence ? "Noctar vaincu" : "Cages accessibles");
                }
                else if (!miniBoss.IsDefeated && lastBossHealth >= 0 && miniBoss.Health < lastBossHealth)
                {
                    AnnounceObjective(miniBoss.UsesWeaknessSequence ? "Rune suivante" : "Garde-Cage touche");
                }

                lastBossHealth = miniBoss.Health;
                lastBossDefeated = miniBoss.IsDefeated;
            }

            if (inventory.VillagersSaved != lastVillagers)
            {
                if (inventory.VillagersSaved >= targetVillagers)
                {
                    AnnounceObjective("Portail ouvert");
                }
                else
                {
                    AnnounceObjective("Villageois " + inventory.VillagersSaved + " / " + targetVillagers);
                }

                lastVillagers = inventory.VillagersSaved;
            }
        }

        private void AnnounceObjective(string text)
        {
            objectivePulse = text;
            objectivePulseUntil = Time.unscaledTime + 2.6f;
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = Texture2D.whiteTexture;
            panelStyle.border = new RectOffset(6, 6, 6, 6);

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = new Color(0.92f, 0.96f, 1f) },
                alignment = TextAnchor.MiddleLeft
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            promptStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            assistStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };
        }

        private void DrawColorAssistMarkers()
        {
            Camera camera = player.mainCamera != null ? player.mainCamera : Camera.main;
            if (camera == null)
            {
                return;
            }

            DrawGateMarkers(camera);
            DrawEnemyMarkers(camera);
            DrawCageMarkers(camera);
            DrawBossMarker(camera);
            DrawExitMarkers(camera);
            DrawPortalMarkers(camera);
        }

        private void DrawGateMarkers(Camera camera)
        {
            AbilityGate[] gates = FindObjectsByType<AbilityGate>();
            for (int i = 0; i < gates.Length; i++)
            {
                if (gates[i] != null && gates[i].gameObject.activeInHierarchy)
                {
                    DrawAssistMarker(camera, gates[i].transform.position + Vector3.up * 1.8f, FormCatalog.AbilityLabel(gates[i].requiredAbility), AbilityColor(gates[i].requiredAbility));
                }
            }
        }

        private void DrawEnemyMarkers(Camera camera)
        {
            MorphoriaEnemy[] enemies = FindObjectsByType<MorphoriaEnemy>();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null && enemies[i].gameObject.activeInHierarchy && !enemies[i].IsDefeated)
                {
                    DrawAssistMarker(camera, enemies[i].transform.position + Vector3.up * 1.9f, FormCatalog.AbilityLabel(enemies[i].weakness), AbilityColor(enemies[i].weakness));
                }
            }
        }

        private void DrawCageMarkers(Camera camera)
        {
            VillagerCage[] cages = FindObjectsByType<VillagerCage>();
            for (int i = 0; i < cages.Length; i++)
            {
                if (cages[i] != null && cages[i].gameObject.activeInHierarchy && !cages[i].IsOpened)
                {
                    DrawAssistMarker(camera, cages[i].transform.position + Vector3.up * 2.2f, "Cage " + FormCatalog.AbilityLabel(cages[i].requiredAbility), AbilityColor(cages[i].requiredAbility));
                }
            }
        }

        private void DrawBossMarker(Camera camera)
        {
            if (miniBoss == null || miniBoss.IsDefeated || !miniBoss.gameObject.activeInHierarchy)
            {
                return;
            }

            MorphoriaAbility ability = miniBoss.UsesWeaknessSequence ? miniBoss.CurrentWeakness : MorphoriaAbility.Break;
            string label = miniBoss.UsesWeaknessSequence ? "Noctar " + FormCatalog.AbilityLabel(ability) : "Boss Pierre/Ciseaux";
            DrawAssistMarker(camera, miniBoss.transform.position + Vector3.up * 3.2f, label, AbilityColor(ability));
        }

        private void DrawExitMarkers(Camera camera)
        {
            LevelExit[] exits = FindObjectsByType<LevelExit>();
            for (int i = 0; i < exits.Length; i++)
            {
                if (exits[i] == null || !exits[i].gameObject.activeInHierarchy)
                {
                    continue;
                }

                string label = player.Inventory.VillagersSaved >= exits[i].requiredVillagers
                    ? "Sortie"
                    : player.Inventory.VillagersSaved + " / " + exits[i].requiredVillagers;
                DrawAssistMarker(camera, exits[i].transform.position + Vector3.up * 2.1f, label, new Color(0.25f, 0.78f, 1f));
            }
        }

        private void DrawPortalMarkers(Camera camera)
        {
            if (showLevelGoals)
            {
                return;
            }

            MorphoriaScenePortal[] portals = FindObjectsByType<MorphoriaScenePortal>();
            for (int i = 0; i < portals.Length; i++)
            {
                if (portals[i] != null && portals[i].gameObject.activeInHierarchy)
                {
                    DrawAssistMarker(camera, portals[i].transform.position + Vector3.up * 2.1f, portals[i].label, new Color(0.25f, 0.78f, 1f));
                }
            }
        }

        private void DrawAssistMarker(Camera camera, Vector3 worldPosition, string label, Color color)
        {
            if (string.IsNullOrEmpty(label) || Vector3.Distance(player.transform.position, worldPosition) > 26f)
            {
                return;
            }

            Vector3 screen = camera.WorldToScreenPoint(worldPosition);
            if (screen.z <= 0f)
            {
                return;
            }

            float width = Mathf.Clamp(label.Length * 8f + 34f, 74f, 168f);
            Rect rect = new Rect(Mathf.Clamp(screen.x - width * 0.5f, 14f, Screen.width - width - 14f), Mathf.Clamp(Screen.height - screen.y - 18f, 112f, Screen.height - 156f), width, 30f);
            DrawPanel(rect, color);
            GUI.Label(new Rect(rect.x + 8f, rect.y + 6f, rect.width - 16f, 18f), label, assistStyle);
        }

        private static Color AbilityColor(MorphoriaAbility ability)
        {
            switch (ability)
            {
                case MorphoriaAbility.Break:
                case MorphoriaAbility.PushHeavy:
                case MorphoriaAbility.ResistWind:
                    return FormCatalog.Get(MorphoriaForm.Stone).accent;
                case MorphoriaAbility.Glide:
                    return FormCatalog.Get(MorphoriaForm.Leaf).accent;
                case MorphoriaAbility.Fold:
                    return FormCatalog.Get(MorphoriaForm.Paper).accent;
                case MorphoriaAbility.Cut:
                    return FormCatalog.Get(MorphoriaForm.Scissors).accent;
                default:
                    return Color.white;
            }
        }

        private void DrawGuideMarker(FormDefinition form)
        {
            RefreshGuideTarget();
            if (guideTarget == null || string.IsNullOrEmpty(guideLabel))
            {
                return;
            }

            Camera camera = player.mainCamera != null ? player.mainCamera : Camera.main;
            if (camera == null)
            {
                return;
            }

            Vector3 worldPosition = guideTarget.position + Vector3.up * 1.8f;
            Vector3 screen = camera.WorldToScreenPoint(worldPosition);
            if (screen.z < 0f)
            {
                screen.x = Screen.width - screen.x;
                screen.y = Screen.height - screen.y;
            }

            float margin = 48f;
            float x = Mathf.Clamp(screen.x, margin, Screen.width - margin);
            float y = Mathf.Clamp(Screen.height - screen.y, margin, Screen.height - margin);
            float distance = Vector3.Distance(player.transform.position, guideTarget.position);
            Rect rect = new Rect(x - 74f, y - 18f, 148f, 36f);
            DrawPanel(rect, form.accent);
            GUI.Label(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, 20f), guideLabel + "  " + Mathf.RoundToInt(distance) + "m", smallStyle);
        }

        private void RefreshGuideTarget()
        {
            if (Time.unscaledTime < nextGuideRefresh && guideTarget != null)
            {
                return;
            }

            nextGuideRefresh = Time.unscaledTime + 0.25f;
            guideTarget = null;
            guideLabel = string.Empty;

            if (showLevelGoals)
            {
                if (miniBoss != null && !miniBoss.IsDefeated && targetVillagers > 0)
                {
                    guideTarget = miniBoss.transform;
                    guideLabel = "Garde-Cage";
                    return;
                }

                if (player.Inventory.VillagersSaved < targetVillagers && TryFindNearestCage(out VillagerCage cage))
                {
                    guideTarget = cage.transform;
                    guideLabel = "Cage";
                    return;
                }

                if (TryFindNearestExit(out LevelExit exit))
                {
                    guideTarget = exit.transform;
                    guideLabel = "Sortie";
                }

                return;
            }

            if (TryFindNearestPortal(out MorphoriaScenePortal portal))
            {
                guideTarget = portal.transform;
                guideLabel = string.IsNullOrEmpty(portal.label) ? "Portail" : portal.label;
            }
        }

        private bool TryFindNearestCage(out VillagerCage nearest)
        {
            VillagerCage[] cages = FindObjectsByType<VillagerCage>();
            nearest = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < cages.Length; i++)
            {
                if (cages[i] == null || cages[i].IsOpened)
                {
                    continue;
                }

                float distance = Vector3.Distance(player.transform.position, cages[i].transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = cages[i];
                }
            }

            return nearest != null;
        }

        private bool TryFindNearestExit(out LevelExit nearest)
        {
            LevelExit[] exits = FindObjectsByType<LevelExit>();
            nearest = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < exits.Length; i++)
            {
                if (exits[i] == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(player.transform.position, exits[i].transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = exits[i];
                }
            }

            return nearest != null;
        }

        private bool TryFindNearestPortal(out MorphoriaScenePortal nearest)
        {
            MorphoriaScenePortal[] portals = FindObjectsByType<MorphoriaScenePortal>();
            nearest = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < portals.Length; i++)
            {
                if (portals[i] == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(player.transform.position, portals[i].transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = portals[i];
                }
            }

            return nearest != null;
        }

        private static string Hearts(int count)
        {
            return new string('*', Mathf.Max(0, count));
        }
    }
}
