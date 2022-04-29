using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] float defaultFadeOutTime = 0.75f;
        [SerializeField] float defaultFadeInTime = 0.75f;
                
        CanvasGroup canvasGroup;

        // GetComponent should be called in Awake to avoid race conditions
        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
            
        }

        public IEnumerator FadeOut(float? customFadeTime)
        {
            float time = customFadeTime ?? defaultFadeOutTime;

            while (canvasGroup.alpha < 1)
            {
                // Move alpha towards 1
                canvasGroup.alpha += Time.deltaTime / time;

                yield return null;
            }
        }

        public IEnumerator FadeIn(float? customFadeTime)
        {
            float time = customFadeTime ?? defaultFadeOutTime;

            while (canvasGroup.alpha > 0)
            {
                // Move alpha towards 0
                canvasGroup.alpha -= Time.deltaTime / time;

                yield return null;
            }
        }

        // These exist so you can cleanly call FadeOut() or FadeIn() w/o parameters
        // Otherwise you would need to call FadeOut(null) or FadeIn(null), which is ugly
        public IEnumerator FadeOut() { return FadeOut(null); }
        public IEnumerator FadeIn() { return FadeIn(null); }
    }
}