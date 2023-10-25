//******************************************************
// File: HazardForcePush.cs
//
// Purpose: Defines HazardForcePush. This is the
// projectile that AbilityForcePush produces.
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
    class HazardForcePush : HazardObj
    {
        //****************************************************
        // Method: NetInitialize
        //
        // Purpose: Initializes values based on the 
        // mesage sent by the server
        //****************************************************
        public override void NetInitialize(NetIncomingMessage msg)
        {
            base.NetInitialize(msg);
            if (m_direction == AnimeFightClub001.Direction.Left)
            {
                m_drawableObj.Origin = new Vector2(m_drawableObj.Origin.X - 15, m_drawableObj.Origin.Y);
            }
            else
            {
                m_drawableObj.Origin = new Vector2(m_drawableObj.Origin.X + 15, m_drawableObj.Origin.Y);
            }
        }

        //*******************************************************
        // Method: HazardForcePush
        //
        // Purpose: Default constructor for HazardForcePush
        //*******************************************************
        public HazardForcePush()
            : base()
        {

            m_physicsObj.Rect.SetDimensions(30, 80);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new Sprite("ForcePush", Color.AntiqueWhite, this, GlobalVariables.GetLayer(ObjectLayer.Hazard));
        }

        //****************************************************
        // Method: HazardForcePush
        //
        // Purpose: Constructor for HazardForcePush
        //****************************************************
        public HazardForcePush(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 0, -1, 1, 400)
        {
            m_direction = GameObjectHandler.FindPlayerObjByID(ownerID).Direction;
            m_physicsObj.Rect.SetDimensions(30, 80);
            SetStartingSpeed(speed);
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new Sprite("ForcePush", Color.AntiqueWhite, this, GlobalVariables.GetLayer(ObjectLayer.Hazard));
            if (m_direction == AnimeFightClub001.Direction.Left)
            {
                m_drawableObj.Origin = new Vector2(m_drawableObj.Origin.X - 15, m_drawableObj.Origin.Y);
            }
            else
            {
                m_drawableObj.Origin = new Vector2(m_drawableObj.Origin.X + 15, m_drawableObj.Origin.Y);
            }
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            base.Update(gameTime, viewport);
            m_physicsObj.Sided[Side.bottom] = true;
            if (m_physicsObj.Vel.X > 0)
            {
                m_direction = AnimeFightClub001.Direction.Right;
            }
            if (m_physicsObj.Vel.X < 0)
            {
                m_direction = AnimeFightClub001.Direction.Left;
            }
        }

        #region Collide Functions
        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with environmentalObjs.
        //****************************************************
        public override void CollideBefore(EnvironmentalObj environmentalObj)
        {
            base.CollideBefore(environmentalObj);
            Delete();
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // BasicObj overload.
        //****************************************************
        public override void CollideBefore(BasicObj basicObj)
        {
            float tempForce = m_physicsObj.Vel.X + ((m_physicsObj.Vel.X / m_physicsObj.Vel.X) * 100);
            if (Math.Abs(basicObj.PhysicsObj.VelX) > Math.Abs(tempForce))
            {
                if ((m_direction == AnimeFightClub001.Direction.Right && basicObj.PhysicsObj.Vel.X < 0) || (m_direction == AnimeFightClub001.Direction.Left && basicObj.PhysicsObj.Vel.X > 0))
                {
                    basicObj.PhysicsObj.VelX = -basicObj.PhysicsObj.VelX;
                }
            }
            else
            {
                basicObj.PhysicsObj.VelX = tempForce;
            }
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // PlayerObj overload.
        //****************************************************
        public override void CollideBefore(PlayerObj player)
        {
            player.StatusHandler.Inflict(StatusName.KnockBack, (int)(m_physicsObj.Vel.X + ((m_physicsObj.Vel.X / m_physicsObj.Vel.X) * 100) * 3), m_ownerID);
            player.StatusHandler.Inflict(StatusName.Stun, 400, m_ownerID);
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // HazardObj overload.
        //****************************************************
        public override void CollideBefore(HazardObj hazard)
        {
            float tempForce = m_physicsObj.Vel.X + ((m_physicsObj.Vel.X / m_physicsObj.Vel.X) * 100);
            if (Math.Abs(hazard.PhysicsObj.VelX) > Math.Abs(tempForce))
            {
                if ((m_direction == AnimeFightClub001.Direction.Right && hazard.PhysicsObj.Vel.X < 0) || (m_direction == AnimeFightClub001.Direction.Left && hazard.PhysicsObj.Vel.X > 0))
                {
                    hazard.PhysicsObj.VelX = -hazard.PhysicsObj.VelX;
                }
            }
            else
            {
                hazard.PhysicsObj.VelX = tempForce;
            }
            hazard.OwnerID = m_ownerID;
            hazard.ResetDespawnClock();
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // SpecialEnvironmentalObj overload.
        //****************************************************
        public override void CollideBefore(SpecialEnvironmentalObj specialEnvironmentalObj)
        {
            float tempForce = m_physicsObj.Vel.X + ((m_physicsObj.Vel.X / m_physicsObj.Vel.X) * 100);
            if (Math.Abs(specialEnvironmentalObj.PhysicsObj.VelX) > Math.Abs(tempForce))
            {
                if ((m_direction == AnimeFightClub001.Direction.Right && specialEnvironmentalObj.PhysicsObj.Vel.X < 0) || (m_direction == AnimeFightClub001.Direction.Left && specialEnvironmentalObj.PhysicsObj.Vel.X > 0))
                {
                    specialEnvironmentalObj.PhysicsObj.VelX = -specialEnvironmentalObj.PhysicsObj.VelX;
                }
            }
            else
            {
                specialEnvironmentalObj.PhysicsObj.VelX = tempForce;
            }
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with a particular
        // type of object.
        // 
        // ItemObj overload.
        //****************************************************
        public override void CollideBefore(ItemObj item)
        {
            float tempForce = m_physicsObj.Vel.X + ((m_physicsObj.Vel.X / m_physicsObj.Vel.X) * 100);
            if (Math.Abs(item.PhysicsObj.VelX) > Math.Abs(tempForce))
            {
                if ((m_direction == AnimeFightClub001.Direction.Right && item.PhysicsObj.Vel.X < 0) || (m_direction == AnimeFightClub001.Direction.Left && item.PhysicsObj.Vel.X > 0))
                {
                    item.PhysicsObj.VelX = -item.PhysicsObj.VelX;
                }
            }
            else
            {
                item.PhysicsObj.VelX = tempForce;
            }
        }
        #endregion
    }
}