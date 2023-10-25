//******************************************************
// File: NetworkingHandler.cs
//
// Purpose: Contains the definition of the
// NetworkingHandler. This class will contain all the
// functions and variables required to talk to the
// network.
//
// (DEVELOPER NOTE: I always send any data regarding
// all object types in the following order:
// PlayerObj, HazardObj, EnvironmentalObj,
// SpecialEnvironmentalObj, ItemObj
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
    enum PacketType { ObjectStateUpdate, WorldInit, VelocityUpdate, AccelerationUpdate, PositionUpdate, ControllerStateUpdate, AddHazardObj, AddItemObj, AddSpecialEnvironmentalObj, AddEnvironmentalObj, AddPlayerObj, DeleteHazardObj, DeleteItemObj, DeleteSpecialEnvironmentalObj, DeleteEnvironmentalObj, DeletePlayerObj, Hail, Approved, Disconnect, ServerListQueryData, CustomizationQuerySendData, RegistrationRequestData, IntroductionRequest, AddPlayerStatus, DeletePlayerStatus, AddPlayerStatusComplex, UpdatePlayerHP, UpdatePlayerMP, MatchEnd }
    enum PacketSequence { GenericDataRequest, ObjectStateUpdate, WorldInit, VelocityUpdate, AccelerationUpdate, PositionUpdate, ControllerStateUpdate, AddDeleteObj, Hail, Disconnect }
    static class NetworkingHandler
    {
        #region Declarions
        static NetPeerConfiguration m_config = new NetPeerConfiguration("MyExampleName");
        static NetPeer m_client;
        static PacketBuffer m_packetBuffer = new PacketBuffer();


        //Not the best way to implement this, but it might be better than the way I did it for the normal server
        //This will be used to enter a string indicating to the LoginMenu what's going on with the connection to the master server when
        //trying to connect. It will be cleared when the LoginMenu is done looking at it
        static string m_masterServerConnectionState = "";
        static NetConnection m_masterServer; //Pointer to master server connection

        static NetConnection m_server; //Pointer to current server connection

        //These might belong in the global variables... lol
        static List<Server> m_serverList = new List<Server>(); //List of servers as of last query
        static List<AbilityInfo> m_abilityInfoList = new List<AbilityInfo>(); //List of servers as of last query

        public static string ServerPurchaseRequestResponse = "";

        public static string WinnerName = "";
        public static int WinnerScore = 0;

        #endregion


        //****************************************************
        // Method: StartClient
        //
        // Purpose: Starts the Lidgren NetClient
        //****************************************************
        static public void StartClient()
        {
            m_client = new NetPeer(m_config);
            m_client.Start();
        }

        //****************************************************
        // Method: ConnectToServer
        //
        // Purpose: Initializes required variables and
        // connects to the server
        //****************************************************
        static public void ConnectToServer(string ipAddress, int port)
        {
            //Inform the master server about the server you wish to connect to

            //Wait until master server knows

            //Hail the server you wish to connect to            
            NetOutgoingMessage hailMessage;
            hailMessage = m_client.CreateMessage();
            hailMessage.Write((byte)PacketType.Hail);
            hailMessage.Write(GlobalVariables.UserName);
            m_server = m_client.Connect(ipAddress, port, hailMessage);


            hailMessage = null; //Clear hail message
        }

        //****************************************************
        // Method: WorldInit
        //
        // Purpose: Waits for the server to send the player
        // a world init packet. If it fails too many time, it
        // times out.
        //****************************************************
        /// <summary>
        /// Attempts to read a world init from the server. If it successfully reads a world init from the server then the connection
        /// was successful. If it doesn't, then it should be called again and again until it determines that the connection was a failure.
        /// </summary>
        /// <param name="connected">A reference parameter that will be changed to true if the connection was successful</param>
        /// <param name="failedToConnect">A reference parameter that will be changed to true if the connection was a failure</param>
        static public void WorldInit(ref bool connected, ref string failedToConnect)
        {
            //NOTE: Make sure to prevent the game from crashing if the server fraks up
            NetIncomingMessage incMsg;
            while ((incMsg = m_client.ReadMessage()) != null)
            {
                switch (incMsg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        try
                        {
                            switch ((NetConnectionStatus)incMsg.ReadByte())
                            {
                                case NetConnectionStatus.Disconnected:
                                    if (incMsg.SenderConnection == m_server)
                                    {
                                        failedToConnect = incMsg.ReadString();
                                    }
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                        break;
                    #region Reciving Data
                    case NetIncomingMessageType.Data:
                        PacketType packetType = (PacketType)incMsg.ReadByte();
                        switch (packetType)
                        {
                            #region WorldInit
                            case PacketType.WorldInit:
                                GlobalVariables.CurrentFrame = (float)(incMsg.ReadUInt32() + 1);
                                UInt32 count; //To be used in all the counts
                                string objectTypeName; //Stores the name of the object type

                                #region Read PlayerObjs
                                count = incMsg.ReadUInt32();

                                for (int i = 0; i < count; i++)
                                {
                                    objectTypeName = incMsg.ReadString();

                                    //Adds an object to the game as the type which was sent
                                    GameObjectHandler.DirectAddObject((PlayerObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + objectTypeName)));

                                    //Initializes the Object
                                    GameObjectHandler.PlayerObjList.Last.Value.NetInitialize(incMsg);
                                }

                                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                                {
                                    if (player.UserName == GlobalVariables.UserName)
                                    {
                                        GameObjectHandler.LocalPlayer = player;
                                    }
                                }

                                #endregion

                                #region Read HazardObjs
                                count = incMsg.ReadUInt32();

                                for (int i = 0; i < count; i++)
                                {
                                    //Adds an object to the game as the type which was sent
                                    GameObjectHandler.DirectAddObject((HazardObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + incMsg.ReadString())));

                                    //Initializes the Object
                                    GameObjectHandler.HazardObjList.Last.Value.NetInitialize(incMsg);
                                }

                                #endregion

                                #region Read EnvironmentalObjs
                                count = incMsg.ReadUInt32();

                                for (int i = 0; i < count; i++)
                                {
                                    //Adds an object to the game as the type which was sent
                                    GameObjectHandler.DirectAddObject((EnvironmentalObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + incMsg.ReadString())));

                                    //Initializes the Object
                                    GameObjectHandler.EnvironmentalObjList.Last.Value.NetInitialize(incMsg);
                                }

                                #endregion

                                #region Read SpecialEnvironmentalObjs
                                count = incMsg.ReadUInt32();

                                for (int i = 0; i < count; i++)
                                {
                                    //Adds an object to the game as the type which was sent
                                    GameObjectHandler.DirectAddObject((SpecialEnvironmentalObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + incMsg.ReadString())));

                                    //Initializes the Object
                                    GameObjectHandler.SpecialEnvironmentalObjList.Last.Value.NetInitialize(incMsg);
                                }

                                #endregion

                                #region Read ItemObjs
                                count = incMsg.ReadUInt32();

                                for (int i = 0; i < count; i++)
                                {
                                    //Adds an object to the game as the type which was sent
                                    GameObjectHandler.DirectAddObject((ItemObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + incMsg.ReadString())));

                                    //Initializes the Object
                                    GameObjectHandler.ItemObjList.Last.Value.NetInitialize(incMsg);
                                }

                                #endregion


                                connected = true;
                                break;
                            #endregion

                            #region Connection Approved
                            case PacketType.Approved:
                                string tempMsg = incMsg.ReadString();
                                switch (tempMsg)
                                {
                                    case "MasterServer":
                                        m_masterServer = incMsg.SenderConnection;
                                        break;
                                    case "Server":
                                        m_server = incMsg.SenderConnection;
                                        break;
                                }
                                break;
                            #endregion

                            #region Queried Server List
                            case PacketType.ServerListQueryData:
                                int serverCount = incMsg.ReadInt32();
                                for (int i = 0; i < serverCount; ++i)
                                {
                                    m_serverList.Add(new Server(incMsg.ReadString(), incMsg.ReadInt32(), incMsg.ReadString(), incMsg.ReadInt32(), incMsg.ReadInt32()));
                                }
                                break;
                            #endregion

                            default:
                                Console.WriteLine("Unhandled Data Type: " + packetType.ToString());
                                break;
                        }
                        break;
                    #endregion
                    default:
                        Console.WriteLine("Unhandled Msg (" + incMsg.MessageType.ToString() + "): " + incMsg.ReadString());
                        break;
                }
            }

        }



        //****************************************************
        // Method: HandleMessages
        //
        // Purpose: Reads and handles all network messages
        //****************************************************
        static public void HandleMessages(Game1 game1)
        {
            #region Declarations
            UInt32 count; //To be used in all the counts
            UInt16 objID; //To be used in checking all objIDs
            NetIncomingMessage msg;
            PlayerObj tempPlayer;
            #endregion

            #region Handle Messages

            while ((msg = m_client.ReadMessage()) != null)
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

                    #region Read Data
                    case NetIncomingMessageType.Data:
                        switch ((PacketType)msg.ReadByte())
                        {
                            #region Controller State Update
                            case PacketType.ControllerStateUpdate:
                                Int64 inPlayerId = msg.ReadInt64();
                                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                                {
                                    if (player.PlayerID == inPlayerId)
                                    {
                                        UInt16 ctrlState = msg.ReadUInt16();
                                        player.ControllerState.setControlStateAsInt16(ctrlState);
                                        break;
                                    }
                                }
                                break;
                            #endregion

                            #region AddObj Handlers
                            #region Add Player
                            case PacketType.AddPlayerObj:
                                //Adds an object to the game as the type which was sent
                                GameObjectHandler.DirectAddObject((PlayerObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + msg.ReadString())));

                                //Initializes the Object
                                GameObjectHandler.PlayerObjList.Last.Value.NetInitialize(msg);
                                break;
                            #endregion

                            #region Add Hazard
                            case PacketType.AddHazardObj:
                                //Adds an object to the game as the type which was sent
                                GameObjectHandler.DirectAddObject((HazardObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + msg.ReadString())));

                                //Initializes the Object
                                GameObjectHandler.HazardObjList.Last.Value.NetInitialize(msg);
                                break;
                            #endregion

                            #region Add Item
                            case PacketType.AddItemObj:
                                //Adds an object to the game as the type which was sent
                                GameObjectHandler.DirectAddObject((ItemObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + msg.ReadString())));

                                //Initializes the Object
                                GameObjectHandler.ItemObjList.Last.Value.NetInitialize(msg);
                                break;
                            #endregion

                            #region Add Special EnvironmentalObj
                            case PacketType.AddSpecialEnvironmentalObj:
                                GameObjectHandler.DirectAddObject((SpecialEnvironmentalObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + msg.ReadString())));

                                //Initializes the Object
                                GameObjectHandler.SpecialEnvironmentalObjList.Last.Value.NetInitialize(msg);
                                break;
                            #endregion

                            #region Add EnvironmentalObj
                            case PacketType.AddEnvironmentalObj:
                                GameObjectHandler.DirectAddObject((EnvironmentalObj)Activator.CreateInstance(Type.GetType("AnimeFightClub001." + msg.ReadString())));

                                //Initializes the Object
                                GameObjectHandler.EnvironmentalObjList.Last.Value.NetInitialize(msg);
                                break;
                            #endregion

                            #endregion

                            #region DeleteObj Handlers
                            #region Delete Player
                            case PacketType.DeletePlayerObj:
                                //Deletes the object from the game by the ID sent
                                GameObjectHandler.DirectDeletePlayerObjectByID(msg.ReadUInt16());
                                break;
                            #endregion

                            #region Delete Hazard
                            case PacketType.DeleteHazardObj:
                                //Deletes the object from the game by the ID sent
                                GameObjectHandler.DirectDeleteHazardObjectByID(msg.ReadUInt16());
                                break;
                            #endregion

                            #region Delete Item
                            case PacketType.DeleteItemObj:
                                //Deletes the object from the game by the ID sent
                                GameObjectHandler.DirectDeleteItemObjectByID(msg.ReadUInt16());
                                break;
                            #endregion

                            #region Delete Special EnvironmentalObj
                            case PacketType.DeleteSpecialEnvironmentalObj:
                                //Deletes the object from the game by the ID sent
                                GameObjectHandler.DirectDeleteSpecialEnvironmentalObjectByID(msg.ReadUInt16());
                                break;
                            #endregion

                            #region Delete EnvironmentalObj
                            case PacketType.DeleteEnvironmentalObj:
                                //Deletes the object from the game by the ID sent
                                GameObjectHandler.DirectDeleteEnvironmentalObjectByID(msg.ReadUInt16());
                                break;
                            #endregion

                            #endregion

                            #region Object State Update
                            case PacketType.ObjectStateUpdate:
                                switch (msg.ReadString())
                                {
                                    case "PlayerObj":
                                        tempPlayer = null;
                                        tempPlayer = GameObjectHandler.FindPlayerObjByID( msg.ReadUInt16());
                                        if (tempPlayer != null)
                                        {
                                            int tempState = msg.ReadInt32();

                                            if ((PlayerState)tempPlayer.State == PlayerState.Sliding && (PlayerState)tempState != PlayerState.Sliding)
                                            {


                                                float tempBottom = tempPlayer.PhysicsObj.Rect.Bottom;
                                                tempPlayer.PhysicsObj.Rect.Width = PlayerObj.NORM_PHYS_WIDTH;
                                                tempPlayer.PhysicsObj.Rect.Height = PlayerObj.NORM_PHYS_HEIGHT;
                                                tempPlayer.PhysicsObj.PosY = tempBottom - PlayerObj.NORM_PHYS_HEIGHT;
                                            }

                                            if ((PlayerState)tempState == PlayerState.Sliding)
                                            {
                                                float tempBottom = tempPlayer.PhysicsObj.Rect.Bottom;
                                                tempPlayer.PhysicsObj.Rect.Width = PlayerObj.SLIDING_PHYS_WIDTH;
                                                tempPlayer.PhysicsObj.Rect.Height = PlayerObj.SLIDING_PHYS_HEIGHT;
                                                tempPlayer.PhysicsObj.PosY = tempBottom - PlayerObj.SLIDING_PHYS_HEIGHT;
                                            }

                                            tempPlayer.State = tempPlayer.ObtainPlayerState((PlayerState)tempState, tempPlayer.Action);
                                            if ((PlayerState)tempPlayer.State == PlayerState.LedgeGrabbing)
                                            {
                                                tempPlayer.LedgeGrabbingPos = new Vector2(msg.ReadFloat(), msg.ReadFloat());
                                            }
                                        }
                                        break;
                                }
                                break;
                            #endregion

                            #region Add Player Status Handler
                            case PacketType.AddPlayerStatus:
                                tempPlayer = null;
                                tempPlayer = GameObjectHandler.FindPlayerObjByID(msg.ReadUInt16());
                                tempPlayer.StatusHandler.DirectInflict((StatusName)msg.ReadByte(), msg.ReadInt32(), msg.ReadUInt16(), msg.ReadUInt16());
                                break;
                            #endregion

                            #region Delete Player Status Handler
                            case PacketType.DeletePlayerStatus:
                                tempPlayer = null;
                                tempPlayer = GameObjectHandler.FindPlayerObjByID(msg.ReadUInt16());
                                Status tempStatus = tempPlayer.StatusHandler.FindStatusByID(msg.ReadUInt16());
                                int tempStatusDuration = msg.ReadInt32();
                                if (tempStatus != null)
                                {
                                    //There's really no reason for this if statement. I could just delete it either way or
                                    //set the duration equal to zero either way. However, in the future I'll probably want to
                                    //differentiate between deleting without wearing off and deleting with wearing off...
                                    //So this allows me to do that if I changed the rest of the deletion structure...
                                    if (tempStatusDuration > 0)
                                    {
                                        tempPlayer.StatusHandler.DeleteStatus(tempStatus);
                                    }
                                    else
                                    {
                                        tempStatus.Duration = 0;
                                    }
                                    
                                }
    
                                break;
                            #endregion

                            #region Update Player HP
                            case PacketType.UpdatePlayerHP:
                                tempPlayer = null;
                                tempPlayer = GameObjectHandler.FindPlayerObjByID(msg.ReadUInt16());
                                if (tempPlayer != null)
                                {
                                    tempPlayer.HP = msg.ReadInt32();
                                }
                                break;
                            #endregion

                            #region Update Player MP
                            case PacketType.UpdatePlayerMP:
                                tempPlayer = null;
                                tempPlayer = GameObjectHandler.FindPlayerObjByID(msg.ReadUInt16());
                                if (tempPlayer != null)
                                {
                                    tempPlayer.MP = msg.ReadInt32();
                                }
                                break;
                            #endregion

                            #region Position Update
                            case PacketType.PositionUpdate:
                                m_packetBuffer.ReadPacket(PacketType.PositionUpdate, msg);
                                break;
                            #endregion

                            #region Velocity Update
                            case PacketType.VelocityUpdate:
                                m_packetBuffer.ReadPacket(PacketType.VelocityUpdate, msg);
                                break;
                            #endregion

                            #region Acceleration Update
                            case PacketType.AccelerationUpdate:
                                m_packetBuffer.ReadPacket(PacketType.AccelerationUpdate, msg);
                                break;
                            #endregion

                            #region Connection Approved
                            case PacketType.Approved:
                                string tempMsg = msg.ReadString();
                                switch (tempMsg)
                                {
                                    case "MasterServer":
                                        /*
                                        if (m_masterServer.Equals(msg.SenderConnection))
                                            m_masterServer = msg.SenderConnection;
                                        else if (m_masterServer == msg.SenderConnection)
                                            m_masterServer = msg.SenderConnection;
                                        */
                                        break;
                                    case "Server":
                                        //m_server = msg.SenderConnection;
                                        break;
                                }
                                break;
                            #endregion

                            #region Queried Server List
                            case PacketType.ServerListQueryData:
                                m_serverList.Clear(); //This is to prevent duplicate server lists popping up due to lag? (we saw it happen during the demonstration)
                                int serverCount = msg.ReadInt32();
                                for (int i = 0; i < serverCount; ++i)
                                {
                                    m_serverList.Add(new Server(msg.ReadString(), msg.ReadInt32(), msg.ReadString(), msg.ReadInt32(), msg.ReadInt32()));
                                }
                                break;
                            #endregion

                            #region Queried Customization Data
                            case PacketType.CustomizationQuerySendData:
                                switch (msg.ReadString())
                                {
                                    #region Read Current Exp
                                    case "CurrentExpQuery":
                                        GlobalVariables.CharLoadout.Exp = msg.ReadInt32();
                                        break;
                                    #endregion

                                    #region Read Purchase Response
                                    case "PurchaseAbility":
                                        ServerPurchaseRequestResponse = msg.ReadString();
                                        break;
                                    #endregion

                                    #region Read Ability List
                                    case "AbilityListQuery":
                                        GlobalVariables.AbilityDict.Clear(); //Just incase...
                                        count = msg.ReadUInt32();
                                        for (int i = 0; i < count; ++i)
                                        {
                                            m_abilityInfoList.Add(new AbilityInfo(msg.ReadString(), msg.ReadString(), msg.ReadString(), msg.ReadInt32(), msg.ReadBoolean()));
                                        }

                                        GlobalVariables.PopulateAbilityDict(m_abilityInfoList);
                                        NetworkingHandler.QueryCurrentAbilities(); //Get the player's current chosen abilities
                                        break;
                                    #endregion

                                    #region Read Currently Selected Abilities
                                    case "CurrentAbilitiesQuery":
                                        GlobalVariables.CharLoadout.MainHand = msg.ReadString();
                                        GlobalVariables.CharLoadout.OffHand = msg.ReadString();
                                        GlobalVariables.CharLoadout.Passive = msg.ReadString();

                                        GlobalVariables.CharLoadout.Special1 = msg.ReadString();
                                        GlobalVariables.CharLoadout.Special2 = msg.ReadString();
                                        GlobalVariables.CharLoadout.Special3 = msg.ReadString();
                                        GlobalVariables.CharLoadout.Avatar = msg.ReadString();
                                        GlobalVariables.CharLoadoutPopulated = true;
                                        break;
                                    #endregion
                                }
                                break;
                            #endregion

                            #region Match End
                            case PacketType.MatchEnd:
                                WinnerName = msg.ReadString();
                                WinnerScore = msg.ReadInt32();
                                game1.currGameState = GameState.InitalizeBetweenMatches;
                                break;
                            #endregion
                        }
                        break;
                    #endregion

                    #region Status Changed
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)msg.ReadByte())
                        {
                            case NetConnectionStatus.Disconnected:
                                m_masterServerConnectionState = msg.ReadString();
                                break;

                            default:
                                Console.WriteLine("Unhandled Status Change: " + msg.SenderConnection.Status.ToString());
                                break;
                        }
                        break;
                    #endregion

                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }
                m_client.Recycle(msg);
            }
            #endregion
        }

        //****************************************************
        // Method: ApplyVectListBuffers
        //
        // Purpose: Calls ApplyVectListBuffers from
        // m_packetBuffer
        //****************************************************
        static public void ApplyVectListBuffers()
        {
            m_packetBuffer.ApplyVectListBuffers();
        }

        //****************************************************
        // Method: SendControllerStateUpdate
        //
        // Purpose: Sends the state of a player's gamepad
        // to every other player
        //****************************************************
        static public void SendControllerStateUpdate(UInt16 controllerState)
        {
            if (m_server != null)
            {
                NetOutgoingMessage sendMsg = m_client.CreateMessage();
                sendMsg.Write((byte)PacketType.ControllerStateUpdate);
                sendMsg.Write(controllerState);
                m_client.SendMessage(sendMsg, m_server, NetDeliveryMethod.UnreliableSequenced, (int)PacketSequence.ControllerStateUpdate);
            }
        }


        //****************************************************
        // Method: QueryServers
        //
        // Purpose: Queries the master server for all the
        // servers connected to it
        //****************************************************
        static public void QueryServers()
        {
            m_serverList.Clear();
            NetOutgoingMessage queryMessage;
            queryMessage = m_client.CreateMessage();
            queryMessage.Write((byte)PacketType.ServerListQueryData);

            m_client.SendMessage(queryMessage, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
        }

        //****************************************************
        // Method: QueryAbilityList
        //
        // Purpose: Queries the master server for the
        // list of abilities that you own and can purchase
        //****************************************************
        static public void QueryAbilityList()
        {
            m_abilityInfoList.Clear();
            NetOutgoingMessage queryMessage;
            queryMessage = m_client.CreateMessage();
            queryMessage.Write((byte)PacketType.CustomizationQuerySendData);
            queryMessage.Write("AbilityListQuery");

            m_client.SendMessage(queryMessage, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);

        }

        //****************************************************
        // Method: QueryCurrentAbilities
        //
        // Purpose: Queries the master server for the
        // currently chosen abilities/weapons/passive that
        // the user has chosen
        //****************************************************
        static public void QueryCurrentAbilities()
        {
            m_abilityInfoList.Clear();
            NetOutgoingMessage queryMessage;
            queryMessage = m_client.CreateMessage();
            queryMessage.Write((byte)PacketType.CustomizationQuerySendData);
            queryMessage.Write("CurrentAbilitiesQuery");

            m_client.SendMessage(queryMessage, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);

        }

        //****************************************************
        // Method: UpdateSelectedAbility
        //
        // Purpose: Sends an update message to the master
        // server indicating the category that was changed
        //****************************************************
        static public void UpdateSelectedAbility(string category, string abilityName)
        {
            NetOutgoingMessage queryMessage;
            queryMessage = m_client.CreateMessage();
            queryMessage.Write((byte)PacketType.CustomizationQuerySendData);
            queryMessage.Write("SelectedAbilityUpdate");

            //The server should be expecting Ability #1, Ability #2, Ability #3... and it should validate that the abilityName is valid
            queryMessage.Write(category);

            queryMessage.Write(abilityName);

            m_client.SendMessage(queryMessage, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
        }

        //****************************************************
        // Method: PurchaseAbility
        //
        // Purpose: Sends an message to the master server
        // indicating that the user wishes to purchase a new
        // ability
        //****************************************************
        static public void PurchaseAbility(string abilitySlot, string abilityName)
        {
            NetOutgoingMessage queryMessage;
            queryMessage = m_client.CreateMessage();
            queryMessage.Write((byte)PacketType.CustomizationQuerySendData);
            queryMessage.Write("PurchaseAbility");

            //The server should be expecting Ability #1, Ability #2, Ability #3... and it should validate that the abilityName is valid
            queryMessage.Write(abilitySlot);

            queryMessage.Write(abilityName);

            m_client.SendMessage(queryMessage, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
        }

        //****************************************************
        // Method: QueryCurrentExp
        //
        // Purpose: Sends an message to the master server
        // requesting the user's current exp
        //****************************************************
        static public void QueryCurrentExp()
        {
            GlobalVariables.CharLoadout.Exp = -1;
            NetOutgoingMessage queryMessage;
            queryMessage = m_client.CreateMessage();
            queryMessage.Write((byte)PacketType.CustomizationQuerySendData);
            queryMessage.Write("CurrentExpQuery");

            m_client.SendMessage(queryMessage, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
        }

        //****************************************************
        // Method: ServerDisconnect
        //
        // Purpose: Disconnects from the server
        //****************************************************
        static public void ServerDisconnect(string reason = "User Request")
        {
            //Needs to be run only when you disconnect for real? sldakjlkdsjgl;sjadg
            GameObjectHandler.Reset();

            //m_client.Disconnect(reason); //How does this differentiate between server and master server?!?!
            if (m_server != null)
                m_server.Disconnect(reason);
        }

        //****************************************************
        // Method: RegisterNewAccount
        //
        // Purpose: This function sends an unconnected
        // message to the master server, containing a
        // user name and password
        //****************************************************
        static public void RegisterNewAccount(string userName, string password)
        {
            NetOutgoingMessage registerMessage;
            registerMessage = m_client.CreateMessage();
            registerMessage.Write((byte)PacketType.RegistrationRequestData);
            registerMessage.Write(userName);
            registerMessage.Write(password);

            m_client.Connect(GlobalVariables.Settings.MasterServerIP, 35335, registerMessage);
        }

        //****************************************************
        // Method: RegisterNewAccountListener
        //
        // Purpose: This function is put in a while loop
        // in the RegistrationMenu. It returns null until
        // it hears a message from the master server.
        // Related to the way WorldInit works
        //****************************************************
        static public void RegisterNewAccountListener(ref string response)
        {
            response = null;
            //NOTE: Make sure to prevent the game from crashing if the server fraks up
            NetIncomingMessage incMsg;
            while ((incMsg = m_client.ReadMessage()) != null)
            {
                switch (incMsg.MessageType)
                {

                    #region Status Changed
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)incMsg.ReadByte())
                        {
                            #region Read Registration Request Response
                            case NetConnectionStatus.Disconnected:
                                response = incMsg.ReadString();
                                break;
                            #endregion
                        }
                        break;
                    #endregion
                    default:
                        Console.WriteLine("Unhandled Msg (" + incMsg.MessageType.ToString() + "): " + incMsg.ReadString());
                        break;
                }
            }


        }


        //****************************************************
        // Method: MasterServerDisconnect
        //
        // Purpose: Disconnects from the server
        //****************************************************
        static public void MasterServerDisconnect(string reason = "User Request")
        {
            if (m_masterServer != null)
                m_masterServer.Disconnect(reason);
        }


        //****************************************************
        // Method: ConnectToMasterServer
        //
        // Purpose: Connects to the master server
        //****************************************************
        static public void ConnectToMasterServer(string userName, string password) //password should be a hash?
        {
            NetOutgoingMessage hailMessage;
            hailMessage = m_client.CreateMessage();
            hailMessage.Write((byte)PacketType.Hail);
            hailMessage.Write("Client" + "," + userName + "," + password);
            m_masterServer = m_client.Connect(GlobalVariables.Settings.MasterServerIP, 35335, hailMessage);
        }

        #region Properties
        public static float AverageRoundTripToServer
        {
            get
            {
                return m_server.AverageRoundtripTime;
            }
        }

        public static List<Server> ServerList
        {
            get
            {
                return m_serverList;
            }
        }

        public static List<AbilityInfo> AbilityInfoList
        {
            get
            {
                return m_abilityInfoList;
            }
        }

        public static string MasterServerConnectionState
        {
            get
            {
                return m_masterServerConnectionState;
            }
            set
            {
                m_masterServerConnectionState = value;
            }
        }

        public static NetConnection MasterServerConnection
        {
            get
            {
                return m_masterServer;
            }
        }

        public static NetConnection CurrentServerConnection
        {
            get
            {
                return m_server;
            }
        }
        #endregion
    }
}

