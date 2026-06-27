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

    public sealed class MorphoriaAvatar : MonoBehaviour
    {
        private Transform visualRoot;

        public void ApplyForm(FormDefinition form)
        {
            if (visualRoot != null)
            {
                Destroy(visualRoot.gameObject);
            }

            visualRoot = new GameObject("Avatar_" + form.heroName).transform;
            visualRoot.SetParent(transform, false);
            visualRoot.localPosition = Vector3.zero;

            switch (form.form)
            {
                case MorphoriaForm.Stone:
                    BuildStone(form);
                    break;
                case MorphoriaForm.Leaf:
                    BuildLeaf(form);
                    break;
                case MorphoriaForm.Paper:
                    BuildPaper(form);
                    break;
                case MorphoriaForm.Scissors:
                    BuildScissors(form);
                    break;
            }
        }

        private void BuildStone(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("rock_body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.95f, 1.15f, 0.95f), body);
            CreatePart("rock_head", PrimitiveType.Sphere, new Vector3(0f, 1.85f, 0f), new Vector3(0.82f, 0.58f, 0.74f), body);
            CreatePart("left_fist", PrimitiveType.Sphere, new Vector3(-0.78f, 0.95f, 0.05f), new Vector3(0.46f, 0.46f, 0.46f), body);
            CreatePart("right_fist", PrimitiveType.Sphere, new Vector3(0.78f, 0.95f, 0.05f), new Vector3(0.46f, 0.46f, 0.46f), body);
            CreatePart("scarf", PrimitiveType.Cube, new Vector3(0f, 1.42f, 0.08f), new Vector3(1.1f, 0.12f, 0.12f), RuntimeMaterial(new Color(0.14f, 0.36f, 0.16f), form.accent));
        }

        private void BuildLeaf(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("leaf_body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.56f, 1.0f, 0.56f), body);
            CreatePart("leaf_head", PrimitiveType.Sphere, new Vector3(0f, 1.75f, 0f), new Vector3(0.62f, 0.58f, 0.62f), body);
            CreatePart("left_wing", PrimitiveType.Cube, new Vector3(-0.68f, 1.18f, -0.05f), new Vector3(0.12f, 0.68f, 1.0f), RuntimeMaterial(form.accent, form.color));
            CreatePart("right_wing", PrimitiveType.Cube, new Vector3(0.68f, 1.18f, -0.05f), new Vector3(0.12f, 0.68f, 1.0f), RuntimeMaterial(form.accent, form.color));
            CreatePart("orange_scarf", PrimitiveType.Cube, new Vector3(0f, 1.36f, 0.08f), new Vector3(0.86f, 0.1f, 0.1f), RuntimeMaterial(new Color(1f, 0.47f, 0.09f), form.accent));
        }

        private void BuildPaper(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("paper_body", PrimitiveType.Cube, new Vector3(0f, 0.95f, 0f), new Vector3(0.78f, 1.25f, 0.18f), body);
            CreatePart("paper_head", PrimitiveType.Cube, new Vector3(0f, 1.78f, 0f), new Vector3(0.7f, 0.54f, 0.22f), body);
            CreatePart("fold_left", PrimitiveType.Cube, new Vector3(-0.48f, 1.2f, 0.03f), new Vector3(0.18f, 0.8f, 0.18f), RuntimeMaterial(new Color(0.72f, 0.62f, 1f), form.accent));
            CreatePart("fold_right", PrimitiveType.Cube, new Vector3(0.48f, 1.2f, 0.03f), new Vector3(0.18f, 0.8f, 0.18f), RuntimeMaterial(new Color(0.72f, 0.62f, 1f), form.accent));
        }

        private void BuildScissors(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("scissors_body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.58f, 1.08f, 0.58f), body);
            CreatePart("scissors_head", PrimitiveType.Sphere, new Vector3(0f, 1.76f, 0f), new Vector3(0.6f, 0.52f, 0.6f), body);
            GameObject left = CreatePart("left_blade", PrimitiveType.Cube, new Vector3(-0.66f, 1.05f, 0.12f), new Vector3(0.16f, 0.9f, 0.18f), RuntimeMaterial(new Color(0.9f, 0.95f, 1f), form.accent));
            GameObject right = CreatePart("right_blade", PrimitiveType.Cube, new Vector3(0.66f, 1.05f, 0.12f), new Vector3(0.16f, 0.9f, 0.18f), RuntimeMaterial(new Color(0.9f, 0.95f, 1f), form.accent));
            left.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
            right.transform.localRotation = Quaternion.Euler(0f, 0f, -18f);
            CreatePart("blue_scarf", PrimitiveType.Cube, new Vector3(0f, 1.35f, 0.08f), new Vector3(0.84f, 0.1f, 0.1f), RuntimeMaterial(new Color(0.05f, 0.2f, 0.42f), form.accent));
        }

        private GameObject CreatePart(string partName, PrimitiveType primitive, Vector3 position, Vector3 scale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(primitive);
            part.name = partName;
            part.transform.SetParent(visualRoot, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;

            Collider partCollider = part.GetComponent<Collider>();
            if (partCollider != null)
            {
                Destroy(partCollider);
            }

            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            return part;
        }

        private static Material RuntimeMaterial(Color color, Color emission)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.SetColor("_EmissionColor", emission * 0.25f);
            material.EnableKeyword("_EMISSION");
            return material;
        }
    }

    public sealed class PlayerInventory : MonoBehaviour
    {
        public int startingHearts = 4;
        public int startingChoiceStars = 5;

        public int Hearts { get; private set; }
        public int GoldenStars { get; private set; }
        public int ChoiceStars { get; private set; }
        public int PrismStars { get; private set; }
        public int VillagersSaved { get; private set; }

        public event Action Changed;

        private void Awake()
        {
            Hearts = startingHearts;
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

        public void Damage(int amount)
        {
            Hearts = Mathf.Max(0, Hearts - amount);
            Changed?.Invoke();
        }
    }

    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 targetOffset = new Vector3(0f, 1.2f, 0f);
        public float distance = 7f;
        public float minPitch = -18f;
        public float maxPitch = 58f;
        public float mouseSensitivity = 2.3f;
        public float followSharpness = 12f;

        private float yaw;
        private float pitch = 24f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = target.position + targetOffset - rotation * Vector3.forward * distance;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            transform.rotation = rotation;
        }
    }

    public sealed class MorphoriaHud : MonoBehaviour
    {
        public MorphoriaPlayer player;
        public MiniBoss miniBoss;
        public string objective = "Liberez les villageois";

        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle smallStyle;

        private void OnGUI()
        {
            if (player == null)
            {
                return;
            }

            EnsureStyles();
            PlayerInventory inventory = player.Inventory;
            FormDefinition form = player.CurrentDefinition;

            DrawPanel(new Rect(18f, 18f, 250f, 128f), form.accent);
            GUI.Label(new Rect(36f, 28f, 210f, 24f), form.heroName + " / " + form.displayName, titleStyle);
            GUI.Label(new Rect(36f, 58f, 210f, 24f), "Coeurs  " + Hearts(inventory.Hearts), labelStyle);
            GUI.Label(new Rect(36f, 84f, 210f, 24f), "Etoiles  " + inventory.GoldenStars + " / 50", labelStyle);
            GUI.Label(new Rect(36f, 110f, 210f, 24f), "Prismes  " + inventory.ChoiceStars + "    Villageois  " + inventory.VillagersSaved + " / 4", labelStyle);

            if (player.ForcedFormTimer > 0f)
            {
                DrawPanel(new Rect(18f, 158f, 250f, 46f), form.accent);
                GUI.Label(new Rect(36f, 168f, 210f, 24f), "Timer  " + player.ForcedFormTimer.ToString("0.0") + " s", labelStyle);
            }

            DrawPanel(new Rect(Screen.width - 318f, 18f, 300f, 86f), form.accent);
            GUI.Label(new Rect(Screen.width - 300f, 30f, 260f, 26f), "Objectif", titleStyle);
            GUI.Label(new Rect(Screen.width - 300f, 62f, 260f, 26f), objective, labelStyle);

            string feedback = player.FeedbackText;
            if (!string.IsNullOrEmpty(feedback))
            {
                Rect rect = new Rect(Screen.width * 0.5f - 180f, 28f, 360f, 48f);
                DrawPanel(rect, form.accent);
                GUI.Label(new Rect(rect.x + 18f, rect.y + 13f, rect.width - 36f, 24f), feedback, titleStyle);
            }

            if (miniBoss != null && !miniBoss.IsDefeated)
            {
                DrawPanel(new Rect(Screen.width * 0.5f - 170f, Screen.height - 70f, 340f, 42f), new Color(0.63f, 0.23f, 0.95f));
                GUI.Label(new Rect(Screen.width * 0.5f - 150f, Screen.height - 60f, 300f, 24f), "Garde-Cage  " + miniBoss.Health + " / " + miniBoss.maxHealth, labelStyle);
            }

            DrawCompactWheel(form);

            if (player.IsWheelOpen)
            {
                DrawLargeWheel(player.WheelSelection);
            }
        }

        private void DrawCompactWheel(FormDefinition activeForm)
        {
            float size = 94f;
            Rect rect = new Rect(Screen.width - size - 28f, Screen.height - size - 24f, size, size);
            DrawPanel(rect, activeForm.accent);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 10f, rect.width - 24f, 22f), "Roue", smallStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 36f, rect.width - 24f, 22f), "1  2  3  4", labelStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 62f, rect.width - 24f, 22f), "Tab", labelStyle);
        }

        private void DrawLargeWheel(MorphoriaForm selection)
        {
            float radius = 142f;
            Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            DrawPanel(new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f), new Color(0.8f, 0.9f, 1f));
            DrawWheelSlot(center + new Vector2(0f, -92f), MorphoriaForm.Leaf, selection);
            DrawWheelSlot(center + new Vector2(-92f, 0f), MorphoriaForm.Stone, selection);
            DrawWheelSlot(center + new Vector2(92f, 0f), MorphoriaForm.Scissors, selection);
            DrawWheelSlot(center + new Vector2(0f, 92f), MorphoriaForm.Paper, selection);
            GUI.Label(new Rect(center.x - 50f, center.y - 14f, 100f, 28f), "Morphoria", titleStyle);
        }

        private void DrawWheelSlot(Vector2 center, MorphoriaForm formId, MorphoriaForm selection)
        {
            FormDefinition form = FormCatalog.Get(formId);
            Rect rect = new Rect(center.x - 48f, center.y - 26f, 96f, 52f);
            DrawPanel(rect, formId == selection ? form.accent : form.color);
            GUI.Label(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, 18f), form.heroName, smallStyle);
            GUI.Label(new Rect(rect.x + 8f, rect.y + 28f, rect.width - 16f, 18f), form.displayName, smallStyle);
        }

        private void DrawPanel(Rect rect, Color accent)
        {
            Color old = GUI.color;
            GUI.color = new Color(0.04f, 0.07f, 0.11f, 0.88f);
            GUI.Box(rect, GUIContent.none, panelStyle);
            GUI.color = new Color(accent.r, accent.g, accent.b, 0.95f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 3f), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = Texture2D.whiteTexture;
            panelStyle.border = new RectOffset(6, 6, 6, 6);

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = new Color(0.92f, 0.96f, 1f) },
                alignment = TextAnchor.MiddleLeft
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };
        }

        private static string Hearts(int count)
        {
            return new string('*', Mathf.Max(0, count));
        }
    }

    public sealed class AbilityGate : MonoBehaviour, IFormInteractable
    {
        public MorphoriaAbility requiredAbility = MorphoriaAbility.Any;
        public string successMessage = "Active";
        public bool destroyOnSuccess = true;
        public GameObject[] activateOnSuccess = Array.Empty<GameObject>();
        public GameObject[] deactivateOnSuccess = Array.Empty<GameObject>();

        private bool completed;

        public bool CanInteract(FormDefinition form)
        {
            return !completed && FormCatalog.HasAbility(form, requiredAbility);
        }

        public string Hint(FormDefinition form)
        {
            return "Forme requise : " + FormCatalog.AbilityLabel(requiredAbility);
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (completed)
            {
                return;
            }

            if (!CanInteract(player.CurrentDefinition))
            {
                player.ShowFeedback(Hint(player.CurrentDefinition));
                return;
            }

            completed = true;

            for (int i = 0; i < activateOnSuccess.Length; i++)
            {
                if (activateOnSuccess[i] != null)
                {
                    activateOnSuccess[i].SetActive(true);
                }
            }

            for (int i = 0; i < deactivateOnSuccess.Length; i++)
            {
                if (deactivateOnSuccess[i] != null)
                {
                    deactivateOnSuccess[i].SetActive(false);
                }
            }

            player.ShowFeedback(successMessage);

            if (destroyOnSuccess)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public sealed class WindZone : MonoBehaviour
    {
        public Vector3 windVelocity = new Vector3(0f, 10f, 3f);
        public Vector3 wrongFormPush = new Vector3(0f, 0f, -4f);

        private void OnTriggerStay(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            if (player.CurrentDefinition.canGlide)
            {
                player.AddExternalVelocity(windVelocity * Time.deltaTime);
                player.ShowFeedback("Courant porteur");
            }
            else if (!player.CurrentDefinition.canResistWind)
            {
                player.AddExternalVelocity(wrongFormPush * Time.deltaTime);
            }
        }
    }

    public sealed class BouncePad : MonoBehaviour
    {
        public float defaultBounce = 9f;
        public float leafBounce = 14f;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            float bounce = player.CurrentDefinition.canGlide ? leafBounce : defaultBounce;
            player.AddExternalVelocity(Vector3.up * bounce);
            player.ShowFeedback(player.CurrentDefinition.canGlide ? "Fleur Luma" : "Rebond");
        }
    }

    public sealed class HeavyPressurePlate : MonoBehaviour
    {
        public GameObject[] activateOnPress = Array.Empty<GameObject>();
        public GameObject[] deactivateOnPress = Array.Empty<GameObject>();
        public string message = "Plaque activee";
        private bool pressed;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null || pressed)
            {
                return;
            }

            if (!player.CurrentDefinition.canPushHeavy)
            {
                player.ShowFeedback("Trop leger : Pierre");
                return;
            }

            pressed = true;
            for (int i = 0; i < activateOnPress.Length; i++)
            {
                if (activateOnPress[i] != null)
                {
                    activateOnPress[i].SetActive(true);
                }
            }

            for (int i = 0; i < deactivateOnPress.Length; i++)
            {
                if (deactivateOnPress[i] != null)
                {
                    deactivateOnPress[i].SetActive(false);
                }
            }

            player.ShowFeedback(message);
        }
    }

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

    public sealed class Checkpoint : MonoBehaviour
    {
        private bool reached;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null || reached)
            {
                return;
            }

            reached = true;
            player.SetCheckpoint(transform);
        }
    }

    public sealed class LevelExit : MonoBehaviour
    {
        public int requiredVillagers = 4;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player == null)
            {
                return;
            }

            if (player.Inventory.VillagersSaved < requiredVillagers)
            {
                player.ShowFeedback("Villageois restants");
                return;
            }

            player.ShowFeedback("Niveau termine");
            Time.timeScale = 0.15f;
        }
    }

    public sealed class VillagerCage : MonoBehaviour, IFormInteractable
    {
        public MorphoriaAbility requiredAbility = MorphoriaAbility.Any;
        public bool requiresBossDefeated = true;
        public MiniBoss boss;
        public GameObject cageVisual;
        public GameObject villagerVisual;
        private bool opened;

        public bool CanInteract(FormDefinition form)
        {
            if (opened)
            {
                return false;
            }

            if (requiresBossDefeated && boss != null && !boss.IsDefeated)
            {
                return false;
            }

            return FormCatalog.HasAbility(form, requiredAbility);
        }

        public string Hint(FormDefinition form)
        {
            if (requiresBossDefeated && boss != null && !boss.IsDefeated)
            {
                return "Battez le Garde-Cage";
            }

            return "Cage : " + FormCatalog.AbilityLabel(requiredAbility);
        }

        public void Interact(MorphoriaPlayer player)
        {
            if (!CanInteract(player.CurrentDefinition))
            {
                player.ShowFeedback(Hint(player.CurrentDefinition));
                return;
            }

            opened = true;
            if (cageVisual != null)
            {
                cageVisual.SetActive(false);
            }

            if (villagerVisual != null)
            {
                villagerVisual.SetActive(true);
            }

            player.Inventory.SaveVillager();
            player.ShowFeedback("Villageois libere");
        }
    }

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

            if (health <= 0)
            {
                defeated = true;
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

    public sealed class PortalFormZone : MonoBehaviour
    {
        public MorphoriaForm forcedForm = MorphoriaForm.Stone;
        public float timerSeconds = 10f;

        private void OnTriggerEnter(Collider other)
        {
            MorphoriaPlayer player = other.GetComponentInParent<MorphoriaPlayer>();
            if (player != null)
            {
                player.ForceForm(forcedForm, timerSeconds);
            }
        }
    }
}
