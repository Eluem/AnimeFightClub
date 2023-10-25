//******************************************************
// File: PacketBuffer.cs
//
// Purpose: The PacketBuffer class will be used to
// store packets that have been read in.
// Based on the time stamp of the packet it will either
// be dropped, used, or held until it is useable.
// This should help to reduce jitter.
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
    class PacketBuffer
    {
        #region Declarations
        LinkedList<VectListUpdatePacket> m_positionPacketBuffer;
        LinkedList<VectListUpdatePacket> m_velocityPacketBuffer;
        LinkedList<VectListUpdatePacket> m_accelerationPacketBuffer;
        #endregion

        //****************************************************
        // Method: PacketBuffer
        //
        // Purpose: Default PacketBuffer constructor
        //****************************************************
        public PacketBuffer()
        {
            #region Initializations
            m_positionPacketBuffer = new LinkedList<VectListUpdatePacket>();
            m_velocityPacketBuffer = new LinkedList<VectListUpdatePacket>();
            m_accelerationPacketBuffer = new LinkedList<VectListUpdatePacket>();
            #endregion
        }


        //****************************************************
        // Method: ReadPacket
        //
        // Purpose: Reads packet into the appropriate 
        // buffer list.
        //****************************************************
        public void ReadPacket(PacketType packetType, NetIncomingMessage msg)
        {
            switch (packetType)
            {
                case PacketType.PositionUpdate:
                    m_positionPacketBuffer.AddLast(new VectListUpdatePacket(msg));
                    break;

                case PacketType.VelocityUpdate:
                    m_velocityPacketBuffer.AddLast(new VectListUpdatePacket(msg));
                    break;

                case PacketType.AccelerationUpdate:
                    m_accelerationPacketBuffer.AddLast(new VectListUpdatePacket(msg));
                    break;
            }
        }

        //****************************************************
        // Method: ApplyVectListBuffers
        //
        // Purpose: Runs through all VectListUpdatePacket
        // buffer lists (Pos, Vel, and Accel) and applies
        // the ones that should be applied during
        // this frame, if any.
        //****************************************************
        public void ApplyVectListBuffers()
        {
            #region Declarations/Initializations
            //Lists used to store the packets which have been applied. This is to allow them to be deleted at the
            // end of the loop
            LinkedList<VectListUpdatePacket> appliedPositionPackets = new LinkedList<VectListUpdatePacket>();
            LinkedList<VectListUpdatePacket> appliedVelocityPackets = new LinkedList<VectListUpdatePacket>();
            LinkedList<VectListUpdatePacket> appliedAccelerationPackets = new LinkedList<VectListUpdatePacket>();
            #endregion

            //NOTE: These need to be changed so that they only apply packets that should be applied!!!!!!! (When should old packets be dropped)
            //(Should they be dropped?) (how to determine if they're old.)
            #region Apply Packets

            #region Apply Position Packets
            foreach (VectListUpdatePacket positionUpdatePacket in m_positionPacketBuffer)
            {
                positionUpdatePacket.ApplyPacketToPos();
                appliedPositionPackets.AddLast(positionUpdatePacket);
            }
            #endregion

            #region Apply Velocity Packets
            foreach (VectListUpdatePacket velocityUpdatePacket in m_velocityPacketBuffer)
            {
                velocityUpdatePacket.ApplyPacketToVel();
                appliedVelocityPackets.AddLast(velocityUpdatePacket);
            }
            #endregion

            #region Apply Acceleration Packets
            foreach (VectListUpdatePacket accelerationUpdatePacket in m_accelerationPacketBuffer)
            {
                accelerationUpdatePacket.ApplyPacketToAccel();
                appliedAccelerationPackets.AddLast(accelerationUpdatePacket);
            }
            #endregion

            #endregion

            #region Remove Applied Packets

            #region Remove Applied Position Packets
            foreach (VectListUpdatePacket positionUpdatePacket in appliedPositionPackets)
            {
                m_positionPacketBuffer.Remove(positionUpdatePacket);
            }
            #endregion

            #region Remove Applied Velocity Packets
            foreach (VectListUpdatePacket velocityUpdatePacket in appliedVelocityPackets)
            {
                m_velocityPacketBuffer.Remove(velocityUpdatePacket);
            }
            #endregion

            #region Remove Applied Acceleration Packets
            foreach (VectListUpdatePacket accelerationUpdatePacket in appliedAccelerationPackets)
            {
                m_accelerationPacketBuffer.Remove(accelerationUpdatePacket);
            }
            #endregion

            #endregion
        }

        #region Properties
        #endregion
    }
}
