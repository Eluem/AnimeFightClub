//******************************************************
// File: SplashScreen.cs
//
// Purpose: Splash screen
//
// Written By: Salvatore Hanusiewicz
//******************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;
namespace AnimeFightClub001
{
    class SplashScreen : Menu
    {
        #region Declarations
        protected double m_splashScreenTimer;
        #endregion

        //****************************************************
        // Method: SplashScreen
        //
        // Purpose: Default constructor for SplashScreen
        //****************************************************
        public SplashScreen(Menu ownerItem, Game1 game1)
            : base(ownerItem, game1, "Splash Screen", GameState.InitializeMainMenu, GameState.MainMenu, new Rectangle(0, 0, 800, 480), Color.White, Color.White, Color.White, Color.White, "Background001", "Background001", true)
        {
            m_splashScreenTimer = 10;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Handles updating anything inside the
        // customization menu 
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            m_splashScreenTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (m_splashScreenTimer < 0)
                m_game1.currGameState = GameState.InitializeLoginMenu;
        }

        //****************************************************
        // Method: HandleInputHook
        //
        // Purpose: Handles any input from player one only.
        //****************************************************
        public override void HandleInputHook(Controller playerOneController)
        {
            base.HandleInputHook(playerOneController);
            bool keyPressed = false;
            foreach(bool temp in playerOneController.ControllerState.ControlArray)
            {
                if (temp)
                {
                    keyPressed = true;
                    break;
                }
            }
            if (keyPressed)
            {
                m_splashScreenTimer = -1;
            }

        }

        //****************************************************
        // Method: Initialize
        //
        // Purpose: Initializes the menu
        //****************************************************
        public override void Initialize()
        {
            m_splashScreenTimer = 10;
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws all the content
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(GlobalVariables.ImageDict["Title"].Texture, new Rectangle(graphicsDevice.Viewport.Bounds.Center.X - GlobalVariables.ImageDict["Title"].Width / 2 - GlobalVariables.ImageDict["Background001_L"].Width, graphicsDevice.Viewport.Bounds.Center.Y - GlobalVariables.ImageDict["Title"].Height / 2, GlobalVariables.ImageDict["Title"].Width, GlobalVariables.ImageDict["Title"].Height), Color.Black);
            spriteBatch.End();
        }

        #region Properties
        #endregion
    }
}
