//******************************************************
// File: HazardBombExplosion.cs
//
// Purpose: Defines HazardBombExplosion.
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
    class HazardBombExplosion : HazardObj
    {
        #region Declarations
        List<PlayerObj> m_playersAlreadyHit = new List<PlayerObj>();
        #endregion
        //*******************************************************
        // Method: HazardBombExplosion
        //
        // Purpose: Default constructor for HazardBombExplosion
        //*******************************************************
        public HazardBombExplosion()
            : base()
        {
            m_physicsObj.Rect.SetDimensions(150, 200);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;
        }

        //****************************************************
        // Method: HazardBombExplosion
        //
        // Purpose: Constructor for HazardBombExplosion
        //****************************************************
        public HazardBombExplosion(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 40, -1, 5000, 520)
        {
            m_physicsObj.Rect.SetDimensions(150, 200);

            SetStartingSpeed(speed);

            m_physicsObj.Restitution = 0f;
        }

        //****************************************************
        // Method: Update
        //
        // Purpose: Updates the object
        //****************************************************
        public override void Update(GameTime gameTime, Viewport viewport)
        {
            m_physicsObj.VelX = 0;
            m_physicsObj.VelY = 0;
            base.Update(gameTime, viewport);
            m_physicsObj.VelX = 0;
            m_physicsObj.VelY = 0;
            m_drawableObj.Update(gameTime);
        }

        #region Collide Functions
        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with players
        //****************************************************
        public override void CollideBefore(PlayerObj target)
        {
            base.CollideBefore(target);
            if (!m_playersAlreadyHit.Contains(target))
            {
                m_playersAlreadyHit.Add(target);
                target.StatusHandler.Inflict(StatusName.Damage, m_damage, m_ownerID);
            }
        }
        #endregion
    }
}
