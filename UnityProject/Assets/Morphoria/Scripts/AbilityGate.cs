using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class AbilityGate : MonoBehaviour, IFormInteractable
    {
        public MorphoriaAbility requiredAbility = MorphoriaAbility.Any;
        public string successMessage = "Active";
        public bool destroyOnSuccess = true;
        public GameObject[] activateOnSuccess = Array.Empty<GameObject>();
        public GameObject[] deactivateOnSuccess = Array.Empty<GameObject>();

        private bool completed;

        public bool CanInteract(FormDefinition form)
        {
            return !completed && FormCatalog.HasAbility(form, requiredAbility);
        }

        public string Hint(FormDefinition form)
        {
            return "Forme requise : " + FormCatalog.AbilityLabel(requiredAbility);
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (completed)
            {
                return;
            }

            if (!CanInteract(player.CurrentDefinition))
            {
                player.ShowFeedback(Hint(player.CurrentDefinition));
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Denied, transform.position + Vector3.up, Color.red, 0.45f);
                return;
            }

            completed = true;

            for (int i = 0; i < activateOnSuccess.Length; i++)
            {
                if (activateOnSuccess[i] != null)
                {
                    activateOnSuccess[i].SetActive(true);
                }
            }

            for (int i = 0; i < deactivateOnSuccess.Length; i++)
            {
                if (deactivateOnSuccess[i] != null)
                {
                    deactivateOnSuccess[i].SetActive(false);
                }
            }

            player.ShowFeedback(successMessage);
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.AbilitySuccess, transform.position + Vector3.up, player.CurrentDefinition.accent, 0.82f);

            if (destroyOnSuccess)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
