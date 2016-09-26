using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class GA_Pred : GeneticAlgorithm, CrossOver
    {

        #region Constructor
        public GA_Pred(Texture2D predTexture)
        {
  
            predPool = new List<Predator>();
            predCreatedSoFar = Utilities.NumOfPredatorsOnScreen;
            numberOfAgentsFromPool = 0;
            totalFitnessScore = 0;

            this.predTexture = predTexture;

        }
        #endregion

        private void UpdateAliveAgent(GameTime gameTime, ref List<Prey> prey, 
            ref List<Predator> pred, int i)
        {

            List<MovingAgent> predators = new List<MovingAgent>();
            predators.AddRange(pred);


            List<MovingAgent> preys = new List<MovingAgent>();
            preys.AddRange(prey);

            pred[i].Update(gameTime, preys, predators);

            for (int b = 0; b < prey.Count; b++)
            {
                if (prey[b].IsAlive)
                {
                    if (pred[i].CheckCollision(prey[b]))
                    {
                        pred[i].Energy += Utilities.EnergyGainedFromPrey;
                        prey[b].Energy = 0;

                    }
                }
            }

            for (int k = 0; k < pred.Count; k++)
            {
                if (!pred[k].Equals(pred[i]) && pred[k].IsAlive)
                    pred[i].CheckCollision(pred[k]);
            }
        }

        private void UpdatePool(Predator pred)
        {
            if (!pred.IsAddedToPool)
            {
                predPool.Add(pred);
                predPool.Sort();

                if (predPool.Count >= Utilities.SizeOfPreyPool)
                {
                    predPool.RemoveAt(predPool.Count - 1);
                }
            }
        }

        protected override void CalculateTotalFitness()
        {
            totalFitnessScore = 0;

            for (int c = 0; c < predPool.Count - 1; c++)
            {
                totalFitnessScore += predPool[c].TimeAlive;
            }
        }

        private void CreateAgentInList(ref List<Predator> pred, int i)
        {
            int tempX = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenWidth - Utilities.AgentTextureSize);
            int tempY = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenHeight - Utilities.AgentTextureSize);

            pred[i] = new Predator(predTexture, new Vector2(tempX, tempY),
                Utilities.AgentTextureSize, Utilities.Mass, Utilities.SightRadiusPredator,
                Utilities.LateralLinePred);

            predCreatedSoFar++;
        }

        public void Update(GameTime gameTime, ref List<Prey> prey, ref List<Predator> pred)
        {

            for (int i = 0; i < pred.Count; i++)
            {
                if (pred[i].IsAlive)
                {
                    UpdateAliveAgent(gameTime, ref prey, ref pred, i);
                }
                else
                {
                    UpdatePool(pred[i]);

                    CalculateTotalFitness();
                
             


                    if (predCreatedSoFar < Utilities.NumOfPredatorsOnScreen)
                    {

                        CreateAgentInList(ref pred, i);

                    }
                    else
                    {


                        if (predPool.Count > 1)
                        {
                            MovingAgent parentOne = FitnessProportionateSelection();
                            MovingAgent parentTwo = FitnessProportionateSelection();

                            if (!parentOne.Equals(parentTwo))
                            {
                                CreateAgentFromCrossover(parentOne, parentTwo, pred, i);
                            }
      
                        }

                    }
                }
            }
        }

        private void CreateAgentFromCrossover(MovingAgent parentOne, MovingAgent parentTwo, 
            List<Predator> pred, int i)
        {
    
                NeuralNet neuralNetwork = CrossOver(parentOne, parentTwo);

                int tempX = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenWidth - Utilities.AgentTextureSize);
                int tempY = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenHeight - Utilities.AgentTextureSize);

                pred[i] = new Predator(predTexture, new Vector2(tempX, tempY),
                    Utilities.AgentTextureSize, Utilities.Mass, Utilities.SightRadiusPredator,
                    Utilities.LateralLinePred, neuralNetwork);


                if (Utilities.RandomMinMax(0, 1) < Utilities.MutationChance)
                {
                    pred[i].Mutate();
                }


                predCreatedSoFar++;

                UpdateGenerationNumber();
            
        }

        protected override void UpdateGenerationNumber()
        {
            numberOfAgentsFromPool++;
            if (numberOfAgentsFromPool % Utilities.SizeOfPredPool == 0)
                Utilities.GenerationNumPred++;
        }

        public MovingAgent TournamentSelection()
        {
            float bestFitnessSoFar = 0;
            MovingAgent choosenAgent = null;
            //preyPool.Sort();


            for (int i = 0; i < Utilities.NumOfTourCompetitors; i++)
            {


                MovingAgent currentAgent
                    = predPool[(int)Utilities.RandomMinMax(0, predPool.Count * Utilities.PercentToSelect)];

                if (currentAgent.TimeAlive > bestFitnessSoFar)
                {
                    choosenAgent = currentAgent;
                    bestFitnessSoFar = currentAgent.TimeAlive;
                }
            }
            return choosenAgent;
        }

        public NeuralNet CrossOver(MovingAgent parentOne, MovingAgent parentTwo)
        {
            NeuralNet neuralNet = new NeuralNet(false);
            List<float> newWeights = new List<float>();
            List<float> parentOneWeights = parentOne.GetNetworkWeights();
            List<float> parentTwoWeights = parentTwo.GetNetworkWeights();

            int crossOverPoint;

            if (Utilities.IsRandomCrossoverPoint)
                crossOverPoint = (int)Utilities.RandomMinMax(0, parentOneWeights.Count);
            else
                crossOverPoint = (int)(parentOneWeights.Count * Utilities.CrossoverPoint);

            for (int i = 0; i < crossOverPoint; i++)
            {
                newWeights.Add(parentOneWeights[i]);
            }

            for (int i = crossOverPoint; i < parentOneWeights.Count; i++)
            {
                newWeights.Add(parentTwoWeights[i]);
            }

            neuralNet.SetWeights(ref newWeights);

            return neuralNet;
        }

        public MovingAgent FitnessProportionateSelection()
        {
            float randomSlice = Utilities.RandomMinMax(0, totalFitnessScore);

            MovingAgent choosenAgent = null;

            float fitnessTotal = 0;

            for (int i = 0; i < predPool.Count; i++)
            {
                fitnessTotal += predPool[i].TimeAlive;

                if (fitnessTotal > randomSlice)
                {
                    choosenAgent = predPool[i];
                    break;
                }
            }

            return choosenAgent;
        }


    }
    
}
