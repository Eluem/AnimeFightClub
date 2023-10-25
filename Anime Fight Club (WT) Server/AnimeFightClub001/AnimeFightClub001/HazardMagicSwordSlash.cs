﻿//******************************************************
// File: HazardMagicSwordSlash.cs
//
// Purpose: Defines HazardMagicSwordSlash. This is the
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
using Lidgren.Network;

namespace AnimeFightClub001
{
    class HazardMagicSwordSlash : HazardObj
    {


        //*******************************************************
        // Method: HazardMagicSwordSlash
        //
        // Purpose: Default constructor for HazardMagicSwordSlash
        //*******************************************************
        public HazardMagicSwordSlash()
            : base()
        {

            m_physicsObj.Rect.SetDimensions(30, 80);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;

        }

        //****************************************************
        // Method: HazardMagicSwordSlash
        //
        // Purpose: Constructor for HazardMagicSwordSlash
        //****************************************************
        public HazardMagicSwordSlash(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 30, -1, 1, 70)
        {
            m_ownerObj = GameObjectHandler.FindPlayerObjByID(ownerID);
            m_direction = m_ownerObj.Direction;
            m_physicsObj.VelY = m_ownerObj.Vel.Y;
            m_physicsObj.Rect.SetDimensions(30, 80);
            SetStartingSpeed(speed);
            m_physicsObj.Restitution = 0f;

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
        // HazardObj overload.
        //****************************************************
        public override void CollideBefore(HazardObj hazard)
        {
            if (hazard.GetType().ToString().Split('.')[1] == "HazardBombExplosion" || hazard.GetType().ToString().Split('.')[1] == "HazardPoisonSpikeTrap" || hazard.GetType().ToString().Split('.')[1] == "HazardSpikeTrap")
                return;
            hazard.Delete();
        }
        #endregion
    }
}