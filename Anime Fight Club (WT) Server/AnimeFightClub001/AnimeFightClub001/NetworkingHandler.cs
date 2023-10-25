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
using Microsoft.Xna.Framework.Net;
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
        static NetServer m_server;
        static NetConnection m_masterServer; //Pointer to master server connection
        private static int m_posUpdateFrameCount = 0;
        private static int m_velUpdateFrameCount = 0;
        private static int m_accelUpdateFrameCount = 0;

        //This is a dictionary of all currently connected user's names paired with their connections
        private static Dictionary<string, NetConnection> m_userNameToConnection = new Dictionary<string, NetConnection>();

        //This is a list of all user names whose loadouts haven't be requested from the master server because we're still in between
        //matches
        private static List<string> m_usersWaitingForMatchStart = new List<string>();
        #endregion

        //****************************************************
        // Method: StartServer
        //
        // Purpose: Initializes required variables and
        // starts the server (needs to be generalized)
        //****************************************************
        static public void StartServer()
        {
            m_config.Port = GlobalVariables.Settings.ServerPort;


            m_server = new NetServer(m_config);
            m_config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            m_server.Start();

            ConnectToMasterServer();
        }


        //****************************************************
        // Method: SendPosUpdateToAll
        //
        // Purpose: Increments the PosUpdateTimer and
        // when it's >= to the PosUpdateSendFrame value
        // it sends the update and resets the timer.
        //****************************************************
        static public void SendPosUpdateToAll(GameTime gameTime)
        {
            ++m_posUpdateFrameCount;

            if (m_posUpdateFrameCount >= GlobalVariables.Settings.PosUpdateSendFrame)
            {
                NetOutgoingMessage sendMsg;
                sendMsg = null;
                sendMsg = m_server.CreateMessage();
                sendMsg.Write((byte)PacketType.PositionUpdate);
                sendMsg.Write(GlobalVariables.CurrentFrame);

                #region Write PlayerObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.PlayerObjList.Count));


                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                {
                    sendMsg.Write(player.ObjectID);
                    sendMsg.Write(player.PhysicsObj.Pos.X);
                    sendMsg.Write(player.PhysicsObj.Pos.Y);
                }
                #endregion

                #region Write HazardObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.HazardObjList.Count));

                foreach (HazardObj hazard in GameObjectHandler.HazardObjList)
                {
                    sendMsg.Write(hazard.ObjectID);
                    sendMsg.Write(hazard.PhysicsObj.Pos.X);
                    sendMsg.Write(hazard.PhysicsObj.Pos.Y);
                }
                #endregion

                #region Write EnvironmentalObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.EnvironmentalObjList.Count));

                foreach (EnvironmentalObj environmentalObj in GameObjectHandler.EnvironmentalObjList)
                {
                    sendMsg.Write(environmentalObj.ObjectID);
                    sendMsg.Write(environmentalObj.PhysicsObj.Pos.X);
                    sendMsg.Write(environmentalObj.PhysicsObj.Pos.Y);
                }
                #endregion

                #region Write SpecialEnvironmentalObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.SpecialEnvironmentalObjList.Count));

                foreach (SpecialEnvironmentalObj specialEnvironmentalObj in GameObjectHandler.SpecialEnvironmentalObjList)
                {
                    sendMsg.Write(specialEnvironmentalObj.ObjectID);
                    sendMsg.Write(specialEnvironmentalObj.PhysicsObj.Pos.X);
                    sendMsg.Write(specialEnvironmentalObj.PhysicsObj.Pos.Y);
                }
                #endregion

                #region Write ItemObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.ItemObjList.Count));

                foreach (ItemObj itemObj in GameObjectHandler.ItemObjList)
                {
                    sendMsg.Write(itemObj.ObjectID);
                    sendMsg.Write(itemObj.PhysicsObj.Pos.X);
                    sendMsg.Write(itemObj.PhysicsObj.Pos.Y);
                }
                #endregion

                //m_server.SendToAll(sendMsg, NetDeliveryMethod.UnreliableSequenced);
                m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.UnreliableSequenced, (int)PacketSequence.PositionUpdate);

                m_posUpdateFrameCount = 0;
            }
        }

        //****************************************************
        // Method: SendVelUpdateToAll
        //
        // Purpose: Increments the VelUpdateTimer and
        // when it's >= to the VelUpdateSendTime value
        // it sends the update and resets the timer.
        //****************************************************
        static public void SendVelUpdateToAll(GameTime gameTime)
        {
            ++m_velUpdateFrameCount;

            if (m_velUpdateFrameCount >= GlobalVariables.Settings.VelUpdateSendFrame)
            {
                NetOutgoingMessage sendMsg;
                sendMsg = null;
                sendMsg = m_server.CreateMessage();
                sendMsg.Write((byte)PacketType.VelocityUpdate);
                sendMsg.Write(GlobalVariables.CurrentFrame);

                #region Write PlayerObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.PlayerObjList.Count));


                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                {
                    sendMsg.Write(player.ObjectID);
                    sendMsg.Write(player.PhysicsObj.VelX);
                    sendMsg.Write(player.PhysicsObj.VelY);
                }
                #endregion

                #region Write HazardObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.HazardObjList.Count));

                foreach (HazardObj hazard in GameObjectHandler.HazardObjList)
                {
                    sendMsg.Write(hazard.ObjectID);
                    sendMsg.Write(hazard.PhysicsObj.Vel.X);
                    sendMsg.Write(hazard.PhysicsObj.Vel.Y);
                }
                #endregion

                #region Write EnvironmentalObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.EnvironmentalObjList.Count));

                foreach (EnvironmentalObj environmentalObj in GameObjectHandler.EnvironmentalObjList)
                {
                    sendMsg.Write(environmentalObj.ObjectID);
                    sendMsg.Write(environmentalObj.PhysicsObj.Vel.X);
                    sendMsg.Write(environmentalObj.PhysicsObj.Vel.Y);
                }
                #endregion

                #region Write SpecialEnvironmentalObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.SpecialEnvironmentalObjList.Count));

                foreach (SpecialEnvironmentalObj specialEnvironmentalObj in GameObjectHandler.SpecialEnvironmentalObjList)
                {
                    sendMsg.Write(specialEnvironmentalObj.ObjectID);
                    sendMsg.Write(specialEnvironmentalObj.PhysicsObj.Vel.X);
                    sendMsg.Write(specialEnvironmentalObj.PhysicsObj.Vel.Y);
                }
                #endregion

                #region Write ItemObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.ItemObjList.Count));

                foreach (ItemObj itemObj in GameObjectHandler.ItemObjList)
                {
                    sendMsg.Write(itemObj.ObjectID);
                    sendMsg.Write(itemObj.PhysicsObj.Vel.X);
                    sendMsg.Write(itemObj.PhysicsObj.Vel.Y);
                }
                #endregion

                //m_server.SendToAll(sendMsg, NetDeliveryMethod.UnreliableSequenced);
                m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.UnreliableSequenced, (int)PacketSequence.VelocityUpdate);

                m_velUpdateFrameCount = 0;
            }
        }

        //****************************************************
        // Method: SendAccelUpdateToAll
        //
        // Purpose: Increments the AccelUpdateTimer and
        // when it's >= to the AccelUpdateSendTime value
        // it sends the update and resets the timer.
        //****************************************************
        static public void SendAccelUpdateToAll(GameTime gameTime)
        {
            ++m_accelUpdateFrameCount;

            if (m_accelUpdateFrameCount >= GlobalVariables.Settings.AccelUpdateSendFrame)
            {
                NetOutgoingMessage sendMsg;
                sendMsg = null;
                sendMsg = m_server.CreateMessage();
                sendMsg.Write((byte)PacketType.AccelerationUpdate);
                sendMsg.Write(GlobalVariables.CurrentFrame);

                #region Write PlayerObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.PlayerObjList.Count));


                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                {
                    sendMsg.Write(player.ObjectID);
                    sendMsg.Write(player.PhysicsObj.AccelX);
                    sendMsg.Write(player.PhysicsObj.AccelY);
                }
                #endregion

                #region Write HazardObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.HazardObjList.Count));

                foreach (HazardObj hazard in GameObjectHandler.HazardObjList)
                {
                    sendMsg.Write(hazard.ObjectID);
                    sendMsg.Write(hazard.PhysicsObj.Accel.X);
                    sendMsg.Write(hazard.PhysicsObj.Accel.Y);
                }
                #endregion

                #region Write EnvironmentalObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.EnvironmentalObjList.Count));

                foreach (EnvironmentalObj environmentalObj in GameObjectHandler.EnvironmentalObjList)
                {
                    sendMsg.Write(environmentalObj.ObjectID);
                    sendMsg.Write(environmentalObj.PhysicsObj.Accel.X);
                    sendMsg.Write(environmentalObj.PhysicsObj.Accel.Y);
                }
                #endregion

                #region Write SpecialEnvironmentalObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.SpecialEnvironmentalObjList.Count));

                foreach (SpecialEnvironmentalObj specialEnvironmentalObj in GameObjectHandler.SpecialEnvironmentalObjList)
                {
                    sendMsg.Write(specialEnvironmentalObj.ObjectID);
                    sendMsg.Write(specialEnvironmentalObj.PhysicsObj.Accel.X);
                    sendMsg.Write(specialEnvironmentalObj.PhysicsObj.Accel.Y);
                }
                #endregion

                #region Write ItemObj Data
                sendMsg.Write((UInt32)(GameObjectHandler.ItemObjList.Count));

                foreach (ItemObj itemObj in GameObjectHandler.ItemObjList)
                {
                    sendMsg.Write(itemObj.ObjectID);
                    sendMsg.Write(itemObj.PhysicsObj.Accel.X);
                    sendMsg.Write(itemObj.PhysicsObj.Accel.Y);
                }
                #endregion

                //m_server.SendToAll(sendMsg, NetDeliveryMethod.UnreliableSequenced);
                m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.UnreliableSequenced, (int)PacketSequence.AccelerationUpdate);

                m_accelUpdateFrameCount = 0;
            }
        }

        //****************************************************
        // Method: HandleMessages
        //
        // Purpose: Reads and handles all network messages
        //****************************************************
        static public void HandleMessages(Game1 game1)
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

                    #region New Connection
                    case NetIncomingMessageType.ConnectionApproval:
                        if (msg.ReadByte() == (byte)PacketType.Hail)
                        {
                            string userName = msg.ReadString();
                            if (!(m_userNameToConnection.ContainsKey(userName) && m_userNameToConnection[userName].Status != NetConnectionStatus.Disconnected))
                            {
                                if (m_userNameToConnection.ContainsKey(userName))
                                {
                                    m_userNameToConnection.Remove(userName);
                                }
                                #region Approval
                                //Check if master server agrees that this client is trying to connect
                                msg.SenderConnection.Approve(); //Connect the client
                                #endregion
                                m_userNameToConnection.Add(userName, msg.SenderConnection);

                                if (game1.currGameState != GameState.InitalizeBetweenMatches && game1.currGameState != GameState.BetweenMatches)
                                {
                                    QueryCurrentAbilities(userName);
                                }
                                else
                                {
                                    m_usersWaitingForMatchStart.Add(userName);
                                }
                            }
                            else
                            {
                                msg.SenderConnection.Deny("Someone with that username is already connected to this server!");
                            }
                        }
                        break;
                    #endregion

                    #region Read Data
                    case NetIncomingMessageType.Data:
                        switch ((PacketType)msg.ReadByte())
                        {
                            #region Controller State Update
                            case PacketType.ControllerStateUpdate:

                                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                                {
                                    if (player.PlayerID == msg.SenderConnection.RemoteUniqueIdentifier)
                                    {
                                        UInt16 ctrlState = msg.ReadUInt16();
                                        player.ControllerState.setControlStateAsInt16(ctrlState);
                                        SendControllerStateUpdate(msg.SenderConnection, msg.SenderConnection.RemoteUniqueIdentifier, ctrlState);
                                        break;
                                    }
                                }
                                break;
                            #endregion

                            #region Read Customization Query Data
                            case PacketType.CustomizationQuerySendData:
                                switch (msg.ReadString())
                                {
                                    #region Read Currently Selected Abilities
                                    case "CurrentAbilitiesServerQuery":
                                        string userName = msg.ReadString();

                                        #region Server Side Player Add
                                        //Add player to game on server side
                                        PlayerObj newPlayer = new PlayerObj(m_userNameToConnection[userName].RemoteUniqueIdentifier, userName, msg);

                                        GameObjectHandler.DirectAddObject(newPlayer);

                                        //***************************************************************************************************
                                        //TEST!!!
                                        GameObjectHandler.TrackingCamera.TrackingObj = GameObjectHandler.PlayerObjList.Last.Value.PhysicsObj;
                                        //***************************************************************************************************

                                        #endregion

                                        #region Send AddPlayer Message to All Current Players
                                        SendAddObjMessage(newPlayer);
                                        //sendMsg = m_server.CreateMessage();
                                        //sendMsg.Write((byte)PacketType.AddPlayerObj);

                                        //Write in all the information
                                        //newPlayer.NetSendMe(sendMsg);

                                        //line below was used to prevent this message from being sent to the player joining. However it was
                                        //deemed unneccessary due to the fact that the player joining cannot accept any messages from the server
                                        //until they recieve a WorldInit packet:
                                        //m_server.SendToAll(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, msg.SequenceChannel);
                                        //m_server.SendToAll(sendMsg,NetDeliveryMethod.ReliableUnordered);
                                        //m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeletePlayerObj);
                                        #endregion

                                        SendWorldInit(userName);
                                        

                                        break;
                                    #endregion
                                }
                                break;
                            #endregion
                        }
                        break;
                    #endregion

                    #region Status Changed
                    case NetIncomingMessageType.StatusChanged:
                        #region Disconnected
                        switch ((NetConnectionStatus)msg.ReadByte())
                        {
                            case NetConnectionStatus.Disconnected:
                                if (msg.SenderConnection == m_masterServer)
                                {
                                    System.Console.WriteLine(msg.ReadString());
                                }
                                else
                                {
                                    //Call player disconnected function here instead?
                                    PlayerObj temp = null;
                                    foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                                    {
                                        if (player.PlayerID == msg.SenderConnection.RemoteUniqueIdentifier)
                                        {
                                            temp = player;
                                            break;
                                        }
                                    }
                                    if (temp != null)
                                    {
                                        m_userNameToConnection.Remove(temp.UserName);
                                        SendDeleteObjMessage(temp);
                                        GameObjectHandler.DirectDeleteObject(temp);
                                    }
                                }
                                break;
                        }
                        break;
                        #endregion

                        #region Connect
                        //Should be replaced with a better method built into lidgren?
                        #region Send Approval Message
                        sendMsg = null;
                        sendMsg = m_server.CreateMessage();
                        sendMsg.Write((byte)PacketType.Approved);
                        sendMsg.Write("Server");

                        m_server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered, (int)PacketSequence.GenericDataRequest);
                        #endregion
                        #endregion
                        break;
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
        // Method: SendControllerStateUpdate
        //
        // Purpose: Sends the state of a player's gamepad
        // to every other player
        //****************************************************
        static public void SendControllerStateUpdate(NetConnection ctrlStateSender, long playerId, UInt16 controllerState)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.ControllerStateUpdate);
            sendMsg.Write(playerId);
            sendMsg.Write(controllerState);
            //m_server.SendToAll(sendMsg, NetDeliveryMethod.UnreliableSequenced); //Add exception so that it doesn't send to the player it came from!?
            List<NetConnection> tempConnectionList = m_server.Connections; //Used to exclude the sender
            tempConnectionList.Remove(ctrlStateSender);
            m_server.SendMessage(sendMsg, tempConnectionList, NetDeliveryMethod.UnreliableSequenced, (int)PacketSequence.ControllerStateUpdate);
        }

        //****************************************************
        // Method: SendStateUpdate
        //
        // Purpose: Sends the state of an object to all
        // clients. Generally only called if an object
        // changes its state
        //****************************************************
        static public void SendStateUpdate(BasicObj obj, string objType, int objState)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.ObjectStateUpdate);
            //sendMsg.Write(objType); //Send the base type? or maybe have different types of objectstate updates for each base type????
            sendMsg.Write(objType);
            sendMsg.Write(obj.ObjectID);
            sendMsg.Write(objState);
            if (objType == "PlayerObj" && (PlayerState)objState == PlayerState.LedgeGrabbing)
            {
                sendMsg.Write(((PlayerObj)obj).LedgeGrabbingPos.X);
                sendMsg.Write(((PlayerObj)obj).LedgeGrabbingPos.Y);
            }

            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.UnreliableSequenced, (int)PacketSequence.ObjectStateUpdate);
        }

        //****************************************************
        // Method: SendDeleteStatusMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to delete the specified status
        // from the specific player
        //****************************************************
        public static void SendDeleteStatusMessage(Status status)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.DeletePlayerStatus);
            sendMsg.Write(status.OwnerObj.ObjectID);
            sendMsg.Write(status.StatusID);
            sendMsg.Write(status.Duration);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendAddStatusMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to add the specified status
        // to the specific player
        //****************************************************
        public static void SendAddStatusMessage(UInt16 ownerID, StatusName name, int severity, UInt16 statusID, PlayerObj inflictorObj)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.AddPlayerStatus);
            sendMsg.Write(ownerID);
            sendMsg.Write((byte)name);
            sendMsg.Write(severity);
            sendMsg.Write(statusID);
            if (inflictorObj != null)
            {
                sendMsg.Write(inflictorObj.ObjectID);
            }
            else
            {
                sendMsg.Write(0);
            }
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendPlayerHPUpdate
        //
        // Purpose: Sends a message to all clients updating
        // a specific player's HP
        //****************************************************
        public static void SendPlayerHPUpdate(UInt16 playerID, int HP)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.UpdatePlayerHP);
            sendMsg.Write(playerID);
            sendMsg.Write(HP);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendPlayerMPUpdate
        //
        // Purpose: Sends a message to all clients updating
        // a specific player's MP
        //****************************************************
        public static void SendPlayerMPUpdate(UInt16 playerID, int MP)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.UpdatePlayerMP);
            sendMsg.Write(playerID);
            sendMsg.Write(MP);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        #region SendDeleteObjMessage Methods
        //****************************************************
        // Method: SendDeleteObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to delete the specified object.
        //****************************************************
        public static void SendDeleteObjMessage(PlayerObj player)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.DeletePlayerObj);
            sendMsg.Write(player.ObjectID);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendDeleteObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to delete the specified object.
        //****************************************************
        public static void SendDeleteObjMessage(HazardObj hazard)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.DeleteHazardObj);
            sendMsg.Write(hazard.ObjectID);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendDeleteObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to delete the specified object.
        //****************************************************
        public static void SendDeleteObjMessage(EnvironmentalObj environmentalObj)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.DeleteEnvironmentalObj);
            sendMsg.Write(environmentalObj.ObjectID);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendDeleteObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to delete the specified object.
        //****************************************************
        public static void SendDeleteObjMessage(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.DeleteSpecialEnvironmentalObj);
            sendMsg.Write(specialEnvironmentalObj.ObjectID);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendDeleteObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to delete the specified object.
        //****************************************************
        public static void SendDeleteObjMessage(ItemObj itemObj)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.DeleteItemObj);
            sendMsg.Write(itemObj.ObjectID);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        #endregion

        #region SendAddObjMessage Methods
        //****************************************************
        // Method: SendAddObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to add the specified object.
        //****************************************************
        public static void SendAddObjMessage(PlayerObj player)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.AddPlayerObj);
            player.NetSendMe(sendMsg);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendAddObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to add the specified object.
        //****************************************************
        public static void SendAddObjMessage(HazardObj hazard)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.AddHazardObj);
            hazard.NetSendMe(sendMsg);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendAddObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to add the specified object.
        //****************************************************
        public static void SendAddObjMessage(EnvironmentalObj environmentalObj)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.AddEnvironmentalObj);
            environmentalObj.NetSendMe(sendMsg);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendAddObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to add the specified object.
        //****************************************************
        public static void SendAddObjMessage(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.AddSpecialEnvironmentalObj);
            specialEnvironmentalObj.NetSendMe(sendMsg);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        //****************************************************
        // Method: SendAddObjMessage
        //
        // Purpose: Sends a message to all clients
        // instructing them to add the specified object.
        //****************************************************
        public static void SendAddObjMessage(ItemObj itemObj)
        {
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.AddItemObj);
            itemObj.NetSendMe(sendMsg);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.AddDeleteObj);
        }

        #endregion

        //****************************************************
        // Method: QueryCurrentAbilities
        //
        // Purpose: Queries the master server for the
        // currently chosen abilities/weapons/passive that
        // the user has chosen
        //****************************************************
        static public void QueryCurrentAbilities(string userName)
        {
            NetOutgoingMessage queryMessage;
            queryMessage = m_server.CreateMessage();
            queryMessage.Write((byte)PacketType.CustomizationQuerySendData);
            queryMessage.Write("CurrentAbilitiesServerQuery");
            queryMessage.Write(userName);
            m_server.SendMessage(queryMessage, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);

        }

        //****************************************************
        // Method: ConnectToMasterServer
        //
        // Purpose: Connects to the master server
        //****************************************************
        static public void ConnectToMasterServer()
        {
            NetOutgoingMessage hailMessage;
            hailMessage = m_server.CreateMessage();
            hailMessage.Write((byte)PacketType.Hail);
            hailMessage.Write("Server" + "," + GlobalVariables.Settings.ServerName + "," + m_config.Port.ToString());
            m_masterServer = m_server.Connect(GlobalVariables.Settings.MasterServerIP, 35335, hailMessage);
        }

        //****************************************************
        // Method: SendMatchEndToMasterServer
        //
        // Purpose: Sends the details of the round to the
        // master server
        //****************************************************
        static public void SendMatchEndToMasterServer()
        {
            NetOutgoingMessage sendMsg;
            sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.MatchEnd);
            sendMsg.Write(GameObjectHandler.PlayerObjList.Count);
            foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
            {
                sendMsg.Write(player.UserName);
                sendMsg.Write(player.Kills);
            }
            m_server.SendMessage(sendMsg, m_masterServer, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
        }

        //****************************************************
        // Method: SendMatchEndToPlayers
        //
        // Purpose: Sends a packet to the players indicating
        // the end of the match and who the winner was and
        // what their score was
        //****************************************************
        static public void SendMatchEndToPlayers()
        {
            PlayerObj tempPlayer = null;
            foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
            {
                if (player.Kills >= GlobalVariables.Settings.KillLimit)
                {
                    tempPlayer = player;
                    break;
                }
            }
            
            NetOutgoingMessage sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.MatchEnd);
            sendMsg.Write(tempPlayer.UserName);
            sendMsg.Write(tempPlayer.Kills);
            m_server.SendMessage(sendMsg, m_server.Connections, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.GenericDataRequest);
        }

        //****************************************************
        // Method: StartNextMatch
        //
        // Purpose: Runs all the networking code to  start
        // the next match, such as send all the previously
        // connected players their WorldInits and querying
        // for all the waiting player's loadouts
        //****************************************************
        static public void StartNextMatch()
        {
            
        }

        //****************************************************
        // Method: SendWorldInit
        //
        // Purpose: Sends all the current information about
        // the world to a newly connecting player. 
        // Also used to send previously connected players the
        // new map information after a map cylce.
        //****************************************************
        static public void SendWorldInit(string userName)
        {
            #region Send WorldInit Message To New Player
            NetOutgoingMessage sendMsg = null;
            sendMsg = m_server.CreateMessage();
            sendMsg.Write((byte)PacketType.WorldInit);
            sendMsg.Write(GlobalVariables.CurrentFrame);

            #region Write PlayerObj Data
            sendMsg.Write((UInt32)(GameObjectHandler.PlayerObjList.Count));


            foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
            {
                player.NetSendMe(sendMsg);
            }
            #endregion

            #region Write HazardObj Data
            sendMsg.Write((UInt32)(GameObjectHandler.HazardObjList.Count));

            foreach (HazardObj hazard in GameObjectHandler.HazardObjList)
            {
                hazard.NetSendMe(sendMsg);
            }
            #endregion

            #region Write EnvironmentalObj Data
            sendMsg.Write((UInt32)(GameObjectHandler.EnvironmentalObjList.Count));

            foreach (EnvironmentalObj environmentalObj in GameObjectHandler.EnvironmentalObjList)
            {
                environmentalObj.NetSendMe(sendMsg);
            }
            #endregion

            #region Write SpecialEnvironmentalObj Data
            sendMsg.Write((UInt32)(GameObjectHandler.SpecialEnvironmentalObjList.Count));

            foreach (SpecialEnvironmentalObj specialEnvironmentalObj in GameObjectHandler.SpecialEnvironmentalObjList)
            {
                specialEnvironmentalObj.NetSendMe(sendMsg);
            }
            #endregion

            #region Write ItemObj Data
            sendMsg.Write((UInt32)(GameObjectHandler.ItemObjList.Count));

            foreach (ItemObj itemObj in GameObjectHandler.ItemObjList)
            {
                itemObj.NetSendMe(sendMsg);
            }
            #endregion

            NetConnection tempConnection = m_userNameToConnection[userName];//null;
            /*
            foreach (NetConnection netConnection in m_server.Connections)
            {
                if (netConnection.RemoteUniqueIdentifier == m_userNameToConnection[userName])
                {
                    tempConnection = netConnection;
                    break;
                }
            }
            */
            if (tempConnection != null)
                m_server.SendMessage(sendMsg, tempConnection, NetDeliveryMethod.ReliableOrdered, (int)PacketSequence.WorldInit);
            #endregion
        }

        //****************************************************
        // Method: NextMap
        //
        // Purpose: Runs all the code to tell players that the
        // next map is loaded, and to let new comers join in.
        //****************************************************
        static public void NextMap()
        {
            foreach (KeyValuePair<string, NetConnection> userNameConnPair in m_userNameToConnection)
            {
                if (!m_usersWaitingForMatchStart.Contains(userNameConnPair.Key))
                {
                    SendWorldInit(userNameConnPair.Key);
                }
            }

            foreach (string userName in m_usersWaitingForMatchStart)
            {
                QueryCurrentAbilities(userName);
            }
            m_usersWaitingForMatchStart.Clear();
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
    }
}
