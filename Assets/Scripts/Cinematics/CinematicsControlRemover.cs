using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        private void Start()
        {
            // Notice that these are called in the exact order that they are added
            GetComponent<FakePlayableDirector>().onFinish += EnableControl;
            GetComponent<FakePlayableDirector>().onFinish += DisableControl;
        }

        void DisableControl(float nonsenseFloat)
        {
            print("DisableControl");
        }

        // This needs a float because the event Action<> was declared to require
        void EnableControl(float nonsenseFloat)
        {
            print("EnableControl");
        }
    }
}