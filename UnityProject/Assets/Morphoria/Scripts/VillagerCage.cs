using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class VillagerCage : MonoBehaviour, IFormInteractable
    {
        public string persistentId;
        public MorphoriaAbility requiredAbility = MorphoriaAbility.Any;
        public bool requiresBossDefeated = true;
        public MiniBoss boss;
        public GameObject cageVisual;
        public GameObject villagerVisual;
        private bool opened;

        public bool IsOpened => opened;

        private void Awake()
        {
            if (string.IsNullOrEmpty(persistentId))
            {
                persistentId = gameObject.name;
            }
        }

        private void Start()
        {
            MorphoriaGameSession session = MorphoriaGameSession.GetOrCreate();
            if (!session.HasRescuedVillagerInActiveLevel(persistentId))
            {
                return;
            }

            ApplyOpenedVisuals();
            MorphoriaPlayer player = FindAnyObjectByType<MorphoriaPlayer>();
            if (player != null)
            {
                player.Inventory.SeedVillagerSaved();
            }
        }

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
            ApplyOpenedVisuals();
            player.Inventory.SaveVillager();
            MorphoriaGameSession.GetOrCreate().RecordRescuedVillagerInActiveLevel(persistentId);
            player.ShowFeedback("Villageois libere");
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.VillagerSaved, transform.position + Vector3.up, Color.cyan, 0.95f);
        }

        private void ApplyOpenedVisuals()
        {
            opened = true;
            if (cageVisual != null)
            {
                cageVisual.SetActive(false);
            }

            if (villagerVisual != null)
            {
                villagerVisual.SetActive(true);
            }
        }
    }
}
