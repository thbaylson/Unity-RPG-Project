using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneTrigger : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 0.75f;
        [SerializeField] float fadeInTime = 0.75f;
        [SerializeField] float fadeWaitTime = 0.75f;

        GameObject player;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.gameObject;
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if(sceneToLoad < 0)
            {
                Debug.LogError("Scene to load is not set.");
                yield break;
            }

            // DontDestroyOnLoad only works if the gameObject is in the root of the scene (not childed)
            DontDestroyOnLoad(gameObject);

            // Disable player control, fade out
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);

            // Load the scene
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            // Position the player
            SceneTrigger otherTrigger = GetOtherTrigger();
            UpdatePlayer(otherTrigger);

            // Fade In, enable player control, destroy this GameObject
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
            Destroy(gameObject);
        }

        private void UpdatePlayer(SceneTrigger otherTrigger)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherTrigger.spawnPoint.position);
            player.transform.rotation = otherTrigger.spawnPoint.rotation;
        }

        private SceneTrigger GetOtherTrigger()
        {
            foreach(SceneTrigger trigger in FindObjectsOfType<SceneTrigger>())
            {
                // We don't care if we find ourselves 
                if (trigger == this) continue;

                // We also don't care if we find a trigger with a different destination value
                if (trigger.destination != this.destination) continue;

                return trigger;
            }

            return null;
        }
    }
}