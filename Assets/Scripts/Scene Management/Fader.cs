using System.Collections;
using UnityEngine;

namespace SciFi.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup = null;
        private Coroutine _currentFade = null;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1f;
        }

        public IEnumerator FadeOut(float time)
        {
            if (_currentFade != null)
            {
                StopCoroutine(_currentFade);
            }

            _currentFade = StartCoroutine(FadeOutRoutine(time));
            yield return _currentFade;
        }

        private IEnumerator FadeOutRoutine(float time)
        {
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            if (_currentFade != null)
            {
                StopCoroutine(_currentFade);
            }

            _currentFade = StartCoroutine(FadeInRoutine(time));
            yield return _currentFade;
        }

        private IEnumerator FadeInRoutine(float time)
        {
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }

}
