using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class HeavyPressurePlate : MonoBehaviour
    {
        public GameObject[] activateOnPress = Array.Empty<GameObject>();
        public GameObject[] deactivateOnPress = Array.Empty<GameObject>();
        public string message = "Plaque activee";
        private bool pressed;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null || pressed)
            {
                return;
            }

            if (!player.CurrentDefinition.canPushHeavy)
            {
                player.ShowFeedback("Trop leger : Pierre");
                return;
            }

            pressed = true;
            for (int i = 0; i < activateOnPress.Length; i++)
            {
                if (activateOnPress[i] != null)
                {
                    activateOnPress[i].SetActive(true);
                }
            }

            for (int i = 0; i < deactivateOnPress.Length; i++)
            {
                if (deactivateOnPress[i] != null)
                {
                    deactivateOnPress[i].SetActive(false);
                }
            }

            player.ShowFeedback(message);
        }
    }
}
