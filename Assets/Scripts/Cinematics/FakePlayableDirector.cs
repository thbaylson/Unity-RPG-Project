using System.Collections;
using System;
using UnityEngine;

namespace RPG.Cinematics
{
    /**<summary>This is only to be used as an example of the Observer Pattern.
     * This class should not be used in the future as anything but an example.
     * As part of the example, this was attached to Intro Sequence in the RicksSandbox scene.</summary>*/
    public class FakePlayableDirector : MonoBehaviour
    {
        public event Action<float> onFinish;

        private void Start()
        {
            Invoke("OnFinish", 3f);
        }

        private void OnFinish()
        {
            onFinish(0f);
        }
    }
}