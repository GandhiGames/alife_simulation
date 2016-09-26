using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class Prey : MovingAgent
    {

        //Holds local copy of objects

        private List<Agent> localVegetationInSight;
        private List<Agent> localVegetationInLateral;

        private List<MovingAgent> predatorsInSight;
        private List<MovingAgent> predatorsInLateral;
        //private Texture2D texture;

        private int weightsCount = 0, velocityCount = 0, distancePredCount = 0, distancePreyCount = 0;
        private float seekLatAv = 0, seekSightAv = 0, alightSightAv = 0,
            alightLatAv = 0, cohesionSightAv = 0,
            cohesionLatAv = 0, seperationSightAv = 0,
            seperationLatAv = 0, wanderAv = 0, velocityAv = 0, meanDistanceFromPred = 0,
            meanDistanceFromPrey = 0f;

        #region Constructor
        public Prey(Texture2D texture, Vector2 position, int size, float mass,
             float sightRadius, float lateralRadius)
            : base(texture, position, size, mass, sightRadius, lateralRadius)
        {
            neuralNet = new NeuralNet(true);

            //this.texture = texture;

            localVegetationInSight = new List<Agent>();
            localVegetationInLateral = new List<Agent>();
            predatorsInSight = new List<MovingAgent>();
            predatorsInLateral = new List<MovingAgent>();
        }


        public Prey(Texture2D texture, Vector2 position, int size, float mass,
            float sightRadius, float lateralRadius, NeuralNet neuralNet)
            : base(texture, position, size, mass, sightRadius, lateralRadius)
        {
            this.neuralNet = neuralNet;

            //this.texture = texture;

            localVegetationInSight = new List<Agent>();
            localVegetationInLateral = new List<Agent>();
            predatorsInSight = new List<MovingAgent>();
            predatorsInLateral = new List<MovingAgent>();

        }
        #endregion


        #region Draw Method
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Utilities.ShowPreyTexture)
            {
                spriteBatch.Draw(Texture,
                    new Rectangle((int)position.X, (int)position.Y, Size, Size), null, TextureColour,
                    0f, imageCenter, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(Utilities.CircleTexture,
                    new Rectangle((int)position.X, (int)position.Y, Size, Size), null, TextureColour,
                    0f, imageCenter, SpriteEffects.None, 0f);
            }
        }
        #endregion

        #region Update Method
        public void Update(GameTime gameTime, List<MovingAgent> otherPrey,
            List<Agent> vegetation, List<MovingAgent> predators)
        {

            if (Energy <= 0)
            {
                if (IsAlive && Utilities.IsFileOutputOn)
                {
                    PrintOutput(Utilities.GenerationNumPrey);
                    ClearUpdate();
                }
                IsAlive = false;
            }
            else
            {
                sightRadius = Size + Utilities.SightRadiusPrey;
                lateralRadius = Size + Utilities.LateralLinePrey;

                if (Utilities.IsChangingAgentSizeOn)
                {
                    ChangeAgentSize();

                }
                else
                {
                    Size = Utilities.AgentTextureSize;
                }

                timeAlive++;

                UpdateWeights(otherPrey, vegetation, predators);

                Vector2 force =
                    behaviourManager.WeightTruncatedWithPrioritization(localPreyInSight,
                        localPreyInLateral, localVegetationInSight, localVegetationInLateral, predatorsInSight,
                        predatorsInLateral);

                localPreyInSight.Clear();
                localPreyInLateral.Clear();
                localVegetationInSight.Clear();
                localVegetationInLateral.Clear();
                predatorsInSight.Clear();
                predatorsInLateral.Clear();

                UpdateOutput();

                force = Truncate(force, Utilities.MaxForce);

                Utilities.Mass = Utilities.AgentTextureSize * 0.8f;

                Vector2 acceleration = force / Utilities.Mass;


                float elapsedTime = gameTime.ElapsedGameTime.Milliseconds;

                velocity += acceleration;// *elapsedTime;
                velocity = Truncate(velocity, Utilities.MaxSpeed);
                velocityAv += velocity.Length();
                velocityCount++;

                Energy -= (acceleration.Length() * 0.2f) + 0.1f;

                position += velocity;// *elapsedTime;
                Utilities.CheckOutOfBounds(this);

                //Update heading
                if (velocity.LengthSquared() > 0.00000001)
                {
                    heading = Vector2.Normalize(velocity);
                }
            }

        }
        #endregion

        #region Neural Network Method
        public void UpdateWeights(List<MovingAgent> prey, List<Agent> vegetation, List<MovingAgent> predators)
        {
            List<float> neuralNetInput = new List<float>();
            List<float> outputs = new List<float>();

            float numOfPreyInLateral = CheckPreyInLateral(prey);
            float numOfPreyInSight = CheckPreyInSight(prey);

            float numOfVegInLateral = CheckVegetationInLateral(vegetation);
            float numOfVegInSight = CheckVegetationInSight(vegetation);

            float numOfPredInLateral = CheckPredInLateral(predators);
            float numOfPredInSight = CheckPredInSight(predators);

            neuralNetInput.Add(numOfPreyInLateral);
            neuralNetInput.Add(numOfPreyInSight);

            neuralNetInput.Add(numOfVegInLateral);
            neuralNetInput.Add(numOfVegInSight);

            neuralNetInput.Add(numOfPredInLateral);
            neuralNetInput.Add(numOfPredInSight);

            outputs.AddRange(neuralNet.Update(ref neuralNetInput));

            behaviourManager.SetWeights(outputs);

        }
        #endregion

        #region ANN Helper Methods

        private new float CheckPreyInSight(List<MovingAgent> agents)
        {
            float numOfAgentsInSight = 0;
            for (int i = 0; i < agents.Count; i++)
            {
                if (!this.Equals(agents[i]) && agents[i].IsAlive)
                {
                    Vector2 to = agents[i].Position - Position;
                    //float range = SightRadius; //+ agents[i].SightRadius;

                    float toLengthSquared = to.LengthSquared();

                    meanDistanceFromPrey += to.Length();
                    distancePreyCount++;

                    if (toLengthSquared < (SightRadius * SightRadius))
                    {
                        //if (!localPreyInLateral.Contains(agents[i]))
                        // {
                        localPreyInSight.Add(agents[i]);
                        numOfAgentsInSight += 1f / Utilities.NumOfPreyOnScreen;
                        //}


             
                    }
                }
            }

            return numOfAgentsInSight;
        }

        private new float CheckPreyInLateral(List<MovingAgent> agents)
        {
            float numOfAgentsInLateral = 0;
            for (int i = 0; i < agents.Count; i++)
            {
                if (!agents[i].Equals(this) && agents[i].IsAlive)
                {
                    Vector2 to = agents[i].Position - Position;
                    //float range = SightRadius; //+ agents[i].SightRadius;

                    float temp = to.LengthSquared();

                    if (temp < (lateralRadius * lateralRadius))
                    {
                        localPreyInLateral.Add(agents[i]);
                        numOfAgentsInLateral += 1f / Utilities.NumOfPreyOnScreen;


               
                    }
                }
            }

            return numOfAgentsInLateral;
        }
        private float CheckVegetationInSight(List<Agent> vegetation)
        {
            float numOfAgentsInSight = 0;
            for (int i = 0; i < vegetation.Count; i++)
            {
                //if (!vegetation[i].Equals(this))
                //{
                Vector2 to = vegetation[i].Position - Position;
                //float range = SightRadius; //+ agents[i].SightRadius;

                float temp = to.LengthSquared();

                if (temp < (SightRadius * SightRadius))
                {
                    //if (!localPreyInLateral.Contains(vegetation[i]))
                    //{
                    localVegetationInSight.Add(vegetation[i]);
                    numOfAgentsInSight += 1f / Utilities.NumOfVegetationOnScreen;
                    //}
                }
                //}
            }

            return numOfAgentsInSight;
        }

        private float CheckVegetationInLateral(List<Agent> vegetation)
        {
            float numOfAgentsInLateral = 0;
            for (int i = 0; i < vegetation.Count; i++)
            {
                if (!vegetation[i].Equals(this))
                {
                    Vector2 to = vegetation[i].Position - Position;
                    //float range = SightRadius; //+ agents[i].SightRadius;

                    float temp = to.LengthSquared();

                    if (temp < (lateralRadius * lateralRadius))
                    {
                        localVegetationInLateral.Add(vegetation[i]);
                        numOfAgentsInLateral += 1f / Utilities.NumOfVegetationOnScreen;
                    }
                }
            }

            return numOfAgentsInLateral;
        }


        private float CheckPredInSight(List<MovingAgent> agents)
        {
            float numOfAgentsInSight = 0;
            for (int i = 0; i < agents.Count; i++)
            {
                if (!this.Equals(agents[i]) && agents[i].IsAlive)
                {
                    Vector2 to = agents[i].Position - Position;
                    //float range = SightRadius; //+ agents[i].SightRadius;

                    float temp = to.LengthSquared();

                    meanDistanceFromPred += to.Length();
                    distancePredCount++;

                    if (temp < (SightRadius * SightRadius))
                    {
                
                        predatorsInSight.Add(agents[i]);
                        numOfAgentsInSight += 1f / Utilities.NumOfPredatorsOnScreen;
                      

                       
                    }
                }
            }

            return numOfAgentsInSight;
        }

        private float CheckPredInLateral(List<MovingAgent> agents)
        {
            float numOfAgentsInLateral = 0;
            for (int i = 0; i < agents.Count; i++)
            {
                if (!agents[i].Equals(this) && agents[i].IsAlive)
                {
                    Vector2 to = agents[i].Position - Position;
                    //float range = SightRadius; //+ agents[i].SightRadius;

                    float temp = to.LengthSquared();

                    if (temp < (lateralRadius * lateralRadius))
                    {
                        predatorsInLateral.Add(agents[i]);
                        numOfAgentsInLateral += 1f / Utilities.NumOfPredatorsOnScreen;

                        if (!predatorsInSight.Contains(agents[i]))
                        {
                            meanDistanceFromPred += to.Length();
                            distancePredCount++;
                        }
                    }
                }
            }

            return numOfAgentsInLateral;
        }
        #endregion

        #region File Output
        private void UpdateOutput()
        {
            weightsCount++;
            List<float> weights = behaviourManager.GetWeights();
            seekLatAv += weights[0];
            seekSightAv += weights[1];
            alightSightAv += weights[2];
            alightLatAv += weights[3];
            cohesionSightAv += weights[4];
            cohesionLatAv += weights[5];
            seperationSightAv += weights[6];
            seperationLatAv += weights[7];
            wanderAv += weights[8];

        }

        public void PrintOutput(int genNo)
        {
            List<float> weights = new List<float>();

            weights.Add(TimeAlive);

            seekLatAv /= weightsCount;
            weights.Add(seekLatAv);

            seekSightAv /= weightsCount;
            weights.Add(seekSightAv);

            alightSightAv /= weightsCount;
            weights.Add(alightSightAv);

            alightLatAv /= weightsCount;
            weights.Add(alightLatAv);

            cohesionSightAv /= weightsCount;
            weights.Add(cohesionSightAv);

            cohesionLatAv /= weightsCount;
            weights.Add(cohesionLatAv);

            seperationSightAv /= weightsCount;
            weights.Add(seperationSightAv);

            seperationLatAv /= weightsCount;
            weights.Add(seperationLatAv);

            wanderAv /= weightsCount;
            weights.Add(wanderAv);

            weights.Add(genNo);

            velocityAv /= velocityCount;
            weights.Add(velocityAv);

            //meanDistance /= distanceCount;
            //weights.Add(meanDistance);

            FileOutput.WriteWeightsToFile(weights);

            weights.Clear();

            if (distancePredCount > 0)
            {
                Utilities.MeanDistancePreyFromPredPerGen += meanDistanceFromPred /= distancePredCount;
            }

            if (distancePreyCount > 0)
            {
                Utilities.MeanDistancePreyFromPreyPerGen += meanDistanceFromPrey /= distancePreyCount;
            }

        }

        private void ClearUpdate()
        {
            velocityAv = 0;
            velocityCount = 0;

            seekLatAv = 0;
            seekSightAv = 0;
            alightSightAv = 0;
            alightLatAv = 0;
            cohesionSightAv = 0;
            cohesionLatAv = 0;
            seperationSightAv = 0;
            seperationLatAv = 0;
            wanderAv = 0;
            weightsCount = 0;

            distancePredCount = 0;
            meanDistanceFromPred = 0;
            distancePreyCount = 0;
            meanDistanceFromPrey = 0;

        }
        #endregion

        #region helper Methods
        public override void Reset()
        {

            
            localVegetationInSight = new List<Agent>();
            localVegetationInLateral = new List<Agent>();
            predatorsInSight = new List<MovingAgent>();
            predatorsInLateral = new List<MovingAgent>();

            base.Reset();

        }

        public override Boolean CheckCollision(Agent otherAgent)
        {
            float dx = Position.X - otherAgent.Position.X;
            float dy = Position.Y - otherAgent.Position.Y;
            float radii = otherAgent.Radius + Radius;

            if ((dx * dx) + (dy * dy) < radii * radii)
            {

                if (otherAgent is Prey)
                {
                    Energy -= 100 / Utilities.AgentEnergy;
                }


                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
