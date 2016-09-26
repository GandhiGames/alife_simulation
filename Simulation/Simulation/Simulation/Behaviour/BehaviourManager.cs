using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class BehaviourManager
    {
        #region Fields
        private MovingAgent agent; //Agent whose behaviour is being managed
        private Vector2 steeringForce; //Steering force of agent
        private Behaviours behaviour; //Procedural behaviour methods

        //Weights for each behaviour
        private float wanderWeight;
        private float seperationSightWeight;
        public float SeperationSightWeight { get { return seperationSightWeight; } }
        private float seperationLateralWeight;
        public float SeperationLateralWeight { get { return seperationLateralWeight; } }

        private float seperationPredSightWeight;
        public float SeperationPredSightWeight { get { return seperationPredSightWeight; } }
        private float seperationPredLateralWeight;
        public float SeperationPredLateralWeight { get { return seperationPredLateralWeight; } }

        private float alignmentSightWeight;
        public float AlignmentSightWeight { get { return alignmentSightWeight; } }
        private float alignmentLateraltWeight;
        public float AlignmentLateraltWeight { get { return alignmentLateraltWeight; } }
        private float cohesionSightWeight;
        public float CohesionSightWeight { get { return cohesionSightWeight; } }
        private float cohesionLateralWeight;
        public float CohesionLateralWeight { get { return cohesionLateralWeight; } }
        private float seekSightFoodWeight;
        public float SeekSightFoodWeight { get { return seekSightFoodWeight; } }
        private float seekLateralFoodWeight;
        public float SeekLateralFoodWeight { get { return seekLateralFoodWeight; } }

        private List<float> behaviourWeights;
 

        #endregion

        #region Constructor
        public BehaviourManager(MovingAgent agent)
        {
            behaviourWeights = new List<float>();
           
       

           /* seperationSightWeight = 2.0f;
            seperationLateralWeight = 2.0f;

            alignmentSightWeight = 0f;
            alignmentLateraltWeight = 0f;

            cohesionSightWeight = 0f;
            cohesionLateralWeight = 0f;

            wanderWeight = 0.4f;

            seekSightFoodWeight = 0f;
            seekLateralFoodWeight = 0f;*/

            this.agent = agent;
            
            behaviour = new Behaviours(agent);

        }
        #endregion

        #region Behaviour Selection Method Prey
        public Vector2 WeightTruncatedWithPrioritization(List<MovingAgent> preyInSight,
            List<MovingAgent> preyInLateral,
            List<Agent> vegetationInSight, List<Agent> vegetationInLateral,
            List<MovingAgent> predInSight, List<MovingAgent> predInLateral)
        {
            Vector2 force = Vector2.Zero;
            steeringForce = Vector2.Zero;



            if (Utilities.IsLateralSeperationPredatorOn)
            {
                force = behaviour.Seperation(predInLateral) * seperationPredLateralWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsSightSeperationPredatorOn)
            {
                force = behaviour.Seperation(predInSight) * seperationPredSightWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsLateralSeperationOn)
            {
                force = behaviour.Seperation(preyInLateral) * seperationLateralWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsSightSeperationOn)
            {
                force = behaviour.Seperation(preyInSight) * seperationSightWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsLateralSeekFoodOn)
            {
                force = behaviour.Seek(vegetationInLateral) * seekLateralFoodWeight;
                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }

            }

            if (Utilities.IsSightSeekFoodOn)
            {
                force = behaviour.Seek(vegetationInSight) * seekSightFoodWeight;
                    
                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsLateralAlignmentOn)
            {
                force = behaviour.Alignment(preyInLateral) * alignmentLateraltWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsSightAlignmentOn)
            {
                force = behaviour.Alignment(preyInSight) * alignmentSightWeight;
              
                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }
            if (Utilities.IsLateralCohesionOn)
            {
                force = behaviour.Cohesion(preyInLateral) * cohesionLateralWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsSightCohesionOn)
            {
                force = behaviour.Cohesion(preyInSight) * cohesionSightWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }


            if (Utilities.IsWanderOn)
            {
                force = behaviour.Wander() * wanderWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            return steeringForce;
        }
        #endregion

        #region Behaviour Selection Method Pred
        public Vector2 WeightTruncatedWithPrioritization(List<MovingAgent> preyInSight,
            List<MovingAgent> preyInLateral,
            List<MovingAgent> predInSight, List<MovingAgent> predInLateral)
        {
            Vector2 force = Vector2.Zero;
            steeringForce = Vector2.Zero;


            if (Utilities.IsLateralSeperationOn)
            {
                force = behaviour.Seperation(predInLateral) * seperationLateralWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsSightSeperationOn)
            {
                force = behaviour.Seperation(predInSight) * seperationSightWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsLateralSeekFoodOn)
            {
                force = behaviour.Seek(preyInLateral) * seekLateralFoodWeight;
                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }

            }

            if (Utilities.IsSightSeekFoodOn)
            {
                force = behaviour.Seek(preyInSight) * seekSightFoodWeight;
                    
                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsLateralAlignmentOn)
            {
                force = behaviour.Alignment(predInLateral) * alignmentLateraltWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsSightAlignmentOn)
            {
                force = behaviour.Alignment(predInSight) * alignmentSightWeight;
              
                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }
            if (Utilities.IsLateralCohesionOn)
            {
                force = behaviour.Cohesion(predInLateral) * cohesionLateralWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            if (Utilities.IsSightCohesionOn)
            {
                force = behaviour.Cohesion(predInSight) * cohesionSightWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }





            if (Utilities.IsWanderOn)
            {
                force = behaviour.Wander() * wanderWeight;

                if (!AccumulateForce(force))
                {
                    return steeringForce;
                }
            }

            return steeringForce;
        }
        #endregion

        #region Helper Methods
        private Boolean AccumulateForce(Vector2 forceToAdd)
        {
            double magSoFar = steeringForce.Length();
            double magRemaining = Utilities.MaxForce - magSoFar;

            if (magRemaining <= 0.0f)
            {
                return false;
            }

            double magToAdd = forceToAdd.Length();

            if (magToAdd < magRemaining)
            {
                steeringForce += forceToAdd;
            }
            else
            {
                steeringForce += Vector2.Normalize(forceToAdd) * (float)magRemaining;
            }

            return true;

        }
        #endregion

        #region get and Set Weight Methods
        public void SetWeights(List<float> weights)
        {
            behaviourWeights = weights;
            seekLateralFoodWeight = weights[0];
            seekSightFoodWeight = weights[1];
            alignmentSightWeight = weights[2];
            alignmentLateraltWeight = weights[3];
            cohesionSightWeight = weights[4];
            cohesionLateralWeight = weights[5];
            seperationSightWeight = weights[6];
            seperationLateralWeight = weights[7];
            wanderWeight = weights[8];

            if (weights.Count > 9)
            {
                seperationPredLateralWeight = weights[9];
                seperationPredSightWeight = weights[10];
            }
            if (Utilities.ExpectedWeights.Count > 0 && agent.Texture != Utilities.SimilarityTexture)
            {
                CompareWeightsToExpected();
            }

          
        }

        public List<float> GetWeights()
        {
            return behaviourWeights;

        }
        #endregion

        #region Helper Methods


        private void CompareWeightsToExpected()
        {
            int numMatching = 0;

            if (seekLateralFoodWeight >= Utilities.ExpectedWeights[0] - Utilities.BehaviourThreshold &&
                seekLateralFoodWeight <= Utilities.ExpectedWeights[0] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }

            if (seekSightFoodWeight >= Utilities.ExpectedWeights[1] - Utilities.BehaviourThreshold &&
              seekSightFoodWeight <= Utilities.ExpectedWeights[1] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }

            if (alignmentSightWeight >= Utilities.ExpectedWeights[2] - Utilities.BehaviourThreshold &&
                alignmentSightWeight <= Utilities.ExpectedWeights[2] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }

            if (alignmentLateraltWeight >= Utilities.ExpectedWeights[3] - Utilities.BehaviourThreshold &&
                alignmentLateraltWeight <= Utilities.ExpectedWeights[3] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }

            if (cohesionSightWeight >= Utilities.ExpectedWeights[4] - Utilities.BehaviourThreshold &&
                cohesionSightWeight <= Utilities.ExpectedWeights[4] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }

            if (cohesionLateralWeight >= Utilities.ExpectedWeights[5] - Utilities.BehaviourThreshold &&
                cohesionLateralWeight <= Utilities.ExpectedWeights[5] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }

            if (seperationSightWeight >= Utilities.ExpectedWeights[6] - Utilities.BehaviourThreshold &&
                seperationSightWeight <= Utilities.ExpectedWeights[6] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }

            if (seperationLateralWeight >= Utilities.ExpectedWeights[7] - Utilities.BehaviourThreshold &&
                 seperationLateralWeight <= Utilities.ExpectedWeights[7] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }


            if (wanderWeight >= Utilities.ExpectedWeights[8] - Utilities.BehaviourThreshold &&
              wanderWeight <= Utilities.ExpectedWeights[8] + Utilities.BehaviourThreshold)
            {
                numMatching++;
            }


            if (numMatching >= Utilities.NumOfBehavioursMatchingThreshold)
            {
                agent.Texture = Utilities.SimilarityTexture;
            }


        }

        public Vector2 GetCohesionLine()
        {
            return behaviour.CohesionLine;
        }

        public Vector2 GetSeperationLine()
        {
            return behaviour.SeperationLine;
        }

        public Vector2 GetAlignmentLine()
        {
            return behaviour.AlignmentLine;
        }

        public Vector2 GetSeekLine()
        {
            return behaviour.SeekLine;
        }


        public void ResetLines()
        {
            behaviour.CohesionLine = Vector2.Zero;
            behaviour.SeperationLine = Vector2.Zero;
            behaviour.AlignmentLine = Vector2.Zero;
            behaviour.SeekLine = Vector2.Zero;
        }

        #endregion
    }
}
