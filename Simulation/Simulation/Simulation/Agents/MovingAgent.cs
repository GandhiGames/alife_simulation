using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class MovingAgent : Agent, IComparable
    {
        #region Fields and Properties
         //Velocity of agent
        protected Vector2 velocity; 
        public Vector2 Velocity { get { return velocity; } }

        protected BehaviourManager behaviourManager; //Manager of behaviour for agent

        //Heading of agent
        protected Vector2 heading;
        public Vector2 Heading { get { return heading; } }

        //Max force of applied behaviours
        private float maxForce;
        public float MaxForce { get { return maxForce; } }
 
        //Sight radius of agent
        protected float sightRadius;
        public float SightRadius { get { return sightRadius; } }

        //Lateral radius of agent
        protected float lateralRadius;
        public float LateralRadius { get { return lateralRadius; } }

        protected int timeAlive;
        //public float StandardisedFitness { get { return 1 / timeAlive; } }
        public int TimeAlive { get { return timeAlive; } }


        protected NeuralNet neuralNet; //Neural net to control weights
        public NeuralNet NeuralNet { get { return neuralNet; } }

        protected float maxEnergy;

        private bool isAddedToPool;
        public bool IsAddedToPool { get { return isAddedToPool; } }

        protected List<MovingAgent> localPreyInSight;
        protected List<MovingAgent> localPreyInLateral;

        public float PositionX
        {
            get
            {
                return Position.X;
            }

            set
            {
                position.X = value;
  
            }
        }

        public float PositionY
        {
            get
            {
                return Position.Y;
            }

            set
            {
                position.Y = value;
            }
        }
        #endregion

        #region Constructor
        public MovingAgent(Texture2D texture, Vector2 position, int size, float mass,
            float sightRadius, float lateralRadius)
            :base(texture, position, size)
        {
           // this.mass = mass;
            //this.sightRadius = sightRadius;
            this.lateralRadius = lateralRadius;

            Energy = Utilities.AgentEnergy;
            maxEnergy = Energy;

            timeAlive = 0;

            velocity = new Vector2();


            Random rand = new Random();
            double rotation = rand.NextDouble() * (Math.PI * 2);
            heading = new Vector2((float)Math.Sin(rotation), (float)-Math.Cos(rotation));

            maxForce = Utilities.MaxForce;

            behaviourManager = new BehaviourManager(this);


            localPreyInSight = new List<MovingAgent>();
            localPreyInLateral = new List<MovingAgent>();

            isAddedToPool = false;

        }

        #endregion

        #region ANN Helper Methods
        protected float CheckPreyInSight(List<MovingAgent> agents)
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
                            localPreyInSight.Add(agents[i]);
                            numOfAgentsInSight += 1f / Utilities.NumOfPreyOnScreen;
                        //}
                    }
                }
            }

            return numOfAgentsInSight;
        }

        protected float CheckPreyInLateral(List<MovingAgent> agents)
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


        #endregion

        #region GA Helper Methods
        public void Mutate()
        {
            List<float> weights = new List<float>();
            weights.AddRange(neuralNet.GetWeights());

           // int mutate = (int)Utilities.RandomMinMax(0, weights.Count);
           // weights[mutate] += (Utilities.RandomMinMax(-1, 1) * Utilities.MaxPerturbation);

            for (int i = 0; i < weights.Count; ++i)
            {
                if (Utilities.RandomMinMax(0, 1) < Utilities.MutationRate)
                {
                    weights[i] += (Utilities.RandomMinMax(-1, 1) * Utilities.MaxPerturbation);
                }
            }

            neuralNet.SetWeights(ref weights);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            MovingAgent otherAgent = obj as MovingAgent;

            if (otherAgent != null)
            {
                //return this.TimeAlive.CompareTo(otherAgent.TimeAlive);  
                return otherAgent.TimeAlive - this.TimeAlive;
            }
            else
            {
                return 1;
            }
        }

        public List<float> GetNetworkWeights()
        {
            return neuralNet.GetWeights();
        }

        #endregion

        #region Misc Helper Methods

        protected void ChangeAgentSize()
        {
            Size = (int)(Energy / (maxEnergy * 0.05f));
            if (Size < Utilities.AgentTextureSize)
                Size = Utilities.AgentTextureSize;
            else if (Size > Utilities.AgentMaxTextureSize)
                Size = Utilities.AgentMaxTextureSize;
        }

       /* private void ChangeAgentSize()
        {

            if (Energy < maxEnergy * 0.1f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.1f);
            else if (Energy < maxEnergy * 0.2f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.2f);
            else if (Energy < maxEnergy * 0.3f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.3f);
            else if (Energy < maxEnergy * 0.4f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.4f);
            else if (Energy < maxEnergy * 0.5f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.5f);
            else if (Energy < maxEnergy * 0.6f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.6f);
            else if (Energy < maxEnergy * 0.7f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.7f);
            else if (Energy < maxEnergy * 0.8f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.8f);
            else if (Energy < maxEnergy * 0.9f)
                Size = (int)(Utilities.AgentTextureSizeMaxVar * 0.9f);
            else
                Size = Utilities.AgentTextureSizeMaxVar;
        }*/

        public List<float> GetBehaviourWeights()
        {
            return behaviourManager.GetWeights();
        }

        public virtual void Reset()
        {
            Energy = Utilities.AgentEnergy;

            timeAlive = 0;

            velocity = new Vector2();

            IsAlive = true;

            localPreyInSight = new List<MovingAgent>();
            localPreyInLateral = new List<MovingAgent>();
   

            Random rand = new Random();
            double rotation = rand.NextDouble() * (Math.PI * 2);
            heading = new Vector2((float)Math.Sin(rotation), (float)-Math.Cos(rotation));

            
            int tempX = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenWidth - Utilities.AgentTextureSize);
            int tempY = (int)Utilities.RandomMinMax(0 + Utilities.AgentTextureSize, Utilities.ScreenHeight - Utilities.AgentTextureSize);

            Position = new Vector2(tempX, tempY);

            behaviourManager = new BehaviourManager(this);

            isAddedToPool = false;

        }

        protected Vector2 Truncate(Vector2 original, float max)
        {
            if (original.Length() > max)
            {
                 
                original.Normalize();

                original *= max;

                if (Utilities.IsAgentColourVelocityOn)
                    Texture = Utilities.MaxVelocityTexture;


                return original;
            }
            else if (Texture == Utilities.MaxVelocityTexture)
            {
                Texture = DefaultTexture;
            }



            return original;
        }


        public Vector2 GetCohesionLine()
        {
            return behaviourManager.GetCohesionLine();
        }

        public Vector2 GetSeperationLine()
        {
            return behaviourManager.GetSeperationLine();
        }

        public Vector2 GetAlignmentLine()
        {
            return behaviourManager.GetAlignmentLine();
        }

        public Vector2 GetSeekLine()
        {
            return behaviourManager.GetSeekLine();
        }

        public float GetCohesionWeight()
        {
            return (behaviourManager.CohesionLateralWeight + behaviourManager.CohesionSightWeight) * 2f;
        }

        public float GetAlignmentWeight()
        {
            return (behaviourManager.AlignmentLateraltWeight + behaviourManager.AlignmentSightWeight) * 2f;
        }

        public float GetSeperationWeight()
        {
            return (behaviourManager.SeperationSightWeight + behaviourManager.SeperationLateralWeight) * 2f;
        }

        public float GetSeekWeight()
        {
            return (behaviourManager.SeekSightFoodWeight + behaviourManager.SeekLateralFoodWeight) * 2f;
        }

        public void ResetLines()
        {
            behaviourManager.ResetLines();
        }
        #endregion


    }
}
