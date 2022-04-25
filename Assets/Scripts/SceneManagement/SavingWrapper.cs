using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

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