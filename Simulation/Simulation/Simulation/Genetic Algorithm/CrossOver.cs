using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simulation
{
    interface CrossOver
    {
        MovingAgent TournamentSelection();

        NeuralNet CrossOver(MovingAgent parentOne, MovingAgent parentTwo);

        MovingAgent FitnessProportionateSelection();

      


    }
}
