using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public enum MorphoriaForm
    {
        Stone,
        Leaf,
        Paper,
        Scissors
    }

    public enum MorphoriaAbility
    {
        Any,
        Break,
        Glide,
        Fold,
        Cut,
        PushHeavy,
        ResistWind
    }

    public enum CollectibleKind
    {
        GoldenStar,
        ChoiceStar,
        PrismStar
    }

    [Serializable]
    public sealed class FormDefinition
    {
        public MorphoriaForm form;
        public string heroName;
        public string displayName;
        public string shortName;
        public Color color;
        public Color accent;
        public float speed;
        public float runSpeed;
        public float acceleration;
        public float jumpHeight;
        public float gravity;
        public float mass;
        public bool canBreak;
        public bool canGlide;
        public bool canFold;
        public bool canCut;
        public bool canPushHeavy;
        public bool canResistWind;
    }

    public static class FormCatalog
    {
        private static readonly FormDefinition Stone = new FormDefinition
        {
            form = MorphoriaForm.Stone,
            heroName = "Rokko",
            displayName = "Pierre",
            shortName = "R",
            color = new Color(0.64f, 0.43f, 0.24f),
            accent = new Color(1.0f, 0.62f, 0.12f),
            speed = 4.1f,
            runSpeed = 5.2f,
            acceleration = 16f,
            jumpHeight = 1.55f,
            gravity = -32f,
            mass = 10f,
            canBreak = true,
            canPushHeavy = true,
            canResistWind = true
        };

        private static readonly FormDefinition Leaf = new FormDefinition
        {
            form = MorphoriaForm.Leaf,
            heroName = "Luma",
            displayName = "Feuille",
            shortName = "L",
            color = new Color(0.22f, 0.72f, 0.26f),
            accent = new Color(0.86f, 1.0f, 0.24f),
            speed = 6.2f,
            runSpeed = 7.2f,
            acceleration = 18f,
            jumpHeight = 2.25f,
            gravity = -22f,
            mass = 2f,
            canGlide = true
        };

        private static readonly FormDefinition Paper = new FormDefinition
        {
            form = MorphoriaForm.Paper,
            heroName = "Papyra",
            displayName = "Papier",
            shortName = "P",
            color = new Color(0.88f, 0.82f, 1.0f),
            accent = new Color(0.55f, 0.32f, 0.94f),
            speed = 5.4f,
            runSpeed = 6.2f,
            acceleration = 15f,
            jumpHeight = 1.95f,
            gravity = -24f,
            mass = 1.4f,
            canFold = true
        };

        private static readonly FormDefinition Scissors = new FormDefinition
        {
            form = MorphoriaForm.Scissors,
            heroName = "Cizo",
            displayName = "Ciseaux",
            shortName = "C",
            color = new Color(0.58f, 0.75f, 0.92f),
            accent = new Color(0.12f, 0.68f, 1.0f),
            speed = 7.0f,
            runSpeed = 8.8f,
            acceleration = 24f,
            jumpHeight = 1.8f,
            gravity = -28f,
            mass = 4f,
            canCut = true
        };

        public static IReadOnlyList<FormDefinition> All { get; } = new[] { Stone, Leaf, Paper, Scissors };

        public static FormDefinition Get(MorphoriaForm form)
        {
            switch (form)
            {
                case MorphoriaForm.Stone:
                    return Stone;
                case MorphoriaForm.Leaf:
                    return Leaf;
                case MorphoriaForm.Paper:
                    return Paper;
                case MorphoriaForm.Scissors:
                    return Scissors;
                default:
                    return Stone;
            }
        }

        public static bool HasAbility(FormDefinition form, MorphoriaAbility ability)
        {
            switch (ability)
            {
                case MorphoriaAbility.Any:
                    return true;
                case MorphoriaAbility.Break:
                    return form.canBreak;
                case MorphoriaAbility.Glide:
                    return form.canGlide;
                case MorphoriaAbility.Fold:
                    return form.canFold;
                case MorphoriaAbility.Cut:
                    return form.canCut;
                case MorphoriaAbility.PushHeavy:
                    return form.canPushHeavy;
                case MorphoriaAbility.ResistWind:
                    return form.canResistWind;
                default:
                    return false;
            }
        }

        public static string AbilityLabel(MorphoriaAbility ability)
        {
            switch (ability)
            {
                case MorphoriaAbility.Break:
                    return "Pierre";
                case MorphoriaAbility.Glide:
                    return "Feuille";
                case MorphoriaAbility.Fold:
                    return "Papier";
                case MorphoriaAbility.Cut:
                    return "Ciseaux";
                case MorphoriaAbility.PushHeavy:
                    return "Pierre";
                case MorphoriaAbility.ResistWind:
                    return "Pierre";
                default:
                    return "Equipe";
            }
        }
    }

    public interface IFormInteractable
    {
        bool CanInteract(FormDefinition form);
        string Hint(FormDefinition form);
        void Interact(MorphoriaPlayer player);
    }
}
