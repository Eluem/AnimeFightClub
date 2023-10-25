//******************************************************
// File: MainMenu.cs
//
// Purpose: Defines the main menu that players use to
// navigate to other menus from.
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
    class MainMenu : Menu
    {
        #region Declarations
        #endregion

        //****************************************************
        // Method: MainMenu
        //
        // Purpose: Default constructor for MainMenu
        //****************************************************
        public MainMenu(Menu ownerItem, Game1 game1)
            : base(ownerItem, game1, "Main Menu", GameState.InitializeMainMenu, GameState.MainMenu, new Rectangle(0, 0, 800, 480), Color.White, Color.White, Color.White, Color.White, "Background001", "Background001", true)
        {
            Title = "Main Menu";
            m_fontColor = Color.Black;
            m_fontColorFocus = Color.Black;


            AddItem(new Button(this, game1, "SelectServerButton", ContentRect.Center.X - 125, 80, 250, 65, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Select Server"), new Point(0, 0), SelectServer);
            AddItem(new Button(this, game1, "CustomizeButton", ContentRect.Center.X - 125, 145, 250, 65, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Customize"), new Point(0, 1), Customize);
            AddItem(new Button(this, game1, "QuitButton", ContentRect.Center.X - 125, 210, 250, 65, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Quit"), new Point(0, 2), Quit);
        }

        #region Menu Item Event Handlers
        //****************************************************
        // Method: SelectServer
        //
        // Purpose: Event handler that fires when the user
        // "clicks" on the Select Server button
        //****************************************************
        public void SelectServer(object sender)
        {
            m_game1.currGameState = GameState.InitializeServerListMenu;
        }

        //****************************************************
        // Method: Customize
        //
        // Purpose: Event handler that fires when the user
        // "clicks" on the Customize button
        //****************************************************
        public void Customize(object sender)
        {
            if (GlobalVariables.AbilityDict.Count > 0 && GlobalVariables.CharLoadoutPopulated)
            {
                NetworkingHandler.QueryCurrentExp();
                m_game1.currGameState = GameState.InitializeCustomizationMenu;
            }
            else
            {
                PopUpMessage(this, "CustomizeMenuAccessError", "You cannot access the Customization Menu yet, still downloading list of abilities!", messageBoxType.ok);
            }
        }

        //****************************************************
        // Method: Exit
        //
        // Purpose: Exits the game
        //****************************************************
        public void Quit(object sender)
        {
            m_game1.Exit();
        }

        #endregion

        #region Properties
        #endregion
    }
}
