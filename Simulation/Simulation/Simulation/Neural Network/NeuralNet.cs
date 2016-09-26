using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    //Contains a list of layers: represents the whole neural net
    class NeuralNet
    {
        #region Variables
        private int numOfInput; //Number of inputs for each neuron
        private int numOfOutput; //Number of outputs of each neuron
        private int numHiddenLayers; //Number of hidden layers
        private int numOfNeuronsPerHiddenLayer; //Number of neurons per hidden layer
        List<NeuronLayer> layers; //List containing layers
        #endregion

        #region Constructor
        public NeuralNet(bool isPrey)
        {
            //use utility class for default values

            if (isPrey)
            {
                numOfInput = Utilities.NumOfInputsPrey;
                numOfOutput = Utilities.NumOfOutputsPrey;
                numHiddenLayers = Utilities.NumOfHiddenLayersPrey;
                numOfNeuronsPerHiddenLayer = Utilities.NumOfNeuronsPerHiddenPrey;
            }
            else
            {
                numOfInput = Utilities.NumOfInputsPred;
                numOfOutput = Utilities.NumOfOutputsPred;
                numHiddenLayers = Utilities.NumOfHiddenLayersPred;
                numOfNeuronsPerHiddenLayer = Utilities.NumOfNeuronsPerHiddenPred;
            }

            layers = new List<NeuronLayer>();

            CreateNeuralNet();

        }
        #endregion

        #region ANN Construction Method
        public void CreateNeuralNet()
        {
            //If there are hidden layers
            if (numHiddenLayers > 0)
            {
                //Create first layer
                layers.Add(new NeuronLayer(numOfNeuronsPerHiddenLayer, numOfInput));

                //Create any other subsequent hidden layers
                for (int i = 0; i < numHiddenLayers - 1; i++)
                {
                    //Input from first hidden layer
                    layers.Add(new NeuronLayer(numOfNeuronsPerHiddenLayer, 
                        numOfNeuronsPerHiddenLayer));
                }

                //Output layer
                //Input from subsequent or first hidden layer
                layers.Add(new NeuronLayer(numOfOutput, numOfNeuronsPerHiddenLayer));
            }
            else //If no hidden layers
            {
                //Input layer
                layers.Add(new NeuronLayer(numOfOutput, numOfInput));
            }


        }
        #endregion

        #region Update Method
        //Receives input and returns output: performs caluclations for neural net
        public List<float> Update(ref List<float> inputs)
        {
            List<float> inputList = new List<float>();
            inputList.AddRange(inputs);

            //Output from each layer
            List<float> outputs = new List<float>();

            int weightCount = 0;

            //Return empty if not corrent number of inputs
            if (inputList.Count != numOfInput)
            {
                Console.WriteLine("NeuralNet|Update|Size of inputs list not equal number of inputs");
                return outputs;
            }

            //Each layer
            for (int i = 0; i < numHiddenLayers + 1; i++)
            {
                if (i > 0)
                {
                    //Clear input and add output from previous layer
                    inputList.Clear();
                    inputList.AddRange(outputs);
                }

                outputs.Clear();

                weightCount = 0;

                for (int j = 0; j < layers[i].NumNeurons; ++j)
                {
                    float netInput = 0.0f;

                    int NumInputs = layers[i].Neurons[j].NumInputs;

                    //Each weight
                    for (int k = 0; k < NumInputs - 1; ++k)
                    {
                        //Sum the weights x inputs
                        netInput += layers[i].Neurons[j].Weight[k] *
                            inputList[weightCount++];
                    }

                    //Add in the bias
                    netInput += layers[i].Neurons[j].Weight[NumInputs - 1] *
                          Utilities.Bias;

                    //Store result in output
                    outputs.Add(Sigmoid(netInput));

                    weightCount = 0;
                }
            }

            return outputs;
        }
        #endregion

        #region Get and Set Methods
        //Gets weights from network
        public List<float> GetWeights()
        {
            //Temporarily store wights
            List<float> weights = new List<float>();

            //Each layer
            for (int i = 0; i < numHiddenLayers + 1; ++i)
            {
                //Each neuron
                for (int j = 0; j < layers[i].NumNeurons; ++j)
                {
                    //Each weight
                    for (int k = 0; k < layers[i].Neurons[j].NumInputs; ++k)
                    {
                        weights.Add(layers[i].Neurons[j].Weight[k]);
                    }
                }
            }

            return weights;
        }

        //Gets number of weights
        public int GetNumberOfWeights()
        {
            int weights = 0;

            //Each layer
            for (int i = 0; i < numHiddenLayers + 1; ++i)
            {
                //Eeach neuron
                for (int j = 0; j < layers[i].NumNeurons; ++j)
                {
                    //Each weight
                    for (int k = 0; k < layers[i].Neurons[j].NumInputs; ++k)
                        weights++;
                }
            }

            return weights;
        }

        //Sets weights for network (initially set to random values)
        public void SetWeights(ref List<float> weights)
        {
            //Used to cycle through received weights
            int weightCount = 0;

            //Each layer
            for (int i = 0; i < numHiddenLayers + 1; ++i)
            {
                //Each neuron
                for (int j = 0; j < layers[i].NumNeurons; ++j)
                {
                    //Each weight
                    for (int k = 0; k < layers[i].Neurons[j].NumInputs; ++k)
                    {
                        layers[i].Neurons[j].Weight[k] = weights[weightCount++];
                    }
                }
            }
        }
        #endregion

        #region Helper Methods
        //S shaped output
        public float Sigmoid(float netInput)
        {
            return (float) (1 / (1 + Math.Exp(-netInput / Utilities.ActivationResponse)));
        }
        #endregion

    }
}
