using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Simulation
{
    class InputManager
    {
        private KeyboardState keyboardState, oldKeyboardState;
        private MouseState mouseState, oldMouseState;

        public void HandleKeyboardInput()
        {
            
          
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyUp(Keys.D1) && oldKeyboardState.IsKeyDown(Keys.D1))
            {
                Utilities.IsLineAlignmentDrawingOn = !Utilities.IsLineAlignmentDrawingOn;
            }

            if (keyboardState.IsKeyUp(Keys.D2) && oldKeyboardState.IsKeyDown(Keys.D2))
            {
                Utilities.IsLineCohesionDrawingOn = !Utilities.IsLineCohesionDrawingOn;
            }

            if (keyboardState.IsKeyUp(Keys.D3) && oldKeyboardState.IsKeyDown(Keys.D3))
            {
                Utilities.IsLineSeperationDrawingOn = !Utilities.IsLineSeperationDrawingOn;
            }

            if (keyboardState.IsKeyUp(Keys.D4) && oldKeyboardState.IsKeyDown(Keys.D4))
            {
                Utilities.IsLineSeekDrawingOn = !Utilities.IsLineSeekDrawingOn;
            }

            if (keyboardState.IsKeyUp(Keys.D5) && oldKeyboardState.IsKeyDown(Keys.D5))
            {
                Utilities.IsChangingAgentSizeOn = !Utilities.IsChangingAgentSizeOn;
            }

            if (keyboardState.IsKeyUp(Keys.D6) && oldKeyboardState.IsKeyDown(Keys.D6))
            {
                Utilities.IsSightRadiusVisible = !Utilities.IsSightRadiusVisible;
                Utilities.IsLateralLineVisible = !Utilities.IsLateralLineVisible;
            }

            if (keyboardState.IsKeyUp(Keys.D7) && oldKeyboardState.IsKeyDown(Keys.D7))
            {
                Utilities.IsAgentColourVelocityOn = !Utilities.IsAgentColourVelocityOn;
            }


            if (keyboardState.IsKeyUp(Keys.LeftAlt) && oldKeyboardState.IsKeyDown(Keys.LeftAlt))
            {
                Utilities.ShowPreyTexture = !Utilities.ShowPreyTexture;
            }

  

            if (keyboardState.IsKeyUp(Keys.RightAlt) && oldKeyboardState.IsKeyDown(Keys.RightAlt))
            {
                Utilities.ShowPredTexture = !Utilities.ShowPredTexture;
            }

            if (keyboardState.IsKeyUp(Keys.D8) && oldKeyboardState.IsKeyDown(Keys.D8))
            {
                if(Utilities.NumOfVegetationOnScreen > 0)
                    Utilities.NumOfVegetationOnScreen--;
            }

            if (keyboardState.IsKeyUp(Keys.D9) && oldKeyboardState.IsKeyDown(Keys.D9))
            {
                Utilities.NumOfVegetationOnScreen++;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Utilities.SightRadiusPrey += 1;
                Utilities.SightRadiusPredator += 1;

            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Utilities.SightRadiusPrey -= 1;
                Utilities.SightRadiusPredator -= 1;

            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                Utilities.LateralLinePrey += 1;
                Utilities.LateralLinePred += 1;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Utilities.LateralLinePrey -= 1;
                Utilities.LateralLinePred -= 1;
            }

            oldKeyboardState = keyboardState;

        }

        public void HandePause()
        {
            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyUp(Keys.P) && oldKeyboardState.IsKeyDown(Keys.P))
            {
                Utilities.IsPaused = !Utilities.IsPaused;
            }
        }

        public void HandleMouseInput(List<MovingAgent> agents)
        {
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();


            if (oldMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                Point mousePosition = new Point(mouseState.X, mouseState.Y);

                for (int i = 0; i < agents.Count; i++)
                {
                    if (agents[i].CheckCollision(mousePosition))
                    {
                        agents[i].Texture = Utilities.SimilarityTexture;
                        Utilities.ExpectedWeights = agents[i].GetBehaviourWeights();
                        
                    }
                    else
                    {
                        agents[i].Texture = agents[i].DefaultTexture;
                    }

                }

                Utilities.IsPaused = false;
            }

        }

    }
}
