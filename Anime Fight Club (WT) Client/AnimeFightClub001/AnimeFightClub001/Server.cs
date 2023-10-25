//******************************************************
// File: Server.cs
//
// Purpose: Server object. Used to store basic
// information about servers in the server list
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
    class Server
    {
        #region Declarations
        protected string m_IP; //Stores the IP address of the server
        protected int m_port; //Stores the port of the server

        protected string m_name; //Stores the name of the server

        protected int m_maxPlayers; //Stores the maximum number of players which can connect
        protected int m_playerCount; //Stores the current number of players connected to the server

        protected int m_ping; //Stores your ping to the server (probably won't use)
        #endregion

        //****************************************************
        // Method: Server
        //
        // Purpose: Constructor for Server
        //****************************************************
        public Server(string IP = "0.0.0.0", int port = 0, string name = "", int maxPlayer = 0, int playerCount = 0, int ping = 0)
        {
            m_IP = IP;
            m_port = port;

            m_name = name;

            m_maxPlayers = maxPlayer;
            m_playerCount = playerCount;
    
            m_ping = ping;
        }


        #region Properties

        public string IP
        {
            get
            {
                return m_IP;
            }
            set
            {
                m_IP = value;
            }
        }

        public int Port
        {
            get
            {
                return m_port;
            }
            set
            {
                m_port = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

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

        public int Ping
        {
            get
            {
                return m_ping;
            }
            set
            {
                m_ping = value;
            }
        }
        #endregion
    }
}
