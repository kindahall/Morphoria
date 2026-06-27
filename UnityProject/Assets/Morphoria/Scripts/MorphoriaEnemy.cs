using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaEnemy : MonoBehaviour, IFormInteractable
    {
        public string displayName = "Ennemi";
        public MorphoriaAbility weakness = MorphoriaAbility.Break;
        public float moveSpeed = 1.8f;
        public float patrolDistance = 2.4f;
        public float contactRadius = 1.35f;
        public float hoverAmplitude = 0.08f;
        public Vector3 patrolAxis = Vector3.right;
        public Renderer[] renderers;

        private Vector3 origin;
        private Vector3 baseScale;
        private MorphoriaPlayer target;
        private MaterialPropertyBlock propertyBlock;
        private float phaseOffset;
        private float damageCooldown;
        private bool defeated;

        public bool IsDefeated => defeated;

        private void Awake()
        {
            origin = transform.position;
            baseScale = transform.localScale;
            phaseOffset = Mathf.Abs(transform.position.x * 0.37f + transform.position.z * 0.19f);
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

            Patrol();
            AnimatePresence();
            TryDamagePlayer();
            damageCooldown -= Time.deltaTime;
        }

        private void Patrol()
        {
            Vector3 axis = patrolAxis.sqrMagnitude > 0.01f ? patrolAxis.normalized : Vector3.right;
            float wave = Mathf.Sin(Time.time * Mathf.Max(0.1f, moveSpeed) * 0.6f + phaseOffset);
            Vector3 destination = origin + axis * (wave * patrolDistance);
            Vector3 flatDirection = destination - transform.position;
            flatDirection.y = 0f;

            if (flatDirection.sqrMagnitude > 0.01f)
            {
                transform.position += flatDirection.normalized * (moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatDirection.normalized, Vector3.up), 8f * Time.deltaTime);
            }

            Vector3 position = transform.position;
            position.y = origin.y + Mathf.Sin(Time.time * 2.8f + phaseOffset) * hoverAmplitude;
            transform.position = position;
        }

        private void AnimatePresence()
        {
            float pulse = 1f + Mathf.Sin(Time.time * 4.4f + phaseOffset) * 0.035f;
            transform.localScale = baseScale * pulse;

            if (renderers == null)
            {
                return;
            }

            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            float glow = 0.5f + Mathf.Sin(Time.time * 5f + phaseOffset) * 0.5f;
            Color weaknessColor = ColorFor(weakness);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].GetPropertyBlock(propertyBlock);
                    propertyBlock.SetColor("_EmissionColor", weaknessColor * Mathf.Lerp(0.15f, 0.55f, glow));
                    renderers[i].SetPropertyBlock(propertyBlock);
                }
            }
        }

        private void TryDamagePlayer()
        {
            if (target == null || damageCooldown > 0f)
            {
                return;
            }

            Vector3 delta = target.transform.position - transform.position;
            delta.y = 0f;
            if (delta.magnitude > contactRadius)
            {
                return;
            }

            target.Inventory.Damage(1);
            target.AddExternalVelocity(delta.normalized * 7f + Vector3.up * 3f);
            target.ShowFeedback(displayName);
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Damage, target.transform.position + Vector3.up, Color.red, 0.62f);
            damageCooldown = 1.25f;
        }

        public bool CanInteract(FormDefinition form)
        {
            return !defeated && FormCatalog.HasAbility(form, weakness);
        }

        public string Hint(FormDefinition form)
        {
            return displayName + " : " + FormCatalog.AbilityLabel(weakness);
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (!CanInteract(player.CurrentDefinition))
            {
                player.ShowFeedback(Hint(player.CurrentDefinition));
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Denied, transform.position + Vector3.up, Color.red, 0.45f);
                return;
            }

            defeated = true;
            player.ShowFeedback(displayName + " vaincu");
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.BossHit, transform.position + Vector3.up, player.CurrentDefinition.accent, 0.72f);
            gameObject.SetActive(false);
        }

        private static Color ColorFor(MorphoriaAbility ability)
        {
            switch (ability)
            {
                case MorphoriaAbility.Break:
                case MorphoriaAbility.PushHeavy:
                case MorphoriaAbility.ResistWind:
                    return FormCatalog.Get(MorphoriaForm.Stone).accent;
                case MorphoriaAbility.Glide:
                    return FormCatalog.Get(MorphoriaForm.Leaf).accent;
                case MorphoriaAbility.Fold:
                    return FormCatalog.Get(MorphoriaForm.Paper).accent;
                case MorphoriaAbility.Cut:
                    return FormCatalog.Get(MorphoriaForm.Scissors).accent;
                default:
                    return Color.white;
            }
        }
    }
}
