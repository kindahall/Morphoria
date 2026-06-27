using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class PortalFormZone : MonoBehaviour
    {
        public MorphoriaForm forcedForm = MorphoriaForm.Stone;
        public float timerSeconds = 10f;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player != null)
            {
                player.ForceForm(forcedForm, timerSeconds);
            }
        }
    }
}
