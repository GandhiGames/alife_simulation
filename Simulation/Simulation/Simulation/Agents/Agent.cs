using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class Agent
    {
        #region Fields and Properties
        private Texture2D defaultTexture;
        public Texture2D DefaultTexture { get { return defaultTexture; } }
        private Texture2D texture; //Image of agent
        public Texture2D Texture { get { return texture; } set { texture = value; } }

        private Color textureColour;
        public Color TextureColour { get { return textureColour; } set { textureColour = value; } }

        //Position of agent
        protected Vector2 position;
        public Vector2 Position 
        { 
            get 
            {
                return position + imageCenter;
            }

            set
            {
                position = value;
            }
        }

        //Radius used for collision
        public float Radius { get { return Size / 2; } }

        //Used to decide wether to display/update object
        private Boolean isAlive;
        public Boolean IsAlive { get { return isAlive; } set { isAlive = value; } }

        //Energy of agent
        protected float energy;
        public float Energy { get { return energy; } set { energy = value; } }

        //Size of texture depends on the energy level of agent
        //Used as for analysing visually
        private int size;
        public int Size { get { return size; } set { size = value; } }

        protected Vector2 imageCenter;
        #endregion

        #region Constructor
        public Agent(Texture2D texture, Vector2 position, int size)
        {
            textureColour = Color.White;
            this.texture = texture;
            defaultTexture = texture;
            this.position = position;
            this.size = size;
            //radius = size / 2;
            isAlive = true;
            imageCenter = new Vector2(size / 2f, size / 2f);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            imageCenter = new Vector2(size / 2f, size / 2f);

       
                spriteBatch.Draw(texture,
                    new Rectangle((int)position.X, (int)position.Y, size, size), null, textureColour,
                    0f, imageCenter, SpriteEffects.None, 0f);
     
        }
        #endregion

        #region Helper Methods
        //Checks for collision
        public virtual Boolean CheckCollision(Agent otherAgent)
        {
            float dx = Position.X - otherAgent.Position.X;
            float dy = Position.Y - otherAgent.Position.Y;
            float radii = otherAgent.Radius + Radius;

            if ((dx * dx) + (dy * dy) < radii * radii)
            {
          
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean CheckCollision(Point p)
        {
            float dx = Position.X - p.X;
            float dy = Position.Y - p.Y;

            if ((dx * dx) + (dy * dy) < Radius * Radius)
            {
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
