using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class FragilePlatform : MonoBehaviour
    {
        public float collapseDelay = 0.35f;
        private bool collapsing;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null || collapsing)
            {
                return;
            }

            if (player.CurrentForm == MorphoriaForm.Stone)
            {
                collapsing = true;
                player.ShowFeedback("Pont fragile");
                Invoke(nameof(Collapse), collapseDelay);
            }
        }

        private void Collapse()
        {
            gameObject.SetActive(false);
        }
    }
}
