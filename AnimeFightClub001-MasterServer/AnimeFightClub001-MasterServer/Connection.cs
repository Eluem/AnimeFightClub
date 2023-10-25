//**********************************************************
// File: Connection.cs
//
// Purpose: Used as a base class for server and client
// connections
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
    class Connection
    {
        #region Declarations
        protected string m_name; //Should also be used as the 'value' for the key in the dictionary it will be stored in
        protected NetConnection m_netConnection;

        protected int m_ping;
        #endregion

        //****************************************************
        // Method: Connection
        //
        // Purpose: Constructor for Connection
        //****************************************************
        public Connection(NetConnection netConnection, string name)
        {
            m_netConnection = netConnection;
            m_name = name;

            m_ping = 0;
        }

        #region Properties
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

        public NetConnection NetConnection
        {
            get
            {
                return m_netConnection;
            }

            set
            {
                m_netConnection = value;
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

        public string IP
        {
            get
            {
                return m_netConnection.RemoteEndpoint.Address.ToString();
            }
        }
        #endregion
    }
}

