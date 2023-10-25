//******************************************************
// File: HazardSwordSlashStrong.cs
//
// Purpose: Defines HazardSwordSlashStrong. This is the
// projectile that AbilitySword produces.
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

namespace AnimeFightClub001
{
    class HazardSwordSlashStrong : HazardObj
    {
        //*********************************************************
        // Method: HazardSwordSlashStrong
        //
        // Purpose: Default constructor for HazardSwordSlashStrong
        //*********************************************************
        public HazardSwordSlashStrong()
            : base()
        {

            m_physicsObj.Rect.SetDimensions(30, 80);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new DrawableObj("ForcePush", Color.Red, this);
        }

        //****************************************************
        // Method: HazardSwordSlashStrong
        //
        // Purpose: Constructor for HazardSwordSlashStrong
        //****************************************************
        public HazardSwordSlashStrong(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 45, -1, 1, 70)
        {
            m_ownerObj = GameObjectHandler.FindPlayerObjByID(ownerID);
            m_direction = m_ownerObj.Direction;
            m_physicsObj.VelY = m_ownerObj.Vel.Y;

            m_physicsObj.Rect.SetDimensions(30, 80);
            SetStartingSpeed(speed);
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new DrawableObj("ForcePush", Color.Red, this);
            if (m_direction == AnimeFightClub001.Direction.Left)
            {
                m_drawableObj.Origin = new Vector2(m_drawableObj.Origin.X - 15, m_drawableObj.Origin.Y);
            }
            else
            {
                m_drawableObj.Origin = new Vector2(m_drawableObj.Origin.X + 15, m_drawableObj.Origin.Y);
                m_drawableObj.SpriteEffect = SpriteEffects.FlipHorizontally;
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
        // HazardObj overload.
        //****************************************************
        public override void CollideBefore(HazardObj hazard)
        {
            if (hazard.GetType().ToString().Split('.')[1] == "HazardBombExplosion" || hazard.GetType().ToString().Split('.')[1] == "HazardPoisonSpikeTrap" || hazard.GetType().ToString().Split('.')[1] == "HazardSpikeTrap")
                return;
            hazard.PhysicsObj.VelX = m_physicsObj.Vel.X + ((m_physicsObj.Vel.X / m_physicsObj.Vel.X) * 100);
            hazard.OwnerID = m_ownerID;
            hazard.ResetDespawnClock();
        }
        #endregion
    }
}