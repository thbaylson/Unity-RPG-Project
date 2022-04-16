using System;
using System.Collections.Generic;

namespace RPG.MachineLearning
{
    public class ANN
    {
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

		// The list of all layers (input, hidden, and output)
		List<Layer> layers = new List<Layer>();

		public ANN(int numInputs, int numOutputs, int numHiddenLayers, int numNeuronsPerHiddenLayer, double alpha)
		{
			this.numInputs = numInputs;
			this.numOutputs = numOutputs;
			this.numHiddenLayers = numHiddenLayers;
			this.numNeuronsPerHiddenLayer = numNeuronsPerHiddenLayer;
			this.alpha = alpha;

			if (numHiddenLayers > 0)
			{
				// Add the input layer
				layers.Add(new Layer(numNeuronsPerHiddenLayer, numInputs));

				// Add the hidden layers
				for (int i = 0; i < numHiddenLayers - 1; i++)
				{
					layers.Add(new Layer(numNeuronsPerHiddenLayer, numNeuronsPerHiddenLayer));
				}

				// Add the output layer
				layers.Add(new Layer(numOutputs, numNeuronsPerHiddenLayer));
			}
			else
			{
				// If there are no hidden layers, then we only need one layer to act as both input and output
				layers.Add(new Layer(numOutputs, numInputs));
			}
		}

		public List<double> Train(List<double> inputValues, List<double> desiredOutput)
		{
			List<double> outputValues = new List<double>();
			outputValues = CalcOutput(inputValues, desiredOutput);
			UpdateWeights(outputValues, desiredOutput);
			return outputValues;
		}

		public List<double> CalcOutput(List<double> inputValues, List<double> desiredOutput)
		{
			List<double> inputs;
			List<double> outputValues = new List<double>();
			int currentInput = 0;

			if (inputValues.Count != numInputs)
			{
				throw new ArgumentException($"Number of inputs must be {numInputs}");
				//Debug.Log("ERROR: Number of Inputs must be " + numInputs);
				//return outputValues;
			}

			inputs = new List<double>(inputValues);
			for (int layerIndex = 0; layerIndex < numHiddenLayers + 1; layerIndex++)
			{// Loops from the input layer, through the hidden layers, to the output layer

				if (layerIndex > 0)
				{
					// For every layer that is not the input layer, use the outputs from the previous layer as the inputs for the current layer
					inputs = new List<double>(outputValues);
				}

				outputValues.Clear();
				for (int neuronIndex = 0; neuronIndex < layers[layerIndex].numNeurons; neuronIndex++)
				{// Loops through each neuron within the current layer

					double N = 0;
					layers[layerIndex].neurons[neuronIndex].inputs.Clear();

					for (int neuronInputIndex = 0; neuronInputIndex < layers[layerIndex].neurons[neuronIndex].numInputs; neuronInputIndex++)
					{// Loops through each input for the current neuron. Each neuron's inputs are the outputs from the previous layer

						layers[layerIndex].neurons[neuronIndex].inputs.Add(inputs[currentInput]);

						// The sum of all (weight * input) calculations. A linear combination of weights and inputs
						N += layers[layerIndex].neurons[neuronIndex].weights[neuronInputIndex] * inputs[currentInput];
						currentInput++;
					}

					N -= layers[layerIndex].neurons[neuronIndex].bias;

					if (layerIndex == numHiddenLayers)// Output layer
					{
						layers[layerIndex].neurons[neuronIndex].output = OutputActivationFunction(N);
					}
					else// Input and hidden layers
					{
						layers[layerIndex].neurons[neuronIndex].output = HiddenActivationFunction(N);
					}

					outputValues.Add(layers[layerIndex].neurons[neuronIndex].output);
					currentInput = 0;
				}
			}
			return outputValues;
		}

		public string PrintWeights()
		{
			string weightStr = "";
			foreach (Layer l in layers)
			{
				foreach (Neuron n in l.neurons)
				{
					foreach (double w in n.weights)
					{
						weightStr += w + ",";
					}
					weightStr += n.bias + ",";
				}
			}
			return weightStr;
		}

		public void LoadWeights(string weightStr)
		{
			if (weightStr == "") return;
			string[] weightValues = weightStr.Split(',');
			int w = 0;
			foreach (Layer l in layers)
			{
				foreach (Neuron n in l.neurons)
				{
					for (int i = 0; i < n.weights.Count; i++)
					{
						n.weights[i] = System.Convert.ToDouble(weightValues[w]);
						w++;
					}
					n.bias = System.Convert.ToDouble(weightValues[w]);
					w++;
				}
			}
		}

		// This is where the backpropagation takes place.
		void UpdateWeights(List<double> outputs, List<double> desiredOutput)
		{
			double error;
			double previousError;
			double previousWeight;
			double errorGradientSum;
			double weightDelta;

			for (int layerIndex = numHiddenLayers; layerIndex >= 0; layerIndex--)
			{// Loops from numHiddenLayer to zero. Works backwards through the layers.

				for (int neuronIndex = 0; neuronIndex < layers[layerIndex].numNeurons; neuronIndex++)
				{

					if (layerIndex == numHiddenLayers)// The output layer; this runs on the first layerIndex iteration
					{
						error = desiredOutput[neuronIndex] - outputs[neuronIndex];

						// errorGradient calculated with Delta Rule: en.wikipedia.org/wiki/Delta_rule
						// Each neuron is responsible for its own portion of the error, so we need to figure out each neuron's individual portion
						layers[layerIndex].neurons[neuronIndex].errorGradient = outputs[neuronIndex] * (1 - outputs[neuronIndex]) * error;
					}
					else
					{
						layers[layerIndex].neurons[neuronIndex].errorGradient = layers[layerIndex].neurons[neuronIndex].output * (1 - layers[layerIndex].neurons[neuronIndex].output);
						errorGradientSum = 0;

						for (int previousLayerIterationNeuron = 0; previousLayerIterationNeuron < layers[layerIndex + 1].numNeurons; previousLayerIterationNeuron++)
						{
							// For each neuron, calculate its errorGradient using the errorGradients of the neurons on the next layer,
							//  which we found on the previous for-loop iteration
							previousError = layers[layerIndex + 1].neurons[previousLayerIterationNeuron].errorGradient;
							previousWeight = layers[layerIndex + 1].neurons[previousLayerIterationNeuron].weights[neuronIndex];
							errorGradientSum += previousError * previousWeight;
						}

						layers[layerIndex].neurons[neuronIndex].errorGradient *= errorGradientSum;
					}
					for (int neuronInputIndex = 0; neuronInputIndex < layers[layerIndex].neurons[neuronIndex].numInputs; neuronInputIndex++)
					{
						if (layerIndex == numHiddenLayers)// The output layer; this runs on the first layerIndex iteration
						{
							// Error = (What we wanted)        - (What we actually got)
							error = desiredOutput[neuronIndex] - outputs[neuronIndex];

							// Update each neuron's weights according to (alpha * input * error). Alpha represents the learning rate
							weightDelta = alpha * layers[layerIndex].neurons[neuronIndex].inputs[neuronInputIndex] * error;
							layers[layerIndex].neurons[neuronIndex].weights[neuronInputIndex] += weightDelta;
						}
						else// When it is NOT the output layer
						{
							// Update each neuron's weights according to (alpha * input * errorGradient)
							weightDelta = alpha * layers[layerIndex].neurons[neuronIndex].inputs[neuronInputIndex] * layers[layerIndex].neurons[neuronIndex].errorGradient;
							layers[layerIndex].neurons[neuronIndex].weights[neuronInputIndex] += weightDelta;
						}
					}

					// Update each neuron's bias
					layers[layerIndex].neurons[neuronIndex].bias += alpha * -1 * layers[layerIndex].neurons[neuronIndex].errorGradient;
				}

			}

		}

		double HiddenActivationFunction(double value)
		{
			return TanH(value);
		}

		double OutputActivationFunction(double value)
		{
			return TanH(value);
		}
		double StepFunction(double value)// AKA: Binary Step Function
		{
			if (value < 0) return 0;
			else return 1;
		}

		double SigmoidFunction(double value)// AKA: Logistic Softstep Function
		{
			double k = (double)System.Math.Exp(value);
			return k / (1.0f + k);
		}

		double TanH(double value)
		{
			// For when we want the output range to be (-1, 1)
			double k = (double)System.Math.Exp(-2 * value);
			return 2 / (1.0f + k) - 1;
		}

		double ReLu(double value)
		{
			if (value > 0) return value;
			else return 0;
		}

		double LeakyReLu(double value)
		{
			if (value < 0) return 0.01 * value;
			else return value;
		}
	}
}
