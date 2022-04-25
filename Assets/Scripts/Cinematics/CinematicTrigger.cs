using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        private bool isTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!isTriggered && other.CompareTag("Player"))
            {
                isTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }

        public object CaptureState()
        {
            return isTriggered;
        }

        public void RestoreState(object state)
        {
            isTriggered = (bool)state;
        }
    }
}