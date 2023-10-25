//******************************************************
// File: MatchResults.cs
//
// Purpose: Defines the screen that players see
// between matches
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
    class MatchResults : Menu
    {
        #region Declarations
        protected double m_nextRoundTimer;
        #endregion

        //****************************************************
        // Method: MatchResults
        //
        // Purpose: Default constructor for MatchResults
        //****************************************************
        public MatchResults(Menu ownerItem, Game1 game1)
            : base(ownerItem, game1, "Match Results", GameState.InitializeMainMenu, GameState.MainMenu, new Rectangle(0, 0, game1.GraphicsDevice.DisplayMode.Width, game1.GraphicsDevice.DisplayMode.Height), Color.White, Color.White, Color.White, Color.White, "Background001", "Background001", true)
        {
            Title = "Match Results";
            m_fontColor = Color.Black;
            m_fontColorFocus = Color.Black;

            m_nextRoundTimer = 10;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Handles updating anything inside the
        // customization menu 
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            m_nextRoundTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (m_nextRoundTimer < 0)
                m_nextRoundTimer = 0;
        }

        //****************************************************
        // Method: Initialize
        //
        // Purpose: Initializes the menu
        //****************************************************
        public override void Initialize()
        {
            m_nextRoundTimer = 10;
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws all the content
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial20"], "Winner", new Vector2(ContentRect.Center.X - GlobalVariables.FontDict["Arial20"].MeasureString("Winner").X / 2, ContentRect.Top + 100), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial20"], NetworkingHandler.WinnerName + " with " + NetworkingHandler.WinnerScore + " Kills", new Vector2(ContentRect.Center.X - GlobalVariables.FontDict["Arial20"].MeasureString(NetworkingHandler.WinnerName + " with " + NetworkingHandler.WinnerScore + " Kills").X / 2, ContentRect.Top + 150), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial20"], "Your Kills: " + GameObjectHandler.LocalPlayer.Kills, new Vector2(ContentRect.Center.X - GlobalVariables.FontDict["Arial20"].MeasureString("Your Kills: " + GameObjectHandler.LocalPlayer.Kills).X / 2, ContentRect.Top + 300), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial20"], "Your Exp: " + (GameObjectHandler.LocalPlayer.Kills * 100), new Vector2(ContentRect.Center.X - GlobalVariables.FontDict["Arial20"].MeasureString("Your Exp: " + (GameObjectHandler.LocalPlayer.Kills * 100)).X / 2, ContentRect.Top + 325), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial20"], "Next Match In", new Vector2(ContentRect.Center.X - GlobalVariables.FontDict["Arial20"].MeasureString("Next Match In").X / 2, ContentRect.Top + 500), Color.Black);
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial20"], Math.Round(m_nextRoundTimer).ToString(), new Vector2(ContentRect.Center.X - GlobalVariables.FontDict["Arial20"].MeasureString(Math.Round(m_nextRoundTimer).ToString()).X / 2, ContentRect.Top + 525), Color.Black);
            spriteBatch.End();
        }

        #region Properties
        #endregion
    }
}
