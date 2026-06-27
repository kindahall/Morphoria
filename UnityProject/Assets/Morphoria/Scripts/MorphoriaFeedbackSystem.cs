using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        private AudioSource ambienceSource;
        private AudioClip ambienceClip;
        private string ambienceScene = string.Empty;
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

            ConfigureAmbienceSource();
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
            StartAmbienceForScene(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= HandleSceneLoaded;
                Instance = null;
            }
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

        private void ConfigureAmbienceSource()
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1)
            {
                ambienceSource = sources[1];
            }
            else
            {
                ambienceSource = gameObject.AddComponent<AudioSource>();
            }

            ambienceSource.playOnAwake = false;
            ambienceSource.loop = true;
            ambienceSource.spatialBlend = 0f;
            ambienceSource.priority = 180;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartAmbienceForScene(scene.name);
        }

        private void StartAmbienceForScene(string sceneName)
        {
            if (ambienceSource == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = "Bootstrap";
            }

            if (ambienceScene == sceneName && ambienceSource.isPlaying)
            {
                return;
            }

            AmbienceSpec spec = AmbienceSpecFor(sceneName);
            ambienceScene = sceneName;
            if (ambienceClip != null)
            {
                Destroy(ambienceClip);
            }

            ambienceClip = CreateAmbienceClip(sceneName, spec);
            ambienceSource.clip = ambienceClip;
            ambienceSource.volume = spec.volume;
            ambienceSource.pitch = 1f;
            ambienceSource.Play();
        }

        private static AudioClip CreateAmbienceClip(string sceneName, AmbienceSpec spec)
        {
            const int sampleRate = 24000;
            const float duration = 8f;
            int samples = Mathf.RoundToInt(duration * sampleRate);
            float[] data = new float[samples];
            float root = Tuned(spec.rootFrequency, duration);
            float low = Tuned(spec.rootFrequency * 0.5f, duration);
            float fifth = Tuned(spec.fifthFrequency, duration);
            float high = Tuned(spec.shimmerFrequency, duration);

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float drift = 0.72f + Mathf.Sin(2f * Mathf.PI * 0.125f * t) * 0.12f;
                float pulse = 0.68f + Mathf.Sin(2f * Mathf.PI * 0.25f * t + spec.phase) * 0.08f;
                float pad =
                    Mathf.Sin(2f * Mathf.PI * root * t) * 0.46f +
                    Mathf.Sin(2f * Mathf.PI * fifth * t) * 0.28f +
                    Mathf.Sin(2f * Mathf.PI * low * t) * 0.22f;
                float shimmer = Mathf.Sin(2f * Mathf.PI * high * t) * Mathf.Sin(2f * Mathf.PI * 0.5f * t) * 0.04f;
                data[i] = (pad * drift * pulse + shimmer * spec.brightness) * 0.13f;
            }

            AudioClip clip = AudioClip.Create("Morphoria_Ambience_" + sceneName, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static float Tuned(float frequency, float duration)
        {
            return Mathf.Max(1f, Mathf.Round(frequency * duration) / duration);
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

        private static AmbienceSpec AmbienceSpecFor(string sceneName)
        {
            MorphoriaLevelInfo level = MorphoriaGameContent.GetLevelByScene(sceneName);
            if (level != null)
            {
                switch (level.worldId)
                {
                    case "canyon":
                        return new AmbienceSpec(86f, 129f, 620f, 0.11f, 0.7f, 1.1f);
                    case "gardens":
                        return new AmbienceSpec(146f, 219f, 980f, 0.12f, 1.15f, 0.35f);
                    case "archives":
                        return new AmbienceSpec(132f, 198f, 760f, 0.115f, 1.05f, 2.2f);
                    case "forge":
                        return new AmbienceSpec(96f, 144f, 540f, 0.105f, 0.85f, 2.9f);
                    case "fortress":
                        return new AmbienceSpec(74f, 111f, 666f, 0.12f, 0.9f, 3.5f);
                    default:
                        return new AmbienceSpec(124f, 186f, 820f, 0.115f, 1.0f, 0.8f);
                }
            }

            if (sceneName == MorphoriaGameContent.MainMenuScene)
            {
                return new AmbienceSpec(118f, 177f, 920f, 0.12f, 1.08f, 0.4f);
            }

            if (sceneName == MorphoriaGameContent.HubScene || sceneName == MorphoriaGameContent.FinaleScene)
            {
                return new AmbienceSpec(164f, 246f, 1040f, 0.13f, 1.18f, 0.15f);
            }

            if (sceneName == MorphoriaGameContent.WorldMapScene)
            {
                return new AmbienceSpec(102f, 153f, 700f, 0.105f, 0.95f, 1.8f);
            }

            return new AmbienceSpec(124f, 186f, 760f, 0.1f, 1f, 0f);
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

        private readonly struct AmbienceSpec
        {
            public readonly float rootFrequency;
            public readonly float fifthFrequency;
            public readonly float shimmerFrequency;
            public readonly float volume;
            public readonly float brightness;
            public readonly float phase;

            public AmbienceSpec(float rootFrequency, float fifthFrequency, float shimmerFrequency, float volume, float brightness, float phase)
            {
                this.rootFrequency = rootFrequency;
                this.fifthFrequency = fifthFrequency;
                this.shimmerFrequency = shimmerFrequency;
                this.volume = volume;
                this.brightness = brightness;
                this.phase = phase;
            }
        }
    }
}
