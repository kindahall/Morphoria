using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class BouncePad : MonoBehaviour
    {
        public float defaultBounce = 9f;
        public float leafBounce = 14f;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            float bounce = player.CurrentDefinition.canGlide ? leafBounce : defaultBounce;
            player.AddExternalVelocity(Vector3.up * bounce);
            player.ShowFeedback(player.CurrentDefinition.canGlide ? "Fleur Luma" : "Rebond");
        }
    }
}
