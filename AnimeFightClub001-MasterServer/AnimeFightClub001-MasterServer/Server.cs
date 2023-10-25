//**********************************************************
// File: Server.cs
//
// Purpose: Used to store a server connection to the
// master server.
//
// Written By: Salvatore Hanusiewicz
//**********************************************************
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Lidgren.Network;

namespace AnimeFightClub001_MasterServer
{
    class Server : Connection
    {
        #region Declarations
        protected int m_maxPlayers; //Stores the maximum number of players which can connect
        protected int m_playerCount; //Stores the current number of players connected to the server
        protected int m_port; //Stores the destination port
        #endregion

        //****************************************************
        // Method: Server
        //
        // Purpose: Constructor for Server
        //****************************************************
        public Server(NetConnection netConnection, string name, int port)
            : base(netConnection, name)
        {
            m_port = port;
        }


        #region Properties
        public int MaxPlayers
        {
            get
            {
                return m_maxPlayers;
            }
            set
            {
                m_maxPlayers = value;
            }
        }

        public int PlayerCount
        {
            get
            {
                return m_playerCount;
            }
            set
            {
                m_playerCount = value;
            }
        }

        public int Port
        {
            get
            {
                return m_port;
            }
        }
        #endregion
    }
}