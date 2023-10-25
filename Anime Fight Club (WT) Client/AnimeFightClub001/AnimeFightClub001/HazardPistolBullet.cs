//******************************************************
// File: HazardPistolBullet.cs
//
// Purpose: Defines HazardPistolBullet.
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
    class HazardPistolBullet : HazardObj
    {

        //*******************************************************
        // Method: HazardPistolBullet
        //
        // Purpose: Default constructor for HazardPistolBullet
        //*******************************************************
        public HazardPistolBullet()
            : base()
        {

            m_physicsObj.Rect.SetDimensions(5, 5);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new Sprite("PistolBullet", Color.White, this, GlobalVariables.GetLayer(ObjectLayer.Hazard));
            m_drawableObj.SetDimensions(10, 10);
        }

        //****************************************************
        // Method: HazardPistolBullet
        //
        // Purpose: Constructor for HazardPistolBullet
        //****************************************************
        public HazardPistolBullet(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 25, -1, 1, 10000)
        {
            m_physicsObj.Rect.SetDimensions(5, 5);
            SetStartingSpeed(speed);
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new Sprite("PistolBullet", Color.White, this, GlobalVariables.GetLayer(ObjectLayer.Hazard));
            m_drawableObj.SetDimensions(10, 10);
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
        }
        #endregion
    }
}