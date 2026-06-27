using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class WindZone : MonoBehaviour
    {
        public Vector3 windVelocity = new Vector3(0f, 10f, 3f);
        public Vector3 wrongFormPush = new Vector3(0f, 0f, -4f);

        private void OnTriggerStay(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            if (player.CurrentDefinition.canGlide)
            {
                player.AddExternalVelocity(windVelocity * Time.deltaTime);
                player.ShowFeedback("Courant porteur");
            }
            else if (!player.CurrentDefinition.canResistWind)
            {
                player.AddExternalVelocity(wrongFormPush * Time.deltaTime);
            }
        }
    }
}
