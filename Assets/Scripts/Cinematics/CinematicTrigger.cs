using UnityEngine;
using UnityEngine.Playables;

namespace SciFi.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _alreadyTriggered = false;
        private void OnTriggerEnter(Collider other)
        {
            if (!_alreadyTriggered && other.gameObject.CompareTag("Player"))
            {
                _alreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}