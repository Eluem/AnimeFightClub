//******************************************************
// File: VectListUpdatePacket.cs
//
// Purpose: The VectListUpdatePacket class will be used
// to store a list of either position updates,
// velocity updates, or acceleration updates.
// It will also store the object id of all objects
// that these updates apply to, along with the
// number of each type of object which is being.
// It will also store a time stamp.
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
    class VectListUpdatePacket
    {
        #region Declarations
        protected UInt32 m_timeStamp; //The time, translated into local time, that this packet was sent
        protected List<Vector2> m_vectorList;
        protected List<UInt16> m_idList;
        protected UInt32[] m_countList;
        #endregion

        #region Constants
        const int NUM_OF_LISTS = 5;
        #endregion

        //****************************************************
        // Method: VectListUpdatePacket
        //
        // Purpose: Default VectListUpdatePacket constructor
        // Reads in all the information from the packet.
        //****************************************************
        public VectListUpdatePacket(NetIncomingMessage msg)
        {
            #region Initializations
            m_countList = new UInt32[NUM_OF_LISTS];
            m_vectorList = new List<Vector2>();
            m_idList = new List<ushort>();
            #endregion

            m_timeStamp = msg.ReadUInt32(); //Read the time stamp in. (Is converting it to a local time useful, should I do it here?)

            //Loop through all the lists and collect all their data
            for (int i = 0; i < NUM_OF_LISTS; ++i)
            {
                m_countList[i] = msg.ReadUInt32();
                for (int j = 0; j < m_countList[i]; ++j)
                {
                    m_idList.Add(msg.ReadUInt16()); //Get the current objID
                    m_vectorList.Add(new Vector2(msg.ReadFloat(), msg.ReadFloat()));
                }
            }

        }

        //**************************************************
        // The ApplyPacket functions below will be called
        // from within the PacketBuffer class.
        // It will store 3 different lists of
        // VectListPacketUpdate instances. One for
        // Pos, Vel, and Accel. It will determine which
        // function to call. (This is basically due to
        // laziness and cleanliness. I felt it would be
        // wasteful to have 3 classes inheriting from a
        // base class simply to allow me to override
        // ONE of the functions.)
        //**************************************************

        #region Apply Packet Functions

        //****************************************************
        // Method: ApplyPacketToPos
        //
        // Purpose: This function will apply all the values
        // in the 2 arrays to the position values of the
        // objects in the game. (Since it is a position
        // update, interpolation will be used)
        //****************************************************
        public void ApplyPacketToPos()
        {
            #region Declarations/Initilizations
            int countAccumulator = 0; //Used to figure out where each loop should start and end
            int i = 0;
            #endregion

            #region Update PlayerObj Pos
            countAccumulator += (int)m_countList[0];
            for (; i < countAccumulator; ++i)
            {
                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                {
                    if (player.ObjectID == m_idList[i])
                    {
                        //player.PhysicsObj.PosX = m_vectorList[i].X;
                        //player.PhysicsObj.PosY = m_vectorList[i].Y;

                        //**********************************************
                        // Test Code!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //**********************************************
                        float tempX = m_vectorList[i].X;
                        float tempY = m_vectorList[i].Y;

                        //UInt32 frameDiff = m_timeStamp - (UInt32)GlobalVariables.CurrentFrame;

                        #region Crappy Lag Compensation
                        int frameDiff = (int)(NetworkingHandler.AverageRoundTripToServer * 60);
                        tempX += (player.Vel.X / GlobalVariables.Settings.fps) * frameDiff;
                        tempY += (player.Vel.Y / GlobalVariables.Settings.fps) * frameDiff;
                        #endregion

                        if (Math.Abs(tempX - player.Pos.X) <= 5)
                        {
                            tempX = (tempX - player.Pos.X) * .1f + player.Pos.X;
                        }

                        if (Math.Abs(tempY - player.Pos.Y) <= 5)
                        {
                            tempY = (tempY - player.Pos.Y) * .1f + player.Pos.Y;
                        }

                        if (player.ObjectID == GameObjectHandler.LocalPlayer.ObjectID)
                        {
                            //System.Console.WriteLine("LAG(ObjID:" + player.ObjectID + ", Time: " + frameDiff + /* NetworkingHandler.m_client.ServerConnection.AverageRoundtripTime * 1000 + */ "): " + (tempX - player.PhysicsObj.Pos.X) + ", " + (tempY - player.PhysicsObj.Pos.Y));
                        }

                        player.PhysicsObj.PosX = tempX;
                        player.PhysicsObj.PosY = tempY;

                        //**********************************************
                        // Test Code!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //**********************************************

                    }
                }
            }
            #endregion
            
            #region Update HazardObj Pos
            countAccumulator += (int)m_countList[1];
            for (; i < countAccumulator; ++i)
            {
                foreach (HazardObj hazard in GameObjectHandler.HazardObjList)
                {
                    if (hazard.ObjectID == m_idList[i])
                    {
                        hazard.PhysicsObj.PosX = m_vectorList[i].X;
                        hazard.PhysicsObj.PosY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update EnvironmentalObj Pos
            countAccumulator += (int)m_countList[2];
            for (; i < countAccumulator; ++i)
            {
                foreach (EnvironmentalObj environmentalObj in GameObjectHandler.EnvironmentalObjList)
                {
                    if (environmentalObj.ObjectID == m_idList[i])
                    {
                        environmentalObj.PhysicsObj.PosX = m_vectorList[i].X;
                        environmentalObj.PhysicsObj.PosY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update Speical EnvironmentalObj Pos
            countAccumulator += (int)m_countList[3];
            for (; i < countAccumulator; ++i)
            {
                foreach (SpecialEnvironmentalObj specialEnvironmentalObj in GameObjectHandler.SpecialEnvironmentalObjList)
                {
                    if (specialEnvironmentalObj.ObjectID == m_idList[i])
                    {
                        specialEnvironmentalObj.PhysicsObj.PosX = m_vectorList[i].X;
                        specialEnvironmentalObj.PhysicsObj.PosY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update ItemObj Pos
            countAccumulator += (int)m_countList[4];
            for (; i < countAccumulator; ++i)
            {
                foreach (ItemObj itemObj in GameObjectHandler.ItemObjList)
                {
                    if (itemObj.ObjectID == m_idList[i])
                    {
                        itemObj.PhysicsObj.PosX = m_vectorList[i].X;
                        itemObj.PhysicsObj.PosY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

        }

        //****************************************************
        // Method: ApplyPacketToVel
        //
        // Purpose: This function will apply all the values
        // in the 2 arrays to the velocity values of the
        // objects in the game. (Since it is a position
        // update, interpolation will be used)
        //****************************************************
        public void ApplyPacketToVel()
        {
            #region Declarations/Initilizations
            int countAccumulator = 0; //Used to figure out where each loop should start and end
            int i = 0;
            #endregion

            #region Update PlayerObj Vel
            countAccumulator += (int)m_countList[0];
            for (; i < countAccumulator; ++i)
            {
                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                {
                    if (player.ObjectID == m_idList[i])
                    {
                        player.PhysicsObj.VelX = m_vectorList[i].X;
                        player.PhysicsObj.VelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update HazardObj Vel
            countAccumulator += (int)m_countList[1];
            for (; i < countAccumulator; ++i)
            {
                foreach (HazardObj hazard in GameObjectHandler.HazardObjList)
                {
                    if (hazard.ObjectID == m_idList[i])
                    {
                        hazard.PhysicsObj.VelX = m_vectorList[i].X;
                        hazard.PhysicsObj.VelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update EnvironmentalObj Vel
            countAccumulator += (int)m_countList[2];
            for (; i < countAccumulator; ++i)
            {
                foreach (EnvironmentalObj environmentalObj in GameObjectHandler.EnvironmentalObjList)
                {
                    if (environmentalObj.ObjectID == m_idList[i])
                    {
                        environmentalObj.PhysicsObj.VelX = m_vectorList[i].X;
                        environmentalObj.PhysicsObj.VelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update Speical EnvironmentalObj Vel
            countAccumulator += (int)m_countList[3];
            for (; i < countAccumulator; ++i)
            {
                foreach (SpecialEnvironmentalObj specialEnvironmentalObj in GameObjectHandler.SpecialEnvironmentalObjList)
                {
                    if (specialEnvironmentalObj.ObjectID == m_idList[i])
                    {
                        specialEnvironmentalObj.PhysicsObj.VelX = m_vectorList[i].X;
                        specialEnvironmentalObj.PhysicsObj.VelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update ItemObj Vel
            countAccumulator += (int)m_countList[4];
            for (; i < countAccumulator; ++i)
            {
                foreach (ItemObj itemObj in GameObjectHandler.ItemObjList)
                {
                    if (itemObj.ObjectID == m_idList[i])
                    {
                        itemObj.PhysicsObj.VelX = m_vectorList[i].X;
                        itemObj.PhysicsObj.VelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

        }

        //****************************************************
        // Method: ApplyPacketToAccel
        //
        // Purpose: This function will apply all the values
        // in the 2 arrays to the acceleration values of the
        // objects in the game. (Since it is a position
        // update, interpolation will be used)
        //****************************************************
        public void ApplyPacketToAccel()
        {
            #region Declarations/Initilizations
            int countAccumulator = 0; //Used to figure out where each loop should start and end
            int i = 0;
            #endregion

            #region Update PlayerObj Accel
            countAccumulator += (int)m_countList[0];
            for (; i < countAccumulator; ++i)
            {
                foreach (PlayerObj player in GameObjectHandler.PlayerObjList)
                {
                    if (player.ObjectID == m_idList[i])
                    {
                        player.PhysicsObj.AccelX = m_vectorList[i].X;
                        player.PhysicsObj.AccelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update HazardObj Accel
            countAccumulator += (int)m_countList[1];
            for (; i < countAccumulator; ++i)
            {
                foreach (HazardObj hazard in GameObjectHandler.HazardObjList)
                {
                    if (hazard.ObjectID == m_idList[i])
                    {
                        hazard.PhysicsObj.AccelX = m_vectorList[i].X;
                        hazard.PhysicsObj.AccelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update EnvironmentalObj Accel
            countAccumulator += (int)m_countList[2];
            for (; i < countAccumulator; ++i)
            {
                foreach (EnvironmentalObj environmentalObj in GameObjectHandler.EnvironmentalObjList)
                {
                    if (environmentalObj.ObjectID == m_idList[i])
                    {
                        environmentalObj.PhysicsObj.AccelX = m_vectorList[i].X;
                        environmentalObj.PhysicsObj.AccelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update Speical EnvironmentalObj Accel
            countAccumulator += (int)m_countList[3];
            for (; i < countAccumulator; ++i)
            {
                foreach (SpecialEnvironmentalObj specialEnvironmentalObj in GameObjectHandler.SpecialEnvironmentalObjList)
                {
                    if (specialEnvironmentalObj.ObjectID == m_idList[i])
                    {
                        specialEnvironmentalObj.PhysicsObj.AccelX = m_vectorList[i].X;
                        specialEnvironmentalObj.PhysicsObj.AccelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

            #region Update ItemObj Accel
            countAccumulator += (int)m_countList[4];
            for (; i < countAccumulator; ++i)
            {
                foreach (ItemObj itemObj in GameObjectHandler.ItemObjList)
                {
                    if (itemObj.ObjectID == m_idList[i])
                    {
                        itemObj.PhysicsObj.AccelX = m_vectorList[i].X;
                        itemObj.PhysicsObj.AccelY = m_vectorList[i].Y;
                    }
                }
            }
            #endregion

        }

        #endregion
    }
}
