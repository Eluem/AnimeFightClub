//******************************************************
// File: Hazard.cs
//
// Purpose: Contains the definition of the
// Hazard Object type. This type of object will be
// used to define most, if not all, of the objects in
// the game which damage (or even heal) players and
// potentially other objects. This will be the class
// inhereted by types such as FireBall, LaserBeam,
// Explosion, TrapCaltropHazard, TrapRazorHazard, ect.
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
    //*********************************************************************
    // Indicates what objects the hazard makes physical collisions with
    // i.e. momentum transfer. Full indicates everything, semi indicates
    // everything but players, and none indicates no momentum transfer
    // with any object type. May add more?
    //*********************************************************************
    enum HazardSolidity { Full, Semi, None };
    class HazardObj : BasicObj
    {
        #region Declarations
        protected int m_damage; //The damage dealt on contact, before defense is applied.
        protected int m_baseDamage; //The hazard's base damage.  Set by an inherited hazard.
        protected int m_hp; //The HP value of the hazard, -1 if invulnerable to HP damage.
        protected int m_durability; //The maximum number of times the hazard can inflict damage, -1 if unlimited.
        protected int m_safeClock; //The time that the owner of this hazard is safe from colliding with it at all
        protected HazardSolidity m_solidity; //Indicates what object types a hazard collides with
        #endregion

        #region Constants
        const int SAFETIME = 300; //Indicates how many miliseconds after a player spawns a hazard before it'll be able to hit them
        #endregion

        //****************************************************
        // Method: NetSendMe
        //
        // Purpose: Accepts a NetOutgoingMessage and writes
        // the approraite data to it
        //****************************************************
        public override void NetSendMe(NetOutgoingMessage sendMsg)
        {
            base.NetSendMe(sendMsg);

            //Send damage
            sendMsg.Write(m_damage);

            //Send hp
            sendMsg.Write(m_hp);

            //Send durability
            sendMsg.Write(m_durability);

            //Send solidity
            sendMsg.Write((byte)m_solidity);
        }

        //****************************************************
        // Method: HazardObj
        //
        // Purpose: Default constructor for HazardObj
        //****************************************************
        public HazardObj()
            : base()
        {
            m_damage = 0;
            m_hp = 0;
            m_durability = 0;

            m_safeClock = SAFETIME;

            m_solidity = HazardSolidity.Semi;
        }

        //****************************************************
        // Method: HazardObj
        //
        // Purpose: Constructor for HazardObj
        //****************************************************
        public HazardObj(UInt16 ownerID = 0, string ownerType = "World", int damage = 0, Int16 hp = 0, Int16 durability = 0, int despawnClock = -1, HazardSolidity solidity = HazardSolidity.Semi)
            : base(ownerID, ownerType, despawnClock)
        {
            m_damage = damage;// *m_baseDamage;
            m_hp = hp;
            m_durability = durability;

            m_safeClock = SAFETIME;

            m_solidity = solidity;
        }

        //****************************************************
        // Method: Delete
        //
        // Purpose: Deletes the object by adding it to the
        // appropraite delete list and setting the deleted
        // boolean to true.
        //****************************************************
        public override void Delete()
        {
            m_deleted = true;
            GameObjectHandler.DeleteObject(this);
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the details of the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            base.Update(gameTime, viewport);
            if (m_despawnClock >= 0)
            {
                m_despawnClock -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                Delete();
            }

            if (m_safeClock >= 0)
            {
                m_safeClock -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        //****************************************************
        // Method: SetStartingSpeed
        //
        // Purpose: Sets the starting velocity of the
        // object based on the ownerObj's facing direction
        //****************************************************
        public virtual void SetStartingSpeed(float speed)
        {
            if (m_ownerType == "Player")
            {
                PlayerObj tempPlayer = GameObjectHandler.FindPlayerObjByID(m_ownerID);
                if (tempPlayer != null)
                {
                    if (tempPlayer.Direction == Direction.Right)
                    {
                        m_physicsObj.VelX = speed;
                        SetPos(tempPlayer.PhysicsObj.Rect.Right + 2, tempPlayer.PhysicsObj.Rect.Top + 40 - m_physicsObj.Rect.Height / 2);
                    }
                    else
                    {
                        m_physicsObj.VelX = -speed;
                        SetPos(tempPlayer.PhysicsObj.Rect.Left - 2 - m_physicsObj.Rect.Width, tempPlayer.PhysicsObj.Rect.Top + 40 - m_physicsObj.Rect.Height / 2);
                    }
                }
            }
            else
            {
                m_physicsObj.VelX = speed;
            }
        }

        #region Collide Functions
        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Reduces durability on contact with 
        // a player.
        //****************************************************
        public override void CollideBefore(PlayerObj target)
        {

            m_durability--;

            if (m_durability == 0)
            {
                Delete();
            }

        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Takes HP damage when attacked.
        //****************************************************
        public override void CollideBefore(HazardObj hazard)
        {
            if (m_hp != -1)
            {
                if (m_hp < hazard.Damage)
                {
                    Delete();
                }
                else
                {
                    m_hp -= hazard.Damage;
                }
            }
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Reduces durability on contact with 
        // a player.
        //****************************************************
        public override void CollideBefore(SpecialEnvironmentalObj target)
        {
            if (target.GetType().ToString().Split('.')[1] == "DeadPlayer")
            {
                m_durability--;

                if (m_durability == 0)
                {
                    Delete();
                }
            }
        }
        #endregion


        #region Properties
        public int Damage
        {
            get
            {
                return m_damage;
            }
            set
            {
                m_hp = value;
            }
        }

        public int HP
        {
            get
            {
                return m_hp;
            }
            set
            {
                m_hp = value;
            }
        }

        public int Durability
        {
            get
            {
                return m_durability;
            }
            set
            {
                m_durability = value;
            }
        }

        public int SafeClock
        {
            get
            {
                return m_safeClock;
            }
            set
            {
                m_safeClock = value;
            }
        }

        public HazardSolidity Solidity
        {
            get
            {
                return m_solidity;
            }
            set
            {
                m_solidity = value;
            }
        }
        #endregion


    }
}
