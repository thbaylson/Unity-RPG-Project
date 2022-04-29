using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    // Note: This class does not currently support saving objects created at runtime
    //    for more information about implementing this functionality, visit: https://www.gamedev.tv/courses/637539/lectures/12236096
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        // This makes Start a coroutine, which is automatically started when the scene loads
        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Save()
        {
            // Call to the saving system to save
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            // Call to the saving system to load
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
    }
}