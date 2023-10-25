//******************************************************
// File: ServerList.cs
//
// Purpose: Defines the ServerList menu that players
// will use to join a server.
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
    class ServerList : Menu
    {
        #region Declarations
        protected bool m_connecting; //Used to determine if attempting to connect to a server
        protected int m_connectingDelay; //Used to prevent the game from connecting right away so it allows it to find the average ping
        protected bool m_connectedWaitingForDelay;
        #endregion

        //****************************************************
        // Method: ServerList
        //
        // Purpose: Default constructor for ServerList
        //****************************************************
        public ServerList(Menu ownerItem, Game1 game1)
            : base(ownerItem, game1, "ServerList", GameState.InitializeServerListMenu, GameState.ServerListMenu, new Rectangle(0, 0, 800, 480), Color.White, Color.White, Color.White, Color.White)
        {
            Title = "Server List";
            m_fontColor = Color.Black;
            m_fontColorFocus = Color.Black;
            m_connecting = false;
            AddItem(new VertButtonList(this, game1, "ServerVertButtonList", ContentRect.Center.X - 350/2, 65, 330, 253, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, new Point(0,0), 280, 48, 5, 5, "TextBox", "TextBox", "Arial18", true), new Point(0, 0), ServerVertButtonList_Clicked);
            //foreach(string serverInfo in NetworkingHandler.QueryServers())
            //{
            //    ((VertButtonList)FindItem("ServerVertButtonList")).AddButton(serverInfo, ServerButton_Click);
            //}
            AddItem(new Button(this, m_game1, "BackButton", ContentRect.Center.X - 60, FindItem("ServerVertButtonList").Bounds.Bottom + 10, 120, 60, Color.Wheat, Color.Wheat, Color.Black, Color.Black, "Back", "Arial12"), new Point(0, 1), GoBack);
        }

        //****************************************************
        // Method: Initialize
        //
        // Purpose: Initializes the menu
        //****************************************************
        public override void Initialize()
        {
            Refresh(this);
            base.Initialize();
            ((VertButtonList)FindItem("ServerVertButtonList")).SelectedSinceInit = false;
            m_connecting = false;
            m_connectingDelay = 0;
            m_connectedWaitingForDelay = false;
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

            RefreshListener();

            if (m_connecting)
            {
                bool connected = false;
                string failedToConnect = "";
                NetworkingHandler.WorldInit(ref connected, ref failedToConnect);
                if (connected)
                {
                    m_game1.currGameState = GameState.InitializeGame;
                    //m_connectedWaitingForDelay = true;
                    //m_connectingDelay = 5000;
                }
                else if (failedToConnect != "")
                {
                    m_connecting = false;
                    m_messageBoxList[0].ClosePopUp(this);
                    PopUpMessage((Menu)FindItem("ServerVertButtonList"), "FailedToConnect", failedToConnect, messageBoxType.ok);
                }
            }

            if (m_connectedWaitingForDelay)
            {
                m_connectingDelay -= gameTime.ElapsedGameTime.Milliseconds;
                if (m_connectingDelay <= 0)
                {
                    m_game1.currGameState = GameState.InitializeGame;
                }
            }
        }

        #region Menu Item Event Handlers
        //****************************************************
        // Method: ServerVertButtonList_Clicked
        //
        // Purpose: When the ServerVertButtonList_Clicked is
        // clicked, the menu gives it focus
        //****************************************************
        public void ServerVertButtonList_Clicked(object sender)
        {
            ((VertButtonList)FindItem("ServerVertButtonList")).Initialize();
            GiveFocus(FindItem("ServerVertButtonList"));
        }

        //****************************************************
        // Method: ServerButton_Click
        //
        // Purpose: When any server button in the list
        // is clicked, this fires
        //****************************************************
        public void ServerButton_Click(object sender)
        {
            PopUpMessage((Menu)FindItem("ServerVertButtonList"), "ConnectingToServer", "Connecting to server...", messageBoxType.cancel, CancelConnect);

            //string[] serverInfoParsed = ((Button)((VertButtonList)FindItem("ServerVertButtonList")).CurrSelectedItem).Text.Split(':');
            //NetworkingHandler.ConnectToServer(serverInfoParsed[0], Convert.ToInt32(serverInfoParsed[1]));

            Server server = NetworkingHandler.ServerList[((VertButtonList)FindItem("ServerVertButtonList")).CurrSelection.Y];
            NetworkingHandler.ConnectToServer(server.IP, server.Port);

            m_connecting = true;
            m_connectedWaitingForDelay = false;
            m_connectingDelay = 0;
        }

        //****************************************************
        // Method: CancelConnect
        //
        // Purpose: Cancels the connection
        //****************************************************
        public void CancelConnect(object sender)
        {
            NetworkingHandler.ServerDisconnect();
            m_connecting = false;
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

        //****************************************************
        // Method: Refresh
        //
        // Purpose: Empties the list of servers and initiates
        // the process of queuing the server list
        //****************************************************
        public void Refresh(object sender)
        {
            ((VertButtonList)FindItem("ServerVertButtonList")).ClearList();
            NetworkingHandler.QueryServers();
        }

        //****************************************************
        // Method: RefreshListener
        //
        // Purpose: Looks at the list of server in the
        // networking handler and adds any new servers to
        // the ServerVertButtonList.
        //****************************************************
        public void RefreshListener()
        {
            int tempNumMissing = NetworkingHandler.ServerList.Count - ((VertButtonList)FindItem("ServerVertButtonList")).ButtonCount;
            if (tempNumMissing > 0)
            {
                for (int i = NetworkingHandler.ServerList.Count - tempNumMissing; i < NetworkingHandler.ServerList.Count; ++i)
                {
                    //((VertButtonList)FindItem("ServerVertButtonList")).AddButton(NetworkingHandler.ServerList[i].IP + ":" + NetworkingHandler.ServerList[i].Port.ToString(), ServerButton_Click);
                    ((VertButtonList)FindItem("ServerVertButtonList")).AddButton(NetworkingHandler.ServerList[i].Name, ServerButton_Click);
                }
            }
        }

        #endregion
        
    }
}
