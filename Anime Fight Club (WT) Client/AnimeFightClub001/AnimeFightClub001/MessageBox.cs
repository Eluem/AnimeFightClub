//******************************************************
// File: MessageBox.cs
//
// Purpose: Defines a basic MessageBox object which is
// used in menus to display messages and hooks into
// their buttons.
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
    enum messageBoxType { ok, cancel, okCancel, yesNo, locked };
    class MessageBox : Menu
    {
        #region Declarations
        protected Menu m_actualOwnerItem;
        protected TextBox m_textBox; //Used for the textbox which can't be focused
        #endregion

        //****************************************************
        // Method: MessageBox
        //
        // Purpose: Constructor for MessageBox
        //****************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawner">The item that should recieve focus when this messagebox closes.</param>
        /// <param name="game1"></param>
        /// <param name="ownerItem">The menu that spawned this messagebox.</param>
        /// <param name="name"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="firstButtonClick"></param>
        /// <param name="secondButtonClick"></param>
        public MessageBox(Menu spawner, Game1 game1, Menu ownerItem, string name, int X, int Y, string message, messageBoxType type, BasicMenuItemEventHandler firstButtonClick = null, BasicMenuItemEventHandler secondButtonClick = null)
            : base(spawner, game1, name, ownerItem.InitState, ownerItem.MenuState, new Rectangle(X,Y,300,0), Color.Wheat, Color.Wheat, Color.Wheat, Color.Wheat, "TextBox", "TextBox", false, false)
        {

            m_actualOwnerItem = ownerItem;


            m_innerShadowSize = new int[4] {9,9,9,9};

            //message = "This is a really long statement that will be used to test my multiline popup thingy This is a really long statement that will be used to test my multiline popup thingy";

            #region Create Textbox
            m_fontColor = Color.Black;
            m_fontColorFocus = Color.Black;

            m_textBox = new TextBox(this, game1, "MessageTextBox", 10, 10, ContentRect.Width - 20, 40, Color.Transparent, Color.Transparent, Color.Black, Color.Black, "Arial18", message, true, "None", "None");
            m_textBox.Height = (int)Math.Round(GlobalVariables.FontDict[m_textBox.Font].MeasureString(m_textBox.Text).Y);
            //m_textBox.InnerShadowSize = new int[4] { 5, 5, 0, 0 };
            #endregion

            #region Determine Size
            m_rect.Width = 300;
            m_rect.Height = m_textBox.Height + 90;
            #endregion

            #region Determine Message Box Type
            switch (type)
            {
                case messageBoxType.cancel:
                    AddItem(new Button(this, m_game1, "CancelButton", ContentRect.Center.X - 60, ContentRect.Bottom - 70, 120, 60, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Cancel", "Arial12"), new Point(0, 0), firstButtonClick);
                    m_menuItems[0][0].Clicked += ClosePopUp;
                    break;
                case messageBoxType.ok:
                    AddItem(new Button(this, m_game1, "OKButton", ContentRect.Center.X - 60, ContentRect.Bottom - 70, 120, 60, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "OK", "Arial12"), new Point(0, 0), firstButtonClick);
                    m_menuItems[0][0].Clicked += ClosePopUp;
                    break;
                case messageBoxType.okCancel:
                    AddItem(new Button(this, m_game1, "OKButton", ContentRect.Center.X - 60 - 70, ContentRect.Bottom - 70, 120, 60, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "OK", "Arial12"), new Point(0, 0), firstButtonClick);
                    AddItem(new Button(this, m_game1, "CancelButton", ContentRect.Center.X - 60 + 70, ContentRect.Bottom - 70, 120, 60, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Cancel", "Arial12"), new Point(1, 0), secondButtonClick);
                    m_menuItems[0][0].Clicked += ClosePopUp;
                    m_menuItems[1][0].Clicked += ClosePopUp;
                    break;
                case messageBoxType.yesNo:
                    AddItem(new Button(this, m_game1, "YesButton", ContentRect.Center.X - 60 - 70, ContentRect.Bottom - 70, 120, 60, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Yes", "Arial12"), new Point(0, 0), firstButtonClick);
                    AddItem(new Button(this, m_game1, "NoButton", ContentRect.Center.X - 60 + 70, ContentRect.Bottom - 70, 120, 60, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "No", "Arial12"), new Point(1, 0), secondButtonClick);
                    m_menuItems[0][0].Clicked += ClosePopUp;
                    m_menuItems[1][0].Clicked += ClosePopUp;
                    break;
                case messageBoxType.locked:
                    m_rect.Height = m_rect.Height - 80;
                    break;
            }
            #endregion
        }

        //****************************************************
        // Method: MessageBox
        //
        // Purpose: Constructor for MessageBox which centers
        // it in the middle of the window.
        //****************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawner">The item that should recieve focus when this messagebox closes.</param>
        /// <param name="game1"></param>
        /// <param name="ownerItem">The menu that spawned this messagebox.</param>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="firstButtonClick"></param>
        /// <param name="secondButtonClick"></param>
        public MessageBox(Menu spawner, Game1 game1, Menu ownerItem, string name, string message, messageBoxType type, BasicMenuItemEventHandler firstButtonClick = null, BasicMenuItemEventHandler secondButtonClick = null)
            : this(spawner, game1, ownerItem, name, 0, 0, message, type, firstButtonClick, secondButtonClick)
        {
            m_rect.X = game1.GraphicsDevice.Viewport.Bounds.Center.X - m_rect.Width / 2;
            m_rect.Y = game1.GraphicsDevice.Viewport.Bounds.Center.Y - m_rect.Height / 2;
        }

        //****************************************************
        // Method: DrawContent
        //
        // Purpose: Draws all the content
        //****************************************************
        public override void DrawContent(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Point offset)
        {
            base.DrawContent(spriteBatch, graphicsDevice, offset);
            m_textBox.Draw(spriteBatch, graphicsDevice, offset);
        }

        #region Menu Item Event Handlers
        //****************************************************
        // Method: ClosePopUp
        //
        // Purpose: When any button is clicked, the popup
        // should be closed
        //****************************************************
        public void ClosePopUp(object sender)
        {
            LoseFocus(sender);
            m_actualOwnerItem.MessageBoxDelList.Add(this);
        }

        
        #endregion
        
    }
}
