using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class VillagerCage : MonoBehaviour, IFormInteractable
    {
        public MorphoriaAbility requiredAbility = MorphoriaAbility.Any;
        public bool requiresBossDefeated = true;
        public MiniBoss boss;
        public GameObject cageVisual;
        public GameObject villagerVisual;
        private bool opened;

        public bool CanInteract(FormDefinition form)
        {
            if (opened)
            {
                return false;
            }

            if (requiresBossDefeated && boss != null && !boss.IsDefeated)
            {
                return false;
            }

            return FormCatalog.HasAbility(form, requiredAbility);
        }

        public string Hint(FormDefinition form)
        {
            if (requiresBossDefeated && boss != null && !boss.IsDefeated)
            {
                return "Battez le Garde-Cage";
            }

            return "Cage : " + FormCatalog.AbilityLabel(requiredAbility);
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (!CanInteract(player.CurrentDefinition))
            {
                player.ShowFeedback(Hint(player.CurrentDefinition));
                return;
            }

            opened = true;
            if (cageVisual != null)
            {
                cageVisual.SetActive(false);
            }

            if (villagerVisual != null)
            {
                villagerVisual.SetActive(true);
            }

            player.Inventory.SaveVillager();
            player.ShowFeedback("Villageois libere");
        }
    }
}
