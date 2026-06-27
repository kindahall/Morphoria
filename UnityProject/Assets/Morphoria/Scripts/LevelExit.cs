using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class LevelExit : MonoBehaviour
    {
        public int requiredVillagers = 4;
        private bool completing;

        private void OnTriggerEnter(Collider other)
        {
            if (completing)
            {
                return;
            }

            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            if (player.Inventory.VillagersSaved < requiredVillagers)
            {
                int missing = Mathf.Max(0, requiredVillagers - player.Inventory.VillagersSaved);
                player.ShowFeedback(missing == 1 ? "1 villageois restant" : missing + " villageois restants");
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Denied, transform.position + Vector3.up, Color.red, 0.45f);
                return;
            }

            completing = true;
            player.ShowFeedback("Niveau termine");
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.LevelComplete, transform.position + Vector3.up * 1.5f, Color.cyan, 1f);
            MorphoriaGameSession session = MorphoriaGameSession.GetOrCreate();
            MorphoriaLevelClearResult result = session.MarkCurrentLevelComplete(player.Inventory.GoldenStars, player.Inventory.PrismObjectivesCollected, player.Inventory.VillagersSaved);
            MorphoriaLevelResultScreen resultScreen = FindAnyObjectByType<MorphoriaLevelResultScreen>();
            if (resultScreen != null)
            {
                resultScreen.Show(result);
            }
            else
            {
                session.LoadSceneAfterDelay(MorphoriaGameContent.HubScene, 1.35f);
            }
        }
    }
}
