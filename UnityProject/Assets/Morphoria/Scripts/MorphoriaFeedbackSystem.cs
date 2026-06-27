using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public enum MorphoriaFeedbackCue
    {
        Denied,
        FormSwitch,
        Dash,
        CollectGolden,
        CollectPrism,
        AbilitySuccess,
        Checkpoint,
        VillagerSaved,
        BossHit,
        BossDefeated,
        Damage,
        LevelComplete
    }

    public sealed class MorphoriaFeedbackSystem : MonoBehaviour
    {
        public static MorphoriaFeedbackSystem Instance { get; private set; }

        private readonly Dictionary<MorphoriaFeedbackCue, AudioClip> clips = new Dictionary<MorphoriaFeedbackCue, AudioClip>();
        private AudioSource audioSource;
        private Material particleMaterial;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeBootstrap()
        {
            GetOrCreate();
        }

        public static MorphoriaFeedbackSystem GetOrCreate()
        {
            if (Instance != null)
            {
                return Instance;
            }

            MorphoriaFeedbackSystem existing = FindAnyObjectByType<MorphoriaFeedbackSystem>();
            if (existing != null)
            {
                return existing;
            }

            GameObject gameObject = new GameObject("Morphoria_FeedbackSystem");
            return gameObject.AddComponent<MorphoriaFeedbackSystem>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 0.74f;
        }

        public void Play(MorphoriaFeedbackCue cue, Vector3 position, Color color, float intensity = 1f)
        {
            intensity = Mathf.Clamp01(intensity);
            PlayTone(cue, intensity);
            SpawnBurst(cue, position, color, intensity);
            ShakeCamera(cue, intensity);
        }

        public void PlayForm(MorphoriaForm form, Vector3 position)
        {
            FormDefinition definition = FormCatalog.Get(form);
            Play(MorphoriaFeedbackCue.FormSwitch, position + Vector3.up * 1.1f, definition.accent, 0.75f);
        }

        public void PlayCollectible(CollectibleKind kind, Vector3 position)
        {
            bool prism = kind == CollectibleKind.ChoiceStar || kind == CollectibleKind.PrismStar;
            Play(prism ? MorphoriaFeedbackCue.CollectPrism : MorphoriaFeedbackCue.CollectGolden, position, prism ? new Color(0.8f, 0.45f, 1f) : new Color(1f, 0.82f, 0.22f), prism ? 0.9f : 0.65f);
        }

        private void PlayTone(MorphoriaFeedbackCue cue, float intensity)
        {
            AudioClip clip = ClipFor(cue);
            audioSource.pitch = PitchFor(cue);
            audioSource.PlayOneShot(clip, VolumeFor(cue) * intensity);
        }

        private AudioClip ClipFor(MorphoriaFeedbackCue cue)
        {
            if (clips.TryGetValue(cue, out AudioClip existing))
            {
                return existing;
            }

            ToneSpec spec = SpecFor(cue);
            int sampleRate = 24000;
            int samples = Mathf.Max(1, Mathf.RoundToInt(spec.duration * sampleRate));
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float u = i / (float)(samples - 1);
                float envelope = Mathf.Sin(Mathf.Clamp01(u) * Mathf.PI);
                float frequency = Mathf.Lerp(spec.startFrequency, spec.endFrequency, u);
                float tone = Mathf.Sin(2f * Mathf.PI * frequency * t);
                float overtone = Mathf.Sin(2f * Mathf.PI * frequency * 2.01f * t) * 0.22f;
                data[i] = (tone + overtone) * envelope * 0.42f;
            }

            AudioClip clip = AudioClip.Create("Morphoria_" + cue, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            clips[cue] = clip;
            return clip;
        }

        private void SpawnBurst(MorphoriaFeedbackCue cue, Vector3 position, Color color, float intensity)
        {
            if (cue == MorphoriaFeedbackCue.Denied)
            {
                return;
            }

            GameObject burst = new GameObject("FX_" + cue);
            burst.transform.position = position;
            ParticleSystem particles = burst.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            main.startLifetime = Mathf.Lerp(0.35f, 0.8f, intensity);
            main.startSpeed = Mathf.Lerp(1.8f, 4.2f, intensity);
            main.startSize = Mathf.Lerp(0.08f, 0.18f, intensity);
            main.startColor = color;
            main.maxParticles = 48;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = false;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = Mathf.Lerp(0.22f, 0.7f, intensity);

            ParticleSystemRenderer renderer = burst.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = ParticleMaterial();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            particles.Emit(ParticleCount(cue));
            StartCoroutine(DestroyWhenDone(burst, particles));
        }

        private IEnumerator DestroyWhenDone(GameObject burst, ParticleSystem particles)
        {
            yield return new WaitForSeconds(1.25f);
            if (burst != null)
            {
                Destroy(burst);
            }
        }

        private void ShakeCamera(MorphoriaFeedbackCue cue, float intensity)
        {
            float amount = ShakeAmount(cue) * intensity;
            if (amount <= 0f)
            {
                return;
            }

            ThirdPersonCamera[] cameras = FindObjectsByType<ThirdPersonCamera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].AddImpulse(amount, 0.22f + amount * 0.15f);
            }
        }

        private Material ParticleMaterial()
        {
            if (particleMaterial != null)
            {
                return particleMaterial;
            }

            Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Standard");
            particleMaterial = new Material(shader);
            return particleMaterial;
        }

        private static ToneSpec SpecFor(MorphoriaFeedbackCue cue)
        {
            switch (cue)
            {
                case MorphoriaFeedbackCue.Denied:
                    return new ToneSpec(160f, 90f, 0.11f);
                case MorphoriaFeedbackCue.FormSwitch:
                    return new ToneSpec(360f, 660f, 0.18f);
                case MorphoriaFeedbackCue.Dash:
                    return new ToneSpec(520f, 230f, 0.13f);
                case MorphoriaFeedbackCue.CollectGolden:
                    return new ToneSpec(740f, 1180f, 0.14f);
                case MorphoriaFeedbackCue.CollectPrism:
                    return new ToneSpec(640f, 1520f, 0.22f);
                case MorphoriaFeedbackCue.AbilitySuccess:
                    return new ToneSpec(300f, 820f, 0.16f);
                case MorphoriaFeedbackCue.Checkpoint:
                    return new ToneSpec(440f, 880f, 0.26f);
                case MorphoriaFeedbackCue.VillagerSaved:
                    return new ToneSpec(500f, 1040f, 0.24f);
                case MorphoriaFeedbackCue.BossHit:
                    return new ToneSpec(220f, 130f, 0.12f);
                case MorphoriaFeedbackCue.BossDefeated:
                    return new ToneSpec(210f, 760f, 0.34f);
                case MorphoriaFeedbackCue.Damage:
                    return new ToneSpec(190f, 80f, 0.16f);
                case MorphoriaFeedbackCue.LevelComplete:
                    return new ToneSpec(520f, 1320f, 0.36f);
                default:
                    return new ToneSpec(440f, 660f, 0.16f);
            }
        }

        private static float PitchFor(MorphoriaFeedbackCue cue)
        {
            return cue == MorphoriaFeedbackCue.CollectGolden ? UnityEngine.Random.Range(0.96f, 1.08f) : 1f;
        }

        private static float VolumeFor(MorphoriaFeedbackCue cue)
        {
            switch (cue)
            {
                case MorphoriaFeedbackCue.Denied:
                    return 0.32f;
                case MorphoriaFeedbackCue.Damage:
                case MorphoriaFeedbackCue.BossHit:
                    return 0.48f;
                case MorphoriaFeedbackCue.LevelComplete:
                case MorphoriaFeedbackCue.BossDefeated:
                    return 0.78f;
                default:
                    return 0.56f;
            }
        }

        private static int ParticleCount(MorphoriaFeedbackCue cue)
        {
            switch (cue)
            {
                case MorphoriaFeedbackCue.LevelComplete:
                case MorphoriaFeedbackCue.BossDefeated:
                    return 42;
                case MorphoriaFeedbackCue.FormSwitch:
                case MorphoriaFeedbackCue.VillagerSaved:
                    return 28;
                default:
                    return 16;
            }
        }

        private static float ShakeAmount(MorphoriaFeedbackCue cue)
        {
            switch (cue)
            {
                case MorphoriaFeedbackCue.Dash:
                    return 0.08f;
                case MorphoriaFeedbackCue.AbilitySuccess:
                case MorphoriaFeedbackCue.BossHit:
                    return 0.13f;
                case MorphoriaFeedbackCue.BossDefeated:
                case MorphoriaFeedbackCue.LevelComplete:
                    return 0.18f;
                case MorphoriaFeedbackCue.Damage:
                    return 0.2f;
                default:
                    return 0f;
            }
        }

        private readonly struct ToneSpec
        {
            public readonly float startFrequency;
            public readonly float endFrequency;
            public readonly float duration;

            public ToneSpec(float startFrequency, float endFrequency, float duration)
            {
                this.startFrequency = startFrequency;
                this.endFrequency = endFrequency;
                this.duration = duration;
            }
        }
    }
}
