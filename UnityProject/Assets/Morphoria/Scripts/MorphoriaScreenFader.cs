using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morphoria
{
    public sealed class MorphoriaScreenFader : MonoBehaviour
    {
        public static MorphoriaScreenFader Instance { get; private set; }

        private float alpha;

        public static MorphoriaScreenFader GetOrCreate()
        {
            if (Instance != null)
            {
                return Instance;
            }

            MorphoriaScreenFader existing = FindAnyObjectByType<MorphoriaScreenFader>();
            if (existing != null)
            {
                return existing;
            }

            GameObject gameObject = new GameObject("Morphoria_ScreenFader");
            return gameObject.AddComponent<MorphoriaScreenFader>();
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
        }

        public IEnumerator FadeTo(float targetAlpha, float duration)
        {
            float startAlpha = alpha;
            float elapsed = 0f;
            duration = Mathf.Max(0.01f, duration);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            alpha = targetAlpha;
        }

        private void OnGUI()
        {
            if (alpha <= 0.01f)
            {
                return;
            }

            Color old = GUI.color;
            int oldDepth = GUI.depth;
            GUI.depth = -1000;
            GUI.color = new Color(0f, 0f, 0f, Mathf.Clamp01(alpha));
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.depth = oldDepth;
            GUI.color = old;
        }
    }
}
