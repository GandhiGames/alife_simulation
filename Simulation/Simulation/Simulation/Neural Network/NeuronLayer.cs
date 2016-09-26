using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    //Contains a list of neurons: represents a layer
    class NeuronLayer
    {
        #region Variables
        //Number of neurons in layer
        private int numOfNeurons;
        public int NumNeurons
        {
            get { return numOfNeurons; }
        }

        //List of neurons in layer
        private List<Neuron> neurons;
        public List<Neuron> Neurons
        {
            get { return neurons; }
        }
        #endregion

        #region Constructor
        public NeuronLayer(int numOfNeurons, int numOfInput)
        {
            this.numOfNeurons = numOfNeurons;
            neurons = new List<Neuron>();

            //Adds neurons to neuron list
            for (int i = 0; i < numOfNeurons; ++i)
            {
                neurons.Add(new Neuron(numOfInput));
            }
        }
        #endregion

    }
}
