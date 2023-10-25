//**********************************************************
// File: Client.cs
//
// Purpose: Used to store a client connection to the
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
    class Client : Connection
    {
        #region Declarations
        #endregion

        //****************************************************
        // Method: Client
        //
        // Purpose: Constructor for Client
        //****************************************************
        public Client(NetConnection netConnection, string name) : base(netConnection, name)
        {
        }


        #region Properties
        #endregion
    }
}
