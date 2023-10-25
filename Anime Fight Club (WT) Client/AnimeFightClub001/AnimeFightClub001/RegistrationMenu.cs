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
    class RegistrationMenu : Menu
    {
        #region Declarations
        bool m_connecting; //Used to as a flag to indicate that the client is currently trying to connect to the master server
        #endregion

        //****************************************************
        // Method: RegistrationMenu
        //
        // Purpose: Default constructor for RegistrationMenu
        //****************************************************
        public RegistrationMenu(Menu ownerItem, Game1 game1)
            : base(ownerItem, game1, "Registration Menu", GameState.InitializeLoginMenu, GameState.LoginMenu, new Rectangle(0, 0, 800, 480), Color.White, Color.White, Color.White, Color.White, "Background001", "Background001", true)
        {
            Title = "Registration";
            m_fontColor = Color.Black;
            m_fontColorFocus = Color.Black;


            AddItem(new TextBox(this, game1, "LoginNameTextBox", ContentRect.Center.X - (300 / 2), 100, 300, 45, Color.Wheat, Color.Wheat, new Color(160, 118, 59), Color.NavajoWhite), new Point(0, 0), LoginNameTextBox_Clicked, null, null, LoginNameTextBox_Selected, LoginNameTextBox_Deselected);

            AddItem(new PasswordTextBox(this, game1, "LoginPasswordTextBox", ContentRect.Center.X - (300 / 2), 155, 300, 45, Color.Wheat, Color.Wheat, new Color(160, 118, 59), Color.NavajoWhite, false), new Point(0, 1), Accept, null, null, LoginPasswordTextBox_Selected, LoginPasswordTextBox_Deselected);



            AddItem(new Button(this, game1, "AcceptButton", 200, 220, 150, 68, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Accept"), new Point(0, 2), Accept);

            AddItem(new Button(this, game1, "BackButton", 360, 220, 150, 68, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Back"), new Point(1, 2), GoBack);
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

            base.DrawContent(spriteBatch, graphicsDevice, offset);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Handles updating anything inside the
        // server list
        //****************************************************
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            string response = null; //Response from the master server
            NetworkingHandler.RegisterNewAccountListener(ref response);

            if (response != null)
            {
                FindMessagebox("RegistrationRequest").ClosePopUp(this);
                if (response == "Account successfully registered!")
                    PopUpMessage(this, "RegistrationRequestResponse", response, messageBoxType.ok, GoBack);
                else
                    PopUpMessage(this, "RegistrationRequestResponse", response, messageBoxType.ok);
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
        // Method: Accept
        //
        // Purpose: Event handler for Accept button. Creates
        // the account
        //****************************************************
        public void Accept(object sender)
        {
            PopUpMessage(this, "RegistrationRequest", "Registering account...", messageBoxType.locked);
            NetworkingHandler.RegisterNewAccount(((TextBox)FindItem("LoginNameTextBox")).Text, ((TextBox)FindItem("LoginPasswordTextBox")).Text);
        }

        //****************************************************
        // Method: GoBack
        //
        // Purpose: Event handler for BackButton
        //****************************************************
        public void GoBack(object sender)
        {
            Back();
        }
        #endregion

        #region Properties
        #endregion
    }
}
