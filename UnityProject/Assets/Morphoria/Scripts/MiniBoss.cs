using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class MiniBoss : MonoBehaviour, IFormInteractable
    {
        public int maxHealth = 4;
        public float moveSpeed = 2.5f;
        public float chargeDistance = 8f;
        public Transform[] patrolPoints;
        public Renderer[] renderers;
        public MorphoriaAbility[] weaknessSequence;

        private int health;
        private int sequenceIndex;
        private int patrolIndex;
        private MorphoriaPlayer target;
        private float attackCooldown;
        private bool defeated;

        public int Health => health;
        public bool IsDefeated => defeated;
        public MorphoriaAbility CurrentWeakness => CurrentRequiredAbility();
        public bool UsesWeaknessSequence => HasWeaknessSequence();

        private void Awake()
        {
            if (weaknessSequence != null && weaknessSequence.Length > 0)
            {
                maxHealth = weaknessSequence.Length;
            }

            health = maxHealth;
        }

        private void Update()
        {
            if (defeated)
            {
                return;
            }

            if (target == null)
            {
                target = FindAnyObjectByType<MorphoriaPlayer>();
            }

            if (target == null)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < chargeDistance)
            {
                Vector3 direction = target.transform.position - transform.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.05f)
                {
                    transform.position += direction.normalized * (moveSpeed * 1.4f * Time.deltaTime);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
                }
            }
            else
            {
                Patrol();
            }

            attackCooldown -= Time.deltaTime;
        }

        private void Patrol()
        {
            if (patrolPoints == null || patrolPoints.Length == 0 || patrolPoints[patrolIndex] == null)
            {
                transform.Rotate(Vector3.up, 35f * Time.deltaTime);
                return;
            }

            Transform point = patrolPoints[patrolIndex];
            Vector3 direction = point.position - transform.position;
            direction.y = 0f;
            if (direction.magnitude < 0.5f)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                return;
            }

            transform.position += direction.normalized * (moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 6f * Time.deltaTime);
        }

        private void OnTriggerStay(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null || attackCooldown > 0f || defeated)
            {
                return;
            }

            player.Inventory.Damage(1);
            player.AddExternalVelocity((player.transform.position - transform.position).normalized * 8f + Vector3.up * 4f);
            player.ShowFeedback("Garde-Cage");
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Damage, player.transform.position + Vector3.up, Color.red, 0.72f);
            attackCooldown = 1.4f;
        }

        public bool CanInteract(FormDefinition form)
        {
            if (defeated)
            {
                return false;
            }

            if (!HasWeaknessSequence())
            {
                return form.canBreak || form.canCut;
            }

            return FormCatalog.HasAbility(form, CurrentRequiredAbility());
        }

        public string Hint(FormDefinition form)
        {
            if (!HasWeaknessSequence())
            {
                return "Pierre ou Ciseaux";
            }

            return FormCatalog.AbilityLabel(CurrentRequiredAbility());
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (!CanInteract(player.CurrentDefinition))
            {
                player.ShowFeedback(Hint(player.CurrentDefinition));
                return;
            }

            MorphoriaAbility resolvedWeakness = HasWeaknessSequence()
                ? CurrentRequiredAbility()
                : player.CurrentDefinition.canBreak ? MorphoriaAbility.Break : MorphoriaAbility.Cut;
            health--;
            if (HasWeaknessSequence())
            {
                sequenceIndex++;
            }
            player.ShowFeedback(HasWeaknessSequence() ? "Noctar : " + FormCatalog.AbilityLabel(resolvedWeakness) : "Garde-Cage");
            Pulse(player.CurrentDefinition.accent);
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.BossHit, transform.position + Vector3.up, player.CurrentDefinition.accent, 0.86f);

            if (health <= 0)
            {
                defeated = true;
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.BossDefeated, transform.position + Vector3.up, new Color(0.78f, 0.42f, 1f), 1f);
                gameObject.SetActive(false);
                player.ShowFeedback("Garde-Cage vaincu");
            }
        }

        private MorphoriaAbility CurrentRequiredAbility()
        {
            if (!HasWeaknessSequence())
            {
                return MorphoriaAbility.Break;
            }

            int index = Mathf.Clamp(sequenceIndex, 0, weaknessSequence.Length - 1);
            return weaknessSequence[index];
        }

        private bool HasWeaknessSequence()
        {
            return weaknessSequence != null && weaknessSequence.Length > 0;
        }

        private void Pulse(Color color)
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].material.color = color;
                }
            }
        }
    }
}
