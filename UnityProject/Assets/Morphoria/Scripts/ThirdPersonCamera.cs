using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 targetOffset = new Vector3(0f, 1.2f, 0f);
        public float distance = 7f;
        public float minDistance = 2.1f;
        public float shoulderOffset = 0.55f;
        public float collisionRadius = 0.34f;
        public float collisionPadding = 0.24f;
        public float minPitch = -18f;
        public float maxPitch = 58f;
        public float mouseSensitivity = 2.3f;
        public float followSharpness = 12f;
        public float rotationSharpness = 16f;
        public float recenterDelay = 1.25f;
        public float recenterSharpness = 2.7f;
        public bool reduceMotion;
        public LayerMask collisionMask = Physics.DefaultRaycastLayers;

        private float yaw;
        private float pitch = 24f;
        private float lastManualLookTime;
        private float impulseStrength;
        private float impulseTimer;

        private void Start()
        {
            Vector3 euler = transform.eulerAngles;
            yaw = euler.y;
            pitch = NormalizePitch(euler.x);
            lastManualLookTime = Time.unscaledTime;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            float lookX = Input.GetAxis("Mouse X");
            float lookY = Input.GetAxis("Mouse Y");
            if (Mathf.Abs(lookX) > 0.01f || Mathf.Abs(lookY) > 0.01f)
            {
                lastManualLookTime = Time.unscaledTime;
            }

            yaw += lookX * mouseSensitivity;
            pitch -= lookY * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveInput.sqrMagnitude > 0.28f && Time.unscaledTime - lastManualLookTime > recenterDelay)
            {
                yaw = Mathf.LerpAngle(yaw, target.eulerAngles.y, 1f - Mathf.Exp(-recenterSharpness * Time.deltaTime));
            }

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pivot = target.position + targetOffset;
            Vector3 desiredPosition = pivot + rotation * Vector3.right * shoulderOffset - rotation * Vector3.forward * distance;
            desiredPosition = ResolveCollision(pivot, desiredPosition);

            float moveSharpness = reduceMotion ? followSharpness * 1.35f : followSharpness;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-moveSharpness * Time.deltaTime));

            Vector3 lookDirection = pivot - transform.position;
            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
                float turnSharpness = reduceMotion ? rotationSharpness * 1.25f : rotationSharpness;
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f - Mathf.Exp(-turnSharpness * Time.deltaTime));
            }

            ApplyImpulse();
        }

        public void AddImpulse(float strength, float duration)
        {
            if (reduceMotion)
            {
                strength *= 0.35f;
                duration *= 0.6f;
            }

            impulseStrength = Mathf.Max(impulseStrength, strength);
            impulseTimer = Mathf.Max(impulseTimer, duration);
        }

        private Vector3 ResolveCollision(Vector3 pivot, Vector3 desiredPosition)
        {
            Vector3 direction = desiredPosition - pivot;
            float desiredDistance = direction.magnitude;
            if (desiredDistance <= 0.01f)
            {
                return desiredPosition;
            }

            direction /= desiredDistance;
            Vector3 castStart = pivot + direction * collisionRadius;
            float castDistance = Mathf.Max(0.01f, desiredDistance - collisionRadius);
            if (Physics.SphereCast(castStart, collisionRadius, direction, out RaycastHit hit, castDistance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                float resolvedDistance = Mathf.Clamp(hit.distance + collisionRadius - collisionPadding, minDistance, desiredDistance);
                return pivot + direction * resolvedDistance;
            }

            return desiredPosition;
        }

        private void ApplyImpulse()
        {
            if (impulseTimer <= 0f || impulseStrength <= 0f)
            {
                return;
            }

            impulseTimer -= Time.deltaTime;
            float fade = Mathf.Clamp01(impulseTimer / 0.35f);
            Vector3 offset = new Vector3(
                Mathf.Sin(Time.time * 71f) * impulseStrength,
                Mathf.Sin(Time.time * 91f + 0.8f) * impulseStrength,
                0f) * fade;
            transform.position += transform.right * offset.x + transform.up * offset.y;

            if (impulseTimer <= 0f)
            {
                impulseStrength = 0f;
            }
        }

        private static float NormalizePitch(float value)
        {
            return value > 180f ? value - 360f : value;
        }
    }
}
