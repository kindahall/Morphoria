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

        private int health;
        private int patrolIndex;
        private MorphoriaPlayer target;
        private float attackCooldown;
        private bool defeated;

        public int Health => health;
        public bool IsDefeated => defeated;

        private void Awake()
        {
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
            return !defeated && (form.canBreak || form.canCut);
        }

        public string Hint(FormDefinition form)
        {
            return "Pierre ou Ciseaux";
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (!CanInteract(player.CurrentDefinition))
            {
                player.ShowFeedback(Hint(player.CurrentDefinition));
                return;
            }

            health--;
            player.ShowFeedback(player.CurrentDefinition.canBreak ? "Impact Rokko" : "Coupe Cizo");
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
