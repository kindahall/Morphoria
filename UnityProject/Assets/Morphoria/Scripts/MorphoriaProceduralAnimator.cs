using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    [RequireComponent(typeof(MorphoriaAvatar))]
    [RequireComponent(typeof(MorphoriaPlayer))]
    public sealed class MorphoriaProceduralAnimator : MonoBehaviour
    {
        private readonly Dictionary<Transform, LocalPose> basePoses = new Dictionary<Transform, LocalPose>();
        private MorphoriaAvatar avatar;
        private MorphoriaPlayer player;
        private Transform cachedRoot;
        private float locomotionPhase;

        private void Awake()
        {
            avatar = GetComponent<MorphoriaAvatar>();
            player = GetComponent<MorphoriaPlayer>();
        }

        private void LateUpdate()
        {
            if (avatar == null || player == null || avatar.VisualRoot == null)
            {
                return;
            }

            Transform root = avatar.VisualRoot;
            if (root != cachedRoot)
            {
                CaptureBasePoses(root);
            }

            float runSpeed = Mathf.Max(0.01f, player.CurrentDefinition.runSpeed);
            float speed01 = Mathf.Clamp01(player.PlanarSpeed / runSpeed);
            float formTempo = TempoFor(player.CurrentForm);
            locomotionPhase += Time.deltaTime * Mathf.Lerp(1.8f, formTempo, speed01);

            ResetChildren();
            AnimateRoot(root, speed01);
            AnimateSharedDetails(speed01);

            switch (player.CurrentForm)
            {
                case MorphoriaForm.Stone:
                    AnimateStone(speed01);
                    break;
                case MorphoriaForm.Leaf:
                    AnimateLeaf(speed01);
                    break;
                case MorphoriaForm.Paper:
                    AnimatePaper(speed01);
                    break;
                case MorphoriaForm.Scissors:
                    AnimateScissors(speed01);
                    break;
            }
        }

        private void CaptureBasePoses(Transform root)
        {
            cachedRoot = root;
            basePoses.Clear();

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                basePoses[child] = new LocalPose(child.localPosition, child.localRotation, child.localScale);
            }
        }

        private void ResetChildren()
        {
            foreach (KeyValuePair<Transform, LocalPose> entry in basePoses)
            {
                if (entry.Key == null)
                {
                    continue;
                }

                entry.Key.localPosition = entry.Value.Position;
                entry.Key.localRotation = entry.Value.Rotation;
                entry.Key.localScale = entry.Value.Scale;
            }
        }

        private void AnimateRoot(Transform root, float speed01)
        {
            float stride = Mathf.Sin(locomotionPhase * Mathf.PI * 2f);
            float groundedWeight = player.IsGrounded ? 1f : 0.32f;
            float bob = Mathf.Abs(stride) * 0.055f * speed01 * groundedWeight;
            float squash = Mathf.Abs(stride) * 0.055f * speed01 * groundedWeight;

            if (player.IsGliding)
            {
                bob = Mathf.Sin(Time.time * 7f) * 0.035f;
                squash = 0f;
            }

            root.localPosition = new Vector3(0f, bob, 0f);
            root.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(locomotionPhase * 3.1f) * speed01 * 1.6f);
            root.localScale = new Vector3(1f + squash * 0.45f, 1f - squash, 1f + squash * 0.45f);
        }

        private void AnimateSharedDetails(float speed01)
        {
            float blink = Mathf.PingPong(Time.time * 0.45f, 1f) > 0.96f ? 0.22f : 1f;
            float eyePulse = 1f + Mathf.Sin(Time.time * 4f) * 0.025f;
            ApplyScale("left_eye", new Vector3(eyePulse, blink, eyePulse));
            ApplyScale("right_eye", new Vector3(eyePulse, blink, eyePulse));

            float scarfWave = Mathf.Sin(Time.time * 5.5f + speed01 * 2f) * (4f + 8f * speed01);
            ApplyRotation("scarf", new Vector3(0f, 0f, scarfWave));
            ApplyRotation("orange_scarf", new Vector3(0f, 0f, scarfWave));
            ApplyRotation("blue_scarf", new Vector3(0f, 0f, scarfWave));
        }

        private void AnimateStone(float speed01)
        {
            float heavy = Mathf.Sin(locomotionPhase * Mathf.PI * 2f);
            ApplyOffset("left_fist", new Vector3(0f, Mathf.Max(0f, heavy) * 0.12f * speed01, -heavy * 0.06f * speed01));
            ApplyOffset("right_fist", new Vector3(0f, Mathf.Max(0f, -heavy) * 0.12f * speed01, heavy * 0.06f * speed01));
            ApplyRotation("left_fist", new Vector3(heavy * 7f * speed01, 0f, -heavy * 10f * speed01));
            ApplyRotation("right_fist", new Vector3(-heavy * 7f * speed01, 0f, -heavy * 10f * speed01));
            ApplyScale("amber_crack", Vector3.one * (1f + Mathf.Sin(Time.time * 5f) * 0.08f));
        }

        private void AnimateLeaf(float speed01)
        {
            float glideWeight = player.IsGliding ? 1f : 0f;
            float flapSpeed = player.IsGliding ? 13f : Mathf.Lerp(4f, 9f, speed01);
            float flap = Mathf.Sin(Time.time * flapSpeed) * (16f + 24f * Mathf.Max(speed01, glideWeight));
            float lift = player.IsGliding ? Mathf.Sin(Time.time * 10f) * 0.08f : 0f;

            ApplyRotation("left_wing", new Vector3(0f, -18f - flap, -10f));
            ApplyRotation("right_wing", new Vector3(0f, 18f + flap, 10f));
            ApplyOffset("left_wing", new Vector3(0f, lift, 0f));
            ApplyOffset("right_wing", new Vector3(0f, lift, 0f));
            ApplyRotation("leaf_crown", new Vector3(0f, Mathf.Sin(Time.time * 3.5f) * 8f, 0f));
        }

        private void AnimatePaper(float speed01)
        {
            float fold = Mathf.Sin(Time.time * 3.8f) * (8f + 10f * speed01);
            ApplyRotation("fold_left", new Vector3(0f, -fold, 0f));
            ApplyRotation("fold_right", new Vector3(0f, fold, 0f));
            ApplyRotation("paper_hat", new Vector3(0f, 0f, Mathf.Sin(Time.time * 2.5f) * 5f));
            ApplyScale("paper_rune", new Vector3(1f + Mathf.Sin(Time.time * 5.5f) * 0.22f, 1f, 1f));
        }

        private void AnimateScissors(float speed01)
        {
            float snap = Mathf.Sin(Time.time * Mathf.Lerp(4.5f, 11f, speed01));
            float angle = 12f + Mathf.Abs(snap) * 18f + speed01 * 10f;
            ApplyRotation("left_blade", new Vector3(0f, 0f, angle));
            ApplyRotation("right_blade", new Vector3(0f, 0f, -angle));
            ApplyRotation("left_handle", new Vector3(0f, 0f, Mathf.Sin(Time.time * 6f) * 7f));
            ApplyRotation("right_handle", new Vector3(0f, 0f, -Mathf.Sin(Time.time * 6f) * 7f));
        }

        private void ApplyOffset(string partName, Vector3 offset)
        {
            if (!TryGetPart(partName, out Transform part, out LocalPose pose))
            {
                return;
            }

            part.localPosition = pose.Position + offset;
        }

        private void ApplyRotation(string partName, Vector3 eulerOffset)
        {
            if (!TryGetPart(partName, out Transform part, out LocalPose pose))
            {
                return;
            }

            part.localRotation = pose.Rotation * Quaternion.Euler(eulerOffset);
        }

        private void ApplyScale(string partName, Vector3 scaleMultiplier)
        {
            if (!TryGetPart(partName, out Transform part, out LocalPose pose))
            {
                return;
            }

            part.localScale = Vector3.Scale(pose.Scale, scaleMultiplier);
        }

        private bool TryGetPart(string partName, out Transform part, out LocalPose pose)
        {
            pose = new LocalPose();
            part = cachedRoot != null ? cachedRoot.Find(partName) : null;
            return part != null && basePoses.TryGetValue(part, out pose);
        }

        private static float TempoFor(MorphoriaForm form)
        {
            switch (form)
            {
                case MorphoriaForm.Stone:
                    return 5.2f;
                case MorphoriaForm.Leaf:
                    return 8.6f;
                case MorphoriaForm.Paper:
                    return 6.4f;
                case MorphoriaForm.Scissors:
                    return 10.4f;
                default:
                    return 7.2f;
            }
        }

        private struct LocalPose
        {
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;
            public readonly Vector3 Scale;

            public LocalPose(Vector3 position, Quaternion rotation, Vector3 scale)
            {
                Position = position;
                Rotation = rotation;
                Scale = scale;
            }
        }
    }
}
