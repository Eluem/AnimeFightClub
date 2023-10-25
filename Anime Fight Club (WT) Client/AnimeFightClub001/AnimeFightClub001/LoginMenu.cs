//******************************************************
// File: LoginMenu.cs
//
// Purpose: Defines the login menu that players
// see after the splash screen, before they can access
// the main menu.
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
    class LoginMenu : Menu
    {
        #region Declarations
        bool m_connecting; //Used to as a flag to indicate that the client is currently trying to connect to the master server
        #endregion

        //****************************************************
        // Method: LoginMenu
        //
        // Purpose: Default constructor for LoginMenu
        //****************************************************
        public LoginMenu(Menu ownerItem, Game1 game1)
            : base(ownerItem, game1, "Login Menu", GameState.InitializeLoginMenu, GameState.LoginMenu, new Rectangle(0, 0, 800, 480), Color.White, Color.White, Color.White, Color.White, "Background001", "Background001", true)
        {
            Title = "Login";
            m_fontColor = Color.Black;
            m_fontColorFocus = Color.Black;

            MenuItem tempMenuItem;
            //Commented out to preserve the awesome color that schoenacher doesn't like
            //tempMenuItem = AddItem(new TextBox(this, game1, "LoginNameTextBox", ContentRect.Center.X - (300 / 2), 100, 300, 45, Color.Wheat, Color.Wheat, new Color(160, 118, 59), Color.NavajoWhite), new Point(0, 0), LoginNameTextBox_Clicked, null, null, LoginNameTextBox_Selected, LoginNameTextBox_Deselected);
            tempMenuItem = AddItem(new TextBox(this, game1, "LoginNameTextBox", ContentRect.Center.X - (300 / 2), 100, 300, 45, Color.Wheat, Color.Wheat, new Color(154, 113, 56), Color.NavajoWhite), new Point(0, 0), LoginNameTextBox_Clicked, null, null, LoginNameTextBox_Selected, LoginNameTextBox_Deselected);
            m_menuItems.Add(new List<MenuItem>());
            m_menuItems[1].Add(tempMenuItem); //1, 0

            //Commented out to preserve the awesome color that schoenacher doesn't like
            //tempMenuItem = AddItem(new PasswordTextBox(this, game1, "LoginPasswordTextBox", ContentRect.Center.X - (300 / 2), 155, 300, 45, Color.Wheat, Color.Wheat, new Color(160, 118, 59), Color.NavajoWhite, false), new Point(0, 1), Login, null, null, LoginPasswordTextBox_Selected, LoginPasswordTextBox_Deselected);
            tempMenuItem = AddItem(new PasswordTextBox(this, game1, "LoginPasswordTextBox", ContentRect.Center.X - (300 / 2), 155, 300, 45, Color.Wheat, Color.Wheat, new Color(154, 113, 56), Color.NavajoWhite, false), new Point(0, 1), Login, null, null, LoginPasswordTextBox_Selected, LoginPasswordTextBox_Deselected);
            m_menuItems[1].Add(tempMenuItem); //1, 1


            AddItem(new Button(this, game1, "LoginButton", 360, 220, 150, 68, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Login"), new Point(1, 2), Login);

            AddItem(new Button(this, game1, "RegisterButton", 200, 220, 150, 68, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Register"), new Point(0, 2), Register);

            tempMenuItem = AddItem(new Button(this, game1, "QuitButton", ContentRect.Center.X - (150 / 2), 298, 150, 68, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Quit"), new Point(0, 3), Quit);
            m_menuItems[1].Add(tempMenuItem); //1, 3
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Listens for success/failed connection
        // to Master Server. As well as runs normal updating
        // code.
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (m_connecting)
            {
                if (NetworkingHandler.MasterServerConnection != null && NetworkingHandler.MasterServerConnection.Status == NetConnectionStatus.Connected)
                {
                    m_connecting = false;
                    GlobalVariables.UserName = ((TextBox)FindItem("LoginNameTextBox")).Text;
                    NetworkingHandler.QueryAbilityList();
                    
                    m_game1.currGameState = GameState.InitializeMainMenu;
                }
                else if (NetworkingHandler.MasterServerConnectionState != "" && NetworkingHandler.MasterServerConnectionState != "User Request")
                {
                    m_messageBoxList[0].ClosePopUp(this);
                    PopUpMessage(this, "ErrorPopUpMessage", NetworkingHandler.MasterServerConnectionState, messageBoxType.ok);

                    NetworkingHandler.MasterServerConnectionState = ""; //Clear state so next time this happens it can be refreshed
                    m_connecting = false;
                }
            }
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws all the content
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial12"], "Username", new Vector2((int)FindItem("LoginNameTextBox").X - GlobalVariables.FontDict["Arial12"].MeasureString("Username").X - 8, (int)FindItem("LoginNameTextBox").Bounds.Center.Y - GlobalVariables.FontDict["Arial12"].MeasureString("Username").Y / 2), new Color(75, 45, 16));
            spriteBatch.DrawString(GlobalVariables.FontDict["Arial12"], "Password", new Vector2((int)FindItem("LoginPasswordTextBox").X - GlobalVariables.FontDict["Arial12"].MeasureString("Password").X - 8, (int)FindItem("LoginPasswordTextBox").Bounds.Center.Y - GlobalVariables.FontDict["Arial12"].MeasureString("Password").Y / 2), new Color(75, 45, 16));
            spriteBatch.End();

            int i = 0;
            int j;
            foreach (List<MenuItem> col in m_menuItems)
            {
                j = 0;
                foreach (MenuItem menuItem in col)
                {
                    if (!((i == 1 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == 3)))
                        menuItem.Draw(spriteBatch, graphicsDevice, offset);
                    ++j;
                }
                ++i;
            }
        }


        #region Menu Item Event Handlers
        #region Login Name TextBox
        //****************************************************
        // Method: LoginNameTextBox_Selected
        //
        // Purpose: When the textbox is clicked, the menu
        // goes down
        //****************************************************
        public void LoginNameTextBox_Clicked(object sender)
        {
            Down();
        }

        //****************************************************
        // Method: LoginNameTextBox_Selected
        //
        // Purpose: When the textbox is selected, it gets
        // to share focus.
        //****************************************************
        public void LoginNameTextBox_Selected(object sender)
        {
            FindItem("LoginNameTextBox").GainFocus(sender);
        }

        //****************************************************
        // Method: LoginNameTextBox_Deselected
        //
        // Purpose: When the textbox is deselected, it loses
        // focus
        //****************************************************
        public void LoginNameTextBox_Deselected(object sender)
        {
            FindItem("LoginNameTextBox").LoseFocus(sender);
        }
        #endregion

        #region Login Name TextBox
        //****************************************************
        // Method: LoginNameTextBox_Selected
        //
        // Purpose: When the textbox is selected, it gets
        // to share focus.
        //****************************************************
        public void LoginPasswordTextBox_Selected(object sender)
        {
            FindItem("LoginPasswordTextBox").GainFocus(sender);
        }

        //****************************************************
        // Method: LoginNameTextBox_Deselected
        //
        // Purpose: When the textbox is deselected, it loses
        // focus
        //****************************************************
        public void LoginPasswordTextBox_Deselected(object sender)
        {
            FindItem("LoginPasswordTextBox").LoseFocus(sender);
        }
        #endregion

        //****************************************************
        // Method: Login
        //
        // Purpose: Event handler that fires when the user
        // "clicks" on the Login button or the password
        // textbox
        //****************************************************
        public void Login(object sender)
        {
            if (((TextBox)FindItem("LoginNameTextBox")).Text.Length > 0 && ((TextBox)FindItem("LoginPasswordTextBox")).Text.Length > 0)
            {
                PopUpMessage(this, "ConnectingToServer", "Connecting to master server...", messageBoxType.cancel, CancelConnect);
                NetworkingHandler.ConnectToMasterServer(((TextBox)FindItem("LoginNameTextBox")).Text, ((TextBox)FindItem("LoginPasswordTextBox")).Text);
                m_connecting = true;
            }
            else
            {
                PopUpMessage(this, "LoginErrorMessageBox1", "You must enter a valid user name and password!", messageBoxType.ok);
            }
        }

        //****************************************************
        // Method: Register
        //
        // Purpose: Event handler that fires when the user
        // "clicks" on the Register button. It navigates
        // to the registerMenu.
        //****************************************************
        public void Register(object sender)
        {
            m_game1.currGameState = GameState.InitializeRegistrationMenu;
            /*
            NetworkingHandler.ConnectToServer("127.0.0.1", 37337);
            bool connected = false;
            bool failedToConnect = false;
            while (!connected && !failedToConnect)
            {
                NetworkingHandler.WorldInit(ref connected, ref failedToConnect);
                if (connected)
                {
                    m_game1.currGameState = GameState.InitializeGame;
                }
                else if (failedToConnect)
                {
                    m_connecting = false;
                    PopUpMessage(this, "Test", "Failed", messageBoxType.ok);
                }
            }
            */
        }

        //****************************************************
        // Method: CancelConnect
        //
        // Purpose: Cancels the connection
        //****************************************************
        public void CancelConnect(object sender)
        {
            NetworkingHandler.MasterServerDisconnect();
            NetworkingHandler.MasterServerConnectionState = ""; //Clear state so next time this happens it can be refreshed
            m_connecting = false;
            SelectItem(m_currSelection.X, m_currSelection.Y);
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
