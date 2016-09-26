using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class Predator : MovingAgent
    {

        private List<MovingAgent> otherPredatorsInSight;
        private List<MovingAgent> otherPredatorsInLateral;
        private Texture2D texture;

        #region Constructor
        public Predator(Texture2D texture, Vector2 position, int size, float mass,
             float sightRadius, float lateralRadius)
            :base (texture, position, size, mass, sightRadius, lateralRadius)
        {
            neuralNet = new NeuralNet(false);
            this.texture = texture;

            otherPredatorsInSight = new List<MovingAgent>();
            otherPredatorsInLateral = new List<MovingAgent>();
        }
  

        public Predator(Texture2D texture, Vector2 position, int size, float mass,
            float sightRadius, float lateralRadius, NeuralNet neuralNet)
            : base (texture, position, size, mass, sightRadius, lateralRadius)
        {
            this.neuralNet = neuralNet;

            this.texture = texture;
            otherPredatorsInSight = new List<MovingAgent>();
            otherPredatorsInLateral = new List<MovingAgent>();

        }
        #endregion

        #region Draw Method
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Utilities.ShowPredTexture)
            {
                spriteBatch.Draw(texture,
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
        public void Update(GameTime gameTime, List<MovingAgent> prey,
            List<MovingAgent> otherPredators)
        {

            if (Energy <= 0)
                IsAlive = false;
            else
            {
                sightRadius = Size + Utilities.SightRadiusPredator;
                lateralRadius = Size + Utilities.LateralLinePred;


                if (Utilities.IsChangingAgentSizeOn)
                {
                    ChangeAgentSize();
                }
                else
                {
                    Size = Utilities.AgentTextureSize;
                }

                timeAlive++;

                UpdateWeights(prey, otherPredators);

                Vector2 force =
                    behaviourManager.WeightTruncatedWithPrioritization(localPreyInSight,
                        localPreyInLateral, otherPredatorsInSight, otherPredatorsInLateral);

                localPreyInSight.Clear();
                localPreyInLateral.Clear();
                otherPredatorsInSight.Clear();
                otherPredatorsInLateral.Clear();

                force = Truncate(force, Utilities.MaxForce);

                Utilities.Mass = Utilities.AgentTextureSize * 0.8f;

                Vector2 acceleration = force / Utilities.Mass;


                //float elapsedTime = gameTime.ElapsedGameTime.Milliseconds;

                velocity += acceleration;// *elapsedTime;
                velocity = Truncate(velocity, Utilities.MaxSpeed);




                Energy -= (acceleration.Length() * 0.2f) + 0.2f;

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
        public void UpdateWeights(List<MovingAgent> prey, List<MovingAgent> pred)
        {
            List<float> neuralNetInput = new List<float>();
            List<float> outputs = new List<float>();

            float numOfPreyInLateral = CheckPreyInLateral(prey);
            float numOfPreyInSight = CheckPreyInSight(prey);

            float numOfPredInLateral = CheckPredInLateral(pred);
            float numOfPredInSight = CheckPredInSight(pred);

            neuralNetInput.Add(numOfPreyInLateral);
            neuralNetInput.Add(numOfPreyInSight);

            neuralNetInput.Add(numOfPredInLateral);
            neuralNetInput.Add(numOfPredInSight);
            //neuralNetInput.Add(Energy / maxEnergy);

            outputs.AddRange(neuralNet.Update(ref neuralNetInput));

            behaviourManager.SetWeights(outputs);

        }
        #endregion



        #region ANN Helper Methods
      
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

                    if (temp < (SightRadius * SightRadius))
                    {
                        //if (!localPreyInLateral.Contains(agents[i]))
                       // {
                            otherPredatorsInSight.Add(agents[i]);
                            numOfAgentsInSight += 1f / Utilities.NumOfPredatorsOnScreen;
                        //}
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
                        otherPredatorsInLateral.Add(agents[i]);
                        numOfAgentsInLateral += 1f / Utilities.NumOfPredatorsOnScreen;
                    }
                }
            }

            return numOfAgentsInLateral;
        }


        #endregion

        public override void Reset()
        {
            otherPredatorsInLateral = new List<MovingAgent>();
            otherPredatorsInSight = new List<MovingAgent>();

            base.Reset();

        }

        public override Boolean CheckCollision(Agent otherAgent)
        {
            float dx = Position.X - otherAgent.Position.X;
            float dy = Position.Y - otherAgent.Position.Y;
            float radii = otherAgent.Radius + Radius;

            if ((dx * dx) + (dy * dy) < radii * radii)
            {

                if (otherAgent is Predator)
                {
                    energy -= 100 / Utilities.AgentEnergy;
                }

                


                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
}
