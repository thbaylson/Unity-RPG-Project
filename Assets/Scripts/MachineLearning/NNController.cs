using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.MachineLearning
{
    // Generic controller class that should be able to use any NN Brain. 
    //    Gets specific training info from Brain implementation.
    public class NNController : MonoBehaviour
    {
        Health health;
        RabbitBrain brain;

        private void Start()
        {
            brain = GetComponent<RabbitBrain>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            if (health.IsDead) return;

            List<double> inputs = GetObservations();
            //List<double> outputs = CalcInputs(inputs);
        }

        private List<double> GetObservations()
        {
            // This will involve a lot of raycasting
            throw new NotImplementedException();
        }

        // Sends inputs to the Brain
        // Recieves outputs from the Brain
        //List<double> CalcInputs(List<double> inputs)
        //{
        //    return brain.CalcOutput();
        //}
    }
}