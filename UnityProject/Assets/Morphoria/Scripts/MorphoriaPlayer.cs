using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(MorphoriaAvatar))]
    public sealed class MorphoriaPlayer : MonoBehaviour
    {
        [Header("Camera")]
        public Camera mainCamera;

        [Header("Interaction")]
        public float interactionRadius = 2.4f;
        public LayerMask interactionMask = ~0;

        private CharacterController controller;
        private PlayerInventory inventory;
        private MorphoriaAvatar avatar;
        private MorphoriaForm currentForm = MorphoriaForm.Stone;
        private Vector3 currentHorizontalVelocity;
        private Vector3 externalVelocity;
        private float verticalVelocity;
        private float dashCooldown;
        private Vector3 checkpointPosition;
        private Quaternion checkpointRotation;
        private float lastFeedbackTime;
        private string feedbackText = string.Empty;
        private bool wheelOpen;
        private MorphoriaForm wheelSelection;
        private float forcedFormTimer;

        public event Action<MorphoriaForm> FormChanged;

        public MorphoriaForm CurrentForm => currentForm;
        public FormDefinition CurrentDefinition => FormCatalog.Get(currentForm);
        public PlayerInventory Inventory => inventory;
        public bool IsWheelOpen => wheelOpen;
        public MorphoriaForm WheelSelection => wheelSelection;
        public float ForcedFormTimer => Mathf.Max(0f, forcedFormTimer);
        public string FeedbackText => Time.time - lastFeedbackTime < 2.25f ? feedbackText : string.Empty;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            inventory = GetComponent<PlayerInventory>();
            avatar = GetComponent<MorphoriaAvatar>();
            checkpointPosition = transform.position;
            checkpointRotation = transform.rotation;

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            ApplyForm(currentForm, false);
        }

        private void Update()
        {
            if (forcedFormTimer > 0f)
            {
                forcedFormTimer = Mathf.Max(0f, forcedFormTimer - Time.deltaTime);
            }

            HandleFormInput();
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
            {
                TryInteract();
            }

            if (transform.position.y < -18f)
            {
                Respawn();
            }
        }

        private void HandleFormInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TrySwitchForm(MorphoriaForm.Stone);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TrySwitchForm(MorphoriaForm.Leaf);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TrySwitchForm(MorphoriaForm.Paper);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TrySwitchForm(MorphoriaForm.Scissors);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                wheelOpen = true;
                wheelSelection = currentForm;
                Time.timeScale = 0.2f;
            }

            if (wheelOpen)
            {
                Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                Vector2 delta = (Vector2)Input.mousePosition - center;

                if (delta.magnitude > 40f)
                {
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        wheelSelection = delta.x < 0 ? MorphoriaForm.Stone : MorphoriaForm.Scissors;
                    }
                    else
                    {
                        wheelSelection = delta.y > 0 ? MorphoriaForm.Leaf : MorphoriaForm.Paper;
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                wheelOpen = false;
                Time.timeScale = 1f;
                TrySwitchForm(wheelSelection);
            }
        }

        private void HandleMovement()
        {
            FormDefinition form = CurrentDefinition;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(horizontal, 0f, vertical);
            input = Vector3.ClampMagnitude(input, 1f);

            Transform cameraTransform = mainCamera != null ? mainCamera.transform : null;
            Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredDirection = forward * input.z + right * input.x;
            bool running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float targetSpeed = running ? form.runSpeed : form.speed;
            Vector3 desiredVelocity = desiredDirection * targetSpeed;
            currentHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, desiredVelocity, form.acceleration * Time.deltaTime);

            bool grounded = controller.isGrounded;
            if (grounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (Input.GetButtonDown("Jump") && grounded)
            {
                verticalVelocity = Mathf.Sqrt(form.jumpHeight * -2f * form.gravity);
            }

            verticalVelocity += form.gravity * Time.deltaTime;

            if (!grounded && form.canGlide && Input.GetButton("Jump") && verticalVelocity < -3f)
            {
                verticalVelocity = Mathf.MoveTowards(verticalVelocity, -3f, 18f * Time.deltaTime);
                externalVelocity += Vector3.up * (0.7f * Time.deltaTime);
            }

            if (currentForm == MorphoriaForm.Scissors && running && dashCooldown <= 0f && input.sqrMagnitude > 0.2f)
            {
                externalVelocity += desiredDirection.normalized * 8.0f;
                dashCooldown = 0.85f;
                ShowFeedback("Dash Cizo");
            }

            dashCooldown -= Time.deltaTime;

            Vector3 motion = currentHorizontalVelocity;
            motion.y = verticalVelocity;
            motion += externalVelocity;
            controller.Move(motion * Time.deltaTime);
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, 5f * Time.deltaTime);

            if (desiredDirection.sqrMagnitude > 0.05f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12f * Time.deltaTime);
            }
        }

        public void AddExternalVelocity(Vector3 velocity)
        {
            externalVelocity += velocity;
            externalVelocity = Vector3.ClampMagnitude(externalVelocity, 18f);
        }

        public void SetCheckpoint(Transform checkpoint)
        {
            checkpointPosition = checkpoint.position + Vector3.up * 1.2f;
            checkpointRotation = checkpoint.rotation;
            ShowFeedback("Checkpoint atteint");
        }

        public void Respawn()
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(checkpointPosition, checkpointRotation);
            verticalVelocity = 0f;
            currentHorizontalVelocity = Vector3.zero;
            externalVelocity = Vector3.zero;
            controller.enabled = true;
            inventory.Damage(1);
            ShowFeedback("Retour au checkpoint");
        }

        public void ShowFeedback(string text)
        {
            feedbackText = text;
            lastFeedbackTime = Time.time;
        }

        public bool TrySwitchForm(MorphoriaForm targetForm)
        {
            if (targetForm == currentForm)
            {
                return true;
            }

            if (forcedFormTimer > 0f)
            {
                ShowFeedback("Forme verrouillee");
                return false;
            }

            if (inventory.ChoiceStars <= 0)
            {
                ShowFeedback("Etoile prismatique requise");
                return false;
            }

            inventory.SpendChoiceStar();
            ApplyForm(targetForm, true);
            return true;
        }

        public void ForceForm(MorphoriaForm targetForm, float timerSeconds)
        {
            forcedFormTimer = Mathf.Max(forcedFormTimer, timerSeconds);
            ApplyForm(targetForm, true);
            ShowFeedback(FormCatalog.Get(targetForm).displayName + " imposee");
        }

        private void ApplyForm(MorphoriaForm targetForm, bool announce)
        {
            currentForm = targetForm;
            avatar.ApplyForm(CurrentDefinition);

            if (announce)
            {
                ShowFeedback(CurrentDefinition.heroName + " / " + CurrentDefinition.displayName);
            }

            FormChanged?.Invoke(currentForm);
        }

        public bool TryInteract()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * 0.8f, interactionRadius, interactionMask, QueryTriggerInteraction.Collide);
            IFormInteractable best = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < hits.Length; i++)
            {
                MonoBehaviour[] behaviours = hits[i].GetComponentsInParent<MonoBehaviour>();
                for (int j = 0; j < behaviours.Length; j++)
                {
                    if (behaviours[j] is IFormInteractable interactable)
                    {
                        float distance = Vector3.Distance(transform.position, behaviours[j].transform.position);
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            best = interactable;
                        }
                    }
                }
            }

            if (best == null)
            {
                ShowFeedback("Rien a activer");
                return false;
            }

            if (!best.CanInteract(CurrentDefinition))
            {
                ShowFeedback(best.Hint(CurrentDefinition));
                return false;
            }

            best.Interact(this);
            return true;
        }
    }
}
