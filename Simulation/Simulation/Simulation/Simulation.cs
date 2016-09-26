using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Simulation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Simulation : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D lineTexture, circleTexture;

        private InputManager inputManager;

        private SpriteFont font;

        private List<Prey> prey;
        private List<Agent> vegetation;
        private List<Predator> predators;

        private GA_Prey preyGa;
        private GA_Pred predatorGA;

        public Simulation()
        {
            graphics = new GraphicsDeviceManager(this);

            //Set width and height of window
            graphics.PreferredBackBufferHeight = Utilities.ScreenHeight;
            graphics.PreferredBackBufferWidth = Utilities.ScreenWidth;

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            inputManager = new InputManager();
            prey = new List<Prey>();
            vegetation = new List<Agent>();
            predators = new List<Predator>();

            Texture2D preyTexture = Content.Load<Texture2D>("Sprites//Sprite - Prey");
            Texture2D vegTexture = Content.Load<Texture2D>("Sprites//Sprite - Vegetation");
            Texture2D predTexture = Content.Load<Texture2D>("Sprites//Sprite - Predator");

            Utilities.LoadTexture(Content.Load<Texture2D>("Sprites//Sprite - Circle"),
                Content.Load<Texture2D>("Sprites//Sprite - MaxVelocity"),
                Content.Load<Texture2D>("Sprites//Sprite - Similar"));

            preyGa = new GA_Prey(preyTexture, vegTexture);
            predatorGA = new GA_Pred(predTexture);

            for (int i = 0; i < Utilities.NumOfPreyOnScreen; i++)
            {
                int preySize = Utilities.AgentTextureSize;

                int tempX = (int)Utilities.RandomMinMax(0 + preySize, Utilities.ScreenWidth - preySize);
                int tempY = (int)Utilities.RandomMinMax(0 + preySize, Utilities.ScreenHeight - preySize);

                prey.Add(new Prey(preyTexture, new Vector2(tempX, tempY),
                    Utilities.AgentTextureSize, Utilities.Mass, Utilities.SightRadiusPrey,
                    Utilities.LateralLinePrey));


            }

            for (int i = 0; i < Utilities.NumOfPredatorsOnScreen; i++)
            {
                int predSize = Utilities.AgentTextureSize;

                int tempX = (int)Utilities.RandomMinMax(0 + predSize, Utilities.ScreenWidth - predSize);
                int tempY = (int)Utilities.RandomMinMax(0 + predSize, Utilities.ScreenHeight - predSize);

                predators.Add(new Predator(predTexture, new Vector2(tempX, tempY),
                    Utilities.AgentTextureSize, Utilities.Mass, Utilities.SightRadiusPredator,
                    Utilities.LateralLinePred));


            }


            for (int i = 0; i < Utilities.NumOfVegetationOnScreen; i++)
            {
                int vegSize = Utilities.VegetationTextureSize;

                int tempX = (int)Utilities.RandomMinMax(0 + vegSize, Utilities.ScreenWidth - vegSize);
                int tempY = (int)Utilities.RandomMinMax(0 + vegSize, Utilities.ScreenHeight - vegSize);

                vegetation.Add(new Agent(vegTexture, new Vector2(tempX, tempY),
                    Utilities.VegetationTextureSize));
            }

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("spriteFont");

            lineTexture = new Texture2D(GraphicsDevice, 1, 1);
            lineTexture.SetData<Color>(new[] { Color.White });

            circleTexture = Content.Load<Texture2D>("Sprites\\Sprite - Circle");

           


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            inputManager.HandePause();

            if (!Utilities.IsPaused)
            {
                inputManager.HandleKeyboardInput();

                if(Utilities.NumOfPreyOnScreen > 0)
                    preyGa.Update(gameTime, ref prey, ref vegetation, ref predators);

                if(Utilities.NumOfPredatorsOnScreen > 0)
                    predatorGA.Update(gameTime, ref prey, ref predators);

                base.Update(gameTime);
            }
            else
            {
                List<MovingAgent> preyTemp = new List<MovingAgent>();
                preyTemp.AddRange(prey);
                inputManager.HandleMouseInput(preyTemp);
            }
        }

       


        


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.GhostWhite);

            //int size = Utilities.AgentTextureSize;
            //int vegSize = Utilities.VegetationTextureSize;

            spriteBatch.Begin();




            for (int i = 0; i < prey.Count; i++)
            {
                if (prey[i].IsAlive)
                {
                    prey[i].Draw(spriteBatch);

                    DrawBehaviourLines(prey[i]);
                    DrawRadiusCircles(prey[i]);

                }
            }


            for (int i = 0; i < predators.Count; i++)
            {
                if (predators[i].IsAlive)
                {
                    predators[i].Draw(spriteBatch);
                    DrawBehaviourLines(predators[i]);
                    DrawRadiusCircles(predators[i]);
                }
            }

            for (int i = 0; i < vegetation.Count; i++)
            {
                vegetation[i].Draw(spriteBatch);
            }

            spriteBatch.DrawString(font, "Prey Generation Number: " + Utilities.GenerationNumPrey, new Vector2(5, 5), Color.Black);
            spriteBatch.DrawString(font, "Pred Generation Number: " + Utilities.GenerationNumPred, new Vector2(5, 25), Color.Black);
    

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Helper Methods

        private void DrawRadiusCircles(MovingAgent agent)
        {
            if (Utilities.IsSightRadiusVisible)
            {
                spriteBatch.Draw(circleTexture, new Rectangle((int)(agent.Position.X - (agent.SightRadius / 2)),
                    (int)(agent.Position.Y - (agent.SightRadius / 2)),
                    (int)agent.SightRadius, (int)agent.SightRadius), Color.Red);
            }

            if (Utilities.IsLateralLineVisible)
            {
                spriteBatch.Draw(circleTexture, new Rectangle((int)(agent.Position.X - (agent.LateralRadius / 2)),
                 (int)(agent.Position.Y - (agent.LateralRadius / 2)),
                 (int)agent.LateralRadius, (int)agent.LateralRadius), Color.White);
            }
        }

        private void DrawBehaviourLines(MovingAgent agent)
        {
            if (Utilities.IsLineCohesionDrawingOn)
            {
                if (agent.GetCohesionLine() != Vector2.Zero)
                {
                    if (agent.GetCohesionWeight() > 0 && agent.GetCohesionWeight() < 0.2)
                        DrawLine(0.2f, Color.Blue, agent.Position, agent.GetCohesionLine());
                    else
                        DrawLine(agent.GetCohesionWeight() + 0.2f, Color.Blue, agent.Position, agent.GetCohesionLine());
                }
            }

            if (Utilities.IsLineAlignmentDrawingOn)
            {

                if (agent.GetAlignmentLine() != Vector2.Zero)
                {
                    if (agent.GetAlignmentWeight() > 0 && agent.GetAlignmentWeight() < 0.2)
                        DrawLine(0.2f, Color.Green, agent.Position, agent.GetAlignmentLine());
                    else
                        DrawLine(agent.GetAlignmentWeight(), Color.Green, agent.Position, agent.GetAlignmentLine());
                }
            }

            if (Utilities.IsLineSeperationDrawingOn)
            {

                if (agent.GetSeperationLine() != Vector2.Zero)
                {
                    if (agent.GetSeperationWeight() > 0 && agent.GetSeperationWeight() < 0.2)
                        DrawLine(0.2f, Color.Red, agent.Position, agent.GetSeperationLine());
                    else
                        DrawLine(agent.GetSeperationWeight(), Color.Red, agent.Position, agent.GetSeperationLine());
                }
            }

            if (Utilities.IsLineSeekDrawingOn)
            {
                if (agent.GetSeekLine() != Vector2.Zero)
                {
                    if (agent.GetSeekWeight() > 0 && agent.GetSeekWeight() < 0.2)
                        DrawLine(0.2f, Color.Yellow, agent.Position, agent.GetSeekLine());
                    else
                        DrawLine(agent.GetAlignmentWeight(), Color.Yellow, agent.Position, agent.GetSeekLine());
                }
            }

        }

        private void DrawLine(float width, Color color, Vector2 firstPoint, Vector2 secondPoint)
        {
            float angle = (float)Math.Atan2(secondPoint.Y - firstPoint.Y, secondPoint.X - firstPoint.X);
            float length = Vector2.Distance(firstPoint, secondPoint);

            spriteBatch.Draw(lineTexture, firstPoint, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);

        }
        #endregion


    }

}
