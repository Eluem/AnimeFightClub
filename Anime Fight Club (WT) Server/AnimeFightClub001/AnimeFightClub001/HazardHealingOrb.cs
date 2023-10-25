//******************************************************
// File: HazardHealingOrb.cs
//
// Purpose: Droped when player dies. Heals player
// when picked up
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
    class HazardHealingOrb : HazardObj
    {

        //****************************************************
        // Method: HazardHealingOrb
        //
        // Purpose: Default constructor for HazardHealingOrb
        //****************************************************
        public HazardHealingOrb()
            : base()
        {
            m_physicsObj.Rect.SetDimensions(25, 25);
            m_physicsObj.Friction = 1f;
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = .6f;
        }

        //****************************************************
        // Method: HazardHealingOrb
        //
        // Purpose: Constructor for HazardHealingOrb
        //****************************************************
        public HazardHealingOrb(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, -50, -1, 1, 10000)
        {
            m_physicsObj.Rect.SetDimensions(25, 25);
            m_physicsObj.Friction = 1f;
            m_physicsObj.Restitution = .6f;
        }
        #region Collision Functions
        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Reduces durability on contact with 
        // a player.
        //****************************************************
        public override void CollideBefore(SpecialEnvironmentalObj target)
        {
        }
        #endregion
    }
}
