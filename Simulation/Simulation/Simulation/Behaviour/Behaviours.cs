using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class Behaviours
    {
        #region Fields
        private MovingAgent agent; //The agent behaviour whose behaviour is being set
        private Random rand;

        private Vector2 wanderTarget; //Target for wander behaviour

        private Vector2 cohesionLine;
        public Vector2 CohesionLine { get { return cohesionLine; } set { cohesionLine = value; } }

        private Vector2 seperationLine;
        public Vector2 SeperationLine { get { return seperationLine; } set { seperationLine = value; } }

        private Vector2 alignmentLine;
        public Vector2 AlignmentLine { get { return alignmentLine; } set { alignmentLine = value; } }

        private Vector2 seekLine;
        public Vector2 SeekLine { get { return seekLine; } set { seekLine = value; } }


        #endregion

        #region Constructor
        public Behaviours(MovingAgent agent)
        {
            cohesionLine = Vector2.Zero;
            seperationLine = Vector2.Zero;
            alignmentLine = Vector2.Zero;
            seekLine = Vector2.Zero;

            this.agent = agent;
            rand = Utilities.Random;

        }
        #endregion


        #region Behaviour Methods

        public Vector2 Wander()
        {
            wanderTarget += new Vector2((float)(rand.NextDouble() - rand.NextDouble()),
                    (float)(rand.NextDouble() - rand.NextDouble()));

            wanderTarget.Normalize();

            wanderTarget *= Utilities.WanderRadius;

            Vector2 displacement = Vector2.Normalize(agent.Heading) * Utilities.WanderRadius;

            Vector2 targetPosition = agent.Position + wanderTarget + displacement;

            return targetPosition - agent.Position;

        }

        public Vector2 Seperation(List<MovingAgent> agents)
        {
            Vector2 steeringForce = Vector2.Zero;
            Vector2 steeringLine = Vector2.Zero;


            seperationLine = Vector2.Zero;


            for (int i = 0; i < agents.Count; i++)
            {


                Vector2 toAgent = agent.Position - agents[i].Position;

               steeringForce += Vector2.Normalize(toAgent) / (toAgent.Length() * 0.5f);

                //steeringForce += Vector2.Normalize(toAgent) / toAgent.Length();

               // seperationLine = agent.Position + (Vector2.Normalize(toAgent) * toAgent.Length());


            }

            seperationLine = agent.Position + (steeringForce * 45f);






            return steeringForce;
        }

        public Vector2 Alignment(List<MovingAgent> agents)
        {
            Vector2 averageHeading = Vector2.Zero;
            int neighbourCount = 0;
            alignmentLine = Vector2.Zero;
            //Vector2 steeringLine = Vector2.Zero;

            for (int i = 0; i < agents.Count; i++)
            {
                averageHeading += agents[i].Heading;
                //alignmentLine = agents[i].Position + agents[i].Heading;
                neighbourCount++;

            }

            /* for (int i = 0; i < agentsInSight.Count; i++)
             {
                //if (!agents[i].Equals(agent))
                //{
                     //Vector2 to = agents[i].Position - agent.Position;
                     //float range = agent.SightRadius + agents[i].SightRadius;

                     //float temp = to.LengthSquared();

                     //if (temp < (range * range))
                     //{
                 averageHeading += agentsInSight[i].Heading;
                 neighbourCount++;
                     //}
               // }
             }*/


            if (neighbourCount > 0)
            {
             
                averageHeading /= (float)neighbourCount;
                alignmentLine = agent.Position + (averageHeading * 35f);
          
                averageHeading -= agent.Heading;


            }



            return averageHeading;
        }

        public Vector2 Cohesion(List<MovingAgent> agents)
        {
            Vector2 steeringForce = Vector2.Zero;
            Vector2 centreOfMass = Vector2.Zero;
            int neighbourCount = 0;
            cohesionLine = Vector2.Zero;

            for (int i = 0; i < agents.Count; i++)
            {
                centreOfMass += agents[i].Position;
                neighbourCount++;

            }



            /*for (int i = 0; i < agentsInSight.Count; i++)
             {
                //if (!agents[i].Equals(agent))
                //{

                     //Vector2 to = agents[i].Position - agent.Position;
                     //float range = agent.SightRadius + agents[i].SightRadius;

                     //float temp = to.LengthSquared();

                     //if (temp < (range * range))
                     //{
                 centreOfMass += agentsInSight[i].Position;
                 neighbourCount++;
                     //}
               // }
             }*/



            if (neighbourCount > 0)
            {
                centreOfMass /= (float)neighbourCount;

                cohesionLine = centreOfMass;

                steeringForce = Seek(centreOfMass);

            }



            return steeringForce;

        }

        public Vector2 Seek(List<Agent> vegetation)
        {

            int seek = -1;
            Vector2 distanceTo = Vector2.Zero;

            for (int i = 0; i < vegetation.Count; i++)
            {
                if (i == 0)
                {
                    distanceTo = vegetation[i].Position - agent.Position;
                    seek = i;
                }
                else
                {
                    Vector2 to = vegetation[i].Position - agent.Position;
                    float temp = to.LengthSquared();

                    if (temp < distanceTo.LengthSquared())
                    {
                        distanceTo = vegetation[i].Position - agent.Position;
                        seek = i;
                    }
                }

            }

            if (seek >= 0)
            {
                seekLine = vegetation[seek].Position;
                return Seek(vegetation[seek].Position);
            }
            else
            {
                seekLine = Vector2.Zero;
                return Vector2.Zero;
            }

        }


        public Vector2 Seek(List<MovingAgent> agents)
        {

            int seek = -1;
            Vector2 distanceTo = Vector2.Zero;

            for (int i = 0; i < agents.Count; i++)
            {
                if (i == 0)
                {
                    distanceTo = agents[i].Position - agent.Position;
                    seek = i;
                }
                else
                {
                    Vector2 to = agents[i].Position - agent.Position;
                    float temp = to.LengthSquared();

                    if (temp < distanceTo.LengthSquared())
                    {
                        distanceTo = agents[i].Position - agent.Position;
                        seek = i;
                    }
                }

            }

            if (seek >= 0)
            {
                seekLine = agents[seek].Position;
                return Seek(agents[seek].Position);
            }
            else
            {
                seekLine = Vector2.Zero;
                return Vector2.Zero;
            }

        }

        public Vector2 Seek(Vector2 targetPosition)
        {
            Vector2 desiredVelocity = Vector2.Normalize(targetPosition - agent.Position)
                * Utilities.MaxSpeed;
            Vector2 force = desiredVelocity - agent.Velocity;
            return force;
        }
        #endregion

        #region Helper Methods

        #endregion
    }
}
