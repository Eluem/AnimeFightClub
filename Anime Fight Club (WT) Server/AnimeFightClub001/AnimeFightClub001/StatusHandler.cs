//******************************************************
// File: StatusHandler.cs
//
// Purpose: Contains the definition of the
// StatusHandler. An instance of this object
// will be definied in each PlayerObj and will be used
// similar to the GameObjHandler.
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
    class StatusHandler
    {
        #region Declarations
        protected PlayerObj m_ownerObj;
        protected List<Status> m_statusList;
        protected List<Status> m_statusAddList;
        protected List<Status> m_statusDeleteList;

        protected UInt16 m_statusIDCounter;
        #endregion

        //****************************************************
        // Method: NetSendMe
        //
        // Purpose: Accepts a NetOutgoingMessage and writes
        // the approraite data to it
        //****************************************************
        public void NetSendMe(NetOutgoingMessage sendMsg)
        {
            if (m_statusAddList.Count > 0)
                System.Console.Write("FUUU");

            sendMsg.Write(m_statusList.Count);

            foreach (Status status in m_statusList)
            {
                sendMsg.Write((byte)status.Name);
                sendMsg.Write(status.Severity);
                sendMsg.Write(status.Duration);
                sendMsg.Write(status.StatusID);
                if (status.InflictorObj != null)
                {
                    sendMsg.Write(status.InflictorObj.ObjectID);
                }
                else
                {
                    sendMsg.Write(0);
                }
            }
        }

        //****************************************************
        // Method: StatusHandler
        //
        // Purpose: Default constructor for StatusHandler
        //****************************************************
        public StatusHandler(PlayerObj ownerObj)
        {
            m_ownerObj = ownerObj;
            m_statusList = new List<Status>();
            m_statusDeleteList = new List<Status>();
            m_statusAddList = new List<Status>();

            m_statusIDCounter = 1;
        }

        //****************************************************
        // Method: Inflict
        //
        // Purpose: Inflicts damage or status effects.
        //****************************************************
        public void Inflict(StatusName status, int severity, UInt16 inflictorID)
        {
            UInt16 statusID = GenerateID();
            PlayerObj inflictorObj = GameObjectHandler.FindPlayerObjByID(inflictorID);
            switch (status)
            {
                case StatusName.Damage:
                    if (!HasStatus(StatusName.ShieldAbility) || severity < 0)
                    {
                        if (severity >= m_ownerObj.HP)
                        {
                            if (m_ownerObj.HP > 0)
                            {
                                m_ownerObj.HP = 0;
                                Inflict(StatusName.Death, 5000, inflictorID);
                            }
                        }
                        else
                        {
                            m_ownerObj.HP -= severity;
                            if (m_ownerObj.HP > 100)
                            {
                                m_ownerObj.HP = 100;
                            }
                        }
                    }
                    break;
                case StatusName.Poison:
                    if (!HasStatus(StatusName.ShieldAbility))
                    {
                        AddStatus(new StatusPoison(statusID, inflictorObj, m_ownerObj, severity, 60000, true));
                    }
                    break;
                case StatusName.Death:
                    if (!HasStatus(StatusName.ShieldAbility))
                    {
                        HazardHealingOrb tempHealingOrb = new HazardHealingOrb(0);
                        tempHealingOrb.PhysicsObj.VelY = (400 * (float)GlobalVariables.Randomizer.NextDouble()) - 200;
                        tempHealingOrb.PhysicsObj.VelX = (400 * (float)GlobalVariables.Randomizer.NextDouble()) - 200;
                        tempHealingOrb.SetPos(m_ownerObj.Center.X, m_ownerObj.PhysicsObj.Rect.Top - 10);
                        GameObjectHandler.AddObject(tempHealingOrb);

                        GameObjectHandler.AddObject(new DeadPlayer(m_ownerObj));
                        AddStatus(new StatusDeath(statusID, inflictorObj, m_ownerObj, severity, severity));
                        if (inflictorID != m_ownerObj.ObjectID && inflictorObj != null)
                        {
                            ++inflictorObj.Kills;
                        }
                    }
                    break;
                case StatusName.Stun:
                    if (!HasStatus(StatusName.ShieldAbility))
                    {
                        m_ownerObj.Loadout.Interrupt();
                        AddStatus(new StatusStun(statusID, inflictorObj, m_ownerObj, severity, severity));
                    }
                    break;
                case StatusName.ShieldAbility:
                    AddStatus(new StatusShieldAbility(statusID, inflictorObj, m_ownerObj, severity, severity));
                    break;
                case StatusName.KnockBack:
                    AddStatus(new StatusKnockBack(statusID, inflictorObj, m_ownerObj, severity, 300));
                    break;
            }
            /*
            if (status != StatusName.Damage)
            {
                NetworkingHandler.SendAddStatusMessage(m_ownerObj.ObjectID, status, severity, statusID, inflictorID);
            }
            */
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object
        //****************************************************
        public void Update(GameTime gameTime)
        {
            #region Add/Delete Statuses
            //Delete all statuses queued to be deleted
            foreach (Status status in m_statusDeleteList)
            {
                m_statusList.Remove(status);
            }
            m_statusDeleteList.Clear();

            //Add all statuses queued to be added
            foreach (Status status in m_statusAddList)
            {
                m_statusList.Add(status);
            }
            m_statusAddList.Clear();
            #endregion

            ApplyStatusEffects(gameTime);
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws all the the statuses that this
        // player is affected by
        //****************************************************
        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        //****************************************************
        // Method: ApplyStatusEffects
        //
        // Purpose: Applies all status effects afflicting
        // the player.  Also deletes any expired statuses.
        //****************************************************
        public void ApplyStatusEffects(GameTime gameTime)
        {
            foreach (Status status in m_statusList)
            {
                status.Update(gameTime);

                if (status.Duration <= 0)
                {
                    status.WearOffHook(gameTime);
                    DeleteStatus(status);
                }
            }
        }

        //****************************************************
        // Method: FindStatus
        //
        // Purpose: Searches the status list for
        // the first instance of a particular status.
        // Returns the status if it was found.
        // Otherwise it returns null.
        //****************************************************
        public Status FindStatus(StatusName name)
        {
            foreach (Status status in m_statusList)
            {
                if (status.Name == name)
                    return status;
            }
            return null;
        }

        //****************************************************
        // Method: FindStatusByID
        //
        // Purpose: Searches the list of statuses for a
        // status with a particular id. If it is found, it
        // is returned. Otherwise, null is returned.
        //****************************************************
        public Status FindStatusByID(UInt16 id)
        {
            foreach (Status status in m_statusList)
            {
                if (status.StatusID == id)
                    return status;
            }
            return null;
        }

        //****************************************************
        // Method: HasStatus
        //
        // Purpose: Searches the status list for
        // the first instance of a particular status.
        // Returns true if the status was found.
        // Otherwise it returns false
        //****************************************************
        public bool HasStatus(StatusName name)
        {
            return FindStatus(name) != null;
        }

        //****************************************************
        // Method: AddStatus
        //
        // Purpose: Queues a status for addition
        //****************************************************
        public void AddStatus(Status status)
        {
            m_statusAddList.Add(status);
            if (status.Name != StatusName.Damage)
            {
                NetworkingHandler.SendAddStatusMessage(m_ownerObj.ObjectID, status.Name, status.Severity, status.StatusID, status.InflictorObj);
            }
        }

        //****************************************************
        // Method: DeleteStatus
        //
        // Purpose: Queues a status for deletion
        //****************************************************
        public void DeleteStatus(Status status)
        {
            m_statusDeleteList.Add(status);
            NetworkingHandler.SendDeleteStatusMessage(status);
        }

        //****************************************************
        // Method: Clear
        //
        // Purpose: Clear's all of the player's statuses
        //****************************************************
        public void Clear()
        {
            foreach (Status status in m_statusList)
            {
                m_statusDeleteList.Add(status);
            }
            m_statusAddList.Clear();
        }

        //****************************************************
        // Method: GenerateID
        //
        // Purpose: Produces an ID for a status, then
        // increments the status counter.
        //****************************************************
        public UInt16 GenerateID()
        {
            return m_statusIDCounter++;
        }

        #region Properties
        #endregion
    }
}
