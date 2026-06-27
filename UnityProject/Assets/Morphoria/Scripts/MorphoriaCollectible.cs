using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaCollectible : MonoBehaviour
    {
        public string persistentId;
        public CollectibleKind kind = CollectibleKind.GoldenStar;
        public int amount = 1;
        public float bobHeight = 0.18f;
        public float spinSpeed = 90f;

        private Vector3 startPosition;
        private bool collected;

        private void Awake()
        {
            startPosition = transform.position;
            if (string.IsNullOrEmpty(persistentId))
            {
                persistentId = gameObject.name;
            }
        }

        private void Start()
        {
            MorphoriaGameSession session = MorphoriaGameSession.GetOrCreate();
            if (!session.HasCollectedInActiveLevel(persistentId, kind))
            {
                return;
            }

            MorphoriaPlayer player = FindAnyObjectByType<MorphoriaPlayer>();
            if (player != null)
            {
                player.Inventory.SeedCollected(kind, amount);
            }

            collected = true;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * 2.5f) * bobHeight);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (collected)
            {
                return;
            }

            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            collected = true;
            player.Inventory.AddCollectible(kind, amount);
            MorphoriaGameSession.GetOrCreate().RecordCollectedInActiveLevel(persistentId, kind);
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
