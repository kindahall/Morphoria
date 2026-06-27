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

        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle smallStyle;
        private GUIStyle promptStyle;

        private void OnGUI()
        {
            if (player == null)
            {
                return;
            }

            EnsureStyles();
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
            GUI.Label(new Rect(Screen.width - 300f, 62f, 260f, 26f), objective, labelStyle);

            string feedback = player.FeedbackText;
            if (!string.IsNullOrEmpty(feedback))
            {
                Rect rect = new Rect(Screen.width * 0.5f - 180f, 28f, 360f, 48f);
                DrawPanel(rect, form.accent);
                GUI.Label(new Rect(rect.x + 18f, rect.y + 13f, rect.width - 36f, 24f), feedback, titleStyle);
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
                GUI.Label(new Rect(Screen.width * 0.5f - 150f, Screen.height - 60f, 300f, 24f), "Garde-Cage  " + miniBoss.Health + " / " + miniBoss.maxHealth, labelStyle);
            }

            DrawCompactWheel(form);

            if (player.IsWheelOpen)
            {
                DrawLargeWheel(player.WheelSelection);
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
        }

        private static string Hearts(int count)
        {
            return new string('*', Mathf.Max(0, count));
        }
    }
}
