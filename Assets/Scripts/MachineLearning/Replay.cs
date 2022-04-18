using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.MachineLearning
{
    public class Replay
    {
        public List<double> states;
        public double reward;

        public Replay(List<double> inputs, double r)
        {
            states = new List<double>();

            foreach(double d in inputs)
            {
                states.Add(d);
            }


            reward = r;
        }
    }
}
