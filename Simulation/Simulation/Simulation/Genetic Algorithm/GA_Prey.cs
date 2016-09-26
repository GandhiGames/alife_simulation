using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class GA_Prey : GeneticAlgorithm, CrossOver
    {
        #region Fields

        #endregion

        #region Constructor
        public GA_Prey(Texture2D preyTexture, Texture2D vegTexture)
        {
    
            preyPool = new List<Prey>();
            preyCreatedSoFar = Utilities.NumOfPreyOnScreen;
            numberOfAgentsFromPool = 0;
            totalFitnessScore = 0;

            this.preyTexture = preyTexture;
            this.vegTexture = vegTexture;

        }
        #endregion

        public void Update(GameTime gameTime, ref List<Prey> prey, ref List <Agent> vegetation,
            ref List<Predator> predator)
        {
            EnsureCorrectVegetationNumbers(vegetation);
            
            for (int i = 0; i < prey.Count; i++)
            {
                if (prey[i].IsAlive)
                {
                    UpdateAliveAgent(gameTime, prey, vegetation, predator, i);
                }
                else
                {
                    UpdatePool(prey[i]);

                    CalculateTotalFitness();

                    

                    if (preyCreatedSoFar < Utilities.NumOfPreyOnScreen)
                    {
                        CreateAgentInList(prey, i);
                    }
                    else
                    {
                        if (preyPool.Count > 1)
                        {
                            MovingAgent parentOne = FitnessProportionateSelection();
                            MovingAgent parentTwo = FitnessProportionateSelection();

                            if (!parentOne.Equals(parentTwo))
                            {
                                CreateAgentFromCrossover(parentOne, parentTwo, prey, i);
                            }
                         
                        }

                    }
                }
            }
        }

        #region Create Agent Methods
        private void CreateAgentFromCrossover(MovingAgent parentOne, MovingAgent parentTwo, 
            List<Prey> prey, int i)
        {
            NeuralNet neuralNetwork = CrossOver(parentOne, parentTwo);

            int tempX = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenWidth - Utilities.AgentTextureSize);
            int tempY = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenHeight - Utilities.AgentTextureSize);

            prey[i] = new Prey(preyTexture, new Vector2(tempX, tempY),
                Utilities.AgentTextureSize, Utilities.Mass, Utilities.SightRadiusPrey,
                Utilities.LateralLinePrey, neuralNetwork);


            if (Utilities.RandomMinMax(0, 1) < Utilities.MutationChance)
            {
                prey[i].Mutate();
            }


            preyCreatedSoFar++;

            UpdateGenerationNumber();
        }

        private void CreateAgentInList(List<Prey> prey, int i)
        {
            int tempX = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenWidth - Utilities.AgentTextureSize);
            int tempY = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenHeight - Utilities.AgentTextureSize);

            prey[i] = new Prey(preyTexture, new Vector2(tempX, tempY),
                Utilities.AgentTextureSize, Utilities.Mass, Utilities.SightRadiusPrey,
                Utilities.LateralLinePrey);

            preyCreatedSoFar++;
        }
        #endregion

        #region Update Helper Methods
        private void UpdateAliveAgent(GameTime gameTime, List<Prey> prey, List<Agent> vegetation,
            List<Predator> predator, int i)
        {


            List<MovingAgent> otherPrey = new List<MovingAgent>();
            otherPrey.AddRange(prey);

            List<MovingAgent> predators = new List<MovingAgent>();
            predators.AddRange(predator);

            prey[i].Update(gameTime, otherPrey, vegetation, predators);
            for (int b = 0; b < vegetation.Count; b++)
            {
                if (prey[i].CheckCollision(vegetation[b]))
                {
                    
                    prey[i].Energy += Utilities.EnergyGainedFromVegetation;
                    vegetation.RemoveAt(b);

                    int tempX = (int)Utilities.RandomMinMax(0, Utilities.ScreenWidth);
                    int tempY = (int)Utilities.RandomMinMax(0, Utilities.ScreenHeight);

                    vegetation.Add(new Agent(vegTexture, new Vector2(tempX, tempY),
                        Utilities.VegetationTextureSize));
                }
            }

            for (int k = 0; k < prey.Count; k++)
            {
                if (!prey[k].Equals(prey[i]) && prey[k].IsAlive)
                    prey[i].CheckCollision(prey[k]);
            }

        }

        protected override void CalculateTotalFitness()
        {
            totalFitnessScore = 0;

            for (int c = 0; c < preyPool.Count - 1; c++)
            {
                totalFitnessScore += preyPool[c].TimeAlive;
            }
        }

        protected override void UpdateGenerationNumber()
        {
            numberOfAgentsFromPool++;

            if (numberOfAgentsFromPool % Utilities.SizeOfPreyPool == 0)
            {
                Utilities.GenerationNumPrey++;

                if (Utilities.IsFileOutputOn)
                {
                    UpdateOutput();
                }

            }
        }
        
        private void UpdatePool(Prey prey)
        {

            if (!prey.IsAddedToPool)
            {
                preyPool.Add(prey);
                preyPool.Sort();

                if (preyPool.Count > Utilities.SizeOfPreyPool)
                {
                    preyPool.RemoveAt(preyPool.Count - 1);
                }
            }

        }

        private void UpdateOutput()
        {
            //FileOutput.WritePreyMeanOverallDistancesFromPredToFile(Utilities.MeanDistancePreyFromPredPerGen);
            FileOutput.WritePreyMeanOverallDistancesToFile(Utilities.MeanDistancePreyFromPreyPerGen);
            Utilities.MeanDistancePreyFromPredPerGen = 0f;

            Utilities.MeanDistancePreyFromPreyPerGen = 0f;
        }

        private void EnsureCorrectVegetationNumbers(List<Agent> vegetation)
        {
            if (vegetation.Count < Utilities.NumOfVegetationOnScreen)
            {
                int numToCreate = Utilities.NumOfVegetationOnScreen - vegetation.Count;

                for (int i = 0; i < numToCreate; i++)
                {
                    int tempX = (int)Utilities.RandomMinMax(0, Utilities.ScreenWidth);
                    int tempY = (int)Utilities.RandomMinMax(0, Utilities.ScreenHeight);

                    vegetation.Add(new Agent(vegTexture, new Vector2(tempX, tempY),
                        Utilities.VegetationTextureSize));
                }

            }
            else if (vegetation.Count > Utilities.NumOfVegetationOnScreen)
            {
                int numToDelete = vegetation.Count - Utilities.NumOfVegetationOnScreen;

                for (int i = 0; i < numToDelete; i++)
                {
                    vegetation.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Selection Methods
        public MovingAgent FitnessProportionateSelection()
        {
            float randomSlice = Utilities.RandomMinMax(0, totalFitnessScore);

            MovingAgent choosenAgent = null;

            float fitnessTotal = 0;

            for (int i = 0; i < preyPool.Count; i++)
            {
                fitnessTotal += preyPool[i].TimeAlive;

                if (fitnessTotal > randomSlice)
                {
                    choosenAgent = preyPool[i];
                    break;
                }
            }

            return choosenAgent;
        }

        public MovingAgent TournamentSelection()
        {
            float bestFitnessSoFar = 0;
            MovingAgent choosenAgent = null;
            //preyPool.Sort();


            for (int i = 0; i < Utilities.NumOfTourCompetitors; i++)
            {


                MovingAgent currentAgent
                    = preyPool[(int)Utilities.RandomMinMax(0, preyPool.Count * Utilities.PercentToSelect)];

                if (currentAgent.TimeAlive > bestFitnessSoFar)
                {
                    choosenAgent = currentAgent;
                    bestFitnessSoFar = currentAgent.TimeAlive;
                }
            }
            return choosenAgent;
        }
        #endregion

        public NeuralNet CrossOver(MovingAgent parentOne, MovingAgent parentTwo)
        {
            NeuralNet neuralNet = new NeuralNet(true);
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



    }
}
