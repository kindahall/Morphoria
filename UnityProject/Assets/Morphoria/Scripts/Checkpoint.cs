using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class Checkpoint : MonoBehaviour
    {
        private bool reached;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null || reached)
            {
                return;
            }

            reached = true;
            player.SetCheckpoint(transform);
        }
    }
}
