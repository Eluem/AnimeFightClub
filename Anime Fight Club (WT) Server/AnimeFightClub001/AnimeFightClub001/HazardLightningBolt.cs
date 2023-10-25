//******************************************************
// File: HazardLightningBolt.cs
//
// Purpose: Defines HazardLightningBolt. This is the
// projectile that AbilityLightningBolt produces.
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
    class HazardLightningBolt : HazardObj
    {
        #region Declarations
        protected int m_flightTimer; //stores how long the lightningbolt has been traveling
        #endregion

        //*******************************************************
        // Method: HazardLightningBolt
        //
        // Purpose: Default constructor for HazardLightningBolt
        //*******************************************************
        public HazardLightningBolt()
            : base()
        {

            m_physicsObj.Rect.SetDimensions(135, 10);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;
            m_flightTimer = 0;

            m_drawableObj.SetDimensions(140, 14);
        }

        //****************************************************
        // Method: HazardLightningBolt
        //
        // Purpose: Constructor for HazardLightningBolt
        //****************************************************
        public HazardLightningBolt(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 15, -1, 1, 3000)
        {
            m_physicsObj.Rect.SetDimensions(135, 10);
            SetStartingSpeed(speed);
            m_physicsObj.Restitution = 0f;
            m_flightTimer = 0;

            m_drawableObj.SetDimensions(140, 14);
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

            m_flightTimer += gameTime.ElapsedGameTime.Milliseconds;
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
        // Purpose: Handles collision with players
        //****************************************************
        public override void CollideBefore(PlayerObj target)
        {
            base.CollideBefore(target);
            target.StatusHandler.Inflict(StatusName.Stun, 1500, m_ownerID);
        }
        #endregion
    }
}
