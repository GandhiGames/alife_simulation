using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    //Basic structure of ANN: the building blocks
    struct Neuron
    {
        #region Variables
        //Number of inputs into the neural network
        private int numOfInput;
        public int NumInputs
        {
            get { return numOfInput; }
        }

        //Weight of each input: determines activity of network
        private List<float> weight;
        public List<float> Weight
        {
            get { return weight; }
        }
        #endregion

        #region Constructor
        public Neuron(int numOfInput)
        {
            //Extra input for threshold - so it can be evolved with GA
            this.numOfInput = numOfInput + 1;

            weight = new List<float>();

            //Initialise random weights for each input
            for (int i = 0; i < this.numOfInput; i++)
            {
                weight.Add(Utilities.RandomMinMax(-1, 1));
            }
        }
        #endregion
    }

}
