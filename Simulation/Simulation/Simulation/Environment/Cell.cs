using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    struct Cell
    {
        private LinkedList<Agent> membersOfCell = new LinkedList<Agent>();
        public LinkedList<Agent> MembersOfCell { get { return membersOfCell; } }

        //the cell's bounding box (it's inverted because the Window's default
        //co-ordinate system has a y axis that increases as it descends)
        private S boundingBox;

        public Cell(Vector2 topleft, Vector2 botright)
        {
            boundingBox = new BoundingBox(
        }
    }
}
