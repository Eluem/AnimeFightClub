//******************************************************
// File: HazardEnergyBlade.cs
//
// Purpose: Defines HazardEnergyBlade. This is the
// projectile that AbilityEnergyBlade produces.
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
    class HazardEnergyBlade : HazardObj
    {
        #region Declarations
        protected int m_flightDuration;
        #endregion
        //*******************************************************
        // Method: HazardEnergyBlade
        //
        // Purpose: Default constructor for HazardEnergyBlade
        //*******************************************************
        public HazardEnergyBlade()
            : base()
        {
            m_flightDuration = 0;
            m_physicsObj.Rect.SetDimensions(30, 80);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new DrawableObj("ForcePush", Color.Green, this);
        }

        //****************************************************
        // Method: HazardEnergyBlade
        //
        // Purpose: Constructor for HazardEnergyBlade
        //****************************************************
        public HazardEnergyBlade(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 0, -1, 1, 1000)
        {
            m_flightDuration = 0;
            m_direction = GameObjectHandler.FindPlayerObjByID(ownerID).Direction;
            m_physicsObj.Rect.SetDimensions(30, 80);
            SetStartingSpeed(speed);
            m_physicsObj.Restitution = 0f;

            m_drawableObj = new DrawableObj("ForcePush", Color.Green, this);
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
            m_flightDuration += gameTime.ElapsedGameTime.Milliseconds;
            m_damage = m_flightDuration / 20;
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
        #endregion
    }
}