using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] float defaultFadeOutTime = 0.75f;
        [SerializeField] float defaultFadeInTime = 0.75f;
                
        CanvasGroup canvasGroup;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOut(float? configFadeTime)
        {
            float time = configFadeTime ?? defaultFadeOutTime;

            while (canvasGroup.alpha < 1)
            {
                // Move alpha towards 1
                canvasGroup.alpha += Time.deltaTime / time;

                yield return null;
            }
        }

        public IEnumerator FadeIn(float? configFadeTime)
        {
            float time = configFadeTime ?? defaultFadeOutTime;

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