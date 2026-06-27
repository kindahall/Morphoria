using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaHubRestoration : MonoBehaviour
    {
        public GameObject[] damagedStage;
        public GameObject[] repairedStage;
        public GameObject[] gardenStage;
        public GameObject[] finaleStage;
        public Light heartLight;

        private MorphoriaGameSession session;

        public int Stage { get; private set; }

        private void Awake()
        {
            session = MorphoriaGameSession.GetOrCreate();
        }

        private void OnEnable()
        {
            if (session == null)
            {
                session = MorphoriaGameSession.GetOrCreate();
            }

            if (session != null)
            {
                session.SaveChanged += HandleSaveChanged;
                Apply(session.SaveData);
            }
        }

        private void OnDisable()
        {
            if (session != null)
            {
                session.SaveChanged -= HandleSaveChanged;
            }
        }

        public static int CalculateStage(MorphoriaSaveData data)
        {
            if (data == null)
            {
                return 0;
            }

            if (data.finalBossDefeated)
            {
                return 3;
            }

            if (data.totalVillagersSaved >= 8 || data.totalPrismStars >= 12)
            {
                return 2;
            }

            if (data.totalVillagersSaved > 0 || data.totalGoldenStars >= 30)
            {
                return 1;
            }

            return 0;
        }

        public static string StageLabel(int stage)
        {
            switch (Mathf.Clamp(stage, 0, 3))
            {
                case 1:
                    return "Village en reconstruction";
                case 2:
                    return "Village rayonnant";
                case 3:
                    return "Ecloria restauree";
                default:
                    return "Village fragile";
            }
        }

        private void HandleSaveChanged(MorphoriaSaveData data)
        {
            Apply(data);
        }

        private void Apply(MorphoriaSaveData data)
        {
            Stage = CalculateStage(data);
            SetActive(damagedStage, Stage == 0);
            SetActive(repairedStage, Stage >= 1);
            SetActive(gardenStage, Stage >= 2);
            SetActive(finaleStage, Stage >= 3);

            if (heartLight != null)
            {
                heartLight.intensity = Mathf.Lerp(1.25f, 4.2f, Stage / 3f);
                heartLight.range = Mathf.Lerp(5f, 10f, Stage / 3f);
                heartLight.color = Stage >= 3 ? new Color(0.95f, 0.86f, 1f) : new Color(0.25f, 0.78f, 1f);
            }
        }

        private static void SetActive(GameObject[] objects, bool active)
        {
            if (objects == null)
            {
                return;
            }

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].SetActive(active);
                }
            }
        }
    }
}
