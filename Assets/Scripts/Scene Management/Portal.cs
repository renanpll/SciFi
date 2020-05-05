using SciFi.Control;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SciFi.SceneManagement
{
    enum DestinationIndentifier
    {
        A, B, C, D, E
    }

    public class Portal : MonoBehaviour
    {
        [SerializeField] private int _sceneToLoad = -1;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private DestinationIndentifier destination;
        [SerializeField] private float _fadeTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (_sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            PlayerController newPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            yield return fader.FadeOut(_fadeTime);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(_sceneToLoad);

            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();

            if (otherPortal != null)
            {
                UpdatePlayer(otherPortal);
            }

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(_fadeTime);

            // newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal._spawnPoint.transform.position;
            player.transform.rotation = otherPortal._spawnPoint.transform.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;

        }
    }

}
