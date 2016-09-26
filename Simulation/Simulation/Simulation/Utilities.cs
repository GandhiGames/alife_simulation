using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    //centralised location to store variables and useful methods required throughout project
    static class Utilities
    {
        //Used for helper methods
        private static Random random = new Random();
        public static Random Random { get { return random; } }

        #region Environment Properties
        //Window Size
        public static int ScreenWidth { get; private set; }
        public static int ScreenHeight { get; private set; }
        #endregion

        #region Behaviour Properties
        //Wander behaviour
        public static float WanderRadius { get; set; } //Radius of circle projected in front of agent
        public static float WanderDistance { get; set; } //Distance of circle from agent

        //Flags
        public static bool IsWanderOn { get; set; } ////On/off flags for wander

        //On/off flags for seperation
        public static bool IsSightSeperationOn { get; set; }
        public static bool IsLateralSeperationOn { get; set; }
        public static bool IsSightSeperationPredatorOn { get; set; }
        public static bool IsLateralSeperationPredatorOn { get; set; }

        //On/off flags for alignment
        public static bool IsSightAlignmentOn { get; set; }
        public static bool IsLateralAlignmentOn { get; set; }

        ///On/off flags for cohesion
        public static bool IsSightCohesionOn { get; set; }
        public static bool IsLateralCohesionOn { get; set; }

        //On/off flags for seek
        public static bool IsSightSeekFoodOn { get; set; } 
        public static bool IsLateralSeekFoodOn { get; set; } 
        #endregion

        #region Agent Properties
        //Sight Radius
        public static float SightRadiusPrey { get; set; } 
        public static float SightRadiusPredator { get; set; } 

        //Lateral line radius
        public static float LateralLinePrey { get; set; }
        public static float LateralLinePred { get; set; }

        //Max number of agents on screen - also number of pre-spawned agents
        public static int NumOfPreyOnScreen { get; private set; }
        public static int NumOfPredatorsOnScreen { get; private set; }
        public static int NumOfVegetationOnScreen { get; set; }
        
        public static int AgentTextureSize { get; private set; } //Size of agent texture onscreen when not variable
        public static int AgentMaxTextureSize { get; private set; }
        public static int VegetationTextureSize { get; private set; } //Size of vegetation
        public static float MaxSpeed { get; set; } //Maximum speed agent can move
        public static float MaxForce { get; set; } //Maximum force of agents behaviour
        public static float Mass { get; set; } //Mass of agent
        public static int AgentEnergy { get; set; } //Initial energy level of agents
        public static float EnergyGainedFromVegetation { get; set; }
        public static float EnergyGainedFromPrey { get; set; } 
        #endregion

        #region ANN Properties
        //Prey
        public static int NumOfInputsPrey { get; set; } //Number of inputs into ANN
        public static int NumOfOutputsPrey { get; private set; } //Number of outputs for the prey from ANN
        public static int NumOfHiddenLayersPrey { get; set; } //Number of hidden layers in ANN
        public static int NumOfNeuronsPerHiddenPrey { get; set; } //Number of neurons per hidden layer

        //Predator
        public static int NumOfInputsPred { get; set; } //Number of inputs for the predator into ANN
        public static int NumOfOutputsPred { get; private set; } //Number of outputs for the predator from ANN
        public static int NumOfHiddenLayersPred { get; set; } //Number of hidden layers in ANN
        public static int NumOfNeuronsPerHiddenPred { get; set; } //Number of neurons per hidden layer

        public static float ActivationResponse { get; private set; } //Used for Sigmoid Function
        public static int Bias { get; private set; } //Bias for weights
        #endregion

        #region GA Properties
        public static float MutationRate { get; set; } //Mutation rate of GA, once mutation has begun
        public static float MutationChance { get; set; } //Chance of mutation occuring
        public static float MaxPerturbation { get; set; } //Max perturbation of GA, used in mutation
        public static int NumOfTourCompetitors { get; set; } //Number of agents in tournament selection
        public static float PercentToSelect { get; set; } //Top percent to select
        public static int GenerationNumPrey { get; set; } //Current generation number - prey
        public static int GenerationNumPred { get; set; } //Current generation number - pred
        public static float CrossoverProbability { get; set; } //Probability two chosen chromosomes will swap bits
        public static bool IsRandomCrossoverPoint { get; set; } //If true, a random crossover point will be selected
        public static float CrossoverPoint { get; set; } //Used if random crossover point false

        //Number of agents stored in genetic pool
        public static int SizeOfPreyPool { get; set; }
        public static int SizeOfPredPool { get; set; }
        #endregion

        #region Evaluation Properties
   
        public static bool IsLineAlignmentDrawingOn { get; set; }
        public static bool IsLineCohesionDrawingOn { get; set; }
        public static bool IsLineSeperationDrawingOn { get; set; }
        public static bool IsLineSeekDrawingOn { get; set; }
        public static bool IsChangingAgentSizeOn { get; set; }
        public static bool IsSightRadiusVisible { get; set; }
        public static bool IsLateralLineVisible { get; set; }
        public static bool IsAgentColourVelocityOn { get; set; }
        public static bool IsPaused { get; set; }
        public static bool IsLookingForExpected { get; set; }
        public static List<float> ExpectedWeights { get; set; }
        //public static Color ExpectedColour { get; private set; }
        public static Texture2D SimilarityTexture { get; private set; } 
        public static Texture2D MaxVelocityTexture { get; private set; }
        public static float BehaviourThreshold { get; private set; }
        public static int NumOfBehavioursMatchingThreshold { get; private set; }
        public static float MeanDistancePreyFromPredPerGen { get; set; }
        public static float MeanDistancePreyFromPreyPerGen { get; set; }
        public static bool IsFileOutputOn { get; private set; }
        public static bool ShowPredTexture { get; set; }
        public static bool ShowPreyTexture { get; set; }
        public static Texture2D CircleTexture { get; private set; }
        #endregion

        #region Constructor
        static Utilities()
        {
            //Set initial values for variables

            //Environment
            ScreenWidth = 1200;
            ScreenHeight = 600;

            //Behaviour
            WanderRadius = 2.0f;
            WanderDistance = 10.0f;
            IsWanderOn = true;
            IsSightSeperationOn = true;
            IsLateralSeperationOn = true;
            IsSightSeperationPredatorOn = true;
            IsLateralSeperationPredatorOn = true;
            IsSightAlignmentOn = true;
            IsLateralAlignmentOn = true;
            IsSightCohesionOn = true;
            IsLateralCohesionOn = true;
            IsSightSeekFoodOn = true;
            IsLateralSeekFoodOn = true;


            //Agent
            SightRadiusPrey = 140f;
            SightRadiusPredator = 140f;
            LateralLinePrey = 20.0f;
            LateralLinePred = 20.0f;
            NumOfPreyOnScreen = 50;
            NumOfPredatorsOnScreen = 10;
            NumOfVegetationOnScreen = 0;
            AgentTextureSize = 8;
            AgentMaxTextureSize = 80;
            VegetationTextureSize = 8;
            MaxSpeed = 3.0f;
            MaxForce = 4.0f;
            Mass = 10.0f;
            AgentEnergy = 100;
            EnergyGainedFromVegetation = 30f;
            EnergyGainedFromPrey = 30f;

            //ANN
            NumOfInputsPrey = 6;
            NumOfOutputsPrey = 11;
            NumOfHiddenLayersPrey = 1;
            NumOfNeuronsPerHiddenPrey = 22;
            NumOfInputsPred = 4;
            NumOfOutputsPred = 9;
            NumOfHiddenLayersPred = 1;
            NumOfNeuronsPerHiddenPred = 18;
            ActivationResponse = 1.0f;
            Bias = -1;

            //GA
            MutationRate = 0.5f;
            MutationChance = 0.2f;
            MaxPerturbation = 1.0f; //2
            NumOfTourCompetitors = 20;
            PercentToSelect = 0.4f;
            GenerationNumPrey = 1;
            GenerationNumPred = 1;
            CrossoverProbability = 0.7f;
            IsRandomCrossoverPoint = false;
            CrossoverPoint = 0.5f;
            SizeOfPreyPool = 20;
            SizeOfPredPool = 10;

            //Evaluation
            IsLineAlignmentDrawingOn = false;
            IsLineCohesionDrawingOn = false;
            IsLineSeperationDrawingOn = false;
            IsLineSeekDrawingOn = false;
            IsChangingAgentSizeOn = false;
            IsSightRadiusVisible = false;
            IsLateralLineVisible = false;
            IsAgentColourVelocityOn = false;
            IsPaused = false;
            IsLookingForExpected = false;
            ExpectedWeights = new List<float>();
            BehaviourThreshold = 0.15f;
            NumOfBehavioursMatchingThreshold = 6;
            MeanDistancePreyFromPredPerGen = 0f;
            MeanDistancePreyFromPreyPerGen = 0f;
            IsFileOutputOn = false;
            ShowPredTexture = true;
            ShowPreyTexture = true;


        }
        #endregion

        #region Helper Methods

        public static void LoadTexture(Texture2D circle, Texture2D maxVelocity, Texture2D similarity)
        {
            CircleTexture = circle;
            MaxVelocityTexture = maxVelocity;
            SimilarityTexture = similarity;
        }

        //Used to initialise ANN with random weights and initial positiom of agents
        public static float RandomMinMax(double min, double max)
        {
            return (float) (random.NextDouble() * (max - min) + min);
        }

        //When agent moves off screen - moves them to opposite side
        public static void CheckOutOfBounds(MovingAgent agent)
        {

            if (agent.Position.X > ScreenWidth + agent.Size)
            {
                agent.ResetLines();
                //agent.Energy-=10;
                agent.PositionX = 0f;
                //agent.PositionX = ScreenWidth - agent.Size;
            }
            if (agent.Position.X < 0 - agent.Size)
            {
                agent.ResetLines();
               // agent.Energy-=10;
                agent.PositionX = ScreenWidth;
            }
            if (agent.Position.Y > ScreenHeight + agent.Size)
            {
                agent.ResetLines();
               // agent.Energy-=10;
                agent.PositionY = 0f;
            }
            if (agent.Position.Y < 0 - agent.Size)
            {
                agent.ResetLines();
                //agent.Energy-=10;
                agent.PositionY = ScreenHeight;
            }

        }

        #endregion

    }
}
