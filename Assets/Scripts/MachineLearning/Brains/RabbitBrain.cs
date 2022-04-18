using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.MachineLearning
{
    /**
     * <summary>
     * Search for food and avoid enemies. 
     * NOTE: Brain classes should be interchangeable. This is analagous to the driver classes from the Udemy course
     * </summary>
     */
    public class RabbitBrain : MonoBehaviour
    {
        // Unity Settings
        Health health;

        // ANN Settings
        // The number of input values
        public int numInputs;

        // The number of output values
        public int numOutputs;

        // The number of hidden layers
        public int numHiddenLayers;

        // The number of neurons in each hidden layer
        public int numNeuronsPerHiddenLayer;

        // Affects learning rate. Determines how much influence a given piece of training data has
        public double alpha;

        ANN ann;

        // RabbitBrain Settings
        // Reward to associate with actions
        float reward = 0.0f;
        // Memory - list of past actions and rewards
        List<Replay> replayMemory = new List<Replay>();
        // Memory capacity
        int mCapacity = 10000;

        // How much future states affect rewards
        float discount = 0.99f;
        // Chance of picking random action
        float exploreRate = 100.0f;
        float maxExploreRate = 100.0f;
        float minExploreRate = 0.01f;
        // Chance decay amount for each update
        float exploreDecay = 0.0001f;

        // Count when the ball is dropped
        int failCount = 0;

        /**
         * Max angle to apply to tilting each update. Make sure this is large enough so that the q value
         * multiplied by it is enough to recover balance when the ball is moving quickly.
         */
        float tiltSpeed = 0.5f;

        // Timer to keep track of balancing
        float timer = 0;
        // Record time ball is kept balanced
        float maxBalanceTime = 0;

        private void Start()
        {
            health = GetComponent<Health>();
        }

        void FixedUpdate()
        {
            timer += Time.deltaTime;
            List<double> inputs = new List<double>();
            List<double> qs;

            //inputs.Add(this.transform.rotation.x);
            //inputs.Add(ball.transform.rotation.z);
            //inputs.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);

            qs = SoftMax(ann.CalcOutput(inputs));
            double maxQ = qs.Max();
            int maxQIndex = qs.ToList().IndexOf(maxQ);
            exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

            // Randomness is very helpful when training for a complex problem
            if (Random.Range(0, 100) < exploreRate)
                maxQIndex = Random.Range(0, 2);

            if (maxQIndex == 0)
                this.transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
            else if (maxQIndex == 1)
                this.transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);

            if (health.IsDead)
                reward = -1.0f;
            else
                reward = 0.1f;

            Replay lastMemory = new Replay(GetObservations(), reward);

            // If we're at capacity, remove the oldest memory
            if (replayMemory.Count > mCapacity)
                replayMemory.RemoveAt(0);

            replayMemory.Add(lastMemory);

            // Reset condition. Perform Q learning on reset.
            if (health.IsDead)
            {
                QLearning();

                if (timer > maxBalanceTime)
                {
                    maxBalanceTime = timer;
                }

                ResetController();
                replayMemory.Clear();
                timer = 0;
                failCount++;
            }
        }

        private List<double> GetObservations()
        {
            throw new System.NotImplementedException();
        }

        private void QLearning()
        {
            double maxQ;

            // This is where the Q learning is happening
            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputsOld = new List<double>();
                List<double> toutputsNew = new List<double>();
                toutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));

                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.ToList().IndexOf(maxQOld);

                double feedback;
                // If this is the last memory, or if this memory generated a negative reward
                // A negative reward means that the ball was dropped, so any further actions don't matter and training should stop
                // A specific training session ends when the ball is dropped, ie: when reward == -1
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    feedback = replayMemory[i].reward;
                else
                {
                    toutputsNew = SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                    maxQ = toutputsNew.Max();
                    // Bellman equation
                    feedback = (replayMemory[i].reward + discount * maxQ);
                }

                toutputsOld[action] = feedback;
                ann.Train(replayMemory[i].states, toutputsOld);
            }
        }

        void ResetController()
        {
            // Code to reset the GameObject and Environment
        }

        /**<summary>
         * Scales down all values to be between 0 and 1. Also scales values such that the sum of the values is 1.
         * In the case of balancing the ball, the resulting List<double> is functionally: ["Tilt left chance", "Tile right chance"]
         * What does SoftMax do for trying to find the pumpkin? 
         * What are my outputs?
         *      Outputs will be coordinates to move towards?
         *      What if outputs represented direction to move in?
         *          [x direction, z direction]
         *      What if ML agent ignored NavMesh and just moved using force vectors?
         * </summary>*/
        List<double> SoftMax(List<double> values)
        {
            double max = values.Max();

            float scale = 0f;
            for (int i = 0; i < values.Count; ++i)
            {
                scale += Mathf.Exp((float)(values[i] - max));
            }

            // result becomes a List whose values add up to 1. Works well when you have percentages
            List<double> result = new List<double>();
            for (int i = 0; i < values.Count; ++i)
                result.Add(Mathf.Exp((float)(values[i] - max)) / scale);

            return result;
        }

        public RabbitBrain(int numInputs, int numOutputs, int numHiddenLayers, int numNeuronsPerHiddenLayer, double alpha)
        {
            ann = new ANN(numInputs, numOutputs, numHiddenLayers, numNeuronsPerHiddenLayer, alpha);
        }
    }
}