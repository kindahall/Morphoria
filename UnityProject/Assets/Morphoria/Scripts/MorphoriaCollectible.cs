using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaCollectible : MonoBehaviour
    {
        public CollectibleKind kind = CollectibleKind.GoldenStar;
        public int amount = 1;
        public float bobHeight = 0.18f;
        public float spinSpeed = 90f;

        private Vector3 startPosition;

        private void Awake()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * 2.5f) * bobHeight);
        }

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            player.Inventory.AddCollectible(kind, amount);
            player.ShowFeedback(Feedback());
            MorphoriaFeedbackSystem.GetOrCreate().PlayCollectible(kind, transform.position);
            gameObject.SetActive(false);
        }

        private string Feedback()
        {
            switch (kind)
            {
                case CollectibleKind.ChoiceStar:
                    return "Etoile prismatique";
                case CollectibleKind.PrismStar:
                    return "Fragment prismatique";
                default:
                    return "Etoile";
            }
        }
    }
}
