using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class PlayerInventory : MonoBehaviour
    {
        public int startingHearts = 4;
        public int startingChoiceStars = 5;

        public int Hearts { get; private set; }
        public int GoldenStars { get; private set; }
        public int ChoiceStars { get; private set; }
        public int ChoiceStarsCollected { get; private set; }
        public int PrismStars { get; private set; }
        public int PrismObjectivesCollected => ChoiceStarsCollected + PrismStars;
        public int VillagersSaved { get; private set; }
        public int MaxHearts => Mathf.Max(1, startingHearts);
        public bool IsDepleted => Hearts <= 0;

        public event Action Changed;

        private void Awake()
        {
            Hearts = MaxHearts;
            ChoiceStars = startingChoiceStars;
        }

        public void AddCollectible(CollectibleKind kind, int amount)
        {
            switch (kind)
            {
                case CollectibleKind.GoldenStar:
                    GoldenStars += amount;
                    break;
                case CollectibleKind.ChoiceStar:
                    ChoiceStars += amount;
                    ChoiceStarsCollected += amount;
                    break;
                case CollectibleKind.PrismStar:
                    PrismStars += amount;
                    break;
            }

            Changed?.Invoke();
        }

        public bool SpendChoiceStar()
        {
            if (ChoiceStars <= 0)
            {
                return false;
            }

            ChoiceStars--;
            Changed?.Invoke();
            return true;
        }

        public void SaveVillager()
        {
            VillagersSaved++;
            ChoiceStars++;
            Changed?.Invoke();
        }

        public bool Damage(int amount)
        {
            if (amount <= 0)
            {
                return IsDepleted;
            }

            Hearts = Mathf.Max(0, Hearts - amount);
            Changed?.Invoke();
            return IsDepleted;
        }

        public void RestoreHearts()
        {
            Hearts = MaxHearts;
            Changed?.Invoke();
        }
    }
}
