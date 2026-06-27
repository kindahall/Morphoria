using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaWorldMapNode : MonoBehaviour
    {
        public string levelId;
        public GameObject lockedVisual;
        public GameObject unlockedVisual;
        public GameObject completedVisual;
        public Light glowLight;
        public TextMesh stateLabel;

        private MorphoriaGameSession session;
        private float baseIntensity;
        private Color baseColor = Color.white;
        private bool currentLevel;

        private void Start()
        {
            session = MorphoriaGameSession.GetOrCreate();
            session.SaveChanged += HandleSaveChanged;
            Apply(session.SaveData);
        }

        private void OnDestroy()
        {
            if (session != null)
            {
                session.SaveChanged -= HandleSaveChanged;
            }
        }

        private void Update()
        {
            if (glowLight == null || !glowLight.enabled || !currentLevel)
            {
                return;
            }

            glowLight.intensity = baseIntensity + Mathf.Sin(Time.time * 2.6f) * 0.35f;
        }

        private void HandleSaveChanged(MorphoriaSaveData data)
        {
            Apply(data);
        }

        private void Apply(MorphoriaSaveData data)
        {
            if (data == null || string.IsNullOrEmpty(levelId))
            {
                return;
            }

            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevel(levelId);
            MorphoriaWorldInfo world = MorphoriaGameContent.GetWorld(level.worldId);
            MorphoriaLevelProgress progress = MorphoriaSaveSystem.GetProgress(data, level.id);
            currentLevel = data.currentLevelId == level.id;

            bool completed = progress.completed;
            bool unlocked = progress.unlocked || completed;
            SetActive(lockedVisual, !unlocked);
            SetActive(unlockedVisual, unlocked && !completed);
            SetActive(completedVisual, completed);

            baseColor = completed ? new Color(1f, 0.82f, 0.22f) : unlocked ? world.color : new Color(0.24f, 0.28f, 0.34f);
            baseIntensity = completed ? 2.3f : unlocked ? 1.45f : 0.25f;

            if (glowLight != null)
            {
                glowLight.enabled = unlocked;
                glowLight.color = Color.Lerp(baseColor, Color.white, completed ? 0.3f : 0.15f);
                glowLight.range = completed ? 5.8f : 4.2f;
                glowLight.intensity = currentLevel ? baseIntensity + 0.35f : baseIntensity;
            }

            if (stateLabel != null)
            {
                stateLabel.text = completed ? "TERMINE" : unlocked ? "OUVERT" : "--";
                stateLabel.color = Color.Lerp(baseColor, Color.white, unlocked ? 0.35f : 0.08f);
            }
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active)
            {
                target.SetActive(active);
            }
        }
    }
}
