//******************************************************
// File: HazardPoisonSpikeTrap.cs
//
// Purpose: Defines HazardPoisonSpikeTrap.
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
    class HazardPoisonSpikeTrap : HazardObj
    {
        #region Declarations
        int m_existFrames; //Time the trap has existed
        #endregion

        //*******************************************************
        // Method: HazardPoisonSpikeTrap
        //
        // Purpose: Default constructor for HazardPoisonSpikeTrap
        //*******************************************************
        public HazardPoisonSpikeTrap()
            : base()
        {
            m_existFrames = 0;
            m_physicsObj.Rect.SetDimensions(80, 80);
            m_physicsObj.VelX = 0;
            m_physicsObj.Restitution = 0f;
        }

        //****************************************************
        // Method: HazardPoisonSpikeTrap
        //
        // Purpose: Constructor for HazardSpikeTrap
        //****************************************************
        public HazardPoisonSpikeTrap(float speed, UInt16 ownerID = 0, string ownerType = "World")
            : base(ownerID, ownerType, 0, -1, 1, 45000)
        {
            m_existFrames = 0;
            m_physicsObj.Rect.SetDimensions(80, 80);

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
            if (m_state == 0)
            {
                m_physicsObj.Update(gameTime, viewport);
            }
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

            if (m_existFrames < 2)
            {
                ++m_existFrames;
                if (m_existFrames >= 2)
                {
                    m_damage = 20;
                }
            }

            if (m_state == 0)
            {
                m_drawableObj.Rotation += .1f;
            }
            m_drawableObj.Update(gameTime);
        }

        //****************************************************
        // Method: Draw
        //
        // Purpose: Draws the appropraite image to the
        // rectangle, with the appropriate color
        //****************************************************
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (m_existFrames >= 2)
                m_drawableObj.Draw(spriteBatch);
        }

        #region Collide Functions
        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with environmentalObjs.
        //****************************************************
        public override void CollideBefore(EnvironmentalObj environmentalObj)
        {   
            if (m_existFrames < 2)
            {
                if (m_ownerType == "Player")
                {
                    PlayerObj tempPlayer = GameObjectHandler.FindPlayerObjByID(m_ownerID);
                    if (tempPlayer != null)
                    {
                        tempPlayer.MP += 5000;
                    }
                }
                Delete();
            }
            else
            {
                base.CollideBefore(environmentalObj);
                if (m_state == 0)
                {
                    m_solidity = HazardSolidity.None;
                    m_state = 1;
                    //m_damage = 35;
                }
            }
        }

        //****************************************************
        // Method: CollideBefore
        //
        // Purpose: Handles collision with players
        //****************************************************
        public override void CollideBefore(PlayerObj target)
        {
            base.CollideBefore(target);
            if (m_state == 1)
            {
                target.StatusHandler.Inflict(StatusName.Poison, 17, m_ownerID);
            }
        }
        #endregion

    }
}
