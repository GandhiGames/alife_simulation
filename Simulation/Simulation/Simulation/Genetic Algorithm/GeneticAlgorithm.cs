using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    abstract class GeneticAlgorithm
    {
        //PRedator GA
        protected List<Predator> predPool;
        protected int predCreatedSoFar;
        protected int numberOfAgentsFromPool;
        protected float totalFitnessScore;
        protected Texture2D predTexture;

        //Prey GA
        protected List<Prey> preyPool;
        protected int preyCreatedSoFar;
        protected Texture2D preyTexture, vegTexture;


        protected abstract void CalculateTotalFitness();


        protected abstract void UpdateGenerationNumber();


    }
}
