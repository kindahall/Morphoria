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

        [Header("Movement Feel")]
        public float coyoteTime = 0.12f;
        public float jumpBufferTime = 0.14f;
        public float airControlMultiplier = 0.62f;
        public float fallGravityMultiplier = 1.35f;
        public float earlyJumpReleaseMultiplier = 1.75f;
        public float maxFallSpeed = 24f;
        public float landingImpulseThreshold = 7.5f;
        public float landingCameraImpulse = 0.045f;

        private CharacterController controller;
        private PlayerInventory inventory;
        private MorphoriaAvatar avatar;
        private ThirdPersonCamera cameraController;
        private MorphoriaForm currentForm = MorphoriaForm.Stone;
        private Vector3 currentHorizontalVelocity;
        private Vector3 externalVelocity;
        private float verticalVelocity;
        private float coyoteTimer;
        private float jumpBufferTimer;
        private float dashCooldown;
        private Vector3 checkpointPosition;
        private Quaternion checkpointRotation;
        private float lastFeedbackTime;
        private string feedbackText = string.Empty;
        private bool wheelOpen;
        private MorphoriaForm wheelSelection;
        private float forcedFormTimer;
        private string interactionPrompt = string.Empty;
        private bool interactionPromptReady;
        private bool knockoutLocked;

        public event Action<MorphoriaForm> FormChanged;

        public MorphoriaForm CurrentForm => currentForm;
        public FormDefinition CurrentDefinition => FormCatalog.Get(currentForm);
        public PlayerInventory Inventory => inventory;
        public bool IsWheelOpen => wheelOpen;
        public MorphoriaForm WheelSelection => wheelSelection;
        public float PlanarSpeed => currentHorizontalVelocity.magnitude;
        public bool IsGrounded => controller != null && controller.isGrounded;
        public bool IsGliding { get; private set; }
        public bool IsKnockedOut => knockoutLocked;
        public float ForcedFormTimer => Mathf.Max(0f, forcedFormTimer);
        public string FeedbackText => Time.time - lastFeedbackTime < 2.25f ? feedbackText : string.Empty;
        public string InteractionPrompt => interactionPrompt;
        public bool InteractionPromptReady => interactionPromptReady;

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

            ResolveCameraController();
            coyoteTimer = coyoteTime;
            ApplyForm(currentForm, false);
        }

        private void Update()
        {
            if (knockoutLocked)
            {
                currentHorizontalVelocity = Vector3.zero;
                externalVelocity = Vector3.zero;
                verticalVelocity = 0f;
                IsGliding = false;
                return;
            }

            if (forcedFormTimer > 0f)
            {
                forcedFormTimer = Mathf.Max(0f, forcedFormTimer - Time.deltaTime);
            }

            HandleFormInput();
            HandleMovement();
            UpdateInteractionPrompt();

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

            bool grounded = controller.isGrounded;
            if (grounded)
            {
                coyoteTimer = coyoteTime;
            }
            else
            {
                coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.deltaTime);
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferTimer = jumpBufferTime;
            }
            else
            {
                jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - Time.deltaTime);
            }

            float controlMultiplier = grounded ? 1f : airControlMultiplier;
            currentHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, desiredVelocity, form.acceleration * controlMultiplier * Time.deltaTime);

            IsGliding = false;
            if (grounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            {
                verticalVelocity = Mathf.Sqrt(form.jumpHeight * -2f * form.gravity);
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
                grounded = false;
                ResolveCameraController();
                if (cameraController != null)
                {
                    cameraController.AddImpulse(0.025f, 0.08f);
                }
            }

            float gravityMultiplier = 1f;
            if (!grounded && verticalVelocity < 0f)
            {
                gravityMultiplier = fallGravityMultiplier;
            }
            else if (!grounded && verticalVelocity > 0f && !Input.GetButton("Jump"))
            {
                gravityMultiplier = earlyJumpReleaseMultiplier;
            }

            verticalVelocity += form.gravity * gravityMultiplier * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, -maxFallSpeed);

            if (!grounded && form.canGlide && Input.GetButton("Jump") && verticalVelocity < -3f)
            {
                verticalVelocity = Mathf.MoveTowards(verticalVelocity, -3f, 18f * Time.deltaTime);
                externalVelocity += Vector3.up * (0.7f * Time.deltaTime);
                IsGliding = true;
            }

            if (currentForm == MorphoriaForm.Scissors && running && dashCooldown <= 0f && input.sqrMagnitude > 0.2f)
            {
                externalVelocity += desiredDirection.normalized * 8.0f;
                dashCooldown = 0.85f;
                ShowFeedback("Dash Cizo");
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Dash, transform.position + Vector3.up, CurrentDefinition.accent, 0.7f);
            }

            dashCooldown -= Time.deltaTime;

            Vector3 motion = currentHorizontalVelocity;
            motion.y = verticalVelocity;
            motion += externalVelocity;
            float fallSpeedBeforeMove = verticalVelocity;
            bool wasGrounded = grounded;
            controller.Move(motion * Time.deltaTime);
            bool groundedAfterMove = controller.isGrounded;
            if (groundedAfterMove && !wasGrounded && fallSpeedBeforeMove < -landingImpulseThreshold)
            {
                ResolveCameraController();
                if (cameraController != null)
                {
                    cameraController.AddImpulse(landingCameraImpulse, 0.16f);
                }
            }

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
            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Checkpoint, checkpoint.position + Vector3.up, Color.cyan, 0.85f);
        }

        public void Respawn()
        {
            bool depleted = ApplyDamage(1, "Retour au checkpoint", Vector3.zero);
            if (depleted)
            {
                return;
            }

            MoveToCheckpoint();
        }

        public bool ApplyDamage(int amount, string feedback, Vector3 knockback)
        {
            if (knockoutLocked || inventory == null)
            {
                return true;
            }

            if (amount <= 0)
            {
                return inventory.IsDepleted;
            }

            MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Damage, transform.position + Vector3.up, Color.red, 0.75f);
            if (knockback.sqrMagnitude > 0.01f)
            {
                AddExternalVelocity(knockback);
            }

            if (!string.IsNullOrEmpty(feedback))
            {
                ShowFeedback(feedback);
            }

            bool depleted = inventory.Damage(amount);
            if (depleted)
            {
                TriggerKnockout();
            }

            return depleted;
        }

        public void RecoverAtCheckpoint()
        {
            knockoutLocked = false;
            forcedFormTimer = 0f;
            wheelOpen = false;
            Time.timeScale = 1f;
            inventory.RestoreHearts();
            MoveToCheckpoint();
            ShowFeedback("Checkpoint");
        }

        private void MoveToCheckpoint()
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(checkpointPosition, checkpointRotation);
            verticalVelocity = 0f;
            coyoteTimer = coyoteTime;
            jumpBufferTimer = 0f;
            currentHorizontalVelocity = Vector3.zero;
            externalVelocity = Vector3.zero;
            IsGliding = false;
            controller.enabled = true;
        }

        private void TriggerKnockout()
        {
            knockoutLocked = true;
            currentHorizontalVelocity = Vector3.zero;
            externalVelocity = Vector3.zero;
            verticalVelocity = 0f;
            IsGliding = false;
            wheelOpen = false;

            MorphoriaGameOverScreen screen = FindAnyObjectByType<MorphoriaGameOverScreen>();
            if (screen == null)
            {
                screen = new GameObject("Game_Over_Screen").AddComponent<MorphoriaGameOverScreen>();
            }

            screen.Show(this);
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
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Denied, transform.position + Vector3.up, Color.red, 0.45f);
                return false;
            }

            if (inventory.ChoiceStars <= 0)
            {
                ShowFeedback("Etoile prismatique requise");
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Denied, transform.position + Vector3.up, Color.red, 0.45f);
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
                MorphoriaFeedbackSystem.GetOrCreate().PlayForm(currentForm, transform.position);
            }

            FormChanged?.Invoke(currentForm);
        }

        public bool TryInteract()
        {
            if (!FindBestInteractable(out IFormInteractable best, out MonoBehaviour bestBehaviour))
            {
                ShowFeedback("Rien a activer");
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Denied, transform.position + Vector3.up, Color.red, 0.35f);
                return false;
            }

            if (!best.CanInteract(CurrentDefinition))
            {
                ShowFeedback(best.Hint(CurrentDefinition));
                MorphoriaFeedbackSystem.GetOrCreate().Play(MorphoriaFeedbackCue.Denied, transform.position + Vector3.up, Color.red, 0.45f);
                return false;
            }

            best.Interact(this);
            UpdateInteractionPrompt();
            return true;
        }

        private void UpdateInteractionPrompt()
        {
            if (!FindBestInteractable(out IFormInteractable best, out MonoBehaviour bestBehaviour))
            {
                interactionPrompt = string.Empty;
                interactionPromptReady = false;
                return;
            }

            interactionPromptReady = best.CanInteract(CurrentDefinition);
            string hint = best.Hint(CurrentDefinition);
            if (string.IsNullOrEmpty(hint))
            {
                hint = bestBehaviour != null ? bestBehaviour.gameObject.name : "Interaction";
            }

            interactionPrompt = interactionPromptReady ? "F  " + hint : hint;
        }

        private bool FindBestInteractable(out IFormInteractable best, out MonoBehaviour bestBehaviour)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * 0.8f, interactionRadius, interactionMask, QueryTriggerInteraction.Collide);
            best = null;
            bestBehaviour = null;
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
                            bestBehaviour = behaviours[j];
                        }
                    }
                }
            }

            return best != null;
        }

        private void ResolveCameraController()
        {
            if (cameraController != null)
            {
                return;
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            cameraController = mainCamera != null ? mainCamera.GetComponent<ThirdPersonCamera>() : null;
        }

        private void OnValidate()
        {
            interactionRadius = Mathf.Max(0.6f, interactionRadius);
            coyoteTime = Mathf.Clamp(coyoteTime, 0f, 0.35f);
            jumpBufferTime = Mathf.Clamp(jumpBufferTime, 0f, 0.35f);
            airControlMultiplier = Mathf.Clamp(airControlMultiplier, 0.15f, 1f);
            fallGravityMultiplier = Mathf.Max(1f, fallGravityMultiplier);
            earlyJumpReleaseMultiplier = Mathf.Max(1f, earlyJumpReleaseMultiplier);
            maxFallSpeed = Mathf.Max(8f, maxFallSpeed);
            landingImpulseThreshold = Mathf.Max(0f, landingImpulseThreshold);
            landingCameraImpulse = Mathf.Clamp(landingCameraImpulse, 0f, 0.2f);
        }
    }
}
