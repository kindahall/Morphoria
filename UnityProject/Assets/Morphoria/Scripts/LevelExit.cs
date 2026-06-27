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
                player.ShowFeedback("Villageois restants");
                return;
            }

            completing = true;
            player.ShowFeedback("Niveau termine");
            MorphoriaGameSession session = MorphoriaGameSession.GetOrCreate();
            session.MarkCurrentLevelComplete(player.Inventory.GoldenStars, player.Inventory.PrismObjectivesCollected, player.Inventory.VillagersSaved);
            session.LoadSceneAfterDelay(MorphoriaGameContent.HubScene, 1.35f);
        }
    }
}
