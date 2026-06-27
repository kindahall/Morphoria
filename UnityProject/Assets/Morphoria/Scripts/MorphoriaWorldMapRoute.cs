using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaWorldMapRoute : MonoBehaviour
    {
        public string fromLevelId;
        public string toLevelId;
        public GameObject lockedVisual;
        public GameObject unlockedVisual;
        public Light routeLight;

        private MorphoriaGameSession session;

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

        private void HandleSaveChanged(MorphoriaSaveData data)
        {
            Apply(data);
        }

        private void Apply(MorphoriaSaveData data)
        {
            if (data == null || string.IsNullOrEmpty(fromLevelId) || string.IsNullOrEmpty(toLevelId))
            {
                return;
            }

            MorphoriaLevelProgress from = MorphoriaSaveSystem.GetProgress(data, fromLevelId);
            MorphoriaLevelProgress to = MorphoriaSaveSystem.GetProgress(data, toLevelId);
            bool open = from.completed || to.unlocked;
            SetActive(lockedVisual, !open);
            SetActive(unlockedVisual, open);

            if (routeLight != null)
            {
                routeLight.enabled = open;
                routeLight.intensity = open ? 0.9f : 0f;
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
