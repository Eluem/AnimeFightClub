//**********************************************************
// File: NetworkHandler.cs
//
// Purpose: Used to control the master server.
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

    enum PacketType { ObjectStateUpdate, WorldInit, VelocityUpdate, AccelerationUpdate, PositionUpdate, ControllerStateUpdate, AddHazardObj, AddItemObj, AddSpecialEnvironmentalObj, AddEnvironmentalObj, AddPlayerObj, DeleteHazardObj, DeleteItemObj, DeleteSpecialEnvironmentalObj, DeleteEnvironmentalObj, DeletePlayerObj, Hail, Approved, Disconnect, ServerListQueryData, CustomizationQuerySendData, RegistrationRequestData, IntroductionRequest, AddPlayerStatus, DeletePlayerStatus, AddPlayerStatusComplex, UpdatePlayerHP, UpdatePlayerMP, MatchEnd }
    enum PacketSequence { GenericDataRequest, ObjectStateUpdate, WorldInit, VelocityUpdate, AccelerationUpdate, PositionUpdate, ControllerStateUpdate, AddDeleteObj, Hail, Disconnect }

    class NetworkHandler
    {
        #region Declarions
        Dictionary<string, Client> m_clientDict; //Dictionary of clients (key is the user name)
        Dictionary<string, Server> m_serverDict; //Dictionary of servers (key is the server's IP)


        NetPeerConfiguration m_config = new NetPeerConfiguration("MyExampleName");
        NetServer m_server;

        Form1 m_form1; //Pointer to the main form (may change/rename)?

        DatabaseHandler m_databaseHandler;

        #endregion

        //****************************************************
        // Method: NetworkHandler
        //
        // Purpose: Constructor for NetworkHandler
        //****************************************************
        public NetworkHandler(Form1 form1)
        {
            m_form1 = form1;
            m_serverDict = new Dictionary<string, Server>();
            m_clientDict = new Dictionary<string, Client>();

            m_databaseHandler = new DatabaseHandler(@"AnimeFightClub001db.accdb");
        }

        //****************************************************
        // Method: StartServer
        //
        // Purpose: Initializes required variables and
        // starts the server (needs to be generalized)
        //****************************************************
        public void StartServer()
        {
            m_config.Port = 35335; //Must be generalized


            m_server = new NetServer(m_config);
            m_config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            m_config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            m_server.Start();
        }

        //****************************************************
        // Method: HandleMessages
        //
        // Purpose: Reads and handles all network messages
        //****************************************************
        public void HandleMessages()
        {
            #region Handle Messages
            NetOutgoingMessage sendMsg;
            NetIncomingMessage msg;
            while ((msg = m_server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    #region Errors
                    case NetIncomingMessageType.VerboseDebugMessage:
                        Console.WriteLine("Verbose Debug Message: " + msg.ReadString());
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        Console.WriteLine("Debug Message: " + msg.ReadString());
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        Console.WriteLine("Warning Message: " + msg.ReadString());
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine("Error Message: " + msg.ReadString());
                        break;
                    #endregion

                    #region Connection Approval
                    case NetIncomingMessageType.ConnectionApproval:
                        switch ((PacketType)msg.ReadByte())
                        {
                            #region Hail
                            case PacketType.Hail:
                                //Hail message sent as a csv string of data (0 = server/client, 1 = 'name')
                                string[] hailStringData = msg.ReadString().Split(',');
                                msg.SenderConnection.Tag = hailStringData; //Use tag because RemoteHailMessage seems broken?


                                switch (hailStringData[0])
                                {
                                    #region Server
                                    case "Server":
                                        //Make sure to have some way of handling hails from clients and servers differently
                                        #region Approval
                                        //Check if server is okay
                                        ApproveDenyServer(hailStringData, msg);

                                        #endregion
                                        break;
                                    #endregion

                                    #region Client
                                    case "Client":
                                        //Make sure to have some way of handling hails from clients and servers differently
                                        #region Approval
                                        //Check if player is okay
                                        ApproveDenyClient(hailStringData, msg);
                                        #endregion
                                        break;
                                    #endregion
                                }
                                break;
                            #endregion

                            #region Register New Account Request
                            case PacketType.RegistrationRequestData:
                                string response = m_databaseHandler.RegisterNewUser(msg.ReadString(), msg.ReadString());
                                msg.SenderConnection.Deny(response);
                                break;
                            #endregion
                        }
                        break;
                    #endregion

                    #region Read Data
                    case NetIncomingMessageType.Data:
                        switch ((PacketType)msg.ReadByte())
                        {
                            #region Queried Server List
                            case PacketType.ServerListQueryData:
                                sendMsg = null;
                                sendMsg = m_server.CreateMessage();
                                sendMsg.Write((byte)PacketType.ServerListQueryData);
                                sendMsg.Write(m_serverDict.Count);
                                foreach (KeyValuePair<string, Server> server in m_serverDict)
                                {
                                    sendMsg.Write(server.Value.IP);
                                    sendMsg.Write(server.Value.Port);
                                    sendMsg.Write(server.Value.Name);
                                    sendMsg.Write(server.Value.MaxPlayers);
                                    sendMsg.Write(server.Value.PlayerCount);
                                }

                                m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered, (int)PacketSequence.GenericDataRequest);

                                break;
                            #endregion

                            #region Queried Customization Data
                            case PacketType.CustomizationQuerySendData:
                                switch (msg.ReadString())
                                {
                                    #region Purchase an Ability
                                    case "PurchaseAbility":
                                        string messsage = m_databaseHandler.PurchaseItem(RetrieveUserName(msg.SenderConnection), msg.ReadString(), msg.ReadString());
                                        sendMsg = null;
                                        sendMsg = m_server.CreateMessage();
                                        sendMsg.Write((byte)PacketType.CustomizationQuerySendData);
                                        sendMsg.Write("PurchaseAbility");
                                        sendMsg.Write(messsage);

                                        m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
                                        break;
                                    #endregion

                                    #region Send Current Exp
                                    case "CurrentExpQuery":
                                        int currentExp = m_databaseHandler.QueryCharacterExp(RetrieveUserName(msg.SenderConnection));
                                        sendMsg = null;
                                        sendMsg = m_server.CreateMessage();
                                        sendMsg.Write((byte)PacketType.CustomizationQuerySendData);
                                        sendMsg.Write("CurrentExpQuery");
                                        sendMsg.Write(currentExp);

                                        m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
                                        break;
                                    #endregion

                                    #region Update Currently Selected Abilities
                                    case "SelectedAbilityUpdate":
                                        m_databaseHandler.ChangeLoadout(RetrieveUserName(msg.SenderConnection), msg.ReadString(), msg.ReadString());
                                        break;
                                    #endregion

                                    #region Send Ability List To Client
                                    case "AbilityListQuery":
                                        List<AbilityInfo> m_abilityInfoList = new List<AbilityInfo>();
                                        string tempName = RetrieveUserName(msg.SenderConnection);
                                        if (tempName == null)
                                            break;
                                        m_abilityInfoList = m_databaseHandler.QueryAbilityList(tempName);
                                        sendMsg = null;
                                        sendMsg = m_server.CreateMessage();
                                        sendMsg.Write((byte)PacketType.CustomizationQuerySendData);
                                        sendMsg.Write("AbilityListQuery");
                                        sendMsg.Write(m_abilityInfoList.Count);
                                        foreach (AbilityInfo abilityInfo in m_abilityInfoList)
                                        {
                                            sendMsg.Write(abilityInfo.Name);
                                            sendMsg.Write(abilityInfo.Category);
                                            sendMsg.Write(abilityInfo.Description);
                                            sendMsg.Write(abilityInfo.Cost);
                                            sendMsg.Write(abilityInfo.Owned);
                                        }

                                        m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);

                                        break;
                                    #endregion

                                    #region Send Currently Selected Abilities To Client
                                    case "CurrentAbilitiesQuery":
                                        sendMsg = null;
                                        sendMsg = m_server.CreateMessage();
                                        sendMsg.Write((byte)PacketType.CustomizationQuerySendData);
                                        sendMsg.Write("CurrentAbilitiesQuery");
                                        Dictionary<string, AbilityInfo> tempAbilityInfoDict = m_databaseHandler.QueryCharacterLoadout(RetrieveUserName(msg.SenderConnection));
                                        sendMsg.Write(tempAbilityInfoDict["MainHand"].Name);
                                        sendMsg.Write(tempAbilityInfoDict["OffHand"].Name);
                                        sendMsg.Write(tempAbilityInfoDict["Passive"].Name);
                                        sendMsg.Write(tempAbilityInfoDict["Special1"].Name);
                                        sendMsg.Write(tempAbilityInfoDict["Special2"].Name);
                                        sendMsg.Write(tempAbilityInfoDict["Special3"].Name);
                                        sendMsg.Write(tempAbilityInfoDict["Avatar"].Name);


                                        m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
                                        break;
                                    #endregion

                                    #region Send Currently Selected Abilities To Server
                                    case "CurrentAbilitiesServerQuery":
                                        string userName = msg.ReadString();
                                        sendMsg = null;
                                        sendMsg = m_server.CreateMessage();
                                        sendMsg.Write((byte)PacketType.CustomizationQuerySendData);
                                        sendMsg.Write("CurrentAbilitiesServerQuery");
                                        sendMsg.Write(userName);
                                        Dictionary<string, AbilityInfo> tempAbilityInfoDictCludge2 = m_databaseHandler.QueryCharacterLoadout(userName);
                                        sendMsg.Write(tempAbilityInfoDictCludge2["MainHand"].Name);
                                        sendMsg.Write(tempAbilityInfoDictCludge2["OffHand"].Name);
                                        sendMsg.Write(tempAbilityInfoDictCludge2["Passive"].Name);
                                        sendMsg.Write(tempAbilityInfoDictCludge2["Special1"].Name);
                                        sendMsg.Write(tempAbilityInfoDictCludge2["Special2"].Name);
                                        sendMsg.Write(tempAbilityInfoDictCludge2["Special3"].Name);
                                        sendMsg.Write(tempAbilityInfoDictCludge2["Avatar"].Name);


                                        m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
                                        break;
                                    #endregion
                                }
                                break;
                            #endregion

                            #region Match End
                            case PacketType.MatchEnd:
                                int count = msg.ReadInt32();
                                for (int i = 0; i < count; ++i)
                                {
                                    m_databaseHandler.GrantExp(msg.ReadString(), msg.ReadInt32() * 100);
                                }
                                break;
                            #endregion

                            #region Introduction Request (Deprecated?)
                            case PacketType.IntroductionRequest:
                                /*
                                string IP = msg.ReadString();
                                int port = msg.ReadInt32();
                                NetConnection tempServer = null;
                                foreach (KeyValuePair<string, Server> server in m_serverDict)
                                {
                                    if (server.Value.IP == IP)
                                        tempServer = server.Value.NetConnection;
                                }
                                */


                                break;
                            #endregion
                        }
                        break;

                    #endregion

                    #region Read Unconnected Data
                    case NetIncomingMessageType.UnconnectedData:

                        break;
                    #endregion

                    #region Status Changed
                    case NetIncomingMessageType.StatusChanged:
                        //I use tag because the RemoteHailMessage seems to chop data off, I can't figure out why...
                        string[] remoteHailMessage = (string[])msg.SenderConnection.Tag;
                        //Cludge try catch
                        #region Disconnected/Connected
                        try
                        {
                            #region Disconnected
                            if (msg.SenderConnection.Status == NetConnectionStatus.Disconnected)
                            {
                                #region Server Disconnect
                                if (remoteHailMessage[0] == "Server")
                                {
                                    //Remove the server
                                    m_serverDict.Remove(remoteHailMessage[1]);

                                    //Fire server list changed
                                    m_form1.ServerListChanged();
                                }
                                #endregion

                                #region Client Disconnect
                                else
                                {
                                    //Remove the client

                                    m_clientDict.Remove(remoteHailMessage[1]);

                                    //Fire client list changed
                                    //m_form1.ClientListChanged();
                                }
                                #endregion
                            }
                            #endregion


                            #region Connected
                            //Not sure what to make the if statement here look like?!?!?!?
                            else if (msg.SenderConnection.Status == NetConnectionStatus.Connected)// || msg.SenderConnection.Status == NetConnectionStatus.RespondedConnect)
                            {
                                #region Server Connect

                                if (remoteHailMessage[0] == "Server")
                                {
                                    //Add server to dictionary
                                    m_serverDict.Add(remoteHailMessage[1], new Server(msg.SenderConnection, remoteHailMessage[1], Convert.ToInt32(remoteHailMessage[2])));

                                    //Should be replaced with a better method built into lidgren?
                                    #region Send Approval Message
                                    sendMsg = null;
                                    sendMsg = m_server.CreateMessage();
                                    sendMsg.Write((byte)PacketType.Approved);
                                    sendMsg.Write("MasterServer");

                                    m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered, (int)PacketSequence.GenericDataRequest);
                                    #endregion

                                    //Fire server list changed
                                    m_form1.ServerListChanged();
                                }
                                #endregion

                                #region Client Connect
                                else
                                {

                                    //Add client to dictionary
                                    m_clientDict.Add(remoteHailMessage[1], new Client(msg.SenderConnection, remoteHailMessage[1]));

                                    //Should be replaced with a better method built into lidgren?
                                    #region Send Approval Message
                                    sendMsg = null;
                                    sendMsg = m_server.CreateMessage();
                                    sendMsg.Write((byte)PacketType.Approved);
                                    sendMsg.Write("MasterServer");

                                    m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
                                    #endregion

                                    //Fire client list changed
                                    //m_form1.ClientListChanged();
                                }
                                #endregion
                            }
                            #endregion

                        }
                        catch (Exception e)
                        {
                        }
                        break;
                        #endregion
                    #endregion

                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }
                m_server.Recycle(msg);
            }
            #endregion
        }


        //****************************************************
        // Method: NetworkListenThread
        //
        // Purpose: Contains the code that will run when
        // the Networking thread is started
        //****************************************************
        public void NetworkListenThread()
        {
            #region Initialize Server
            StartServer();
            #endregion

            #region Network Loop
            while (true)
            {
                try
                {

                    HandleMessages();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("There was an error: " + e.ToString());
                }
                Thread.Sleep(1);
            }
            #endregion
        }


        //****************************************************
        // Method: ApproveDenyServer
        //
        // Purpose: Approves or denies a server connection.
        // returns true if connection was approved and
        // false if it wasn't.
        //****************************************************
        /// <summary>
        /// Approves or denies a server connection.
        /// Returns true if connection was approved and
        /// false if it wasn't.
        /// </summary>
        /// <param name="hailStringData">The array of data obtained by parsing the hail message string</param>
        /// <param name="msg">The message sent. (Used to approve or deny the connection)</param>
        /// <returns>True if approved, false if denied.</returns>
        public bool ApproveDenyServer(string[] hailStringData, NetIncomingMessage msg)
        {
            if (m_serverDict.ContainsKey(hailStringData[1]))
            {
                msg.SenderConnection.Deny("A server with this name already exists!");
                return false;
            }
            msg.SenderConnection.Approve();

            return true;
        }

        //****************************************************
        // Method: ApproveDenyClient
        //
        // Purpose: Approves or denies a server connection.
        // returns true if connection was approved and
        // false if it wasn't.
        //****************************************************
        /// <summary>
        /// Approves or denies a client connection.
        /// Returns true if connection was approved and
        /// false if it wasn't.
        /// </summary>
        /// <param name="hailStringData">The array of data obtained by parsing the hail message string</param>
        /// <param name="msg">The message sent. (Used to approve or deny the connection)</param>
        /// <returns>True if approved, false if denied.</returns>
        public bool ApproveDenyClient(string[] hailStringData, NetIncomingMessage msg)
        {
            if (m_clientDict.ContainsKey(hailStringData[1]))
            {
                msg.SenderConnection.Deny("Master Server: Someone with this user name is already logged in!");
                return false;
            }

            if (!VerifyClientCredentials(hailStringData[1], hailStringData[2]))
            {
                msg.SenderConnection.Deny("Master Server: Invalid user name and password!");
                return false;
            }

            msg.SenderConnection.Approve();

            return true;
        }


        //****************************************************
        // Method: VerifyClientCredentials
        //
        // Purpose: Determines whether or not the entered
        // user name and password match.
        //****************************************************
        public bool VerifyClientCredentials(string userName, string password)
        {
            return m_databaseHandler.ValidateUser(userName, password);
        }


        //****************************************************
        // Method: RetrieveUserName
        //
        // Purpose: Finds the user name bound to a particular
        // NetConnection. If it wasn't found, return null.
        //****************************************************
        public string RetrieveUserName(NetConnection connection)
        {
            foreach (KeyValuePair<string, Client> stringClientPair in m_clientDict)
            {
                if (stringClientPair.Value.NetConnection.Equals(connection))
                {
                    return stringClientPair.Key;
                }
            }
            return null;
        }

        #region Properties

        public List<NetConnection> ServerNetConnections
        {
            get
            {
                List<NetConnection> tempNetConnectionList = new List<NetConnection>();
                foreach (KeyValuePair<string, Server> server in m_serverDict)
                {
                    tempNetConnectionList.Add(server.Value.NetConnection);
                }

                return tempNetConnectionList;
            }
        }

        public Dictionary<string, Server> ServerDict
        {
            get
            {
                return m_serverDict;
            }
        }
        #endregion
    }
}
